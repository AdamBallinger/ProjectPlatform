
namespace Assets.Scripts.AI.Pathfinding
{

    public enum NodeLinkType
    {
        Walk,
        Fall,
        Jump
    }

    public class NodeLink
    {
        // G cost constants for different link types.
        public const float WALK_COST = 1.0f;
        public const float FALL_COST = 1.5f;
        public const float JUMP_COST = 5.0f;

        public PathNode ParentNode { get; private set; }

        public PathNode DestinationNode { get; private set; }

        public NodeLinkType LinkType { get; private set; }

        public float LinkCost { get; private set; }


        public NodeLink(PathNode _destination, NodeLinkType _type)
        {
            DestinationNode = _destination;
            LinkType = _type;

            switch (_type)
            {
                case NodeLinkType.Walk:
                    LinkCost = WALK_COST;
                    break;

                case NodeLinkType.Fall:
                    LinkCost = FALL_COST;
                    break;

                case NodeLinkType.Jump:
                    LinkCost = JUMP_COST;
                    break;
            }
        }

        public void SetParentNode(PathNode _parent)
        {
            ParentNode = _parent;
        }
    }
}
