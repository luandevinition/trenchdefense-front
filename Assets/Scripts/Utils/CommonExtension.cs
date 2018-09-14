using UnityEngine;

namespace Utils
{
    public static class CommonExtension
    {
        public static bool IsLeft(this Vector3 A, Vector3 B)
        {
            return B.x < A.x;
        }
        
    }
}