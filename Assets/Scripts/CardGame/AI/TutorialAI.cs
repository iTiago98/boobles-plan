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
            if (_contender.currentMana > 0)
            {
                List<Card> playableCards = new List<Card>();
                CardZone emptyCardZone = RandomEmptyCardZone();

                foreach (GameObject cardObj in _contender.hand.cards)
                {
                    Card card = cardObj.GetComponent<Card>();

                    if (card.manaCost > _contender.currentMana) continue;

                    switch (card.type)
                    {
                        case CardType.ARGUMENT:
                            if (emptyCardZone) playableCards.Add(card);
                            break;
                        case CardType.ACTION:
                            if (card.hasEffect && card.effect.IsAppliable()) playableCards.Add(card);
                            break;
                        case CardType.FIELD:
                            break;
                    }
                }

                if (playableCards.Count == 0) SkipTurn();
                else
                {
                    int index = new System.Random().Next(0, playableCards.Count);
                    Card card = playableCards[index];

                    switch (card.type)
                    {
                        case CardType.ARGUMENT:
                            PlayArgument(card, emptyCardZone);
                            break;
                        case CardType.ACTION:
                            PlayAction(card);
                            break;
                    }
                }
            }
            else
            {
                SkipTurn();
            }
        }

        //public override void Play()
        //{
        //    CardZone emptyCardZone = RandomEmptyCardZone();
        //    if (_contender.currentMana > 0 && emptyCardZone)
        //    {
        //        bool cardPlayed = false;
        //        foreach (GameObject cardObj in _contender.hand.cards)
        //        {
        //            Card card = cardObj.GetComponent<Card>();

        //            if (card.manaCost <= _contender.currentMana && card.type == CardType.ARGUMENT)
        //            {
        //                PlayArgument(card, emptyCardZone);
        //                cardPlayed = true;
        //                break;
        //            }
        //        }

        //        if (!cardPlayed) SkipTurn();
        //    }
        //    else
        //    {
        //        SkipTurn();
        //    }
        //}
    }
}