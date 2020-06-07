using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Code
{
    public class GameObjectsPool
    {
        private static Dictionary<BallsTypes, Stack<IGameObject>> objects = new Dictionary<BallsTypes, Stack<IGameObject>>();

        public static IGameObject getOrCreateFromPool( BallsTypes type, int row = 0, int col = 0, bool isPlayer = false)
        {
            if (objects.ContainsKey(type))
            {
                if (objects[type].Count > 0)
                {
                    IGameObject obj = objects[type].Pop();
                    obj.setActive = true;
                    obj.setCoords(row, col, isPlayer);

                    return obj;
                }
            }

            return GameObjectsFactory.createGameObject(row, col, type, isPlayer);
        }

        public static void addToPool(IGameObject obj)
        {
            if (!objects.ContainsKey(obj.type))
            {
                objects[obj.type] = new Stack<IGameObject>();
            }

            obj.isRemoved = false;
            objects[obj.type].Push(obj);
        }
    }

    public static class GameObjectsFactory
    {
        public static IGameObject createGameObject(int row, int col, BallsTypes type, bool isPlayer = false)
        {
            IGameObject gameObject;

            switch (type)
            {
                case BallsTypes.Universal:
                    gameObject = new UniversalBubble( type);
                    break;
                default:
                    gameObject = new Bubble(type);
                    break;
            }

            Color32 color = getColor(type);

            Vector3 pos;
            if (isPlayer)
            {
                pos = FieldManager.findCoordsForPlayer();
            }
            else
            {
                pos = FieldManager.findCoordsByRowCol(row, col);

                gameObject.row = row;
                gameObject.col = col;
            }

            gameObject.createShape(FieldManager.CELL_WIDTH * 0.5f, pos.x, pos.y, color, FieldManager.gamePivot);
            gameObject.setActive = true;
            return gameObject;
        }

        public static void setCoords(this IGameObject obj, int row, int col, bool isPlayer = false)
        {
            Vector3 pos;
            if (isPlayer)
            {
                pos = FieldManager.findCoordsForPlayer();
            }
            else
            {
                pos = FieldManager.findCoordsByRowCol(row, col);

                obj.row = row;
                obj.col = col;
            }

            obj.view.localPosition = new Vector3(pos.x, pos.y, 0);
        }


        private static Color getColor(BallsTypes type)
        {
            switch (type)
            {
                case BallsTypes.Yellow:
                    return Color.yellow;
                case BallsTypes.Red:
                    return Color.red;
                case BallsTypes.Green:
                    return Color.green;
                case BallsTypes.Universal:
                    return Color.black;
                default:
                    return Color.black;
            }
        }
    }
}