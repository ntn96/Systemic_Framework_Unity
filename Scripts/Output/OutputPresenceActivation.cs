using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Output de presencia especial que envía estímulos
    /// a los input direct connection de las entidades que detecte
    /// su collider que esté entre los objetos estímulables que este output
    /// tiene listado. Cuando encuentra una output de presencia de 
    /// otra entidad dentro del collider de este output y su estímulo
    /// asociado está entre los estímulos de los objetos estímulables
    /// entonces envía el estímulo.
    /// </summary>
    public abstract class OutputPresenceActivation : OutputSimple
    {
        /// <summary>
        /// Collider de detección de colliders de presencia de otras entidades
        /// </summary>
        [SerializeField] private Collider presenceCollider;
        /// <summary>
        /// Lista de estímulos que identifican, según su presencia, a las entidades estimulables.
        /// </summary>
        [SerializeField] private List<string> stimulableObjects = new List<string>();

        /// <summary>
        /// Se ejecuta una vez por frame y se encarga de habilitar y deshabilitar
        /// el collider según el estado de este output
        /// </summary>
        private void Update()
        {
            presenceCollider.enabled = activated;
        }

        /// <summary>
        /// Método que dado un collider de otra entidad que se ha detectado por el
        /// collider de esta entidad, determina si es un collider de presencia, luego
        /// que esa entidad no sea esta misma y que su estimulo de presencia se encuentre
        /// entre la lista de objetos estimulables, si es así transmite el estímulo asociado
        /// por input de conexión directa.
        /// </summary>
        /// <param name="other"></param>
        protected void SpreadStimulus(Collider other)
        {
            if (actualNumActivations >= maxNumOfActivations) return;
            OutputPresence output = other.gameObject.GetComponent<OutputPresence>();
            if (debug) Debug.Log(Entity.gameObject.name + " encuentra " + output, output);
            if (output == null || output.Entity == this.Entity) return;
            if (output != null && stimulableObjects.Contains(output.Stimulus))
                if (output.Entity.SendDirectStimulus(stimulus)) actualNumActivations++;
        }
    }
}
