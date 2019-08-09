using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Input sistémico destinado a ser una puerta de entrada
    /// de información en forma de estímulos hacia la entidad sistémica
    /// para que esta pueda procesar dicha información con independencia
    /// del método de extracción de la información o el autor.
    /// </summary>
    public abstract class Input : Activable
    {
        /// <summary>
        /// Referencia a la entidad a la que está enlazada el componente sistémico
        /// esta será la que represente a la entidad IA a la que el input está sirviendo información
        /// </summary>
        [SerializeField] protected Entity entity;

        /// <summary>
        /// Referencia a la entidad a la que está enlazada el componente sistémico
        /// esta será la que represente a la entidad IA a la que el input está sirviendo información
        /// </summary>
        public Entity Entity
        {
            get { return entity; }
        }
    }
}
