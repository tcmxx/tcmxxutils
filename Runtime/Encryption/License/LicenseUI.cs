using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace TCUtils
{

    public class LicenseUI : MonoBehaviour
    {
        public GameObject licenseUIBlockerRef;
        public GameObject[] enableObjects;
        public Text info;
        public Text title;
        private void Awake()
        {
            try
            {
                bool licenseGood = LicenseChecker.CheckLicense();
                if (LicenseChecker.CheckLicense())
                {
                    licenseUIBlockerRef.SetActive(false);
                    foreach (var o in enableObjects)
                    {
                        o.SetActive(false);
                    }
                    Debug.Log("License Pass");
                }
                else
                {
                    licenseUIBlockerRef.SetActive(true);
                    foreach (var o in enableObjects)
                    {
                        o.SetActive(true);
                    }
                    title.text = LicenseChecker.tempErrorMessage;
                    Debug.Log(LicenseChecker.tempErrorMessage);
                    info.text = "Please send the file: " + LicenseChecker.DeviceInfoFullPath + " to the developer to get the key.txt file. " +
                        "Then put the file to " + LicenseChecker.LicenseFullPath;
                }
            }
            catch (Exception e)
            {
                Debug.Log("License check error:" + e);
            }

        }
    }
}