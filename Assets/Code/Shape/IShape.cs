using UnityEngine;

namespace Code
{
    public interface IShape
    {
        void draw();
        void attachToParent(GameObject parent);
        Vector3 localPosition { get; set; }
        void destroy();
        bool setActive { get; set; }
        void attachParticles(ParticleSystem child);
        void rotate(Vector3 angle);
    }
}