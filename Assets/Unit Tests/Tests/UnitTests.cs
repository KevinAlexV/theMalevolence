using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitTests
{
    /*
        Tests:
        - Draw
        - Shuffle
        - Reshuffle
        - Damage
    
    */
    // A Test behaves as an ordinary method
    [Test]
    public void DamageTest()
    {
        // Use the Assert class to test conditions
        int min = 1;
        int max = 6;
        Damage damage;
        for(int i = 0; i < 100; i++){
            damage = new Damage(min, max, 0);
            int roll = damage.Value;
            if(roll > max || roll < min){
                Assert.Fail();
            }
        }
        Assert.Pass();
    }

    [Test]
    public void DrawTest(){
        try{
            Deck deck = new Deck(new List<Card>());
            for(int i = 0; i < 10; i++){
                var card = ScriptableObject.CreateInstance<Card>();
                card.name = i.ToString();
                deck.AddCard(card);
            }
            var drawn = deck.Draw();
            Assert.False(deck.CardList.Contains(drawn));
        } catch(System.Exception e){
            Debug.Log(e);
        }
        
    }

    [Test]
    public void ShuffleTest(){
        try{
            Deck deck = new Deck(new List<Card>());
            for(int i = 0; i < 1; i++){
                var card = ScriptableObject.CreateInstance<Card>();
                card.name = i.ToString();
                deck.AddCard(card);
            }
            var cardOrder = new List<Card>(deck.CardList);
            deck.Shuffle();
            Assert.IsFalse(cardOrder.Equals(deck.CardList));
        } catch(System.Exception e){
            Debug.Log(e);
            Assert.Fail();
        }
        
    }

    [Test]
    public void ReshuffleTest(){
        try{
            Deck deck = new Deck(new List<Card>());
            for(int i = 0; i < 10; i++){
                var card = ScriptableObject.CreateInstance<Card>();
                card.name = i.ToString();
                deck.AddCard(card);
                deck.DiscardList.Add(deck.Draw());
            }
            Assert.IsTrue(deck.CardList.Count == 0);
            Assert.IsTrue(deck.DiscardList.Count == 10);
            deck.Reshuffle();
            Assert.IsTrue(deck.CardList.Count == 10);
            Assert.IsTrue(deck.DiscardList.Count == 0);
        } catch(System.Exception e){
            Debug.Log(e);
        }
        
    }
}
