using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public class DynamicBallsRemoveAnimation : IAnimation
    {
        public Queue<Vector3> bubbleAnimation { get; set; } = new Queue<Vector3>();
        private Vector3 _startFromPos;
        public event Action onCompleteAnimation;
        public IGameObject target;

        private List<(IGameObject obj, float percents)> objectsToInteract;

        public DynamicBallsRemoveAnimation(IGameObject target)
        {
            this.target = target;
        }

        public void playAnimation(List<(IGameObject obj, float percents)> objects = null)
        {
            if (bubbleAnimation.Count == 0)
            {
                onCompleteAnimation?.Invoke();
                return;
            }

            if (objects != null)
            {
                objectsToInteract = objects;
            }

            _startFromPos = target.view.localPosition;

            if (target.row <= FieldManager.LAST_VISIBLE_ROW)
            {
                target.view.setActive = true;
            }

            Vector3 point = bubbleAnimation.Peek();
            SITween.To(target, update, point.z, onComplete);
        }

        private void update(SITween tween)
        {
            Vector3 position = new Vector3(tween.GetTweenValue(_startFromPos.x, bubbleAnimation.Peek().x, SITween.EaseType.Linear),
                tween.GetTweenValue(_startFromPos.y, bubbleAnimation.Peek().y, SITween.EaseType.Linear));

            target.view.localPosition = position;

            if (objectsToInteract != null)
            {
                foreach ((IGameObject obj, float percents) info in objectsToInteract)
                {
                    if (target != info.obj)
                    {

                        if (info.obj.view.setActive)
                        {
                            if (tween.percentage >= info.percents)
                            {
                                Debug.Log(new Vector2(tween.percentage, info.percents));
                                info.obj.view.setActive = false;
                            }
                        }
                    }
                }   
            }
        }

        private void onComplete()
        {
            _startFromPos = bubbleAnimation.Dequeue();
            playAnimation();
            objectsToInteract = null;
        }
    }
}