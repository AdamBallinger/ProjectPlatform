
namespace Assets.Scripts.AI.AStar
{

    public enum NodeLinkType
    {
        Walk,
        Fall,
        Jump
    }

    public class NodeLink
    {

        public PathNode DestinationNode { get; private set; }

        public NodeLinkType LinkType { get; private set; }

        public float LinkScore { get; set; }


        public NodeLink(PathNode _destination, NodeLinkType _type)
        {
            DestinationNode = _destination;
            LinkType = _type;
        }
    }
}
