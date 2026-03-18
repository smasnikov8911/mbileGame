#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class DropdownAttribute : PropertyAttribute
{
    public string[] options;

    public DropdownAttribute(string optionsFieldName)
    {
        this.optionsFieldName = optionsFieldName;
    }

    public string optionsFieldName;
}

[CustomPropertyDrawer(typeof(DropdownAttribute))]
public class DropdownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = attribute as DropdownAttribute;
        var options = GetOptions(property, attr.optionsFieldName);

        if (options == null || options.Length == 0)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        int selectedIndex = Mathf.Max(0, System.Array.IndexOf(options, property.stringValue));
        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, options);

        if (selectedIndex >= 0 && selectedIndex < options.Length)
        {
            property.stringValue = options[selectedIndex];
        }
    }

    private string[] GetOptions(SerializedProperty property, string fieldName)
    {
        var targetObject = property.serializedObject.targetObject;
        var fieldInfo = targetObject.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public);

        return fieldInfo?.GetValue(targetObject) as string[];
    }
}
#endif