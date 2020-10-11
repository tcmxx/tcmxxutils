using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LightIntensityChangeRepeat : MonoBehaviour {


    public AnimationCurve changeCurve;
    public float cyclingTime;
    protected Light mLight;
    //protected AreaLight aLight;
    //protected TubeLight tLight;

    protected float currentTime = 0;
    private void Awake()
    {
        mLight = GetComponent<Light>();
        //aLight = GetComponent<AreaLight>();
        //tLight = GetComponent<TubeLight>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        currentTime += Time.deltaTime;
        if(currentTime >= cyclingTime)
        {
            currentTime = currentTime - cyclingTime;
        }
        if(mLight)
            mLight.intensity = changeCurve.Evaluate(Mathf.Clamp01(currentTime / cyclingTime));
        //if(aLight)
        //    aLight.m_Intensity = changeCurve.Evaluate(Mathf.Clamp01(currentTime / cyclingTime));
       // if(tLight)
        //    tLight.m_Intensity = changeCurve.Evaluate(Mathf.Clamp01(currentTime / cyclingTime));
    }



}
