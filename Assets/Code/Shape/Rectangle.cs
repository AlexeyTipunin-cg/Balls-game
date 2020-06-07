using System.Linq;
using UnityEngine;

namespace Code
{
    public class Rectangle : IShape
    {
        private static Shader _shader = Shader.Find("Sprites/Default");
        private static int _rectangleNumber = 0;

        private static GameObject _rectanglePrototype;
        private GameObject _rectangle;

        private Vector3[] _verticesPos;
        private Vector3 _center;
        private Color32[] _verticesColor;
        private Vector3 _localPosition;
        private GameObject _parent;
        private RectTransform _rectTransform;

        public RectTransform rectTransform
        {
            get
            {
                if (ReferenceEquals(_rectTransform, null))
                {
                    _rectTransform = _rectangle.GetComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }

        static Rectangle()
        {
            _rectanglePrototype = new GameObject("rectangle");
            _rectanglePrototype.AddComponent<MeshFilter>();
            Material mat = new Material(_shader);
            mat.renderQueue = 3600;
            _rectanglePrototype.AddComponent<MeshRenderer>().material = mat;

            _rectanglePrototype.GetComponent<MeshRenderer>().rendererPriority = 3500;
            _rectanglePrototype.AddComponent<CircleCollider2D>().isTrigger = true;
            _rectanglePrototype.AddComponent<RectTransform>();
        }

        public Rectangle(Vector3 center, Vector3[] verticesPos, Color32 color)
        {
            _verticesPos = verticesPos;
            _center = center;
            _verticesColor = Enumerable.Repeat(color, verticesPos.Length).ToArray();

            _rectangle = UnityEngine.Object.Instantiate(_rectanglePrototype);
            _rectangle.name = "rectangle" + _rectangleNumber;
        }

        public Rectangle(float height, float width, Vector3 center, Color32 color, GameObject parent)
        {
            _verticesPos = new Vector3[4]
            {
                new Vector3(-width * 0.5f, -height * 0.5f), new Vector3(-width * 0.5f, height * 0.5f),
                new Vector3(width * 0.5f, height * 0.5f),
                new Vector3(width * 0.5f, -height * 0.5f)
            };

            _center = center;
            _verticesColor = Enumerable.Repeat(color, _verticesPos.Length).ToArray();
            _parent = parent;

            _rectangle = UnityEngine.Object.Instantiate(_rectanglePrototype);
            _rectangle.name = "rectangle" + _rectangleNumber;
        }

        public void draw()
        {
            if (_parent != null)
            {
                _rectangle.transform.SetParent(_parent.transform);
            }

            _rectangle.AddComponent<MeshFilter>();
            Mesh rectangleMesh = _rectangle.GetComponent<MeshFilter>().mesh;

            rectangleMesh.vertices = _verticesPos;
            rectangleMesh.triangles = new int[] {0, 1, 2, 2, 3, 0};
            rectangleMesh.colors32 = _verticesColor;

            _rectTransform = _rectangle.GetComponent<RectTransform>();
            _rectTransform.sizeDelta = rectangleMesh.bounds.size;
            position = _center;

            _rectangleNumber++;
        }

        public GameObject view()
        {
            return _rectangle;
        }

        public void attachToParent(GameObject parent)
        {
            _rectangle.transform.SetParent(parent.transform);
        }

        Vector3 IShape.localPosition
        {
            get { return _rectangle.transform.localPosition; }
            set { _rectangle.transform.localPosition = value; }
        }

        public Vector3 position
        {
            get { return _rectangle.transform.position; }
            set { _rectangle.transform.position = value; }
        }

        public Vector2 anchoredPosition { get; set; }

        public void destroy()
        {
            throw new System.NotImplementedException();
        }

        public bool setActive { get; set; }
        public void attachParticles(ParticleSystem child)
        {
            throw new System.NotImplementedException();
        }

        public void rotate(Vector3 angle)
        {
            throw new System.NotImplementedException();
        }

        public GameObject gameObj { get; }

        public Vector3 pivotPoint
        {
            get { return rectTransform.pivot; }
            set { rectTransform.pivot = value; }
        }

        public int x { get; }
        public int y { get; }

        public Vector3 localPosition()
        {
            return _rectangle.transform.localPosition;
        }
    }
}