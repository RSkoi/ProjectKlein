using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(AudioEffectDataTypeCollection))]
public class AudioEffectDataTypeCollectionDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var effectsField = new PropertyField(property.FindPropertyRelative("effects"));
        var stopAllLoopingEffectsField = new PropertyField(property.FindPropertyRelative("stopAllLoopingEffects"));

        // Add fields to the container.
        container.Add(effectsField);
        container.Add(stopAllLoopingEffectsField);

        return container;
    }
}
