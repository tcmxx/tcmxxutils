using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Camera2DController : MonoBehaviour {

    public AnimationCurve defaultBlendCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public Camera Camera => cam == null ? cam = GetComponent<Camera>() : cam;
    private Camera cam;

    private VirtualCamera2DPose blendStartPose;

    public IVirtualCamera2D CurrentVirtualCamera => cameraStack.Count > 0 ? cameraStack[^1] : null;

    public float CurrentBlendingProgress {
        get {
            if (blendTotalTime <= 0) {
                return 1;
            }

            return Mathf.Clamp01(1 - blendCountDownTimer / blendTotalTime);
        }
    }

    private float blendCountDownTimer = 0;
    private float blendTotalTime = 0;

    private readonly List<IVirtualCamera2D> cameraStack = new List<IVirtualCamera2D>();

    // Update is called once per frame
    void Update() {
        UpdateBlending(Time.deltaTime);
        UpdateCameraForPose();
    }

    private void UpdateBlending(float deltaTime) {
        blendCountDownTimer -= deltaTime;
        blendCountDownTimer = Mathf.Max(blendCountDownTimer, 0);
    }

    private void UpdateCameraForPose() {
        var currentPose = GetCurrentPose();
        currentPose.ApplyToCamera(Camera);
    }

    private void StartBlendFromCurrent(float blendTime) {
        blendStartPose = GetCurrentPose();
        blendCountDownTimer = blendTime;
        blendTotalTime = blendTime;
    }

    public void Clear() {
        cameraStack.Clear();
    }

    public void PushCamera(IVirtualCamera2D newCamera, float blendTime) {
        StartBlendFromCurrent(blendTime);

        cameraStack.Add(newCamera);
    }

    public void PopCamera(float blendTime) {
        StartBlendFromCurrent(blendTime);

        cameraStack.RemoveAt(cameraStack.Count - 1);
    }

    public void PopOrRemoveCameraCamera(IVirtualCamera2D cameraToRemove, float blendTime) {
        StartBlendFromCurrent(blendTime);

        cameraStack.Remove(cameraToRemove);
    }

    public void ChangeCamera(IVirtualCamera2D newCamera, float blendTime) {
        StartBlendFromCurrent(blendTime);

        cameraStack[^1] = newCamera;
    }

    public void ChangeOrPushCamera(IVirtualCamera2D newCamera, float blendTime) {
        StartBlendFromCurrent(blendTime);

        if (cameraStack.Count > 0) {
            cameraStack[^1] = newCamera;
        } else {
            cameraStack.Add(newCamera);
        }
    }

    public bool ChangeCamera(IVirtualCamera2D oldCamera, IVirtualCamera2D newCamera, float blendTime) {
        for (var i = 0; i < cameraStack.Count; ++i) {
            if (cameraStack[i] == oldCamera) {
                StartBlendFromCurrent(blendTime);
                cameraStack[i] = newCamera;
                return true;
            }
        }

        return false;
    }

    public VirtualCamera2DPose GetCurrentPose() {
        if (CurrentVirtualCamera == null) {
            return new VirtualCamera2DPose();
        }

        if (CurrentBlendingProgress >= 1) {
            return CurrentVirtualCamera.GetPose();
        } else {

            return VirtualCamera2DPose.Lerp(blendStartPose, CurrentVirtualCamera.GetPose(), defaultBlendCurve.Evaluate(CurrentBlendingProgress));
        }
    }
}