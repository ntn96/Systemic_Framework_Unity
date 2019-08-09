using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Output de emisión de partículas de olor, simula
    /// el olor que pueden desprender las cosas. El output emite con regularidad
    /// partículas que poseen estímulos y que pueden moverse por el escenario.
    /// </summary>
    public class OutputEmitParticle : OutputMultiple
    {
        /// <summary>
        /// Lista de tiempos de emisión, las partículas se emiten conforme a estos tiempos.
        /// El estímulo i se emite conforme al tiempo i.
        /// </summary>
        [SerializeField] private List<float> emission = new List<float>();
        /// <summary>
        /// Radio en torno al que se generan aleatoriamente las partículas con respecto
        /// al centro de la entidad. 
        /// A mayor radio más alejadas de la entidad pueden generarse las partículas.
        /// </summary>
        [SerializeField] private float emissionRadius = 1f;
        /// <summary>
        /// Prefab del gameObject de la partícula que se emite
        /// </summary>
        [SerializeField] private GameObject particlePrefab;
        /// <summary>
        /// Booleano auxiliar que sirve para la activación y desactivación
        /// de corrutinas cuando se habilita e inhabilita el output.
        /// </summary>
        private bool lastActivatedValue;

        /// <summary>
        /// Método para lanzar las corrutinas de emisión de partículas cuando el
        /// output se inicia. Muestra un mensaje de error cuando el número de 
        /// estímulos y tiempos de emisión no coinciden.
        /// </summary>
        private void Start()
        {
            lastActivatedValue = activated;
            if (activated)
            {
                if (stimuli.Count != emission.Count)
                {
                    Debug.LogError("OutputEmitParticle: Error, el número de estímulos no coincide con el número de tiempos de emisión");
                    return;
                }
                StartAllParticlesEmitters();
            }
        }

        /// <summary>
        /// Método que se encarga de detectar cuando se reactiva el output para volver a 
        /// lanzar las corrutinas.
        /// </summary>
        protected void Update()
        {
            if (!lastActivatedValue && activated)
            {
                lastActivatedValue = activated;
                StartAllParticlesEmitters();
            }
            else if (lastActivatedValue && !activated)
            {
                lastActivatedValue = activated;
            }
        }

        /// <summary>
        /// Lanza todas las corrutinas de emisión de las
        /// distintas partículas y estímulos.
        /// </summary>
        private void StartAllParticlesEmitters()
        {
            for (int i = 0; i < stimuli.Count; i++)
            {
                StartCoroutine(Emission(stimuli[i], emission[i]));
            }
        }

        /// <summary>
        /// Cuando se activa esta corrutina, si no se ha cumplido el máximo
        /// de activaciones y se ha cumplido el tiempo de emision especificado,
        /// se instancia una nueva partícula con el estímulo y una posición
        /// aleatoria en torno al radio de emission.
        /// </summary>
        /// <param name="stimulus">Estímulo de la partícula a instanciar</param>
        /// <param name="time">Tiempo entre emisiones</param>
        /// <returns></returns>
        private IEnumerator Emission(string stimulus, float time)
        {
            while (activated)
            {
                if (!infiniteActivations && actualNumActivations >= maxNumOfActivations) break;
                GameObject actualParticle = GameObject.Instantiate(particlePrefab);
                Particle particleScript = actualParticle.GetComponent<Particle>();
                particleScript.InitParticle(Entity, stimulus);
                actualParticle.transform.parent = SmellSystem.Instance.transform;
                actualParticle.transform.position = CalculateRandomPosition();
                actualNumActivations++;
                yield return new WaitForSeconds(time);
            }
        }

        /// <summary>
        /// Calcula una posición aleatoria en torno al centro del gameObject y
        /// el radio máximo en todas direcciones.
        /// </summary>
        /// <returns>La posición aleatoria alrededor del centro</returns>
        private Vector3 CalculateRandomPosition()
        {
            Vector3 position = (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;
            position = position * Random.Range(0, emissionRadius) + transform.position;
            return position;
        }

        /// <summary>
        /// Cuando se desactiva el output se paran todas las corrutinas
        /// </summary>
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Método génirico que muestra el nombre del componente sistémico en cuestión
        /// </summary>
        /// <returns>Nombre del componente sistémico</returns>
        public override string ToString()
        {
            return "Output Emit Particle";
        }
    }
}
