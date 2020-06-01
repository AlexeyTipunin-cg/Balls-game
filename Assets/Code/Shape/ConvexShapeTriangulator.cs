using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public static class ConvexShapeTriangulator
    {
        public static int[] getTriangles( int segmentNumber)
        {
            List<int> triangles = new List<int>(segmentNumber);
            for (int i = 2; i < segmentNumber; i++)
            {
                triangles.Add(0);
                triangles.Add(i - 1);
                triangles.Add(i);
            }
            return triangles.ToArray();
        }
    }
}