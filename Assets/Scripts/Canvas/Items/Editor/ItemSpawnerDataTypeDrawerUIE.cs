using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ItemSpawnerDataType))]
public class ItemSpawnerDataTypeDrawerUIE : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        var container = new VisualElement();

        // Create property fields.
        var itemField = new PropertyField(property.FindPropertyRelative("item"));
        var quantityRangeMaxField = new PropertyField(property.FindPropertyRelative("quantityRangeMax"));
        var spawnerCapacityField = new PropertyField(property.FindPropertyRelative("spawnerCapacity"));

        // Add fields to the container.
        container.Add(itemField);
        container.Add(quantityRangeMaxField);
        container.Add(spawnerCapacityField);

        return container;
    }
}
