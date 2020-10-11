using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSpectrum))]
public class SpectrumAnimationTrigger : MonoBehaviour {
    public bool spatialModified = true;
    public Transform listenerTrans;
    public SpriteRenderer spriteRemderer;
    public Transform transformToAnim;
    public SpectrumIntensityAnimation[] spectrumAnimations;
    public ParameterSet[] animationParameters;

    private AudioSpectrum spectrum;
    private ChangeRecord[] intensityAnimRecord;
    private Color initialColor;
    //private Vector3 initialScale;
    private AudioSource audioSource;
    private AnimationCurve volumeCurve;

    private Animator animator;

    public enum BandMode
    {
        Band8,
        Band512
    }
    [Serializable]
    public struct SpectrumIntensityAnimation
    {
        public BandMode bandMode;
        public int[] bands;
        public float[] weights;
        public float scale;
        public float max;
        public float offset;
        public int animParametersIndex;
        public UnityEngine.Events.UnityAction<float> events;
    }

    private struct ChangeRecord
    {
        public Vector3 deltaScale;
        public Color deltaColor;
    }

    [Serializable]
    public struct ParameterSet
    {
        public Vector3 paramScale;
        public Color paramColor;
    }

    private void Awake()
    {
        spectrum = GetComponent<AudioSpectrum>();
        intensityAnimRecord = new ChangeRecord[spectrumAnimations.Length];
        if (spriteRemderer)
        {
            initialColor = spriteRemderer.color;
            //initialScale = transformToAnim.localScale;
        }
        audioSource = GetComponent<AudioSource>();
        volumeCurve = audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
        if(animator && !animator.enabled)
            UpdateIntensityAnim();

    }

    private void UpdateIntensityAnim()
    {
        float spatialModify = 1.0f;
        if (spatialModified)
        {
            spatialModify = Mathf.Max(SpatialAudioVolume(),0.01f);
        }
        for(int i = 0; i < spectrumAnimations.Length; ++i)
        {
            SpectrumIntensityAnimation anim = spectrumAnimations[i];
            float value = CalculateSpectrum(anim.bands, anim.weights, anim.scale/ spatialModify, anim.max, anim.offset, anim.bandMode);

            if (spriteRemderer)
            {
                Color deltaColorNew = animationParameters[anim.animParametersIndex].paramColor - initialColor;
                deltaColorNew = deltaColorNew * value / anim.max;
                spriteRemderer.color = spriteRemderer.color - intensityAnimRecord[i].deltaColor + deltaColorNew;
                intensityAnimRecord[i].deltaColor = deltaColorNew;
            }
            if (transformToAnim)
            {
                Vector3 deltaScaleNew = animationParameters[anim.animParametersIndex].paramScale * value / anim.max;
                transformToAnim.localScale = transformToAnim.localScale - intensityAnimRecord[i].deltaScale + deltaScaleNew;
                intensityAnimRecord[i].deltaScale = deltaScaleNew;
            }

        }
        
    }
    

    private float CalculateSpectrum(int[] bands, float[] weights,float scale, float max, float offset, BandMode bandMode)
    {
        float value = 0;
        for(int i = 0; i < bands.Length; ++i)
        {
            if(bandMode == BandMode.Band8)
                value += spectrum.FrequencyBand8Ave[bands[i]] * (weights.Length>i?weights[i]:1);
            else if(bandMode == BandMode.Band512)
            {
                int bandNum = bands[i];
                float steroClamped = Mathf.Clamp01(spectrum.stero);
                value += ((1- steroClamped) *spectrum.SamplesLeft[bandNum] + steroClamped * spectrum.SamplesRight[bandNum]) * (weights.Length > i ? weights[i] : 1);
            }
        }
        value = value * scale - offset;

        value = Mathf.Max(0,Mathf.Min(value, max));

        return value;
    }


    private float SpatialAudioVolume()
    {
        return volumeCurve.Evaluate(Mathf.Clamp01(Vector3.Distance(listenerTrans.position, transform.position) / audioSource.maxDistance));
    }

}
