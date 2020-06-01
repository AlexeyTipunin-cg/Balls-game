using UnityEngine;

namespace Code
{
    public class UniversalBubble : Bubble
    {
        private static Shader _shaderDefault = Shader.Find("BubbleGame/AllColorsShader");
        private static Material _material = new Material(_shaderDefault){renderQueue = 3500};
        private static ParticleSystem starsPrortotype;
        private ParticleSystem _stars;

        private IAnimation _animation;
        
        public override IAnimation animation
        {
            get
            {
                if (_animation == null)
                {
                    _animation = new DynamicBallsRemoveAnimation(this);
                }

                return _animation;
            }
        }

        static UniversalBubble()
        {
            starsPrortotype = Resources.Load<ParticleSystem>("StarsEffect");
        }
        
        public UniversalBubble(int row, int col, int type) : base(row, col, type)
        {
            _stars = GameObject.Instantiate(starsPrortotype);
        }


        public override void createShape(float radius, float x, float y, Color32 color, GameObject parent)
        {
            _circle = new Circle(radius ,x, y, color, parent, _material );
            _circle.draw();
            _stars.transform.SetParent(_circle.gameObj.transform);
            _stars.transform.localPosition = new Vector3();
        }

        public override bool needMatchType
        {
            get { return false; }
        }
    }
}