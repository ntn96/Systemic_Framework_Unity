using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEditor.SceneManagement;
using System;

//ENGLISH VERSION//

namespace SystemicDesign
{
    /// <summary>
    /// Editor que permite ver y modificar la estructura interna de una entidad sistémica,
    /// así como añadir y eliminar inputs y outputs.
    /// </summary>
    [InitializeOnLoad]
    public class EntityEditor : EditorWindow
    {
        //CHECKED//
        ///<summary>
        ///El script de la entidad concreta que está el editor actualmente editando
        ///</summary>
        private static Entity actualEntity;
        /// <summary>
        /// El gameObject que está editando el editor de entidades. Puede tener o no
        /// un script de entidad sistémica
        /// </summary>
        private static GameObject gameObjectEntity;
        /// <summary>
        /// Una instancia auxiliar para gestionar si el gameobject asociado ha cambiado o no
        /// </summary>
        private static GameObject lastGameObject;
        /// <summary>
        /// Referencia a la ventana abierta actualmente del entity editor
        /// </summary>
        private static EntityEditor window;
        /// <summary>
        /// Referencia local para este editor para el root system usado para listado
        /// de sistemas padre y demás
        /// </summary>
        private static RootSystem rootSystem;
        /// <summary>
        /// Booleano que guarda si la entidad sistemica actual es un sistema o no
        /// </summary>
        private static bool isSystem = false;
        /// <summary>
        /// Booleano utilizado para mostrar información sobre errores por medio de la consola
        /// de debug
        /// </summary>
        private static bool debugEditor = false;
        /// <summary>
        /// Booleano que determina si el editor está o no en modo autoActualización. En este modo
        /// las referencias de los componentes se actualizarán cada vez en OnGUI, lo cual puede ser
        /// costoso.
        /// </summary>
        private bool autoUpdate = false;
        /// <summary>
        /// Array de listas de componentes, cada posición del array corresponde con uno de los
        /// gameobjects hijos que tiene la entidad, y cada lista tiene los componentes sistémicos de dicho
        /// hijo. Es necesario para que se muestre correctamente el editor y poder modificar bien las entidades
        /// </summary>
        private static List<Component>[] allComponents;
        /// <summary>
        /// Array de listas de booleanos que se corresponden uno a uno con el array de listas
        /// de componentes de allComponents. Es usado para los foldouts del editor
        /// </summary>
        private static List<bool>[] compFoldouts;
        /// <summary>
        /// Estilo de GUI personalizado para los foldouts del editor.
        /// </summary>
        private static GUIStyle foldoutStyle;
        /// <summary>
        /// Estilo de GUI personalizado para mostrar un error por medio de labels en el editor
        /// </summary>
        private static GUIStyle errorStyle;
        /// <summary>
        /// Estilo de GUI personalizado para mostrar un error por medio de textArea en el editor
        /// </summary>
        private static GUIStyle errorTextAreaStyle;
        /// <summary>
        /// Vector bidimensional que controla la posición de la barra de scroll que aparece cuando
        /// el editor se hace demasiado grande
        /// </summary>
        private Vector2 scroll;
        /// <summary>
        /// Booleano que determina si se ha borrado o no un componente antes de mostrar toda la GUI
        /// del editor, lo que propiciaría una terminación temprana de la función OnGUI y un repintando
        /// del editor
        /// </summary>
        private bool componentErased;
        /// <summary>
        /// Índice para el popup de elección de sistema padre
        /// </summary>
        private int parentIndex = 0;
        /// <summary>
        /// Índice para el popup de los componentes sistémicos disponibles que se pueden añadir
        /// </summary>
        private int sysCompSelected = 0;
        /// <summary>
        /// Booleano que determina si se muestra o no el foldout del editor componentes sistémicos
        /// </summary>
        private bool sysCompFoldout;
        /// <summary>
        /// Booleano que determina si se muestra o no el folfout del editor de colliders
        /// </summary>
        private bool colliderFoldout;
        /// <summary>
        /// El nombre del gameObject del nuevo collider a añadir en la entidad actual
        /// </summary>
        private string newColliderName;
        /// <summary>
        /// Referncia al gameObject del nuevo collider que se está añadiendo
        /// </summary>
        private GameObject newColliderGameObject;
        /// <summary>
        /// Índice que determina el collider elegido en la unidad
        /// </summary>
        private int colliderIndex = 0;
        /// <summary>
        /// Determina si al gameObject de nuevo collider se le ha añadido ya o no el collider
        /// </summary>
        private bool colliderAdded = false;
        /// <summary>
        /// Booleano que determina si se ha añadido o no el componente sistémico finalmente
        /// </summary>
        private bool notAdded = false;
        /// <summary>
        /// Determina si se ha añadido o no un rigibody al gameObject de nuevo collider
        /// </summary>
        private bool rigidAdded = false;
        /// <summary>
        /// Guarda las componentes transform de los gameObject hijos de la entidad actual para poder listarlos
        /// </summary>
        private Transform[] entityComponents;
        /// <summary>
        ///  Índice que determina el gameObject hijo elegido actualmente
        /// </summary>
        private int childIndex = 0;
        /// <summary>
        /// Índice que determina el gameObject hijo a borrar
        /// </summary>
        private int childEraseIndex = 0;
        /// <summary>
        /// Nombres de los tipos de colliders soportados para añadir a los gameObjects
        /// </summary>
        private string[] colliderOptions =
        {
        "Box Collider",
        "Capsule Collider",
        "Mesh Collider",
        "Sphere Collider"
    };
        /// <summary>
        /// Lista de los distintos componentes sistémicos disponibles para usar
        /// </summary>
        private string[] systemicComponentOptions = {
        "Input Direct Connection",
        "Input Periodic Activation",
        "Input Random Activation",
        "Input Smell",
        "Input Vision Enter",
        "Input Vision Stay",
        "Output Broadcast",
        "Output Direct Connection",
        "Output Emit Particle",
        "Output Presence",
        "Output Presence Activation Enter",
        "Output Presence Activation Stay"
    };
        //-------//

        #region Initializing Editor
        //CHECKED//
        /// <summary>
        /// Constructor para la inicialización del editor. Es usado para iniciar de nuevo
        /// el editor después de cambiar a modo play o modo editor
        /// </summary>
        static EntityEditor()
        {
            EditorApplication.playModeStateChanged += InitializeGUIStyles;
            EditorApplication.playModeStateChanged += UpdateRootReference;
            EditorApplication.playModeStateChanged += SetGameObjectEntityReference;
        }

        //CHECKED//
        /// <summary>
        /// El primero de los dos métodos para mostrar la ventana del editor de entidades.
        /// Este método es usado para que el editor sea abierto por el editor sistémico ya con 
        /// una entidad sistémica enlazada para editar
        /// </summary>
        /// <param name="entity">Entidad asociada de antemano por el editor sistémico para ser editada</param>
        public static void ShowWindow(Entity entity)
        {
            window = GetWindow<EntityEditor>("Entity Editor");
            SetMinimalEntityConfiguration(entity);
            InitializeGUIStyles();
        }

        //CHECKED//
        /// <summary>
        /// El segundo de los métodos para mostrar la ventana de editor de entidades.
        /// En esta versión no hay entidad sistémica por defecto y se accede por medio de
        /// la opción de window en la barra de principal de Unity
        /// </summary>
        [MenuItem("Window/Entity Editor")]
        public static void ShowWindow()
        {
            window = GetWindow<EntityEditor>("Entity Editor");
            InitializeGUIStyles();
        }

        //CHECKED//
        /// <summary>
        /// Ofrece la configuración mínima de la ventana del editor con la entidad pasada por parámetro.
        /// Pone como entidad actual la pasada. Si la entidad es un sistema pone el gameObject objetivo al 
        /// gameObject de la entidad, si es en cambio una unidad trata de asignar al gameObject pardre, si no
        /// hay ninguno muestra un mensaje de alerta y asigna el gameObject de la unidad
        /// </summary>
        /// <param name="entity">Entidad que va a ser modificada por el editor</param>
        private static void SetMinimalEntityConfiguration(Entity entity)
        {
            if (entity != null)
            {
                gameObjectEntity = entity.gameObject;
                Type type = entity.GetType();
                if (type == typeof(System) || type == typeof(RootSystem) || type == typeof(SmellSystem))
                    isSystem = true;
                else if (entity.transform.parent != null)
                    gameObjectEntity = entity.transform.parent.gameObject;
                else
                    if (debugEditor) Debug.LogWarning("Entity Editor: The Unit doesn't follow an appropriate structure, it hasn't got a parent gameObject");
                actualEntity = entity;
            }
        }

