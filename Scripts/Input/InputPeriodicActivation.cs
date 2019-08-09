using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SystemicDesign
{
    /// <summary>
    /// Input que cada cierto tiempo se activa automáticamente un método asociado
    /// </summary>
    public class InputPeriodicActivation : Input
    {
        /// <summary> Tiempo que tarda que en activar el método la corrutina asociada 
        /// </summary>
        [SerializeField] protected float activationTime = 0;
        /// <summary> Tiempo aleatorio que se le suma al tiempo de activación para que no sean tiempos tan estrictos,
        /// por lo que el tiempo de activación final será el tiempo de activación más un número aleatorio 
        /// entreo 0 y el tiempo extra aleatorio.</summary>
        [SerializeField] protected float extraRandomTime = 0;
        /// <summary> Métodos a ejecutar cuando se cumple el tiempo de activación final. 
        /// </summary>
        [SerializeField] protected UnityEvent activationMethods;
        /// <summary> Booleano auxiliar para activar y desactivar correctamente las corrutinas de 
        /// activación del input </summary>
        protected bool lastActivatedValue;



        /// <summary> Inicialización del input 
        /// </summary>
        protected virtual void Start()
        {
            lastActivatedValue = activated;
            if (activated) StartCoroutine(PeriodicActivation());
        }

        /// <summary>
        /// Se ejecuta una vez por frame y se encarga de activar y desactivar correctamente el input cuando proceda
        /// </summary>
        protected void Update()
        {
            if (!lastActivatedValue && activated)
            {
                lastActivatedValue = activated;
                IniciarCorrutina();
            }
            else if (lastActivatedValue && !activated)
            {
                lastActivatedValue = activated;
            }
        }

        /// <summary>
        /// Inicia el funcionamiento del input Periodic Activation.
        /// Cada cierto tiempo la corrutina se activa y con ella los métodos
        /// de activación pertinentes
        /// </summary>
        protected virtual void IniciarCorrutina()
        {
            StartCoroutine(PeriodicActivation());
        }

        /// <summary>
        /// Comprueba si se puede activar el input, calcula el tiempo de activación, espera hasta
        /// que se cumpla y entonces ejecuta el método asociado.
        /// </summary>
        /// <returns> Esto es una corrutina y de ahí el IEnumerator</returns>
        private IEnumerator PeriodicActivation()
        {
            while (activated)
            {
                if (!infiniteActivations && actualNumActivations >= maxNumOfActivations) break;
                float calculatedTime = activationTime + Random.Range(0, extraRandomTime);
                yield return new WaitForSeconds(calculatedTime);
                actualNumActivations++;
                activationMethods.Invoke();
                if (debug) Debug.LogWarning("Activation " + calculatedTime);
            }
        }

        /// <summary>
        /// Cuando se deshabilita este input se detienen todas las corrutinas.
        /// Cuando se reactiva el input se vuelve a iniciar
        /// </summary>
        protected void OnDisable()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Método génirico que muestra el nombre del componente sistémico en cuestión
        /// </summary>
        /// <returns>Nombre del componente sistémico</returns>
        public override string ToString()
        {
            return "Input Periodic Activation";
        }
    }
}