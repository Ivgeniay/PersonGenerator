using AtomEngine.Meshes.Constructor;
using System.Collections.Generic;
using AtomEngine.VisualElements;
using UnityEngine.UIElements;
using AtomEngine.Components;
using AtomEngine.SceneViews;
using AtomEngine.Testing;
using UnityEditor;
using UnityEngine;
using System.Linq;
using AtomEngine;
using System; 
using static AtomEngine.SceneViews.SceneToolsOverlay;

namespace Inspector
{
    public delegate void AtomObjectSelectedHandler(List<AtomObject> atomObjects);

    [CustomEditor(typeof(ConstructedElement))]
    internal class ConstructorElementInspector : TestedEditor
    {
        public AtomObjectSelectedHandler OnAtomObjectsSelected;

        public static ConstructorElementInspector Instance;

        private ModeType _modeType = ModeType.None;
        private ToolsType _toolsType = ToolsType.Translate;
        private HandlesOrientationType handlesOrientatiom = HandlesOrientationType.World;
        private VisualElement root; 

        #region Inspector
        public override VisualElement CreateInspectorGUI()
        {
            Instance = this;
            ConstructedElement constructedElement = (ConstructedElement)target;

            Selection.selectionChanged += OnInspectorClose;
            void OnInspectorClose()
            {
                if (Selection.activeObject == null ||
                Selection.activeObject != constructedElement)
                {
                    UnselectAll();
                    Selection.selectionChanged -= OnInspectorClose;
                }
            }

            root = new VisualElement();

            ConstructorOnInspector constructorOnInspector = new ConstructorOnInspector(constructedElement, serializedObject);
            OnAtomObjectsSelected += constructorOnInspector.AtomObject_Selected_Handler;
            root.Add(constructorOnInspector);

            root.Add(base.CreateInspectorGUI());
            root.schedule.Execute(Update).Every(100);
            return root;
        }
        private void Update()
        {
            foreach (AtomView e in root.FindElementsOfType<AtomView>()) e.Update(); 
        }
        #endregion

        #region Scene

        ConstructedElement constructedElement;
        ConstructorOnScene constructorOnScene;
        private void OnEnable()
        {
            constructedElement = (ConstructedElement)target;
            constructorOnScene = new ConstructorOnScene(constructedElement, serializedObject);
            AtomObject[] atomObjects = constructedElement.AtomObject; 
        }

        private void OnSceneGUI()
        {
            Type workingType = null;
            switch (_modeType)
            {
                case ModeType.None: return;
                case ModeType.Object: break;
                case ModeType.Atom: workingType = typeof(Atom); break; 
                case ModeType.Edge: workingType = typeof(Edge); break;
                case ModeType.Face: workingType = typeof(Face); break;
            }

            AtomObject[] aObj = constructorOnScene.SortAtomObjectByDistanceToCamera(
                                    constructedElement.AtomObject
                                    .Where(e => e.GetType() == workingType)
                                    .ToArray());

            bool isStop = false;
            for (int i = 0; i < aObj.Length; i++)
            {
                if (isStop) break;

                AtomObject atomObject = aObj[i]; 

                var checker = atomObject.GetAssignableComponent<AtomEngineDistanceCheckerComponent>();
                var outliner = atomObject.GetComponent<AtomEngineOutlineComponent>();

                isStop = constructorOnScene.CheckHower(checker, outliner);
                constructorOnScene.Handle_AtomObject_Selection(outliner); 
            }

            constructorOnScene.DrawMarker(aObj);
            AtomObject[] selectedAObj = aObj.Where(e => e.GetComponent<AtomEngineOutlineComponent>().IsSelected).ToArray();
            constructorOnScene.DrawPositionHandle(_toolsType, handlesOrientatiom, selectedAObj);

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
        }
        internal void UnselectAll() => constructorOnScene.UnselectAll();

        private void AtomObjectMoved(AtomObject atom) => constructedElement.AtomMoved(atom);
        private void AtomObjectRotate(AtomObject atom) => constructedElement.AtomRotate(atom);

        
        internal void SwitchTools(SceneToolsOverlay.ToolsType toolsType)
        {
            this._toolsType = toolsType;
            SceneView.RepaintAll();
        }
        internal void SwitchMode(SceneToolsOverlay.ModeType modeType)
        {
            this._modeType = modeType;
            SceneView.RepaintAll();
        }
        internal void SwitchHandlesOrientationType(HandlesOrientationType handlesOrientationType)
        {
            handlesOrientatiom = handlesOrientationType;
        }

