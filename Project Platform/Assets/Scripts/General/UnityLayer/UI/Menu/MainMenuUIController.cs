using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.General.UnityLayer.UI.Menu
{
    public class MainMenuUIController : MonoBehaviour
    {

        public void OnLevelEditorButtonPress()
        {
            SceneManager.LoadScene("level_editor");
        }

        public void OnExitButtonPress()
        {
            Application.Quit();
        }
    }
}
