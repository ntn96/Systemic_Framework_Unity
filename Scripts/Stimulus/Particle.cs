using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Clase que representan una partícula de "olor" del diseño sistémico.
    /// Será un gameObject invisible que portará un estímulo que representa su olor.
    /// Tiene un tamaño, un autor que lo origino, una dirección de movimiento y un tiempo de vida.
    /// Una vez expire el tiempo de vida "se diluira", destruyéndose el objeto.
    /// </summary>
    public class Particle : MonoBehaviour
    {
        /// <summary>
        /// Collider que le da presencia a la partícula de olor y que la hace detectable.
        /// </summary>
        [SerializeField] private SphereCollider particleCollider;
        /// <summary>
        /// Tamaño del collider que le da presencia a la partícula de olor.
        /// </summary>
        [SerializeField] private float colliderRadius = 0.5f;
        /// <summary>
        /// Estímulo que porta la información del "olor" que lleva la partída, será el estímulo
        /// que analizará un input smell cuando capture la partícula.
        /// </summary>
        [SerializeField] private string stimulus = "";
        /// <summary>
        /// Tiempo en segundos que la partícula permanecerá con vida una vez creada, tras este tiempo
        /// el gameObject asociado será destruido
        /// </summary>
        [SerializeField] private float timeAlive = 5f;
        /// <summary>
        /// Dirección de movimiento que lleva la partícula, la velocidad con la que se mueve 
        /// la partícula va implícita en el módulo del vector
        /// </summary>
        [SerializeField] private Vector3 direction;
        /// <summary>
        /// La entidad que creo el objeto, se usa para que una entidad no sea estimulada
        /// por su propio olor.
        /// </summary>
        [SerializeField] private Entity author;
        /// <summary>
        /// Determina si se muestra o no el gizmo que muestra la partícula, si no se muestra
        /// será completamente invisible
        /// </summary>
        [SerializeField] private bool showGizmos = true;
        /// <summary>
        /// Booleano usado para que solo se pueda asignar valores a la particula una vez,
        /// evitando así el falseado de la información que porta.
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// La entidad que creo el objeto, se usa para que una entidad no sea estimulada
        /// por su propio olor.
        /// </summary>
        public Entity Author
        {
            /// Solo se permite saber cual es el autor de la partícula y no
            /// se permite cambiarlo, así se evita que se falsee.
            get
            {
                return author;
            }
        }

        /// <summary>
        /// Dirección de movimiento que lleva la partícula, la velocidad con la que se mueve 
        /// la partícula va implícita en el módulo del vector
        /// </summary>
        public Vector3 Direction
        {
            /// Se permite que se mire la dirección para saber de donde venía el olor y así rastrear
            get
            {
                return direction;
            }
            /// Se permite modificar la dirección de la partícula, por ejeplo si se le ejerce la fuerza del viento
            set
            {
                direction = value;
            }
        }

        /// <summary>
        /// Inicializa los valores de la partícula. Una vez introducidos
        /// no se permite que se modifiquen. De este modo se evita que se falseen los
        /// valores
        /// </summary>
        /// <param name="author">Entidad generadora de la partícula</param>
        /// <param name="stimulus">Estímulo que porta la partícula, representa el olor que desprende</param>
        public void InitParticle(Entity author, string stimulus)
        {
            if (!initialized)
            {
                this.author = author;
                this.stimulus = stimulus;
                initialized = true;
            }
        }

        /// <summary>
        /// Método ejecutado por las entidades receptoras del olor, extraen el olor
        /// que tiene la partícula en forma de estímulo y la partícula es destruida.
        /// </summary>
        /// <returns>El estímulo que porta la partícula</returns>
        public string ConsumeParticle()
        {
            Destroy(gameObject);
            return stimulus;
        }

        /// <summary>
        /// Usada para inicializar la partícula una vez creada, añadiendo su
        /// referencia al smell system. Adapatando el radio de su collider a los valores
        /// de referencia. Activa su tiempo de vida hasta que muera.
        /// </summary>
        void Start()
        {
            SmellSystem.Instance.AddParticle(this);
            particleCollider.radius = colliderRadius;
            particleCollider.isTrigger = true;
            Destroy(gameObject, timeAlive);
        }

        /// <summary>
        /// Actualiza la posición de la partícula según la dirección que tenga en
        /// cada momento.
        /// </summary>
        void Update()
        {
            transform.position += direction * Time.deltaTime;
        }

        /// <summary>
        /// Una vez es destruida la partícula se borra su referencia del smell system
        /// </summary>
        private void OnDestroy()
        {
            SmellSystem.Instance.EraseParticle(this);
        }

        /// <summary>
        /// Función destinada a dibujar el collider invisible de la partícula
        /// si la opción de gizmos está activada.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (showGizmos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, colliderRadius);
            }
        }
    }
}
