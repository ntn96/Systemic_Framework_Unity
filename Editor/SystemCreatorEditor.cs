using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SystemicDesign
{
    /// <summary>
    /// Editor que permite crear sistemas y asignarles nombre y una clave
    /// </summary>
    public class SystemCreatorEditor : EditorWindow
    {
        /// <summary>
        /// Nombre del nuevo sistema a crear
        /// </summary>
        private string newSystemName = "New System";
        /// <summary>
        /// Clave ID que debe ser única del nuevo sistema
        /// </summary>
        private string newSystemID = "";
        /// <summary>
        /// Referencia local al sistema raiz para poder añadir el nuevo sistema
        /// </summary>
        private static RootSystem rootSystem;
        /// <summary>
        /// Booleano que determina si la última ID del sistema es válido o no
        /// </summary>
        private bool badID = false;
        /// <summary>
        /// Estilo de GUI para los mensajes de error
        /// </summary>
        private static GUIStyle redStyle;
        /// <summary>
        /// Referencia local a la misma ventana del editor
        /// </summary>
        private static SystemCreatorEditor window;
        /// <summary>
        /// Índice que determina el sistema elegido para que sea el sistema padre del nuevo sistema
        /// </summary>
        private int parentSystemSelected = 0;
        /// <summary>
        /// Refencia local a la ventana del editor sistémico que ha creado esta ventana
        /// y que está actualmente abierta.
        /// </summary>
        private static EditorWindow systemicEditor;
        /// <summary>
        /// Lista de los nombre de los distintos sistemas que podemos elegir para que sean
        /// el sistema padre del sistema que se está creando en este momento
        /// </summary>
        private List<string> options;

        /// <summary>
        /// Método para mostrar la ventana, no puede ser llamado directamente por el usuario
        /// sino que este editor solo aparece cuando el editor sistémico lo invoca
        /// </summary>
        /// <param name="root">Referencia al root system proporcionada por el editor sistémico</param>
        /// <param name="sysEditor">Referencia a la ventana del editor sistémico que ha creado esta ventana</param>
        public static void ShowWindow(RootSystem root, EditorWindow sysEditor)
        {
            window = GetWindow<SystemCreatorEditor>("Systemic Creator");
            rootSystem = root;
            redStyle = new GUIStyle(EditorStyles.label);
            redStyle.normal.textColor = Color.red;
            systemicEditor = sysEditor;
        }

        /// <summary>
        /// Inicializa la ventana y el estilo de GUI asociado, también actualiza la
        /// referencia al sistema raíz y si no lo haya cierra la ventana.
        /// También pone el tamaño mínimo de la ventana.
        /// </summary>
        private void SetUpWindow()
        {
            if (window == null) window = GetWindow<SystemCreatorEditor>("Systemic Creator");
            if (redStyle == null)
            {
                redStyle = new GUIStyle(EditorStyles.label);
                redStyle.normal.textColor = Color.red;
            }
            if (rootSystem == null) rootSystem = RootSystem.Instance;
            if (rootSystem == null) window.Close();
            window.minSize = new Vector2(400f, 200f);
        }

        /// <summary>
        /// Función que se ejecuta cada vez que se actualiza la GUI de la ventana del creador de sistemas.
        /// Inicializa los valores de la ventana, luego muestra los valores que se pueden introducir para crear
        /// un ventana y dado el caso crea el sistema
        /// </summary>
        private void OnGUI()
        {
            SetUpWindow();
            ShowSystemInfoFields();
            if (rootSystem != null)
            {
                SetUpParentSystemList();
                parentSystemSelected = EditorGUILayout.Popup("Parent system", parentSystemSelected, options.ToArray());
                if (GUILayout.Button("Create System"))
                {
                    if (newSystemID != "" && newSystemID != "Root" && newSystemID != "Smell" && !rootSystem.isKeyUsed(newSystemID))
                    {
                        CreateSystem();
                        badID = false;
                        newSystemName = "New System";
                        newSystemID = "";
                        if (systemicEditor != null) systemicEditor.Repaint();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                    else
                    {
                        badID = true;
                    }

                }
                if (badID) GUILayout.Label("System ID is incorrect or is already in use.", redStyle);
            }
        }

        /// <summary>
        /// Muestra los campos rellenables del editor de crear sistemas para que
        /// el usuario pueda introducir los datos.
        /// </summary>
        private void ShowSystemInfoFields()
        {
            GUILayout.Label("Systems creator editor", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Name of the new system:");
            GUILayout.Label("ID of the new system:");
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            newSystemName = GUILayout.TextField(newSystemName);
            newSystemID = GUILayout.TextField(newSystemID);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Rellena una lista de string con los nombres de todos los sistemas, esta
        /// lista se usará para que el usuario pueda elegir cual es el sistema padre
        /// del sistema a crear.
        /// </summary>
        private void SetUpParentSystemList()
        {
            options = new List<string>();
            options.Add(rootSystem.gameObject.name);
            for (int i = 0; i < rootSystem.AllSystems.Count; i++)
                options.Add(rootSystem.AllSystems[i].gameObject.name);
        }

        /// <summary>
        /// Crea un sistema con los datos que ha introducido el usario y lo
        /// añade a la jerarquía de entidades sistémicas, luego abre
        /// una nueva ventana del editor de entidades para poder modificarla
        /// </summary>
        private void CreateSystem()
        {
            GameObject newSystem = new GameObject(newSystemName);
            System parent;
            if (parentSystemSelected == 0)
                parent = rootSystem;
            else
                parent = rootSystem.AllSystems[parentSystemSelected - 1];
            newSystem.transform.parent = parent.transform;
            System systemScript = newSystem.AddComponent<System>();
            systemScript.ParentSystem = parent;
            parent.AddEntity(systemScript);
            SerializedObject obj = new SerializedObject(systemScript);
            obj.FindProperty("systemID").stringValue = newSystemID;
            obj.ApplyModifiedProperties();
            RootSystem.AddSystem(systemScript);
            EntityEditor.ShowWindow(systemScript);
        }
    }
}