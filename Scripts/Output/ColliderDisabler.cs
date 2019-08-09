using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SystemicDesign
{
    /// <summary>
    /// Clase auxiliar que sirve para que Unity detecte que un collider
    /// ha salido de contactar con otro cuando se inhabilita. Para ello
    /// Se almacenan los valores clave del collider, se minimiza a tamaño cero,
    /// entonces Unity detecta que el collider ha salido y se puede deshabilitar.
    /// Para habilitarlo de nuevo se vuelve a maximizar con los valores originales.
    /// Es compatible con los colliders box, capsule, sphere y mesh.
    /// Se aplica en output de presencia.
    /// </summary>
    public class ColliderDisabler
    {
        // Variables para guardar las proporciones originales de los colliders
        // y referencias auxiliares
        private BoxCollider auxBoxCollider;
        private CapsuleCollider auxCapsuleCollider;
        private SphereCollider auxSphereCollider;

        private Vector3 initialBoxColliderSize;

        private float initialCapsuleRadius;
        private float initialCapsuleHeight;

        private float initialSphereRadius;

        private Vector3 initialMeshColliderSize;
        private Transform auxMeshColliderTransform;

        /// <summary>
        /// Minimiza el collider a tamaño cero
        /// guardando los valores originales
        /// </summary>
        /// <param name="presenceCollider">Collider a minimizar</param>
        public void MinimizeCollider(Collider presenceCollider)
        {
            Type colliderType = presenceCollider.GetType();
            if (colliderType == typeof(BoxCollider))
            {
                auxBoxCollider = (BoxCollider)presenceCollider;
                initialBoxColliderSize = auxBoxCollider.size;
                auxBoxCollider.size = Vector3.zero;
            }
            else if (colliderType == typeof(CapsuleCollider))
            {
                auxCapsuleCollider = (CapsuleCollider)presenceCollider;
                initialCapsuleRadius = auxCapsuleCollider.radius;
                initialCapsuleHeight = auxCapsuleCollider.height;
                auxCapsuleCollider.radius = 0;
                auxCapsuleCollider.height = 0;
            }
            else if (colliderType == typeof(SphereCollider))
            {
                auxSphereCollider = (SphereCollider)presenceCollider;
                initialSphereRadius = auxSphereCollider.radius;
                auxCapsuleCollider.radius = 0;
            }
            else if (colliderType == typeof(MeshCollider))
            {
                auxMeshColliderTransform = presenceCollider.transform;
                initialMeshColliderSize = presenceCollider.transform.localScale;
                presenceCollider.transform.localScale = Vector3.zero;
            }
        }

        /// <summary>
        /// Devuelve el collider a su tamaño original.
        /// </summary>
        public void RestoreColliderSize()
        {
            if (auxBoxCollider != null)
            {
                auxBoxCollider.size = initialBoxColliderSize;
            }
            else if (auxCapsuleCollider != null)
            {
                auxCapsuleCollider.radius = initialCapsuleRadius;
                auxCapsuleCollider.height = initialCapsuleHeight;
            }
            else if (auxSphereCollider != null)
            {
                auxSphereCollider.radius = initialSphereRadius;
            }
            else if (auxMeshColliderTransform != null)
            {
                auxMeshColliderTransform.localScale = initialMeshColliderSize;
            }
            CleanVariables();
        }

        /// <summary>
        /// Limpia los valores de las variables para que 
        /// el collider disabler pueda volverse a usar.
        /// </summary>
        private void CleanVariables()
        {
            auxBoxCollider = null;
            auxCapsuleCollider = null;
            auxSphereCollider = null;
            auxMeshColliderTransform = null;
            initialBoxColliderSize = Vector3.zero;
            initialCapsuleRadius = 0;
            initialCapsuleHeight = 0;
            initialSphereRadius = 0;
            initialMeshColliderSize = Vector3.zero;
        }
    }
}
