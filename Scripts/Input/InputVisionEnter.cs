using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SystemicDesign
{
    /// <summary>
    /// Implementación concreta del input de vision basada en ejecutar
    /// una función de entrada cuando el collider entre y una función de salida cuando el
    /// collider salga. La cual es una implementación mucho más eficiente que su contraparte.
    /// </summary>
    public class InputVisionEnter : InputVision
    {
        /// <summary>
        /// Lista de conjuntos de métodos ejecutados a la salida de
        /// estímulos del interior del collider trigger
        /// </summary>
        [SerializeField] private List<UnityEvent> exitMethods = new List<UnityEvent>();

        /// <summary>
        /// Función ejecutada al entrar en el collider trigger
        /// </summary>
        /// <param name="other"> Collider que collisiona con el trigger</param>
        private void OnTriggerEnter(Collider other)
        {
            ExecuteInput(other);
        }

        /// <summary>
        /// Función ejecutada al salir del collider trigger
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            if (!activated) return;
            OutputSimple output = other.gameObject.GetComponent<OutputSimple>();
            if (output != null && stimuli.Contains(output.Stimulus))
            {
                if (output.Entity == this.Entity)
                {
                    Debug.LogWarning("Input vision Exit: una entidad ha intentado estimularse a sí misma por medio de " + output.Stimulus);
                    return;
                }
                if (debug) Debug.Log(this.Entity.gameObject.name + " termina de recibir un estímulo de " + output.Entity.gameObject.name);
                if (!EvaluateExit(output.Stimulus))
                    Debug.LogWarning("Input Vision: no se ha encontrado métodos de salida para el estímulo capturado " + output.Stimulus);
            }
        }

        /// <summary>
        /// Ejecuta el conjunto de métodos a invocar a la salida
        /// de un estímulo concreto del collider trigger
        /// </summary>
        /// <param name="stimulus"></param>
        /// <returns></returns>
        private bool EvaluateExit(string stimulus)
        {
            int index = stimuli.IndexOf(stimulus);
            if (exitMethods.Count < index + 1)
                return false;
            else
            {
                exitMethods[index].Invoke();
                if (debug) Debug.Log("Ejecuta OnTriggerExit");
                return true;
            }
        }

        /// <summary>
        /// Método génirico que muestra el nombre del componente sistémico en cuestión
        /// </summary>
        /// <returns>Nombre del componente sistémico</returns>
        public override string ToString()
        {
            return "Input Vision Enter";
        }
    }
}
