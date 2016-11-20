﻿
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

        public PathNode ParentNode { get; private set; }

        public PathNode DestinationNode { get; private set; }

        public NodeLinkType LinkType { get; private set; }

        public float LinkScore { get; set; }


        public NodeLink(PathNode _destination, NodeLinkType _type)
        {
            DestinationNode = _destination;
            LinkType = _type;
        }

        public void SetParentNode(PathNode _parent)
        {
            ParentNode = _parent;
        }
    }
}
