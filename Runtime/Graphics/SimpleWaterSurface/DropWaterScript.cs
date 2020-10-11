using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWaterScript : MonoBehaviour {

    private Material mat;
    
    public float runningSpeed = 2;
    public int maxColNum = 3;
    public float depthMultiplier = 1;
    private int currentColInd = 0;
    public float sameObjectMinInterval = 2;
    [SerializeField]
    private float[] timers;
    private float[] depths;
    private Vector2[] dropPositions;

    private Dictionary<Rigidbody, float> forbiddenRbTimer;
    // Use this for initialization
    void Start () {
        mat = GetComponent<MeshRenderer>().material;
        timers = new float[maxColNum];
        depths = new float[maxColNum];
        forbiddenRbTimer = new Dictionary<Rigidbody, float>();
        for (int i = 0; i < maxColNum; ++i)
        {
            timers[i] = 1000;
            depths[i] = 0;
        }
        dropPositions = new Vector2[maxColNum];
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < maxColNum; ++i)
        {
            Waving(i);
        }

        List<Rigidbody> keysToRemove = new List<Rigidbody>(forbiddenRbTimer.Keys);
        foreach(var r in keysToRemove)
        {
            forbiddenRbTimer[r] -= Time.deltaTime;
            if(forbiddenRbTimer[r] <= 0)
            {
                forbiddenRbTimer.Remove(r);
            }
        }
	}

    private void Waving(int num)
    {
        mat.SetVector("_Collision" + (num+1).ToString(), new Vector4(dropPositions[num].x, dropPositions[num].y, depths[num], timers[num] * runningSpeed));
        timers[num] += Time.deltaTime;
    }

    public void DropWater(Vector2 position, float depth)
    {
       
        currentColInd = (currentColInd + 1) % maxColNum;
        timers[currentColInd] = 0;
        depths[currentColInd] = depth;
        dropPositions[currentColInd] = position;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody && !forbiddenRbTimer.ContainsKey(other.attachedRigidbody))
        {
            float dep = -other.attachedRigidbody.velocity.y * depthMultiplier;
            Vector3 localPos = transform.InverseTransformPoint(other.gameObject.transform.position);
            DropWater(new Vector2(localPos.x, localPos.z), dep);
            forbiddenRbTimer[other.attachedRigidbody] = sameObjectMinInterval;
        }
    }


}
