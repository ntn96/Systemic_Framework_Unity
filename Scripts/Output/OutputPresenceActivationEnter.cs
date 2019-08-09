using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Implementación específica del output presence activation
    /// que consiste en que el estímulo se envía una vez por cada vez que
    /// el collider de la otra entidad entra en el collider de este output.
    /// </summary>
    public class OutputPresenceActivationEnter : OutputPresenceActivation
    {
        /// <summary>
        /// Se activa cuando un collider entra en contacto con otra
        /// una única vez
        /// </summary>
        /// <param name="other">El collider de la otra entidad</param>
        private void OnTriggerEnter(Collider other)
        {
            if (activated) SpreadStimulus(other);
        }

        /// <summary>
        /// Método génirico que muestra el nombre del componente sistémico en cuestión
        /// </summary>
        /// <returns>Nombre del componente sistémico</returns>
        public override string ToString()
        {
            return "Output Presence Activation Enter";
        }
    }
}
