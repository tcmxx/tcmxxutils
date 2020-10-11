using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;




//use for loading the data in advance and access the data by names
[Serializable]
public class LabeledAssetStorage<T> where T : UnityEngine.Object
{
    public AssetLabelReference label;
    public bool Loaded { get; private set; } = false;
    protected Dictionary<string, T> assetDictionary = new Dictionary<string, T>();
    public List<T> AllAssets { get; private set; }

    public AsyncOperationHandle Handle { get; private set; }

    public AsyncOperationHandle RequestAssets(bool forceReload = true)
    {
        if (Handle.IsValid() && Handle.Status == AsyncOperationStatus.Succeeded)
            return Handle;
        
        var loadAbilityOp = Addressables.LoadAssetsAsync<T>(label.labelString, null);
        loadAbilityOp.Completed +=
            t =>
            {
                if (Loaded && !forceReload)
                    return;
                assetDictionary = new Dictionary<string, T>();
                AllAssets = new List<T>(t.Result);
                assetDictionary = new Dictionary<string, T>();
                if (t.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning("Load ability info faild: " + t.Status);
                    return;
                }


                foreach (var a in t.Result)
                {
                    if (assetDictionary.ContainsKey(a.name))
                    {
                        Debug.LogError("there are muliple asset info uses the name " + a.name);
                    }
                    
                    assetDictionary[a.name] = a;
                }
                Loaded = true;
            };

        Handle = loadAbilityOp;
        return loadAbilityOp;
    }

    public void RemoveAssetsRequest()
    {
        Addressables.Release(Handle);
    }

    public T GetAsset(string name)
    {
        if (!Loaded)
        {
            Debug.LogError("Assets of type" + typeof(T).Name + " with label: " + label.labelString + " not loaded yet while trying to get: " + name);
            return null;
        }

        T asset = null;


        if (assetDictionary.TryGetValue(name, out asset))
        {
            return asset;
        }
        else
        {
            Debug.LogError("Assets of type" + typeof(T).Name + " with label: " + label.labelString + "and name:" + name + " not found.");
            return null;
        }
    }
}