using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

[System.Obsolete]
public class BatchYieldInstructionLong: BatchYieldInstruction<long>
{
    public BatchYieldInstructionLong(List<AsyncOperationHandle<long>> handles, List<float> progressWeight = null):
        base(handles, progressWeight)
    {

    }
    public long SumResult
    {
        get
        {
            long result = 0;
            foreach(var h in Handles)
            {
                result += h.Result;
            }
            return result;
        }
    }
}


[System.Obsolete]
public class BatchYieldInstruction<T> : CustomYieldInstruction
{
    public List<AsyncOperationHandle<T>> Handles { get; private set; }
    protected List<float> progressWeight;
    protected float totalProgress;

    public BatchYieldInstruction(List<AsyncOperationHandle<T>> handles, List<float> progressWeight = null)
    {
        Debug.Assert(progressWeight == null || progressWeight.Count == handles.Count);
        if (progressWeight == null)
        {
            progressWeight = new List<float>();
            for (int i = 0; i <= handles.Count; ++i)
            {
                progressWeight.Add(1);
            }
        }

        foreach (var p in progressWeight)
        {
            totalProgress += p * 100;
        }

        this.Handles = handles;
        this.progressWeight = progressWeight;
    }

    public override bool keepWaiting => !AllDone();

    public bool AllDone()
    {
        bool done = true;
        foreach (var h in Handles)
        {
            if (!h.IsDone)
            {
                done = false;
                break;
            }
        }
        return done;
    }

    //progress. 0 to 100
    public float PercentageComplete
    {
        get
        {
            float progress = 0;
            for (int i = 0; i < Handles.Count; ++i)
            {
                progress += Handles[i].PercentComplete * progressWeight[i];
            }
            return progress / totalProgress;
        }
    }

    public List<T> Results
    {
        get
        {
            return Handles.Select((x) => { return x.Result; }).ToList();
        }
    }
}


public class BatchYieldInstruction : CustomYieldInstruction
{
    public List<AsyncOperationHandle> Handles { get; private set; }
    protected List<float> progressWeight;
    protected float totalProgress;

    public BatchYieldInstruction(List<AsyncOperationHandle> handles, List<float> progressWeight = null)
    {
        Debug.Assert(progressWeight == null || progressWeight.Count == handles.Count);
        if (progressWeight == null)
        {
            progressWeight = new List<float>();
            for (int i = 0; i < handles.Count; ++i)
            {
                progressWeight.Add(1);
            }
        }

        foreach (var p in progressWeight)
        {
            totalProgress += p;
        }

        this.Handles = handles;
        this.progressWeight = progressWeight;
    }

    public void SetProgressWeights(List<float> progressWeight)
    {
        Debug.Assert(progressWeight != null && progressWeight.Count == Handles.Count);

        this.progressWeight = progressWeight;

        totalProgress = 0;
        foreach (var p in progressWeight)
        {
            totalProgress += p;
        }
    }

    public override bool keepWaiting => !AllDone();

    public bool AllDone()
    {
        bool done = true;
        foreach (var h in Handles)
        {
            if (!h.IsDone)
            {
                done = false;
                break;
            }
        }
        return done;
    }

    /// <summary>
    /// Progress. 0~1
    /// </summary>
    public float PercentageComplete
    {
        get
        {
            float progress = 0;
            for (int i = 0; i < Handles.Count; ++i)
            {
                progress += Handles[i].PercentComplete * progressWeight[i];
            }
            return progress / totalProgress;
        }
    }
    
}