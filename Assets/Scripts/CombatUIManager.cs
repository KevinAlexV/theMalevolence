using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUIManager : MonoBehaviour {

    private static CombatUIManager instance;

    [Header("UI Elements")]
    [SerializeField] private DisplayText displayText;

    [SerializeField] private GameObject CardRevealDisplay;

    private DamageText PopupDamageText;

    public static CombatUIManager Instance { get { return instance; } }

    void Awake () {
        if (instance == null)
            instance = this;
        else if (this != instance)
            Destroy(this);
        PopupDamageText = Resources.Load<DamageText>("Prefabs/DamagePopUp");
    }


    public void SetDamageText (int value, Transform location) {
        DamageText instance = Instantiate(PopupDamageText);
        Vector2 screenPos = Camera.main.WorldToScreenPoint(location.position);
        instance.transform.SetParent(this.transform, false);
        instance.transform.position = screenPos;
        instance.SetText(Mathf.Abs(value).ToString());
    }
    
    public void SetDamageText (int value, Transform location, Color32 color) {
        DamageText instance = Instantiate(PopupDamageText);
        Vector2 screenPos = Camera.main.WorldToScreenPoint(location.position);
        instance.transform.SetParent(this.transform, false);
        instance.transform.position = screenPos;
        instance.SetText(Mathf.Abs(value).ToString());
        instance.SetColor(color);
    }

    public IEnumerator DisplayMessage (string msg, float duration = 1f) {
        displayText.SetMessage(msg);
        yield return new WaitForSeconds(duration);
        displayText.StartFade();
    }

    public void SetMessage(string msg){
        displayText.SetMessage(msg);
    }

    public void HideMessage(){
        displayText.StartFade();
    }

    public IEnumerator RevealCard(Card card, float duration = 1f){
        CardDisplayController display = CardDisplayController.CreateCard(card);
        display.GetComponent<Draggable>().followMouse = false;
        
        CardRevealDisplay.active = true;
        CardRevealDisplay.GetComponent<Canvas>().enabled = true;

        RectTransform cardRectTransform = display.GetComponent<RectTransform>();
        RectTransform DisplayArea = CardRevealDisplay.GetComponent<RectTransform>();
        cardRectTransform.SetParent(DisplayArea.transform);

        yield return new WaitForSeconds(duration);
        displayText.SetMessage("Press any key to continue");
        while(!Input.anyKey){
            yield return new WaitForEndOfFrame();
        }
        displayText.StartFade();

        CardRevealDisplay.GetComponent<Canvas>().enabled = false;
        CardRevealDisplay.active = false;

        Destroy(display.gameObject);
    }

    //The list given will given will be modified to contain only the selected card
    public IEnumerator DisplayChoice(List<Card> choices){

        List<CardDisplayController> displays = new List<CardDisplayController>();
        int selectedIndex = -1;
        
        CardRevealDisplay.active = true;
        CardRevealDisplay.GetComponent<Canvas>().enabled = true;

        foreach (Card card in choices){
            CardDisplayController display = CardDisplayController.CreateCard(card);
            display.GetComponent<Draggable>().followMouse = false;
            display.GetComponent<Draggable>().planningPhaseOnly = false;

            display.transform.SetParent(displayText.transform);

            RectTransform cardRectTransform = display.GetComponent<RectTransform>();
            RectTransform DisplayArea = CardRevealDisplay.GetComponent<RectTransform>();
            cardRectTransform.SetParent(DisplayArea.transform);

            //cardRectTransform.offsetMin = new Vector2((choices.IndexOf(card)) * 250 - ((choices.Count - 1) * 125), 120);
            //cardRectTransform.sizeDelta = new Vector2(60, 90);

            displays.Add(display);
            display.GetComponent<Draggable>().onDragStart += (drag, drop) => {
                selectedIndex = displays.IndexOf(display);
            };
        }

        while(selectedIndex == -1){
            yield return new WaitForEndOfFrame();
        }

        var choice = choices[selectedIndex];
        choices.Clear();
        choices.Add(choice);

        CardRevealDisplay.GetComponent<Canvas>().enabled = false;
        CardRevealDisplay.active = false;

        foreach (CardDisplayController display in displays){
            Destroy(display.gameObject);
        }
        
    }
}
