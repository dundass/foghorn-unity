using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IEffect), useForChildren: true)]
public class IEffectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // For serialization to work, we draw as a generic object field
        // but validate it's actually an IEffect implementation
        EditorGUI.BeginProperty(position, label, property);
        
        var obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(ScriptableObject), false);
        
        // Validate that the selected object implements IEffect
        if (obj != null && obj is ScriptableObject so)
        {
            if (!(obj is IEffect))
            {
                EditorUtility.DisplayDialog("Invalid Selection", $"{obj.name} does not implement IEffect", "OK");
                obj = null;
            }
        }
        
        property.objectReferenceValue = obj;
        EditorGUI.EndProperty();
    }
}
