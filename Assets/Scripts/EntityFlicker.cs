using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityFlicker : MonoBehaviour {
    [SerializeField] private Image flickerImage;
    [SerializeField] private float imageMinTime;
    [SerializeField] private float imageMaxTime;
    [SerializeField] private float imageHoldTime;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker() {
        float timeToWait = 0;
        while (true) {
            timeToWait = Random.Range(imageMinTime, imageMaxTime);
            yield return new WaitForSeconds(timeToWait);
            flickerImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(imageHoldTime);
            flickerImage.gameObject.SetActive(false);
        }
    }
}
