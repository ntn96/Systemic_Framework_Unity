using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SystemicDesign
{
    /// <summary>
    /// Ouput que se enlaza con un collider que sea representativo de la esencia, es decir
    /// que represente el cuerpo de la entidad o de su área de influencia, y que le asigna
    /// un estímulo identificativo de la información que transmita su presencia.
    /// </summary>
    public class OutputPresence : OutputSimple
    {
        /// <summary>
        /// Collider asociado que representa la presencia de la entidad
        /// </summary>
        [SerializeField] private Collider presenceCollider;

        #region InternalParams
        /// <summary>
        /// Deshabilitador del collider asociado para cuando se quiera
        /// desactivar el output.
        /// </summary>
        private ColliderDisabler colliderDisabler;
        /// <summary>
        /// Booleano que determina si el collider está o no minimizado en tamaño
        /// </summary>
        private bool minimized = false;
        #endregion

        /// <summary>
        /// Al iniciarse el output crea una nueva instancia del
        /// deshabilitador de colliders.
        /// </summary>
        private void Start()
        {
            colliderDisabler = new ColliderDisabler();
        }

        /// <summary>
        /// Función ejecutada durante todos los frames al finalizar
        /// el cálculo de físicas que sirve para detectar el cambio de
        /// activo a inactivo y viceversa del collider, para usar correctamente
        /// el collider disabler.
        /// </summary>
        private void FixedUpdate()
        {
            if (presenceCollider.enabled == true && activated == false)
            {
                if (!minimized)
                {
                    colliderDisabler.MinimizeCollider(presenceCollider);
                    minimized = true;
                    if (debug) Debug.Log("Ejecuta minimizado de collider");
                }
                else
                {
                    colliderDisabler.RestoreColliderSize();
                    minimized = false;
                    presenceCollider.enabled = activated;
                    if (debug) Debug.Log("Ejecuta redimensionado de collider");
                }
            }
            else presenceCollider.enabled = activated;
        }

        /// <summary>
        /// Método génirico que muestra el nombre del componente sistémico en cuestión
        /// </summary>
        /// <returns>Nombre del componente sistémico</returns>
        public override string ToString()
        {
            return "Output Presence";
        }
    }
}
