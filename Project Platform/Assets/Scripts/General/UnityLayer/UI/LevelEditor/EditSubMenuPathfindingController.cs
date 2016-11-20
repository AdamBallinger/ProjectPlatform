using System.Diagnostics;
using Assets.Scripts.AI.Pathfinding;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class EditSubMenuPathfindingController : MonoBehaviour
    {

        public Button pathfindingButton;

        public bool displayGizmos = true;

        public void OnEnable()
        {
            pathfindingButton.GetComponentInChildren<Text>().text = "Pathfinding      <";
        }

        public void OnDisable()
        {
            pathfindingButton.GetComponentInChildren<Text>().text = "Pathfinding      >";
        }

        public void OnBuildNavGraphButtonPress()
        {
            var watch = new Stopwatch();
            watch.Start();
            World.Current.NavGraph.ScanGraph();
            watch.Stop();
            Debug.Log("Node graph scan took: " + watch.ElapsedMilliseconds + " ms");
        }

        public void OnToggleGizmosButtonPress()
        {
            displayGizmos = !displayGizmos;
        }

        public void OnDrawGizmos()
        {
            if (World.Current == null || !displayGizmos)
                return;

            var nodes = World.Current.NavGraph.Nodes;

            // Draw node markers.
            for (var x = 0; x < World.Current.NavGraph.Width; x++)
            {
                for (var y = 0; y < World.Current.NavGraph.Height; y++)
                {
                    if (nodes[x, y].NodeType != PathNodeType.None)
                    {
                        var nodeType = nodes[x, y].NodeType;

                        if (nodeType == PathNodeType.LeftEdge || nodeType == PathNodeType.RightEdge)
                        {
                            Gizmos.color = Color.magenta;
                        }

                        if (nodeType == PathNodeType.Platform)
                        {
                            Gizmos.color = Color.black;
                        }

                        if (nodeType == PathNodeType.Single)
                        {
                            Gizmos.color = Color.yellow;
                        }

                        Gizmos.DrawSphere(new Vector2(x, y - 0.5f), 0.25f);
                    }
                }
            }

            // Draw node links.
            for (var x = 0; x < World.Current.NavGraph.Width; x++)
            {
                for (var y = 0; y < World.Current.NavGraph.Height; y++)
                {
                    if (nodes[x, y].NodeType != PathNodeType.None)
                    {
                        foreach (var link in nodes[x, y].NodeLinks)
                        {
                            switch (link.LinkType)
                            {
                                case NodeLinkType.Walk:
                                    Gizmos.color = Color.cyan;
                                    break;

                                case NodeLinkType.Fall:
                                    Gizmos.color = Color.blue;
                                    break;

                                case NodeLinkType.Jump:
                                    Gizmos.color = Color.green;
                                    break;
                            }

                            Gizmos.DrawLine(new Vector2(x, y - 0.5f), new Vector2(link.DestinationNode.X, link.DestinationNode.Y - 0.5f));
                        }
                    }
                }
            }
        }
    }
}