        //CHECKED//
        /// <summary>
        /// Cuando se habilita la ventana se trata de encontrar el sistema raiz y se
        /// asigna el objeto activo por la selección como el gameObject clave que se editará
        /// si es que no hay ya uno impuesto
        /// </summary>
        private void OnEnable()
        {
            SearchRootSystem();
            if (gameObjectEntity == null)
                gameObjectEntity = Selection.activeGameObject;
        }

        //CHECKED//
        /// <summary>
        /// Inicializa los estilos de GUI utilizados en el editor sistémico
        /// </summary>
        private static void InitializeGUIStyles()
        {
            foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;
            errorStyle = new GUIStyle(EditorStyles.label);
            errorStyle.normal.textColor = Color.red;
            errorStyle.fontStyle = FontStyle.Bold;
            errorTextAreaStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
            errorTextAreaStyle.normal.textColor = Color.red;
            errorTextAreaStyle.fontStyle = FontStyle.Bold;
        }
        /// <summary>
        /// Inicializa los estilos de GUI utilizados en el editor sistémico
        /// </summary>
        private static void InitializeGUIStyles(PlayModeStateChange state) { InitializeGUIStyles(); }

        //CHECKED//
        /// <summary>
        /// Actualiza la refencia del sistema root entre cambios de modo en el editor, por ejemplo de stop a play.
        /// Si la instancia de root system no está inicializada se busca un candidato, si no se encuentra ninguno
        /// se termina la ejecución del método. Si se encuentra una instancia 
        /// se guarda la referencia a dicha instancia de manera local.
        /// </summary>
        private static bool UpdateRootReference()
        {
            if (RootSystem.Instance == null)
                return SearchRootSystem();
            else if (rootSystem == null)
                rootSystem = RootSystem.Instance;
            return true;
        }

        /// <summary>
        /// Actualiza la refencia del sistema root entre cambios de modo en el editor, por ejemplo de stop a play.
        /// Si la instancia de root system no está inicializada se busca un candidato, si no se encuentra ninguno
        /// se termina la ejecución del método. Si se encuentra una instancia 
        /// se guarda la referencia a dicha instancia de manera local.
        /// </summary>
        /// <param name="state">Estado de modo de juego cuando se produce un cambio</param>
        private static void UpdateRootReference(PlayModeStateChange state) { UpdateRootReference(); }

        //CHECKED//
        /// <summary>
        /// En un cambio de modo pone el gameObject de la entidad activa como aquel que esté seleccionado
        /// en ese momento. Si hay alguna selección y se está cambiando al modo edit se pasa a buscar el script
        /// de la entidad. 
        /// Se obtienen los nuevos componentes sistémicos y se procede a repintar la ventana.
        /// </summary>
        /// <param name="state">Estado del modo de juego, usado para saber si esta función debe o no aplicarse</param>
        private static void SetGameObjectEntityReference(PlayModeStateChange state)
        {
            gameObjectEntity = Selection.activeGameObject;
            if (state == PlayModeStateChange.EnteredEditMode && gameObjectEntity != null)
                SetGameObjectEntityReference();
            if (window != null) window.Repaint();
        }

        //CHECKED//
        /// <summary>
        /// Se busca la referencia al script de entidad sistémica.
        /// Primero se mira en el gameObject objetivo, si no está en este se mira en uno de los hijos.
        /// También puede darse el caso de que no se encuentre ningúna entidad por lo que se podrá crear una en el editor.
        /// Si se encuentra alguna entidad se comprueba que esté localizada correctamente de acuerdo si es un sistema o una unidad.
        /// En caso de una mala configuración se mostrará un warning.
        /// </summary>
        private static void SetGameObjectEntityReference()
        {
            actualEntity = gameObjectEntity.GetComponent<Entity>();
            bool inChild = false;

            if (actualEntity == null)
            {
                actualEntity = gameObjectEntity.GetComponentInChildren<Entity>();
                inChild = true;
            }

            if (actualEntity != null)
            {
                if (actualEntity.GetType() != typeof(Unit))
                {
                    isSystem = true;
                    if (debugEditor && inChild) Debug.LogWarning("Entity Editor: System Script should be in the actual gameObject");
                    GetSystemicsComponentsFromSystem();
                }
                else GetSystemicsComponenetsFromUnit();
            }
        }

        //CHECKED//
        /// <summary>
        /// Realiza una búsqueda del sistema raiz de juego de entre los objetos de juego.
        /// Si no existe ninguno muestra un warning.
        /// Si existe más de uno muestra un warning y escoge al primero.
        /// El root system elegido se pone como instancia del singleton y se guarda la referencia
        /// para la ventana actual.
        /// </summary>
        private static bool SearchRootSystem()
        {
            RootSystem[] roots = GameObject.FindObjectsOfType<RootSystem>();
            if (roots.GetLength(0) == 0)
            {
                if (debugEditor) Debug.LogWarning("Entity Editor: Doesn't exist any game system");
                return false;
            }
            else if (roots.GetLength(0) > 1)
            {
                if (debugEditor) Debug.LogWarning("Entity Editor: There are more than one root systems, it will be used first one found");
            }
            roots[0].UpdateInstance();
            rootSystem = RootSystem.Instance;
            return true;
        }

        //CHECKED//
        /// <summary>
        /// Asegura que exista la referncia a la ventana de este editor
        /// </summary>
        private void CheckWindowReference()
        {
            if (window == null) window = GetWindow<EntityEditor>("Entity Editor");
            if (foldoutStyle == null) InitializeGUIStyles();    
        }

        //CHECKED//
        /// <summary>
        /// Comprueba la integridad de la referencia local de la ventana al root system.
        /// Si no tiene la referencia al root, trata de adquirirla.
        /// </summary>
        /// <returns>Devuelve true cuando obtiene correctamente a referencia al root y falso cuando no la encuentra</returns>
        private bool CheckRootReference()
        {
            if (rootSystem == null && !UpdateRootReference())
            {
                GUILayout.TextArea("Entity Editor: There's none root system in current scene\nAdd one from the systemic editor", errorTextAreaStyle);
                return false;
            }
            return true;
        }
        #endregion

        //CHECKED//
        /// <summary>
        /// Muestra la disposición del editor de entidades con cada actualización de GUI.
        /// </summary>
        private void OnGUI()
        {
            CheckWindowReference();
            window.minSize = new Vector2(400f, 300f);
            ShowWindowTitle();
            if (!CheckRootReference()) return;

            scroll = EditorGUILayout.BeginScrollView(scroll);

            ShowGameObjectEntitySlot();

            if (actualEntity != null)           // Si hemos encontrado una entidad asociada al gameObject la muestra sin dejar editarla
                EditorGUILayout.ObjectField("Current Entity", actualEntity, typeof(Entity), true);
            else if (gameObjectEntity != null)  // Si no hay script de entidad pero si gameObject asociado
            {
                // Lo notifica con un mensaje en rojo y muestra un menu que permite añadir el script de entidad al gameObject asociado
                GUILayout.Label("GameObject named " + gameObjectEntity.name + " hasn't got an unit script\n. Do you wish to add one?", errorStyle);
                ShowCreateUnitMenu();
            }

            if (actualEntity != null)
            {
                ShowSystemicComponents();
                ShowSystemicComponentsEditor();
                ShowColliderEditor();

                if (GUILayout.Button("Manual Refresh")) GetSystemicsComponenetsFromUnit();
                if (GUILayout.Button("Save as a Prefab")) SaveAsAPrefab();
                if (GUILayout.Button("Delete Entity")) DeleteEntity(actualEntity, rootSystem);
            }
            EditorGUILayout.EndScrollView();
        }

