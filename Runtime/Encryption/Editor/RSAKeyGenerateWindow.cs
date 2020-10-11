using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace TCUtils
{
    public class RSAKeyGenerateWindow : EditorWindow
    {
        private (string publicKey, string privateKey) rasKeys;
        private (string key, string IV) aesKeys;

        private SimpleRSACrypto.EncryptionBits encryptionBits;

        private string keyToUse;
        private string IVToUse;
        private string valueToEncryptOrDecrypt;
        private string resultValue;

        public enum Method
        {
            AES,
            RSA
        }
        private Method method;

        // Add menu item named "My Window" to the Window menu
        [MenuItem("TC/Crypto/CryptoWindow")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(RSAKeyGenerateWindow));
        }

        void OnGUI()
        {
            method = (Method)EditorGUILayout.EnumPopup((Method)method);
            if (method == Method.AES)
            {
                OnGUIAES();
            }
            else if (method == Method.RSA)
            {
                OnGUIRSA();
            }

        }

        private void OnGUIAES()
        {
            if (GUILayout.Button("Generate Key"))
            {
                aesKeys.key = SimpleAES.GenerateKeyString();
            }

            if (GUILayout.Button("Generate IV"))
            {
                aesKeys.IV = SimpleAES.GenerateIVString();
            }

            EditorGUILayout.LabelField("Key");
            EditorGUILayout.SelectableLabel(aesKeys.key, GUILayout.Height(30));
            EditorGUILayout.LabelField("IV");
            EditorGUILayout.SelectableLabel(aesKeys.IV, GUILayout.Height(30));

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Encrypt"))
                {
                    resultValue = SimpleAES.EncryptStringToString(valueToEncryptOrDecrypt, keyToUse, IVToUse);
                }
                if (GUILayout.Button("Decrypt"))
                {
                    resultValue = SimpleAES.DecryptStringFromString(valueToEncryptOrDecrypt, keyToUse, IVToUse);
                }
            }
            EditorGUILayout.LabelField("Key To Use");
            keyToUse = EditorGUILayout.TextArea(keyToUse, GUILayout.Height(30));
            EditorGUILayout.LabelField("IV To Use");
            IVToUse = EditorGUILayout.TextArea(IVToUse, GUILayout.Height(30));

            EditorGUILayout.LabelField("Value");
            valueToEncryptOrDecrypt = EditorGUILayout.TextArea(valueToEncryptOrDecrypt, GUILayout.Height(70));
            EditorGUILayout.LabelField("Result");
            resultValue = EditorGUILayout.TextArea(resultValue, GUILayout.Height(70));
        }

        private void OnGUIRSA()
        {
            encryptionBits = (SimpleRSACrypto.EncryptionBits)EditorGUILayout.EnumPopup((Enum)encryptionBits);
            if (GUILayout.Button("Generate Keys"))
            {
                rasKeys = SimpleRSACrypto.GenerateKeys(encryptionBits);
            }
            EditorGUILayout.LabelField("Public Key");
            EditorGUILayout.SelectableLabel(rasKeys.publicKey, GUILayout.Height(70));
            EditorGUILayout.LabelField("Private Key");
            EditorGUILayout.SelectableLabel(rasKeys.privateKey, GUILayout.Height(70));

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Encrypt"))
                {
                    resultValue = SimpleRSACrypto.Encrypt(valueToEncryptOrDecrypt, keyToUse);
                }
                if (GUILayout.Button("Decrypt"))
                {
                    resultValue = SimpleRSACrypto.Decrypt(valueToEncryptOrDecrypt, keyToUse);
                }
            }
            EditorGUILayout.LabelField("Key To Use");
            keyToUse = EditorGUILayout.TextArea(keyToUse, GUILayout.Height(70));
            EditorGUILayout.LabelField("Value");
            valueToEncryptOrDecrypt = EditorGUILayout.TextArea(valueToEncryptOrDecrypt, GUILayout.Height(70));
            EditorGUILayout.LabelField("Result");
            resultValue = EditorGUILayout.TextArea(resultValue, GUILayout.Height(70));
        }
    }
}