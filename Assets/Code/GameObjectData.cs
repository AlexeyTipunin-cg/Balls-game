using System.Security.Cryptography;

namespace Code
{
    public struct GameObjectData
    {
        public int row;
        public int col;
        public BallsTypes type;
        public GameObjectData(int row, int col, BallsTypes type)
        {
            this.row = row;
            this.col = col;
            this.type = type;
        }
    }
}