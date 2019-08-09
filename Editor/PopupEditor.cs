using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SystemicDesign
{
    /// <summary>
    /// Clase del systemic editor utilizada para crear ventanas que hagan de popup
    /// Muestra una ventana con un mensaje y pueden hacerse de una o dos opciones para elegir.
    /// Se puede elegir el texto del mensaje y de los botones.
    /// Se le pasa al popup functiones que se ejecutará cuando se seleccione uno de los botones.
    /// Si se deja las funciones en nulo no se ejecutará nada.
    /// </summary>
    class PopupEditor : EditorWindow
    {
        private enum PopupType { TwoOptions, OneOption }                        ///Determina el tipo de popup que se mostrará

        private static EditorWindow window;                                     ///Variable en la que se guarda la ventana para mostrarla
        private static string message;                                          ///Mensaje que se muestra en el popup para informar
        private static SystemicEditor.ResponsePopup functionToApplyYes;         ///Función que se aplicará si se presiona el botón de sí
        private static SystemicEditor.ResponsePopup functionToApplyNo;          ///Función que se aplicará si se presiona el botón de no
        private static PopupType type;                                          ///Tipo de popup, se determina por el constructor empleado
        private static string defaultYesMessage = "Yes";                        ///Mensaje por defecto escrito en el botón de sí, se modifica si se usa ciertos constructores
        private static string defaultNoMessage = "No";                          ///Mensaje por defecto escrito en el botón de no, se modifica si se usa ciertos constructores

        /// <summary>
        /// Crea una ventana de popup con dos botones a elegir por el usuario
        /// </summary>
        /// <param name="showMessage">Mensaje que se le mostrará al usuario para informarle</param>
        /// <param name="yesMessage">Mensaje que pondrá en el botón de sí</param>
        /// <param name="noMessage">Mensaje que pondrá en el botón de no</param>
        /// <param name="functionYes">Función que se ejecutará cuando se presione el botón de sí</param>
        /// <param name="functionNo">función que se ejecutará cuando se presione el botón de no</param>
        public static void ShowWindow(string showMessage, string yesMessage, string noMessage, SystemicEditor.ResponsePopup functionYes, SystemicEditor.ResponsePopup functionNo)
        {
            defaultYesMessage = yesMessage;
            defaultNoMessage = noMessage;
            ShowWindow(showMessage, functionYes, functionNo);
        }

        /// <summary>
        /// Crea una ventana de popup con dos botones a elegir por el usuario
        /// </summary>
        /// <param name="showMessage">Mensaje que se le mostrará al usuario para informarle</param>
        /// <param name="yesMessage">Mensaje que pondrá en el botón de sí</param>
        /// <param name="functionYes">Función que se ejecutará cuando se presione el botón de sí</param>
        public static void ShowWindow(string showMessage, string yesMessage, SystemicEditor.ResponsePopup functionYes)
        {
            defaultYesMessage = yesMessage;
            ShowWindow(showMessage, functionYes);
        }

        /// <summary>
        /// Crea una ventana de popup con dos botones a elegir por el usario con texto para botones por defecto
        /// </summary>
        /// <param name="showMessage">Mensaje que se le mostrará al usuario para informarle</param>
        /// <param name="functionYes">Función que se ejecutará cuando se presione el botón de sí</param>
        /// <param name="functionNo">Función que se ejecutará cuando se presione el botón de no</param>
        public static void ShowWindow(string showMessage, SystemicEditor.ResponsePopup functionYes, SystemicEditor.ResponsePopup functionNo)
        {
            type = PopupType.TwoOptions;
            functionToApplyYes = functionYes;
            functionToApplyNo = functionNo;
            message = showMessage;
            window = GetWindow<PopupEditor>();
            window.ShowPopup();
        }

        /// <summary>
        /// Crea una ventana de popup con dos botones a elegir por el usario con texto para botones por defecto
        /// </summary>
        /// <param name="showMessage">Mensaje que se le mostrará al usuario para informarle</param>
        /// <param name="functionYes">Función que se ejecutará cuando se presione el botón de sí</param>
        public static void ShowWindow(string showMessage, SystemicEditor.ResponsePopup functionYes)
        {
            type = PopupType.OneOption;
            functionToApplyYes = functionYes;
            message = showMessage;
            window = GetWindow<PopupEditor>();
            window.ShowPopup();
        }

        /// <summary>
        /// Muestra la disposición del popup dependiendo si ha sido o no creada para ser un popup de 
        /// una o de dos opciones a elegir
        /// </summary>
        public void OnGUI()
        {
            switch (type)
            {
                case PopupType.TwoOptions: ShowTwoOptions(); break;
                case PopupType.OneOption: ShowOneOption(); break;
            }
        }

        /// <summary>
        /// Muestra un popup de dos botones a elegir
        /// </summary>
        private void ShowTwoOptions()
        {
            GUILayout.TextArea(message, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(defaultYesMessage))
            {
                if (functionToApplyYes != null) functionToApplyYes();
                this.Close();
            }
            else if (GUILayout.Button(defaultNoMessage))
            {
                if (functionToApplyNo != null) functionToApplyNo();
                this.Close();
            }
        }

        /// <summary>
        /// Muestra un popup con un único botón
        /// </summary>
        private void ShowOneOption()
        {
            GUILayout.TextArea(message, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button(defaultYesMessage))
            {
                if (functionToApplyYes != null) functionToApplyYes();
                this.Close();
            }
        }
    }
}