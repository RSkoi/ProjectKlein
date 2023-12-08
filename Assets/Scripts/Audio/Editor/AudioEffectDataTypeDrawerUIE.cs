using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(AudioEffectDataType))]
public class AudioEffectDataTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var clipField = new PropertyField(property.FindPropertyRelative("clip"));
        var priorityField = new PropertyField(property.FindPropertyRelative("priority"));
        var volumeField = new PropertyField(property.FindPropertyRelative("volume"));
        var pitchField = new PropertyField(property.FindPropertyRelative("pitch"));
        var stereoPanField = new PropertyField(property.FindPropertyRelative("stereoPan"));
        var loopField = new PropertyField(property.FindPropertyRelative("loop"));
        var forceNewInitField = new PropertyField(property.FindPropertyRelative("forceNewInit"));

        // Add fields to the container.
        container.Add(clipField);
        container.Add(priorityField);
        container.Add(volumeField);
        container.Add(pitchField);
        container.Add(stereoPanField);
        container.Add(loopField);
        container.Add(forceNewInitField);

        return container;
    }
}
