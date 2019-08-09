using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Implementación concreta del input de vision basada en ejecutar
    /// el input en cada frame que el estímulo se mantenga en el interior del collider.
    /// Habrá de usarse con cuidado pues puede consumir bastante tiempo de ejecución de manera
    /// inutil por cada instancia que lo use.
    /// </summary>
    public class InputVisionStay : InputVision
    {

        /// <summary>
        /// Función que se ejecuta durante cada frame en el que el collider trigger
        /// tenga otro collider en colisión
        /// </summary>
        /// <param name="other"> El collider que entra en colisión con el trigger</param>
        private void OnTriggerStay(Collider other)
        {
            ExecuteInput(other);
        }

        /// <summary>
        /// Método génirico que muestra el nombre del componente sistémico en cuestión
        /// </summary>
        /// <returns>Nombre del componente sistémico</returns>
        public override string ToString()
        {
            return "Input Vision Stay";
        }
    }
}
