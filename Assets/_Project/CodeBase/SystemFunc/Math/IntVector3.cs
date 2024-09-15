using UnityEngine;

namespace CodeBase.SystemFunc.Math
{
    [System.Serializable]
    public struct IntVector3
    {
        public int x;
        public int y;
        public int z;
         
        public IntVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public IntVector3(Vector3 position)
        {
            this.x = Mathf.RoundToInt(position.x);
            this.y = Mathf.RoundToInt(position.y);
            this.z = Mathf.RoundToInt(position.z); 
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }
         
        public static implicit operator Vector3(IntVector3 intVec)
        {
            return new Vector3(intVec.x, intVec.y, intVec.z);
        }

        public static implicit operator IntVector3(Vector3 vec)
        {
            return new IntVector3(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
        }

        //public static explicit operator IntVector3(Vector3 vec)
        //{
        //    return new IntVector3(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
        //}
         
        public static IntVector3 operator +(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static IntVector3 operator -(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static IntVector3 operator *(IntVector3 a, int scalar)
        {
            return new IntVector3(a.x * scalar, a.y * scalar, a.z * scalar);
        }

        public static IntVector3 operator /(IntVector3 a, int scalar)
        {
            return new IntVector3(a.x / scalar, a.y / scalar, a.z / scalar);
        }
    }
}
