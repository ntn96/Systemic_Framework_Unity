using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Output destinado para los sistemas que se usa
    /// para retransmitir un estímulo entrante hacia las distintas
    /// subentidades contenidas en el sistema. Lo cual permite
    /// una transmisión rápida de estímulos a gran número de entidades.
    /// </summary>
    public class OutputBroadcast : Output
    {
        /// <summary>
        /// Booleano que determina si la entidad asociada al output es un sistema o no
        /// </summary>
        private bool isSystem = true;
        /// <summary>
        /// Booleano que determina si la entidad asociada es el root system o no
        /// </summary>
        private bool isRoot = false;
        /// <summary>
        /// Referencia al sistema al que está asociado este output
        /// </summary>
        private System actualSystem;
        /// <summary>
        /// Referencia al sistema raíz, si es el sistema raíz el sistema asociado.
        /// </summary>
        private RootSystem root;

        /// <summary>
        /// Cuando el output se activa detecta si la entidad asociada es un sistema, una unidad o el sistema raíz.
        /// Si es una unidad muestra un mensaje de error y no deja ejecutarse.
        /// </summary>
        private void Awake()
        {
            if (entity.GetType() == typeof(Unit))
            {
                Debug.LogWarning("OutputBroadcast: No se está aplicando sobre un sistema sino sobre una unidad", this);
                isSystem = false;
            }
            else if (entity.GetType() == typeof(RootSystem))
            {
                root = (RootSystem)entity;
                isRoot = true;
                isSystem = false;
            }
            else
            {
                actualSystem = (System)entity;
            }
        }

        /// <summary>
        /// Retransmite el estímulo recibido a todas las subentidades si la entidad
        /// actual es un sistema. Si la entidad es el sistema raíz lo transmite a todos los sistemas
        /// </summary>
        /// <param name="stimulus">Estímulo a retransmitir</param>
        public void BroadcastStimulus(string stimulus)
        {
            if (isSystem)
            {
                List<Entity> entities = actualSystem.Entities;
                for (int i = 0; i < entities.Count; i++)
                    entities[i].SendDirectStimulus(stimulus);
                Debug.Log("Se ha emitido un broadcast de estímulo");
            }
            else if (isRoot)
            {
                for (int i = 0; i < root.AllSystems.Count; i++)
                    root.AllSystems[i].SendDirectStimulus(stimulus);
            }
        }

        /// <summary>
        /// Método génirico que muestra el nombre del componente sistémico en cuestión
        /// </summary>
        /// <returns>Nombre del componente sistémico</returns>
        public override string ToString()
        {
            return "Output Broadcast";
        }
    }
}
