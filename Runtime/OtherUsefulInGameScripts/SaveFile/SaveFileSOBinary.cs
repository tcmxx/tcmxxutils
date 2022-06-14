using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public abstract class SaveFileSOBinary<T> : SaveFileSO<T> {

    protected override void SaveImplementation(string fullPath, T data) {
        FileStream file;
        if (!File.Exists(fullPath)) {
            file = File.Create(fullPath);
        } else {
            file = File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
        }

        var bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    protected override T LoadImplementation(string fullPath, T defaultValue) {
        T result = default;
        if (File.Exists(fullPath)) {
            var bf = new BinaryFormatter();
            var file = File.Open(fullPath, FileMode.Open, FileAccess.Read);

            try {
                result = (T)bf.Deserialize(file);

            } catch (Exception) {
                result = defaultValue;
                Debug.LogError("Can not deserialize the saved data");
            }

            file.Close();
        } else {
            result = defaultValue;
        }

        return result;
    }

    protected override bool CheckValidSaveImplementation(string fullPath) {
        var result = true;
        if (File.Exists(fullPath)) {
            FileStream file = null;
            try {
                BinaryFormatter bf = new BinaryFormatter();
                file = File.Open(fullPath, FileMode.Open, FileAccess.Read);

                T tmpGameData = (T)bf.Deserialize(file);
            } catch (Exception) {
                result = false;
            } finally {
                if (file != null)
                    file.Close();
            }

        } else {
            result = false;
        }

        return result;
    }
}