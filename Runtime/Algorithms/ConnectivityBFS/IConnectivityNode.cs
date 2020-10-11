using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConnectivityNode  {
    List<IConnectivityNode> GetNeighbors();

    bool IsAvailable();

    //the property of each node will be bitwise and, and the final result is the property of the group.
    uint GetAndPropertyBits();
}
