using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    public bool shuffleOnStart = false;

    public Enums.GameplayPhase phase;

    //Speeds the delay between phases, should also be applied to animations
    public float speedScale;

    public Dictionary<Enums.Character, Deck> decks = new Dictionary<Enums.Character, Deck>();

    public HandDisplayController hand;
    public DropZone cardDropZone;
    public List<TurnOrderSlot> turnSlots;
    public Canvas messager;
    public bool actionsEnabled = false, testing = false;

    public List<Character> party = new List<Character>();
    public List<Character> foes = new List<Character>();

    public Dictionary<Enums.Character, Character> characters = new Dictionary<Enums.Character, Character>();

    public List<ITurnExecutable> turns;

    private int turnNumber = 0;
    private IEnumerator battleEnumerator;
    private bool gameOver = false;

    public GameObject endPhaseButton;

    public AudioClip battleMusic;

    public delegate void PhaseChangeHandler(Enums.GameplayPhase phase);
    public event PhaseChangeHandler onPhaseChange;

    private void Awake()
    {
        if (manager != null)
        {
            Destroy(this);
        }
        manager = this;
    }

    void Start()
    {

        StartBattle();

        //Checks if something dropped in the card zone is actually a card, and only continues if it is
        cardDropZone.onDrop += (drag, drop) =>{
            CardDisplayController cardController = null;
            if(drag.gameObject.TryGetComponent(out cardController)){
                drag.Drop(drop);
            }
        };

        hand.GetComponent<DropZone>().onDrop += (drag, drop) => {
            CardDisplayController cardController = null;
            if (drag.gameObject.TryGetComponent(out cardController))
            {
                drag.Drop(drop);
            }
        };

        endPhaseButton.GetComponent<Button>().onClick.AddListener(() => {
            EndPlanning();
        });

    }

    //Starts a new battle with listed enemies. This initializes the characers, decks, and starts a new coroutine: battleEnumerator (like a thread)
    public void StartBattle()
    {
        //Play battle start effects
        AudioManager.audioMgr.ChangeMusic(battleMusic);
        //Draw starting hand
        InitializeCharacters();
        InitializeDecks();

        foreach(Character c in party)
        {
            Draw(c.data.characterType);
        }

        battleEnumerator = ExecuteBattle();
        StartCoroutine(battleEnumerator);
    }

    //Loops through turns untill the battleEnumerator is stopped (By CheckGameOver)
    public IEnumerator ExecuteBattle()
    {
        while(true)
        {
            yield return ExecutePlanning();
            yield return ExecuteTurn();
            yield return ExecuteDrawPhase();
        }
        
    }

    // Planning phase
    public IEnumerator ExecutePlanning()
    {
        phase = Enums.GameplayPhase.Planning;
        ToggleEndPhaseButton(true);

        actionsEnabled = true;

        foreach (PartyCharacter c in party)
        {
            c.checkVoicelines();
        }

        //For each turn in turnSlots, enabled return card button
        foreach (TurnOrderSlot turnSlot in turnSlots)
        {
            var display = turnSlot.currentTurnDraggable.GetComponent<CharacterDisplayController>();

            //If character is not defeated, enable their character button.
            toggleCharButton(display, true);

            //If the current list of foes does not contain the character type used for this turn, then brighten their turn slot.
            if (!foes.Contains(characters[display.Character.data.characterType]))
                display.highlightTurnSlot(true);

        }

        if (onPhaseChange != null){
            onPhaseChange(phase);
        }
        Debug.Log("<color=yellow>Planning Phase</color>");
        while(phase == Enums.GameplayPhase.Planning)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    // Turn/action phase
    public IEnumerator ExecuteTurn()
    {
        turnNumber++;

        actionsEnabled = false;
        //UI turn resolving starts
        phase = Enums.GameplayPhase.Resolve;
        
        Debug.Log("<color=yellow>Resolving Phase</color>");
        turns = new List<ITurnExecutable>();

        if (!testing)
        {
            //For each turn in turnSlots, add a turn to the turns list
            foreach (TurnOrderSlot turnSlot in turnSlots)
            {
                turns.Add(turnSlot.Turn);

                var display = turnSlot.currentTurnDraggable.GetComponent<CharacterDisplayController>();
                display.highlightTurnSlot(false);

                toggleCharButton(display, false);
            }


            if (onPhaseChange != null)
            {
                onPhaseChange(phase);
            }

            int count = 0;

            foreach (ITurnExecutable turn in turns)
            {
                CharacterDisplayController CurrentCharDisplay = null;

                try { CurrentCharDisplay = turnSlots[count].currentTurnDraggable.GetComponent<CharacterDisplayController>(); }
                catch { Debug.Log($"<color=red>Error: {turn} at {count}'th turn contains no valid character display controller. </color>"); }

                count++;

                if (CurrentCharDisplay != null)
                    CurrentCharDisplay.highlightTurn(true);

                yield return turn.GetTurn();
                if (gameOver)
                {
                    yield break;
                }

                if (CurrentCharDisplay != null)
                    CurrentCharDisplay.highlightTurn(false);
            }

            //Discards all cards that were played
            foreach (ITurnExecutable turn in turns)
            {
                Character c = turn as Character;
                if (c != null)
                {
                    c.EndResolvePhase();
                }
            }
        }
        else
        {
            foes[0].Health -= 1;
        }
    }

    //Executes while turn is in draw phase
    public IEnumerator ExecuteDrawPhase(){
        
        actionsEnabled = true;

        phase = Enums.GameplayPhase.Draw;
        Debug.Log("<color=yellow>Draw Phase</color>:");
        if(onPhaseChange != null){
            onPhaseChange(phase);
        }
        StartCoroutine(CombatUIManager.Instance.DisplayMessage("Draw a card", 3f));
        bool cardsToDraw = false;
        
        int cardsInHand = hand.DisplayedCards.Count;
        //Enable draw buttons (could be better optimized)
        foreach(TurnOrderSlot turnSlot in turnSlots)
        {
            var display = turnSlot.currentTurnDraggable.GetComponent<CharacterDisplayController>();
            if(party.Contains(display.Character) && !display.Character.Defeated && testing != true){
                display.ToggleDrawButton(true);
                cardsToDraw = true;
            }
        }
        //Only wait to draw if at least one deck has cards in it
        if(cardsToDraw){
            //Wait untill one card has been drawn
            while(hand.DisplayedCards.Count != cardsInHand + 1){
                yield return new WaitForEndOfFrame();
            }
            //Disable draw buttons
            foreach(TurnOrderSlot turnSlot in turnSlots)
            {
                var display = turnSlot.currentTurnDraggable.GetComponent<CharacterDisplayController>();
                if(party.Contains(display.Character) && !display.Character.Defeated){
                    display.ToggleDrawButton(false);
                }
            }
        }
        phase = Enums.GameplayPhase.Planning;
    }

    //Checks if the game is over. Should be called whenever a character or foe is Defeated
    public void CheckGameOver()
    {
        bool playerDefeated = true;

        foreach(Character partyMember in party)
        {
            playerDefeated = playerDefeated && partyMember.Defeated;
        }

        if(playerDefeated)
        {
            StopCoroutine(battleEnumerator);
            Debug.Log("Game Over! TPK");
            //Return to main menu ui
            StartCoroutine(GameOverScreen());
        }

        bool foesDefeated = true;
        foreach(Character foe in foes)
        {
            foesDefeated = foesDefeated && foe.Defeated;
        }

        if(foesDefeated)
        {
            StopCoroutine(battleEnumerator);
            Debug.Log("Game Over! Defeated enemies");
            //Card Drafting ui
            StartCoroutine(GameWinScreen());

        }
        gameOver = foesDefeated || playerDefeated;
    }

    public IEnumerator GameWinScreen(){
        yield return CombatUIManager.Instance.DisplayMessage("", 4f);
        foreach(Character c in party){
            c.data.UpdateStats(c);
        }
        if (!testing) { LevelManager.Instance.ToNextLevel(); }
        else { SceneManager.LoadScene("Main Menu"); }
    }

    public IEnumerator GameOverScreen(){
        yield return CombatUIManager.Instance.DisplayMessage("Everyone has fallen...", 2f);
        yield return CombatUIManager.Instance.DisplayMessage("Consumed by the Malevolence...", 4f);
        SceneManager.LoadScene("Main Menu");
        LevelManager.Instance.ToMainMenu();
    }

    //UI function, is called when the player presses the end planning button
    public void EndPlanning(){
        if(phase == Enums.GameplayPhase.Planning)
        {
            phase = Enums.GameplayPhase.Resolve;
            ToggleEndPhaseButton(false);
            AudioManager.audioMgr.PlayUISFX("PaperInteraction");
        }
    }

    public void ToggleEndPhaseButton(bool enabled){
        endPhaseButton.SetActive(enabled);
    }

    //Toggle character action button
    public void toggleCharButton(CharacterDisplayController display, bool enabled)
    {
        if (display.Character.Defeated == false && display.Character.Action != Enums.Action.Attack)
            display.actionButton.interactable = enabled;
        else
            display.actionButton.interactable = false;
    }

    //toggle all party members action buttons within turnslots
    public void togglePartyButton(bool enabled)
    {
        
        foreach (TurnOrderSlot turnSlot in turnSlots)
        {
            var display = turnSlot.currentTurnDraggable.GetComponent<CharacterDisplayController>();
            toggleCharButton(display, enabled);
        }
    }

    public void updateDiscardPile(Enums.Character character)
    {

        characters[character].Discarded = decks[character].DiscardList.Count;

    }

    public void Draw(Enums.Character characterDeckToDrawFrom)
    {
        var card = decks[characterDeckToDrawFrom].Draw();
        PlaceCardInHand(card);
    }

    //Return card to discard pile. Note: doesn't remove from hand
    public void Discard(Card card){
        if(card == null) return;
        if (card.Exiled) return;
        decks[card.Character].DiscardList.Add(card);
        updateDiscardPile(card.Character);
    }

    //Remove card display from hand: Note: doesn't discard
    public void PlaceCardInHand(Card c){
        hand.AddCard(CardDisplayController.CreateCard(c));
        AudioManager.audioMgr.PlayUISFX("PickupCard");
    }


    public void RemoveCardFromHand(CardDisplayController cd){
        hand.RemoveCard(cd);
    }
    
    //Links data from the inspector's characters and enum's class.
    public void InitializeDecks()
    {
        Character ch;
        characters.TryGetValue(Enums.Character.Goth, out ch);
        decks[Enums.Character.Goth] = ch.data.Deck;
        if(shuffleOnStart) decks[Enums.Character.Goth].Shuffle();

        foreach (Card c in ch.data.Deck.CardList)
            c.Color = ch.data.color;

        characters.TryGetValue(Enums.Character.Jock, out ch);
        decks[Enums.Character.Jock] = ch.data.Deck;
        if(shuffleOnStart) decks[Enums.Character.Jock].Shuffle();

        foreach (Card c in ch.data.Deck.CardList)
            c.Color = ch.data.color;

        characters.TryGetValue(Enums.Character.Nerd, out ch);
        decks[Enums.Character.Nerd] = ch.data.Deck;
        if(shuffleOnStart) decks[Enums.Character.Nerd].Shuffle();

        foreach (Card c in ch.data.Deck.CardList)
            c.Color = ch.data.color;

        characters.TryGetValue(Enums.Character.Popular, out ch);
        decks[Enums.Character.Popular] = ch.data.Deck;
        if(shuffleOnStart) decks[Enums.Character.Popular].Shuffle();

        foreach (Card c in ch.data.Deck.CardList)
            c.Color = ch.data.color;

        if (characters.TryGetValue(Enums.Character.Driver, out ch))
            decks[Enums.Character.Driver] = ch.data.Deck;
        if (characters.TryGetValue(Enums.Character.Headmaster, out ch))
            decks[Enums.Character.Headmaster] = ch.data.Deck;
        if (characters.TryGetValue(Enums.Character.PuzzleBox, out ch))
            decks[Enums.Character.PuzzleBox] = ch.data.Deck;
        if (characters.TryGetValue(Enums.Character.Entity, out ch))
            decks[Enums.Character.Entity] = ch.data.Deck;
    }

    //Initialize each character in party list established. 
    public void InitializeCharacters()
    {

        //for each character in the party, make that character type in characters dictionary equal to the party member
        foreach (Character c in party)
        {
            characters[c.data.characterType] = c;
        }

        foreach (Character e in foes)
        {
            characters[e.data.characterType] = e;
        }

    }

    //If looking for a child gameobject, find the gameobject by name and return the object (if none found, return null.
    public GameObject getChildGameObject(GameObject source, string name)
    {
        Transform[] children = source.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.gameObject.name == name) return child.gameObject;
        }

        return null;
    }

}

//Interface inherited by anything that can take a turn
public interface ITurnExecutable {

    //Returns an ienumerator with the runtime logic of the object's turn that is executed when it's turn is resolved
    IEnumerator GetTurn();
}