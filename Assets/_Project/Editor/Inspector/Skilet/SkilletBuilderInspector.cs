using AtomEngine.SystemFunc.Attributes;
using AtomEngine.SystemFunc.Extensions;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements; 
using AtomEngine.SystemFunc;
using AtomEngine.Skillets;
using AtomEngine.Testing;
using System.Reflection;
using UnityEditor;
using UnityEngine; 
using System;


namespace AtomEngine.Skilet
{
    [CustomEditor(typeof(SkilletBuilder))]
    internal class SkilletBuilderInspector : TestedEditor
    {
        private VisualTreeAsset transformField;
        private VisualTreeAsset chracterInterface;

        public const string TRANSFORM_FIELD_NAME = "TransformField";
        public const string CHARACTER_FIELD = "Character";
        public const string CONTAINER = "Container";
        public const string S_WEGHT = "Weght";
        public const string S_DISTANCE = "Distance"; 
        public const string TEXT_DESCRIPTION = "Used to create a default skeleton or auto-configuration of an empty object";

        public override VisualElement CreateInspectorGUI()
        {
            if (transformField == null) transformField = UniversalAssetFinder.LoadAsset<VisualTreeAsset>(TRANSFORM_FIELD_NAME, "uxml");
            if (chracterInterface == null) chracterInterface = UniversalAssetFinder.LoadAsset<VisualTreeAsset>(CHARACTER_FIELD, "uxml");

            VisualElement root = new VisualElement();

            SkilletBuilder builder = (SkilletBuilder)target;
            SerializedObject serializedObject = base.serializedObject;

            CharacterVisual characterVisual = new CharacterVisual(chracterInterface).Initialize();
            TransformTableVisual transformTableVisual = new TransformTableVisual(transformField, this).Initialize();

            characterVisual.OnBodyChapterClick += transformTableVisual.OnBodyChapterClick;
            root.RegisterCallback<MouseLeaveEvent>((e) =>
            {
                transformTableVisual.OnEmptySpaceClick();
            });

            VisualElement wrapContainers = new VisualElement();
            VisualElement wrapTitleLabel = new VisualElement();
            VisualElement wrapDescriptionLabel = new VisualElement();

            wrapTitleLabel.style.flexDirection = FlexDirection.Row;
            wrapTitleLabel.style.justifyContent = Justify.Center;
            wrapTitleLabel.style.marginBottom = 4;

            SetUp_ContainersStyle(wrapContainers, wrapTitleLabel, wrapDescriptionLabel);

            root.Add(wrapContainers);
            root.Add(characterVisual.Root);
            root.Add(transformTableVisual.Root);

            CreateAndAddHelpBtns(builder, root);

            return root;
        }

        private void SetUp_ContainersStyle(VisualElement wrapContainers, VisualElement wrapTitleLabel, VisualElement wrapDescriptionLabel)
        {
            Label titleLabel = new Label(nameof(SkilletBuilder));
            titleLabel.style.fontSize = 20;
            Label descriptionLabel = new Label(TEXT_DESCRIPTION);
            descriptionLabel.style.flexShrink = 0;
            descriptionLabel.style.flexGrow = 1;
            descriptionLabel.style.whiteSpace = WhiteSpace.Normal;
            descriptionLabel.style.width = new StyleLength(StyleKeyword.Auto);
            descriptionLabel.style.unityTextAlign = TextAnchor.UpperCenter;
            wrapDescriptionLabel.style.flexDirection = FlexDirection.Column;
            wrapDescriptionLabel.style.alignContent = Align.Center;
            wrapDescriptionLabel.style.alignItems = Align.Center;
            wrapDescriptionLabel.style.justifyContent = Justify.Center;

            wrapTitleLabel.Add(titleLabel);
            wrapDescriptionLabel.Add(descriptionLabel);
            wrapDescriptionLabel.style.marginBottom = 8;

            wrapContainers.Add(wrapTitleLabel);
            wrapContainers.Add(wrapDescriptionLabel);
            wrapContainers.style.flexDirection = FlexDirection.Column;
            wrapContainers.style.justifyContent = Justify.Center;
        }
        private void CreateAndAddHelpBtns(SkilletBuilder builder, VisualElement root)
        {
            Button autoConfigureBtn = new Button(() => { builder.AutoConfigure(); Repaint(); }) { text = "Auto Configure" };
            Button clearBtn = new Button(() => { builder.Clear(); Repaint(); }) { text = "Clear Bone Bindered Links" };
            Button createNewSkiletBtn = new Button(() => { builder.CreateNewSkilet(); Repaint(); }) { text = "Create Mixamo Men Skillet" };
            createNewSkiletBtn.style.marginTop = 8;

            root.Add(autoConfigureBtn);
            root.Add(clearBtn);
            root.Add(createNewSkiletBtn);
        } 
    }

    public class CharacterVisual
    {
        public event Action<BodyChapterType> OnBodyChapterClick;

        public const string HeadGreen = "HeadGreen";
        public const string BodyGreen = "BodyGreen";
        public const string RHandGreen = "RHandGreen";
        public const string LHandGreen = "LHandGreen";
        public const string RLegGreen = "RLegGreen";
        public const string LLegGreen = "LLegGreen";

        public readonly VisualElement Root;
        private VisualTreeAsset chracterInterface;

        public CharacterVisual(VisualTreeAsset chracterInterface)
        {
            Root = new VisualElement();
            this.chracterInterface = chracterInterface;
        }

