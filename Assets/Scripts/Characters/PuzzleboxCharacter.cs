using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleboxCharacter : EnemyCharacter {

    //===DECK SETUP===//
    //0  - Dark Insight
    //1  - Confound
    //2  - Volatile Ejections
    //3  - Dominating Will
    //4  - Vengeful Retaliation
    //5  - Sear Thoughts
    //6  - Sigil of the Discarded
    //7  - Strangulate
    //8  - Crimson Mark
    //9  - Voracious Hunger
    //10 - Flesh from Bone
    //11 - Coalescing Prism
    //12 - Chromatic Shattering
    //13 - Shifting Variance

    [SerializeField] private List<Transform> ShardSpawns;

    private Enums.PuzzleBoxConfigurations currentConfiguration = Enums.PuzzleBoxConfigurations.Default;
    private bool turnEnd = false;
    private ShardOfEternityCharacter[] Shards;

    private bool AchieverConfig = true;
    private bool ExplorerConfig = true;
    private bool KillerConfig = true;
    private bool SocializerConfig = true;

    private int configCount = 2;
    private bool changingConfig = false;

    public Enums.PuzzleBoxConfigurations Configuration { get { return currentConfiguration; } }

    public override void Awake () {
        base.Awake();
        animator = GetComponent<Animator>();
        Shards = new ShardOfEternityCharacter[ShardSpawns.Count];
    }


    [SerializeField] private int currentCard = 2;
    public override IEnumerator GetTurn () {
        changingConfig = false;
        if (Action != Enums.Action.Stunned && Health > 0) {
            if (deck.CardList.Count == 0) {
                deck.Reshuffle();
            }
            //Check if current configuration is valid
            if ((currentConfiguration == Enums.PuzzleBoxConfigurations.Achiever && !AchieverConfig)
                || (currentConfiguration == Enums.PuzzleBoxConfigurations.Explorer && !ExplorerConfig)
                || (currentConfiguration == Enums.PuzzleBoxConfigurations.Killer && !KillerConfig)
                || (currentConfiguration == Enums.PuzzleBoxConfigurations.Socializer && !SocializerConfig))
                yield return ChangeConfiguration();
            //Determine if this is action 1 or 2
            else {
                if (turnEnd == false) {
                    int cardChoice;
                    //If there are 5 Shards of Eternity, play "Coalescing  Prism"
                    if (GameManager.manager.foes.Count >= 6)
                        CardToPlay = deck.CardList[11];
                    else {
                        //Find configuration to determine cards to play
                        switch (currentConfiguration) {
                            //Default Configuration
                            case Enums.PuzzleBoxConfigurations.Default:
                                //In default configuration, play card "Dark Insight"
                                CardToPlay = deck.CardList[0];
                                break;
                            //Achiever Configuration
                            case Enums.PuzzleBoxConfigurations.Achiever:
                                cardChoice = Random.Range(1, 100);
                                //Randomly play "Volatile Ejections" (25%), "Dominating Will" (37%) or "Vengeful Retaliation" (38%)
                                if (cardChoice <= 25)
                                    CardToPlay = deck.CardList[2];
                                else if (cardChoice <= 62)
                                    CardToPlay = deck.CardList[3];
                                else
                                    CardToPlay = deck.CardList[4];
                                break;
                            //Explorer Configuration
                            case Enums.PuzzleBoxConfigurations.Explorer:
                                cardChoice = Random.Range(1, 100);
                                //Randomly play "Searing Thoughts" (25%), "Sigil of the Discarded" (37%) or "Strangulate" (38%)
                                if (cardChoice <= 25)
                                    CardToPlay = deck.CardList[5];
                                else if (cardChoice <= 62)
                                    CardToPlay = deck.CardList[6];
                                else
                                    CardToPlay = deck.CardList[7];
                                break;
                            //Killer Configuration
                            case Enums.PuzzleBoxConfigurations.Killer:
                                //If first turn as Killer, play "Crimson Mark"
                                if (configCount == 0) {
                                    CardToPlay = deck.CardList[8];
                                    break;
                                }
                                //Otherwise, play either "Voracious Hunger" or "Flesh from Bone"
                                cardChoice = Random.Range(1, 100);
                                if (cardChoice <= 50)
                                    CardToPlay = deck.CardList[9];
                                else
                                    CardToPlay = deck.CardList[10];
                                break;
                            //Socializer Configuration
                            case Enums.PuzzleBoxConfigurations.Socializer:
                            //If first turn as Socializer, play "Chromatic Shatter"
                                if (configCount == 0)
                                    CardToPlay = deck.CardList[12];
                                //Otherwise, play "Shifting Variance"
                                else
                                    CardToPlay = deck.CardList[13];
                                break;
                        }
                    }
                } else {
                    //If cube has been in configuration for 2 turns, change configuration.
                    //Else, play "Confound"
                    if (configCount == 2) {
                        yield return ChangeConfiguration();
                        changingConfig = true;
                        configCount = 0;
                    } else {
                        CardToPlay = deck.CardList[1];
                        configCount++;
                    }
                    for (int i = 0; i < Shards.Length; i++) {
                        if (Shards[i] != null) {
                            yield return Shards[i].GetTurn();
                        }
                    }
                }
                if (!changingConfig) {
                    yield return CombatUIManager.Instance.RevealCard(CardToPlay);

                    Debug.Log($"{name} playing card {CardToPlay.Name}");
                    CombatUIManager.Instance.DisplayMessage($"{name} plays {CardToPlay.Name}");
                    yield return CardToPlay.Activate();
                }
            }
        }
        turnEnd = !turnEnd;
        changingConfig = false;
    }

    private IEnumerator ChangeConfiguration() {
        if (CheckLast())
            yield break;
        bool valid = false;
        int newConfig = 0;
        do {
            newConfig = Random.Range(1, 4);
            if (newConfig == (int)currentConfiguration)
                continue;
            if (newConfig == 1 && !AchieverConfig)
                continue;
            if (newConfig == 2 && !ExplorerConfig)
                continue;
            if (newConfig == 3 && !KillerConfig)
                continue;
            if (newConfig == 4 && !SocializerConfig)
                continue;
            valid = true;
        } while (valid == false);
        currentConfiguration = (Enums.PuzzleBoxConfigurations)newConfig;
        animator.SetBool("Achiever", false);
        animator.SetBool("Explorer", false);
        animator.SetBool("Killer", false);
        animator.SetBool("Socializer", false);
        yield return new WaitForSeconds(2f);
        switch (currentConfiguration) {
            case Enums.PuzzleBoxConfigurations.Achiever:
                animator.SetBool("Achiever", true);
                yield return CombatUIManager.Instance.DisplayMessage("The Puzzle Box switches to the Achiever configuration");
                break;
            case Enums.PuzzleBoxConfigurations.Explorer:
                animator.SetBool("Explorer", true);
                yield return CombatUIManager.Instance.DisplayMessage("The Puzzle Box switches to the Explorer configuration");
                break;
            case Enums.PuzzleBoxConfigurations.Killer:
                animator.SetBool("Killer", true);
                yield return CombatUIManager.Instance.DisplayMessage("The Puzzle Box switches to the Killer configuration");
                break;
            case Enums.PuzzleBoxConfigurations.Socializer:
                animator.SetBool("Socializer", true);
                yield return CombatUIManager.Instance.DisplayMessage("The Puzzle Box switches to the Socializer configuration");
                break;
        }
        changingConfig = true;
        configCount = 0;
    }

    public IEnumerator Solve (Enums.PuzzleBoxConfigurations config) {
        switch(config) {
            case Enums.PuzzleBoxConfigurations.Achiever:
                AchieverConfig = false;
                break;
            case Enums.PuzzleBoxConfigurations.Explorer:
                ExplorerConfig = false;
                break;
            case Enums.PuzzleBoxConfigurations.Killer:
                KillerConfig = false;
                break;
            case Enums.PuzzleBoxConfigurations.Socializer:
                SocializerConfig = false;
                break;
        }
        yield return CombatUIManager.Instance.DisplayMessage("The " + config + " configuration has been solved");
        if (!AchieverConfig && !ExplorerConfig && !KillerConfig && !SocializerConfig)
            Health = 0;
    }

    public IEnumerator SpawnShard () {
        for (int i = 0; i < Shards.Length; i++) {
            if (Shards[i] == null) {
                ShardOfEternityCharacter newShard = Instantiate(Resources.Load<ShardOfEternityCharacter>("prefabs/Shard Of Eternity"), ShardSpawns[i].transform);
                newShard.transform.localPosition = Vector3.zero;
                yield return CombatUIManager.Instance.DisplayMessage("A Shard of Eternity has been created");
                Shards[i] = newShard;
                GameManager.manager.foes.Add(newShard);
                break;
            }
        }
    }

    public bool CheckLast() {
        int solved = 0;
        if (!AchieverConfig) solved++;
        if (!ExplorerConfig) solved++;
        if (!KillerConfig) solved++;
        if (!SocializerConfig) solved++;

        if (solved == 3)
            return true;
        return false;
    }
}
