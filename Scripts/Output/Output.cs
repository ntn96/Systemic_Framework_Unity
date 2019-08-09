using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Output sistémico destinado a ser una puerta de salida
    /// de información en forma de estímulos desde la entidad sistémica
    /// hacia el exterior.
    /// </summary>
    public abstract class Output : Activable
    {
        /// <summary>
        /// Referencia a la entidad a la que está enlazada el componente sistémico
        /// esta será la que represente a la entidad IA de la que está emitiendo información
        /// </summary>
        [SerializeField] protected Entity entity;

        /// <summary>
        /// Referencia a la entidad a la que está enlazada el componente sistémico
        /// esta será la que represente a la entidad IA de la que está emitiendo información
        /// </summary>
        public Entity Entity
        {
            get { return entity; }
        }
    }
}
