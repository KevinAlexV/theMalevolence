using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Character))]
public class Targetable : MonoBehaviour, IPointerClickHandler
{
    private static bool targetting = false;
    private static Enums.TargetType targetType;
    public static List<ITargetable> currentTargets = new List<ITargetable>();
    public static Character targetSource;

    [Tooltip("List of target types that apply to this gameobject")]
    public List<Enums.TargetType> targetTypes;
    public ITargetable target; //Assume characters are the only targetable entities

    public void Start(){
        target = GetComponent<Character>();
    }

    public void OnPointerClick(PointerEventData data){ //Camera needs to have the PhysicsRaycast Component
        if(targetting && targetTypes.Contains(targetType) && ((Character)target).Defeated == false)
        {
            currentTargets.Add(((Character)target).Targeted(targetSource));//May want to change so that a target already in the list cannot be added a second time
            Debug.Log($"<color=Cyan>Target assigned:</color> {name} has been targeted");
            ;
        }
    }

    public static void highlightTargets()
    {
        
        foreach (Character c in GameManager.manager.party)
        {
            c.GetComponent<Targetable>().highlightTarget(c);
        }

        foreach (Character c in GameManager.manager.foes)
        {
            c.GetComponent<Targetable>().highlightTarget(c);
        }
    }

    public void highlightTarget(Character character)
    {
        
        if (targetting && targetTypes.Contains(targetType))
        {
            character.toggleHighlight();
        }

    }

    public static IEnumerator GetTargetable(Enums.TargetType type, Character source, string msg, int count = 1){

        //send msg to some Text object in the screen to inform the player what they are targetting
        CombatUIManager.Instance.SetMessage(msg);

        //Disables raycast of dropzone to prevent from being targetted.
        GameManager.manager.cardDropZone.GetComponent<UnityEngine.UI.Image>().raycastTarget = false;

        currentTargets = new List<ITargetable>();
        targetting = true;
        targetType = type;
        targetSource = source;

        highlightTargets();

        //loop while target is being found based on 'targetting'. The onpointerclick function is utilized while this keeps the function from ending \
        // Checks each frame if the number of targets is returned.
        while (currentTargets.Count < count) {
            yield return new WaitForEndOfFrame();
        }


        highlightTargets();

        GameManager.manager.cardDropZone.GetComponent<UnityEngine.UI.Image>().raycastTarget = true;
        targetting = false;
        CombatUIManager.Instance.HideMessage();
    }
}

