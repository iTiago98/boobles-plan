using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.AI
{
    public class TutorialAI : BaseAI
    {
        public override void Play()
        {
            CardZone emptyCardZone = RandomEmptyCardZone();
            if (_contender.currentMana > 0 && emptyCardZone)
            {
                bool cardPlayed = false;
                foreach (GameObject cardObj in _contender.hand.cards)
                {
                    Card card = cardObj.GetComponent<Card>();

                    if (card.manaCost <= _contender.currentMana && card.type == CardType.ARGUMENT)
                    {
                        PlayCard(card, emptyCardZone);
                        cardPlayed = true;
                        break;
                    }
                }

                if (!cardPlayed) SkipTurn();
            }
            else
            {
                SkipTurn();
            }
        }
    }
}