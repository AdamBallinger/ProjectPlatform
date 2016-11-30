using System.IO;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class FileButtonController : MonoBehaviour
    {

        public string LoadFile { get; set; }

        public string LoadDataFile { get; set; }

        public void OnClick()
        {
            FindObjectOfType<LevelEditorUIController>().levelName.text = FindObjectOfType<WorldController>().Load(LoadFile, LoadDataFile);
            FindObjectOfType<LoadLevelUIController>().gameObject.SetActive(false);          
        }

        public void OnDeleteClick()
        {
            File.Delete(LoadFile);
            File.Delete(LoadDataFile);
            Destroy(gameObject);
        }

    }
}