        public CharacterVisual Initialize()
        {
            var charac = chracterInterface.CloneTree();

            var headG = charac.Q<VisualElement>(HeadGreen);
            headG.dataSource = HeadGreen;
            var bodyG = charac.Q<VisualElement>(BodyGreen);
            bodyG.dataSource = BodyGreen;
            var rHandG = charac.Q<VisualElement>(RHandGreen);
            rHandG.dataSource = RHandGreen;
            var lHandG = charac.Q<VisualElement>(LHandGreen);
            lHandG.dataSource = LHandGreen;
            var rLegG = charac.Q<VisualElement>(RLegGreen);
            rLegG.dataSource = RLegGreen;
            var lLegG = charac.Q<VisualElement>(LLegGreen);
            lLegG.dataSource = LLegGreen;

            OpacityChangeBodyChapter(headG, bodyG, rHandG, lHandG, rLegG, lLegG);
            Root.Add(charac);

            return this;
        }
        private void OpacityChangeBodyChapter(params VisualElement[] visualElements)
        {
            foreach (VisualElement visualElement in visualElements)
            {
                visualElement.RegisterCallback<MouseEnterEvent>(e =>
                {
                    foreach (VisualElement el in visualElements)
                    {
                        el.style.opacity = 0;
                    }
                    visualElement.StartFadeInAnimation();
                });
                visualElement.RegisterCallback<MouseLeaveEvent>(e =>
                {
                    visualElement.StartFadeOutAnimation();
                });

                visualElement.RegisterCallback<MouseDownEvent>(e =>
                {
                    VisualElement ve = e.target as VisualElement;
                    if (ve != null)
                    {
                        switch (ve.dataSource)
                        {
                            case HeadGreen:
                                OnBodyChapterClick?.Invoke(BodyChapterType.Head);
                                break;
                            case BodyGreen: 
                                OnBodyChapterClick?.Invoke(BodyChapterType.Body);
                                break;
                            case RHandGreen:
                                OnBodyChapterClick?.Invoke(BodyChapterType.RHand); 
                                break;
                            case LHandGreen:
                                OnBodyChapterClick?.Invoke(BodyChapterType.LHand); 
                                break;
                            case RLegGreen:
                                OnBodyChapterClick?.Invoke(BodyChapterType.RLeg); 
                                break;
                            case LLegGreen:
                                OnBodyChapterClick?.Invoke(BodyChapterType.LLeg); 
                                break;
                        }

                    }
                });
            }
        }

    }
    public class TransformTableVisual
    {
        public const string TRANSFORM_FIELD_NAME = "TransformField";
        public const string CONTAINER = "Container";
        public const string S_WEGHT = "Weght";
        public const string S_DISTANCE = "Distance";

        public readonly VisualElement Root;
        private readonly VisualTreeAsset transformField;
        private readonly Editor editor;
        private Dictionary<FieldInfo, VisualElement> transformAtomList = new Dictionary<FieldInfo, VisualElement>();

        public TransformTableVisual(VisualTreeAsset transformField, Editor editor)
        {
            Root = new VisualElement();

            this.transformField = transformField;
            this.editor = editor;
        }
        public TransformTableVisual Initialize()
        {
            Type targetType = editor.target.GetType();
            FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(Transform))
                {
                    Transform currentTransform = (Transform)field.GetValue(editor.target);
                    SerializedProperty cProperty = editor.serializedObject.FindProperty(field.Name);

                    TemplateContainer tContainer = transformField.CloneTree();
                    transformAtomList.Add(field, tContainer);

                    ObjectField transfField = tContainer.Q<ObjectField>(TRANSFORM_FIELD_NAME);
                    transfField.objectType = typeof(Transform);
                    transfField.value = currentTransform;
                    transfField.label = field.Name;
                    transfField.RegisterValueChangedCallback(evt =>
                    {
                        field.SetValue(editor.target, evt.newValue);
                        editor.serializedObject.Update();
                        editor.SaveChanges();
                    });
                    ConnectionAttribute connectionAttribute = field.GetCustomAttribute<ConnectionAttribute>();
                    if (connectionAttribute != null) transfField.tooltip = $"Connected to {connectionAttribute.ConnectionFieldName}";
                    transfField.BindProperty(cProperty);

                    Slider weightSlider = tContainer.Q<Slider>(S_WEGHT);
                    Slider distanceSlider = tContainer.Q<Slider>(S_DISTANCE);
                    weightSlider.label = "Weght";
                    distanceSlider.label = "Distance";

                    weightSlider.style.opacity = 0f;
                    weightSlider.style.display = DisplayStyle.None;
                    distanceSlider.style.opacity = 0f;
                    distanceSlider.style.display = DisplayStyle.None;

                    VisualElement container = tContainer.Q<VisualElement>(CONTAINER);

                    container.RegisterCallback<MouseEnterEvent>(e =>
                    {
                        weightSlider.StartFadeInAnimation();
                        distanceSlider.StartFadeInAnimation();
                        weightSlider.style.display = DisplayStyle.Flex;
                        distanceSlider.style.display = DisplayStyle.Flex;
                    });
                    container.RegisterCallback<MouseLeaveEvent>(e =>
                    {
                        weightSlider.StartFadeOutAnimation(onComplete: () =>
                        {
                            weightSlider.style.display = DisplayStyle.None;
                        });
                        distanceSlider.StartFadeOutAnimation(onComplete: () =>
                        {
                            distanceSlider.style.display = DisplayStyle.None;
                        });
                    });

                    Root.Add(tContainer);
                }
            }
            return this;
        }

        internal void OnBodyChapterClick(BodyChapterType type)
        {
            foreach (var item in transformAtomList)
            {
                ConnectionAttribute attribute = item.Key.GetCustomAttribute<ConnectionAttribute>();
                if (attribute != null && attribute.BodyChapterType == type) item.Value.style.opacity = 1; 
                else item.Value.style.opacity = 0.5f;
            }
        }
        internal void OnEmptySpaceClick()
        {
            foreach (var item in transformAtomList)
                item.Value.style.opacity = 1;
        }



    }

}
