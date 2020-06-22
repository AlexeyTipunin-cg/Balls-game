using System.Collections.Generic;

namespace Code
{
    public class ClusterFinderLogic
    {
        public static readonly ClusterFinderLogic Instance = new ClusterFinderLogic();

        public Dictionary<int, bool> isChecked = new Dictionary<int, bool>();

        private readonly int[,] ODD_NEIGHBOURS = {{0, 1}, {1, 1}, {1, 0}, {0, -1}, {-1, 0}, {-1, 1}};
        private readonly int[,] EVEN_NEIGHBOURS = {{0, 1}, {1, 0}, {1, -1}, {0, -1}, {-1, -1}, {-1, 0}};

        public List<(IGameObject obj, float percents)> findFloatingClusters(int currentHeight)
        {
            removeCheckedFlag();

            List<(IGameObject obj, float percents)> foundClusters = new List<(IGameObject obj, float percents)>();
            foreach (KeyValuePair<int, IGameObject> idToObj in FieldManager.Instance.field)
            {
                IGameObject gameObj = idToObj.Value;

                if (gameObj.isRemoved)
                {
                    continue;
                }

                if (!isChecked[gameObj.id])
                {
                    List<IGameObject> foundCluster = findCluster(gameObj.row, gameObj.col, false, false);

                    if (foundCluster.Count <= 0)
                    {
                        continue;
                    }

                    bool floating = true;

                    foreach (IGameObject o in foundCluster)
                    {
                        if (o.row == currentHeight)
                        {
                            floating = false;
                            break;
                        }
                    }

                    if (floating)
                    {
                        for (int i = 0; i < foundCluster.Count; i++)
                        {
                            foundClusters.Add((foundCluster[i], 1));
                        }
                    }
                }
            }

            return foundClusters;
        }

        public List<IGameObject> findCluster(int row, int col, bool matchType, bool reset)
        {
            if (reset)
            {
                removeCheckedFlag();
            }

            int[,] fieldMatrix = getFieldMatrix();
            int targetObjID = fieldMatrix[col, row];

            if (targetObjID == -1)
            {
                return new List<IGameObject>();
            }

            IGameObject targetObj = FieldManager.Instance.field[targetObjID];

            Stack<int> processed = new Stack<int>();
            processed.Push(targetObjID);
            isChecked[targetObjID] = true;

            List<IGameObject> foundCluster = new List<IGameObject>();

            while (processed.Count > 0)
            {
                int currentObjID = processed.Pop();
                IGameObject currentObj = FieldManager.Instance.field[currentObjID];

                if (currentObjID == -1)
                {
                    continue;
                }

                if (!matchType || currentObj.type == targetObj.type)
                {
                    foundCluster.Add(currentObj);

                    List<int> neighboursIDs = getNeighbours(currentObj.col, currentObj.row, fieldMatrix);

                    foreach (int id in neighboursIDs)
                    {
                        if (id != -1)
                        {
                            if (!isChecked[id])
                            {
                                processed.Push(id);
                                isChecked[id] = true;
                            }
                        }
                    }
                }
            }

            return foundCluster;
        }

        private List<int> getNeighbours(int col, int row, int[,] fieldMatrix)
        {
            int[,] currentOffset;
            List<int> neighbors = new List<int>();

            if ((row + FieldManager.rowOffset) % 2 == 0)
            {
                currentOffset = EVEN_NEIGHBOURS;
            }
            else
            {
                currentOffset = ODD_NEIGHBOURS;
            }

            for (int i = 0; i < 6; i++)
            {
                int currentX = col + currentOffset[i, 1];
                int currentY = row + currentOffset[i, 0];

                if (currentX >= 0 && currentX < FieldManager.FIELD_WIDTH && currentY >= 0 && currentY < FieldManager.maxHeight)
                {
                    if (col % FieldManager.FIELD_WIDTH == 0)
                    {
                        if ((row + FieldManager.rowOffset) % 2 == 0)
                        {
                            if (i == 2 || i == 3 || i == 4)
                            {
                                continue;
                            }

                            neighbors.Add(fieldMatrix[currentX, currentY]);
                        }
                        else
                        {
                            if (i == 3)
                            {
                                continue;
                            }

                            neighbors.Add(fieldMatrix[currentX, currentY]);
                        }
                    }
                    else if ((col + 1) % FieldManager.FIELD_WIDTH == 0)
                    {
                        if ((row + FieldManager.rowOffset) % 2 == 0)
                        {
                            if (i == 0)
                            {
                                continue;
                            }

                            neighbors.Add(fieldMatrix[currentX, currentY]);
                        }
                        else
                        {
                            if (i == 0 || i == 1 || i == 5)
                            {
                                continue;
                            }

                            neighbors.Add(fieldMatrix[currentX, currentY]);
                        }
                    }
                    else neighbors.Add(fieldMatrix[currentX, currentY]);
                }
            }

            return neighbors;
        }

        public int[,] getFieldMatrix()
        {
            int[,] matrix = new int[FieldManager.FIELD_WIDTH, FieldManager.maxHeight];

            for (int col = 0; col < FieldManager.FIELD_WIDTH; col++)
            {
                for (int row = 0; row < FieldManager.maxHeight; row++)
                {
                    matrix[col, row] = -1;
                }
            }

            foreach (KeyValuePair<int, IGameObject> pair in FieldManager.Instance.field)
            {
                if (pair.Value.isRemoved)
                {
                    continue;
                }

                matrix[pair.Value.col, pair.Value.row] = pair.Key;
            }

            return matrix;
        }

        private void removeCheckedFlag()
        {
            foreach (KeyValuePair<int, IGameObject> pair in FieldManager.Instance.field)
            {
                isChecked[pair.Key] = false;
            }
        }
    }
}