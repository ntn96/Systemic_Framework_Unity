using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Output directo que sirve como conexión directa a entidades.
    /// Cuando se activa se puede enviar un estímulo directamente al input direct
    /// connection de la entidad objetivo
    /// </summary>
    public class OutputDirectConnection : OutputMultiple
    {
        /// <summary>
        /// Lista de entidades que esta entidad puede estimular directamente desde
        /// este output a sus respectivos inputs.
        /// </summary>
        [SerializeField] List<Entity> entities = new List<Entity>();

        /// <summary>
        /// Función usada para activar el output y enviar a la entidad
        /// índicada por el índice el estímulo indicado por el índice.
        /// </summary>
        /// <param name="index">Índice que determina el par entidad estímulo a activar</param>
        /// <returns></returns>
        public bool SendStimulus(int index)
        {
            if (index + 1 > stimuli.Count || index + 1 > entities.Count || entities[index] == null) return false;
            else return entities[index].SendDirectStimulus(stimuli[index]);
        }

        /// <summary>
        /// Método génirico que muestra el nombre del componente sistémico en cuestión
        /// </summary>
        /// <returns>Nombre del componente sistémico</returns>
        public override string ToString()
        {
            return "Output Direct Connection";
        }
    }
}
