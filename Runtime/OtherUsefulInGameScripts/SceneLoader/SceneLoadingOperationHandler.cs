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
    //todo: add error handling
    public class SceneLoadingOperationHandle : IEnumerator
    {
        public float Progress
        {
            get
            {
                if (!Started)
                    return 0;
                float p = 0;
                float divider = 0;
                if (extraNeededAssetsLoadingOperation.IsValid())
                {
                    divider += 1;
                    p += extraNeededAssetsLoadingOperation.PercentComplete;
                }
                foreach (var o in builtinLoadingOperations)
                {
                    p += o.progress;
                    divider += 1;
                }
                foreach (var o in addressableLoadingOperations)
                {
                    p += o.PercentComplete;
                    divider += 1;
                }

                return p / divider;
            }
        }


        public event Action<SceneLoadingOperationHandle> onCompleted;
        public bool IsDone
        {
            get
            {
                return IsSceneLoadingDone && IsExtraAssetsLoadingDone;
            }
        }
        public LoadSceneMode LoadMode { get; protected set; }
        public List<Scene> Scenes { get; protected set; } = new List<Scene>();
        public bool Started { get; protected set; } = false;
        public List<object> ExtraNeededAssetKeys { get; set; }

        protected int SceneLoadingDoneCount { get; set; } = 0;
        protected bool IsSceneLoadingDone { get; set; } = false;
        protected bool IsExtraAssetsLoadingDone { get; set; } = false;

        protected List<AsyncOperationHandle<SceneInstance>> addressableLoadingOperations = new List<AsyncOperationHandle<SceneInstance>>();
        protected List<AsyncOperation> builtinLoadingOperations = new List<AsyncOperation>();

        protected AsyncOperationHandle<IList<object>> extraNeededAssetsLoadingOperation;

        protected List<object> sceneAddressableKeysList = new List<object>();
        protected List<string> sceneBuiltinNamesList = new List<string>();

        public SceneLoadingOperationHandle(List<string> builtinSceneNames, List<object> sceneAddressableKeys, List<object> extraNeededAssetKeys = null, LoadSceneMode loadMode = LoadSceneMode.Additive)
        {

            ExtraNeededAssetKeys = extraNeededAssetKeys;
            if (builtinSceneNames != null)
                sceneBuiltinNamesList = builtinSceneNames;
            if (sceneAddressableKeys != null)
                sceneAddressableKeysList = sceneAddressableKeys;

            if (loadMode != LoadSceneMode.Additive && sceneAddressableKeysList.Count + sceneBuiltinNamesList.Count > 1)
            {
                Debug.LogError("Can not load multiple scenes in Single Mode. Will use Additive Mode instead");
                loadMode = LoadSceneMode.Additive;
            }

            LoadMode = loadMode;
        }


        public void StartLoading()
        {
            if (Started)
            {
                Debug.LogError("Repeated callling of start loading scene!! Ignored!");
                return;
            }
            if (ExtraNeededAssetKeys != null && ExtraNeededAssetKeys.Count > 0)
            {
                extraNeededAssetsLoadingOperation = Addressables.LoadAssetsAsync<object>(ExtraNeededAssetKeys, null, Addressables.MergeMode.Union);//for now use union mode
                extraNeededAssetsLoadingOperation.Completed += ExtraAssetsLoadingCompleteNodify;
            }
            else
            {
                ExtraAssetsLoadingCompleteNodify(default);
            }

            builtinLoadingOperations = new List<AsyncOperation>();
            for (int i = 0; i < sceneBuiltinNamesList.Count; ++i)
            {
                Scenes.Add(SceneManager.GetSceneByName(sceneBuiltinNamesList[i]));
                var op = SceneManager.LoadSceneAsync(sceneBuiltinNamesList[i], i == 0 ? LoadMode : LoadSceneMode.Additive);
                builtinLoadingOperations.Add(op);
                op.completed += SceneLoadingCompleteNodify;
            }

            addressableLoadingOperations = new List<AsyncOperationHandle<SceneInstance>>();
            for (int i = 0; i < sceneAddressableKeysList.Count; ++i)
            {
                var op = Addressables.LoadSceneAsync(sceneAddressableKeysList[i], i == 0 ? LoadMode : LoadSceneMode.Additive);
                addressableLoadingOperations.Add(op);
                op.Completed += SceneLoadingCompleteNodify;
            }

        }

        #region private methods
        private void ExtraAssetsLoadingCompleteNodify(AsyncOperationHandle<IList<object>> obj)
        {
            IsExtraAssetsLoadingDone = true;
            CallOnCompleteIfSo();
        }

        private void SceneLoadingCompleteNodify(AsyncOperation op)
        {
            SceneLoadingDoneCount++;
            if (SceneLoadingDoneCount == builtinLoadingOperations.Count + addressableLoadingOperations.Count)
                IsSceneLoadingDone = true;
            CallOnCompleteIfSo();
        }

        private void SceneLoadingCompleteNodify(AsyncOperationHandle<SceneInstance> op)
        {
            SceneLoadingDoneCount++;
            if (SceneLoadingDoneCount == builtinLoadingOperations.Count + addressableLoadingOperations.Count)
                IsSceneLoadingDone = true;

            Scenes.Add(op.Result.Scene);
            CallOnCompleteIfSo();
        }

        private void CallOnCompleteIfSo()
        {
            if (IsDone)
                onCompleted?.Invoke(this);
        }

        bool IEnumerator.MoveNext()
        {
            return !IsDone;
        }

        void IEnumerator.Reset() { }

        object IEnumerator.Current
        {
            get { return Scenes; }
        }
        #endregion
    }
}