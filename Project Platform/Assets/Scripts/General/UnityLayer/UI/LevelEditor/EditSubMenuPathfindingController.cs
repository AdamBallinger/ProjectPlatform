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
        public PathfindingDebugDrawController pathDebugController;
        public Button pathfindingButton;

        public GameObject pathfindingMenu;

        public GameObject aiPrefab;
        
        private bool displayNodeGizmos = true;
        private bool displayWalkLinkGizmos = true;
        private bool displayFallLinkGizmos = true;
        private bool displayJumpLinkGizmos = true;

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

        public void OnToggleNodeGizmosButtonPress()
        {
            displayNodeGizmos = !displayNodeGizmos;
            pathDebugController.ToggleDebugView(DebugViewType.Nodes, displayNodeGizmos);
        }

        public void OnToggleWalkLinkGizmosButtonPress()
        {
            displayWalkLinkGizmos = !displayWalkLinkGizmos;
            pathDebugController.ToggleDebugView(DebugViewType.WalkLinks, displayWalkLinkGizmos);
        }

        public void OnToggleFallLinkGizmosButtonPress()
        {
            displayFallLinkGizmos = !displayFallLinkGizmos;
            pathDebugController.ToggleDebugView(DebugViewType.FallLinks, displayFallLinkGizmos);
        }

        public void OnToggleJumpLinkGizmosButtonPress()
        {
            displayJumpLinkGizmos = !displayJumpLinkGizmos;
            pathDebugController.ToggleDebugView(DebugViewType.JumpLinks, displayJumpLinkGizmos);
        }

        public void OnCreateTestPathButtonPress()
        {
            if(GameObject.FindGameObjectWithTag("AI") == null)
            {
                Instantiate(aiPrefab, Vector2.one, Quaternion.identity);
            }

            editorUIController.mouseController.SelectMode = SelectionMode.TestPathStart;
        }

        public void OnClearTestPathButtonPress()
        {
            var ai = GameObject.FindGameObjectWithTag("AI");

            if(ai != null)
            {
                ai.GetComponent<PathfinderAgent>().ClearPath();
                Destroy(ai);
            }
        }

        public void CreatePath(Vector2 _start, Vector2 _end)
        {
            var ai = GameObject.FindGameObjectWithTag("AI").GetComponent<PathfinderAgent>();
            ai.transform.position = _start;
            ai.StartPathing(_start, _end);
        }
    }
}
