
namespace Assets.Scripts.AI.AStar
{
    public class Node
    {

        public float GCost { get; private set; }
        public float HCost { get; private set; }

        public float FCost
        {
            get { return GCost + HCost; }
        }

        /// <summary>
        /// World Grid X position.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// World Grid Y position.
        /// </summary>
        public int Y { get; private set; }


        /// <summary>
        /// Create a new node at given X and Y position in the world grid.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        public Node(int _x, int _y)
        {
            X = _x;
            Y = _y;

            // TODO: add to some kind of pathfinder grid 
            // TODO: create pathfinder class to contain node grid etc.
            // Node grid will be built when level is "started". Search through all platform tiles, and if the tile above it is empty, place a node there.
            // For jumps there may need to be some kind of checks at the search phase to see if the node is adjacent etc.
        }
    }
}
