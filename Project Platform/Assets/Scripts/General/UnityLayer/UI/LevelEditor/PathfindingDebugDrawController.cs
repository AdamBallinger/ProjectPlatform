using System.Collections.Generic;
using Assets.Scripts.AI.Pathfinding;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public enum DebugViewType
    {
        All,
        Nodes,
        WalkLinks,
        FallLinks,
        JumpLinks
    }

    public class PathfindingDebugDrawController : MonoBehaviour
    {

        public EditSubMenuPathfindingController pathfindingSubMenu;

        public Material debugMaterial;

        public Sprite nodeSprite;

        private List<GameObject> debugNodeObjects;
        private List<GameObject> debugNodeWalkLinkObjects;
        private List<GameObject> debugNodeFallLinkObjects;
        private List<GameObject> debugNodeJumpLinkObjects;

        public void OnDestroy()
        {
            ClearAll();
        }

        public void Start()
        {
            debugNodeObjects = new List<GameObject>();
            debugNodeWalkLinkObjects = new List<GameObject>();
            debugNodeFallLinkObjects = new List<GameObject>();
            debugNodeJumpLinkObjects = new List<GameObject>();
        }

        public void RebuildDebugObjects()
        {
            ClearAll();

            var nodes = World.Current.NavGraph.Nodes;

            for (var x = 0; x < World.Current.NavGraph.Width; x++)
            {
                for (var y = 0; y < World.Current.NavGraph.Height; y++)
                {
                    var node = nodes[x, y];

                    if (node.NodeType == PathNodeType.None) continue;

                    var nodeObject = new GameObject("NodeDebug: " + node.NodeType);
                    nodeObject.transform.SetParent(transform);
                    nodeObject.transform.position = new Vector2(node.X, node.Y - 0.5f);

                    var spriteRenderer = nodeObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sortingLayerName = "UI";
                    spriteRenderer.sprite = nodeSprite;

                    switch (node.NodeType)
                    {
                        case PathNodeType.LeftEdge:
                        case PathNodeType.RightEdge:
                            spriteRenderer.color = Color.magenta;
                            break;

                        case PathNodeType.Single:
                            spriteRenderer.color = Color.yellow;
                            break;

                        case PathNodeType.Platform:
                            spriteRenderer.color = Color.black;
                            break;

                        case PathNodeType.DropTo:
                            spriteRenderer.color = Color.blue;
                            break;

                        case PathNodeType.JumpFrom:
                            spriteRenderer.color = Color.green;
                            break;
                    }

                    debugNodeObjects.Add(nodeObject);

                    foreach (var link in node.NodeLinks)
                    {
                        var linkObject = new GameObject("NodeLinkDebug: " + link.LinkType);
                        linkObject.transform.SetParent(transform);
                        var lineRenderer = linkObject.AddComponent<LineRenderer>();
                        lineRenderer.startWidth = 0.05f;
                        lineRenderer.endWidth = 0.05f;
                        lineRenderer.material = debugMaterial;
                        lineRenderer.numPositions = 2;
                        lineRenderer.SetPosition(0, new Vector3(x, y - 0.5f, -1.0f));
                        lineRenderer.SetPosition(1, new Vector3(link.DestinationNode.X, link.DestinationNode.Y - 0.5f, -1.0f));

                        switch (link.LinkType)
                        {
                            case NodeLinkType.Walk:
                                lineRenderer.material.color = Color.cyan;
                                debugNodeWalkLinkObjects.Add(linkObject);
                                break;

                            case NodeLinkType.Fall:
                                lineRenderer.material.color = Color.blue;
                                debugNodeFallLinkObjects.Add(linkObject);
                                break;

                            case NodeLinkType.Jump:
                                lineRenderer.material.color = Color.green;
                                debugNodeJumpLinkObjects.Add(linkObject);
                                break;
                        }
                    }
                }
            }
        }

        public void ToggleDebugView(DebugViewType _type, bool _toggle)
        {
            List<GameObject> collectionForToggle = null;

            switch (_type)
            {
                case DebugViewType.Nodes:
                    collectionForToggle = debugNodeObjects;
                    break;

                case DebugViewType.WalkLinks:
                    collectionForToggle = debugNodeWalkLinkObjects;
                    break;

                case DebugViewType.FallLinks:
                    collectionForToggle = debugNodeFallLinkObjects;
                    break;

                case DebugViewType.JumpLinks:
                    collectionForToggle = debugNodeJumpLinkObjects;
                    break;
            }

            if(collectionForToggle != null)
            {
                foreach(var obj in collectionForToggle)
                {
                    obj.SetActive(_toggle);
                }
            }
        }

        private void ClearAll()
        {
            for(var i = debugNodeObjects.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(debugNodeObjects[i]);
                debugNodeObjects.RemoveAt(i);
            }

            for (var i = debugNodeWalkLinkObjects.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(debugNodeWalkLinkObjects[i]);
                debugNodeWalkLinkObjects.RemoveAt(i);
            }

            for (var i = debugNodeFallLinkObjects.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(debugNodeFallLinkObjects[i]);
                debugNodeFallLinkObjects.RemoveAt(i);
            }

            for (var i = debugNodeJumpLinkObjects.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(debugNodeJumpLinkObjects[i]);
                debugNodeJumpLinkObjects.RemoveAt(i);
            }
        }
    }
}
