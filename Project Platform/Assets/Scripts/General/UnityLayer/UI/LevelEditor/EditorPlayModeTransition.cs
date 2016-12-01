using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class EditorPlayModeTransition : MonoBehaviour
    {

        public static string PlayModeLevel { get; private set; }
        public static string PlayModeLevelData { get; private set; }

        public InputField levelName;

        public static string LastLoadedLevelName { get; private set; }

        /// <summary>
        /// Returns from the editor play scene to the editor scene.
        /// </summary>
        public static void ReturnToEditor()
        {
            // Only load the level editor scene if it is the current scene.
            if(SceneManager.GetActiveScene().name != "level_editor")
            {
                Debug.Log("Pre scene load: " + LastLoadedLevelName);
                SceneManager.LoadScene("level_editor");
                Debug.Log("Post scene load: " + LastLoadedLevelName);
            }
        }

        /// <summary>
        /// Saves then loads the current level being edited into the playmode scene.
        /// </summary>
        public void PlayLevel()
        {
            if(SceneManager.GetActiveScene().name == "level_editor")
            {
                if(levelName.text == string.Empty)
                {
                    levelName.text = "un-named level";
                }

                LastLoadedLevelName = levelName.text;
                Debug.Log("Set last loaded level name to " + LastLoadedLevelName);

                FindObjectOfType<WorldController>().Save(levelName.text);
                PlayModeLevel = Path.Combine(Directories.Save_Levels_Directory, levelName.text + ".xml");
                PlayModeLevelData = Path.Combine(Directories.Save_Levels_Data_Directory, levelName.text + ".xml");
                SceneManager.LoadScene("editor_playmode");
            }
        }
    }
}
