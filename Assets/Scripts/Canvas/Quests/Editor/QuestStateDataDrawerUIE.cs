using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

public class QuestStateDataDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var questField = new PropertyField(property.FindPropertyRelative("quest"));
        var stateField = new PropertyField(property.FindPropertyRelative("state"));

        // Add fields to the container.
        container.Add(questField);
        container.Add(stateField);

        return container;
    }
}
