using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

namespace SystemicDesign
{
    /// <summary>
    /// Editor que permite lleva un seguimiento en profundidad de toda la jerarquía sistémica.
    /// Permitiendo crear, modificar y elminar toda la jerarquía, nuevos sistemas y estímulos.
    /// También permite comprobar la integridad de la jerarquía solucionando algunos problemas que
    /// pudiera haber en ella.
    /// </summary>
    [InitializeOnLoad]
    public class SystemicEditor : EditorWindow
    {
        /// <summary>
        /// Referencia local al sistema raíz para poder tener accesible toda la jerarquía sistémica
        /// </summary>
        private static RootSystem rootSystem;
        /// <summary>
        /// Referencia local a esta misma ventana para inicialización
        /// </summary>
        private static SystemicEditor window;
        /// <summary>
        /// String usado para guardar el nombre del nuevo estímulo que se va a crear.
        /// </summary>
        private string newStimulusName = "";
        /// <summary>
        /// Vector usado para controllar la posición de scroll en el editor.
        /// </summary>
        private Vector2 scroll;
        /// <summary>
        /// Vector usado para controllar la posición de scroll en la caja de diasnóstico
        /// </summary>
        private Vector2 diagScroll;
        /// <summary>
        /// Booleano usado para el foldout que guarda la jerarquía de sistemas
        /// </summary>
        private bool foldoutHierarchy = false;
        /// <summary>
        /// Booleano usado para el foldout que guarda las estadísticas del sistema
        /// </summary>
        private bool foldoutStats = false;
        /// <summary>
        /// Booleano usado para el foldout que guarda la parte del editor destinada a los estímulos
        /// </summary>
        private bool stimulusFoldout;
        /// <summary>
        /// Referencia al sistema actualmente seleccionado en el editor para que se pueda borrar
        /// </summary>
        private System actualSystem;
        /// <summary>
        /// Estilo de GUI destinado a los foldouts
        /// </summary>
        private static GUIStyle foldoutStyle;
        /// <summary>
        /// Estilo de GUI destinado a los mensajes de error de la caja de diagnóstico
        /// </summary>
        private static GUIStyle labelErrorStyle;
        /// <summary>
        /// Lista de strings que guarda todos los mensajes generados por el último diagnóstico
        /// </summary>
        private List<string> lastDiagnosis = new List<string>();
        /// <summary>
        /// Booleano que determina si hay un nuevo diagnóstico que deba mostrarse
        /// </summary>
        private bool diagnosisDone = false;
        /// <summary>
        /// Índice que determina qué sistema se está eligiendo en el editor en este momento
        /// </summary>
        int selectedSystemIndex = -1;
        /// <summary>
        /// Booleano que determina si se ha elegido o lo el borrado recursivo de sistemas.
        /// En este modo no solo se borra el sistema sino que también todas las subentidades
        /// </summary>
        bool recursiveElimination = false;

        /// <summary>
        /// Esto es para que poder pasar las funciones del editor sistémico como
        /// parámetros a los popup que este cree
        /// </summary>
        public delegate void ResponsePopup();


        /// <summary>
        /// Muestra la ventana del editor de sistemas
        /// </summary>
        [MenuItem("Window/Systemic Editor")]
        public static void ShowWindow()
        {
            window = GetWindow<SystemicEditor>("Systemic Editor");
        }

        /// <summary>
        /// Inicializización de la ventanta, se inicializará
        /// también cuando se produzca un cambio en el estado de juego
        /// </summary>
        static SystemicEditor()
        {
            EditorApplication.playModeStateChanged += UpdateRootReference;
        }

        /// <summary>
        /// Actualiza la refencia del sistema root entre cambios de modo
        /// en el editor, por ejemplo de stop a play.
        /// </summary>
        /// <param name="state"></param>
        private static void UpdateRootReference(PlayModeStateChange state)
        {
            if (RootSystem.Instance == null)
                SearchRootSystem();
            else if (rootSystem == null)
                rootSystem = RootSystem.Instance;
            if (window != null) window.Repaint();
        }

        /// <summary>
        /// Cuando se habilita la ventana se trata de encontrar el sistema raiz
        /// </summary>
        private void OnEnable()
        {
            SearchRootSystem();
        }

