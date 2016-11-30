using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public enum LevelFileTypes
    {
        Stock,
        Saved
    }

    public class LoadLevelUIController : MonoBehaviour
    {

        public GameObject fileListObject;
        public GameObject fileEntryPrefab;

        public Button cancelButton;

        private List<string> fileDirectories = new List<string>();
        private List<string> fileDataDirectories = new List<string>();

        private List<GameObject> fileEntryObjects = new List<GameObject>();


        public void OnEnable()
        {
            Directories.CheckDirectories();
            fileDirectories.Clear();
            fileDataDirectories.Clear();
            AddLevelFilesToWindow(LevelFileTypes.Stock);
        }

        private void AddLevelFilesToWindow(LevelFileTypes _type)
        {
            ClearEntryObjects();

            var saveFolder = _type == LevelFileTypes.Stock ? Directories.Stock_Levels_Directory : Directories.Save_Levels_Directory;
            var saveDataFolder =  _type == LevelFileTypes.Stock ? Directories.Stock_Levels_Data_Directory : Directories.Save_Levels_Data_Directory;

            fileDirectories.AddRange(Directory.GetFiles(saveFolder, "*.xml"));
            fileDataDirectories.AddRange(Directory.GetFiles(saveDataFolder, "*.xml"));

            foreach (var file in fileDirectories)
            {
                var fileEntry = Instantiate(fileEntryPrefab);
                fileEntryObjects.Add(fileEntry);
                fileEntry.transform.SetParent(fileListObject.transform, false);
                if(_type == LevelFileTypes.Stock)
                {
                    // Hide the delete button from stock levels.
                    fileEntry.transform.FindChild("Delete").gameObject.SetActive(false);
                }
                fileEntry.GetComponentInChildren<Text>().text = Path.GetFileName(file);
                fileEntry.GetComponent<FileButtonController>().LoadFile = file;
                fileEntry.GetComponent<FileButtonController>().LoadDataFile = fileDataDirectories[fileDirectories.IndexOf(file)];
            }
        }

        public void OnCancelButtonPress()
        {
           gameObject.SetActive(false); 
        }

        public void OnStockLevelsButtonPress()
        {
            AddLevelFilesToWindow(LevelFileTypes.Stock);
        }

        public void OnSavedLevelsButtonPress()
        {
            AddLevelFilesToWindow(LevelFileTypes.Saved);
        }

        /// <summary>
        /// Destroy all file entry gameobjects that were instantiated and clear the file lists.
        /// </summary>
        private void ClearEntryObjects()
        {
            fileDirectories.Clear();
            fileDataDirectories.Clear();

            for(var i = fileEntryObjects.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(fileEntryObjects[i]);
            }
        }
    }
}
