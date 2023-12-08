using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(NodeTextType))]
public class NodeTextTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var textField = new PropertyField(property.FindPropertyRelative("text"));
        var requiresFlagField = new PropertyField(property.FindPropertyRelative("requiresFlag"));
        var flagIdField = new PropertyField(property.FindPropertyRelative("flagId"));
        var flagValueField = new PropertyField(property.FindPropertyRelative("flagValue"));
        var viewCharAnimField = new PropertyField(property.FindPropertyRelative("viewCharAnim"));

        // Add fields to the container.
        container.Add(textField);
        container.Add(requiresFlagField);
        container.Add(flagIdField);
        container.Add(flagValueField);
        container.Add(viewCharAnimField);

        return container;
    }
}
