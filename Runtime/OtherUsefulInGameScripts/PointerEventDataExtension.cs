using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class PointerEventDataExtension 
{
    public static Vector3 PointerWorldPosition(this PointerEventData eventData, float z = 0)
    {
        var camera = eventData.enterEventCamera;
        if (camera == null)
            camera = eventData.pressEventCamera;
        Vector3 newPos = camera.ScreenToWorldPoint(eventData.position);
        newPos.z = z;
        return newPos;
    }

    public static Vector3 PointerWorldDelta(this PointerEventData eventData, float z = 0)
    {
        var camera = eventData.enterEventCamera;
        if (camera == null)
            camera = eventData.pressEventCamera;

        Vector3 newPos = camera.ScreenToWorldPoint(eventData.position);
        Vector3 oldPos = camera.ScreenToWorldPoint(eventData.position - eventData.delta);
        var result = newPos - oldPos;
        result.z = z;
        return result;
    }
}
