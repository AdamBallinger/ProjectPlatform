using System.Diagnostics;
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
            if (testPath != null)
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
    }
}
