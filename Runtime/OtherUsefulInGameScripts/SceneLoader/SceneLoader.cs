using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace TCUtils
{
    public class SceneLoader : SingletonBaseBehaviour<SceneLoader>
    {
        protected Coroutine loadSceneCoroutine = null;
        public bool IsLoading { get { return loadSceneCoroutine != null; } }
        public SceneLoadingOperationHandle LoadBuiltinSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single, List<object> extraAssetKeys = null, ISceneLoadingProcess process = null)
        {
            if (loadSceneCoroutine != null)
            {
                Debug.LogError("Can not have load scene calls while one is running. Wait until it is done, or use LoadScenesAsync to load all together instead");
                return null;
            }
            SceneLoadingOperationHandle handle = new SceneLoadingOperationHandle(new List<string>() { sceneName }, null,extraAssetKeys, loadMode);
            loadSceneCoroutine = StartCoroutine(LoadingSceneCoroutine(handle, process));
            return handle;
        }

        public SceneLoadingOperationHandle LoadAddressableSceneAsync(object sceneKey, LoadSceneMode loadMode = LoadSceneMode.Single, List<object> extraAssetKeys = null, ISceneLoadingProcess process = null)
        {
            if (loadSceneCoroutine != null)
            {
                Debug.LogError("Can not have load scene calls while one is running. Wait until it is done, or use LoadScenesAsync to load all together instead");
                return null;
            }
            SceneLoadingOperationHandle handle = new SceneLoadingOperationHandle(null, new List<object>() { sceneKey }, extraAssetKeys, loadMode);
            loadSceneCoroutine = StartCoroutine(LoadingSceneCoroutine(handle, process));
            return handle;
        }

        public SceneLoadingOperationHandle LoadScenesAsync(List<string> builtinSceneNames, List<object> addressableSceneKeys, LoadSceneMode loadMode = LoadSceneMode.Single, List<object> extraAssetKeys = null, ISceneLoadingProcess process = null)
        {
            if (loadSceneCoroutine != null)
            {
                Debug.LogError("Can not have load scene calls while one is running. Wait until it is done, or use LoadScenesAsync to load all together instead");
                return null;
            }
            SceneLoadingOperationHandle handle = new SceneLoadingOperationHandle(builtinSceneNames, addressableSceneKeys, extraAssetKeys, loadMode);
            loadSceneCoroutine = StartCoroutine(LoadingSceneCoroutine(handle, process));
            return handle;
        }
        
        private IEnumerator LoadingSceneCoroutine(SceneLoadingOperationHandle handle, ISceneLoadingProcess process = null)
        {
            if (process == null)
                process = new SceneLoadingProcessBase();
            handle.onCompleted += ClearCoroutine;

            yield return process.PreLoadingProcess();

            handle.StartLoading();

            process.OnLoadingStarted();

            float progress = -1;
            while (!handle.IsDone)
            {
                float p = handle.Progress;
                if (p != progress)
                {
                    process.OnProgressChanged(p);
                    progress = p;
                }
                yield return null;
            }

            process.OnLoadingCompleted();
        }
        
        protected void ClearCoroutine(SceneLoadingOperationHandle handle)
        {
            loadSceneCoroutine = null;
        }
    }

}