using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DraggableButton :MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
 
    public ScrollRect ScrollerRef { get; set; }

    public event Action<PointerEventData> onBeginDrag;
    public event Action<PointerEventData> onEndDrag;
    public event Action<PointerEventData> onMoving;

    public RectTransform uiPanelRef;
    public bool passHorizontalDragEvent = true;
    protected enum Status
    {
        Default,
        Moving,
        Placing,
        Placed,
        Confirmed
    }
    protected Status status = Status.Default;
    protected Image imageRef;


    #region drag listeners

    public void OnBeginDrag(PointerEventData eventData)
    {
        PointerEventData e = eventData;
        if (passHorizontalDragEvent && Mathf.Abs(e.delta.y) < Mathf.Abs(e.delta.x))
        {
            if (ScrollerRef != null)
                ScrollerRef.SendMessage("OnBeginDrag", eventData);
            return;
        }

        onBeginDrag?.Invoke(eventData);
        if (status == Status.Default)
        {
            TransDefaultToMoving();
        }
        else if (status == Status.Placed)
        {
            TransPlacedToPlacing();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (status == Status.Default)
        {
            if (ScrollerRef != null)
                ScrollerRef.SendMessage("OnDrag", eventData);
            return;
        }
        else if (status == Status.Moving)
        {
            onMoving?.Invoke(eventData);
            if (uiPanelRef == null || RectTransformUtility.RectangleContainsScreenPoint(uiPanelRef, eventData.position))
            {
                transform.position = eventData.position;
            }
            else
            {
                TransMovingToPlacing();
            }
        }
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        if (status == Status.Default)
        {
            if (ScrollerRef != null)
                ScrollerRef.SendMessage("OnEndDrag", eventData);
        }
        else
        {
            onEndDrag?.Invoke(eventData);
            if (status == Status.Moving)
                TransMovingToDefault();
            else if (status == Status.Placing)
                TransPlacingToPlaced();
        }
    }
    
    #endregion

    #region state transition functions

    protected void TransMovingToPlacing()
    {
        status = Status.Placing;
    }

    protected void TransDefaultToMoving()
    {
        status = Status.Moving;
    }
    protected void TransMovingToDefault()
    {
        status = Status.Default;
        transform.localPosition = Vector3.zero;
    }

    protected void TransPlacingToPlaced()
    {
        status = Status.Placed;
    }
    protected void TransPlacedToPlacing()
    {
        status = Status.Placing;
    }

    public void TransToDefault()
    {
        status = Status.Default;
        transform.localPosition = Vector3.zero;
        imageRef.color = new Color(1, 1, 1, 1);
    }

    #endregion
    
}