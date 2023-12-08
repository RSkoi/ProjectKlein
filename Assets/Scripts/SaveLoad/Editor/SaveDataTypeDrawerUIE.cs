using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SaveFileDataType))]
public class SaveFileDataTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var timestampField = new PropertyField(property.FindPropertyRelative("timestamp"));
        var sceneNameField = new PropertyField(property.FindPropertyRelative("sceneName"));
        var dialogueStateField = new PropertyField(property.FindPropertyRelative("dialogueState"));
        var backgroundStateField = new PropertyField(property.FindPropertyRelative("backgroundState"));
        var entityStateField = new PropertyField(property.FindPropertyRelative("entityState"));
        var bgSongsStateField = new PropertyField(property.FindPropertyRelative("bgSongsState"));
        var audioEffectsStateField = new PropertyField(property.FindPropertyRelative("audioEffectsState"));
        var particleSystemsStateField = new PropertyField(property.FindPropertyRelative("particleSystemsState"));
        var itemsDataField = new PropertyField(property.FindPropertyRelative("itemsData"));
        var journalDataField = new PropertyField(property.FindPropertyRelative("journalData"));
        var flagDataField = new PropertyField(property.FindPropertyRelative("flagData"));
        var dnCycleDataField = new PropertyField(property.FindPropertyRelative("dnCycleData"));

        // Add fields to the container.
        container.Add(timestampField);
        container.Add(sceneNameField);
        container.Add(dialogueStateField);
        container.Add(backgroundStateField);
        container.Add(entityStateField);
        container.Add(bgSongsStateField);
        container.Add(audioEffectsStateField);
        container.Add(particleSystemsStateField);
        container.Add(itemsDataField);
        container.Add(journalDataField);
        container.Add(flagDataField);
        container.Add(dnCycleDataField);

        return container;
    }
}
