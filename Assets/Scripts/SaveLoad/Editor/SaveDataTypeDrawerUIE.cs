using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

public class SaveFileDataTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var sceneNameField = new PropertyField(property.FindPropertyRelative("sceneName"));
        var dialogueStateField = new PropertyField(property.FindPropertyRelative("dialogueState"));
        var backgroundStateField = new PropertyField(property.FindPropertyRelative("backgroundState"));
        var entityStateField = new PropertyField(property.FindPropertyRelative("entityState"));
        var bgSongsStateField = new PropertyField(property.FindPropertyRelative("bgSongsState"));
        var audioEffectsStateField = new PropertyField(property.FindPropertyRelative("audioEffectsState"));
        var itemsDataField = new PropertyField(property.FindPropertyRelative("itemsData"));
        var journalDataField = new PropertyField(property.FindPropertyRelative("journalData"));

        // Add fields to the container.
        container.Add(sceneNameField);
        container.Add(dialogueStateField);
        container.Add(backgroundStateField);
        container.Add(entityStateField);
        container.Add(bgSongsStateField);
        container.Add(audioEffectsStateField);
        container.Add(itemsDataField);
        container.Add(journalDataField);

        return container;
    }
}
