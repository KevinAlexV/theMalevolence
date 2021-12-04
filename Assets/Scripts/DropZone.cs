using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event Draggable.DragHandler onDrop;
    public bool hovering = false;

    public void OnPointerEnter(PointerEventData data){
        hovering = true;
    }
    public void OnPointerExit(PointerEventData data){
        hovering = false;
    }

    public void Update(){
        if(hovering && Draggable.dragTarget != null && Input.GetMouseButtonUp(0)){
            Debug.Log($"<color=purple>DropZone.cs</color>: {Draggable.dragTarget.name} dropped in {name}");

            if(onDrop != null){
                onDrop(Draggable.dragTarget, this);
            }
            //If the draggable is a valid option for the drop zone, there should be a handler subscribed to onDrop that calls the code below
            //drag.Drop(drop);
            
        }
    }
}
