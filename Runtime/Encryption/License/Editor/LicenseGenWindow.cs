using UnityEditor;
using UnityEngine;
using System.IO;
namespace TCUtils
{
    public class LicenGenWindow : EditorWindow
    {
        string key = "";
        string deviceID;
        string fullPath;

        // Add menu item named "My Window" to the Window menu
        [MenuItem("TC/Crypto/LicenseGenerate")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            var w = EditorWindow.GetWindow(typeof(LicenGenWindow)) as LicenGenWindow;
            w.fullPath = Path.Combine(Application.dataPath, "key.txt");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Device ID");
            deviceID = EditorGUILayout.TextArea(deviceID);

            if (GUILayout.Button("Generate") && !string.IsNullOrEmpty(deviceID))
            {
                key = LicenseChecker.GetCorrectDeviceKey(deviceID);

                System.IO.File.WriteAllText(fullPath, key);
            }
            EditorGUILayout.LabelField("Key: (Also in file: " + fullPath + ")");
            EditorGUILayout.SelectableLabel(key);
        }
    }
}