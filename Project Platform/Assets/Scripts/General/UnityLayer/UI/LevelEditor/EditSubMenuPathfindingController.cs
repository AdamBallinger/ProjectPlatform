using System.Diagnostics;
using Assets.Scripts.AI.Pathfinding;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class EditSubMenuPathfindingController : MonoBehaviour
    {
        public LevelEditorUIController editorUIController;
        public Button pathfindingButton;

        public GameObject pathfindingMenu;

        public bool displayGizmos = false;

        public void OnPathfindingButtonPress()
        {
            editorUIController.OpenSubMenu(pathfindingMenu);
        }

        public void Update()
        {
            var buttonText = editorUIController.GetActiveSubMenu() == pathfindingMenu ? "Pathfinding      <" : "Pathfinding      >";
            pathfindingButton.GetComponentInChildren<Text>().text = buttonText;
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

                        switch (nodeType)
                        {
                            case PathNodeType.LeftEdge:
                            case PathNodeType.RightEdge:
                                Gizmos.color = Color.magenta;
                                break;

                            case PathNodeType.Platform:
                                Gizmos.color = Color.black;
                                break;

                            case PathNodeType.Single:
                                Gizmos.color = Color.yellow;
                                break;
                        }

                        Gizmos.DrawCube(new Vector2(x, y - 0.5f), Vector2.one * 0.4f);
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
                                    var trajectory = link.GetData<JumpTrajectory>().Trajectory;
                                    var lastPoint = trajectory[0];
                                    for(var i = 1; i < trajectory.Count - 1; i++)
                                    {
                                        Gizmos.DrawLine(lastPoint, trajectory[i]);
                                        lastPoint = trajectory[i];
                                    }
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