        #endregion

    }

}

namespace AtomEngine.VisualElements
{
    public class ConstructorOnScene : ConstructorElementView
    {
        public ConstructorOnScene(ConstructedElement constructedElement, SerializedObject serializedObject) : base(constructedElement, serializedObject) { }

        internal bool CheckHower(AtomEngineDistanceCheckerComponent checker, AtomEngineOutlineComponent outliner)
        { 
            if (checker.CheckDistance(Event.current.mousePosition))
            {
                if (!outliner.IsHovered)
                {
                    outliner.OnHover();
                    return true;
                }
            }
            else
            {
                if (outliner.IsHovered) outliner.OnUnhover();
            }
            return false;
        }
        internal void Handle_AtomObject_Selection(AtomEngineOutlineComponent outliner)
        {
            Event e = Event.current;
            if (e == null) return; 
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                if (!outliner.IsHovered) return;
                AtomObject atomObject = outliner.Parent;
                UpdateSelectedAO(atomObject, outliner, e.shift || e.control);
                SceneView.RepaintAll();
                e.Use();
            }
        }
        
        #region Draw
        internal void DrawMarker(params AtomObject[] atomObjects)
        {
            for (int i = 0; i < atomObjects.Length; i++)
            {
                var outliner = atomObjects[i].GetComponent<AtomEngineOutlineComponent>();
                Handles.color = outliner.IsSelected ? outliner.SelectedColor : outliner.NonSelecteColor;
                float handlerSphereRadius = outliner.IsSelected ? outliner.SelectedWidth : outliner.NonSelectedWidth;
                if (atomObjects[i] is Atom atom)
                { 
                    Handles.SphereHandleCap(0, atomObjects[i].Transform.Position, Quaternion.identity, outliner.IsSelected ? outliner.SelectedWidth : outliner.NonSelectedWidth, EventType.Repaint);
                    Handles.Label(atomObjects[i].Transform.Position + Vector3.up * 0.25f, $"Atom {atomObjects[i].GetComponent<AtomEngineAtomIndex>().Index}");
                }
                if (atomObjects[i] is Edge edge)
                {
                    Atom atom1 = edge.Atom;
                    Atom atom2 = edge.Atom2;
                    Vector3 startPos = atom1.GetComponent<AtomEngineTransform>().Position;
                    Vector3 endPos = atom2.GetComponent<AtomEngineTransform>().Position;
                    Handles.DrawLine(startPos, endPos, handlerSphereRadius);
                }
                if (atomObjects[i] is Face face)
                {
                    Handles.color = outliner.NonSelecteColor;
                    Vector3[] positions = face.Atoms.Select(a => a.Transform.Position).ToArray();
                    Handles.DrawAAConvexPolygon(positions);
                }
            }
        } 
        internal void DrawPositionHandle(ToolsType toolsType, HandlesOrientationType handlesOrientatiom, params AtomObject[] atomObjects)
        {
            if (atomObjects == null || atomObjects.Length == 0) return;
            Type type = atomObjects[0].GetType();

            if (atomObjects.Length == 1)
            {
                switch(type)
                {
                    case Type t when t == typeof(Atom):
                        AtomPositionHandler(toolsType, handlesOrientatiom, atomObjects[0] as Atom);
                        break;
                    case Type t when t == typeof(Edge):
                        EdgePositionHandler(toolsType, handlesOrientatiom, atomObjects[0] as Edge);
                        break;
                    case Type t when t == typeof(Face):
                        FacePositionHandler(toolsType, handlesOrientatiom, atomObjects[0] as Face);
                        break;
                } 
            }
            else
            {
                switch (type)
                {
                    case Type t when t == typeof(Atom):
                        AtomMultiPositionHandler(toolsType, handlesOrientatiom, atomObjects.Select(e => e as Atom).ToArray());
                        break;
                    case Type t when t == typeof(Edge):
                        EdgeMultiPositionHandler(toolsType, handlesOrientatiom, atomObjects.Select(e => e as Edge).ToArray());
                        break;
                    case Type t when t == typeof(Face):
                        FaceMultiPositionHandler(toolsType, handlesOrientatiom, atomObjects[0] as Face);
                        break;
                }
            }
        }
        #endregion

