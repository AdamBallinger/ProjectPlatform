using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class LoadLevelUIController : MonoBehaviour
    {

        public GameObject fileListObject;
        public GameObject fileEntryPrefab;

        public Button cancelButton;

        private List<string> fileDirectories = new List<string>();

        public void Awake()
        {
            fileDirectories.Clear();
            AddLevelFilesToWindow();
        }

        private void AddLevelFilesToWindow()
        {
            var saveFolder = Path.Combine(Application.persistentDataPath, "Save_Levels");
            fileDirectories.AddRange(Directory.GetFiles(saveFolder));

            for(var i = 0; i < fileDirectories.Count; i++)
            {
                var fileEntry = Instantiate(fileEntryPrefab);
                fileEntry.transform.SetParent(fileListObject.transform, false);
                fileEntry.GetComponentInChildren<Text>().text = Path.GetFileName(fileDirectories[i]);
                fileEntry.GetComponent<FileButtonController>().LoadFile = fileDirectories[i];
            }
        }

        public void OnCancelButtonPress()
        {
           gameObject.SetActive(false); 
        }
    }
}
