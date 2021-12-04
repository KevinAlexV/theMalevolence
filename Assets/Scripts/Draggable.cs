using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public delegate void DragHandler(Draggable drag, DropZone drop);
    public event DragHandler onDragStart;
    public event DragHandler onDrag;
    public event DragHandler onDragStop;
    public static Draggable dragTarget;
    public bool dragging = false;
    [Tooltip("Will return to draggable object to it's previous position if it did not land on a valid drop zone")]
    public bool returnIfNotDropped = true;
    private Vector3 returnPos;
    [Tooltip("If True, the object will follow the mouse while being dragged. If false, some other code must handle the movement using the draghandler events")]
    public bool followMouse = true;
    [Tooltip("If true, can only be dragged while in the planning phase")]
    public bool planningPhaseOnly = false;
    private bool letGo = false;
    public DropZone zone;
    public DropZone returnDropZone;
    public void OnPointerDown(PointerEventData data){
        if(planningPhaseOnly && GameManager.manager != null && GameManager.manager.phase != Enums.GameplayPhase.Planning) return;
        dragTarget = this;
        dragging = true;

        if(onDragStart != null){
            onDragStart(this, null);
        }

        returnPos = this.transform.position;
        GetComponent<GraphicRaycaster>().enabled = false;
        transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<RectTransform>());
    }

    public void OnPointerUp(PointerEventData data){
        if(planningPhaseOnly && GameManager.manager != null && GameManager.manager.phase != Enums.GameplayPhase.Planning) return;

        dragging = false;
        letGo = true;
        GetComponent<GraphicRaycaster>().enabled = true;

    }

    public void Drop(DropZone zoneWhereDropped){
        letGo = false;
        dragTarget = null;

        if (zoneWhereDropped != null){
            zone = zoneWhereDropped;
            returnDropZone = zone;
            transform.SetParent(zoneWhereDropped.GetComponent<RectTransform>());
        } else if(returnIfNotDropped) {
            transform.SetParent(returnDropZone.GetComponent<RectTransform>());
            transform.position = returnPos;
        }

        if (onDragStop != null){
            onDragStop(this, zone);
        }
    }

    public void LateUpdate(){

        if (dragging){

            if (followMouse) transform.position = Input.mousePosition;
            if(onDrag != null){
                onDrag(this, zone);
            }
        }
        else if(letGo) {
            Drop(null);
        }
    }

    public void ClearHandlers(){
        onDragStart = null;
        onDragStop = null;
        onDrag = null;
    }
}
