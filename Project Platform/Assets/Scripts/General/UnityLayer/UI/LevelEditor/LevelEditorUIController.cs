using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class LevelEditorUIController : MonoBehaviour
    {

        // Reference to mouse controller for handling building/clearing tiles.
        public MouseController mouseController;

        public Text totalTilesText;
        public Text platformsText;

        public Button solidPlatformButton;
        public Button clearToolButton;

        public Button saveLevelButton;
        public Button clearLevelButton;
        public Button exitEditorButton;

        public void Start()
        {
            totalTilesText.text = "Total Tiles: " + World.Current.GetTileCount();
            platformsText.text = "Platforms: " + World.Current.GetTileCountOfType(TileType.Platform);
        }

        public void OnWorldModified()
        {
            platformsText.text = "Platforms: " + World.Current.GetTileCountOfType(TileType.Platform);
        }

        public void OnSolidPlatformButtonPress()
        {
            mouseController.SelectMode = SelectionMode.BuildMode;
            mouseController.TileBuildType = TileType.Platform;
        }

        public void OnClearToolButtonPress()
        {
            mouseController.SelectMode = SelectionMode.ClearMode;
        }

        public void OnSaveButtonPress()
        {
            // TODO: Save world state to file on disk
        }

        public void OnClearLevelButtonPress()
        {
            World.Current.Clear();
            platformsText.text = "Platforms: 0";
        }

        public void OnExitButtonPress()
        {
            Application.Quit();
        }
    }
}
