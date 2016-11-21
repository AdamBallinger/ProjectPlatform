using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.AI.Pathfinding
{

    public enum PathNodeType
    {
        None,
        Platform,
        LeftEdge,
        RightEdge,
        Single
    }

    public class PathNode
    {

        public int X { get; private set; }

        public int Y { get; private set; }

        public PathNodeType NodeType { get; set; }

        public float PlatformID { get; set; }

        public List<NodeLink> NodeLinks { get; private set; }


        public PathNode(int _x, int _y, PathNodeType _type)
        {
            X = _x;
            Y = _y;
            NodeType = _type;
            NodeLinks = new List<NodeLink>();
        }

        public void ClearLinks()
        {
            NodeLinks.Clear();
        }

        public void AddLink(NodeLink _link)
        {
            _link.SetParentNode(this);
            NodeLinks.Add(_link);
        }

        /// <summary>
        /// Returns whether or not the node contains at least 1 of the given link type.
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public bool HasLinkOfType(NodeLinkType _type)
        {
            return NodeLinks.Any(link => link.LinkType == _type);
        }
    }
}
