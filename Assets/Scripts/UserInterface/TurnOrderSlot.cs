using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DropZone))]
public class TurnOrderSlot : MonoBehaviour
{
    public DropZone dropZone;
    public Draggable currentTurnDraggable;
    public ITurnExecutable Turn{
        get{
            return currentTurnDraggable.GetComponent<CharacterDisplayController>().Character;
        }
    }

    //Place the CharacterDisplayController into this turn slot
    public void PlaceTurn(CharacterDisplayController display){
        currentTurnDraggable = display.GetComponent<Draggable>();
        display.currentTurnSlot = this;
        display.transform.position = transform.position;
    }
    
    
    void Start()
    {
        dropZone = GetComponent<DropZone>();
        dropZone.onDrop += (drag, drop) =>{
            if(drag == currentTurnDraggable) return;
            CharacterDisplayController characterDisplay = null;
            if(drag.TryGetComponent(out characterDisplay)){
                drag.Drop(drop);
                //Drop this slot's turn in the new turn's slot
                characterDisplay.currentTurnSlot.PlaceTurn(currentTurnDraggable.GetComponent<CharacterDisplayController>());
                //Drop new turn into this slot
                PlaceTurn(characterDisplay);
            }
       };
    }
}
