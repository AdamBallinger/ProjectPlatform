using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.General
{
    public class World
    {

        public static World Current { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private Tile[,] Tiles { get; set; }

        public static void Create(int _width = 128, int _height = 128)
        {
            Current = new World();
            Current.Width = _width;
            Current.Height = _height;
            Current.Tiles = new Tile[Current.Width, Current.Height];

            Current.InitWorld();
        }

        private void InitWorld()
        {
            for(var x = 0; x < Current.Width; x++)
            {
                for(var y = 0; y < Current.Height; y++)
                {
                    Current.Tiles[x, y] = new Tile(x, y);
                }
            }
        }

        public int GetTileCount()
        {
            return Current.Width * Current.Height;
        }

        public Tile GetTileAt(int _x, int _y)
        {
            if(_x < 0 || _x > Current.Width || _y < 0 || _y > Current.Height)
            {
                return null;
            }

            return Tiles[_x, _y];
        }

        public Tile GetTileAtWorldCoord(Vector2 _coord)
        {
            var x = Mathf.FloorToInt(_coord.x + 0.5f);
            var y = Mathf.FloorToInt(_coord.y + 0.5f);
            return GetTileAt(x, y);
        }

        public void Clear()
        {
            for (var x = 0; x < Current.Width; x++)
            {
                for (var y = 0; y < Current.Height; y++)
                {
                    Current.Tiles[x, y].Type = TileType.Empty;
                }
            }
        }
    }
}
