using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(BackgroundDataType))]
public class BackgroundDataTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var textureField = new PropertyField(property.FindPropertyRelative("texture"));
        var fadeBlackTransitionField = new PropertyField(property.FindPropertyRelative("fadeBlackTransition"));
        var fadeAlphaTransitionField = new PropertyField(property.FindPropertyRelative("fadeAlphaTransition"));

        // Add fields to the container.
        container.Add(textureField);
        container.Add(fadeBlackTransitionField);
        container.Add(fadeAlphaTransitionField);

        return container;
    }
}
