using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SystemicDesign
{
    /// <summary>
    /// Input de visión que es capaz de percibir entidades que se introduzcan
    /// en el interior de un collider trigger
    /// </summary>
    public abstract class InputVision : Input
    {
        /// <summary>
        /// Collider asociado que será el encargado de percibir
        /// los estímulos con los que contacten
        /// </summary>
        [SerializeField] private Collider visionCollider;
        /// <summary>
        /// Lista de estímulos que este input está escuchando, si recibe uno
        /// de ellos ejecutará un conjunto de métodos asociado
        /// </summary>
        [SerializeField] protected List<string> stimuli = new List<string>();
        /// <summary>
        /// Lista de conjuntos de métodos asociados a cada uno de los estímulos,
        /// a los que el input está a la escucha.
        /// </summary>
        [SerializeField] private List<UnityEvent> activationMethods = new List<UnityEvent>();

        /// <summary>
        /// Método que se encarga de asegurarse que el collider se active solo
        /// cuando esté activo el input y se desactive cuando este lo haga
        /// </summary>
        private void Update()
        {
            visionCollider.enabled = activated;
        }

        /// <summary>
        /// Una vez se han detectado un collider entrante (dependerá de la implementación el cómo)
        /// se comprueba si este contiene un estímulo y si es así lo evalua
        /// </summary>
        /// <param name="other"> Collider que ha sido detectado por el input</param>
        protected void ExecuteInput(Collider other)
        {
            if (!activated || (!infiniteActivations && actualNumActivations >= maxNumOfActivations)) return;
            OutputSimple output = other.gameObject.GetComponent<OutputSimple>();
            if (output != null && stimuli.Contains(output.Stimulus))
            {
                if (output.Entity == this.Entity)
                {
                    Debug.LogWarning("Input vision: una entidad ha intentado estimularse a sí misma por medio de " + output.Stimulus, this);
                    return;
                }
                if (debug) Debug.Log(this.Entity.gameObject.name + " recibe un estímulo de " + output.Entity.gameObject.name);
                if (!EvaluateStimulus(output.Stimulus))
                    Debug.LogWarning("Input Vision: no se ha encontrado métodos de invocación para el estímulo capturado " + output.Stimulus);
                else actualNumActivations++;
            }
        }

        /// <summary>
        /// Evalua un estímulo detectado por los colliders anteriormente y compruba
        /// si tiene asociado un conjunto de métodos a ejecutar.
        /// </summary>
        /// <param name="stimulus"> El estímulo detectado </param>
        /// <returns> Si se ha invocado o no métodos asociados al estímulo</returns>
        protected bool EvaluateStimulus(string stimulus)
        {
            int index = stimuli.IndexOf(stimulus);
            if (activationMethods.Count < index + 1)
                return false;
            else
            {
                activationMethods[index].Invoke();
                return true;
            }
        }
    }
}
