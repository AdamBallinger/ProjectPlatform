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

        private GameObject activeSubMenu;

        public InputField levelName;

        public Text totalTilesText;
        public Text platformsText;

        public void Start()
        {
            totalTilesText.text = "Total Tiles: " + World.Current.GetTileCount();
            //levelName.text = FindObjectOfType<WorldController>().Load(Directories.Stock_Levels_Directory + "\\AI Test Level 2.xml",
            //                                                            Directories.Stock_Levels_Data_Directory + "\\AI Test Level 2.xml");
        }

        public void OnPlatformButtonPress()
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

        public void OnSaveLevelButtonPress()
        {
            FindObjectOfType<WorldController>().Save(levelName.text);
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
        public void OpenSubMenu(GameObject _menu)
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

        /// <summary>
        /// Returns the currently active sub menu.
        /// </summary>
        /// <returns></returns>
        public GameObject GetActiveSubMenu()
        {
            return activeSubMenu;
        }

        public void Update()
        {
            // Keep platform count updated.
            platformsText.text = "Platforms: " + World.Current.PlatformCount;
        }
    }
}
