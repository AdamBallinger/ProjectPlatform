
using Assets.Scripts.General;

namespace Assets.Scripts.AI.AStar
{
    public class Pathfinder
    {

        public static Pathfinder Current { get; private set; }

        public Node[,] NodeGraph { get; private set; }


        public static void BuildNodeGraph(int _width, int _height)
        {
            if(Current == null)
            {
                Current = new Pathfinder();
            }

            if(Current.NodeGraph == null)
            {
                Current.NodeGraph = new Node[_width, _height];
            }

            Current.Clear();

            for(var x = 0; x < _width; x++)
            {
                for(var y = 0; y < _height; y++)
                {
                    if(World.Current.GetTileAt(x, y).Type == TileType.Platform)
                    {
                        var tileAbove = World.Current.GetTileAt(x, y + 1);
                        if (tileAbove != null && tileAbove.Type == TileType.Empty)
                        {
                            Current.NodeGraph[x, y + 1] = new Node(x, y + 1);
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            for (var x = 0; x < World.Current.Width; x++)
            {
                for (var y = 0; y < World.Current.Height; y++)
                {
                    Current.NodeGraph[x, y] = null;
                }
            }
        }
    }
}
