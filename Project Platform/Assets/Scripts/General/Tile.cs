using System;
using System.Diagnostics;

namespace Assets.Scripts.General
{
    public enum TileType
    {
        Platform,       // Should Allow Pathing
        PhysicsZone,    // Manipulate Rigid bodies in the zone E.g. Wind zone
        Empty,          // Nothing I.e and empty tile in the world (Air)
        Wall            // Blocks bodies and pathing
    }

    [Flags]
    public enum AdjacentFlag
    {
        None = 1,
        Left = 1 << 1,
        Right = 1 << 2,
        Up = 1 << 3,
        Down = 1 << 4,

        All = Left | Right | Up | Down,
        LeftRight = Left | Right,
        UpDown = Up | Down,
        UpLeft = Up | Left,
        UpRight = Up | Right,
        DownLeft = Down | Left,
        DownRight = Down | Right,
        UpLeftDown = UpLeft | Down,
        UpRightDown = UpRight | Down,
        DownLeftRight = Down | LeftRight,
        UpLeftRight = Up | LeftRight

    }

    public class Tile
    {

        public int X { get; protected set; }

        public int Y { get; protected set; }

        public AdjacentFlag Adjacent { get; set; }

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

            Adjacent = AdjacentFlag.None;
            OldType = TileType.Empty;
        }

        /// <summary>
        /// Register a callback function to call when the type for this tile has changed.
        /// </summary>
        /// <param name="_callback"></param>
        public void RegisterTileTypeChangeCallback(Action<Tile> _callback)
        {
            typeChangeCallback += _callback;
        }

        /// <summary>
        /// Returns a tile type enum from a given string.
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if the tile has the given adjacent flags.
        /// </summary>
        /// <param name="_flag"></param>
        /// <returns></returns>
        public bool HasAdjacentFlags(AdjacentFlag _flag)
        {
            return (Adjacent & _flag) == _flag;
        }
    }
}
