using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    public interface ISceneLoadingProcess
    {
        IEnumerator PreLoadingProcess();
        void OnLoadingStarted();
        void OnProgressChanged(float progress);
        void OnLoadingCompleted();
    }

    public class SceneLoadingProcessBase: ISceneLoadingProcess
    {
        public virtual IEnumerator PreLoadingProcess()
        {
            yield break;
        }

        public virtual void OnLoadingStarted() { }

        public virtual void OnProgressChanged(float progress) { }

        public virtual void OnLoadingCompleted() { }
    }
}