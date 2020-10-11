using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// partialy adapted from the tutorial https://www.youtube.com/watch?v=mHk3ZiKNH48
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioSpectrum : MonoBehaviour {

    private AudioSource audioSource;

    //samples get directly from the spectrum data. Must be a power of 2. Min = 64. Max = 8192
    private readonly int TotalNumOfSample = 512;
    public int AverageHistoryCount = 10;

    public float[] SamplesLeft { get; private set; }
    public float[] SamplesRight { get; private set; }

    [Range(0, 1)]
    public float stero = 0.5f;
    
    public float[] FrequencyBand8 { get; private set; }
    private float[,] frequencyBandHistory;
    private int frequencyBandHistoryPointer = 0;

    public float[] FrequencyBand8Buffered { get; private set; }
    public float bufferDownRate = 0.1f;
    public float[] FrequencyBand8Deri { get; private set; }
    public float[] FrequencyBand8Ave { get; private set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        SamplesLeft = new float[TotalNumOfSample];
        SamplesRight = new float[TotalNumOfSample];
        FrequencyBand8 = new float[8];
        FrequencyBand8Buffered = new float[8];
        FrequencyBand8Deri = new float[8];
        FrequencyBand8Ave =  new float[8];
        frequencyBandHistory = new float[8,AverageHistoryCount];
        frequencyBandHistoryPointer = 0;

        
    }

    // Use this for initialization
    void Start () {

    }
	




	// Update is called once per frame
	void Update () {
    }

    private void FixedUpdate()
    {
        GetSamples();
        UpdateFrequencyBand8();
    }


    private void GetSamples()
    {
        audioSource.GetSpectrumData(SamplesLeft, 0, FFTWindow.Blackman);
        audioSource.GetSpectrumData(SamplesRight, 1, FFTWindow.Blackman);
    }

    private void UpdateFrequencyBand8()
    {
        float steroClamped = Mathf.Clamp01(stero);
        int count = 0;
        for (int i = 0; i < 8; ++i)
        {
            int thisBandSampleCount = (int)Mathf.Pow(2, i + 1);
            float sum = 0;
            if(i == 7)
            {
                thisBandSampleCount += 2;
            }

            for(int j = 0; j < thisBandSampleCount; ++j)
            {
                sum += ((1 - steroClamped) * SamplesLeft[count] + (steroClamped) * SamplesRight[count]);
                count++;
            }

            FrequencyBand8[i] = sum;// / thisBandSampleCount;
            FrequencyBand8Ave[i] = FrequencyBand8Ave[i] + (FrequencyBand8[i] - frequencyBandHistory[i, frequencyBandHistoryPointer]) / AverageHistoryCount;
            frequencyBandHistory[i, frequencyBandHistoryPointer] = FrequencyBand8[i];


            FrequencyBand8Deri[i] = FrequencyBand8[i] - frequencyBandHistory[i, (frequencyBandHistoryPointer + AverageHistoryCount-1)% AverageHistoryCount];

            if (FrequencyBand8[i] > FrequencyBand8Buffered[i])
            {
                FrequencyBand8Buffered[i] = FrequencyBand8[i];
            }
            else
            {
                FrequencyBand8Buffered[i] = Mathf.Max(FrequencyBand8[i], FrequencyBand8Buffered[i] - bufferDownRate * Time.fixedDeltaTime);
            }

            
        }

        frequencyBandHistoryPointer = (frequencyBandHistoryPointer + 1)% AverageHistoryCount;
    }
    

}
