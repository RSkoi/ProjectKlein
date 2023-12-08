using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ParticleSystemDataTypeCollection))]
public class ParticleSystemDataTypeCollectionDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var particleSystemsField = new PropertyField(property.FindPropertyRelative("particleSystems"));

        // Add fields to the container.
        container.Add(particleSystemsField);

        return container;
    }
}
