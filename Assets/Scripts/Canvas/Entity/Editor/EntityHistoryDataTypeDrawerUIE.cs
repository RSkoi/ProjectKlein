using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

public class EntityHistoryDataTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var dataField = new PropertyField(property.FindPropertyRelative("data"));

        // Add fields to the container.
        container.Add(dataField);

        return container;
    }
}
