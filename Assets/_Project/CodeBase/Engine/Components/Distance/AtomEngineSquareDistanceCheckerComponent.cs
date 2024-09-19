using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AtomEngine.Components
{
    [Serializable]
    public class AtomEngineSquareDistanceCheckerComponent : AtomEngineDistanceCheckerComponent
    {
        public AtomEngineSquareDistanceCheckerComponent(AtomObject parenObject, params Vector3[] points) : base(parenObject, points) { }


        public override bool CheckDistance(Vector3 position)
        {
            return IsInsideSurface(position);
        }
        public bool IsInsideSurface(Vector3 position)
        {
            // Преобразуем позицию мыши в мировые координаты
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            // Получаем нормаль плоскости через векторное произведение первых трёх вершин
            Vector3 v0 = positions[0];
            Vector3 v1 = positions[1];
            Vector3 v2 = positions[2];

            Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

            // Уравнение плоскости: A * x + B * y + C * z + D = 0
            // A, B, C — компоненты нормали к плоскости
            float A = normal.x;
            float B = normal.y;
            float C = normal.z;
            float D = -(A * v0.x + B * v0.y + C * v0.z); // Используем v0 для нахождения D

            // Параметры луча
            Vector3 rayOrigin = mouseRay.origin;
            Vector3 rayDirection = mouseRay.direction;

            // Решаем уравнение пересечения луча с плоскостью
            float denominator = A * rayDirection.x + B * rayDirection.y + C * rayDirection.z;
            if (Mathf.Abs(denominator) < Mathf.Epsilon)
            {
                // Луч параллелен плоскости, пересечения нет
                return false;
            }

            float t = -(A * rayOrigin.x + B * rayOrigin.y + C * rayOrigin.z + D) / denominator;

            if (t < 0)
            {
                // Пересечение находится позади камеры (то есть мышь не указывает на поверхность)
                return false;
            }

            // Точка пересечения на плоскости
            Vector3 intersectionPoint = rayOrigin + t * rayDirection;

            // Проверяем, находится ли точка пересечения внутри многоугольника
            return IsPointInsidePolygon(intersectionPoint);
        }

        private bool IsPointInsidePolygon(Vector3 point)
        {
            Vector2[] screenPositions = new Vector2[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                screenPositions[i] = HandleUtility.WorldToGUIPoint(positions[i]);
            }

            // Преобразуем точку пересечения в экранные координаты
            Vector2 screenPoint = HandleUtility.WorldToGUIPoint(point);

            bool isInside = false;
            for (int i = 0, j = screenPositions.Length - 1; i < screenPositions.Length; j = i++)
            {
                Vector2 point1 = screenPositions[i];
                Vector2 point2 = screenPositions[j];

                if ((point1.y > screenPoint.y) != (point2.y > screenPoint.y) &&
                    screenPoint.x < (point2.x - point1.x) * (screenPoint.y - point1.y) / (point2.y - point1.y) + point1.x)
                {
                    isInside = !isInside;
                }
            }

            return isInside;
        }

    }
}
