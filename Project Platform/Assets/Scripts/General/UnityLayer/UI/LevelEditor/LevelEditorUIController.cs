using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class LevelEditorUIController : MonoBehaviour
    {

        // Reference to mouse controller for handling building/clearing tiles.
        public MouseController mouseController;

        public GameObject fileUI;
        public GameObject pathfindingSubMenu;

        public InputField levelName;

        public Text totalTilesText;
        public Text platformsText;

        public void Start()
        {
            totalTilesText.text = "Total Tiles: " + World.Current.GetTileCount();
            platformsText.text = "Platforms: " + World.Current.GetTileCountOfType(TileType.Platform);
            World.Current.Load(Path.Combine(Application.persistentDataPath, "Save_Levels") + "\\AI Test Level.xml");
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

        public void OnSetPlayerSpawnButtonPress()
        {
            mouseController.SelectMode = SelectionMode.PlayerSpawnSet;
        }

        public void OnPathfindingButtonPress()
        {
            pathfindingSubMenu.SetActive(!pathfindingSubMenu.activeSelf);
        }

        public void OnSaveLevelButtonPress()
        {
            World.Current.Save(levelName.text);
        }

        public void OnLoadLevelButtonPress()
        {
            fileUI.SetActive(true);
        }

        public void OnClearLevelButtonPress()
        {
            World.Current.Clear();
            World.Current.SetBorderAsPlatform();

            levelName.text = string.Empty;
        }

        public void OnExitButtonPress()
        {
            SceneManager.LoadScene("_menu");
        }

        public void Update()
        {
            // Keep platform count updated.
            platformsText.text = "Platforms: " + World.Current.PlatformCount;
        }
    }
}
