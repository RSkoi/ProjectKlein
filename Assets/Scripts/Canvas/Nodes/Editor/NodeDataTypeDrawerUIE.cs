using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

public class NodeDataTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var textField = new PropertyField(property.FindPropertyRelative("text"));
        var speedField = new PropertyField(property.FindPropertyRelative("speed"));
        var titleField = new PropertyField(property.FindPropertyRelative("title"));
        var nextNodeSceneNamesField = new PropertyField(property.FindPropertyRelative("nextNodeSceneNames"));

        // Add fields to the container.
        container.Add(textField);
        container.Add(speedField);
        container.Add(titleField);
        container.Add(nextNodeSceneNamesField);

        return container;
    }
}