        #region Handlers
        public Quaternion CalculateEdgeRotation(Vector3 atom1, Vector3 atom2)
        {
            // Вектор направления вдоль ребра
            Vector3 edgeDirection = (atom2 - atom1).normalized;

            // Выбираем вспомогательный вектор (например, мировую ось вверх)
            Vector3 up = Vector3.up;

            // Если ребро почти вертикально, берем другую ось
            if (Vector3.Dot(up, edgeDirection) > 0.99f)
            {
                up = Vector3.forward;
            }

            // Находим вектор, перпендикулярный к направлению ребра
            Vector3 perpendicular = Vector3.Cross(up, edgeDirection).normalized;

            // Находим вторую ось, которая завершит систему координат
            Vector3 secondPerpendicular = Vector3.Cross(edgeDirection, perpendicular);

            // Создаем кватернион на основе локальной системы координат
            Quaternion rotation = Quaternion.LookRotation(edgeDirection, secondPerpendicular);

            return rotation;
        }
        private void AtomPositionHandler(ToolsType _toolsType, HandlesOrientationType handlesOrientatiom, Atom atom) 
        {
            var transform = atom.GetComponent<AtomEngineTransform>();
            switch (_toolsType)
            {
                case ToolsType.Translate:
                     
                    Vector3 centerPosition = transform.Position;
                    Vector3 newCenterPosition = Handles.PositionHandle(centerPosition, handlesOrientatiom == HandlesOrientationType.World ? Quaternion.identity : transform.Rotation);

                    if (newCenterPosition != centerPosition)
                    {
                        Vector3 offset = newCenterPosition - centerPosition;  // Сдвиг центра  
                        transform.SetPosition(atom.Transform.Position + offset);
                        constructedElement.AtomMoved(atom);
                        Undo.RecordObject(constructedElement, $"Moved Atom id: {atom.GetComponent<AtomEngineAtomIndex>().Index}"); 
                    }

                    break;

                case ToolsType.Rotate:
                    
                    Quaternion newRotation = Handles.RotationHandle(transform.Rotation, transform.Position);
                    if (newRotation != transform.Rotation)
                    {
                        transform.Rotation = newRotation;
                        constructedElement.AtomRotate(atom);
                        Undo.RecordObject(constructedElement, $"Rotated Atom");
                    }
                    break;
            }
        }
        private void AtomMultiPositionHandler(ToolsType _toolsType, HandlesOrientationType handlesOrientatiom, params Atom[] atoms) 
        {
            Vector3 centerPosition = Vector3.zero;

            foreach (Atom atom in atoms) 
                centerPosition += atom.Transform.Position;
            
            centerPosition /= atoms.Length;

            switch (_toolsType)
            {
                case ToolsType.Translate:

                    //Quaternion handlesRotation = handlesOrientatiom == HandlesOrientationType.World ? Quaternion.identity : transform.Rotation;
                    Vector3 newCenterPosition = Handles.PositionHandle(centerPosition, Quaternion.identity);
                    if (newCenterPosition != centerPosition)
                    {
                        Vector3 offset = newCenterPosition - centerPosition;
                        foreach (Atom atom in atoms)
                        {
                            atom.Transform.SetPosition(atom.Transform.Position + offset);
                            constructedElement.AtomMoved(atom);
                            Undo.RecordObject(constructedElement, $"Moved Atom id: {atom.GetComponent<AtomEngineAtomIndex>().Index}");
                        }
                    }
                    break;

                case ToolsType.Rotate:
                    Quaternion centerRotation = Quaternion.identity;
                    Quaternion newCenterRotation = Handles.RotationHandle(centerRotation, centerPosition);

                    if (newCenterRotation != centerRotation)
                    {
                        Quaternion rotationDelta = newCenterRotation * Quaternion.Inverse(centerRotation);
                        foreach (Atom atom in atoms)
                        { 
                            Vector3 direction = atom.Transform.Position - centerPosition;
                            Vector3 newDirection = rotationDelta * direction;
                            atom.Transform.SetPosition(centerPosition + newDirection);
                            constructedElement.AtomMoved(atom);
                            Undo.RecordObject(constructedElement, $"Rotated Atom id: {atom.GetComponent<AtomEngineAtomIndex>().Index}");
                        }
                    }
                    break;
            }
        }
        private void EdgePositionHandler(ToolsType _toolsType, HandlesOrientationType handlesOrientatiom, Edge edge) 
        {
            Atom atom1 = edge.Atom;
            Atom atom2 = edge.Atom2;

            switch (_toolsType)
            {
                case ToolsType.Translate:
                     
                    Vector3 startPos = atom1.Transform.Position;
                    Vector3 endPos = atom2.Transform.Position;
                    
                    Vector3 midpoint = (startPos + endPos) / 2;
                    Vector3 newPosition = Handles.PositionHandle(midpoint, handlesOrientatiom == HandlesOrientationType.World ? Quaternion.identity : CalculateEdgeRotation(startPos, endPos));
                    Vector3 offset = newPosition - midpoint;  

                    if (offset != Vector3.zero)
                    {
                        Undo.RecordObject(constructedElement, $"Moved Edge {edge}");

                        // Перемещение обоих атомов на половину смещения грани
                        atom1.Transform.SetPosition(startPos + offset / 2);
                        atom2.Transform.SetPosition(endPos + offset / 2);

                        constructedElement.AtomMoved(atom1);
                        constructedElement.AtomMoved(atom2);
                    } 
                    break;

                case ToolsType.Rotate:
                    // Сохраняем предыдущее вращение как поле класса, чтобы отслеживать дельту вращения между кадрами
                    Quaternion previousRotation = Quaternion.identity;

                    // Определяем центральную точку между атомами
                    Vector3 centerPos = (atom1.Transform.Position + atom2.Transform.Position) / 2;

                    // Ручка для вращения вокруг центральной точки
                    Quaternion newRotation = Handles.RotationHandle(previousRotation, centerPos);

                    // Если вращение изменилось
                    if (newRotation != previousRotation)
                    {
                        // Рассчитываем дельту вращения как разницу между предыдущим и текущим вращением
                        Quaternion rotationDelta = newRotation * Quaternion.Inverse(previousRotation);

                        // Применяем вращение к каждому атому относительно центральной точки
                        Vector3 direction1 = atom1.Transform.Position - centerPos;
                        Vector3 direction2 = atom2.Transform.Position - centerPos;

                        // Вращаем атомы, смещая их позиции относительно центральной точки
                        Vector3 newDirection1 = rotationDelta * direction1;
                        Vector3 newDirection2 = rotationDelta * direction2;

                        atom1.GetComponent<AtomEngineTransform>().SetPosition(centerPos + newDirection1);
                        atom2.GetComponent<AtomEngineTransform>().SetPosition(centerPos + newDirection2);

                        constructedElement.AtomMoved(atom1);
                        constructedElement.AtomMoved(atom2);

                        // Записываем изменение в Undo для возможности отката действий
                        Undo.RecordObject(constructedElement, $"Rotated Edge {edge}");

                        // Обновляем предыдущее вращение для следующего шага
                        previousRotation = newRotation;
                    }
                    break;

                case ToolsType.Scale:
                    // Определяем начальные позиции атомов и центральную точку
                    startPos = atom1.Transform.Position;
                    endPos = atom2.Transform.Position;
                    midpoint = (startPos + endPos) / 2;

                    // Масштабирование через ScaleHandle, начинаем с единичного масштаба
                    float currentDistance = Vector3.Distance(startPos, endPos);
                    float newDistance = Handles.ScaleSlider(currentDistance, midpoint, Vector3.forward, Quaternion.identity, currentDistance, 0.1f);

                    if (!Mathf.Approximately(newDistance, currentDistance))
                    {
                        float scaleFactor = newDistance / currentDistance;  // Определяем коэффициент масштаба

                        // Перемещаем атомы относительно центральной точки, изменяя расстояние между ними
                        Vector3 scaledOffset1 = (startPos - midpoint) * scaleFactor;
                        Vector3 scaledOffset2 = (endPos - midpoint) * scaleFactor;

                        atom1.Transform.SetPosition(midpoint + scaledOffset1);
                        atom2.Transform.SetPosition(midpoint + scaledOffset2);

                        constructedElement.AtomMoved(atom1);
                        constructedElement.AtomMoved(atom2);

                        Undo.RecordObject(constructedElement, $"Scaled Edge {edge}");
                    }
                    break;
            }
        }
        private void EdgeMultiPositionHandler(ToolsType _toolsType, HandlesOrientationType handlesOrientatiom, params Edge[] edges)
        {
            if (edges == null || edges.Length == 0) return;

            switch (_toolsType)
            {
                case ToolsType.Translate:
                    // Вычисляем среднюю точку всех граней
                    Vector3 totalMidpoint = Vector3.zero;
                    foreach (var edge in edges)
                    {
                        Vector3 startPos = edge.Atom.Transform.Position;
                        Vector3 endPos = edge.Atom2.Transform.Position;
                        totalMidpoint += (startPos + endPos) / 2;
                    }
                    totalMidpoint /= edges.Length;

                    // Отображаем PositionHandle в средней точке
                    Vector3 newTotalMidpoint = Handles.PositionHandle(totalMidpoint, Quaternion.identity);
                    Vector3 offset = newTotalMidpoint - totalMidpoint;

                    if (offset != Vector3.zero)
                    {
                        // Перемещаем все атомы на половину смещения грани
                        foreach (var edge in edges)
                        {
                            Atom atom1 = edge.Atom;
                            Atom atom2 = edge.Atom2;

                            Vector3 startPos = atom1.Transform.Position;
                            Vector3 endPos = atom2.Transform.Position;

                            atom1.GetComponent<AtomEngineTransform>().SetPosition(startPos + offset / 2);
                            atom2.GetComponent<AtomEngineTransform>().SetPosition(endPos + offset / 2);

                            constructedElement.AtomMoved(atom1);
                            constructedElement.AtomMoved(atom2);

                            Undo.RecordObject(constructedElement, $"Moved Edge {edge}");
                        }
                    }
                    break;

                case ToolsType.Rotate:
                    // Сохраняем предыдущее вращение как поле класса, чтобы отслеживать дельту вращения между кадрами
                    Quaternion previousRotation = Quaternion.identity;

                    // Определяем центр вращения (средняя точка)
                    Vector3 totalCenter = Vector3.zero;
                    foreach (var edge in edges)
                    {
                        Vector3 startPos = edge.Atom.Transform.Position;
                        Vector3 endPos = edge.Atom2.Transform.Position;
                        totalCenter += (startPos + endPos) / 2;
                    }
                    totalCenter /= edges.Length;

                    // Ручка для вращения вокруг центральной точки
                    Quaternion newRotation = Handles.RotationHandle(previousRotation, totalCenter);

                    // Если вращение изменилось
                    if (newRotation != previousRotation)
                    {
                        // Рассчитываем дельту вращения
                        Quaternion rotationDelta = newRotation * Quaternion.Inverse(previousRotation);

                        // Вращаем каждую грань
                        foreach (var edge in edges)
                        {
                            Atom atom1 = edge.Atom;
                            Atom atom2 = edge.Atom2;

                            // Рассчитываем направление атомов относительно центра
                            Vector3 direction1 = atom1.Transform.Position - totalCenter;
                            Vector3 direction2 = atom2.Transform.Position - totalCenter;

                            // Применяем дельту вращения
                            Vector3 newDirection1 = rotationDelta * direction1;
                            Vector3 newDirection2 = rotationDelta * direction2;

                            atom1.GetComponent<AtomEngineTransform>().SetPosition(totalCenter + newDirection1);
                            atom2.GetComponent<AtomEngineTransform>().SetPosition(totalCenter + newDirection2);

                            constructedElement.AtomMoved(atom1);
                            constructedElement.AtomMoved(atom2);

                            Undo.RecordObject(constructedElement, $"Rotated Edge {edge}");
                        }

                        // Обновляем предыдущее вращение
                        previousRotation = newRotation;
                    }
                    break;

                case ToolsType.Scale:
                    // Вычисляем средний масштаб для всех граней
                    Vector3 totalMidpointForScale = Vector3.zero;
                    float totalCurrentDistance = 0f;
                    foreach (var edge in edges)
                    {
                        Vector3 startPos = edge.Atom.Transform.Position;
                        Vector3 endPos = edge.Atom2.Transform.Position;

                        totalMidpointForScale += (startPos + endPos) / 2;
                        totalCurrentDistance += Vector3.Distance(startPos, endPos);
                    }
                    totalMidpointForScale /= edges.Length;
                    totalCurrentDistance /= edges.Length;

                    float newDistance = Handles.ScaleSlider(totalCurrentDistance, totalMidpointForScale, Vector3.forward, Quaternion.identity, totalCurrentDistance, 0.1f);

                    if (!Mathf.Approximately(newDistance, totalCurrentDistance))
                    {
                        float scaleFactor = newDistance / totalCurrentDistance;

                        foreach (var edge in edges)
                        {
                            Atom atom1 = edge.Atom;
                            Atom atom2 = edge.Atom2;

                            Vector3 startPos = atom1.Transform.Position;
                            Vector3 endPos = atom2.Transform.Position;

                            Vector3 scaledOffset1 = (startPos - totalMidpointForScale) * scaleFactor;
                            Vector3 scaledOffset2 = (endPos - totalMidpointForScale) * scaleFactor;

                            atom1.GetComponent<AtomEngineTransform>().SetPosition(totalMidpointForScale + scaledOffset1);
                            atom2.GetComponent<AtomEngineTransform>().SetPosition(totalMidpointForScale + scaledOffset2);

                            constructedElement.AtomMoved(atom1);
                            constructedElement.AtomMoved(atom2);

                            Undo.RecordObject(constructedElement, $"Scaled Edge {edge}");
                        }
                    }
                    break;
            }
        }
        private void FacePositionHandler(ToolsType _toolsType, HandlesOrientationType handlesOrientatiom, Face face)
        {
            var atoms = face.Atoms;

            switch (_toolsType)
            {
                case ToolsType.Translate:
                    Vector3 totalPosition = Vector3.zero;
                    foreach (var atom in atoms)
                    {
                        totalPosition += atom.Transform.Position;
                    }
                    totalPosition /= atoms.Length;

                    Vector3 v1 = atoms[1].Transform.Position - atoms[0].Transform.Position;
                    Vector3 v2 = atoms[2].Transform.Position - atoms[0].Transform.Position; 
                    Vector3 normal = Vector3.Cross(v1, v2);
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);

                    Quaternion handlesRotation = handlesOrientatiom == HandlesOrientationType.World ? Quaternion.identity : rotation;
                    Vector3 newTotalPosition = Handles.PositionHandle(totalPosition, handlesRotation);
                    Vector3 offset = newTotalPosition - totalPosition;

                    if (offset != Vector3.zero)
                    {
                        Undo.RecordObject(constructedElement, $"Moved Face {face}");

                        foreach (var atom in atoms)
                        {
                            Vector3 atomPosition = atom.Transform.Position;
                            atom.GetComponent<AtomEngineTransform>().SetPosition(atomPosition + offset);
                            constructedElement.AtomMoved(atom);
                        }
                    }
                    break;

                case ToolsType.Rotate:
                    Quaternion previousRotation = Quaternion.identity;

                    Vector3 centerPosition = Vector3.zero;
                    foreach (var atom in atoms)
                    {
                        centerPosition += atom.Transform.Position;
                    }
                    centerPosition /= atoms.Length;

                    Quaternion newRotation = Handles.RotationHandle(previousRotation, centerPosition);

                    if (newRotation != previousRotation)
                    {
                        Quaternion rotationDelta = newRotation * Quaternion.Inverse(previousRotation);

                        foreach (var atom in atoms)
                        {
                            Vector3 direction = atom.Transform.Position - centerPosition;
                            Vector3 newDirection = rotationDelta * direction;

                            atom.GetComponent<AtomEngineTransform>().SetPosition(centerPosition + newDirection);
                            constructedElement.AtomMoved(atom);
                        }

                        Undo.RecordObject(constructedElement, $"Rotated Face {face}");
                        previousRotation = newRotation;
                    }
                    break;

                case ToolsType.Scale:
                    Vector3 centerForScale = Vector3.zero;
                    foreach (var atom in atoms)
                    {
                        centerForScale += atom.Transform.Position;
                    }
                    centerForScale /= atoms.Length;

                    float currentDistance = 0f;
                    foreach (var atom in atoms)
                    {
                        currentDistance += Vector3.Distance(atom.Transform.Position, centerForScale);
                    }
                    currentDistance /= atoms.Length;

                    float newDistance = Handles.ScaleSlider(currentDistance, centerForScale, Vector3.forward, Quaternion.identity, currentDistance, 0.1f);

                    if (!Mathf.Approximately(newDistance, currentDistance))
                    {
                        float scaleFactor = newDistance / currentDistance;

                        foreach (var atom in atoms)
                        {
                            Vector3 direction = atom.Transform.Position - centerForScale;
                            Vector3 newDirection = direction * scaleFactor;

                            atom.GetComponent<AtomEngineTransform>().SetPosition(centerForScale + newDirection);
                            constructedElement.AtomMoved(atom);
                        }

                        Undo.RecordObject(constructedElement, $"Scaled Face {face}");
                    }
                    break;
            }
        }
        private void FaceMultiPositionHandler(ToolsType _toolsType, HandlesOrientationType handlesOrientatiom, params Face[] faces)
        {
            if (faces == null || faces.Length == 0) return;

            switch (_toolsType)
            {
                case ToolsType.Translate:
                    Vector3 totalPosition = Vector3.zero;
                    int atomCount = 0;
                    foreach (var face in faces)
                    {
                        foreach (var atom in face.Atoms)
                        {
                            totalPosition += atom.Transform.Position;
                            atomCount++;
                        }
                    }
                    totalPosition /= atomCount;

                    Vector3 newTotalPosition = Handles.PositionHandle(totalPosition, Quaternion.identity);
                    Vector3 offset = newTotalPosition - totalPosition;

                    if (offset != Vector3.zero)
                    {
                        foreach (var face in faces)
                        {
                            foreach (var atom in face.Atoms)
                            {
                                Vector3 atomPosition = atom.Transform.Position;
                                atom.GetComponent<AtomEngineTransform>().SetPosition(atomPosition + offset);
                                constructedElement.AtomMoved(atom);
                            }
                            Undo.RecordObject(constructedElement, $"Moved Face {face}");
                        }
                    }
                    break;

                case ToolsType.Rotate:
                    Quaternion previousRotation = Quaternion.identity;
                    Vector3 centerPosition = Vector3.zero;
                    int totalAtoms = 0;

                    foreach (var face in faces)
                    {
                        foreach (var atom in face.Atoms)
                        {
                            centerPosition += atom.Transform.Position;
                            totalAtoms++;
                        }
                    }
                    centerPosition /= totalAtoms;

                    Quaternion newRotation = Handles.RotationHandle(previousRotation, centerPosition);

                    if (newRotation != previousRotation)
                    {
                        Quaternion rotationDelta = newRotation * Quaternion.Inverse(previousRotation);

                        foreach (var face in faces)
                        {
                            foreach (var atom in face.Atoms)
                            {
                                Vector3 direction = atom.Transform.Position - centerPosition;
                                Vector3 newDirection = rotationDelta * direction;

                                atom.GetComponent<AtomEngineTransform>().SetPosition(centerPosition + newDirection);
                                constructedElement.AtomMoved(atom);
                            }
                            Undo.RecordObject(constructedElement, $"Rotated Face {face}");
                        }

                        previousRotation = newRotation;
                    }
                    break;

                case ToolsType.Scale:
                    Vector3 centerForScale = Vector3.zero;
                    int totalAtomCount = 0;
                    float totalCurrentDistance = 0f;

                    foreach (var face in faces)
                    {
                        foreach (var atom in face.Atoms)
                        {
                            centerForScale += atom.Transform.Position;
                            totalAtomCount++;
                        }
                    }
                    centerForScale /= totalAtomCount;

                    foreach (var face in faces)
                    {
                        foreach (var atom in face.Atoms)
                        {
                            totalCurrentDistance += Vector3.Distance(atom.Transform.Position, centerForScale);
                        }
                    }
                    totalCurrentDistance /= totalAtomCount;

                    float newDistance = Handles.ScaleSlider(totalCurrentDistance, centerForScale, Vector3.forward, Quaternion.identity, totalCurrentDistance, 0.1f);

                    if (!Mathf.Approximately(newDistance, totalCurrentDistance))
                    {
                        float scaleFactor = newDistance / totalCurrentDistance;

                        foreach (var face in faces)
                        {
                            foreach (var atom in face.Atoms)
                            {
                                Vector3 direction = atom.Transform.Position - centerForScale;
                                Vector3 newDirection = direction * scaleFactor;

                                atom.GetComponent<AtomEngineTransform>().SetPosition(centerForScale + newDirection);
                                constructedElement.AtomMoved(atom);
                            }
                            Undo.RecordObject(constructedElement, $"Scaled Face {face}");
                        }
                    }
                    break;
            }
        }
        #endregion

        #region Select
        private void UpdateSelectedAO(AtomObject atomObject, AtomEngineOutlineComponent outliner, bool isMultiTab)
        {
            if (isMultiTab)
            {
                if (outliner.IsSelected)
                { 
                    UnSelectAtomObject(atomObject, outliner);
                }
                else
                {
                    SelectObjectAtom(atomObject, outliner);
                } 
            }
            else
            {
                UnselectAll();
                SelectObjectAtom(atomObject, outliner);
            } 
        }
        internal void UnselectAll()
            {
                foreach (var item in constructedElement.AtomObject)
                    item.GetComponent<AtomEngineOutlineComponent>().OnDeselected();
            }
        internal void SelectObjectAtom(AtomObject atomObject, AtomEngineOutlineComponent outliner)
            {
                if (atomObject is null) return; 
                outliner.OnSelected(); 
            }
        internal void UnSelectAtomObject(AtomObject atomObject, AtomEngineOutlineComponent outliner)
        {
            if (atomObject is null) return;
            outliner.OnDeselected();
        }
        #endregion

        #region Sort
        public AtomObject[] SortAtomObjectByDistanceToCamera(AtomObject[] atomObjects)
        {
            if (atomObjects == null || atomObjects.Length == 0) return atomObjects;
            if (atomObjects[0] is Atom atom) return SortAtomsByDistanceToCamera(atomObjects.Cast<Atom>().ToArray());
            if (atomObjects[0] is Edge edge) return SortEdgesByDistanceToCamera(atomObjects.Cast<Edge>().ToArray());
            if (atomObjects[0] is Face face) return SortFacesByDistanceToCamera(atomObjects.Cast<Face>().ToArray());

            return atomObjects;
        }
        public Atom[] SortAtomsByDistanceToCamera(Atom[] atoms)
        {
            var camera = SceneView.lastActiveSceneView.camera;
            return atoms.OrderBy(atom => Vector3.Distance(camera.transform.position, atom.Transform.Position)).ToArray();
        }
        public Edge[] SortEdgesByDistanceToCamera(Edge[] edges)
        {
            var camera = SceneView.lastActiveSceneView.camera;

            return edges.OrderBy(edge =>
            {
                Vector3 midPoint = (edge.Atom.Transform.Position + edge.Atom2.Transform.Position) / 2;
                return Vector3.Distance(camera.transform.position, midPoint);
            }).ToArray();
        }
        public Face[] SortFacesByDistanceToCamera(Face[] faces)
        {
            var camera = SceneView.lastActiveSceneView.camera; 
            return faces.OrderBy(face =>
            {
                Vector3 centroid = Vector3.zero;
                foreach (var atom in face.Atoms)
                {
                    centroid += atom.Transform.Position;
                }
                centroid /= face.Atoms.Length;
                return Vector3.Distance(camera.transform.position, centroid);
            }).ToArray();
        }
        #endregion


        private void AtomObjectMoved(AtomObject atom) => constructedElement.AtomMoved(atom);
        private void AtomObjectRotate(AtomObject atom) => constructedElement.AtomRotate(atom);
    }
} 