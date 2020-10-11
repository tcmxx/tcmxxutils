using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;



public class SyncedValueFloat : SyncedValue<float>
{
    protected override float Add(float one, float other)
    {
        return one + other;
    }

    protected override float Divide(float value, double t)
    {
        return (float)(value / t);
    }

    protected override float Lerp(float start, float end, float amount)
    {
        return Mathf.Lerp(start, end, amount);
    }

    protected override float Multiply(float value, float t)
    {
        return value * t;
    }

    protected override float Norm(float value)
    {
        return value*value;
    }

    protected override float Normalize(float value)
    {
        return 1;
    }

    protected override float Substract(float one, float other)
    {
        return one - other;
    }
}

public abstract class SyncedValue<T>
{
    public double extrapolationLimit = 0.5; //max allowed extrpolation  time
    public double delay = 0.05;

    public bool checkForSpeedHack = false;
    public float maxSpeedDeltaSqr = 9;

    public int HistorySize { get; private set; } = 20;
    public T Value { get; set; }
    
    private State[] proxyStates;
    private int latestProxyIndex = 0;
    private int PreviousProxyIndex { get { return (latestProxyIndex + proxyStates.Length - 1) % proxyStates.Length; } }
    private int proxyStateCount;


    private T prevValue;
    private T deltaValue;
    private double prevServerTime;

    /// <summary>
    /// Synchronized object state
    /// </summary>
    public struct State
    {
        public double timestamp;
        public T value;
        public T velocity;
    }


    public SyncedValue(int historySize = 20)
    {
        HistorySize = historySize;
        proxyStates = new State[historySize];
        prevServerTime = 0;
    }



    public State ServerUpdate(T value, double serverTime)
    {
        Debug.Assert(serverTime > prevServerTime, "server time is lower than the previous server time!");
        prevValue = Value;
        Value = value;
        double deltaTime = serverTime - prevServerTime;
        prevServerTime = serverTime;

        T velocity = Divide((Substract(Value,prevValue)) , deltaTime);

        State result = new State();
        result.value = value;
        result.velocity = velocity;
        result.timestamp = serverTime;
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="package"> data package</param>
    public void ClientReceive(State package)
    {
        Debug.Assert(package.timestamp > proxyStates[latestProxyIndex].timestamp, "Received package from older timestamp!");
        // Check for speed hacks
        if (checkForSpeedHack && proxyStates.Length > 0)
        {
            var delta = Substract(proxyStates[latestProxyIndex].value, package.value);
            if (Norm(delta) > maxSpeedDeltaSqr)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Speed hack detected. Throttling velocity of " + Norm(delta));
                package.value = Add(proxyStates[latestProxyIndex].value, Multiply(Normalize(delta),Mathf.Sqrt(maxSpeedDeltaSqr)));
#endif
            }
        }


        latestProxyIndex++; //shift the index
        latestProxyIndex = latestProxyIndex % proxyStates.Length;

        // Record current state in latest slot
        proxyStates[latestProxyIndex] = package;

        // Update used slot count, however never exceed the buffer size
        // Slots aren't actually freed so this just makes sure the buffer is
        // filled up and that uninitalized slots aren't used.
        proxyStateCount = Mathf.Min(proxyStateCount + 1, proxyStates.Length);

        // Check if states are in order
        if (proxyStates[latestProxyIndex].timestamp < proxyStates[PreviousProxyIndex].timestamp)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Timestamp inconsistent: " + proxyStates[latestProxyIndex].timestamp + " should be greater than " + proxyStates[PreviousProxyIndex].timestamp);
#endif
        }
    }

    /// <summary>
    /// get the the value for current timestep. Note that the final value is intepreted for the timestamp at currentTimeStep-delay.
    /// </summary>
    /// <param name="currentTimeStep"></param>
    public T ClientUpdateValueFor(double currentTimeStep)
    {
        // You are not the owner, so you have to converge the object's state toward the server's state.
        // Entity interpolation happens here; see https://developer.valvesoftware.com/wiki/Source_Multiplayer_Networking

        // This is the target playback time of this body
        double interpolationTime = currentTimeStep - delay;

        // Use interpolation if the target playback time is present in the buffer
        if (proxyStates[latestProxyIndex].timestamp > interpolationTime)
        {
            // Go through buffer and find correct state to play back
            for (int i = 0; i < proxyStateCount; i++)
            {
                int index = ((latestProxyIndex - i) + proxyStates.Length) % proxyStates.Length;
                if (proxyStates[index].timestamp <= interpolationTime || i == proxyStateCount - 1)
                {
                    // The state one slot newer (<100ms) than the best playback state
                    State rhs = proxyStates[(index + 1) % proxyStates.Length];
                    // The best playback state (closest to 100 ms old (default time))
                    State lhs = proxyStates[index];

                    // Use the time between the two slots to determine if interpolation is necessary
                    double length = rhs.timestamp - lhs.timestamp;
                    float t = 0.0F;
                    // As the time difference gets closer to 100 ms t gets closer to 1 in 
                    // which case rhs is only used
                    // Example:
                    // Time is 10.000, so sampleTime is 9.900 
                    // lhs.time is 9.910 rhs.time is 9.980 length is 0.070
                    // t is 9.900 - 9.910 / 0.070 = 0.14. So it uses 14% of rhs, 86% of lhs
                    if (length > 0.0001)
                        t = (float)((interpolationTime - lhs.timestamp) / length);

                    // set the value with lerping
                    Value = Lerp(lhs.value, rhs.value, t);
                    break;
                }
            }
        }
        // Use extrapolation
        else
        {
            State latest = proxyStates[latestProxyIndex];

            float extrapolationLength = (float)(interpolationTime - latest.timestamp);
            // Don't extrapolation for more than 500 ms, you would need to do that carefully
            if (extrapolationLength < extrapolationLimit)
            {
                Value = Add(latest.value, Multiply(latest.velocity,extrapolationLength));
            }
            else
            {
                Value = proxyStates[latestProxyIndex].value;
            }
        }

        return Value;
    }

    
    protected abstract float Norm(T value);
    protected abstract T Add(T one, T other);
    protected abstract T Substract(T one, T other);
    protected abstract T Lerp(T start, T end, float amount);
    protected abstract T Normalize(T value);
    protected abstract T Multiply(T value, float t);
    protected abstract T Divide(T value, double t);
}
