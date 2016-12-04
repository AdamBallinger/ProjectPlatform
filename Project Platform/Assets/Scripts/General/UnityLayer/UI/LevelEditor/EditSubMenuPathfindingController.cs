using System.Diagnostics;
using System.Threading;
using Assets.Scripts.AI.Pathfinding;
using Assets.Scripts.General.UnityLayer.AI;
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

        private bool displayGizmos = true;
        private bool displayNodeGizmos = true;
        private bool displayWalkLinkGizmos = false;
        private bool displayFallLinkGizmos = false;
        private bool displayJumpLinkGizmos = false;

        private Path testPath = null;

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

        public void OnToggleNodeGizmosButtonPress()
        {
            displayNodeGizmos = !displayNodeGizmos;
        }

        public void OnToggleWalkLinkGizmosButtonPress()
        {
            displayWalkLinkGizmos = !displayWalkLinkGizmos;
        }

        public void OnToggleFallLinkGizmosButtonPress()
        {
            displayFallLinkGizmos = !displayFallLinkGizmos;
        }

        public void OnToggleJumpLinkGizmosButtonPress()
        {
            displayJumpLinkGizmos = !displayJumpLinkGizmos;
        }

        public void OnCreateTestPathButtonPress()
        {
            editorUIController.mouseController.SelectMode = SelectionMode.TestPathStart;
        }

        public void OnClearTestPathButtonPress()
        {
            if(testPath != null)
            {
                testPath.ClearPath();
            }
        }

        public void CreatePath(Vector2 _start, Vector2 _end)
        {
            var pathFinder = new PathFinder(_start, _end);
            var watch = new Stopwatch();
            watch.Start();
            testPath = pathFinder.FindPath();
            watch.Stop();
            Debug.Log("Path created in " + watch.ElapsedMilliseconds + " ms");
            GameObject.FindGameObjectWithTag("AI").GetComponent<PathfinderAgent>().SetPath(testPath);
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

                            case PathNodeType.DropTo:
                                Gizmos.color = Color.blue;
                                break;

                            case PathNodeType.JumpFrom:
                                Gizmos.color = Color.green;
                                break;

                            case PathNodeType.Single:
                                Gizmos.color = Color.yellow;
                                break;
                        }

                        if(!displayNodeGizmos)
                        {
                            Gizmos.color = Color.clear;
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
                                    Gizmos.color = displayWalkLinkGizmos ? Color.cyan : Color.clear;
                                    break;

                                case NodeLinkType.Fall:
                                    Gizmos.color = displayFallLinkGizmos ? Color.blue : Color.clear;
                                    break;

                                case NodeLinkType.Jump:
                                    Gizmos.color = displayJumpLinkGizmos ? Color.green : Color.clear;
                                    break;
                            }

                            Gizmos.DrawLine(new Vector2(x, y - 0.5f), new Vector2(link.DestinationNode.X, link.DestinationNode.Y - 0.5f));
                        }
                    }
                }
            }

            // Draw a test path if 1 is created.
            if(testPath != null && testPath.VectorPath.Count > 0)
            {
                var last = testPath.VectorPath[0];

                foreach(var vector in testPath.VectorPath)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(last, vector);
                    last = vector;
                }
            }
        }
    }
}
