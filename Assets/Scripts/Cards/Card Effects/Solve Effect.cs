using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>Unique Card effect: Solves a configuration of the Puzzle Box.</summary> */
[System.Serializable]
public class SolveEffect : CardEffect {
    [SerializeField] private Enums.PuzzleBoxConfigurations configuration;

    public override IEnumerator ApplyEffect () {
        if (targets[0].data.characterType == Enums.Character.PuzzleBox) {
            PuzzleboxCharacter boss = (PuzzleboxCharacter)targets[0];
            if (boss.Configuration == configuration) {
                yield return boss.Solve(configuration);
            card.Exile();
            } else
                yield return CombatUIManager.Instance.DisplayMessage("Failed to solve the " + configuration + " configuration");
        }
        yield return null;
    }
}
