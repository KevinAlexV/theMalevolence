using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Draggable))]
public class CharacterDisplayController : MonoBehaviour, IPointerClickHandler {
    [SerializeReference]
    private Text _hptxt;
    [SerializeReference]
    private Text _cptxt;
    [SerializeReference]
    private Text _discardtxt;
    [SerializeReference]
    private Text _nametxt;
    [SerializeReference]
    private Image _nameHighlight;
    [SerializeReference]
    private Image _deathMark;
    [SerializeReference]
    private Image _profile;
    [SerializeReference]
    private Image _thumbtack;
    [SerializeReference]
    private Image _action;
    [SerializeReference]
    private Text _actionText;
    [SerializeReference]
    private GameObject _corruption;
    [SerializeReference]
    private GameObject _turnHighlight;
    [SerializeReference]
    private Color DefaultColor;

    private Enums.Action prevAction;

    private bool _returnCard = true;
    private bool draw = false;


    [SerializeReference]
    private Text _actiontxt;

    public Text HealthDisplay { get { return _hptxt;} set { _hptxt = value; } } 
    public Text CorruptionDisplay { get { return _cptxt;} set { _cptxt = value; } }
    public Text DiscardDisplay { get { return _discardtxt; } set { _discardtxt = value; } }
    public Text NameDisplay { get { return _nametxt; } set { _nametxt = value; } } 
    public Text ActionDisplay { get { return _actiontxt; } set { _actiontxt = value; } }
    public bool ReturnCard { get { return _returnCard; } set { _returnCard = value; } }

    public Button actionButton;

    //private Dictionary<string, StatusEffectDisplay> statusEffects;

    [SerializeField]
    private Character _character;
    public Character Character {
        get{
            return _character;
        }
        set{
            _character = value;
            //Set the fields based on the character data
            //Subscribe to character events to continually update
            _character.onStatChange += (string statName, ref int oldValue, ref int newValue) => {
                //Debug.Log($"<color=cyan>{statName} stat change</color> from {oldValue} to {newValue}");
                if(statName == "health"){
                    ChangeHealth(newValue);
                } else if(statName == "corruption"){
                    ChangeCorruption(newValue);
                }
                else if (statName == "discard")
                {
                    ChangeDiscardPile(newValue);
                }
            };

            ChangeHealth(Character.Health);
            ChangeCorruption(Character.Corruption);
            ChangeName(Character.data.name, Character.data.color);
            ChangeProfile(Character.data.avatar);
            ChangeThumbtack(Character.data.thumbtack);
            ChangeAction(Character.data.weapon);
            _actionText.text = "<color=black>Attack\n\n\n</color>";

            _character.onActionChange += ChangeAction;
        }
    }

    //Setters for the CharacterDisplay Prefab
    public void ChangeProfile(Sprite newProfile) {
        _profile.sprite = newProfile;
    }
    public void ChangeThumbtack(Sprite newTack)
    {
        _thumbtack.sprite = newTack;
    }
    public void ChangeAction(Sprite newAction)
    {
        if(newAction != null)
            _action.sprite = newAction;
    }
    public void ChangeName(string name, Color color)
    {
        NameDisplay.text = name;
        _nameHighlight.color = color;
    }
    public void ChangeHealth(int currentHealth) {

        //Debug.Log($"{Character.name} Health being changed to {currentHealth}. Defeated = {Character.Defeated}");

        if (Character.Defeated == true)
        {
            _deathMark.enabled = true;
        }
        else { _deathMark.enabled = false; }

        HealthDisplay.text = currentHealth + "/" + Character.data.health;
    }
    public void ChangeCorruption(int currentCorruption) {

        CorruptionDisplay.text = currentCorruption.ToString() +"%";

        switch (currentCorruption)
        {
            case int n when (n > 75):
                _corruption.GetComponent<Image>().sprite = _corruption.transform.GetChild(3).gameObject.GetComponent<Image>().sprite;
                break;
            case int n when (n > 50):
                _corruption.GetComponent<Image>().sprite = _corruption.transform.GetChild(2).gameObject.GetComponent<Image>().sprite;
                break;
            case int n when (n > 25):
                _corruption.GetComponent<Image>().sprite = _corruption.transform.GetChild(1).gameObject.GetComponent<Image>().sprite;
                break;
            case int n when (n > 0):
                _corruption.GetComponent<Image>().sprite = _corruption.transform.GetChild(0).gameObject.GetComponent<Image>().sprite;
                break;
            default:
                try { _corruption.GetComponent<Image>().sprite = _corruption.transform.GetChild(0).gameObject.GetComponent<Image>().sprite; }catch{ }
                break;
        }

    }
    public void ChangeDiscardPile(int currentDiscarded)
    {
        Debug.Log($"{currentDiscarded} are the cards currently discarded.");
        DiscardDisplay.text = currentDiscarded.ToString();
    }

