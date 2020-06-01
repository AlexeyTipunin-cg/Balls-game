using UnityEditor;
using UnityEngine;

namespace Code
{
    public class Bubble : IGameObject
    {
        private int _id;
        private int _row;
        private int _col;
        private int _type;
        private IAnimation _animation;
        protected IShape _circle;
        
        private static Shader _shaderDefault = Shader.Find("Sprites/Default");
        private static Material _material = new Material(_shaderDefault){renderQueue = 3500};

        public int row
        {
            get { return _row; }
            set { _row = value; }
        }

        public int col
        {
            get { return _col; }
            set { _col = value; }
        }

        public int id
        {
            get { return _id; }
        }

        public int type
        {
            get { return _type; }
        }

        public virtual bool needMatchType
        {
            get { return true; }
        }

        public IShape view
        {
            get { return _circle; }
        }

        public bool isRemoved { get;  set; }

        public virtual IAnimation animation
        {
            get
            {
                if (_animation == null)
                {
                    _animation = new Animation(this);
                }

                return _animation;
            }
        }

        public Bubble(int row, int col, int type)
        {
            _row = row;
            _col = col;
            _type = type;

            _id = GetHashCode();
        }

        public virtual void createShape(float radius,float x, float y, Color32 color, GameObject parent)
        {
            _circle = new Circle(radius ,x, y, color, parent, _material);
            _circle.draw();
        }

        public void removeObject()
        {
            isRemoved = true;
            _circle.destroy();
        }
        
    }
}