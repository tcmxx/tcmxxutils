using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TCUtils
{
    public static class LicenseChecker
    {
        public const string SecretKey = "k6sl%9e_";
        public const string LicenseRelativePath = "key.txt";
        public const string DeviceInfoPath = "device.txt";

        public static readonly string LicenseFullPath = Path.Combine(Application.dataPath, LicenseRelativePath);
        public static readonly string DeviceInfoFullPath = Path.Combine(Application.dataPath, DeviceInfoPath);

        public static string tempErrorMessage;

        public static string GetCorrectDeviceKeyForLocal()
        {
            return GetCorrectDeviceKey(SystemInfo.deviceUniqueIdentifier);
        }
        public static string GetCorrectDeviceKey(string deviceID)
        {
            return SimpleEncrypt.Encrypt(deviceID, SecretKey);
        }
        public static bool CheckLicense()
        {
            string licenseKey = GetLicenseKeyString();
            Debug.Log("License key: " + licenseKey);
            if (string.IsNullOrEmpty(licenseKey))
            {
                tempErrorMessage = "No license file.";
                SaveDeviceInfo();
                return false;
            }
            else
            {
                var isGood = licenseKey == GetCorrectDeviceKeyForLocal();
                if (!isGood)
                {
                    tempErrorMessage = "License not matching";
                    SaveDeviceInfo();
                }
                return isGood;
            }
        }

        private static void SaveDeviceInfo()
        {
            System.IO.File.WriteAllText(DeviceInfoFullPath, SystemInfo.deviceUniqueIdentifier);
            Debug.Log("Device Info saved at:" + DeviceInfoFullPath);
        }

        public static string GetLicenseKeyString()
        {
            try
            {
                Debug.Log("Try getting License from " + LicenseFullPath);
                StreamReader reader = new StreamReader(LicenseFullPath);
                return reader.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }
    }
}