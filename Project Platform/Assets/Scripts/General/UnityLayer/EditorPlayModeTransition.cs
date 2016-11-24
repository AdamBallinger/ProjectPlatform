using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.General.UnityLayer
{
    public class EditorPlayModeTransition : MonoBehaviour
    {

        public static string PlayModeLevel { get; private set; }

        public Text levelName;

        /// <summary>
        /// Returns from the editor play scene to the editor scene.
        /// </summary>
        public static void ReturnToEditor()
        {
            // Only load the level editor scene if it is the current scene.
            if(SceneManager.GetActiveScene().name != "level_editor")
            {
                SceneManager.LoadScene("level_editor");
                World.Current.Load(PlayModeLevel);
            }
        }

        /// <summary>
        /// Saves then loads the current level being edited into the playmode scene.
        /// </summary>
        public void PlayLevel()
        {
            if(SceneManager.GetActiveScene().name == "level_editor")
            {
                World.Current.Save(levelName.text);
                PlayModeLevel = Path.Combine(Application.persistentDataPath, "Save_Levels" + "\\" + levelName.text + ".xml");
                SceneManager.LoadScene("editor_playmode");
            }
        }
    }
}
