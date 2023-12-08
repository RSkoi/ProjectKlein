using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(FlagDataType))]
public class FlagDataTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var idField = new PropertyField(property.FindPropertyRelative("id"));
        var valueField = new PropertyField(property.FindPropertyRelative("_value"));

        // Add fields to the container.
        container.Add(idField);
        container.Add(valueField);

        return container;
    }
}
