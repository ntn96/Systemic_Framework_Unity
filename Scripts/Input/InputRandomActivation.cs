using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SystemicDesign
{
    /// <summary>
    /// Input destinado a una activación aleatoria: Cada cierto
    /// tiempo se comprueba una probabilidad de activación, si se cumple se
    /// activan también los métodos correspondientes.
    /// </summary>
    public class InputRandomActivation : InputPeriodicActivation
    {
        /// <summary>
        /// Probabilidad que tienen los métodos de activación de activarse
        /// la próxima vez que se cumpla el tiempo
        /// </summary>
        [SerializeField] private float activationProbability = 0.5f;
        /// <summary>
        /// Tasa en la que asciende la probabilidad cada vez que no se comprueba la probabilidad 
        /// y no se activan los métodos
        /// </summary>
        [SerializeField] private float increasingProbability = 0.05f;
        /// <summary>
        /// Valor en segundos de tiempo extra que se espera tras una activación exitosa
        /// </summary>
        [SerializeField] private float activationExtraTime = 0f;
        /// <summary>
        /// Determina si tras una activación la probabilidad de activación retorna a su estado incial,
        /// o permanece con el valor incrementado.
        /// </summary>
        [SerializeField] private bool resetAfterActivation = true;
        /// <summary>
        /// Determina si la probabilidad de activación es o no calculada por un método externo o no.
        /// Si es el caso entonces para calcular la probabilidad se llama a los métodos de probabilidad
        /// que deberán encargarse de modificar el valor de la probabilidad de activación de este input
        /// por su cuenta
        /// </summary>
        [SerializeField] private bool probabilityByMethod = false;
        /// <summary>
        /// Métodos de probabilidad que deberán encargarse de modificar el valor de la 
        /// probabilidad de activación de este input por su cuenta
        /// </summary>
        [SerializeField] private UnityEvent probabilityMethods;
        /// <summary>
        /// Valor auxiliar que guarda el valor original de la probabilidad de
        /// activación para resetear el actual tras una activación
        /// </summary>
        private float initialActivationProb;
        /// <summary>
        /// Probabilidad que tienen los métodos de activación de activarse
        /// la próxima vez que se cumpla el tiempo
        /// </summary>
        public float ActivationProbability
        {
            get { return activationProbability; }
            set { activationProbability = value; }
        }

        /// <summary>
        /// Al arrancar este input guarda el valor incial de la probabilidad de activación.
        /// </summary>
        protected override void Start()
        {
            initialActivationProb = activationProbability;
            base.Start();
        }

        /// <summary>
        /// Lanza la corrutina de tiempo que se encargará de comprobar periódicamente
        /// si se cumple o no la activación de los métodos.
        /// </summary>
        protected override void IniciarCorrutina()
        {
            StartCoroutine(CheckActivation());
        }

        /// <summary>
        /// Corrutina que al activarse comprueba si se cumple la probabilidad de activación y si es así,
        /// invoca a los métodos de activación.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckActivation()
        {
            while (activated)
            {
                if (!infiniteActivations && actualNumActivations >= maxNumOfActivations) break;
                float calculatedTime = activationTime + Random.Range(0f, extraRandomTime);
                yield return new WaitForSeconds(calculatedTime);
                if (probabilityByMethod) probabilityMethods.Invoke();
                if (Random.value <= activationProbability)
                {
                    actualNumActivations++;
                    if (resetAfterActivation) activationProbability = initialActivationProb;
                    activationMethods.Invoke();
                    yield return new WaitForSeconds(activationExtraTime);
                }
                else
                {
                    activationProbability = Mathf.Min(1f, activationProbability + increasingProbability);
                }
            }
        }

        /// <summary>
        /// Método génirico que muestra el nombre del componente sistémico en cuestión
        /// </summary>
        /// <returns>Nombre del componente sistémico</returns>
        public override string ToString()
        {
            return "Input Random Activation";
        }
    }
}
