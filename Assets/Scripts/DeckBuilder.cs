using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeckBuilder : MonoBehaviour
{
    private static DeckBuilder _instance;
    public static DeckBuilder Instance {
        get { return _instance; }
    }

    public CardDraftPool cardDraftPool;
    public int cardsToPull;
    public int cardsToKeep;
    public List<Card> pulledCards;
    public List<CardDisplayController> selectedCards;

    [SerializeField]
    private List<CharacterData> _party;
    public Dictionary<Enums.Character, CharacterData> party = new Dictionary<Enums.Character, CharacterData>();

    public HandDisplayController characterDeckDisplay;
    private Enums.Character characterDisplayed;
    public HandDisplayController draftDisplay;
    public TMPro.TextMeshProUGUI draftMessage;

    public GameObject draftButton;
    public GameObject exitButton;


    //Start the draft!
    public void Start(){
        if(_instance != null){
            Destroy(this);
        }
        _instance = this;
        foreach(CharacterData c in _party){
            party[c.characterType] = c;
        }
        DisplayDeck(Enums.Character.Goth);

        StartDraft();

        exitButton.SetActive(true);
        exitButton.GetComponent<Button>().onClick.RemoveAllListeners();
        exitButton.GetComponent<Button>().onClick.AddListener(Continue);
        //Debug.Log(GameManager.manager);
    }


    //Pull cards and display them to the draft hand.
    public IEnumerator PullCards(){
        pulledCards = cardDraftPool.GetShuffled().GetRange(0, cardsToPull);
        DisplayPulledCards();
        yield return null;
    }

    //Display the given characters deck from char data.
    public void DisplayDeck(Enums.Character character){
        characterDisplayed = character;
        ClearDeckDisplay();

        foreach(Card c in party[character].Deck.CardList){
            c.Color = party[c.Character].color;
            var display = CardDisplayController.CreateCard(c);
            display.GetComponent<Draggable>().enabled = false;
            characterDeckDisplay.AddCard(display);
        }
    }
    
    //Display character specific decks currently in char data.
    public void DisplayGoth(){
        if(Enums.Character.Goth == characterDisplayed) return;
        DisplayDeck(Enums.Character.Goth);
    }

    public void DisplayJock(){
        if(Enums.Character.Jock == characterDisplayed) return;
        DisplayDeck(Enums.Character.Jock);
    }

    public void DisplayNerd(){
        if(Enums.Character.Nerd == characterDisplayed) return;
        DisplayDeck(Enums.Character.Nerd);
    }

    public void DisplayPopular(){
        if(Enums.Character.Popular == characterDisplayed) return;
        DisplayDeck(Enums.Character.Popular);
    }


    //Clear the display for the current character.
    private void ClearDeckDisplay(){
        var displays = new List<CardDisplayController>(characterDeckDisplay.DisplayedCards);
        foreach(CardDisplayController display in displays){
            characterDeckDisplay.RemoveCard(display);
        }
    }


    //Display the cards from the pool of rewarded cards.
    public void DisplayPulledCards(){

        foreach (Card c in pulledCards){

            c.Color = party[c.Character].color;

            CardDisplayController display = CardDisplayController.CreateCard(c);
            Draggable draggable = display.GetComponent<Draggable>();

            draggable.followMouse = false;
            draggable.planningPhaseOnly = false;
            draggable.returnIfNotDropped = false;
            draggable.enabled = false;

            draggable.ClearHandlers();

            //Add a button to each card to interact and select while using the CardDisplayController class and avoiding conflicts/reorganization from the grid layout component.
            display.gameObject.AddComponent<Button>();

            Button cardSelector = display.GetComponent<Button>();

            cardSelector.onClick.AddListener(() => {
                SelectCard(display);
            });

            /*
            draggable.onDragStart += (a, b) => {

                if (selectedCards.Contains(display))
                {
                    selectedCards.Remove(display);
                }
                else if (selectedCards.Count == cardsToKeep)
                {
                    //unhighlight index 0
                    Destroy(selectedCards[0].transform.GetChild(0).gameObject);
                    selectedCards.RemoveAt(0);
                    selectedCards.Add(display);

                    //highlight card
                    var glow = Instantiate(Resources.Load<GameObject>("UserInterface/CardGlow"), display.transform);
                    glow.transform.SetAsFirstSibling();
                }
                else
                {
                    selectedCards.Add(display);
                    //highlight card
                    var glow = Instantiate(Resources.Load<GameObject>("UserInterface/CardGlow"), display.transform);
                    glow.transform.SetAsFirstSibling();
                }

            };

            draggable.onDragStop += (a,b) => {
                display.transform.SetParent(draftDisplay.transform);
            };*/

            draftDisplay.AddCard(display);
        }
    }

    public void SelectCard(CardDisplayController display)
    {

        Debug.Log($"<color=Purple>Card Clicked</color>: {display.CardData.name}");

        //If already selected, unselect card.
        if (selectedCards.Contains(display))
        {
            Debug.Log($"Card was unselected");
            
            //unhighlight display
            Destroy(display.transform.GetChild(0).gameObject);
            selectedCards.Remove(display);

        }//Replace oldest selection when selected cards is equal to cards to keep.
        else if (selectedCards.Count == cardsToKeep)
        {
            Debug.Log($"Card was selected");

            //unhighlight index 0
            Destroy(selectedCards[0].transform.GetChild(0).gameObject);
            selectedCards.RemoveAt(0);
            selectedCards.Add(display);

            //highlight card
            var glow = Instantiate(Resources.Load<GameObject>("UserInterface/CardGlow"), display.transform);
            glow.transform.SetAsFirstSibling();
            glow.transform.localScale = new Vector3(1.35f, 1.2f, 1f);

        }//Select current card and add it to selectedcards
        else
        {

            Debug.Log($"Card was selected");

            selectedCards.Add(display);

            //highlight card
            var glow = Instantiate(Resources.Load<GameObject>("UserInterface/CardGlow"), display.transform);
            glow.transform.SetAsFirstSibling();
            glow.transform.localScale = new Vector3(1.35f,1.2f,1f);
        }



    }


    public void StartDraft(){
        draftDisplay.transform.parent.transform.parent.gameObject.SetActive(true);
        StartCoroutine(PullCards());
        draftButton.SetActive(true);
        exitButton.GetComponentInChildren<Text>().text = "Continue";
        exitButton.SetActive(false);
        exitButton.GetComponent<Button>().onClick.RemoveAllListeners();
        exitButton.GetComponent<Button>().onClick.AddListener(Continue);
        draftMessage.gameObject.SetActive(true);
        draftMessage.text = $"Pick {cardsToKeep} card" + ((cardsToKeep > 1) ? "s" : "");
    }

    public void ConfirmDraft(){
        if(selectedCards.Count == cardsToKeep){
            Enums.Character draftedCharacter = Enums.Character.Popular;
            draftButton.SetActive(false);
            exitButton.SetActive(true);
            foreach(CardDisplayController display in selectedCards){
                draftedCharacter = display.CardData.Character;
                party[display.CardData.Character].cards.Insert(0, display.CardData);
            }
            draftDisplay.transform.parent.transform.parent.gameObject.SetActive(false);
            DisplayDeck(draftedCharacter);
            draftMessage.gameObject.SetActive(false);
        }
        
    }

    public void Continue(){
        LevelManager.Instance.ToNextLevel();

    }
}
