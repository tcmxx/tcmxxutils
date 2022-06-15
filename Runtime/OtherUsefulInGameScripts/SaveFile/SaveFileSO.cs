using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace TCUtils {
    public enum SaveFilePathType {
        UnderPersistentDataPath,
        UnderDataPath,
        UnderTemporaryCachePath,
        Absolute
    }

    public abstract class SaveFileSO<T> : ScriptableObject {

        public SaveFilePathType pathType;
        public string filePath;

        [SerializeField]
        private T defaultValue;

        public T CurrentValue { get; set; }

        public string GetFullFilePath() {
            return Path.Combine(GetRootPath(), filePath);
        }

        public void ResetToDefault() {
            CurrentValue = defaultValue;
        }

        public void Save() {
            SaveImplementation(GetFullFilePath(), CurrentValue);
        }

        public void Load() {
            CurrentValue = LoadImplementation(GetFullFilePath(), defaultValue);
        }

        public bool CheckValidSave() {
            return CheckValidSaveImplementation(GetFullFilePath());
        }

        protected abstract void SaveImplementation(string fullPath, T data);

        protected abstract T LoadImplementation(string fullPath, T defaultValue);

        protected abstract bool CheckValidSaveImplementation(string fullPath);

        public string GetRootPath() {
            switch (pathType) {
                case SaveFilePathType.UnderDataPath:
                    return Application.dataPath;
                case SaveFilePathType.UnderPersistentDataPath:
                    return Application.persistentDataPath;
                case SaveFilePathType.UnderTemporaryCachePath:
                    return Application.temporaryCachePath;
                default:
                    return "";
            }
        }
    }
}