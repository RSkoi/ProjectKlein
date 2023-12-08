using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(BgSongDataType))]
public class BgSongDataTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();
        var priorityField = new PropertyField(property.FindPropertyRelative("priority"));
        var volumeField = new PropertyField(property.FindPropertyRelative("volume"));
        var pitchField = new PropertyField(property.FindPropertyRelative("pitch"));
        var stereoPanField = new PropertyField(property.FindPropertyRelative("stereoPan"));
        var loopField = new PropertyField(property.FindPropertyRelative("loop"));

        // Create property fields.
        var clipField = new PropertyField(property.FindPropertyRelative("clip"));

        // Add fields to the container.
        container.Add(clipField);
        container.Add(priorityField);
        container.Add(volumeField);
        container.Add(pitchField);
        container.Add(stereoPanField);
        container.Add(loopField);

        return container;
    }
}