        /// <summary>
        /// Busca en la jerarquía de gameObjects de la escena el root system.
        /// Si no encuentra ninguna instancia termina sin más problema dejando la referencia al root system a nulo.
        /// Si hay más de un root system muestra una alerta y usa la referencia al primero.
        /// El root system elegido será el root system principal.
        /// </summary>
        private static void SearchRootSystem()
        {
            RootSystem[] roots = GameObject.FindObjectsOfType<RootSystem>();
            if (roots.GetLength(0) == 0) return;
            else if (roots.GetLength(0) > 1)
            {
                Debug.LogWarning("Systemic Editor: There are more than one root systems, it will be used first one found");
            }
            roots[0].UpdateInstance();
            rootSystem = RootSystem.Instance;
        }

        /// <summary>
        /// Función que muestra la GUI del editor.
        /// Primero inicializa los valores de la ventana, luego comprueba si hay
        /// una instancia activa del sistema raiz, si no es así, permite crear un sistema raíz.
        /// Si lo hay muestra la jerarquía de sistemas y el menú relativo a los estímulos.
        /// Por último muestra el botón de comprobación de integridad de la jerarquía y
        /// el cuadro de texto de diagnóstico.
        /// </summary>
        private void OnGUI()
        {
            SetUpWindow();
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("Root system", EditorStyles.boldLabel);
            if (RootSystem.Instance == null)
                ShowCreateRootMode();
            else
            {
                ShowSystemsMode();
                ShowStimulusMenu();
            }
            if (GUILayout.Button("Check Hierarchy's Integrity")) CheckIntegrity();
            if (diagnosisDone)
            {
                diagScroll = GUILayout.BeginScrollView(diagScroll);
                if (lastDiagnosis.Count == 1)
                {
                    GUIStyle blue = new GUIStyle(EditorStyles.textArea);
                    blue.normal.textColor = Color.blue;
                    EditorGUILayout.LabelField(0 + ": " + lastDiagnosis[0], blue);
                }
                else
                    for (int i = 0; i < lastDiagnosis.Count; i++)
                        EditorGUILayout.LabelField(i + ": " + lastDiagnosis[i], labelErrorStyle);
                GUILayout.EndScrollView();
            }
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// Función para realizar un checkeo de la integridad de la jerarquía sistémica.
        /// Comprueba si hay un sistema raíz instanciado, si las entidades tienen un sistema padre
        /// y si el sistema padre reconoce dichas instancias como hijas, y si las entidades están presentes en el root system. 
        /// Según va realizando el diagnóstico trata de solucionar algunos errores eque encuentra.
        /// Una vez realizado el diagnóstico mostrará logs del checkeo mostrando alertas, errores arreglados y errores sin arreglar
        /// </summary>
        private void CheckIntegrity()
        {
            GUILayout.Label("Diagnosis", EditorStyles.boldLabel);
            lastDiagnosis = new List<string>();
            Entity[] entities = FindObjectsOfType<Entity>();
            int warnings = 0;
            int fixedErrors = 0;
            int unfixedErrors = 0;
            if (RootSystem.Instance == null)
            {
                lastDiagnosis.Add("Doesn't exist a root system, you should add one. Otherwise, the systemic hierarchy won't work: UNFIXED");
                unfixedErrors++;
            }
            else
            {
                if (DeleteNullEntities(RootSystem.Instance.AllSystems))
                {
                    lastDiagnosis.Add("The root system had null references in its systems list: FIXED");
                    fixedErrors++;
                }
                if (DeleteNullEntities(RootSystem.Instance.AllUnits))
                {
                    lastDiagnosis.Add("The root system had null references in its units list: FIXED");
                    fixedErrors++;
                }
            }
            for (int i = 0; i < entities.GetLength(0); i++)
            {
                if (entities[i].GetType() != typeof(RootSystem))
                {
                    if (entities[i].ParentSystem == null)
                    {
                        lastDiagnosis.Add("Entity named " + entities[i].gameObject.name + " has no parent: IGNORED");
                        warnings++;
                    }
                    else
                    {
                        if (!entities[i].ParentSystem.Entities.Contains(entities[i]))
                        {
                            entities[i].ParentSystem.Entities.Add(entities[i]);
                            lastDiagnosis.Add("Entity named " + entities[i].gameObject.name + " was not recognized by its parent system: FIXED");
                            fixedErrors++;
                        }
                        if (entities[i].GetType() == typeof(Unit))
                        {
                            Unit unit = (Unit)entities[i];
                            if (RootSystem.Instance != null && !RootSystem.Instance.AllUnits.Contains(unit))
                            {
                                lastDiagnosis.Add("Entity named " + entities[i].gameObject.name + " was not recognized by root system: FIXED");
                                RootSystem.Instance.AllUnits.Add(unit);
                                fixedErrors++;
                            }
                        }
                        else
                        {
                            System system = (System)entities[i];
                            if (RootSystem.Instance != null && !RootSystem.Instance.AllSystems.Contains(system))
                            {
                                lastDiagnosis.Add("Entity named " + entities[i].gameObject.name + " was not recognized by root system: FIXED");
                                RootSystem.Instance.AllSystems.Add(system);
                                fixedErrors++;
                            }

                            if (DeleteNullEntities(system.Entities))
                            {
                                lastDiagnosis.Add("System named " + entities[i].gameObject.name + " had got null references between its entities list: FIXED");
                                fixedErrors++;
                            }
                        }
                    }
                }
            }
            lastDiagnosis.Add("Diagnosis ended: warnings " + warnings + ", fixed errors " + fixedErrors + ", unfixed errors " + unfixedErrors + "\n");
            diagnosisDone = true;
            if (fixedErrors > 0) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        /// <summary>
        /// Borra referencias nulas de entidades en listas de entidades.
        /// Es usado por la herramienta de diagnóstico de la integridad de la jerarquía sistémica.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool DeleteNullEntities<T>(List<T> list)
        {
            bool someNullReference = false;
            for (int j = list.Count - 1; j > -1; j--)
            {
                if (list[j] == null)
                {
                    list.RemoveAt(j);
                    someNullReference = true;
                }
            }
            return someNullReference;
        }

        /// <summary>
        /// Inicializa la ventana y guarda su referencia, crea los estilos de GUI
        /// y le da a la ventana su tamaño mínimo.
        /// </summary>
        private void SetUpWindow()
        {
            if (window == null) window = GetWindow<SystemicEditor>("Systemic Editor");
            SetUpGUIStyle();
            window.minSize = new Vector2(300f, 200f);
        }

        /// <summary>
        /// Inicializa con los valores adecuados todos los estilos de GUI
        /// </summary>
        private void SetUpGUIStyle()
        {
            if (foldoutStyle == null)
            {
                foldoutStyle = new GUIStyle(EditorStyles.foldout);
                foldoutStyle.fontStyle = FontStyle.Bold;
                labelErrorStyle = new GUIStyle(EditorStyles.textArea);
                labelErrorStyle.normal.textColor = Color.red;
            }
        }

        /// <summary>
        /// Muestra el editor en modo crear un sistema raíz.
        /// Esto se ejecuta cuando no existe sistema raíz actualmente.
        /// </summary>
        private void ShowCreateRootMode()
        {
            GUILayout.BeginVertical("Box");
            if (GUILayout.Button("Create Root System"))
                CreateRootSystem();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Función que crea los elementos básicos de la jerarquía sistémica:
        /// el sistema raíz (root system), el sistema de olor (smell system) y
        /// el controlador de estímulos (stimulus manager).
        /// </summary>
        private void CreateRootSystem()
        {
            GameObject rootGameObject = new GameObject("Root System");
            RootSystem aux = rootGameObject.AddComponent<RootSystem>();
            aux.UpdateInstance();
            rootSystem = RootSystem.Instance;
            rootSystem.StimulusManager = rootGameObject.AddComponent<StimulusManager>();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            GameObject smellGameObject = new GameObject("Smell System");
            SmellSystem smellSystem = smellGameObject.AddComponent<SmellSystem>();
            smellSystem.ParentSystem = rootSystem;
            SerializedObject obj2 = new SerializedObject(smellSystem);
            obj2.FindProperty("systemID").stringValue = "Smell";
            obj2.ApplyModifiedProperties();
            smellSystem.transform.parent = rootSystem.transform;
            RootSystem.AddSystem(smellSystem);
            rootSystem.AddEntity(smellSystem);
            rootSystem.SmellSystem = smellSystem;
        }

        /// <summary>
        /// Modo del editor que se muestra cuando sí que hay un sistema raíz.
        /// Muestra la jerarquía de sistemas y su disposición, permite añadir nuevos sistemas,
        /// eliminarlos y muestra estadísticas sobre la jerarquía.
        /// </summary>
        private void ShowSystemsMode()
        {
            SetUpGUIStyle();
            GUILayout.BeginVertical("Box");
            if (rootSystem == null) rootSystem = RootSystem.Instance;
            GUILayout.Label("Systems", EditorStyles.boldLabel);
            GUILayout.BeginVertical("Box");
            if (rootSystem.AllSystems.Count == 0)
                GUILayout.Label("There are no systems");
            else ShowSystemsList();
            GUILayout.EndVertical();
            if (GUILayout.Button("Add system"))
            {
                SystemCreatorEditor.ShowWindow(rootSystem, window);
            }
            GUILayout.EndVertical();
            if (GUILayout.Button("Delete Root System"))
            {
                PopupEditor.ShowWindow("Are you sure you want to delete the root system? This will delete all the systems. CAUTION", DeleteRootSystem, null);
            }
            ShowSystemStats();
        }

        /// <summary>
        /// Muestra estadísticas de la jerarquía de sistemas.
        /// Por ahora únicamente el número de sistemas y unidades que hay en este momento.
        /// </summary>
        private void ShowSystemStats()
        {
            foldoutStats = EditorGUILayout.Foldout(foldoutStats, "Stats", foldoutStyle);
            if (foldoutStats)
            {
                GUILayout.Label("Systems: " + rootSystem.AllSystems.Count);
                GUILayout.Label("Units: " + rootSystem.AllUnits.Count);
            }
        }


        /// <summary>
        /// Función para borrar toda la jerarquía sistémica, y con ella todos
        /// los sistemas y unidades de la escena.
        /// </summary>
        private void DeleteRootSystem()
        {
            Unit[] units = rootSystem.AllUnits.ToArray();
            System[] systems = rootSystem.AllSystems.ToArray();
            for (int i = 0; i < units.GetLength(0); i++)
                if (units[i] != null)
                    DestroyImmediate(units[i].gameObject);
            for (int i = 0; i < systems.GetLength(0); i++) DestroyImmediate(systems[i]);
            DestroyImmediate(rootSystem.gameObject);
            rootSystem = null;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            window.Repaint();
        }

        /// <summary>
        /// Muestra la sección del editor dedicada a la creación de estímulos.
        /// Permite añadirlos y comprobar que estímulos existen actualmente.
        /// Por ahora los estímulos creados no pueden borrarse o eleminarse.
        /// </summary>
        private void ShowStimulusMenu()
        {
            stimulusFoldout = EditorGUILayout.Foldout(stimulusFoldout, "Stimuli", foldoutStyle);
            if (stimulusFoldout)
            {
                GUILayout.Label("Number of types of stimuli: " + rootSystem.StimulusManager.Values.Count);
                for (int i = 0; i < rootSystem.StimulusManager.Values.Count; i++)
                    GUILayout.Label(rootSystem.StimulusManager.Values[i]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Space(3);
                newStimulusName = GUILayout.TextField(newStimulusName);
                GUILayout.EndVertical();
                if (GUILayout.Button("Add")) AddNewStimulus();
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Función que crea y añade un nuevo estímulo a la jerarquía
        /// sistémica. No permite que el nombre del estímulo sea cadena vacía.
        /// </summary>
        private void AddNewStimulus()
        {
            Undo.RecordObject(rootSystem.StimulusManager, "Add stimulus");
            if (newStimulusName != "") rootSystem.StimulusManager.AddStimulus(newStimulusName);
            EditorUtility.SetDirty(rootSystem.StimulusManager);
            newStimulusName = "";
            window.Repaint();
        }


        /// <summary>
        /// Muestra la lista de todos los sistemas actualmente existentes
        /// Permite seleccionarlos para modificarlos en otra ventana o eliminarlos.
        /// También muestra los sistemas en una disposición jerarquica.
        /// </summary>
        private void ShowSystemsList()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            for (int i = 0; i < rootSystem.AllSystems.Count; i++)
            {
                GUIStyle style;
                if (selectedSystemIndex == i) style = EditorStyles.objectFieldThumb;
                else style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleLeft;
                if (GUILayout.Button(rootSystem.AllSystems[i].gameObject.name, style))
                {
                    selectedSystemIndex = i;
                    Selection.activeGameObject = rootSystem.AllSystems[i].gameObject;
                }
                GUILayout.Space(3);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            for (int i = 0; i < rootSystem.AllSystems.Count; i++)
            {
                if (GUILayout.Button("Delete"))
                {
                    actualSystem = rootSystem.AllSystems[i];
                    PopupEditor.ShowWindow("Are you sure you want to delete the system " + actualSystem.gameObject.name +
                        "? All the entities associated will be lost too.", EraseSystem, null);
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Edit") && selectedSystemIndex >= 0)
                EntityEditor.ShowWindow(rootSystem.AllSystems[selectedSystemIndex]);
            ShowRecursiveEliminationToggle();
            GUILayout.Space(10);
            foldoutHierarchy = EditorGUILayout.Foldout(foldoutHierarchy, "Systems Hierarchy", foldoutStyle);
            if (foldoutHierarchy) ShowSystemHierarchy();
        }

        /// <summary>
        /// Muestra el toggle de borrado recusivo de sistemas.
        /// Esto permite dos modos de borrado de sistema. Uno que solo borra
        /// el sistema y no borra las subentidades y un segundo que borra
        /// recursivamente tanto el sistema como todas las subentidades
        /// </summary>
        private void ShowRecursiveEliminationToggle()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("System recursive elimination", EditorStyles.boldLabel);
            GUILayout.BeginVertical();
            GUILayout.Space(7);
            recursiveElimination = EditorGUILayout.Toggle(recursiveElimination);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Muestra una lista de todos los sistemas siguiendo una
        /// disposición jerarquica en la que se puede ver los subsistemas
        /// incluidos en otros sistemas.
        /// </summary>
        private void ShowSystemHierarchy()
        {
            GUILayout.BeginVertical("Box");
            GUILayout.Label(rootSystem.gameObject.name);
            for (int i = 0; i < rootSystem.AllSystems.Count; i++)
            {
                if (rootSystem.AllSystems[i].ParentSystem == rootSystem)
                {
                    GUILayout.Label(">>" + rootSystem.AllSystems[i].name);
                    ShowSubsystems(rootSystem.AllSystems[i], 2);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Función auxiliar que muestra de forma recursiva los subsistemas
        /// del sistema actual, del mismo modo muestra los subsitemas de los subsistemas
        /// hasta llegar a la máxima profundidad.
        /// </summary>
        /// <param name="system">Sistema actual del que se quiere mostrar los subsistemas</param>
        /// <param name="depth">La profundidad actual para mostrar tabulaciones</param>
        private void ShowSubsystems(System system, int depth)
        {
            for (int i = 0; i < system.Entities.Count; i++)
            {
                Entity entity = system.Entities[i];
                if (entity != null)
                {
                    Type type = entity.GetType();
                    if (type == typeof(System) || type == typeof(RootSystem) || type == typeof(SmellSystem))
                    {
                        System auxSystem = (System)entity;
                        string tabs = "";
                        for (int j = 0; j < depth - 1; j++) tabs += "     ";
                        tabs += ">>";
                        GUILayout.Label(tabs + auxSystem.gameObject.name);
                        ShowSubsystems(auxSystem, depth + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Función para eleminar un sistema de la jerarquía
        /// recurriendo al root system para ello.
        /// </summary>
        private void EraseSystem()
        {
            if (recursiveElimination) EntityEditor.DeleteEntity(actualSystem, rootSystem);
            else
            {
                rootSystem.EraseSystem(actualSystem);
                if (actualSystem.ParentSystem != null)
                    actualSystem.ParentSystem.Entities.Remove(actualSystem);
                DestroyImmediate(actualSystem.gameObject);
            }
            selectedSystemIndex = -1;
        }
    }
}