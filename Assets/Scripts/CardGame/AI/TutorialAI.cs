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
            if (_contender.currentMana > 0 || _contender.freeMana)
            {
                List<Card> playableCards = new List<Card>();
                CardZone emptyCardZone = RandomEmptyCardZone();
                CardZone fieldCardZone = FieldCardZone();

                foreach (GameObject cardObj in _contender.hand.cards)
                {
                    Card card = cardObj.GetComponent<Card>();

                    if (card.manaCost > _contender.currentMana && !_contender.freeMana) continue;

                    switch (card.type)
                    {
                        case CardType.ARGUMENT:
                            if (emptyCardZone) playableCards.Add(card);
                            break;
                        case CardType.ACTION:
                            if (card.hasEffect && card.effect.IsAppliable()) playableCards.Add(card);
                            break;
                        case CardType.FIELD:
                            if (fieldCardZone) playableCards.Add(card);
                            break;
                    }
                }

                if (playableCards.Count == 0) SkipTurn();
                else
                {
                    int index = new System.Random().Next(0, playableCards.Count);
                    Card card = playableCards[index];
                    CardZone cardZone = null;

                    switch (card.type)
                    {
                        case CardType.ARGUMENT:
                            cardZone = emptyCardZone;
                            break;
                        case CardType.ACTION:
                            break;
                        case CardType.FIELD:
                            cardZone = fieldCardZone;
                            break;
                    }

                    PlayCard(card, cardZone);
                }
            }
            else
            {
                SkipTurn();
            }
        }
    }
}