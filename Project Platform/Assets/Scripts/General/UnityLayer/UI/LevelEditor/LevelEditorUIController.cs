using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class LevelEditorUIController : MonoBehaviour
    {

        // Reference to the world controller.
        public WorldController worldController;

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

            // if the last loaded level for the editor transition object isnt empty then load the last loaded level.
            if(EditorPlayModeTransition.LastLoadedLevelName != null)
            {
                levelName.text = worldController.Load(EditorPlayModeTransition.PlayModeLevel, EditorPlayModeTransition.PlayModeLevelData);
            }
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

        public void OnCoinPickupButtonPress()
        {
            mouseController.SelectMode = SelectionMode.CoinPickup;
        }

        public void OnSaveLevelButtonPress()
        {
            worldController.Save(levelName.text);
        }

        public void OnLoadLevelButtonPress()
        {
            fileUI.SetActive(true);
        }

        public void OnClearLevelButtonPress()
        {
            worldController.Clear();
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
