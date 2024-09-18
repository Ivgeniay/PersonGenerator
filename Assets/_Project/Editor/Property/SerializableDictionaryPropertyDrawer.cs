using AtomEngine.SystemFunc.Collections; 
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AtomEngine.Property
{
    //[CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    //public class SerializableDictionaryPropertyDrawer : PropertyDrawer
    //{
    //    private const float LineHeight = 18f;
    //    private const float Padding = 2f;

    //    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    //    {
    //        VisualElement container = new VisualElement();

    //        // Найти свойства ключей и значений
    //        SerializedProperty keysProperty = property.FindPropertyRelative("keys");
    //        SerializedProperty valuesProperty = property.FindPropertyRelative("values");

    //        if (keysProperty == null || valuesProperty == null)
    //        {
    //            container.Add(new Label("Error: Keys or Values property is missing."));
    //            return container;
    //        }

    //        if (keysProperty.arraySize != valuesProperty.arraySize)
    //        {
    //            container.Add(new Label("Error: Keys and Values array sizes do not match."));
    //            return container;
    //        }

    //        // Отобразить пары ключ-значение
    //        for (int i = 0; i < keysProperty.arraySize; i++)
    //        {
    //            VisualElement row = new VisualElement();
    //            row.style.flexDirection = FlexDirection.Row;
    //            row.style.marginBottom = 2;

    //            SerializedProperty keyProperty = keysProperty.GetArrayElementAtIndex(i);
    //            SerializedProperty valueProperty = valuesProperty.GetArrayElementAtIndex(i);

    //            if (keyProperty != null && valueProperty != null)
    //            {
    //                PropertyField keyField = new PropertyField(keyProperty);
    //                keyField.label = "Key";
    //                row.Add(keyField);

    //                PropertyField valueField = new PropertyField(valueProperty);
    //                valueField.label = "Value";
    //                row.Add(valueField);

    //                container.Add(row);
    //            }
    //        }

    //        return container;
    //    }


    //}
}
