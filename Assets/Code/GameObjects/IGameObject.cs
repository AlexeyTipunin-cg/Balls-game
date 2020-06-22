using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public interface IGameObject
    {
        int id { get; }
        int row { get; set; }
        int col { get; set; }
        BallsTypes type { get; }
        bool needMatchType { get; }
        bool isRemoved { get; set; }
        IAnimation animation { get; }
        IShape view { get; }
        void createShape(float radius ,float x, float y, Color32 color, GameObject parent);
        bool setActive { get; set; }
        void rotate(Vector3 rotate);
        List<(IGameObject obj, float percents)> findObjectsToDelete(BallControllerSwitcher controllerSwitcher, float angle);
    }
}