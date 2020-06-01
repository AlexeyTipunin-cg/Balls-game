using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code
{
    public class SimpleBallController
    {
        public static readonly float HID_DIST = FieldManager.CELL_WIDTH * 0.8f;
        private static float speed = 10f;

        public static List<(IGameObject obj, float percents) > OnPlayerShot(IGameObject playerBall, float arrowAngle)
        {
            bool isPop = findPath(arrowAngle, playerBall, playerBall.view.localPosition.x, playerBall.view.localPosition.y,
                FieldManager.Instance.findCurrentMaxHeight(), 1);

            Queue<Vector3> playerAnimation = playerBall.animation.bubbleAnimation;

            Vector2 gridPosition = FieldManager.Instance.getGridHexPosition(playerAnimation.Last());

            playerBall.col = (int) gridPosition.x;
            playerBall.row = (int) gridPosition.y;

            FieldManager.Instance.field.Add(playerBall.id, playerBall);

            List<(IGameObject obj, float percents)> bubblesToDelete = new List<(IGameObject obj, float percents)>();

            if (!isPop)
            {
                int currentHeight = FieldManager.Instance.findCurrentMaxHeight();

                List<(IGameObject obj, float percents)> cluster = null;

                cluster = GameManager.Instance.findCluster((int) gridPosition.y, (int) gridPosition.x, playerBall.needMatchType, true).Select(x => (x, 1f)).ToList();


                if (cluster.Count >= 3)
                {
                    foreach ((IGameObject obj, float percents) info in cluster)
                    {
                        info.obj.isRemoved = true;
                        GameManager.Instance.isChecked.Remove(info.obj.id);
                        bubblesToDelete.Add(info);
                    }

                    List<(IGameObject obj, float percents)> floatingCluster = GameManager.Instance.findFloatingClusters(currentHeight);
                    foreach ((IGameObject obj, float percents) info  in floatingCluster)
                    {
                        info.obj.isRemoved = true;
                        GameManager.Instance.isChecked.Remove(info.obj.id);
                        bubblesToDelete.Add(info);
                    }

                }
            }


            Vector3 playerEndAnimation = FieldManager.Instance.findCoordsByRowCol((int) gridPosition.y, (int) gridPosition.x);
            playerAnimation.Enqueue(playerEndAnimation);

            return bubblesToDelete;
        }


        private static bool findPath(float angle, IGameObject playerBall, float x, float y, int maxHeight, int direction)
        {
            int colNum = FieldManager.FIELD_WIDTH;
            float pathTime = 0;
            float upperBorderY = Scene.Instance.sizeDeltaY - 0.01f;
            float downBorderY = 0;
            float leftBorderX = FieldManager.CELL_WIDTH * 0.5f - 0.01f;
            float rightBorderX = Scene.Instance.sizeDeltaX - FieldManager.CELL_WIDTH * 0.5f - 0.01f;
            IGameObject lowObj;

            float lowPointY;
            lowPointY = direction == 1 ? float.MaxValue : float.MinValue;
            float lowPointX = 0;

            IGameObject hitObject;

            float tempX;
            float tempY;

            float playerLineCoef = y - (float) Math.Tan(angle) * x;

            foreach (KeyValuePair<int, IGameObject> idObjPair in FieldManager.Instance.field)
            {
                IGameObject obj = idObjPair.Value;
                int objId = idObjPair.Key;

                if (objId == playerBall.id || obj.row == 0)
                {
                    continue;
                }

                Vector3 viewCoord = FieldManager.Instance.findCoordsByRowCol(obj.row, obj.col);
                if ((viewCoord.y - y) * direction < 0)
                {
                    continue;
                }

                float normalLineCoef = viewCoord.y + 1 / Mathf.Tan(angle) * viewCoord.x;
                Vector3 nPoint;
                nPoint.z = 0;
                nPoint.x = (normalLineCoef - playerLineCoef) / (Mathf.Tan(angle) + 1 / Mathf.Tan(angle));

                nPoint.y = Mathf.Tan(angle) * nPoint.x + playerLineCoef;

                if (angle == Math.PI / 2)
                {
                    nPoint.x = viewCoord.x;
                    nPoint.y = viewCoord.y;
                }

                float n_dist = Vector3.Distance(viewCoord, nPoint);

                if (n_dist <= HID_DIST)
                {
                    float dist2 = Mathf.Sqrt(HID_DIST * HID_DIST - n_dist * n_dist);

                    Vector3 result1 = new Vector3();
                    Vector3 result2 = new Vector3();
                    if (angle != -Math.PI / 2)
                    {
                        result1.x = (float) ((dist2 / Math.Sqrt(1 + Math.Pow((viewCoord.x - nPoint.x) / (viewCoord.y - nPoint.y), 2))) + nPoint.x);
                        result1.y = Mathf.Tan(angle) * result1.x + playerLineCoef;

                        result2.x = (float) ((-dist2 / Math.Sqrt(1 + Math.Pow((viewCoord.x - nPoint.x) / (viewCoord.y - nPoint.y), 2))) + nPoint.x);
                        result2.y = Mathf.Tan(angle) * result2.x + playerLineCoef;
                        if ((result1.y - result2.y) * direction < 0)
                        {
                            tempX = result1.x;
                            tempY = result1.y;
                        }
                        else
                        {
                            tempX = result2.x;
                            tempY = result2.y;
                        }

                        if ((tempY - lowPointY) * direction < 0)
                        {
                            lowPointX = tempX;
                            lowPointY = tempY;
                            hitObject = obj;
                            lowObj = obj;
                        }
                    }
                    else
                    {
                        result1.y = dist2 + nPoint.y;
                        if ((nPoint.y - lowPointY) * direction < 0)
                        {
                            lowPointX = FieldManager.Instance.playerStartPoint.x;
                            lowPointY = result1.y;
                            hitObject = obj;
                            lowObj = obj;
                        }
                    }
                }
            }

            if (lowPointY == float.MaxValue || lowPointY == float.MinValue)
            {
                if (direction == -1 && (downBorderY - playerLineCoef) / Mathf.Tan(angle) > leftBorderX && (downBorderY - playerLineCoef) / Mathf.Tan(angle) < rightBorderX)
                {
                    lowPointY = downBorderY;
                    lowPointX = (lowPointY - playerLineCoef) / Mathf.Tan(angle);
                    pathTime = Mathf.Abs((lowPointY - y) / (speed * Mathf.Sin(angle)));
                    playerBall.animation.bubbleAnimation.Enqueue(new Vector3(lowPointX, lowPointY, pathTime));
                    return true;
                }
                else if (direction == 1 && (upperBorderY - playerLineCoef) / Mathf.Tan(angle) > leftBorderX && (upperBorderY - playerLineCoef) / Mathf.Tan(angle) < rightBorderX)
                {
                    if (maxHeight == FieldManager.LAST_VISIBLE_ROW)
                    {
                        lowPointY = upperBorderY;
                        lowPointX = (lowPointY - playerLineCoef) / Mathf.Tan(angle);
                        pathTime = Mathf.Abs((lowPointY - y) / (speed * Mathf.Sin(angle)));
                        playerBall.animation.bubbleAnimation.Enqueue(new Vector3(lowPointX, lowPointY, pathTime));
                    }
                    else
                    {
                        direction = -1;
                        lowPointY = upperBorderY;
                        lowPointX = (lowPointY - playerLineCoef) / Mathf.Tan(angle);
                        pathTime = Mathf.Abs((lowPointY - y) / (speed * Mathf.Sin(angle)));
                        playerBall.animation.bubbleAnimation.Enqueue(new Vector3(lowPointX, lowPointY, pathTime));

                        return findPath(Mathf.PI - angle, playerBall, lowPointX, lowPointY, maxHeight, direction);
                    }
                }
                else if ((angle - Mathf.PI * 0.5f) * direction > 0)
                {
                    pathTime = Mathf.Abs((leftBorderX - x) / (speed * Mathf.Cos(angle)));
                    playerBall.animation.bubbleAnimation.Enqueue(new Vector3(leftBorderX, Mathf.Tan(angle) * (leftBorderX) + playerLineCoef, pathTime));
                    return findPath(Mathf.PI - angle, playerBall, leftBorderX, Mathf.Tan(angle) * (leftBorderX) + playerLineCoef, maxHeight, direction);
                }
                else if ((angle - Mathf.PI * 0.5f) * direction < 0)
                {
                    pathTime = Mathf.Abs((rightBorderX - x) / (speed * Mathf.Cos(angle)));
                    playerBall.animation.bubbleAnimation.Enqueue(new Vector3(rightBorderX, Mathf.Tan(angle) * (rightBorderX) + playerLineCoef, pathTime));
                    return findPath(Mathf.PI - angle, playerBall, rightBorderX, Mathf.Tan(angle) * (rightBorderX) + playerLineCoef, maxHeight, direction);
                }
                else if (angle == Mathf.PI / 2)
                {
                    pathTime = Mathf.Abs((y - lowPointY) / (speed * Mathf.Sin(angle)));
                    playerBall.animation.bubbleAnimation.Enqueue(new Vector3(FieldManager.Instance.playerStartPoint.x, upperBorderY, pathTime));
                    return findPath(Mathf.PI - angle, playerBall, FieldManager.Instance.playerStartPoint.x, upperBorderY, maxHeight, direction);
                }
            }
            else if (lowPointX < leftBorderX)
            {
                pathTime = Mathf.Abs((leftBorderX - x) / (speed * Mathf.Cos(angle)));
                playerBall.animation.bubbleAnimation.Enqueue(new Vector3(leftBorderX, Mathf.Tan(angle) * (leftBorderX) + playerLineCoef, pathTime));
                return findPath(Mathf.PI - angle, playerBall, leftBorderX, Mathf.Tan(angle) * (leftBorderX) + playerLineCoef, maxHeight, direction);
            }
            else if (lowPointX > rightBorderX)

            {
                pathTime = Mathf.Abs((rightBorderX - x) / (speed * Mathf.Cos(angle)));
                playerBall.animation.bubbleAnimation.Enqueue(new Vector3(rightBorderX, Mathf.Tan(angle) * (rightBorderX) + playerLineCoef, pathTime));
                return findPath(Mathf.PI - angle, playerBall, rightBorderX, Mathf.Tan(angle) * (rightBorderX) + playerLineCoef, maxHeight, direction);
            }
            else if (angle == Mathf.PI / 2)
            {
                pathTime = Mathf.Abs((y - lowPointY) / (speed * Mathf.Sin(angle)));
                playerBall.animation.bubbleAnimation.Enqueue(new Vector3(FieldManager.Instance.playerStartPoint.x, lowPointY, pathTime));
            }
            else
            {
                pathTime = Mathf.Abs((x - lowPointX) / (speed * Mathf.Cos(angle)));
                playerBall.animation.bubbleAnimation.Enqueue(new Vector3(lowPointX, lowPointY, pathTime));
            }

            return false;
        }
    }
}