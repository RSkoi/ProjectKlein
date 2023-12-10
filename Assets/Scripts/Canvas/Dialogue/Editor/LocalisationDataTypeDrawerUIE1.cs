using System;
using System.Linq;
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
        var hasChoicesField = new PropertyField(property.FindPropertyRelative("hasChoices"));
        var choicesField = new PropertyField(property.FindPropertyRelative("choices"));
        var hasChapterPopupField = new PropertyField(property.FindPropertyRelative("hasChapterPopup"));
        var chapterPopupField = new PropertyField(property.FindPropertyRelative("chapterPopup"));
        var isConditionalField = new PropertyField(property.FindPropertyRelative("isConditional"));
        var conditionalTextField = new PropertyField(property.FindPropertyRelative("conditionalText"));

        var scriptedEffectsProperty = property.FindPropertyRelative("scriptedEffects");
        var scriptedEffectsField = new PropertyField(scriptedEffectsProperty);

        // Add fields to the container.
        container.Add(textField);
        container.Add(nameField);
        container.Add(namePosField);
        container.Add(isNarratorField);
        container.Add(sizeIncreaseField);
        container.Add(scriptedEffectsField);

        DropdownField dropdownAddEffect = new (Enum.GetNames(typeof(ScriptedEffect.ScriptedEffectEnum)).ToList(), 0);
        container.Add(dropdownAddEffect);
        Button buttonAddEffect = new()
        {
            name = "buttonAddEffect",
            text = "Add effect"
        };
        buttonAddEffect.clicked += () =>
        {
            scriptedEffectsProperty.InsertArrayElementAtIndex(0);
            scriptedEffectsProperty.GetArrayElementAtIndex(0).managedReferenceValue =
                Activator.CreateInstance(ScriptedEffect.scriptedEffectTypes[Enum.Parse<ScriptedEffect.ScriptedEffectEnum>(dropdownAddEffect.value)]);
            property.serializedObject.ApplyModifiedProperties();
        };
        container.Add(buttonAddEffect);

        container.Add(hasChoicesField);
        // this is a bit fucky, choices property field will only be updated when the Editor view updates
        if (property.FindPropertyRelative("hasChoices").boolValue)
            container.Add(choicesField);

        container.Add(hasChapterPopupField);
        if (property.FindPropertyRelative("hasChapterPopup").boolValue)
            container.Add(chapterPopupField);

        container.Add(isConditionalField);
        if (property.FindPropertyRelative("isConditional").boolValue)
            container.Add(conditionalTextField);

        return container;
    }
}
