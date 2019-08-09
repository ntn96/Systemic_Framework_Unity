using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Entidad que representa un agente IA concreto final sin subentidades.
    /// </summary>
    public class Unit : Entity
    {
        /// <summary>
        /// Id del sistema padre que contiene a esta unidad
        /// </summary>
        [SerializeField] private string parentSystemKey;

        /// <summary>
        /// Cuando la unidad aparece trata de enlazarse con su sistema
        /// padre.
        /// </summary>
        private void Awake()
        {
            if (parentSystem == null)
            {
                System parent = RootSystem.getSystem(parentSystemKey);
                if (parent == null) return;
                parentSystem = parent;
                if (!parentSystem.Entities.Contains(this))
                    parentSystem.AddEntity(this);
            }
            if (!RootSystem.Instance.AllUnits.Contains(this))
                RootSystem.Instance.AllUnits.Add(this);
        }

        /// <summary>
        /// Cuando la unidad sea destruida borra su referencia
        /// del sistema padre y del rootSystem.
        /// </summary>
        private void OnDestroy()
        {
            if (parentSystem != null) parentSystem.EraseEntity(this);
            RootSystem.Instance.AllUnits.Remove(this);
        }
    }
}
