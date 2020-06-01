using System;
using UnityEngine;

namespace Code
{
    public class Camera : MonoBehaviour
    {
        public static float orthographicSize;

        private void Awake()
        {
            if (UnityEngine.Camera.main != null)
            {
                orthographicSize = UnityEngine.Camera.main.orthographicSize;

                FieldManager.screenWidth = orthographicSize * 2 * Screen.width / Screen.height;
                FieldManager.wallsWidth = FieldManager.screenWidth * 0.1f * 0.5f;
            }
        }
    }
}