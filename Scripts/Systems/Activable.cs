using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign {
    /// <summary>
    /// Clase con funcionalidad común a todos los componentes sistémicos
    /// que permiten activar y desactivar los componentes y poner un
    /// límite a las veces que puede activarse.
    /// </summary>
    public class Activable : MonoBehaviour {
        /// <summary>
        /// Booleano que deterina si se muestra o no información de debug.
        /// </summary>
        [SerializeField] protected bool debug = false;
        /// <summary>
        /// Determina si el componente sistémico está o no activado y puede por
        /// tanto entrar en acción o no.
        /// </summary>
        [SerializeField] protected bool activated = true;
        /// <summary>
        /// Índica si el componente sistémico puede o no activarse las veces que se
        /// quiera indefinidamente.
        /// </summary>
        [SerializeField] protected bool infiniteActivations = true;
        /// <summary>
        /// Número máximo de veces que se permite que se active el componente sistémico
        /// </summary>
        [SerializeField] protected int maxNumOfActivations = 100;
        /// <summary>
        /// Número actual de veces que se ha activado el componente sistémico hasta ahora
        /// </summary>
        [SerializeField] protected int actualNumActivations = 0;

        /// <summary>
        /// Determina si el componente sistémico está o no activado y puede por
        /// tanto entrar en acción o no.
        /// </summary>
        public bool Activated
        {
            get { return activated; }
            set { activated = value; }
        }

        /// <summary>
        /// Resetea el valor del número actual de veces que se ha 
        /// activado el componente sistémico, permitiendo que pueda volver
        /// a usarse una vez se ha llegado al máximo.
        /// </summary>
        public void ResetActivations()
        {
            actualNumActivations = 0;
        }
    }
}
