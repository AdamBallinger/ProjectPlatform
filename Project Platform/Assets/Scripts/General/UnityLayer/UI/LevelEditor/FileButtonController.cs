using System.IO;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class FileButtonController : MonoBehaviour
    {

        public string LoadFile { get; set; }

        public void OnClick()
        {
            FindObjectOfType<LevelEditorUIController>().levelName.text = World.Current.Load(LoadFile);
            FindObjectOfType<LoadLevelUIController>().gameObject.SetActive(false);          
        }

        public void OnDeleteClick()
        {
            File.Delete(LoadFile);
            Destroy(gameObject);
        }

    }
}
