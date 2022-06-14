using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VirtualCamera2DPose {
    public Vector2 position;
    public Quaternion rotation;
    public float size;

    public void ApplyToCamera(Camera camera) {
        camera.transform.SetPositionAndRotation(new Vector3(position.x, position.y, camera.transform.position.z), rotation);
        camera.orthographicSize = size;
    }

    public static VirtualCamera2DPose Lerp(VirtualCamera2DPose start, VirtualCamera2DPose end, float t) {

        return new VirtualCamera2DPose() {
            position = Vector2.Lerp(start.position, end.position, t),
            rotation = Quaternion.Lerp(start.rotation, end.rotation, t),
            size = Mathf.Lerp(start.size, end.size, t)
        };
    }
}

public interface IVirtualCamera2D {
    VirtualCamera2DPose GetPose();
}