using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Clase que representa una entidad IA cualquiera,
    /// que puede ser una unidad, que es un agente concreto IA sobre el terreno,
    /// o un sistema con otras subentidades.
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        /// <summary>
        /// Referencia al sistema padre que contiene a esta entidad
        /// </summary>
        [SerializeField] protected System parentSystem;
        // El conjunto de las referencias de todos los componentes sistémicos que 
        // puede llegar a tener una entidad sistémica.
        [SerializeField] protected InputDirectConnection inputDirect;
        [SerializeField] protected OutputDirectConnection outputDirect;
        [SerializeField] protected OutputBroadcast outputBroadcast;
        [SerializeField] protected List<OutputPresence> outputsPresence = new List<OutputPresence>();
        [SerializeField] protected List<OutputPresenceActivation> outputsPresenceActivation = new List<OutputPresenceActivation>();
        [SerializeField] protected List<InputPeriodicActivation> inputsPeriodicActivation = new List<InputPeriodicActivation>();
        [SerializeField] protected List<InputRandomActivation> inputsRandomActivation = new List<InputRandomActivation>();
        [SerializeField] protected List<InputVision> inputsVision = new List<InputVision>();
        [SerializeField] protected List<InputSmell> inputsSmell = new List<InputSmell>();
        [SerializeField] protected List<OutputEmitParticle> outputEmit = new List<OutputEmitParticle>();

        /// <summary>
        /// Referencia al input direct connection de esta
        /// entidad si es que lo tiene.
        /// </summary>
        public InputDirectConnection InputDirect
        {
            get { return inputDirect; }
        }

        /// <summary>
        /// Referencia al sistema padre que contiene a esta entidad
        /// </summary>
        public System ParentSystem
        {
            get { return parentSystem; }
            set { parentSystem = value; }
        }

        /// <summary>
        /// Método utilizado por los usarios para iniciar una entidad
        /// una vez que se instancia una durante el in-game.
        /// </summary>
        /// <param name="systemKey">ID del sistema padre que va a albergar a la entidad</param>
        public void InitEntity(string systemKey)
        {
            System parent = RootSystem.getSystem(systemKey);
            InitEntity(parent);
        }

        /// <summary>
        /// Método utilizado por los usarios para iniciar una entidad
        /// una vez que se instancia una durante el in-game.
        /// </summary>
        /// <param name="systemKey">Referencia directa al sistema padre que lo va a albergar</param>
        public void InitEntity(System parent)
        {
            if (parent != null)
            {
                parentSystem = parent;
                parent.AddEntity(this);
            }
            else
            {
                Debug.LogWarning("Entity-InitEntity: El sistema no existe");
            }
        }

        /// <summary>
        /// Trata de enviar un estímulo pasado por parámetro a
        /// input direct connection si es que hay alguno. 
        /// Si no hay ninguno retorna false.
        /// </summary>
        /// <param name="stimulus">El estímulo a enviar</param>
        /// <returns>booleano que representa si se ha podido o no enviar el estímulo</returns>
        public bool SendDirectStimulus(string stimulus)
        {
            if (inputDirect == null) return false;
            return inputDirect.EvaluateStimulus(stimulus);
        }

        /// <summary>
        /// Cuando la entidad se destruye
        /// se borra la referencia a la entidad en
        /// el sistema padre y en el root system.
        /// </summary>
        private void OnDestroy()
        {
            if (parentSystem != null)
                parentSystem.EraseEntity(this);
            if (GetType() == typeof(System))
                RootSystem.Instance.EraseSystem((System)this);
            else if (GetType() == typeof(Unit))
                RootSystem.Instance.EraseUnit((Unit)this);
            else if (GetType() == typeof(SmellSystem))
                RootSystem.Instance.EraseSystem((SmellSystem)this);
        }
    }
}
