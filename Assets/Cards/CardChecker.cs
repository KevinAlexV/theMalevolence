using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardChecker : MonoBehaviour
{
    [SerializeField] private Card card;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(
            card.Name + ":\n"
            + card.Description + "\n"
            + card.cardEffects.Count
            );
    }

    // Update is called once per frame
    void Update() {
        
    }
}
