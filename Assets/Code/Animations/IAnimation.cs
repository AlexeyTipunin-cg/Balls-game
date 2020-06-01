using System;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

namespace Code
{
    public interface IAnimation
    {
        Queue<Vector3> bubbleAnimation { get; set; }
        void playAnimation(List<(IGameObject obj, float percents)> objects = null);
        event Action onCompleteAnimation;
    }
}