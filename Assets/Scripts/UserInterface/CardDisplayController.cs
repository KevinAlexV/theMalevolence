using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Draggable))]
public class CardDisplayController : MonoBehaviour {
   
    public static GameObject cardDisplayPrefab;

    internal int Handx;
    internal int Handy;

    //The name of the displayed card. Must be changed to display the cards proper name (seperate field)
    [SerializeReference]
    private Text _name;
    public Text Name
    {
        get{return _name;}
    }

    [SerializeReference]
    private Text _corrPass;
    public Text CorruptionPassDescription
    {
        get { return _corrPass; }
        set { _corrPass = value; }
    }

    [SerializeReference]
    private Text _corrFail;
    public Text CorruptionFailDescription
    {
        get { return _corrFail; }
        set { _corrFail = value; }
    }

    //The description of the displayed card. Must be changed to display the cards proper description (seperate field)
    [SerializeReference]
    private Text _description;
    public Text Description
    {
        get { return _description; }
        set { _description = value; }
    }

    [SerializeReference]
    private Image _front;
    public Image Front
    {
        get { return _front; }
        set { _front = value; }
    }

    [SerializeReference]
    private Image _back;
    public Image Back
    {
        get { return _back; }
        set { _back = value; }
    }

    [SerializeReference]
    private GameObject _corEye;
    [SerializeReference]
    private Image[] _icons = new Image[4];

    private Card _cardData;

    //The public facing card of this class. This is where all the cards data (from _cardData) is returned
    public Card CardData
    {
        get{return _cardData;}
        set
        {
            _cardData = value;
            //TODO: Set the text and image aspects of the cards using the card data

        }
    }

    public void Play()
    {
        var character = GameManager.manager.characters[CardData.Character];
        if(character.Incapacitated){
            //Display some error
            StartCoroutine(CombatUIManager.Instance.DisplayMessage($"{character.data.name} is incapacitated and cannot play a card"));
            var card = CardData;
            GameManager.manager.PlaceCardInHand(card);
            GameManager.manager.RemoveCardFromHand(this);
            return;
        }
        character.CardToPlay = CardData;
        GetComponent<Draggable>().enabled = false;
        //Activate the cards designate targets function.
        StartCoroutine(ResolveTargets());
    }

    
    public void Start(){
        GetComponent<Draggable>().zone = GameObject.FindGameObjectWithTag("HandDisplay").GetComponent<DropZone>();
        GetComponent<Draggable>().returnDropZone = GetComponent<Draggable>().zone;

        GetComponent<Draggable>().onDragStop += (drag, drop) => {
            if(drop != null && drop.name == "DropZone"){
                Play();
            }
        };
    }

    // Delete cards once targets have been designated
    public IEnumerator ResolveTargets()
    {
        GameManager.manager.cardDropZone.enabled = false;

        GameManager.manager.ToggleEndPhaseButton(false);

        this.GetComponent<RectTransform>().position = new Vector3(175, 200, 0);
        _back.color = new Color(_back.color.r, _back.color.g, _back.color.b, .5f);

        yield return CardData.DesignateTargets();
        Debug.Log($"<color=blue>{CardData.Name} </color>Designating target...");
        GameManager.manager.RemoveCardFromHand(this);
        GameManager.manager.cardDropZone.enabled = true;


        AudioManager.audioMgr.PlayUISFX("PlaceCard");

        GameManager.manager.ToggleEndPhaseButton(true);
    }

    //Create a new display of the card selected
    public static CardDisplayController CreateCard(Card card){

        if (cardDisplayPrefab == null)
            cardDisplayPrefab = Resources.Load<GameObject>("UserInterface/CardDisplay");
        
        var cardGameObject = Instantiate<GameObject>(cardDisplayPrefab, Vector3.zero, Quaternion.identity);

        var cardDisplay = cardGameObject.GetComponent<CardDisplayController>();

        cardDisplay.CardData = card;

        //Debug.Log($"CDC: Creating card {card}");

        cardDisplay._name.text = card.Name;
        cardDisplay._description.text = card.Description;

        Debug.Log($"{card.CorruptionPassDescription}");

        cardDisplay._front.color = card.Color;

        if(card.BackArt != null)
            cardDisplay._back.sprite = card.BackArt;
        if (card.FrontArt != null)
            cardDisplay._front.sprite = card.FrontArt;
        if (card.cardCorFail.Count > 0 || card.cardCorPass.Count > 0)
        { 
            cardDisplay._corEye.active = true;
            cardDisplay._corrPass.text = "Pass: " + card.CorruptionPassDescription;
            cardDisplay._corrFail.text = "Fail: " + card.CorruptionFailDescription;
        }
        else
            cardDisplay._corEye.active = false;
        for (int i = 0; i < 4; i++) {
            if (card.Icons[i] != null)
                cardDisplay._icons[i].sprite = card.Icons[i];
        }

        return cardDisplay;
    }
    
}
