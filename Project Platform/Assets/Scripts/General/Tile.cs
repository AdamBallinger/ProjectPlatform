
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

        private Action<Tile> typeChangeCallback;

        public TileType Type
        {
            get { return type; }
            set
            {
                OldType = type;
                type = value;
                if (typeChangeCallback != null && OldType != type)
                    typeChangeCallback(this);
            }
        }

        public TileType OldType { get; private set; }

        public Tile(int _x, int _y)
        {
            X = _x;
            Y = _y;

            OldType = TileType.Empty;
        }

        public void RegisterTileTypeChangeCallback(Action<Tile> _callback)
        {
            typeChangeCallback += _callback;
        }

        public static TileType GetTypeFromString(string _type)
        {
            switch(_type)
            {
                case "Empty":
                    return TileType.Empty;
                case "Platform":
                    return TileType.Platform;
                case "PhysicsZone":
                    return TileType.PhysicsZone;
                case "Wall":
                    return TileType.Wall;

                default:
                    return TileType.Empty;
            }
        }
    }
}
