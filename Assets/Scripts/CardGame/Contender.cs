using DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Contender : MonoBehaviour
{
    public enum Role
    {
        PLAYER, OPPONENT
    }

    public Role role;

    public CardsDataContainer deckCards;

    public int eloquence { private set; get; }
    public int currentMana { private set; get; }
    public int currentMaxMana { private set; get; }

    //[Header("Close Up")]
    //public Transform closeUpPosition;
    //public float closeUpScaleMultiplier;
    //public float closeUpTime;

    public Hand hand { private set; get; }
    public List<CardZone> cardZones { private set; get; }

    public void Initialize(Hand hand, List<CardZone> cardZone)
    {
        this.hand = hand;
        this.cardZones = cardZone;
    }

    public void InitializeStats(int initialEloquence, int initialManaCounter)
    {
        eloquence = initialEloquence;
        currentMaxMana = initialManaCounter;
        currentMana = currentMaxMana;
        UpdateManaUI();
    }

    public void FillMana()
    {
        if (currentMaxMana < TurnManager.Instance.settings.maxManaCounter)
        {
            currentMaxMana++;
        }
        currentMana = currentMaxMana;
        UpdateManaUI();
    }

    public void MinusMana(int cost)
    {
        currentMana -= cost;
        UpdateManaUI();
    }

    private void UpdateManaUI()
    {
        //manaUIText.text = "Mana: " + currentMana + "/" + currentMaxMana;

    }

    public void Hit(int strength)
    {
        eloquence -= strength;
    }
}
