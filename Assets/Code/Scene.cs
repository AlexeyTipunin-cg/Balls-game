using UnityEngine;

namespace Code
{
    public class Scene
    {
        public static readonly Scene Instance = new Scene();

        private GameObject _view;
        private RectTransform _rectTransform;
        private BoxCollider _boxCollider;
        private Vector3 _center;
        private float _x;
        private float _y;

        private Scene()
        {
        }

        public GameObject view
        {
            get
            {
                if (ReferenceEquals(_view, null))
                {
                    _view = new GameObject("Scene");
                    _view.AddComponent<Controller>();
                }

                return _view;
            }
        }

        public RectTransform rectTransform
        {
            get
            {
                if (ReferenceEquals(_rectTransform, null))
                {
                    _rectTransform = view.AddComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }

        public BoxCollider boxCollider
        {
            get
            {
                if (ReferenceEquals(_boxCollider, null))
                {
                    _boxCollider = view.AddComponent<BoxCollider>();
                }

                return _boxCollider;
            }
        }

        public bool isTrigger
        {
            get { return boxCollider.isTrigger; }
            set { boxCollider.isTrigger = value; }
        }

        public Vector2 position
        {
            get { return rectTransform.position; }
            set { rectTransform.position = value; }
        }

        public Vector2 size
        {
            get { return rectTransform.sizeDelta; }
            set
            {
                rectTransform.sizeDelta = value;
                boxCollider.size = value;
                boxCollider.center = center;
            }
        }

        public Vector2 pivot
        {
            get { return rectTransform.pivot; }
            set { rectTransform.pivot = value; }
        }

        public float sizeDeltaX
        {
            get { return rectTransform.sizeDelta.x; }
        }

        public float sizeDeltaY
        {
            get { return rectTransform.sizeDelta.y; }
        }

        public Vector3 center
        {
            get { return size * 0.5f; }
        }
    }
}