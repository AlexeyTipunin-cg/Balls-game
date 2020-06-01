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
        GameObject gameObj { get; }
    }
}