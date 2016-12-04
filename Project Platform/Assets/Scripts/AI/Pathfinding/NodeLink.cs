
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
        public const float WALK_COST = 4.0f;
        public const float FALL_COST = 4.0f;
        public const float JUMP_COST = 6.0f;

        public PathNode ParentNode { get; private set; }

        public PathNode DestinationNode { get; private set; }

        public NodeLinkType LinkType { get; private set; }

        private object LinkData { get; set; }

        public float LinkCost { get; private set; }


        public NodeLink(PathNode _destination, NodeLinkType _type)
        {
            DestinationNode = _destination;
            LinkType = _type;

            switch(_type)
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

        /// <summary>
        /// Sets the data for this link.
        /// </summary>
        /// <param name="_data"></param>
        public void SetData(object _data)
        {
            LinkData = _data;
        }

        /// <summary>
        /// Gets the data for this link as given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>()
        {
            return (T)LinkData;
        }
    }
}
