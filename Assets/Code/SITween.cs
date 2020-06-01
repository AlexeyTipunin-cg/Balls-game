using System;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

namespace Code
{
    public class SITween
    {
        private static List<SITween> _tweensToAdd = new List<SITween>();
        private static List<SITween> _tweens = new List<SITween>();
        private static bool _isUpdating;
        public object target { get; private set; }
        public float percentage
        {
            get { return _percentage; }
        }

        private Action<SITween> _onUpdate;
        private float _time;
        private Action _onComplete;
        private Action _onTweenKill;

        private bool _needToKill;

        private float _runningTime;
        private float _percentage;


        public static SITween To(object target, Action<SITween> onUpdate, float time, Action onComplete = null, Action onTweenKill = null)
        {
            SITween tween = new SITween();

            tween.target = target;
            tween._onUpdate = onUpdate;
            tween._time = time;
            tween._onComplete = onComplete;
            tween._onTweenKill = onTweenKill;

            _tweensToAdd.Add(tween);

            AddToFrameUpdate();

            return tween;
        }

        private static void AddToFrameUpdate()
        {
            if (!_isUpdating)
            {
                _isUpdating = true;
                GameLoader.OnUpdate += updateTweens;
            }
        }

        private static void RemoveFromFrameUpdate()
        {
            if (_isUpdating)
            {
                _isUpdating = false;
                GameLoader.OnUpdate -= updateTweens;
            }
        }

        private static void updateTweens(float deltaTime)
        {
            List<SITween> tweensForKill = null;

            if (_tweensToAdd.Count > 0)
            {
                foreach (SITween tween in _tweensToAdd)
                {
                    _tweens.Add(tween);
                }
            }

            _tweensToAdd.Clear();

            foreach (SITween tween in _tweens)
            {
                if (tween._needToKill)
                {
                    if (tweensForKill == null)
                    {
                        tweensForKill = new List<SITween>();
                    }

                    tweensForKill.Add(tween);
                    continue;
                }

                tween.update(deltaTime);
            }

            if (tweensForKill != null)
            {
                foreach (SITween tween in tweensForKill)
                {
                    _tweens.Remove(tween);
                }
            }

            if (_tweens.Count < 1)
            {
                RemoveFromFrameUpdate();
            }
        }

        private void update(float deltaTime)
        {
            if (_needToKill)
            {
                return;
            }

            calculateTimePercentage(deltaTime);

            if (_percentage >= 1f)
            {
                _onUpdate?.Invoke(this);
                _needToKill = true;
                _onComplete?.Invoke();
            }
            else
            {
                _onUpdate.Invoke(this);
            }
        }

        private void calculateTimePercentage(float deltaTime)
        {
            _runningTime += deltaTime;
            _percentage = _runningTime / _time;
        }

        public float GetTweenValue(float startValue, float endValue, EaseType type )
        {
            if (_percentage >= 1f)
            {
                return endValue;
            }

            float result = startValue;
            switch (type)
            {
                case EaseType.Linear:
                    result = EaseLinear(startValue, endValue, _percentage);
                    break;
            }

            return result;
        }

        public static bool isTweening(object target)
        {            
            foreach (SITween tween in _tweens)
            {   
                if (tween._needToKill && tween.target == target) return false;
                if (tween.target == target) return true;
            }
            
            return false;
        }
        
        public static bool hasAnyTweening()
        {
            if (_tweensToAdd.Count > 0 )
            {
                return true;
            }

            for (int i = 0; i < _tweens.Count; i++)
            {
                if (!_tweens[i]._needToKill) return true;
            }
            return false;
        }

        private float EaseLinear(float startValue, float endValue, float percentage)
        {
            return Mathf.Lerp(startValue, endValue, percentage);
        }
        
        public enum EaseType
        {
            Linear
        }
        
        
    }
}