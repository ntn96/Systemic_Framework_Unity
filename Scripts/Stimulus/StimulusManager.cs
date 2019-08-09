using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Clase destinada a controlar y gestionar el total de todos los estímulos presentes
    /// en la escena de juego. Cada estímulo es simplemente un string que lo identifica
    /// y representa la información que porta. Es este string el que se interpreta.
    /// </summary>
    public class StimulusManager : MonoBehaviour
    {
        /// <summary>
        /// La lista de los estímulos creados y disponibles.
        /// </summary>
        [SerializeField] private List<string> values = new List<string>();

        /// <summary>
        /// Método para recuperar el estímulo localizado en un índice dado
        /// de la lista total de estímulos
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string Value(int index)
        {
            return values[index];
        }

        /// <summary>
        /// La lista de los estímulos creados y disponibles.
        /// </summary>
        public List<string> Values
        {
            /// Véase que no se deja que se modifique directamente la
            /// lista sino que se proporciona una copia para consulta
            get
            {
                return new List<string>(values);
            }
        }

        /// <summary>
        /// Añade un estímulo a la lista de estímulos pasándolo por parámetro
        /// Véase que no se puede repetir un estímulo
        /// </summary>
        /// <param name="stimulus">El estímulo nuevo a añadir</param>
        public void AddStimulus(string stimulus)
        {
            if (!values.Contains(stimulus))
                values.Add(stimulus);
        }

        /// <summary>
        /// Comprueba si el estímulo pasado por parámetro
        /// es un estímulo válido comprobando si está entre los creados
        /// del stimulus manager.
        /// </summary>
        /// <param name="stimulus">Estímulo a comprobar si es o no correcto</param>
        /// <returns></returns>
        public bool ValidStimulus(string stimulus)
        {
            return values.Contains(stimulus);
        }
    }
}
