using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CutsceneManager : MonoBehaviour
{

    public string cutsceneName;
    [SerializeField]
    private TextMeshProUGUI dialogue, voices, events;
    [SerializeField]
    private Button skipScene, next;
    [SerializeField]
    private Image fadeImage, cutsceneImage, drawingImage, notebook, tutorial;
    [SerializeField]
    private List<Sprite> cutScene1Images, cutScene2Images, cutScene3Images, cutScene4Images, cutScene5Images, cutScene6Images;

    private List<Sprite> CurrentCutsceneImages;
    private float fadeTime = 3f, alphaToFadeTo;
    private int cutsceneNum, cutsceneImageStage = 0, cutsceneStage = 0;
    private Color currentTextCol;
    private string gothSColor = "purple", popSColor = "yellow", jockSColor = "red", nerdSColor = "green";
    private Color gothColor = new Color (160/ 255.0f, 32/255.0f, 240/255.0f), popColor = Color.yellow, jockColor = Color.red, nerdColor = Color.green;

    private bool coroutineRunning = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector4 targetColor;
    private GameObject modifiedObject;

    
    //fade simply fades the 'fadeImage' panel to a new color, which is changing the alpha mainly.
    private void fade()
    {

        if (fadeImage.color == new Color(0f, 0f, 0f, 1f))
            alphaToFadeTo = 0f;
        else
            alphaToFadeTo = 1f;

        Debug.Log($"{fadeImage.color} fading now to {alphaToFadeTo}...");

        StartCoroutine(FadeTo(alphaToFadeTo, fadeTime));

    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        coroutineRunning = true;

        targetColor = new Vector4(0,0,0,aValue);

        float alpha = fadeImage.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(0, 0, 0, Mathf.Lerp(alpha, aValue, t));
            fadeImage.color = newColor;
            yield return null;
        }

        coroutineRunning = false;
    }

    IEnumerator Shake(GameObject toShake, float scale)
    {
        coroutineRunning = true;

        originalPosition = toShake.transform.position;
        modifiedObject = toShake;

        do
        {
            float random = Random.Range(originalPosition.x-scale, originalPosition.x+scale);
            float random2 = Random.Range(originalPosition.y - scale, originalPosition.y + scale);
            float random3 = Random.Range(originalPosition.z - scale, originalPosition.z + scale);

            toShake.transform.position = new Vector3(random, random2, random3);
            Debug.Log($"{toShake.transform.position} is the current position of the object");

            yield return new WaitForSeconds(.1f);

        }
        while (true);
        

    }

    IEnumerator Tilt(GameObject toTilt)
    {
        coroutineRunning = true;

        originalRotation = toTilt.transform.rotation;
        modifiedObject = toTilt;

        do
        {
            float random = Random.Range(originalRotation.z - .03f, originalPosition.z + .03f);

            toTilt.transform.rotation = new Quaternion(originalRotation.x, originalRotation.y, random, originalRotation.w);
            Debug.Log($"{toTilt.transform.rotation} is the current rotation of the object");

            yield return new WaitForSeconds(.1f);

        }
        while (true);

    }

    

    private void toggleTutorialImage()
    {
        if(tutorial.color == new Color(1f,1f,1f,1f))
            tutorial.color = new Color(1f,1f,1f,0f);
        else
            tutorial.color = new Color(1f, 1f, 1f, 1f);
    }

    private void updateTutorial(int index)
    {

        tutorial.sprite = CurrentCutsceneImages[index];

    }

    private void updateDrawing(int index)
    {

        drawingImage.sprite = CurrentCutsceneImages[index];

    }

    private void updateImage(int index)
    {
        cutsceneImage.sprite = CurrentCutsceneImages[index];
    }

    private void updateText(string newText)
    {
        dialogue.color = currentTextCol;
        dialogue.text = newText;
    }

    private void changeImageAlpha(Image image, float alpha)
    {
        Color currentColor = image.color;
        image.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
    }

    private void eventText(string newText)
    {
        
        events.color = currentTextCol;
        events.text = newText;

    }


    // When adding new cutscenes, remember to add a new level to the list on the level manager, found in the main menu
    public void StartScene(string sceneName)
    {
        cutsceneName = sceneName;
        fadeImage.color = new Color(0,0,0,1f);
        dialogue.text = "";
        voices.text = "";
        events.text = "";
        cutsceneStage = 0;

        switch (cutsceneName)
        {
            case "Cutscene_PreBossOne":
                cutsceneNum = 1;
                CurrentCutsceneImages = cutScene1Images;
                drawingImage.sprite = cutScene1Images[0];
                fade();
                break;
            case "Cutscene_PostBossOne":
                cutsceneNum = 2;
                CurrentCutsceneImages = cutScene2Images;
                cutsceneImage.sprite = cutScene2Images[0];
                updateImage(0);
                
                break;
            case "Cutscene_PreBossHeadmaster":
                cutsceneNum = 3;
                CurrentCutsceneImages = cutScene3Images;
                updateImage(0);
                break;
            case "Cutscene_PostBossHeadmaster":
                cutsceneNum = 4;
                CurrentCutsceneImages = cutScene4Images;
                updateImage(0);
                break;
            case "Cutscene_PreBossPuzzleBox":
                cutsceneNum = 5;
                CurrentCutsceneImages = cutScene5Images;
                updateImage(0);
                break;
            case "Cutscene_PostBossPuzzleBox":
                cutsceneNum = 6;
                CurrentCutsceneImages = cutScene6Images;
                updateImage(0);
                break;
            default:
                Debug.Log($"<color=red>Error: {cutsceneName} cutscene not found. Setting to intro scene...</color>");
                cutsceneNum = 1;
                break;
        }

        next.GetComponent<Button>().onClick.AddListener(() => {
            nextStage();
        });

        skipScene.GetComponent<Button>().onClick.AddListener(() => {
            skip();
        });
    }

    //This will change the scene
    private void skip()
    {
        Debug.Log($"Skipping cutscene and changing scene...");
        LevelManager.Instance.ToNextLevel();

    }

    private void nextStage()
    {
        Debug.Log($"Changing Cutscene stage...");

        //Update the current stage of the cutscene and call the function associated to the cutsceneNum this script is using
        //Which would have been sent by the previous scene
        cutsceneStage++;


        //If a coroutine is running, finish the routine and continue the cutscene.
        if (coroutineRunning)
        { 
            StopAllCoroutines();

            if (modifiedObject != null)
            { 
                modifiedObject.transform.position = originalPosition;
                modifiedObject.transform.rotation = originalRotation;
            }

            fadeImage.color = targetColor;

            coroutineRunning = false;
        }

        targetColor = fadeImage.color;

        switch (cutsceneNum)
        {
            case 1: progressPreBusDriver();
                break;
            case 2: progressPostBusDriver();
                break;
            case 3: progressPreHeadmaster();
                break;
            case 4: progressPostHeadmaster();
                break;
            case 5:
                progressPrePuzzlebox();
                break;
            case 6:
                progressPostPuzzlebox();
                break;
            default:
                Debug.Log($"<color=red>Error: {cutsceneNum} is not in scope of available cutscenes.</color>");
                break;
        }

    }

    private void progressPreBusDriver()
    {
        switch (cutsceneStage)
        {
            case 0:
                Debug.Log("Moving to next scene.");

                break;
            case 1:
                currentTextCol = Color.white;
                updateText("<i>sigh</i>, the road's really foggy today.");
                break;
            case 2:
                updateText("Reminds me of that drawing I made the other day. A lot more foggy though. Wonder if I could still find that doodle.");
                break;
            case 3:
                updateDrawing(1);
                updateText("Though, I was hoping to get home before that pizza arrived, not after.");
                break;
            case 4:
                updateText("But... so long as the kids are safe, I guess that's all that matters. ");
                break;
            case 5:
                updateText("");
                updateDrawing(0);
                break;
            case 6:
                updateDrawing(6);
                currentTextCol = Color.black;
                updateText("What the - why is the fog getting thicker?");
                break;
            case 7:
                voices.text = "Children are safe";
                StartCoroutine(Shake(voices.gameObject, 400f));
                StartCoroutine(Tilt(voices.gameObject));
                break;
            case 8:
                voices.text = "";
                updateText("Huh? Who said that?? And what do you mean the children are safe?");
                break;
            case 9:
                toggleTutorialImage();
                updateTutorial(7);
                changeImageAlpha(tutorial, .1f);
                updateText("The... children... are safe?");
                break;
            case 10:
                changeImageAlpha(tutorial, 0f);
                updateText("The children... are safe. You're right...");
                break;
            case 11:
                updateText("They're safe.");
                fadeImage.color = new Color(0f, 0f, 0f, .16f);
                fadeImage.color = new Color(0f, 0f, 0f, .33f);
                break;
            case 12:
                changeImageAlpha(tutorial, .25f);
                updateText("They're safe... ");
                fadeImage.color = new Color(0f, 0f, 0f, .498f);
                break;
            case 13:
                currentTextCol = Color.white;
                changeImageAlpha(tutorial, .5f);
                updateText("Safe...");
                fadeImage.color = new Color(0f, 0f, 0f, .664f);
                break;
            case 14:
                changeImageAlpha(tutorial, .75f);
                updateText("I'm safe.");
                fadeImage.color = new Color(0f, 0f, 0f, .830f);
                break;
            case 15:
                changeImageAlpha(tutorial, 1f);
                updateText("You're safe");
                fadeImage.color = new Color(0f, 0f, 0f, 1f);
                break;
            case 16:
                updateText("We're safe.");
                break;
            case 17:
                updateText("");
                break;
            case 18:
                toggleTutorialImage();
                updateText("");
                currentTextCol = Color.white;
                eventText("CRASH");
                StartCoroutine(Shake(events.gameObject, 100f));
                break;
            case 19:
                eventText("");
                currentTextCol = popColor;
                updateText($"Ugghhhh");
                break;
            case 20:
                currentTextCol = gothColor;
                updateText($"You okay <color={popSColor}>Jacklyn</color>?");
                break;
            case 21:
                currentTextCol = popColor;
                updateText($"I think so. My harmonica bruised my side a bit, but other than that, it could be worse... I guess...");
                break;
            case 22:
                currentTextCol = gothColor;
                updateText($"Well, at least it wasn't a switch blade.");
                break;
            case 23:
                currentTextCol = popColor;
                updateText($"Yeah... Wait, <color={gothSColor}>Charles</color>, what do you mean by that?? Do you <i>have</i> a switchblade?");
                break;
            case 24:
                currentTextCol = gothColor;
                updateText($"<i>ahem</i>");
                break;
            case 25:
                updateText($"...Yo, <color={nerdSColor}>Zoey</color>, you doing okay over there?");
                break;
            case 26:
                currentTextCol = nerdColor;
                updateText($"<i>Urgh</i>, yeah... Just give me a minute.");
                break;
            case 27:
                updateText($"WAIT");
                break;
            case 28:
                updateText("");
                break;
            case 29:
                updateText($"Oh thank god, my keyboard's fine. I just bought this thing, jeez. I was hoping to get home before I broke it in.");
                break;
            case 30:
                updateText($"Ummm, where's the bus driver?");
                break;
            case 31:
                currentTextCol = jockColor;
                updateText($"I found his notebook over here.");
                break;
            case 32:
                currentTextCol = nerdColor;
                updateText($"<color={jockSColor}>Johny</color>, you are way too quick to search through his stuff. Did we not <i>just</i> crash a few minutes ago?");
                break;
            case 33:
                currentTextCol = jockColor;
                updateText($"Eh, what can I say - my blood is pumping. Plus, this fog isn't helping.");
                break;
            case 34:
                currentTextCol = nerdColor;
                updateText($"Yeah, maybe let's head outside and see if we can find him. I'd rather get home in one piece.");
                break;
            case 35:
                currentTextCol = gothColor;
                updateText($"Agreed.");
                break;
            case 36:
                updateText("");
                break;
            case 37:
                currentTextCol = jockColor;
                updateText($"...Hey, there he is!");
                break;
            case 38:
                currentTextCol = Color.grey;
                updateText($"Kids... <i>are</i>... safe... Kids... are... <i>safe</i>...");
                break;
            case 39:
                currentTextCol = jockColor;
                updateText($"I... don't like that look in his eyes.");
                break;
            case 40:
                currentTextCol = Color.grey;
                updateText($"Kids... Come here... You're <i>safe</i> now... Join me... Join <i>us</i>...");
                break;
            case 41:
                currentTextCol = jockColor;
                updateText($"Whoa, hey there bucko, that's a <i>little</i> too close, even for hockey standards.");
                break;
            case 42:
                currentTextCol = Color.grey;
                updateText($"Come");
                break;
            case 43:
                updateText($"HERE");
                StartCoroutine(Shake(dialogue.gameObject, 4f));
                break;
            case 44:
                currentTextCol = jockColor;
                updateText("Hey, <i>hey</i>, back UP.");
                break;
            case 45:
                currentTextCol = jockColor;
                updateText($"Ok ok, so that didn't seem to phase you. Time to bail!");
                break;
            case 46:
                currentTextCol = jockColor;
                updateText($"Guys, let's go please!");
                break;
            case 47:
                updateText("");
                break;
            case 48:
                updateText($"<i>Haa</i>, I think we lost him. He's definitely a no go, we need to figure something out.");
                break;
            case 49:
                currentTextCol = nerdColor;
                updateText("Can you pass me that notebook?");
                break;
            case 50:
                currentTextCol = jockColor;
                updateText("Uhh, sure?");
                break;
            case 51:
                currentTextCol = nerdColor;
                updateText($"Here, let me write down what we can do. It'll help for planning.");
                break;
            case 52:
                currentTextCol = jockColor;
                updateText("What we <i>should</i> do is run away.");
                break;
            case 53:
                currentTextCol = nerdColor;
                updateText("Yeah, but what if he finds us? Let's just prep for the worse, since this fog will be hard to navigate through anyway.");
                break;
            case 54:
                updateText($"Look, I'll keep track of us using these scraps of paper.");
                break;
            case 55:
                toggleTutorialImage();
                updateTutorial(2);
                updateText($"This is the driver. I doodled an eye to represent whatever is affecting him.");
                break;
            case 56:
                updateTutorial(3);
                updateText($"And this is what we look like. If one of you wants to do something first, move these around to represent the <color=white>order of our actions</color>.");
                break;
            case 57:
                updateText($"These thumbtacks should keep them in place and make it easier to track.");
                break;
            case 58:
                currentTextCol = gothColor;
                updateText($"Okaayyy? Well, if we're writing stuff down, here-");
                break;
            case 59:
                updateTutorial(4);
                updateText($"This eye will represent us, and how much we've turned.");
                break;
            case 60:
                currentTextCol = nerdColor;
                updateText("Turned? <i>Turned</i>?? What do you think this is - some zombie virus?");
                break;
            case 61:
                currentTextCol = gothColor;
                updateText($"Hey, I just want to cover our bases. If I start feeling funny, I'll be increasing mine. I suggest you all do the same.");
                break;
            case 62:
                updateText($"The higher it gets, the harder I can imagine it'll be for me to resist turning into a zombie.");
                break;
            case 63:
                currentTextCol = nerdColor;
                updateText($"Jesus, you're ridiculous. So what, you're claiming to be trying to <color=white>resist whatever force</color> is affecting the driver?");
                break;
            case 64:
                currentTextCol = gothColor;
                updateText($"Yeah. I am. So as long as I <color=white>keep that percentage low</color>, it'll be a piece of cake. Got a problem with that?");
                break;
            case 65:
                updateTutorial(5);
                updateText($"And this fun little ball of purple will be what I use as I laugh at you trying to resist.");
                break;
            case 66:
                currentTextCol = jockColor;
                updateText($"Uhh, guys, we have company.");
                break;
            case 67:
                currentTextCol = Color.grey;
                updateText("Safe... Safe... Safe... Safe...Safe... Safe... Safe... Safe...Safe... Safe... Safe... Safe...Safe... Safe... Safe... Safe...");
                voices.text = ("Children are safe");
                StartCoroutine(Shake(voices.gameObject, 300f));
                break;
            case 68:
                voices.text = ("");
                currentTextCol = popColor;
                updateText("Really good choice, running back to the bus. Where we could easily be found in this fog.");
                break;
            case 69:
                updateText("Y'know, given that there are bright headlights lighting this place up like a beacon.");
                break;
            case 70:
                currentTextCol = jockColor;
                updateText("WHATEVER! Bring out that notebook, let's try and knock him out.");
                break;
            case 71:
                skip();
                break;
            default:
                Debug.Log($"<color=red>Error: {cutsceneStage} is not in scope of available stages for cutscene #{cutsceneNum}</color>");
                break;

        }
        
    }


    private void progressPostBusDriver()
    {

        switch (cutsceneStage)
        {
            case 1:
                currentTextCol = jockColor;
                updateText("Is he... dead?");
                break;
            case 2:
                updateImage(1);
                currentTextCol = nerdColor;
                updateText("And what's with those runes on the floor?");
                break;
            case 3:
                currentTextCol = gothColor;
                updateText("Oooh, I'll write them down. I can finally say I've seen some daarrrkk stuff.");
                break;
            case 4:
                currentTextCol = nerdColor;
                updateText("That... Is a huge waste of time.");
                break;
            case 5:
                updateImage(0);
                updateText("");
                break;
            case 6:
                updateText("...The driver seems to be still alive though, I can feel his heartbeat. Faint, but present.");
                break;
            case 7:
                updateText("Hmm. I don't want to just leave him here.");
                break;
            case 8:
                currentTextCol = gothColor;
                updateText("It's not a phase... not a phase... not a phase...");
                break;
            case 9:
                currentTextCol = nerdColor;
                updateText("?");
                break;
            case 10:
                currentTextCol = gothColor;
                updateText("Heh, these runes are more useful than I thought. They change into actual things we can say. So if we read them out, we can do some preeeetty neat stuff.");
                break;
            case 11:
                updateText("Not sure why they changed into our language, but whatever.");
                break;
            case 12:
                updateText("I'm writing more of these down.");
                break;
            case 13:
                currentTextCol = nerdColor;
                updateText("<i>sigh</i>, well, it's getting darker, so let's start heading back to the school.");
                break;
            case 14:
                updateText("It's not too far off - I know the way from here.");
                break;
            case 15:
                currentTextCol = popColor;
                updateText("Well, I suggest we get writing. Then, head back to the school.");
                break;
            case 16:
                currentTextCol = nerdColor;
                updateText("Fine.");
                break;
            case 17:
                skip();
                break;
            default:
                Debug.Log($"<color=red>Error: {cutsceneStage} is not in scope of available stages for cutscene #{cutsceneNum}</color>");
                break;
        }
    }

    private void progressPreHeadmaster()
    {

        switch (cutsceneStage)
        {
            case 1:
                currentTextCol = popColor;
                updateText("Hey, there's the school!");
                break;
            case 2:
                updateText("Do you... think everyone else is okay?");
                break;
            case 3:
                currentTextCol = jockColor;
                updateText("Hopefully. My hockey team had a meetup today. I... I'm worried about them.");
                break;
            case 4:
                //updateImage(1);
                break;
            case 5:
                currentTextCol = gothColor;
                updateText("Despite the late nights I've had here, this is the deadest I've <i>ever</i> seen this school.");
                break;
            case 6:
                currentTextCol = popColor;
                updateText("Can we stop by the washroom? My makeup could use a bit of a touch-up after that walking...");
                break;
            case 7:
                currentTextCol = nerdColor;
                updateText("Is that really your concern right now?");
                break;
            case 8:
                currentTextCol = Color.white;
                eventText("Step... step.... step...");
                break;
            case 9:
                currentTextCol = Color.white;
                eventText($"<color={popSColor}???</color> <color={jockSColor}???</color> <color={nerdSColor}???</color> <color={gothSColor}???</color>");
                break;
            case 10:
                //updateImage(1);
                break;
            case 11:
                updateText("Now what do we have here...?");
                break;
            case 12:
                updateText("Students who haven't joined us?");
                break;
            case 13:
                updateText("It's time to change that...");
                break;
            case 14:
                skip();
                break;
            default:
                Debug.Log($"<color=red>Error: {cutsceneStage} is not in scope of available stages for cutscene #{cutsceneNum}</color>");
                break;
        }
    }

    private void progressPostHeadmaster()
    {

        switch (cutsceneStage)
        {
            case 1:
                currentTextCol = jockColor;
                updateText("Another one, huh? What's going on???");
                break;
            case 2:
                currentTextCol = popColor;
                updateText("If we're gonna have to face more of these people, can I at least go get something from the music room?");
                break;
            case 3:
                updateText("I'm sure I can find something a bit more lethal than this harmonica...");
                break;
            case 4:
                currentTextCol = gothColor;
                updateText("Couldn't you grab something more lethal than an instrument?");
                break;
            case 5:
                updateText("Like, oh, I dunno, a baseball bat??");
                break;
            case 6:
                updateText("I mean, jeez, at this rate, I'd take that over this knife. Really hard to hurt something otherworldly with a switchblade.");
                break;
            case 7:
                currentTextCol = nerdColor;
                updateText("Hey, I hear something... coming from downstairs");
                break;
            case 8:
                updateText("...Let's pick up some of these runes first, then go check it out.");
                break;
            case 9:
                updateText($"I doubt finding the janitor's master key will be easy anyway, so getting more weapons is off the table.");
                break;
            case 10:
                updateText($"Besides, these runes seem to be a lot stronger.");
                break;
            case 11:
                currentTextCol = gothColor;
                updateText("Told ya.");
                break;
            case 12:
                skip();
                break;
            default:
                Debug.Log($"<color=red>Error: {cutsceneStage} is not in scope of available stages for cutscene #{cutsceneNum}</color>");
                break;
        }
    }

    private void progressPrePuzzlebox()
    {

        switch (cutsceneStage)
        {
            case 1:
                currentTextCol = popColor;
                updateText("This is the place, yeah?");
                break;
            case 2:
                currentTextCol = nerdColor;
                updateText("Yeah. I'm sure of it.");
                break;
            case 3:
                currentTextCol = gothColor;
                updateText("And how can you be so sure? I mean, you doubted the runes, so why should I believe in your confidence?");
                break;
            case 4:
                currentTextCol = jockColor;
                updateText("Hey, we're in this together, ok?");
                break;
            case 5:
                updateText("Let's just check and find out. Not like it's a good idea to wander away from the school in this fog anyway.");
                break;
            case 6:
                currentTextCol = popColor;
                updateText("And we don't even have reception, so what else are we gonna do? Sit around and wait till more creeps show up and kill us?");
                break;
            case 7:
                currentTextCol = gothColor;
                updateText("<i>haaaa</i>, fair... fine, let's check it out.");
                break;
            case 8:
                currentTextCol = nerdColor;
                updateText("Thank you.");
                break;
            case 9:
                updateText($"");
                break;
            case 10:
                break;
            case 11:
                break;
            case 12:
                break;
            case 13:
                eventText($"<color={nerdSColor}!!!</color> <color={gothSColor}!!!</color> <color={popSColor}!!!</color> <color={jockSColor}!!!</color>");
                break;
            case 14:
                updateText("Wha... what the heck is it?");
                break;
            case 15:
                skip();
                break;
            default:
                Debug.Log($"<color=red>Error: {cutsceneStage} is not in scope of available stages for cutscene #{cutsceneNum}</color>");
                break;
        }
    }


    private void progressPostPuzzlebox()
    {

        switch (cutsceneStage)
        {
            case 1:
                currentTextCol = popColor;
                updateText("This is the place, yeah?");
                break;
            default:
                Debug.Log($"<color=red>Error: {cutsceneStage} is not in scope of available stages for cutscene #{cutsceneNum}</color>");
                break;
        }
    }

}
