using NUnit.Framework;
using Altom.AltUnityDriver;
using System.Collections.Generic;

public class UITests
{
    public AltUnityDriver altUnityDriver;
    //Before any test it connects with the socket
    [OneTimeSetUp]
    public void SetUp()
    {
        altUnityDriver =new AltUnityDriver();
    }

    //At the end of the test closes the connection with the socket
    [OneTimeTearDown]
    public void TearDown()
    {
        altUnityDriver.Stop();
    }

    [Test]
    public void EndPhaseTest()
    {
	//Here you can write the test
        altUnityDriver.LoadScene("BossOne");
        altUnityDriver.FindObject(By.NAME, "EndPhaseButton").Click();
        //var gm = altUnityDriver.FindObject(By.NAME, "Gample")
        if(GameManager.manager.phase == Enums.GameplayPhase.Resolve){
            Assert.Pass();
        } else {
            Assert.Fail();
        }
    }

    [Test]
    public void DraftingTest(){
        altUnityDriver.LoadScene("Main Menu");
        altUnityDriver.LoadScene("BossOne");
        altUnityDriver.LoadScene("DeckBuilder");
        //Get all card displays in 
        var cards = altUnityDriver.FindObjects(By.PATH, "//Main Camera/DeckBuildCanvas/DraftContainer/DraftMask/DraftDisplay/*");
        
        cards[0].Click();
        cards[1].Click();
        cards[2].Click();
        Assert.Pass();
        
        
        var confirmDraftButton = altUnityDriver.FindObject(By.NAME, "EndDraftButton");
        confirmDraftButton.Click();
        var continueButton = altUnityDriver.FindObject(By.NAME, "ExitButton");
        continueButton.Click();
        altUnityDriver.WaitForCurrentSceneToBe("BossHeadmaster");
    }

    [Test]
    public void AttackTest(){
        altUnityDriver.LoadScene("Main Menu");
        altUnityDriver.LoadScene("BossOne");
        altUnityDriver.FindObject(By.NAME, "EndPhaseButton").Click();
        var boss = altUnityDriver.FindObject(By.NAME, "Boss");
        int bossHealth = System.Int32.Parse(boss.GetComponentProperty("DriverCharacter", "Health"));
        boss.Click();//Attack the boss
        int newHealth = System.Int32.Parse(boss.GetComponentProperty("DriverCharacter", "Health"));
        Assert.That((bossHealth - newHealth) < 7 && (bossHealth - newHealth) > 0 );
    }

    [Test]
    public void PlayCardTest(){
        altUnityDriver.LoadScene("Main Menu");
        altUnityDriver.LoadScene("BossOne");
        var card = altUnityDriver.FindObjects(By.COMPONENT, "CardDisplayController")[0];
        var id = card.id;
        var dropzone = altUnityDriver.FindObject(By.NAME, "DropZone");
        altUnityDriver.MoveMouseAndWait(new AltUnityVector2(500f, 200f), 0.5f);
        AltUnityKeyCode kcode = AltUnityKeyCode.Mouse0;
        altUnityDriver.KeyDown(kcode, 1);
        altUnityDriver.MoveMouseAndWait(new AltUnityVector2(500f, 800f), 0.5f);
        altUnityDriver.KeyUp(kcode);
        altUnityDriver.FindObject(By.NAME, "Boss").Click();
        altUnityDriver.WaitForObjectNotBePresent(By.ID, id.ToString());
    }

}