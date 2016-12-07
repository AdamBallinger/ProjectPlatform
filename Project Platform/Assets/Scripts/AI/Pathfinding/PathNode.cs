using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.AI.Pathfinding
{

    public enum PathNodeType
    {
        None,
        Platform,
        DropTo,
        JumpFrom,
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

        public PathNode Parent { get; set; }

        private object NodeData { get; set; }

        public float G { get; set; }

        public float H { get; set; }

        public float F
        {
            get { return G + H; }
        }


        public PathNode(int _x, int _y, PathNodeType _type)
        {
            X = _x;
            Y = _y;
            NodeType = _type;
            NodeLinks = new List<NodeLink>();
            NodeData = 0.0f; // default data to 0
            H = 0.0f;
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
        /// Sets the data for this node.
        /// </summary>
        /// <param name="_data"></param>
        public void SetData(object _data)
        {
            NodeData = _data;
        }

        /// <summary>
        /// Gets the data for this node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>()
        {
            return (T)NodeData;
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
