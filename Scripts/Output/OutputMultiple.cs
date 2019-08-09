using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Subtipo de output que puede emitir más un tipo de estímulo
    /// </summary>
    public abstract class OutputMultiple : Output
    {
        /// <summary>
        /// Lista de estímulos que puede emitir el output
        /// </summary>
        [SerializeField] protected List<string> stimuli = new List<string>();
    }
}
