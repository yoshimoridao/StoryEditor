using System;
using UnityEngine;
using UnityEngine.UI;

// Include these namespaces to use BinaryFormatter
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace GracesGames.SimpleFileBrowser.Scripts
{
    // Demo class to illustrate the usage of the FileBrowser script
    // Able to save and load files containing serialized data (e.g. text)
    public class FileBrowserCaller : MonoBehaviour
    {
        // Use the file browser prefab
        public GameObject FileBrowserPrefab;

        // Define a file extension
        public string[] FileExtensions;

        public bool PortraitMode;

        private Action callback;
        [SerializeField]
        private Image bgImg;
        private FileBrowser fileBrowserScript;

        // Find the input field, label objects and add a onValueChanged listener to the input field
        private void Start()
        {
            // default hide bg
            ActiveBg(false);
        }

        // Open the file browser using boolean parameter so it can be called in GUI
        public void OpenFileBrowser(bool saving, Action _callback)
        {
            // store call back func
            if (_callback != null)
                callback = _callback;

            OpenFileBrowser(saving);
        }

        public void OpenFileBrowser(bool saving)
        {
            OpenFileBrowser(saving ? FileBrowserMode.Save : FileBrowserMode.Load);
        }

        public void OnFileBrowserDestroy()
        {
            // hide bg
            ActiveBg(false);

            fileBrowserScript.actOnDestroy -= OnFileBrowserDestroy;
            fileBrowserScript = null;
        }

        // Open a file browser to save and load files
        private void OpenFileBrowser(FileBrowserMode fileBrowserMode)
        {
            // show bg
            ActiveBg(true);

            // Create the file browser and name it
            GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
            fileBrowserObject.name = "FileBrowser";
            // Set the mode to save or load
            fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser(PortraitMode ? ViewMode.Portrait : ViewMode.Landscape);
            if (fileBrowserMode == FileBrowserMode.Save)
            {
                fileBrowserScript.SaveFilePanel(DataDefine.save_filename_default, FileExtensions);
                // Subscribe to OnFileSelect event (call SaveFileUsingPath using path) 
                fileBrowserScript.OnFileSelect += SaveFileUsingPath;
            }
            else
            {
                fileBrowserScript.OpenFilePanel(FileExtensions);
                // Subscribe to OnFileSelect event (call LoadFileUsingPath using path) 
                fileBrowserScript.OnFileSelect += LoadFileUsingPath;
            }

            // register action destroy
            fileBrowserScript.actOnDestroy += OnFileBrowserDestroy;
        }

        // Saves a file with the textToSave using a path
        private void SaveFileUsingPath(string _path)
        {
            // do call back
            if (callback != null)
            {
                callback.Invoke();
                callback = null;
            }

            DataMgr.Instance.Save(_path);

            // hide bg
            ActiveBg(false);

            //// Make sure path and _textToSave is not null or empty
            //if (!String.IsNullOrEmpty(_path))
            //{
            //    BinaryFormatter bFormatter = new BinaryFormatter();
            //    // Create a file using the path
            //    FileStream file = File.Create(_path);
            //    // Serialize the data (textToSave)
            //    bFormatter.Serialize(file, _textToSave);
            //    // Close the created file
            //    file.Close();
            //}
            //else
            //{
            //    Debug.Log("Invalid path or empty file given");
            //}
        }

        // Loads a file using a path
        private void LoadFileUsingPath(string _path)
        {
            // do call back
            if (callback != null)
            {
                callback.Invoke();
                callback = null;
            }

            DataMgr.Instance.Load(_path);

            // hide bg
            ActiveBg(false);

            //if (path.Length != 0)
            //{
            //    BinaryFormatter bFormatter = new BinaryFormatter();
            //    // Open the file using the path
            //    FileStream file = File.OpenRead(path);
            //    // Convert the file from a byte array into a string
            //    string fileData = bFormatter.Deserialize(file) as string;
            //    // We're done working with the file so we can close it
            //    file.Close();
            //    // Set the LoadedText with the value of the file
            //    _loadedText.GetComponent<Text>().text = "Loaded data: \n" + fileData;
            //}
            //else
            //{
            //    Debug.Log("Invalid path given");
            //}
        }

        private void ActiveBg(bool _isEnable)
        {
            if (bgImg)
                bgImg.enabled = _isEnable;
        }
    }
}