using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ParticleSystemDataType))]
public class ParticleSystemDataTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var nameField = new PropertyField(property.FindPropertyRelative("name"));
        var prefabField = new PropertyField(property.FindPropertyRelative("prefab"));
        var destroyFlagField = new PropertyField(property.FindPropertyRelative("destroyFlag"));

        // Add fields to the container.
        container.Add(nameField);
        container.Add(prefabField);
        container.Add(destroyFlagField);

        return container;
    }
}