        //CHECKED//
        /// <summary>
        /// Borra una entidad de toda la estructura de la jerarquía sistémica.
        /// Si es una unidad simplemente la elimina de la lista de todas las unidades del rootSystem.
        /// Si es un sistema borra en profundidad recursivamente todas las entidades que cuelguen de él,
        /// ya sean unidades u otros sistemas.
        /// </summary>
        /// <param name="entity"></param>
        public static void DeleteEntity(Entity entity, RootSystem root)
        {
            bool entityIsSystem = false;
            if (entity.GetType() == typeof(Unit))
            {
                Unit unit = (Unit)entity;
                root.AllUnits.Remove(unit);
            }
            else
            {
                entityIsSystem = true;
                System system = (System)entity;
                root.AllSystems.Remove(system);
                Entity[] entities = system.Entities.ToArray();
                for (int i = 0; i < entities.GetLength(0); i++)
                    if (entities[i] != null)
                        DeleteEntity(entities[i], root);
            }
            if (entity.ParentSystem != null)
                entity.ParentSystem.EraseEntity(entity);
            if (!entityIsSystem && entity.transform.parent != null)
                DestroyImmediate(entity.transform.parent.gameObject);
            else DestroyImmediate(entity.gameObject);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        //CHECKED//
        /// <summary>
        /// Muestra los menús para añadir colliders a los gameObjects hijos que van a albergar los componentes sistémicos.
        /// Permite crear un nuevo gameObject para que guarde colliders.
        /// Si el gameObject actual no tiene un collider, el editor permite elegir uno y añadirlo.
        /// Si el gameObject no tiene un componente regibody permite a añadirlo.
        /// Con el botón terminar edición permite desligar el editor del gameObject para crear otro gameObject nuevo
        /// </summary>
        private void ShowColliderEditor()
        {
            GUILayout.BeginVertical("Box");
            colliderFoldout = EditorGUILayout.Foldout(colliderFoldout, "Collider Editor", foldoutStyle);
            if (colliderFoldout)
            {
                newColliderName = EditorGUILayout.TextField("Collider's name", newColliderName);

                if (newColliderGameObject == null)
                {
                    if (GUILayout.Button("Add collider's GameObject"))
                    {
                        newColliderGameObject = new GameObject(newColliderName);
                        newColliderGameObject.transform.parent = actualEntity.transform;
                        newColliderGameObject.transform.localPosition = Vector3.zero;
                        newColliderGameObject.transform.localRotation = Quaternion.identity;
                    }
                }
                else
                {
                    newColliderGameObject.name = newColliderName;
                    if (!colliderAdded)
                    {
                        colliderIndex = EditorGUILayout.Popup(colliderIndex, colliderOptions);
                        if (GUILayout.Button("Add Collider"))
                        {
                            AddCollider(colliderIndex, newColliderGameObject);
                            colliderAdded = true;
                        }
                    }
                    if (!rigidAdded && GUILayout.Button("Add Rigidbody"))
                    {
                        Rigidbody rb = newColliderGameObject.AddComponent<Rigidbody>();
                        rb.isKinematic = true;
                        rb.useGravity = false;
                        rigidAdded = true;
                    }
                    if (GUILayout.Button("Finish Edition"))
                    {
                        newColliderGameObject = null;
                        colliderAdded = false;
                        rigidAdded = false;
                    }
                }
                if (newColliderGameObject != null && newColliderGameObject.name == "") newColliderName = "New Collider";
                if (actualEntity.transform.childCount > 0)
                {
                    GUILayout.Space(5);
                    GUILayout.Label("Delete collider's gameObject", EditorStyles.boldLabel);
                    childEraseIndex = EditorGUILayout.Popup(childEraseIndex, GetChildsNames(actualEntity.transform));
                    if (GUILayout.Button("Delete"))
                        DeleteSystemicComponents(actualEntity.transform.GetChild(childEraseIndex).gameObject);
                }
            }
            GUILayout.EndVertical();
        }

        //CHECKED//
        /// <summary>
        /// Borra todos los componentes sistémicos presentes en un gameObject.
        /// Elimina las referencias presentes en el script entity que las contenía.
        /// </summary>
        /// <param name="child">GameObject del hijo que contienen los componentes sistémicos a borrar</param>
        private void DeleteSystemicComponents(GameObject child)
        {
            Activable[] cmps = child.GetComponents<Activable>();
            SerializedObject obj = new SerializedObject(actualEntity);
            for (int i = 0; i < cmps.GetLength(0); i++)
            {
                Type type = cmps[i].GetType();
                if (type == typeof(InputDirectConnection)) DeleteSingleComponent(obj, cmps[i], "inputDirect");
                else if (type == typeof(InputPeriodicActivation)) DeleteFromListComponent(obj, cmps[i], "inputsPeriodicActivation");
                else if (type == typeof(InputRandomActivation)) DeleteFromListComponent(obj, cmps[i], "inputsRandomActivation");
                else if (type == typeof(InputSmell)) DeleteFromListComponent(obj, cmps[i], "inputsSmell");
                else if (type == typeof(InputVisionEnter)) DeleteFromListComponent(obj, cmps[i], "inputsVision");
                else if (type == typeof(InputVisionStay)) DeleteFromListComponent(obj, cmps[i], "inputsVision");
                else if (type == typeof(OutputBroadcast)) DeleteSingleComponent(obj, cmps[i], "outputBroadcast");
                else if (type == typeof(OutputDirectConnection)) DeleteSingleComponent(obj, cmps[i], "outputDirect");
                else if (type == typeof(OutputEmitParticle)) DeleteFromListComponent(obj, cmps[i], "outputEmit");
                else if (type == typeof(OutputPresence)) DeleteFromListComponent(obj, cmps[i], "outputsPresence");
                else if (type == typeof(OutputPresenceActivationEnter)) DeleteFromListComponent(obj, cmps[i], "outputsPresenceActivation");
                else if (type == typeof(OutputPresenceActivationStay)) DeleteFromListComponent(obj, cmps[i], "outputsPresenceActivation");
            }
            obj.ApplyModifiedProperties();
            DestroyImmediate(child);
            GetSystemicsComponenetsFromUnit();
            childEraseIndex = -1;
            window.Repaint();
        }

        //CHECKED//
        /// <summary>
        /// Borra un componente sistémico de naturaleza única, es decir un componente del que solo puede haber
        /// uno cada vez en una única entidad. Lo borra de manera genérica sin entrar en cual es concretamente.
        /// </summary>
        /// <param name="obj">Objecto serializable del gameObject que contiene la entidad</param>
        /// <param name="input">Componente sistémico concreto a borrar</param>
        /// <param name="serializedName">Nombre del atributo que posee el componente sistémico en cuestión</param>
        private void DeleteSingleComponent(SerializedObject obj, Activable input, string serializedName)
        {
            SerializedProperty inputprop = obj.FindProperty(serializedName);
            if (inputprop.objectReferenceValue == input)
                inputprop.objectReferenceValue = null;
        }

        //CHECKED//
        /// <summary>
        /// Borra un componente sistémico de naturaleza múltiple, es decir un componente del que puede haber más de
        /// una instancia cada vez en una única entidad. Lo borra de manera genérica sin entrar en cual es concretamente.
        /// </summary>
        /// <param name="obj">Objecto serializable del gameObject que contiene la entidad</param>
        /// <param name="input">Componente sistémico concreto a borrar</param>
        /// <param name="serializedName">Nombre del atributo que posee el componente sistémico en cuestión</param>
        private void DeleteFromListComponent(SerializedObject obj, Activable input, string serializedName)
        {
            SerializedProperty inputprop = obj.FindProperty(serializedName);
            for (int i = 0; i < inputprop.arraySize; i++)
            {
                SerializedProperty prop = inputprop.GetArrayElementAtIndex(i);
                if (prop.objectReferenceValue == input)
                {
                    prop.objectReferenceValue = null;
                    inputprop.DeleteArrayElementAtIndex(i);
                }
            }
        }

        //CHECKED//
        /// <summary>
        /// Añade un collider al gameObject pasado como parámetro.
        /// Solo soporta los collider Box, Capsule, Mesh y Sphere.
        /// </summary>
        /// <param name="index">Índice que determina cual de los 4 tipos de collider añadirá.
        /// 0 = Box, 1 = Capsule, 2 = Mesh y 3 = Sphere.
        /// </param>
        /// <param name="target">GameObject al cual se le añadirá el collider</param>
        private void AddCollider(int index, GameObject target)
        {
            switch (index)
            {
                case 0:
                    target.AddComponent<BoxCollider>().isTrigger = true;
                    break;
                case 1:
                    target.AddComponent<CapsuleCollider>().isTrigger = true;
                    break;
                case 2:
                    MeshCollider aux = target.AddComponent<MeshCollider>();
                    aux.convex = true;
                    aux.isTrigger = true;
                    break;
                case 3:
                    target.AddComponent<SphereCollider>().isTrigger = true;
                    break;
            }
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones de editor necesarias para añadir
        /// componentes sistémicos como inputs y outputs.
        /// </summary>
        private void ShowSystemicComponentsEditor()
        {
            GUILayout.BeginVertical("Box");
            sysCompFoldout = EditorGUILayout.Foldout(sysCompFoldout, "Systemic components editor", foldoutStyle);
            if (sysCompFoldout)
            {
                GUILayout.Label("Systemic Component to add", EditorStyles.boldLabel);
                sysCompSelected = EditorGUILayout.Popup(sysCompSelected, systemicComponentOptions);
                GUILayout.Label("Target GameObject", EditorStyles.boldLabel);
                if (actualEntity.transform.childCount != 0)
                {
                    childIndex = EditorGUILayout.Popup(childIndex, GetChildsNames(actualEntity.transform));
                    if (GUILayout.Button("Add Input/Output"))
                        AddSystemicComponent(sysCompSelected, entityComponents[childIndex].gameObject);
                    if (notAdded)
                        GUILayout.TextArea("Systemic component don't added\nIt already exists an instance of the current component. " +
                            "Dupplicates of this systemic component are not allowed. You should try to edit the already existing instance instead", errorTextAreaStyle);
                }
                else
                {
                    EditorGUILayout.TextArea("You should create a target gameObject in the collider editor " +
                        "before trying to add a systemic component", errorTextAreaStyle);
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(5);
        }

        //CHECKED//
        /// <summary>
        /// Añade un componente sistémico al gameObject pasado como parámetro.
        /// Además para los componentes sistémicos que lo requieran tratará de enlazar un collider
        /// que tenga el gameObject.
        /// Después actualiza la lista de componentes sistémicos del editor.
        /// </summary>
        /// <param name="index">Índice que representa el componente sistémico a añadir</param>
        /// <param name="target">GameObject destino al que se le añadirá el componente sistémico</param>
        private void AddSystemicComponent(int index, GameObject target)
        {
            notAdded = false;
            SerializedObject serialEntity = new SerializedObject(actualEntity);
            switch (index)
            {
                case 0: // Input Direct Conncetion
                    if (actualEntity.InputDirect == null)
                        AddInputOutput(target, typeof(InputDirectConnection), "inputDirect", false);
                    else notAdded = true;
                    break;
                case 1:  // Input Periodic Activation
                    AddInputOutput(target, typeof(InputPeriodicActivation), "inputsPeriodicActivation", true);
                    break;
                case 2:  // Input Random Activation
                    AddInputOutput(target, typeof(InputRandomActivation), "inputsRandomActivation", true);
                    break;
                case 3:  // Input Smell
                    InputSmell inputSmell = (InputSmell)AddInputOutput(target, typeof(InputSmell), "inputsSmell", true);
                    SphereCollider colSmell = target.GetComponent<SphereCollider>();
                    if (colSmell != null)
                    {
                        SerializedObject obj = new SerializedObject(inputSmell);
                        SerializedProperty prop = obj.FindProperty("detectionCollider");
                        prop.objectReferenceValue = colSmell;
                        obj.ApplyModifiedProperties();
                    }
                    break;
                case 4:  // Input Vision Enter
                    InputVisionEnter inputVisionE =
                        (InputVisionEnter)AddInputOutput(target, typeof(InputVisionEnter), "inputsVision", true);
                    Collider[] colVisionE = target.GetComponents<Collider>();
                    for (int i = 0; i < colVisionE.GetLength(0); i++)
                    {
                        if (colVisionE[i].GetType() == typeof(BoxCollider) || colVisionE[i].GetType() == typeof(SphereCollider) ||
                            colVisionE[i].GetType() == typeof(CapsuleCollider) || colVisionE[i].GetType() == typeof(MeshCollider))
                        {
                            SerializedObject obj = new SerializedObject(inputVisionE);
                            SerializedProperty prop = obj.FindProperty("visionCollider");
                            prop.objectReferenceValue = colVisionE[i];
                            obj.ApplyModifiedProperties();
                            break;
                        }
                    }
                    break;
                case 5:  // Input Vision Stay
                    InputVisionStay inputVisionS =
                        (InputVisionStay)AddInputOutput(target, typeof(InputVisionStay), "inputsVision", true);
                    Collider[] colVisionS = target.GetComponents<Collider>();
                    for (int i = 0; i < colVisionS.GetLength(0); i++)
                    {
                        if (colVisionS[i].GetType() == typeof(BoxCollider) || colVisionS[i].GetType() == typeof(SphereCollider) ||
                            colVisionS[i].GetType() == typeof(CapsuleCollider) || colVisionS[i].GetType() == typeof(MeshCollider))
                        {
                            SerializedObject obj = new SerializedObject(inputVisionS);
                            SerializedProperty prop = obj.FindProperty("visionCollider");
                            prop.objectReferenceValue = colVisionS[i];
                            obj.ApplyModifiedProperties();
                            break;
                        }
                    }
                    break;
                case 6:  // Output Broadcast
                    if (serialEntity.FindProperty("outputBroadcast").objectReferenceValue == null)
                        AddInputOutput(target, typeof(OutputBroadcast), "outputBroadcast", false);
                    else notAdded = true;
                    break;
                case 7:  // Output Direct Connection
                    if (serialEntity.FindProperty("outputDirect").objectReferenceValue == null)
                        AddInputOutput(target, typeof(OutputDirectConnection), "outputDirect", false);
                    else notAdded = true;
                    break;
                case 8:  // Output Emit Particle
                    AddInputOutput(target, typeof(OutputEmitParticle), "outputEmit", true);
                    break;
                case 9:  // Output Presence
                    OutputPresence outputPres =
                        (OutputPresence)AddInputOutput(target, typeof(OutputPresence), "outputsPresence", true);
                    Collider[] colPres = target.GetComponents<Collider>();
                    for (int i = 0; i < colPres.GetLength(0); i++)
                    {
                        if (colPres[i].GetType() == typeof(BoxCollider) || colPres[i].GetType() == typeof(SphereCollider) ||
                            colPres[i].GetType() == typeof(CapsuleCollider) || colPres[i].GetType() == typeof(MeshCollider))
                        {
                            SerializedObject obj = new SerializedObject(outputPres);
                            SerializedProperty prop = obj.FindProperty("presenceCollider");
                            prop.objectReferenceValue = colPres[i];
                            obj.ApplyModifiedProperties();
                            break;
                        }
                    }
                    break;
                case 10: // Output Presence Activation Enter
                    OutputPresenceActivationEnter outputPresE = (OutputPresenceActivationEnter)
                        AddInputOutput(target, typeof(OutputPresenceActivationEnter), "outputsPresenceActivation", true);
                    Collider[] colPresE = target.GetComponents<Collider>();

                    for (int i = 0; i < colPresE.GetLength(0); i++)
                    {
                        if (colPresE[i].GetType() == typeof(BoxCollider) || colPresE[i].GetType() == typeof(SphereCollider) ||
                            colPresE[i].GetType() == typeof(CapsuleCollider) || colPresE[i].GetType() == typeof(MeshCollider))
                        {
                            SerializedObject obj = new SerializedObject(outputPresE);
                            SerializedProperty prop = obj.FindProperty("presenceCollider");
                            prop.objectReferenceValue = colPresE[i];
                            obj.ApplyModifiedProperties();
                            break;
                        }
                    }
                    break;
                case 11: // Output Presence Activation Stay
                    OutputPresenceActivationStay outputPresS = (OutputPresenceActivationStay)
                        AddInputOutput(target, typeof(OutputPresenceActivationStay), "outputsPresenceActivation", true);
                    Collider[] colPresS = target.GetComponents<Collider>();

                    for (int i = 0; i < colPresS.GetLength(0); i++)
                    {
                        if (colPresS[i].GetType() == typeof(BoxCollider) || colPresS[i].GetType() == typeof(SphereCollider) ||
                            colPresS[i].GetType() == typeof(CapsuleCollider) || colPresS[i].GetType() == typeof(MeshCollider))
                        {
                            SerializedObject obj = new SerializedObject(outputPresS);
                            SerializedProperty prop = obj.FindProperty("presenceCollider");
                            prop.objectReferenceValue = colPresS[i];
                            obj.ApplyModifiedProperties();
                            break;
                        }
                    }
                    break;
            }
            GetSystemicsComponenetsFromUnit();
        }

        //CHECKED//
        /// <summary>
        /// Método genérico para añadir un componente sistémico y enlazarlo
        /// correctamente con su entidad padre.
        /// </summary>
        /// <param name="target">GameObject objetivo en el que se añadirá el nuevo componente</param>
        /// <param name="type">Tipo concreto del componente sistémico</param>
        /// <param name="serializedName">Nombre del parámetro serializado en el que se añadirá el componente</param>
        /// <param name="list">Determina si el parámetros serializado es o no una lista</param>
        /// <returns></returns>
        private Activable AddInputOutput(GameObject target, Type type, string serializedName, bool list)
        {
            Activable act = (Activable)target.AddComponent(type);
            SerializedObject obj = new SerializedObject(actualEntity);
            SerializedObject obj2 = new SerializedObject(act);
            SerializedProperty entity = obj2.FindProperty("entity");
            entity.objectReferenceValue = actualEntity;
            SerializedProperty prop = obj.FindProperty(serializedName);
            if (list)
            {
                prop.InsertArrayElementAtIndex(prop.arraySize);
                SerializedProperty aux = prop.GetArrayElementAtIndex(prop.arraySize - 1);
                aux.objectReferenceValue = act;
            }
            else
            {
                prop.objectReferenceValue = act;
            }
            obj.ApplyModifiedProperties();
            obj2.ApplyModifiedProperties();
            return act;
        }

        //CHECKED//
        /// <summary>
        /// Obtiene una lista de los nombres de todos los gameObjects hijos de
        /// la componente transform acutal.
        /// También se actualiza la lista de los componentes entity.
        /// </summary>
        /// <param name="transform">Componente transform de la que se quiere saber el nombre de los hijos</param>
        /// <returns></returns>
        private string[] GetChildsNames(Transform transform)
        {
            int childs = transform.childCount;
            string[] names = new string[childs];
            entityComponents = new Transform[childs];
            for (int i = 0; i < childs; i++)
            {
                Transform aux = transform.GetChild(i).transform;
                names[i] = aux.name;
                entityComponents[i] = aux;
            }
            return names;
        }

        //CHECKED//
        /// <summary>
        /// Muestra el título de la ventana del editor correctamente formateado
        /// </summary>
        private void ShowWindowTitle() { GUILayout.Label("Entity Editor", EditorStyles.boldLabel); }

        //CHECKED//
        /// <summary>
        /// Muestra el slot del gameObject sobre el que se va a editar una entidad.
        /// Cuando se añade un gameObject trata de recuperar la entidad concreta y mostrarla
        /// Tambien muestra la opción de poner el editor en modo actualización automática.
        /// </summary>
        private void ShowGameObjectEntitySlot()
        {
            autoUpdate = GUILayout.Toggle(autoUpdate, "Automatic Refresh");
            gameObjectEntity = (GameObject)EditorGUILayout.ObjectField("Entity to modify: ", gameObjectEntity, typeof(GameObject), true);
            UpdateGameObjectEntity();
        }

        //CHECKED//
        /// <summary>
        /// Muestra el listado de todos los componentes sistémicos que tiene
        /// asociada la entidad en cuestión. Si el editor está en modo autoUpdate durante
        /// cada iteración OnGUI se realizará el barrido en busca de los componentes. Lo
        /// cual puede resultar algo costos en tiempo.
        /// También detecta si se ha borrado un componente durante el listado para realizar 
        /// una terminación temprana y reescribir el listado en la siguiente iteración OnGUI.
        /// </summary>
        private void ShowSystemicComponents()
        {
            GUILayout.Label("Inputs/outputs groupings:", EditorStyles.boldLabel);
            if (autoUpdate) GetSystemicsComponenetsFromUnit();
            for (int i = 0; i < allComponents.GetLength(0); i++)
            {
                if (allComponents[i].Count > 0)
                {
                    GUILayout.BeginVertical("Box");
                    GUILayout.Label(allComponents[i][0].gameObject.name, EditorStyles.boldLabel);
                    for (int j = 0; j < allComponents[i].Count; j++)
                    {
                        bool aux = compFoldouts[i][j];
                        if (aux) GUILayout.BeginVertical("Box");
                        aux = EditorGUILayout.Foldout(aux, allComponents[i][j].ToString(), foldoutStyle);
                        if (aux)
                        {
                            ShowComponenteMenu(allComponents[i][j]);
                            if (componentErased)
                            {
                                EndSoon();
                                return;
                            }
                        }
                        if (compFoldouts[i][j]) GUILayout.EndVertical();
                        compFoldouts[i][j] = aux;
                    }
                    GUILayout.EndVertical();
                }
            }
        }

        //CHECKED//
        /// <summary>
        /// Guarda el gameObject en su estado actual como prefab.
        /// Si detetecta que dicho gameObject ya pertenece a un prefab lo sobreescribe.
        /// </summary>
        private void SaveAsAPrefab()
        {
            UnityEngine.Object prefab = PrefabUtility.GetPrefabInstanceHandle(gameObjectEntity); 
            if (prefab == null)
            {
                if (!AssetDatabase.IsValidFolder("Assets/SystemicPrefabs"))
                    AssetDatabase.CreateFolder("Assets", "SystemicPrefabs");
                GameObject newPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(gameObjectEntity,
                    "Assets/SystemicPrefabs/" + gameObjectEntity.name + ".prefab", InteractionMode.UserAction); 
            }
            else
                PrefabUtility.ApplyPrefabInstance(gameObjectEntity, InteractionMode.UserAction);
        }

        //CHECKED//
        /// <summary>
        /// Muestra un menú que permite añadir un script de entidad al gameObject asociado.
        /// Muestra un listado de todos los sistemas para que el usuario elija el sistema padre al que estará asociado
        /// la unidad. Véase que siempre se añade una unidad, los sistemas se crean por medio del editor sistémico.
        /// Se crea el script en un gameObject hijo de nombre el de el padre junto la palabra "Unit".
        /// Despues se llama a actualizar la lista de componentes para que se reflejen los cambios en el editor
        /// </summary>
        private void ShowCreateUnitMenu()
        {
            string[] systemOptions = rootSystem.GetAllSystemsNameList();
            parentIndex = EditorGUILayout.Popup(parentIndex, systemOptions);
            if (GUILayout.Button("Add Unit Script"))
            {
                GameObject newEntity = new GameObject(gameObjectEntity.name + " Unit");
                actualEntity = newEntity.AddComponent<Unit>();
                rootSystem.AllUnits.Add((Unit)actualEntity);
                newEntity.transform.parent = gameObjectEntity.transform;
                newEntity.transform.localPosition = Vector3.zero;
                newEntity.transform.localRotation = Quaternion.identity;
                SerializedObject serializedEntity = new SerializedObject(actualEntity);
                SerializedProperty parent = serializedEntity.FindProperty("parentSystem");
                System parentSystem;
                if (parentIndex == 0) parentSystem = rootSystem;
                else parentSystem = rootSystem.AllSystems[parentIndex - 1];
                parent.objectReferenceValue = parentSystem;
                SerializedObject serializedSystem = new SerializedObject(parentSystem);
                SerializedProperty entities = serializedSystem.FindProperty("entities");
                serializedEntity.FindProperty("parentSystemKey").stringValue = parentSystem.SystemID;
                entities.InsertArrayElementAtIndex(entities.arraySize);
                entities.GetArrayElementAtIndex(entities.arraySize - 1).objectReferenceValue = actualEntity;
                serializedEntity.ApplyModifiedProperties();
                serializedSystem.ApplyModifiedProperties();
                GetSystemicsComponenetsFromUnit();
            }
        }

        //CHECKED//
        /// <summary>
        /// Detecta si se ha cambiado o no el gameobject asociado al editor, si es así
        /// trata de recuperar el script de entidad, primero mirando si está en el GameObject actual
        /// si no es así, busca entre los gameObjects hijos. Si lo encuentra registra si dicha entidad
        /// es o no un sistema para futuras configuraciones. Una vez hecho eso obtiene la lista
        /// de componentes sistémicos que se mostrará más adelante;
        /// </summary>
        private void UpdateGameObjectEntity()
        {
            if (gameObjectEntity != null && gameObjectEntity != lastGameObject)
                SetGameObjectEntityReference();
            else if (gameObjectEntity == null) actualEntity = null;
            lastGameObject = gameObjectEntity;
        }

        //CHECKED//
        /// <summary>
        /// Obtiene la lista de componentes y los subsiguientes booleanos de foldouts para su 
        /// futura impresión en GUI. Este es el método utilizado para una entidad de tipo unidad.
        /// </summary>
        private static void GetSystemicsComponenetsFromUnit()
        {
            allComponents = new List<Component>[actualEntity.transform.childCount];
            compFoldouts = new List<bool>[actualEntity.transform.childCount];
            for (int i = 0; i < actualEntity.transform.childCount; i++)
            {
                allComponents[i] = new List<Component>();
                compFoldouts[i] = new List<bool>();
                GameObject child = actualEntity.transform.GetChild(i).gameObject;
                Component[] cmp = child.GetComponents<Activable>();
                for (int j = 0; j < cmp.GetLength(0); j++)
                {
                    allComponents[i].Add(cmp[j]);
                    compFoldouts[i].Add(false);
                }
            }
        }

        //CHECKED//
        /// <summary>
        /// Obtiene la lista de componentes y los subsiguientes booleanos de foldouts para su 
        /// futura impresión en GUI. Este es el método utilizado para una entidad de tipo sistema.
        /// </summary>
        private static void GetSystemicsComponentsFromSystem()
        {
            allComponents = new List<Component>[1];
            compFoldouts = new List<bool>[1];
            allComponents[0] = new List<Component>();
            compFoldouts[0] = new List<bool>();
            Component[] cmp = actualEntity.GetComponents<Activable>();
            for (int i = 0; i < cmp.GetLength(0); i++)
            {
                allComponents[0].Add(cmp[i]);
                compFoldouts[0].Add(false);
            }

        }

        //CHECKED//
        /// <summary>
        /// Método que realiza una terminación temprana correcta a la hora de mostrar la interfaz del
        /// editor. Esto es requerido cuando se modifican los datos que debe mostrar el editor mientras
        /// este ya los está mostrando. Entonces termina la disposición de la ventana rápidamente y la
        /// marca para que sea repintada.
        /// </summary>
        private void EndSoon()
        {
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            window.Repaint();
            componentErased = false;
        }

        //CHECKED//
        /// <summary>
        /// Método que dado un componente determina qué tipo de componente sistémico es y ejecuta
        /// en consecuencia el método apropiado para mostrar su información en el editor y permitir
        /// al usario editarlo.
        /// </summary>
        /// <param name="component">Componente que será mostrado en el editor de entidades</param>
        private void ShowComponenteMenu(Component component)
        {
            Type type = component.GetType();
            if (type == typeof(InputDirectConnection)) ShowInputDirect(component);
            else if (type == typeof(InputPeriodicActivation)) ShowInputPeriodic(component);
            else if (type == typeof(InputRandomActivation)) ShowInputRandom(component);
            else if (type == typeof(InputSmell)) ShowInputSmell(component);
            else if (type == typeof(InputVisionEnter)) ShowInputVisionEnter(component);
            else if (type == typeof(InputVisionStay)) ShowInputVisionStay(component);
            else if (type == typeof(OutputBroadcast)) ShowOutputBroadcast(component);
            else if (type == typeof(OutputDirectConnection)) ShowOutputDirect(component);
            else if (type == typeof(OutputEmitParticle)) ShowOutputEmit(component);
            else if (type == typeof(OutputPresence)) ShowOutputPresence(component);
            else if (type == typeof(OutputPresenceActivationEnter)) ShowOutputPresenceActivationEnter(component);
            else if (type == typeof(OutputPresenceActivationStay)) ShowOutputPresenceActivationStay(component);
        }

        //CHECKED//
        /// <summary>
        /// Muestra la lista de todas las opciones a motificar comunes para todos los componentes
        /// sistémicos.
        /// </summary>
        /// <param name="input">El componente sistémico actual a modificar</param>
        private void ShowActivableOptions(Activable input)
        {
            EditorGUILayout.ObjectField("Location:", input.gameObject, typeof(GameObject), true);
            SerializedObject obj = new SerializedObject(input);
            SerializedProperty debug = obj.FindProperty("debug");
            EditorGUILayout.PropertyField(debug);
            string[] options = { "Activation without limit", "Limited activation" };
            int selected = 0;
            SerializedProperty infiniteActivations = obj.FindProperty("infiniteActivations");
            if (!infiniteActivations.boolValue) selected = 1;
            selected = EditorGUILayout.Popup(selected, options);
            if (selected == 0) infiniteActivations.boolValue = true;
            else
            {
                infiniteActivations.boolValue = false;
                SerializedProperty maxNumOfActivations = obj.FindProperty("maxNumOfActivations");
                EditorGUILayout.PropertyField(maxNumOfActivations);
                SerializedProperty actualNumActivations = obj.FindProperty("actualNumActivations");
                int numActivations = actualNumActivations.intValue;
                if (numActivations > 0) EditorGUILayout.IntField("Actual num of activations", numActivations);
            }
            obj.ApplyModifiedProperties();
        }

        //CHECKED//
        /// <summary>
        /// Borra un componente sistémico concreto, desenlazándolo de su entidad.
        /// Actualiza la lista de componentes.
        /// </summary>
        /// <param name="activable">El componente sistémico concreto a borrar</param>
        /// <param name="serializedName">El nombre del parámetro serializado del que se va a borrar</param>
        private void DeleteComponent(Activable activable, string serializedName, bool list)
        {
            SerializedObject serialObj = new SerializedObject(actualEntity);
            if (list) DeleteFromListComponent(serialObj, activable, serializedName);
            else DeleteSingleComponent(serialObj, activable, serializedName);
            serialObj.ApplyModifiedProperties();
            DestroyImmediate(activable);
            GetSystemicsComponenetsFromUnit();
            componentErased = true;
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modificar para un input direct connection
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowInputDirect(Component component)
        {
            InputDirectConnection input = (InputDirectConnection)component;
            ShowActivableOptions(input);
            SerializedObject obj = new SerializedObject(input);
            if (isSystem)
            {
                SerializedProperty rebroadcast = obj.FindProperty("rebroadcast");
                EditorGUILayout.PropertyField(rebroadcast);
            }
            ShowListStimulusGUI(obj, false);
            obj.ApplyModifiedProperties();
            if (GUILayout.Button("Delete"))
                DeleteComponent(input, "inputDirect", false);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modificar para un input periodic activation
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowInputPeriodic(Component component)
        {
            InputPeriodicActivation input = (InputPeriodicActivation)component;
            ShowActivableOptions(input);
            SerializedObject obj = new SerializedObject(input);
            ShowPeriodicRandomCommonParams(obj);
            obj.ApplyModifiedProperties();
            if (GUILayout.Button("Delete"))
                DeleteComponent(input, "inputsPeriodicActivation", true);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones comunes a modificar tanto para un input
        /// Periodic Activation como el Random Activation.
        /// </summary>
        /// <param name="obj">El input como objeto serializado</param>
        private void ShowPeriodicRandomCommonParams(SerializedObject obj)
        {
            SerializedProperty activationTime = obj.FindProperty("activationTime");
            EditorGUILayout.PropertyField(activationTime);
            SerializedProperty extraRandomTime = obj.FindProperty("extraRandomTime");
            EditorGUILayout.PropertyField(extraRandomTime);
            SerializedProperty activationMethods = obj.FindProperty("activationMethods");
            EditorGUILayout.PropertyField(activationMethods);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modificar para un input random activation
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowInputRandom(Component component)
        {
            InputRandomActivation input = (InputRandomActivation)component;
            ShowActivableOptions(input);
            SerializedObject obj = new SerializedObject(input);
            ShowPeriodicRandomCommonParams(obj);

            SerializedProperty activationProbability = obj.FindProperty("activationProbability");
            activationProbability.floatValue = (EditorGUILayout.Slider("Activation Probability", activationProbability.floatValue * 100, 0f, 100f)) / 100;

            SerializedProperty increasingProbability = obj.FindProperty("increasingProbability");
            if (increasingProbability.floatValue > (1f - activationProbability.floatValue))
                increasingProbability.floatValue = 1f - activationProbability.floatValue;
            increasingProbability.floatValue = (EditorGUILayout.Slider("Increasing Probability",
                increasingProbability.floatValue * 100, 0f, (1f - activationProbability.floatValue) * 100) / 100);

            SerializedProperty activationExtraTime = obj.FindProperty("activationExtraTime");
            EditorGUILayout.PropertyField(activationExtraTime);
            SerializedProperty resetAfterActivation = obj.FindProperty("resetAfterActivation");
            EditorGUILayout.PropertyField(resetAfterActivation);
            SerializedProperty probabilityByMethod = obj.FindProperty("probabilityByMethod");
            EditorGUILayout.PropertyField(probabilityByMethod);
            if (probabilityByMethod.boolValue)
            {
                SerializedProperty probabilityMethods = obj.FindProperty("probabilityMethods");
                EditorGUILayout.PropertyField(probabilityMethods);
            }
            obj.ApplyModifiedProperties();
            if (GUILayout.Button("Eliminar"))
                DeleteComponent(input, "inputsRandomActivation", true);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modificar para un input smell
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowInputSmell(Component component)
        {
            InputSmell input = (InputSmell)component;
            ShowActivableOptions(input);
            SerializedObject obj = new SerializedObject(input);
            SerializedProperty detectionCollider = obj.FindProperty("detectionCollider");
            ShowDetectionColliderGUI(detectionCollider, input);
            ShowListStimulusGUI(obj, false);
            obj.ApplyModifiedProperties();
            if (GUILayout.Button("Delete"))
                DeleteComponent(input, "inputsSmell", true);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modifciar para aquellos componentes sistémicos
        /// que tengan alguna clase de collider.
        /// </summary>
        /// <param name="detectionCollider">El collider representado como una propiedad serializada</param>
        /// <param name="input">El componente sistémico actual</param>
        private void ShowDetectionColliderGUI(SerializedProperty detectionCollider, Activable input)
        {
            EditorGUILayout.PropertyField(detectionCollider);
            if (detectionCollider.objectReferenceValue == null)
            {
                GUILayout.Label("There is no collider associated, you should add one.", errorStyle);
                if (GUILayout.Button("Add collider"))
                {
                    SphereCollider collider = input.gameObject.AddComponent<SphereCollider>();
                    collider.isTrigger = true;
                    detectionCollider.objectReferenceValue = collider;
                }
            }
            else
            {
                Collider collider = (Collider)detectionCollider.objectReferenceValue;
                if (collider.gameObject != input.gameObject)
                    GUILayout.Label("Detection collider is not located in the same\nGameObject as the input. This will prevent its operation.", errorStyle);
                if (!collider.isTrigger)
                {
                    GUILayout.Label("Collider should be a trigger collider.\nOtherwise input component won't work", errorStyle);
                    if (GUILayout.Button("Set to trigger")) collider.isTrigger = true;
                }
            }
        }

        //CHECKED//
        /// <summary>
        /// Muestra una lista de estímulos para un componente sistémico y sus métodos de activación
        /// asocidados. Permite añadir estímulos o borrarlos, además de modificarlos.
        /// También tiene una opción para mostrar los métodos de sálida de los estímulos.
        /// </summary>
        /// <param name="obj">El componente sistémico como objeto serializado</param>
        /// <param name="exit">Determina si debe o no mostrar una lista de métodos de salida</param>
        private void ShowListStimulusGUI(SerializedObject obj, bool exit)
        {
            GUILayout.Label("Heard stimuli:");
            string[] stimuliOptions = rootSystem.StimulusManager.Values.ToArray();
            SerializedProperty stimuli = obj.FindProperty("stimuli");
            SerializedProperty activationMethods = obj.FindProperty("activationMethods");
            SerializedProperty exitMethods = obj.FindProperty("exitMethods");
            if (rootSystem.StimulusManager.Values == null || rootSystem.StimulusManager.Values.Count == 0)
            {
                EditorGUILayout.TextArea("Doesn't exist any stimulus, you should create some in the Systemic Editor.", errorTextAreaStyle);
            }
            else
            {
                for (int i = 0; i < stimuli.arraySize; i++)
                {
                    SerializedProperty prop = stimuli.GetArrayElementAtIndex(i);
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    GUILayout.Space(4);
                    int selectedIndex = rootSystem.StimulusManager.Values.IndexOf(prop.stringValue);
                    selectedIndex = EditorGUILayout.Popup(selectedIndex, stimuliOptions);
                    prop.stringValue = rootSystem.StimulusManager.Values[selectedIndex];
                    GUILayout.EndVertical();
                    if (GUILayout.Button("Delete"))
                    {
                        stimuli.DeleteArrayElementAtIndex(i);
                        activationMethods.DeleteArrayElementAtIndex(i);
                        if (exit) exitMethods.DeleteArrayElementAtIndex(i);
                    }
                    GUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add Stimulus"))
                {
                    stimuli.InsertArrayElementAtIndex(stimuli.arraySize);
                    stimuli.GetArrayElementAtIndex(stimuli.arraySize - 1).stringValue = rootSystem.StimulusManager.Values[0];
                    activationMethods.InsertArrayElementAtIndex(activationMethods.arraySize);
                    if (exit) exitMethods.InsertArrayElementAtIndex(exitMethods.arraySize);
                }
                GUILayout.Label("Activation methods when contacting collider for each stimulus");
                ShowListProperties(activationMethods);
                if (exit)
                {
                    GUILayout.Label("Activation methods when ending contact between stimulus and collider:");
                    ShowListProperties(exitMethods);
                }
            }
            obj.ApplyModifiedProperties();
        }

        //CHECKED//
        /// <summary>
        /// Muestra campos para modificar cada uno de los elementos
        /// de una lista como propiedad serializada.
        /// </summary>
        /// <param name="propList">Propiedad serializada que contiene la lista</param>
        private void ShowListProperties(SerializedProperty propList)
        {
            for (int i = 0; i < propList.arraySize; i++)
            {
                SerializedProperty prop = propList.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(prop);
            }
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modificar para un input vision enter
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowInputVisionEnter(Component component)
        {
            InputVisionEnter input = (InputVisionEnter)component;
            ShowInputVision(input, true);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modificar para un input vision stay
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowInputVisionStay(Component component)
        {
            InputVisionStay input = (InputVisionStay)component;
            ShowInputVision(input, false);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones comunes a ambas implementaciones del input vison
        /// </summary>
        /// <param name="input">El input vision a mostrar</param>
        /// <param name="enter">Booleano de si el input vision es enter o no</param>
        private void ShowInputVision(InputVision input, bool enter)
        {
            SerializedObject obj = new SerializedObject(input);
            SerializedProperty detectionCollider = obj.FindProperty("visionCollider");
            ShowActivableOptions(input);
            GUILayout.Space(5);
            ShowDetectionColliderGUI(detectionCollider, input);
            ShowListStimulusGUI(obj, enter);
            obj.ApplyModifiedProperties();
            if (GUILayout.Button("Delete"))
                DeleteComponent(input, "inputsVision", true);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modificar para un output direct connection
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowOutputDirect(Component component)
        {
            OutputDirectConnection output = (OutputDirectConnection)component;
            SerializedObject obj = new SerializedObject(output);
            SerializedProperty stimuli = obj.FindProperty("stimuli");
            SerializedProperty entities = obj.FindProperty("entities");
            string[] stimuliOptions = rootSystem.StimulusManager.Values.ToArray();
            ShowActivableOptions(output);
            GUILayout.Space(5);
            if (rootSystem.StimulusManager.Values == null || rootSystem.StimulusManager.Values.Count == 0)
            {
                EditorGUILayout.TextArea("Doesn't exist any stimulus, you should create some in the Systemic Editor.", errorTextAreaStyle);
            }
            else
            {
                for (int i = 0; i < stimuli.arraySize; i++)
                {
                    SerializedProperty prop = stimuli.GetArrayElementAtIndex(i);
                    SerializedProperty entity = entities.GetArrayElementAtIndex(i);
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    GUILayout.Space(4);
                    GUILayout.BeginHorizontal();
                    int selectedIndex = rootSystem.StimulusManager.Values.IndexOf(prop.stringValue);
                    selectedIndex = EditorGUILayout.Popup(selectedIndex, stimuliOptions);
                    prop.stringValue = rootSystem.StimulusManager.Values[selectedIndex];
                    entity.objectReferenceValue = EditorGUILayout.ObjectField("", entity.objectReferenceValue, typeof(Entity), true);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    if (GUILayout.Button("Delete"))
                    {
                        stimuli.DeleteArrayElementAtIndex(i);
                        entities.GetArrayElementAtIndex(i).objectReferenceValue = null;
                        entities.DeleteArrayElementAtIndex(i);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(3);
                if (GUILayout.Button("Add Stimulus"))
                {
                    stimuli.InsertArrayElementAtIndex(stimuli.arraySize);
                    stimuli.GetArrayElementAtIndex(stimuli.arraySize - 1).stringValue = stimuliOptions[0];
                    entities.InsertArrayElementAtIndex(entities.arraySize);
                }
                GUILayout.Space(3);
            }
            obj.ApplyModifiedProperties();
            if (GUILayout.Button("Delete"))
                DeleteComponent(output, "outputDirect", false);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modificar para un output broadcast
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowOutputBroadcast(Component component)
        {
            OutputBroadcast output = (OutputBroadcast)component;
            ShowActivableOptions(output);
            if (!isSystem)
            {
                GUILayout.Label("There should not be an Output broadcast\nin an entity that is not a system", errorStyle);
                if (GUILayout.Button("Delete"))
                    DeleteComponent(output, "outputBroadcast", false);
            }
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modificar para un output emit
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowOutputEmit(Component component)
        {
            OutputEmitParticle output = (OutputEmitParticle)component;
            ShowActivableOptions(output);
            SerializedObject obj = new SerializedObject(output);
            SerializedProperty stimuli = obj.FindProperty("stimuli");
            SerializedProperty emission = obj.FindProperty("emission");
            string[] stimuliOptions = rootSystem.StimulusManager.Values.ToArray();
            GUILayout.Space(5);
            GUILayout.Label("Smells emitted by the entity with an emission lapse.");
            if (rootSystem.StimulusManager.Values == null || rootSystem.StimulusManager.Values.Count == 0)
            {
                EditorGUILayout.TextArea("There isn't any stimulus, you should create some in the Systemic Editor.", errorTextAreaStyle);
            }
            else
            {
                for (int i = 0; i < stimuli.arraySize; i++)
                {
                    SerializedProperty prop = stimuli.GetArrayElementAtIndex(i);
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    GUILayout.Space(4);
                    GUILayout.BeginHorizontal();
                    int selectedIndex = rootSystem.StimulusManager.Values.IndexOf(prop.stringValue);
                    selectedIndex = EditorGUILayout.Popup(selectedIndex, stimuliOptions);
                    prop.stringValue = rootSystem.StimulusManager.Values[selectedIndex];
                    GUILayout.Label("Emission time:");
                    SerializedProperty emitTime = emission.GetArrayElementAtIndex(i);
                    emitTime.floatValue = EditorGUILayout.FloatField(emitTime.floatValue);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    if (GUILayout.Button("Delete"))
                    {
                        stimuli.DeleteArrayElementAtIndex(i);
                        emission.DeleteArrayElementAtIndex(i);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
                if (GUILayout.Button("Add Stimulus"))
                {
                    stimuli.InsertArrayElementAtIndex(stimuli.arraySize);
                    stimuli.GetArrayElementAtIndex(stimuli.arraySize - 1).stringValue = stimuliOptions[0];
                    emission.InsertArrayElementAtIndex(emission.arraySize);
                }
                GUILayout.Space(5);
                SerializedProperty emissionRadius = obj.FindProperty("emissionRadius");
                SerializedProperty particlePrefab = obj.FindProperty("particlePrefab");
                EditorGUILayout.PropertyField(emissionRadius);
                EditorGUILayout.PropertyField(particlePrefab);
            }
            obj.ApplyModifiedProperties();
            if (GUILayout.Button("Delete"))
                DeleteComponent(output, "outputEmit", true);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modificar para un output presence
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowOutputPresence(Component component)
        {
            OutputPresence output = (OutputPresence)component;
            ShowActivableOptions(output);
            SerializedObject obj = new SerializedObject(output);
            SerializedProperty presenceCollider = obj.FindProperty("presenceCollider");
            SerializedProperty stimulus = obj.FindProperty("stimulus");
            if (rootSystem.StimulusManager.Values == null || rootSystem.StimulusManager.Values.Count == 0)
            {
                EditorGUILayout.TextArea("There isn't any stimulus, you should create some in the Systemic Editor.", errorTextAreaStyle);
            }
            else
            {
                string[] stimuliOptions = rootSystem.StimulusManager.Values.ToArray();
                int selectedIndex = rootSystem.StimulusManager.Values.IndexOf(stimulus.stringValue);
                selectedIndex = EditorGUILayout.Popup(selectedIndex, stimuliOptions);
                if (selectedIndex != -1) stimulus.stringValue = rootSystem.StimulusManager.Values[selectedIndex];
                else stimulus.stringValue = stimuliOptions[0];
            }
            ShowDetectionColliderGUI(presenceCollider, output);
            obj.ApplyModifiedProperties();
            if (GUILayout.Button("Delete"))
                DeleteComponent(output, "outputsPresence", true);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modificar para un output presence activation enter
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowOutputPresenceActivationEnter(Component component)
        {
            OutputPresenceActivationEnter output = (OutputPresenceActivationEnter)component;
            ShowOutputPresenceActivation(output);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones específicas a modificar para un output presence activation stay
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowOutputPresenceActivationStay(Component component)
        {
            OutputPresenceActivationStay output = (OutputPresenceActivationStay)component;
            ShowOutputPresenceActivation(output);
        }

        //CHECKED//
        /// <summary>
        /// Muestra las opciones comunes a ambas implementaciones del output presence activation
        /// </summary>
        /// <param name="component">El componente sistémico a utilizar</param>
        private void ShowOutputPresenceActivation(OutputPresenceActivation output)
        {
            SerializedObject obj = new SerializedObject(output);
            SerializedProperty presenceCollider = obj.FindProperty("presenceCollider");
            SerializedProperty stimulableObjects = obj.FindProperty("stimulableObjects");
            ShowActivableOptions(output);
            GUILayout.Space(5);
            ShowDetectionColliderGUI(presenceCollider, output);
            GUILayout.Space(5);
            if (rootSystem.StimulusManager.Values == null || rootSystem.StimulusManager.Values.Count == 0)
            {
                EditorGUILayout.TextArea("There isn't any stimulus, you should create some in the Systemic Editor.", errorTextAreaStyle);
            }
            else
            {
                string[] stimuliOptions = rootSystem.StimulusManager.Values.ToArray();
                SerializedProperty prop2 = obj.FindProperty("stimulus");
                int selectedIndex2 = rootSystem.StimulusManager.Values.IndexOf(prop2.stringValue);
                if (selectedIndex2 < 0) selectedIndex2 = 0;
                selectedIndex2 = EditorGUILayout.Popup(selectedIndex2, stimuliOptions);
                prop2.stringValue = rootSystem.StimulusManager.Values[selectedIndex2];

                GUILayout.Label("Stimulable objects");
                for (int i = 0; i < stimulableObjects.arraySize; i++)
                {
                    SerializedProperty prop = stimulableObjects.GetArrayElementAtIndex(i);
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    GUILayout.Space(4);
                    int selectedIndex = rootSystem.StimulusManager.Values.IndexOf(prop.stringValue);
                    selectedIndex = EditorGUILayout.Popup(selectedIndex, stimuliOptions);
                    prop.stringValue = rootSystem.StimulusManager.Values[selectedIndex];
                    GUILayout.EndVertical();
                    if (GUILayout.Button("Delete"))
                        stimulableObjects.DeleteArrayElementAtIndex(i);
                    GUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add stimulable object"))
                {
                    stimulableObjects.InsertArrayElementAtIndex(stimulableObjects.arraySize);
                    stimulableObjects.GetArrayElementAtIndex(stimulableObjects.arraySize - 1).stringValue = stimuliOptions[0];
                }
            }
            obj.ApplyModifiedProperties();
            if (GUILayout.Button("Delete"))
                DeleteComponent(output, "outputsPresenceActivation", true);
        }
    }
}

