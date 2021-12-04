using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{
    //Allows selection of character based on Enums script.
    public Enums.Character characterType;

    //Base values for a new character asset (which can be modified in the inspector)
    public new string name;
    public int health;
    
    [HideInInspector]
    public int currentHealth;
    public int corruption;
    [HideInInspector]
    public int currentCorruption;
    public Sprite avatar;
    public Sprite thumbtack;
    public Sprite weapon;
    public Color color;
    public Sprite cardBack;

    [SerializeField] public List<Card> cards = new List<Card>();

    //The basic attack damage attached to a new character. This uses the damage script to create a new type following
    // the format int dieNum, int dieSize, int bonus (so 1 6 0 = 1d6 no bonus')
    [SerializeField]
    public Damage basicAttack = new Damage(1,6,0);

    public Deck Deck {
        get { return new Deck(cards); }
    }

    public void OnEnable(){
        currentHealth = health;
        currentCorruption = corruption;
    }

    public void UpdateStats(Character c){
        //currentHealth = c.Health;
        currentCorruption = c.Corruption;
    }
}
