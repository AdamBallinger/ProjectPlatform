
using System;

namespace Assets.Scripts.General
{
    public enum TileType
    {
        Platform,       // Should Allow Pathing
        PhysicsZone,    // Manipulate Rigid bodies in the zone E.g. Wind zone
        Empty,          // Nothing I.e and empty tile in the world (Air)
        Wall            // Blocks bodies and pathing
    }

    public class Tile
    {

        public int X { get; protected set; }

        public int Y { get; protected set; }

        private TileType type = TileType.Empty;
        private TileType previousType;

        private Action<Tile> typeChangeCallback;

        public TileType Type
        {
            get { return type; }
            set
            {
                OldType = type;
                type = value;
                if (typeChangeCallback != null && previousType != type)
                    typeChangeCallback(this);
            }
        }

        public TileType OldType
        {
            get { return previousType; }
            private set { previousType = value; }
        }

        public Tile(int _x, int _y)
        {
            X = _x;
            Y = _y;
        }

        public void RegisterTileTypeChangeCallback(Action<Tile> _callback)
        {
            typeChangeCallback += _callback;
        }
    }
}
