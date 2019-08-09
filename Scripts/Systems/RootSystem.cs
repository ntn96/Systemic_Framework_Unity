using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Sistema raíz de la jerarquía sistémica, todos los sistemas
    /// debería colgar de este sistema a la profundidad que sea. Tiene
    /// referencias a todos los sistemas y todas las unidades del juego.
    /// También tiene referencias a otras clases importantes como el
    /// stimulus manager o el smell system.
    /// </summary>
    public class RootSystem : System
    {
        /// <summary>
        /// Referencia al único smell system que debe haber en la escena
        /// </summary>
        [SerializeField] private SmellSystem smellSystem;
        [Space(10)]
        [Header("Lists of Entites")]
        
        /// <summary>
        /// Lista de todas y cada una de las unidades instanciadas en la escena
        /// </summary>
        [SerializeField] private List<Unit> allUnits = new List<Unit>();
        /// <summary>
        /// Lista de todos y cada uno de los sistemas instanciados en la escena
        /// </summary>
        [SerializeField] private List<System> allSystems = new List<System>();
        /// <summary>
        /// Diccionario que relaciona cada sistema con su id de sistema para que todos sean fácilmente
        /// accesible con una llamada al root system
        /// </summary>
        [SerializeField] private Dictionary<string, System> systemDictionary = new Dictionary<string, System>();
        /// <summary>
        /// Referencia al manager de estímulos
        /// </summary>
        [SerializeField] private StimulusManager stimulusManager;

        /// <summary>
        /// Instancia única del root system para convertirlo en un singleton
        /// </summary>
        private static RootSystem instance;
        /// <summary>
        /// Instancia única del root system para convertirlo en un singleton
        /// </summary>
        public static RootSystem Instance { get { return instance; } }

        /// <summary>
        /// Lista de todos y cada uno de los sistemas instanciados en la escena
        /// </summary>
        public List<System> AllSystems
        {
            get
            {
                return allSystems;
            }
        }

        /// <summary>
        /// Método set para inicializar el smell system asociado al root system
        /// si es que no hay ya uno asociado.
        /// </summary>
        public SmellSystem SmellSystem
        {
            set
            {
                if (smellSystem == null) smellSystem = value;
            }
        }

        /// <summary>
        /// Referencia al manager de estímulos
        /// </summary>
        public StimulusManager StimulusManager
        {
            get
            {
                return stimulusManager;
            }
            set
            {
                stimulusManager = value;
            }
        }

        /// <summary>
        /// Lista de todas y cada una de las unidades instanciadas en la escena
        /// </summary>
        public List<Unit> AllUnits
        {
            get
            {
                return allUnits;
            }
        }

        /// <summary>
        /// Actualiza la única instancia de root system,
        /// si esta no es la única instancia y no es la principal
        /// se elimina esta instancia.
        /// </summary>
        public void UpdateInstance()
        {
            if (instance == null)
            {
                instance = this;
                systemID = "Root";
            }
            else if (instance != this) DestroyImmediate(gameObject);
        }

        /// <summary>
        /// Actualiza la única instancia de root system,
        /// si esta no es la única instancia y no es la principal
        /// se elimina esta instancia. Además añade todos los
        /// sistemas al diccionario de sistemas.
        /// </summary>
        private void Awake()
        {
            systemID = "Root";
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
            if (systemDictionary.Count == 0)
            {
                for (int i = 0; i < allSystems.Count; i++)
                    systemDictionary.Add(allSystems[i].SystemID, allSystems[i]);
            }
        }

        /// <summary>
        /// Comprueba si la ID de sistema pasada por parámetro
        /// está o no en uso.
        /// </summary>
        /// <param name="key">ID a verificar si está en uso</param>
        /// <returns>Si está o no en uso</returns>
        public bool isKeyUsed(string key)
        {
            System salida;
            return instance.systemDictionary.TryGetValue(key, out salida);
        }

        /// <summary>
        /// Función auxiliar para mostrar por consola
        /// el contenido del diccionario de sistemas.
        /// </summary>
        public void DebugSystemDictionary()
        {
            foreach (KeyValuePair<string, System> system in systemDictionary)
            {
                Debug.Log(system.Key + " " + system.Value);
            }
        }

        /// <summary>
        /// Devulve una lista de string con los nombres
        /// de todos los sistemas en orden.
        /// </summary>
        /// <returns>La lista de nombres de los sistemas</returns>
        public string[] GetAllSystemsNameList()
        {
            List<string> list = new List<string>();
            list.Add(gameObject.name);
            foreach (System system in allSystems)
            {
                list.Add(system.gameObject.name);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Dada una ID de sistema retorna el sistema
        /// asociado si hay alguno, si no lo hay retornará nulo.
        /// </summary>
        /// <param name="key">ID del sistema buscado</param>
        /// <returns>Referencia al sistema si existe</returns>
        public static System getSystem(string key)
        {
            System salida;
            instance.systemDictionary.TryGetValue(key, out salida);
            return salida;
        }

        /// <summary>
        /// Elimina el sistema pasado por parámetro
        /// de la lista de sistemas y del diccionario.
        /// </summary>
        /// <param name="system">Referencia al sistema a borrar</param>
        public void EraseSystem(System system)
        {
            instance.AllSystems.Remove(system);
            instance.systemDictionary.Remove(system.SystemID);
        }

        /// <summary>
        /// Elimina una unidad de la lista total de unidades del root system
        /// </summary>
        /// <param name="unit">Referencia de la unidad a borrar</param>
        public void EraseUnit(Unit unit)
        {
            instance.AllUnits.Remove(unit);
        }

        /// <summary>
        /// Añade una unidad a la lista total de unidades del root system
        /// </summary>
        /// <param name="actual">Unidad actual a añadir</param>
        public static void AddUnit(Unit actual)
        {
            instance.allUnits.Add(actual);
        }

        /// <summary>
        /// Añade un sistema a la lista total de sistemas del root system
        /// y al diccionario de sistemas.
        /// </summary>
        /// <param name="actual">El sistema actual a añadir</param>
        public static void AddSystem(System actual)
        {
            instance.allSystems.Add(actual);
            instance.systemDictionary.Add(actual.SystemID, actual);
        }
    }
}