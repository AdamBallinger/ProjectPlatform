
namespace Assets.Scripts.AI.Pathfinding
{
    public class PathfindingSettings
    {

        /// <summary>
        /// What heuristic to use for pathfinding.
        /// </summary>
        public static Heuristic HeuristicFunction { get; set; }

        /// <summary>
        /// How many tiles across on the X axis can a fall and jump link generate.
        /// </summary>
        public static int FallJumpLinkMaxDist { get; set; }

        /// <summary>
        /// Initialise settings to default values.
        /// </summary>
        public static void Init()
        {
            HeuristicFunction = Heuristic.Manhattan;
            FallJumpLinkMaxDist = 4;
        }

        public static void SetHeuristicFromID(int _id)
        {
            switch(_id)
            {
                case 0:
                    HeuristicFunction = Heuristic.Manhattan;
                    break;

                case 1:
                    HeuristicFunction = Heuristic.Euclidean;
                    break;

                default:
                    HeuristicFunction = Heuristic.Manhattan;
                    break;
            }
        }
    }
}
