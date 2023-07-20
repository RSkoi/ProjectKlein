using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(LocalisationDataType))]
public class LocalisationDataTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var textField = new PropertyField(property.FindPropertyRelative("text"));
        var nameField = new PropertyField(property.FindPropertyRelative("name"));
        var namePosField = new PropertyField(property.FindPropertyRelative("namePos"));
        var isNarratorField = new PropertyField(property.FindPropertyRelative("isNarrator"));
        var sizeIncreaseField = new PropertyField(property.FindPropertyRelative("sizeIncrease"));

        // Add fields to the container.
        container.Add(textField);
        container.Add(nameField);
        container.Add(namePosField);
        container.Add(isNarratorField);
        container.Add(sizeIncreaseField);

        return container;
    }
}
