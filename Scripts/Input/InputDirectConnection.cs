using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SystemicDesign
{
    /// <summary>
    /// Input de conexión directa que evalua el estímulo recibido y si tiene algún conjunto de métodos asociado a dicho
    /// estímulo realiza su invocación.
    /// </summary>
    public class InputDirectConnection : Input
    {
        /// <summary>
        /// Lista de estímulos que este input está escuchando, si recibe uno
        /// de ellos ejecutará un conjunto de métodos asociado
        /// </summary>
        [SerializeField] private List<string> stimuli = new List<string>();
        /// <summary>
        /// Lista de conjuntos de métodos asociados a cada uno de los estímulos,
        /// a los que el input está a la escucha.
        /// </summary>
        [SerializeField] private List<UnityEvent> activationMethods = new List<UnityEvent>();
        /// <summary>
        /// Booleano que determina si el input, una vez recibido un estímulo, va 
        /// a reenviar el estímulo a través de las subentidades.
        /// Solo tiene utilidad si la entidad asociada es un sistema y tiene asociado
        /// un output broadcast.
        /// </summary>
        [SerializeField] private bool rebroadcast = false;


        /// <summary>
        /// Evalua el estímulo recibido y si tiene algún conjunto de métodos asociado a dicho
        /// estímulo realiza su invocación.
        /// </summary>
        /// <param name="stimulus"> El estímulo recibido </param>
        /// <returns> Si se ha captado un estímulo correctamente y se han ejecutado sus acciones asociadas </returns>
        public bool EvaluateStimulus(string stimulus)
        {
            if (!activated || (!infiniteActivations && actualNumActivations >= maxNumOfActivations))
            {
                if (debug) Debug.LogWarning("InputDirectConnection máximo número de activaciones alcanzado");
                return false;
            }
            if (!stimuli.Contains(stimulus))
            {
                if (debug) Debug.LogWarning("InputDirectConnection ha recibido un estimulo al que no está a la escucha");
                return false;
            }
            if (rebroadcast)
            {
                OutputBroadcast output = GetComponent<OutputBroadcast>();
                if (output != null) output.BroadcastStimulus(stimulus);
                else if (debug) Debug.LogWarning("InputDirectConnection ha tratado de reemitir un estimulo sin que haya output broadcast");
            }
            int index = stimuli.IndexOf(stimulus);
            if (activationMethods.Count < index + 1)
                return false;
            else
            {
                activationMethods[index].Invoke();
                actualNumActivations++;
                return true;
            }
        }

        /// <summary>
        /// Método génirico que muestra el nombre del componente sistémico en cuestión
        /// </summary>
        /// <returns>Nombre del componente sistémico</returns>
        public override string ToString()
        {
            return "Input Direct Connection";
        }
    }
}
