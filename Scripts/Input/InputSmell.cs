using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SystemicDesign
{
    /// <summary>
    /// Input que simula un olfato o un radio de detección de partículas.
    /// Usa un collider y se queda a la escucha de todas las partículas que entren en
    /// contacto con dicho radio, luego consume la partícula y trata el estímulo
    /// </summary>
    public class InputSmell : Input
    {
        /// <summary>
        /// Collider esférico que sirve de zona de detección de partículas
        /// </summary>
        [SerializeField] private SphereCollider detectionCollider;
        /// <summary>
        /// Lista de estímulos que puede percibir este input
        /// </summary>
        [SerializeField] private List<string> stimuli = new List<string>();
        /// <summary>
        /// Lista de métodos de activación que se activarán según el estímulo recibido
        /// </summary>
        [SerializeField] private List<UnityEvent> activationMethods = new List<UnityEvent>();

        /// <summary>
        /// Cuando el collider asociado al olfato capta que algo, se comprueba si es una particula.
        /// Si es una partícula y no es proveniente de esta misma entidad, la consume y analiza el estímulo.
        /// Si no lo es se ignora.
        /// </summary>
        /// <param name="other">El collider del objeto entrante</param>
        private void OnTriggerEnter(Collider other)
        {

            if (!Activated || (!infiniteActivations && actualNumActivations >= maxNumOfActivations)) return;
            Particle particle = other.gameObject.GetComponent<Particle>();
            if (particle == null || particle.Author == this.Entity) return;
            string stimulus = particle.ConsumeParticle();
            if (!stimuli.Contains(stimulus)) return;
            EvaluateStimulus(stimulus);
            if (debug) Debug.Log("Input smell: se ha evaluado un olor a " + stimulus);
        }

        /// <summary>
        /// Evalua un estímulo detectado por los colliders anteriormente y compruba
        /// si tiene asociado un conjunto de métodos a ejecutar.
        /// </summary>
        /// <param name="stimulus"> El estímulo detectado </param>
        /// <returns> Si se ha invocado o no métodos asociados al estímulo</returns>
        private bool EvaluateStimulus(string stimulus)
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

        /// <summary>
        /// Método génirico que muestra el nombre del componente sistémico en cuestión
        /// </summary>
        /// <returns>Nombre del componente sistémico</returns>
        public override string ToString()
        {
            return "Input Smell";
        }
    }
}
