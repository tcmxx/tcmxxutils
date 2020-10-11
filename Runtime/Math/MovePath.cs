using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class MovePath : MonoBehaviour {

    public GameObject autoMovingObject = null;

    public List<Transform> paths;

    public UnityEvent onPathFinished;
    public bool autoTurning = true;
    public int steps = 100;
    public bool looping = true;
    public List<Curves.CurvePoint> points = new List<Curves.CurvePoint>();

    [SerializeField]
    private float moveTime;
    [SerializeField]
    private float moveTimer;
    private Transform transformToMove = null;
    public Transform moveTransformDir = null;


    public enum PathMode
    {
        Direct,
        Bezier,
        Bspline,
        CRspline
    }

    public PathMode pathMode = PathMode.Bezier;


	// Use this for initialization
	void Start () {
        if(autoMovingObject != null)
            StartMove(autoMovingObject, moveTime);
        EvaluatePath();
    }
	
	// Update is called once per frame
	void Update () {
        
        if (moveTimer > 0 && transformToMove != null)
        {
            moveTimer -=  Time.deltaTime;
            float t = (moveTime - moveTimer) / moveTime;
            transformToMove.position = Curves.LerpTranslation(points, t);
            if (autoTurning)
            {
                if(moveTransformDir != null)
                {
                    transformToMove.localRotation = Curves.LerpOrientation(points, t)* moveTransformDir.rotation;
                }
                else
                {
                    transformToMove.localRotation = Curves.LerpOrientation(points, t);
                }
                
            }
            if(moveTimer <= 0 && looping)
            {
                ResetMove();
            }
        }
	}

    public void EvaluatePath()
    {
        List<Vector3> temppoints = new List<Vector3>();
        for(int i = 0; i < paths.Count;++i)
        {
            temppoints.Add(paths[i].position);
        }
        if(pathMode == PathMode.Bezier)
        {
            points = Curves.EvalBezier(temppoints, steps);
        }else if(pathMode == PathMode.Bspline)
        {
            points = Curves.EvalBspline(temppoints, steps, true);
        }else if(pathMode == PathMode.CRspline)
        {
            points = Curves.EvalCRspline(temppoints, steps);
        }
    }


    public void StartMove(GameObject obj, float time)
    {
        moveTimer = time;
        moveTime = time;
        transformToMove = obj.transform;
    }


    public void ResetMove()
    {
        moveTimer = moveTime;
    }



    private void OnDrawGizmos()
    {
        EvaluatePath();
        for (int i = 0; i < points.Count-1; ++i)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(points[i].V, points[i + 1].V);
        }
        foreach(var t in paths)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(t.position, 0.2f);
        }
    }
}
