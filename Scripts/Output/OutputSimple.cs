using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemicDesign
{
    /// <summary>
    /// Subtipo de output que puede emitir un único tipo de estímulo
    /// </summary>
    public abstract class OutputSimple : Output
    {
        /// <summary>
        /// El único tipo de estímulo que puede enviar el output
        /// </summary>
        [SerializeField] protected string stimulus;

        /// <summary>
        /// El único tipo de estímulo que puede enviar el output
        /// </summary>
        public string Stimulus
        {
            get { return stimulus; }
            set { stimulus = value; }
        }
    }
}
