using AtomEngine.Testing.Inspector;
using System.Collections.Generic;
using UnityEditor.UIElements; 
using UnityEngine.UIElements;
using System.Diagnostics;
using System.Reflection; 
using UnityEditor;
using UnityEngine;
using System;

namespace AtomEngine.Testing
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    internal class TestedEditor : Editor
    {
        protected const string BASE_INSPECTOR_GUI = "BASE";
        protected const string TEST_INSPECTOR_GUI = "TEST";

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            root.name = BASE_INSPECTOR_GUI;

            if (target is not ITestableInInspector testableTarget)
            {
                root.Add(base.CreateInspectorGUI());
                return root;
            }


            VisualElement testWrap = new VisualElement();
            testWrap.name = TEST_INSPECTOR_GUI; 

            if (target is IPublicMethodsInspector publicMethodsTarget)
            {
                VisualElement pubMethodsVisualElement = Public_Methods_Section();
                if (pubMethodsVisualElement.childCount > 1) testWrap.Add(pubMethodsVisualElement);          //Label + Buttons
            }

            if (target is IPrivateMethodInspector privateMethodsTarget)
            {
                VisualElement privateMethodsVisualElement = Private_Method_Section();
                if (privateMethodsVisualElement.childCount > 1) testWrap.Add(privateMethodsVisualElement);  //Label + Buttons
            }

            if(target is IFieldsInspector publicFieldsTarget)
            {
                VisualElement publicFieldsVisualElement = Fields_Section();
                if (publicFieldsVisualElement.childCount > 1) testWrap.Add(publicFieldsVisualElement);      //Label + Fields
            }

            if (testWrap.childCount > 0)
            { 
                testWrap.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
                testWrap.style.marginTop = 10;
                testWrap.style.borderTopWidth = 5;
                testWrap.style.borderTopLeftRadius = 5;
                testWrap.style.borderTopRightRadius = 5;
                testWrap.style.borderBottomRightRadius = 5;
                testWrap.style.borderBottomLeftRadius = 5;
                Label label = new Label("Test Inspector Section");
                label.style.unityFontStyleAndWeight = FontStyle.Bold;
                label.style.fontSize = 22;
                testWrap.Insert(0, label);

                VisualElement baseWrap = new VisualElement();
                baseWrap.name = BASE_INSPECTOR_GUI;
                baseWrap.Add(base.CreateInspectorGUI());

                root.Add(baseWrap);
                root.Add(testWrap);
                return root;
            }
            else
            {
                return base.CreateInspectorGUI();
            }
        }

        private VisualElement Fields_Section()
        {
            List<VisualElement> content = new List<VisualElement>();
             
            Type componentType = target.GetType(); 
            foreach (FieldInfo field in componentType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            { 
                bool isSerializable = IsSerializableInUnity(field); 
                if (isSerializable)
                {
                    content.AddRange(CreateFieldForType(field, target));
                }
                else
                {
                    VisualElement fieldContainer = CreateNonSerializableField(field, target);
                    content.Add(fieldContainer);
                }
            }
              
            return Create_TestSection(content.ToArray());
        }

        private List<VisualElement> CreateFieldForType(FieldInfo field, object target)
        {
            List<VisualElement> content = new();
            object fieldValue = field.GetValue(target);

            if (field.FieldType == typeof(int))
            {
                // Для int создаем IntegerField
                IntegerField intField = new IntegerField(field.Name)
                {
                    value = (int)fieldValue
                };
                intField.RegisterValueChangedCallback(evt => field.SetValue(target, evt.newValue));
                content.Add(intField);
            }
            else if (field.FieldType == typeof(float))
            {
                // Для float создаем FloatField
                FloatField floatField = new FloatField(field.Name)
                {
                    value = (float)fieldValue
                };
                floatField.RegisterValueChangedCallback(evt => field.SetValue(target, evt.newValue));
                content.Add(floatField);
            }
            else if (field.FieldType == typeof(string))
            {
                // Для string создаем TextField
                TextField textField = new TextField(field.Name)
                {
                    value = (string)fieldValue
                };
                textField.RegisterValueChangedCallback(evt => field.SetValue(target, evt.newValue));
                content.Add(textField);
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType))
            {
                // Для объектов Unity создаем ObjectField
                ObjectField objectField = new ObjectField(field.Name)
                {
                    objectType = field.FieldType,
                    value = (UnityEngine.Object)fieldValue
                };
                objectField.RegisterValueChangedCallback(evt => field.SetValue(target, evt.newValue));
                content.Add(objectField);
            }
            else if (typeof(Color).IsAssignableFrom(field.FieldType))
            {
                ColorField colorField = new ColorField(field.Name)
                {
                    value = (Color)fieldValue
                };
                colorField.RegisterValueChangedCallback(e => field.SetValue(target, e.newValue));
                content.Add(colorField);
            }
            else if (typeof(Vector3).IsAssignableFrom(field.FieldType))
            {
                Vector3Field vector3Field = new Vector3Field(field.Name)
                {
                    value = (Vector3)fieldValue
                };
                vector3Field.RegisterValueChangedCallback(e => field.SetValue(target, e.newValue));
                content.Add(vector3Field);
            }
            else 
            {
                // Для всех остальных типов создаем TextField с ToString()
                TextField genericField = new TextField(field.Name)
                {
                    value = fieldValue?.ToString() ?? "null"
                };
                genericField.isReadOnly = true;
                content.Add(genericField);
            }

            return content;
        }

        // Создание VisualElement для несериализуемого поля
        private VisualElement CreateNonSerializableField(FieldInfo field, object target)
        {
            object fieldValue = field.GetValue(target);

            VisualElement fieldContainer = new VisualElement();
            fieldContainer.style.flexDirection = FlexDirection.Row;
            fieldContainer.style.alignItems = Align.Center;

            Label fieldLabel = new Label(field.Name);
            fieldContainer.Add(fieldLabel);

            TextField fieldValueField = new TextField
            {
                value = fieldValue?.ToString() ?? "null",
                isReadOnly = true
            };
            fieldContainer.Add(fieldValueField);

            return fieldContainer;
        }

        // Проверка на возможность сериализации Unity
        private bool IsSerializableInUnity(FieldInfo field)
        {
            bool isPublic = field.IsPublic;
            bool hasSerializeField = field.GetCustomAttribute<SerializeField>() != null;
            bool isUnitySerializableType = field.FieldType.IsPrimitive ||
                                           field.FieldType == typeof(string) ||
                                           typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType);

            //return (isPublic || hasSerializeField) && isUnitySerializableType;
            return (isPublic || hasSerializeField || isUnitySerializableType);
        }

        private VisualElement Private_Method_Section()
        {
            List<VisualElement> content = new List<VisualElement>();

            Type targetType = target.GetType();
            MethodInfo[] publicMethods = targetType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < publicMethods.Length; i++)
            {
                MethodInfo method = publicMethods[i];
                if (method.DeclaringType == typeof(MonoBehaviour) ||
                    method.DeclaringType == typeof(Behaviour) ||
                    method.DeclaringType == typeof(Component) ||
                    method.DeclaringType == typeof(UnityEngine.Object) ||
                    method.DeclaringType == typeof(object)
                    )
                {
                    continue;
                }
                if (method.GetParameters().Length > 0) continue;
                content.Add(new Button(() => { method.Invoke(target, null); }) { text = method.Name });
            }
            return Create_TestSection(content.ToArray());
        } 
        private VisualElement Public_Methods_Section()
        {
            List<VisualElement> content = new List<VisualElement>();

            Type targetType = target.GetType();
            MethodInfo[] publicMethods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance );
            for (int i = 0; i < publicMethods.Length; i++)
            {
                MethodInfo method = publicMethods[i];
                if (method.DeclaringType == typeof(MonoBehaviour) ||
                    method.DeclaringType == typeof(Behaviour) ||
                    method.DeclaringType == typeof(Component) ||
                    method.DeclaringType == typeof(UnityEngine.Object) ||
                    method.DeclaringType == typeof(object)
                    )
                {
                    continue;
                }
                if (method.GetParameters().Length > 0) continue;
                content.Add(new Button(() => { method.Invoke(target, null); }) { text = method.Name }); 
            }
            return Create_TestSection(content.ToArray());
        }

        private VisualElement Create_TestSection(params VisualElement[] content)
        {
            VisualElement root = new VisualElement();
            root.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.1f);
            root.style.marginTop = 10;

            StackTrace stackTrace = new StackTrace();
            string methodName = stackTrace.GetFrame(1).GetMethod().Name.ToString().Replace("_", " ");

            Label label = new Label(methodName);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginBottom = 10;
            root.Add(label);

            foreach (var item in content) root.Add(item);
             
            return root;
        } 
    }


}
