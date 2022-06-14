using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public abstract class SaveFileSOJson<T> : SaveFileSO<T> {

    public bool prettyJson = true;

    protected override void SaveImplementation(string fullPath, T data) {

        var json = JsonUtility.ToJson(data, prettyJson);
        File.WriteAllText(fullPath, json);
    }

    protected override T LoadImplementation(string fullPath, T defaultValue) {
        if (!File.Exists(fullPath)) {
            Debug.Log($"Save File Not Exist.{fullPath}, use default value");
            return defaultValue;
        }

        try {
            var json = File.ReadAllText(fullPath);

            var result = JsonUtility.FromJson<T>(json);
            return result;
        } catch (Exception) {
            return defaultValue;
        }
    }

    protected override bool CheckValidSaveImplementation(string fullPath) {
        var result = true;
        try {
            var json = File.ReadAllText(fullPath);

            JsonUtility.FromJson<T>(json);
        } catch (Exception) {
            result = false;
        }

        return result;
    }
}