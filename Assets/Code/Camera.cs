using System;
using UnityEngine;

namespace Code
{
    public class Camera : MonoBehaviour
    {
        [SerializeField] private GameObject _background;
        public static float orthographicSize;
        private static Shader _shaderDefault;

        private void Awake()
        {
                
            if (UnityEngine.Camera.main != null)
            {
                orthographicSize = UnityEngine.Camera.main.orthographicSize;

                FieldManager.screenWidth = orthographicSize * 2 * Screen.width / Screen.height;
                FieldManager.wallsWidth = FieldManager.screenWidth * 0.1f * 0.5f;
        
                Vector2 cameraSize = new Vector2(UnityEngine.Camera.main.aspect * orthographicSize * 2, orthographicSize * 2);
                Vector2 spriteSize = _background.GetComponent<SpriteRenderer>().sprite.bounds.size;
        
                Vector2 scale = _background.transform.localScale;
                if (cameraSize.x >= cameraSize.y) { // Landscape (or equal)
                    scale *= cameraSize.x / spriteSize.x;
                } else { // Portrait
                    scale *= cameraSize.y / spriteSize.y;
                }
        
                _background.transform.position = Vector2.zero; // Optional
                _background.transform.localScale = scale;
                
            }
        }
    }
}