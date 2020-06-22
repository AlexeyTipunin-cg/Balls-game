using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code
{
    public class Controller : MonoBehaviour
    {
        public float arrowAngle { get; private set; }
        public static bool hasWon { get; private set; }
        private Vector3 _prevMousePos;
        private Vector3 _lastMousePos;

        private bool _isEntered;
        
        private BallControllerSwitcher switcher = new BallControllerSwitcher();

        private void OnMouseDown()
        {
            if (_isEntered && !EventSystem.current.IsPointerOverGameObject())
            {
                if (processMouseCoords())
                {
                    if (!SITween.hasAnyTweening())
                    {
                        OnBubbleShot();
                    }
                }
            }
        }

        public static int totalScore;

        private void OnBubbleShot()
        {
            IGameObject playerBubble = FieldManager.Instance.currentPlayerBall;

            List<(IGameObject obj, float percents)> bubblesToDelete = playerBubble.findObjectsToDelete(switcher, arrowAngle);

            void onCompletePlayerAnimation()
            {
                playerBubble.animation.onCompleteAnimation -= onCompletePlayerAnimation;
                removeCluster(bubblesToDelete.Select(x => x.obj).ToList());
                FieldManager.Instance.createPlayerBubble();

                totalScore += bubblesToDelete.Count;

                if (FieldManager.Instance.findCurrentMinHeight() <= 3)
                {
                    hasWon = false;
                    GameLoader.uiController.openWindow();
                    SaveManager.Instance.bestScore = totalScore;
                    return;
                }

                if (FieldManager.Instance.field.Count == 0)
                {
                    hasWon = true;
                    GameLoader.uiController.openWindow();
                    SaveManager.Instance.bestScore = totalScore;
                    return;
                }

                moveField();
            }

            playerBubble.animation.onCompleteAnimation += onCompletePlayerAnimation;
            playerBubble.animation.playAnimation(bubblesToDelete);
        }
        
        
        public void removeCluster(List<IGameObject> gameObjects)
        {
            foreach (IGameObject gameObject in gameObjects)
            {
                gameObject.setActive = false;
                FieldManager.Instance.field.Remove(gameObject.id);
            }
        }


        private void moveField()
        {
            RowInfo rowInfo = moveRow();

            if (rowInfo.needMove)
            {
                foreach (KeyValuePair<int, IGameObject> gameObj in FieldManager.Instance.field)
                {
                    gameObj.Value.animation.playAnimation();
                }
            }
        }

        private void OnMouseEnter()
        {
            _isEntered = true;
            GameLoader.OnUpdate += onMouseMove;
        }

        private void OnMouseExit()
        {
            _isEntered = false;
            GameLoader.OnUpdate -= onMouseMove;
        }

        private void onMouseMove(float deltaTime)
        {
            if (Math.Abs(Input.mousePosition.x - _prevMousePos.x) > 0.1f || Math.Abs(Input.mousePosition.y - _prevMousePos.y) > 0.1f)
            {
                _prevMousePos = Input.mousePosition;
                processMouseCoords();
            }
        }

        public const int LastRow = 5;

        public RowInfo moveRow()
        {
            int maxRow = FieldManager.Instance.findCurrentMaxHeight();
            int minRow = FieldManager.Instance.findCurrentMinHeight();
            int dY = minRow - LastRow;

            if (dY <= 0 || maxRow == FieldManager.LAST_VISIBLE_ROW)
            {
                return new RowInfo() {needMove = false};
            }

            int remainder = maxRow - FieldManager.LAST_VISIBLE_ROW;

            if (dY > remainder)
            {
                dY = remainder;
            }

            FieldManager.rowOffset = ((FieldManager.maxHeight - 1 - maxRow + dY) % 2 == 0) ? 1 : 0;

            foreach (KeyValuePair<int, IGameObject> keyValuePair in FieldManager.Instance.field)
            {
                IGameObject obj = keyValuePair.Value;

                if (obj.row == 0)
                {
                    continue;
                }

                if (obj.isRemoved)
                {
                    continue;
                }

                obj.row -= dY;
                Vector3 coords = FieldManager.findCoordsByRowCol(obj.row, obj.col) + new Vector3(0, 0, 0.5f);
                obj.animation.bubbleAnimation.Enqueue(coords);
            }


            return new RowInfo() {needMove = true, dY = dY, maxRow = maxRow};
        }

        public struct RowInfo
        {
            public bool needMove;
            public int dY;
            public int maxRow;
        }

        private bool processMouseCoords()
        {
            _lastMousePos = Scene.Instance.view.transform.InverseTransformPoint(UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Vector3 mousePos = _lastMousePos;
            Vector2 gridPos = FieldManager.Instance.getGridHexPosition(_lastMousePos);
            _lastMousePos.x = gridPos[0];
            _lastMousePos.y = gridPos[1];

            if (_lastMousePos.x < 0 || _lastMousePos.x >= FieldManager.FIELD_WIDTH || _lastMousePos.y < 0 || _lastMousePos.y >= FieldManager.LAST_VISIBLE_ROW)
            {
                return false;
            }

            float angleRad = Mathf.Atan2(mousePos.y - FieldManager.Instance.playerArrow.transform.localPosition.y,
                mousePos.x - FieldManager.Instance.playerArrow.transform.localPosition.x);
            float angle = Mathf.Rad2Deg * angleRad;
            if (angle < 170 && angle > 10)
            {
                arrowAngle = angleRad;
                FieldManager.Instance.playerArrow.transform.rotation = Quaternion.Euler(0, 0, angle);
                
                if (!SITween.hasAnyTweening())
                {
                    FieldManager.Instance.currentPlayerBall.rotate(new Vector3(0, 0, angle - 90));
                }
            }

            return true;
        }
    }
}