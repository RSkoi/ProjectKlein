using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

public class JournalCrossSceneDataTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var questStatesField = new PropertyField(property.FindPropertyRelative("questStates"));

        // Add fields to the container.
        container.Add(questStatesField);

        return container;
    }
}
