using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// 1. Creamos la etiqueta que le pondremos a nuestra lista
public class SubclassSelectorAttribute : PropertyAttribute { }

// 2. Le decimos a Unity cómo debe dibujar esta etiqueta en el Inspector
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Dibujamos el campo normal (esto pinta la flechita para desplegar)
        EditorGUI.PropertyField(position, property, label, true);

        // Calculamos un rectángulo para poner nuestro botón tapando el texto "Element 0"
        Rect buttonRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth - 15, EditorGUIUtility.singleLineHeight);

        // Comprobamos qué tipo de efecto está seleccionado actualmente
        string typeName = property.managedReferenceValue != null ? property.managedReferenceValue.GetType().Name : "Select an effect";

        // Dibujamos el botón desplegable
        if (GUI.Button(buttonRect, typeName, EditorStyles.popup))
        {
            GenericMenu menu = new GenericMenu();

            // Opción para dejarlo vacío
            menu.AddItem(new GUIContent("None (Delete)"), false, () =>
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });

            // Buscamos automáticamente todas las clases que hereden de CardEffect
            Type baseType = GetBaseType(fieldInfo.FieldType);
            var types = TypeCache.GetTypesDerivedFrom(baseType).Where(t => !t.IsAbstract && !t.IsInterface);

            // Añadimos cada clase al menú
            foreach (Type type in types)
            {
                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            menu.ShowAsContext();
        }

        EditorGUI.EndProperty();
    }

    // Función auxiliar para detectar si es una Lista, Array o un tipo normal
    private Type GetBaseType(Type fieldType)
    {
        if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
            return fieldType.GetGenericArguments()[0];
        if (fieldType.IsArray)
            return fieldType.GetElementType();
        return fieldType;
    }
}
#endif