    public void ChangeAction(Enums.Action oldAction, Enums.Action newAction) {
        if(oldAction == newAction && oldAction != Enums.Action.Card) return;

        //ActionDisplay.text = "";

        switch (newAction){
            case Enums.Action.Attack:
                ActionDisplay.text = "";

                actionButton.interactable = false;

                if(_character.data.weapon != null)
                {
                    _action.color = DefaultColor;
                    _action.sprite = _character.data.weapon;
                    _actionText.text = "<color=black>Attack\n\n\n</color>";
                }
                break;
            case Enums.Action.Card:

                ActionDisplay.text = "";

                if (_character.CardToPlay != null)
                { 
                    _action.sprite = _character.data.cardBack;
                    _actionText.text = $"<color=white>{_character.CardToPlay.Name}</color>";
                }

                break;
            case Enums.Action.Stunned:
                ActionDisplay.text = "Stunned";
                break;
            case Enums.Action.Draw:
                _action.sprite = _character.data.cardBack;
                if(GameManager.manager.decks[Character.data.characterType].CardList.Count != 0)
                    _actionText.text = $"<color=white>Draw Card</color>";
                else if (GameManager.manager.decks[Character.data.characterType].DiscardList.Count > 0)
                    _actionText.text = $"<color=white>Reshuffle and Draw</color>";
                else
                    _actionText.text = $"<color=white>Out of Cards</color>";
                break;
        }

    }


    public void ToggleDrawButton(bool enabled)
    {
        draw = enabled;

        if (enabled)
        {
            prevAction = Character.Action;
            Character.Action = Enums.Action.Draw;

            if (GameManager.manager.decks[Character.data.characterType].CardList.Count != 0 || GameManager.manager.decks[Character.data.characterType].DiscardList.Count != 0 || enabled == false)
                GameManager.manager.toggleCharButton(this, enabled);
        }
        else if (!enabled)
        {
            Character.Action = prevAction;
            GameManager.manager.toggleCharButton(this, GameManager.manager.actionsEnabled);
        }

    }


    public TurnOrderSlot currentTurnSlot;


    public void Start(){
        Character = _character;
        var d = GetComponent<Draggable>();
        d.followMouse = false;

        //Subscribe to the 'onDrag' event and execute code upon the event.
        d.onDrag += (drag, drop) =>{
            Vector3 pos = d.transform.position;
            pos.y = Input.mousePosition.y;
            transform.position = pos;
        };

        //Subscribe to the 'onDragStart' event and execute code upon the event.
        d.onDragStart += (drag, drop) =>{
            ToggleRayCastOnOthers(false);
            this._thumbtack.enabled = false;
        };

        //Subscribe to the 'onDragStop' event and execute code upon the event.
        d.onDragStop += (drag, drop) =>{
            ToggleRayCastOnOthers(true);
            this._thumbtack.enabled = true;
        };

        _character.onCorruptionCheckAttempt += StartCorruptionCheck;
        _character.onCorruptionCheckResult += ShowCorruptionCheck;

        actionButton.onClick.AddListener(() => {
            CheckAction();
        });
    }

    //For performing corruption checks, and providing some feedback for players.
    public void StartCorruptionCheck(ref int corruptionValueForCheck)
    {
        Debug.Log("<color=magenta>CorruptionCheckAnimating</color>");
        
        //Enable a particle effect on corruption for the display controller, where the number is also highlighted
        Animator corrAnim = GetComponent<Animator>();
        corrAnim.enabled = true;
        corrAnim.Play("Check");
    }

    //For performing corruption checks, and providing some feedback for players.
    public void ShowCorruptionCheck(bool passed)
    {
        Debug.Log("<color=magenta>Corruption check resolved</color>");

        Animator corrAnim = GetComponent<Animator>();

        //depending on result, perform a 'drop corruption orb' effect, or 'merge' with the corruption value (just fade).
        if (passed)
        {
            corrAnim.SetTrigger("CheckPass");
        }
        else
        {
            corrAnim.SetTrigger("CheckFail");
        }
    }

    public void highlightTurn(bool state)
    {

        if (_turnHighlight != null && Character.Defeated == false)
            _turnHighlight.active = state;

    }

    public void highlightTurnSlot(bool state)
    {

        if (state)
            this.GetComponent<Image>().color = new Color(1f,1f,1f,1f);
        else
            this.GetComponent<Image>().color = new Color(1f, 1f, 1f, .75f); ;
    }

    //Toggles the graphic raycast component on all other (Slightly jank, a better method probably exists)
    private void ToggleRayCastOnOthers(bool enabled){
        var gr = GetComponent<GraphicRaycaster>();
        bool myToggleState = gr.enabled;
        foreach(TurnOrderSlot slot in GameManager.manager.turnSlots){
            slot.currentTurnDraggable.GetComponent<GraphicRaycaster>().enabled = enabled;
        }
        gr.enabled = myToggleState;
    }

    //Reset the character back to attacking if their display is right clicked
    public void OnPointerClick(PointerEventData d){
        if(d.button == PointerEventData.InputButton.Right && Character.CardToPlay != null){
            GameManager.manager.PlaceCardInHand(Character.CardToPlay);
            Character.CardToPlay = null;
        }
    }


    //Reset the character back to attacking if their action button is clicked
    public void CheckAction()
    {
        if (draw == true)
        {
            GameManager.manager.Draw(Character.data.characterType);
        }
        else if (Character.CardToPlay != null && _returnCard == true)
        {
            GameManager.manager.PlaceCardInHand(Character.CardToPlay);
            Character.CardToPlay = null;
        }
    }

}
