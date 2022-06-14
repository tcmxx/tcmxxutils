using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraSimple2DFollowing : VirtualCamera2DBase {
    public float maxLerpFactor = 10f;
    public Vector3 offset;
    public Vector2 upDirection = Vector2.up;
    public float size = 6;

    private VirtualCamera2DPose? followedPose;

    private void FixedUpdate() {

        if (!followedPose.HasValue) {
            followedPose = GetTargetPose();
            return;
        }

        var t = 1 - Mathf.Exp(-maxLerpFactor * Time.fixedDeltaTime);
        followedPose = VirtualCamera2DPose.Lerp(followedPose.Value, GetTargetPose(), t);
    }

    public VirtualCamera2DPose GetTargetPose() {
        var tempTransform = transform;
        return new VirtualCamera2DPose {
            position = tempTransform.position + offset,
            rotation = Quaternion.LookRotation(Vector3.forward, upDirection),
            size = size
        };
    }

    public override VirtualCamera2DPose GetPose() {
        return followedPose ?? GetTargetPose();
    }
}