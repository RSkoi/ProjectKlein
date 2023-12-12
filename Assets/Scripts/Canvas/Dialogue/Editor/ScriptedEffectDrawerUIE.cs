using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;

[CustomPropertyDrawer(typeof(ScriptedEffect))]
public class ScriptedEffectDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var textField = new PropertyField(property);
        var conditionsProperty = property.FindPropertyRelative("conditions");

        container.Add(textField);

        DropdownField dropdownAddCondition = new(Enum.GetNames(typeof(ScriptedCondition.ScriptedConditionEnum)).ToList(), 0);
        container.Add(dropdownAddCondition);
        Button buttonAddCondition = new()
        {
            name = "buttonAddCondition",
            text = "Add condition"
        };
        buttonAddCondition.clicked += () =>
        {
            conditionsProperty.InsertArrayElementAtIndex(0);
            conditionsProperty.GetArrayElementAtIndex(0).managedReferenceValue =
                Activator.CreateInstance(ScriptedCondition.conditionTypes[Enum.Parse<ScriptedCondition.ScriptedConditionEnum>(dropdownAddCondition.value)]);
            property.serializedObject.ApplyModifiedProperties();
        };
        container.Add(buttonAddCondition);

        return container;
    }
}
