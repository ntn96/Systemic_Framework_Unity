using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Sistema de olores, es singleton y tiene una referencia
    /// a todas las partículas sistémicas instanciadas en el juego.
    /// </summary>
    public class SmellSystem : System
    {
        /// <summary>
        /// Instancia única del smell system para convertirlo en un singleton
        /// </summary>
        private static SmellSystem instance;
        /// <summary>
        /// Instancia única del smell system para convertirlo en un singleton
        /// </summary>
        public static SmellSystem Instance { get { return instance; } }
        /// <summary>
        /// Una lista de todas las partículas sistémicas instanciadas en el juego en el momento
        /// </summary>
        [SerializeField] private List<Particle> particles = new List<Particle>();

        /// <summary>
        /// Añade una nueva partícula a lista de partículas del sistema.
        /// </summary>
        /// <param name="particle">La nueva partícula a añadir</param>
        public void AddParticle(Particle particle)
        {
            particles.Add(particle);
        }

        /// <summary>
        /// Elimina una partícula de la lista de referencias de partículas
        /// </summary>
        /// <param name="particle">Partícula a borrar</param>
        public void EraseParticle(Particle particle)
        {
            particles.Remove(particle);
        }

        /// <summary>
        /// Al iniciar el sistema comprueba si se ha asignado algún smell system,
        /// si no es así se asigna como única instancia, si ya hay alguna instancia
        /// y no es esta misma, este objeto se destruye.
        /// </summary>
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }

    }
}
