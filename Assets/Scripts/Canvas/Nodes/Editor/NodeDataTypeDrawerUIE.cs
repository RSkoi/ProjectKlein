using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;

[CustomPropertyDrawer(typeof(NodeDataType))]
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
        var nextNodeSceneListsField = new PropertyField(property.FindPropertyRelative("nextNodeSceneLists"));
        var nextNodeSceneNamesField = new PropertyField(property.FindPropertyRelative("nextNodeSceneNames"));
        var packageNameField = new PropertyField(property.FindPropertyRelative("packageName"));

        var travelConditionsProperty = property.FindPropertyRelative("travelConditions");
        var travelConditionsField = new PropertyField(travelConditionsProperty);

        var travelEffectsProperty = property.FindPropertyRelative("travelEffects");
        var travelEffectsField = new PropertyField(travelEffectsProperty);

        // Add fields to the container.
        container.Add(textField);
        container.Add(speedField);
        container.Add(titleField);
        container.Add(nextNodeSceneListsField);
        container.Add(nextNodeSceneNamesField);
        container.Add(packageNameField);
        container.Add(travelConditionsField);

        DropdownField dropdownAddCondition = new(Enum.GetNames(typeof(ScriptedCondition.ScriptedConditionEnum)).ToList(), 0);
        container.Add(dropdownAddCondition);
        Button buttonAddCondition = new()
        {
            name = "buttonAddCondition",
            text = "Add condition"
        };
        buttonAddCondition.clicked += () =>
        {
            travelConditionsProperty.InsertArrayElementAtIndex(0);
            travelConditionsProperty.GetArrayElementAtIndex(0).managedReferenceValue =
                Activator.CreateInstance(ScriptedCondition.conditionTypes[Enum.Parse<ScriptedCondition.ScriptedConditionEnum>(dropdownAddCondition.value)]);
            property.serializedObject.ApplyModifiedProperties();
        };
        container.Add(buttonAddCondition);

        container.Add(travelEffectsField);
        DropdownField dropdownAddEffect = new(Enum.GetNames(typeof(ScriptedEffect.ScriptedEffectEnum)).ToList(), 0);
        container.Add(dropdownAddEffect);
        Button buttonAddEffect = new()
        {
            name = "buttonAddEffect",
            text = "Add effect"
        };
        buttonAddEffect.clicked += () =>
        {
            travelEffectsProperty.InsertArrayElementAtIndex(0);
            travelEffectsProperty.GetArrayElementAtIndex(0).managedReferenceValue =
                Activator.CreateInstance(ScriptedEffect.scriptedEffectTypes[Enum.Parse<ScriptedEffect.ScriptedEffectEnum>(dropdownAddEffect.value)]);
            property.serializedObject.ApplyModifiedProperties();
        };
        container.Add(buttonAddEffect);

        return container;
    }
}
