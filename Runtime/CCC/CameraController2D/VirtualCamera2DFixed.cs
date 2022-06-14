using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCamera2DFixed : VirtualCamera2DBase {
    public float cameraSize = 6;

    public override VirtualCamera2DPose GetPose() {
        var tempTransform = transform;
        return new VirtualCamera2DPose {
            position = tempTransform.position,
            rotation = tempTransform.rotation,
            size = cameraSize
        };
    }
}