using UnityEngine;
using System; 
using MvLib;

namespace AtomEngine.Components
{
    [Serializable]
    public class AtomEngineSquareDistanceCheckerComponent : AtomEngineDistanceCheckerComponent
    {
        public AtomEngineSquareDistanceCheckerComponent() : base() { }


        public override bool CheckDistance(Vector3 position)
        {
            return IsInsideSurface(position);
        }
            //Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition); 
        public bool IsInsideSurface(Vector3 position)
        {
            // Преобразуем позицию мыши в мировые координаты
            Face face = parentObject as Face;
            // Получаем нормаль плоскости через векторное произведение первых трёх вершин
            Vector3 v0 = face.Atoms[0].Transform.Position;  // positions[0];
            Vector3 v1 = face.Atoms[1].Transform.Position;  // positions[1];
            Vector3 v2 = face.Atoms[2].Transform.Position;  // positions[2];

            Vector2 mousePosition = Event.current.mousePosition;
            Vector3 positonOnInfinityPlane = mousePosition.GetPointOnPlane(v0 ,v1, v2);
            return positonOnInfinityPlane.IsPointInTriangle(v0, v1, v2);
        }
    }
}
