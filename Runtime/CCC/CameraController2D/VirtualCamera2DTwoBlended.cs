using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCamera2DTwoBlended : VirtualCamera2DBase {
    public VirtualCamera2DBase camera1;
    public VirtualCamera2DBase camera2;
    public float t = 0.5f;

    public override VirtualCamera2DPose GetPose() {
        return VirtualCamera2DPose.Lerp(camera1.GetPose(), camera2.GetPose(), t);
    }
}