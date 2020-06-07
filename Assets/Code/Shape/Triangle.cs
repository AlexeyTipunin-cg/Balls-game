using UnityEngine;

namespace Code
{
    public class Triangle:IShape
    {
        private static Shader _shader = Shader.Find("Sprites/Default");
        private static int _triangleNumber = 0;
        private Vector3[] _verticesPos;
        private Vector2[] _triangleSize;
        private Color32[] _verticesColor;
        private Vector3 _localPosition;

        public Triangle(Vector3[] verticesPos, Vector2[] triangleSize, Color32[] verticesColor)
        {
            _verticesPos = verticesPos;
            _triangleSize = triangleSize;
            _verticesColor = verticesColor;
        }
        
        public void draw()
        {
            GameObject triangle = new GameObject("triangle" + _triangleNumber);
            
            triangle.AddComponent<MeshFilter>();
            triangle.AddComponent<MeshRenderer>().material = new Material(_shader);
            Mesh triangleMesh = triangle.GetComponent<MeshFilter>().mesh;
            
            triangleMesh.Clear();
        
            triangleMesh.vertices = _verticesPos;
            triangleMesh.uv = _triangleSize;
            triangleMesh.triangles =  new int[] {0, 1, 2};
            triangleMesh.colors32 = _verticesColor;

            _triangleNumber++;
        }

        public GameObject view()
        {
            throw new System.NotImplementedException();
        }

        public void attachToParent(GameObject parent)
        {
            throw new System.NotImplementedException();
        }

        Vector3 IShape.localPosition
        {
            get => _localPosition;
            set => _localPosition = value;
        }

        public Vector3 position { get; set; }
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

        public Vector3 pivotPoint { get; set; }

        public int x { get; }
        public int y { get; }

        public Vector3 localPosition()
        {
            throw new System.NotImplementedException();
        }
    }
}