using UnityEngine;
using System.Collections;
using System;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour
{

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;

    public Transform xTransform;
    public Transform yTransform;

    public Transform test;

    void Update()
    {

    }

    void Start()
    {
        //SmoothMoveRotTo(test.forward, 10, 3);
        rotationY = -yTransform.localEulerAngles.x;
    }


    public void Rotate(float mouseAxisX, float mouseAxisY)
    {
        if (axes == RotationAxes.MouseXAndY)
        {
            xTransform.Rotate(0, mouseAxisX * sensitivityX, 0);

            rotationY += mouseAxisY * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            yTransform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            xTransform.Rotate(0, mouseAxisX * sensitivityX, 0);
        }
        else
        {
            rotationY += mouseAxisY * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            yTransform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
    }



    public void SmoothMoveRotTo(Vector3 front, float time, float maxSpeed, Action onMoveDone = null)
    {
        StartCoroutine(MoveRotTo(front, time, maxSpeed, onMoveDone));
    }

    IEnumerator MoveRotTo(Vector3 front, float time, float maxSpeed, Action onMoveDone)
    {
        yield return null;
        float t = 0;
        while(t < time)
        {
            t += Time.deltaTime;
            RotationTowards(front, maxSpeed);
            yield return null;
        }
        if(onMoveDone != null)
        {
            onMoveDone.Invoke();
        }
    }

    public void RotationTowards(Vector3 front, float maxDeltaAngle)
    {
        var temp = front;
        front.y = 0;
        front.Normalize();
        xTransform.rotation = Quaternion.RotateTowards(xTransform.rotation, Quaternion.LookRotation(front, Vector3.up), maxDeltaAngle);

        Vector3 localEulerAngles = yTransform.localEulerAngles;
        float desiredYAngle = Mathf.Atan2(temp.y, Mathf.Sqrt(temp.x * temp.x + temp.z * temp.z))*Mathf.Rad2Deg;
        if(desiredYAngle - rotationY >= 180)
        {
            desiredYAngle -= 360;
        }else if (desiredYAngle - rotationY <= -180)
        {
            desiredYAngle += 360;
        }

        rotationY += Mathf.Clamp(maxDeltaAngle,0,Mathf.Abs(desiredYAngle - rotationY)) * Mathf.Sign(desiredYAngle - rotationY);
        yTransform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
    }


    public void RotateLookFront(float maxSpeed = 1)
    {
        Vector3 dir = transform.forward;
        dir.y = 0;

        SmoothMoveRotTo(dir.normalized, 5, maxSpeed, null);
    }
}