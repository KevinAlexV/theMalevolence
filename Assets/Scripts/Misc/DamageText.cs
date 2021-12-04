using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

/** <summary>Controls popup combat text.</summary> */
public class DamageText : MonoBehaviour {

    /** <summary>The GameObject the damage text is attached to.</summary> */
    [SerializeField] private GameObject floatingText;


    void Start() {
        AnimatorClipInfo[] clipInfos = floatingText.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfos[0].clip.length);
    }

    /**
     * <summary>Sets the popup text to a chosen string.</summary>
     * <param name="value">The message or value of the popup text.</param>
     */
    public void SetText(string value) {
        floatingText.GetComponent<TextMeshProUGUI>().text = value ;
    }

    internal void SetColor (Color color) {
        floatingText.GetComponent<TextMeshProUGUI>().color = color;
    }
}
