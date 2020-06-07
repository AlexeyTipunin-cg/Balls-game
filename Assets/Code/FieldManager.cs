using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Code
{
    public class FieldManager
    {
        public static readonly FieldManager Instance = new FieldManager();

        public const int FIELD_WIDTH = 8;
        public static int LAST_VISIBLE_ROW;
        public static int MOVING_BORDER_ROW = 4;

        private int _fieldHeight = 20;

        public static float CELL_WIDTH = 0.6f;
        public static float CELL_HEIGHT;
        private readonly float SQRT_3 = (float) Math.Sqrt(3);

        private GameObject _scene;
        public IGameObject currentPlayerBall;

        public static int rowOffset;

        public static float wallsWidth;
        public static float screenWidth;

        public int worldHeight;


        public List<GameObjectData> tempField = new List<GameObjectData>();


        public Dictionary<int, IGameObject> field = new Dictionary<int, IGameObject>();
        public List<IShape> player = new List<IShape>();

        private List<List<int>> _field = new List<List<int>>();

        public static int maxHeight;

        public static GameObject gamePivot;
        
        private void populateField()
        {
//            if (tempField.Count == 0)
//            {
//                return;
//            }
            
            Random rand = new Random();
            var ballsNum = 200;
            var startRow = 5;

            for (int i = 0; i < ballsNum; i++)
            {
                BallsTypes type = (BallsTypes)rand.Next(1, 4);

                int row = startRow + i / FIELD_WIDTH;
                int col = i + startRow * FIELD_WIDTH - FIELD_WIDTH * row;

                tempField.Add(new GameObjectData(row, col, type));
            }


            maxHeight = tempField.Max(data => data.row) + 1;
            FieldManager.rowOffset = ((maxHeight - 1) % 2 == 0) ? 0 : 1;
        }

        public void clearField()
        {
            foreach (KeyValuePair<int,IGameObject> idToObj in field)
            {
                idToObj.Value.setActive = false;
            }

            currentPlayerBall.setActive = false;
            currentPlayerBall = null;
            field.Clear();
            tempField.Clear();
        }
        
        public List<GameObject> lines =new List<GameObject>();

        public void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 100f)
        {
            GameObject myLine = new GameObject();
            myLine.transform.SetParent(gamePivot.transform);
            myLine.transform.localPosition = start;
            myLine.AddComponent<LineRenderer>();
            lines.Add(myLine);
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            lr.SetColors(color, color);
            lr.SetWidth(0.05f, 0.05f);
            lr.SetPosition(0, start - new Vector3(Scene.Instance.sizeDeltaX/2, Scene.Instance.sizeDeltaY/2));
            lr.SetPosition(1, end - new Vector3(Scene.Instance.sizeDeltaX/2, Scene.Instance.sizeDeltaY/2));
        }



        public void CreateField()
        {
            populateField();

            CELL_WIDTH = (screenWidth - wallsWidth * 2) / (FIELD_WIDTH + 0.5f);
            CELL_HEIGHT = 1.5f * CELL_WIDTH / SQRT_3;
            LAST_VISIBLE_ROW = (int) ((Camera.orthographicSize * 2 - wallsWidth - CELL_HEIGHT) / CELL_HEIGHT);

            Scene.Instance.position = new Vector3(0.5f * (CELL_WIDTH * FIELD_WIDTH - 0.5f * CELL_WIDTH), 0);
            Scene.Instance.size = new Vector2(CELL_WIDTH * FIELD_WIDTH + 0.5f * CELL_WIDTH, CELL_HEIGHT * LAST_VISIBLE_ROW + 0.5f * CELL_HEIGHT);
            gamePivot = new GameObject("ScenePivot");
            gamePivot.transform.SetParent(Scene.Instance.view.transform);
            gamePivot.transform.localPosition = new Vector3();

            foreach (GameObjectData param in tempField)
            {
                if (param.type == 0)
                {
                    continue;
                }
 
                createGameObject(param.row, param.col, param.type);
            }

            createPlayerBubble();

            Scene.Instance.pivot = new Vector3();
            Scene.Instance.position = new Vector3(-screenWidth * 0.5f + wallsWidth, Camera.orthographicSize - wallsWidth - Scene.Instance.sizeDeltaY);

            if (playerArrow == null)
            {
                playerArrow = new GameObject("ArrowPoint");
                playerArrow.transform.SetParent(Scene.Instance.view.transform);
                playerArrow.AddComponent<RectTransform>().sizeDelta = new Vector2(CELL_WIDTH,CELL_WIDTH);
                playerArrow.transform.localPosition = currentPlayerBall.view.localPosition;
            
                IShape _playerArrow = new Rectangle(CELL_WIDTH *2 /20, CELL_WIDTH * 2, new Vector3(0,0,0),  Color.blue, playerArrow);
                _playerArrow.draw();
                _playerArrow.localPosition = new Vector3(CELL_WIDTH,0,0);
                playerArrow.transform.rotation = Quaternion.Euler(0,0,90);
            }

        }

        public void createGameObject(int row, int col, BallsTypes type)
        {

            IGameObject gameObject = GameObjectsPool.getOrCreateFromPool( type, row, col);
            field.Add(gameObject.id, gameObject);

            if (gameObject.row > LAST_VISIBLE_ROW)
            {
                gameObject.view.setActive = false;
            }
        }


        public static Vector3 findCoordsByRowCol(int row, int col)
        {
            Vector3 pos;
            pos.z = 0;
            pos.x = col * CELL_WIDTH + CELL_WIDTH * 0.5f;
            if ((row + rowOffset) % 2 != 0)
            {
                pos.x = pos.x + CELL_WIDTH * 0.5f;
            }

            pos.y = row * CELL_HEIGHT + CELL_HEIGHT * 0.5f;

            return pos;
        }
       

        public static Vector3 findCoordsForPlayer()
        {
            Vector3 pos;
            pos.z = 0;

            pos.x = Scene.Instance.sizeDeltaX * 0.5f;
            pos.y = CELL_WIDTH * 0.5f;

            return pos;
        }

        public Vector3 playerStartPoint;


        public void createPlayerBubble()
        {
            Random rand = new Random();
            BallsTypes type = (BallsTypes) rand.Next(1, 5);

            IGameObject gameObject = GameObjectsPool.getOrCreateFromPool(type, isPlayer: true);

            currentPlayerBall = gameObject;
        }

        private Color getColor(BallsTypes type)
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

        public float leftBorderX;
        public float rightBorderX;
        public float upperBorderY;



        public GameObject playerArrow;



        public void createWalls()
        {
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(-screenWidth * 0.5f, wallsWidth * 0.5f), new Vector3(-screenWidth * 0.5f, -wallsWidth * 0.5f), new Vector3(screenWidth * 0.5f, -wallsWidth * 0.5f),
                new Vector3(screenWidth * 0.5f, wallsWidth * 0.5f)
            };
            IShape upperBorder = new Rectangle(new Vector3(0, UnityEngine.Camera.main.orthographicSize - 0.5f * wallsWidth), vertices, Color.black);
            upperBorder.draw();

            vertices = new Vector3[4]
            {
                new Vector3(-wallsWidth * 0.5f, -UnityEngine.Camera.main.orthographicSize), new Vector3(-wallsWidth * 0.5f, UnityEngine.Camera.main.orthographicSize - wallsWidth),
                new Vector3(wallsWidth * 0.5f, UnityEngine.Camera.main.orthographicSize - wallsWidth),
                new Vector3(wallsWidth * 0.5f, -UnityEngine.Camera.main.orthographicSize)
            };
            IShape leftBorder = new Rectangle(new Vector3(-screenWidth * 0.5f + wallsWidth * 0.5f, 0), vertices, Color.black);
            leftBorder.draw();

            vertices = new Vector3[4]
            {
                new Vector3(-wallsWidth * 0.5f, -UnityEngine.Camera.main.orthographicSize), new Vector3(-wallsWidth * 0.5f, UnityEngine.Camera.main.orthographicSize - wallsWidth),
                new Vector3(wallsWidth * 0.5f, UnityEngine.Camera.main.orthographicSize - wallsWidth),
                new Vector3(wallsWidth * 0.5f, -UnityEngine.Camera.main.orthographicSize)
            };
            IShape rightBorder = new Rectangle(new Vector3(screenWidth * 0.5f - wallsWidth * 0.5f, 0), vertices, Color.black);
            rightBorder.draw();
        }

        public Vector2 getGridHexPosition(Vector3 pos)
        {
            float x = pos.x;
            float y = pos.y;

            int row = (int) Mathf.Floor(y / CELL_HEIGHT);

             float offset = 0;
            if ((row + rowOffset) % 2 != 0 && row != 0)
            {
                offset = 0.5f * CELL_WIDTH;
            }

            int col = (int)((x - offset) / CELL_WIDTH);

            float relY = y - row * CELL_HEIGHT;
            float relX = x - col * CELL_WIDTH - offset;

            float k = Mathf.Tan((float) (30 * Math.PI / 180));
            float c = 0.5f * CELL_WIDTH / SQRT_3;

            if (relY < -k * relX + c)
            {
                row--;
                if (offset == 0)
                {
                    col--;
                }
            }
            else if (relY < k * relX - c)
            {
                row--;
                if (offset != 0)
                {
                    col++;
                }
            }

            return new Vector2(col, row);
        }
        
        public int findCurrentMaxHeight()
        {
            int maxY = -1;
            foreach (KeyValuePair<int,IGameObject> objPair in field)
            {
                IGameObject obj = objPair.Value;
                if (obj.isRemoved)
                {
                    continue;
                }

                if (obj.row > maxY)
                {
                    maxY = obj.row;
                }
            }

            return maxY;
        }
        
        public int findCurrentMinHeight()
        { 
            int minY = LAST_VISIBLE_ROW - 1;
            foreach (KeyValuePair<int,IGameObject> objPair in field)
            {
                IGameObject obj = objPair.Value;
                if (obj.isRemoved)
                {
                    continue;
                }

                if (obj.row == 0)
                {
                    continue;
                }

                if (obj.row < minY)
                {
                    minY = obj.row;
                }
            }

            return minY;
        }
    }

    public enum BallsTypes
    {
        Yellow = 1,
        Red = 2,
        Green = 3,
        Universal = 4
    }
}