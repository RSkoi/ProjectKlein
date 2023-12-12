using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;

[CustomPropertyDrawer(typeof(NextNodeListType))]
public class NextNodeListTypeDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var sceneNameField = new PropertyField(property.FindPropertyRelative("sceneName"));

        var sceneTravelConditionsProperty = property.FindPropertyRelative("sceneTravelConditions");
        var sceneTravelConditionsField = new PropertyField(sceneTravelConditionsProperty);

        // Add fields to the container.
        container.Add(sceneNameField);

        container.Add(sceneTravelConditionsField);
        DropdownField dropdownAddCondition = new(Enum.GetNames(typeof(ScriptedCondition.ScriptedConditionEnum)).ToList(), 0);
        container.Add(dropdownAddCondition);
        Button buttonAddCondition = new()
        {
            name = "buttonAddCondition",
            text = "Add condition"
        };
        buttonAddCondition.clicked += () =>
        {
            sceneTravelConditionsProperty.InsertArrayElementAtIndex(0);
            sceneTravelConditionsProperty.GetArrayElementAtIndex(0).managedReferenceValue =
                Activator.CreateInstance(ScriptedCondition.conditionTypes[Enum.Parse<ScriptedCondition.ScriptedConditionEnum>(dropdownAddCondition.value)]);
            property.serializedObject.ApplyModifiedProperties();
        };
        container.Add(buttonAddCondition);

        return container;
    }
}
