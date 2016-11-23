using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class LevelEditorUIController : MonoBehaviour
    {

        // Reference to mouse controller for handling building/clearing tiles.
        public MouseController mouseController;

        private GameObject activeSubMenu;

        public GameObject fileUI;
        public GameObject platformSubMenu;
        public GameObject pathfindingSubMenu;

        public InputField levelName;

        public Text totalTilesText;
        public Text platformsText;

        public void Start()
        {
            totalTilesText.text = "Total Tiles: " + World.Current.GetTileCount();
            //levelName.text = World.Current.Load(Path.Combine(Application.persistentDataPath, "Save_Levels") + "\\AI Test Level 2.xml");
        }

        public void OnPlatformButtonPress()
        {
            mouseController.SelectMode = SelectionMode.BuildMode;
            mouseController.TileBuildType = TileType.Platform;
        }

        public void OnPlatformSettingsButtonPress()
        {
            OpenSubMenu(platformSubMenu);
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
            OpenSubMenu(pathfindingSubMenu);
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

        /// <summary>
        /// Switch the active sub menu to the given menu.
        /// Only 1 sub menu can be open at a time.
        /// </summary>
        /// <param name="_menu"></param>
        private void OpenSubMenu(GameObject _menu)
        {
            if(activeSubMenu == null)
            {
                activeSubMenu = _menu;
                activeSubMenu.SetActive(true);
            }
            else
            {
                if(activeSubMenu == _menu)
                {
                    activeSubMenu.SetActive(false);
                    activeSubMenu = null;
                }
                else
                {
                    activeSubMenu.SetActive(false);
                    activeSubMenu = _menu;
                    activeSubMenu.SetActive(true);
                }
            }
        }

        public void Update()
        {
            // Keep platform count updated.
            platformsText.text = "Platforms: " + World.Current.PlatformCount;
        }
    }
}
