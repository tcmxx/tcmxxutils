
using System;
using UnityEngine.Profiling;
using System.Diagnostics;
using UnityEngine;

public struct ProfileWatch : IDisposable
{
    public ProfileWatch(string name)
    {
        Profiler.BeginSample(name);
    }

    public void Dispose()
    {
        Profiler.EndSample();
    }
}

public struct TimeWatch : IDisposable
{
    private string name;
    private Stopwatch timer;
    private int startFrameCount;

    public TimeWatch(string name)
    {
        this.name = name;
        this.timer = new Stopwatch();
        this.timer.Start();
        startFrameCount = Time.frameCount;
    }

    public void Dispose()
    {
        timer.Stop();
        var framesElapsed = Time.frameCount - startFrameCount;
        UnityEngine.Debug.Log("TimeWatch " + name + " took " + timer.ElapsedMilliseconds + " ms, " + framesElapsed + " frames");
    }
}