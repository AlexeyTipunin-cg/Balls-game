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
        
        public UniversalBubble( BallsTypes type) : base(type)
        {
            _stars = GameObject.Instantiate(starsPrortotype);
        }


        public override void createShape(float radius, float x, float y, Color32 color, GameObject parent)
        {
            _circle = new Circle(radius ,x, y, color, parent, _material );
            _circle.draw();
            _circle.attachParticles(_stars);
        }

        private bool isActive;
        public override bool setActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    if (!value)
                    {
                        GameObjectsPool.addToPool(this);
                        _stars.Stop();
                    }

                    if (value)
                    {
                        _stars.Play();
                    }


                    isRemoved = !value;
                    _circle.setActive = value;
                    isActive = value;
                }
            }
        }

        public override bool needMatchType
        {
            get { return false; }
        }
        
        
    }
}