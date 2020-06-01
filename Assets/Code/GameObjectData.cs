using System.Security.Cryptography;

namespace Code
{
    public struct GameObjectData
    {
        public int row;
        public int col;
        public int type;
        public GameObjectData(int row, int col, int type)
        {
            this.row = row;
            this.col = col;
            this.type = type;
        }
    }
}