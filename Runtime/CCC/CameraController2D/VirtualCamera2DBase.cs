using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VirtualCamera2DBase : MonoBehaviour, IVirtualCamera2D {

    public abstract VirtualCamera2DPose GetPose();
}
