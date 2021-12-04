using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/*
Display text behaviour
- Instantiate


*/

public class DisplayText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI primaryMessage;
    public TextMeshProUGUI secondaryMessage;
    public bool forceVisible = false;
    private IEnumerator primaryFadeEnumerator;
    private IEnumerator secondaryFadeEnumerator;
    private delegate void OnCompleteFade();
    public void OnPointerEnter(PointerEventData d){
        SetVisible();
    }

    public void OnPointerExit(PointerEventData d){
        if(!forceVisible){
            StartFade(false);
        }
        FadeSecondary();
    }

    public void SetMessage(string msg){
        secondaryMessage.text = primaryMessage.text;
        primaryMessage.text = msg;
        forceVisible = true;
        SetVisible(false);
    }

    public void StartFade(bool bothMessages = true){
        forceVisible = false;
        primaryFadeEnumerator = Fade(primaryMessage, ()=>{primaryFadeEnumerator = null;});
        StartCoroutine(primaryFadeEnumerator);
        if(bothMessages){
            FadeSecondary();
        }
    }

    private void FadeSecondary(){
        secondaryFadeEnumerator = Fade(secondaryMessage, ()=>{secondaryFadeEnumerator = null;});
        StartCoroutine(secondaryFadeEnumerator);
    }

    private IEnumerator Fade(TextMeshProUGUI message, OnCompleteFade onComplete){
        Color color = message.color;
        while(color.a > 0.001f){
            color.a -= 0.01f;
            message.color = color;
            yield return new WaitForFixedUpdate();
        }
        onComplete();
        yield return null;
    }

    public void SetVisible(bool bothMessages = true){
        if(primaryFadeEnumerator != null) StopCoroutine(primaryFadeEnumerator);
        Color primaryColor = primaryMessage.color;
        primaryColor.a = 1;
        primaryMessage.color = primaryColor;
        
        if(bothMessages){
            if(secondaryFadeEnumerator != null) StopCoroutine(secondaryFadeEnumerator);
            Color secondaryColor = secondaryMessage.color;
            secondaryColor.a = 1;
            secondaryMessage.color = secondaryColor;
        }
    }
}
