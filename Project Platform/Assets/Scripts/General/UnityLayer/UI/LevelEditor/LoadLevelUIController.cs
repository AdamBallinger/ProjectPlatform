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

        private List<GameObject> fileEntryObjects = new List<GameObject>();

        public void OnEnable()
        {
            fileDirectories.Clear();
            AddLevelFilesToWindow();
        }

        public void OnDisable()
        {
            ClearEntryObjects();
        }

        private void AddLevelFilesToWindow()
        {
            var saveFolder = Path.Combine(Application.persistentDataPath, "Save_Levels");
            fileDirectories.AddRange(Directory.GetFiles(saveFolder));

            foreach (var file in fileDirectories)
            {
                var fileEntry = Instantiate(fileEntryPrefab);
                fileEntryObjects.Add(fileEntry);
                fileEntry.transform.SetParent(fileListObject.transform, false);
                fileEntry.GetComponentInChildren<Text>().text = Path.GetFileName(file);
                fileEntry.GetComponent<FileButtonController>().LoadFile = file;
            }
        }

        public void OnCancelButtonPress()
        {
           gameObject.SetActive(false); 
        }

        /// <summary>
        /// Destroy all file entry gameobjects that were instantiated.
        /// </summary>
        private void ClearEntryObjects()
        {
            for(var i = fileEntryObjects.Count - 1; i >= 0; i--)
            {
                Destroy(fileEntryObjects[i]);
            }
        }
    }
}
