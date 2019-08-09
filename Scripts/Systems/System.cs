using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Entidad concreta que representa un sistema de otras entiedades.
    /// Organiza subentidades que a su vez pueden ser unidades u otros sistemas.
    /// </summary>
    public class System : Entity
    {
        /// <summary>
        /// ID unica para identificar a este sistema
        /// </summary>
        [SerializeField] protected string systemID;
        /// <summary>
        /// Lista de unidades que este sistema contiene, organiza y gestiona.
        /// </summary>
        [SerializeField] private List<Entity> entities = new List<Entity>();

        /// <summary>
        /// ID unica para identificar a este sistema
        /// </summary>
        public string SystemID
        {
            get
            {
                return systemID;
            }
        }

        /// <summary>
        /// Lista de unidades que este sistema contiene, organiza y gestiona.
        /// </summary>
        public List<Entity> Entities
        {
            get
            {
                return entities;
            }
        }

        /// <summary>
        /// Añade una nueva entidad al sistema.
        /// </summary>
        /// <param name="entity">La entidad a añadir</param>
        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }

        /// <summary>
        /// Borra una entidad de entre las que el sistema contiene
        /// </summary>
        /// <param name="entity">Entidad a borrar</param>
        public void EraseEntity(Entity entity)
        {
            entities.Remove(entity);
        }
    }
}
