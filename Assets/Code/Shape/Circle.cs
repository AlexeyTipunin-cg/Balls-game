using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code
{
    public class Circle : IShape
    {
        private const float doubledPI = 2 * (float) Math.PI;
        
        private static int _circleNumber = 0;
        private static int _segmentNumber = 100;
        private static int[] _triangles;
        private static GameObject _circlePrototype;
        
        private Vector3 _center;
        private float _radius;
        private Color32 _color;
        private GameObject _parent;

        private GameObject _circle;
        private Material _material;
        
        private static Dictionary<Color32, Color32[]> _colorCash = new Dictionary<Color32, Color32[]>();
        
        static Circle()
        {
            _circlePrototype = new GameObject("circle");
            _circlePrototype.AddComponent<MeshFilter>();

            _circlePrototype.AddComponent<MeshRenderer>().rendererPriority = 3500;
            _circlePrototype.AddComponent<CircleCollider2D>().isTrigger = true;
            _circlePrototype.AddComponent<RectTransform>();
            _triangles = ConvexShapeTriangulator.getTriangles(_segmentNumber);
        }

        public Circle(float radius, float x,float y, Color32 color , GameObject parent, Material material )
        {
            _radius = radius;
            _center = new Vector3(x,y);
            _color = color;
            _parent = parent;

            _material = material;
        }
        
        public void draw()
        {
            _circle = GameObject.Instantiate(_circlePrototype);
            
            _circle.GetComponent<MeshRenderer>().material = _material;
            _circle.name = "circle" + _circleNumber;
            if (_parent != null)
            {
                _circle.transform.SetParent(_parent.transform);
            }

            _circle.transform.localPosition  = _center;

            Mesh circleMesh = _circle.GetComponent<MeshFilter>().mesh;
            CircleCollider2D collider = _circle.GetComponent<CircleCollider2D>();
            collider.radius = _radius;
            circleMesh.vertices = GetVertices();
            circleMesh.triangles = _triangles;
            _circle.GetComponent<RectTransform>().sizeDelta = circleMesh.bounds.size;
            
            if (_colorCash.ContainsKey(_color))
            {
                circleMesh.colors32 = _colorCash[_color];
            }
            else
            {
                circleMesh.colors32 = Enumerable.Repeat(_color, circleMesh.vertices.Length).ToArray();
                _colorCash.Add(_color, circleMesh.colors32);
            }

            _circleNumber++;
        }
        
        private Vector3[] GetVertices()
        {
            Vector3[] vertices = new Vector3[_segmentNumber];

            for (int point = 0; point < _segmentNumber; point++)
            {
                float x = (float) Math.Cos(doubledPI * point / _segmentNumber) * _radius;
                float y = (float) Math.Sin(doubledPI * point / _segmentNumber) * _radius;
                vertices[point] = new Vector3(x, y);
            }

            return vertices;
        }

        public void attachToParent(GameObject parent)
        {
            _circle.transform.SetParent(_parent.transform);
        }

        public Vector3 localPosition
        {
            get { return _circle.transform.localPosition; }
            set { _circle.transform.localPosition = value; }
        }

        public void destroy()
        {
            _circle.GetComponent<Renderer>().enabled = false;
        }

        private bool _active = true;
        public virtual bool setActive
        {
            get { return _active;}
            set
            {
                if (_active != value)
                {
                    _circle.GetComponent<MeshRenderer>().enabled = value;
                    _active = value;
                }
            }
        }

        public void attachParticles(ParticleSystem child)
        {
            Transform partciles = child.transform;
            partciles.SetParent(_circle.transform);
            partciles.localPosition = new Vector3();
        }

        public void rotate(Vector3 angle)
        {
            _circle.transform.rotation = Quaternion.Euler(angle);
        }

        public float x { get; }
        public float y { get; }
    }
}