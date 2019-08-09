using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SystemicDesign
{
    /// <summary>
    /// Editor customizado para la clase Root system para mostrar
    /// solo las propiedades que a mí me interesan
    /// </summary>
    [CustomEditor(typeof(RootSystem))]
    public class RootSystemEditor : Editor
    {
        // Los parametros serailizados que deben tenerse en cuenta a la hora de mostrar el editor customizado del root system
        SerializedProperty smellSystem;
        SerializedProperty entities;
        SerializedProperty allUnits;
        SerializedProperty allSystems;
        SerializedProperty inputDirect;
        SerializedProperty outputDirect;
        SerializedProperty outputsPresence;
        SerializedProperty outputsPresenceActivation;
        SerializedProperty inputsPeriodicActivation;
        SerializedProperty inputVision;
        SerializedProperty stimulusManager;
        SerializedProperty outputBroadcast;
        SerializedProperty inputsRandomActivation;
        SerializedProperty inputsSmell;
        SerializedProperty outputEmit;

        /// <summary>
        /// Obtiene los propiedades serializadas de la instancia de rootSystem
        /// </summary>
        void OnEnable()
        {
            inputDirect = serializedObject.FindProperty("inputDirect");
            outputDirect = serializedObject.FindProperty("outputDirect");
            outputBroadcast = serializedObject.FindProperty("outputBroadcast");
            outputsPresence = serializedObject.FindProperty("outputsPresence");
            outputsPresenceActivation = serializedObject.FindProperty("outputsPresenceActivation");
            inputsPeriodicActivation = serializedObject.FindProperty("inputsPeriodicActivation");
            inputsRandomActivation = serializedObject.FindProperty("inputsRandomActivation");
            inputVision = serializedObject.FindProperty("inputsVision");
            inputsSmell = serializedObject.FindProperty("inputsSmell");
            smellSystem = serializedObject.FindProperty("smellSystem");
            outputEmit = serializedObject.FindProperty("outputEmit");
            entities = serializedObject.FindProperty("entities");
            allUnits = serializedObject.FindProperty("allUnits");
            allSystems = serializedObject.FindProperty("allSystems");
            stimulusManager = serializedObject.FindProperty("stimulusManager");
        }

        /// <summary>
        /// Muestra todas las propiedades serializadas que sean relevantes a la
        /// hora de actualizar la GUI de este editor customizado
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(inputDirect);
            EditorGUILayout.PropertyField(outputDirect);
            EditorGUILayout.PropertyField(outputBroadcast);
            EditorGUILayout.PropertyField(outputsPresence, true);
            EditorGUILayout.PropertyField(outputsPresenceActivation, true);
            EditorGUILayout.PropertyField(inputsPeriodicActivation, true);
            EditorGUILayout.PropertyField(inputsRandomActivation, true);
            EditorGUILayout.PropertyField(inputVision, true);
            EditorGUILayout.PropertyField(inputsSmell, true);
            EditorGUILayout.PropertyField(outputEmit, true);
            EditorGUILayout.PropertyField(smellSystem);
            EditorGUILayout.PropertyField(entities, true);
            EditorGUILayout.PropertyField(allUnits, true);
            EditorGUILayout.PropertyField(allSystems, true);
            EditorGUILayout.PropertyField(stimulusManager);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
