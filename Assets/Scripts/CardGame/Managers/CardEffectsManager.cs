using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;
using CardGame.Level;
using Santi.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Managers
{
    public class CardEffectsManager : Singleton<CardEffectsManager>
    {
        private CardEffect _currentEffect;
        private int _storedValue;

        public void SetCurrentEffect(CardEffect effect)
        {
            _currentEffect = effect;
        }

        private void SetEffectApplied()
        {
            _currentEffect.SetEffectApplied();
        }

        #region Deffensive Effects

        #region Restore Life

        public void RestoreLife(int life, Contender contender)
        {
            StartCoroutine(RestoreLifeCoroutine(life, contender));
        }

        private IEnumerator RestoreLifeCoroutine(int life, Contender contender)
        {
            contender.RestoreLife(life);

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            SetEffectApplied();
        }

        #endregion

        #region Increase Max Mana

        public void IncreaseMaxMana(int mana, Contender contender)
        {
            StartCoroutine(IncreaseMaxManaCoroutine(mana, contender));
        }

        private IEnumerator IncreaseMaxManaCoroutine(int mana, Contender contender)
        {
            contender.IncreaseMaxMana(mana);

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            SetEffectApplied();
        }

        #endregion

        #endregion

        #region Offensive Effects

        #region Destroy Card

        public void DestroyCard(Card source, object target, Target targetType)
        {
            StartCoroutine(DestroyCardCoroutine(source, target, targetType));
        }

        private IEnumerator DestroyCardCoroutine(Card source, object target, Target targetType)
        {
            Card aux = null;
            Contender otherContender = CardGameManager.Instance.GetOtherContender(source.contender);

            switch (targetType)
            {
                case Target.ENEMY:
                case Target.CARD:
                    aux = (Card)target;
                    aux.DestroyCard();
                    break;

                case Target.AENEMY:
                    aux = DestroyCards(otherContender);
                    break;

                case Target.ACARD:
                    DestroyCards(source.contender);
                    aux = DestroyCards(otherContender);
                    break;

                case Target.FIELDCARD:
                    aux = otherContender.fieldCardZone.GetCard();
                    if (aux != null) aux.DestroyCard();
                    break;
            }

            yield return new WaitUntil(() => aux == null);

            SetEffectApplied();
        }

        private Card DestroyCards(Contender contender)
        {
            Card aux = null;
            foreach (CardZone zone in contender.cardZones)
            {
                if (zone.isNotEmpty)
                {
                    Card card = zone.GetCard();
                    aux = card;
                    card.DestroyCard();
                }
            }
            if (contender.fieldCardZone.isNotEmpty)
            {
                Card fieldCard = contender.fieldCardZone.GetCard();
                aux = fieldCard;
                fieldCard.DestroyCard();
            }

            return aux;
        }

        #endregion

        #region Deal Damage

        public void DealDamage(Card source, object target, Target targetType, int value)
        {
            StartCoroutine(DealDamageCoroutine(source, target, targetType, value));
        }

        private IEnumerator DealDamageCoroutine(Card source, object target, Target targetType, int value)
        {
            switch (targetType)
            {
                case Target.ENEMY:
                    Card card = (Card)target;
                    card.ReceiveDamage(value);

                    if (card.Stats.defense == 0)
                        yield return new WaitUntil(() => card == null);
                    else
                        yield return new WaitWhile(() => card.CardUI.IsPlayingAnimation);

                    break;

                case Target.PLAYER:
                    CardGameManager.Instance.GetOtherContender(source.contender).ReceiveDamage(value);
                    yield return new WaitUntil(() => UIManager.Instance.statsUpdated);
                    break;

                case Target.SELF:
                    source.contender.ReceiveDamage(value);
                    yield return new WaitUntil(() => UIManager.Instance.statsUpdated);
                    break;

                case Target.ALL:
                    {
                        Contender player = CardGameManager.Instance.player;
                        Contender opponent = CardGameManager.Instance.opponent;
                        Card aux = null;

                        if (!HasParaguas(source, player))
                        {
                            aux = HitCards(player, value);
                            player.ReceiveDamage(value);
                        }

                        if (!HasParaguas(source, opponent))
                        {
                            aux = HitCards(opponent, value);
                            opponent.ReceiveDamage(value);
                        }

                        if (aux != null) yield return new WaitWhile(() => aux.CardUI.IsPlayingAnimation);
                        break;
                    }
            }

            SetEffectApplied();
        }

        private bool HasParaguas(Card source, Contender contender)
        {
            return source.name == "�Va a llover!" && Board.Instance.HasCard(contender, "Abro paraguas");
        }

        private Card HitCards(Contender contender, int value)
        {
            Card aux = null;

            foreach (CardZone cardZone in contender.cardZones)
            {
                if (cardZone.isNotEmpty)
                {
                    Card card = cardZone.GetCard();
                    bool destroy = card.ReceiveDamage(value);

                    if (aux == null || destroy) aux = card;
                }
            }

            return aux;
        }

        #endregion

        #region Decrease Mana

        public void DecreaseMana(Card source, int value)
        {
            StartCoroutine(DecreaseManaCoroutine(source, value));
        }

        private IEnumerator DecreaseManaCoroutine(Card source, int value)
        {
            Contender contender = CardGameManager.Instance.GetOtherContender(source.contender);
            if (value == 0) value = contender.currentMana;

            contender.SubstractMana(value);

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            SetEffectApplied();
        }

        #endregion

        #endregion

        #region Boost Effects

        #region Combat

        #region Lifelink

        public void Lifelink(Card source, object target)
        {
            StartCoroutine(LifelinkCoroutine(source, target));
        }

        private IEnumerator LifelinkCoroutine(Card source, object target)
        {
            int value = 0;
            if (target is Card)
            {
                Card targetCard = (Card)target;
                value = Mathf.Min(source.Stats.strength, targetCard.Stats.defense);
            }
            else if (target is Contender)
            {
                value = source.Stats.strength;
            }
            source.contender.RestoreLife(value);

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            SetEffectApplied();
        }

        #endregion

        #region Rebound

        public void Rebound(Card source, object target)
        {
            StartCoroutine(ReboundCoroutine(source, target));
        }

        private IEnumerator ReboundCoroutine(Card source, object target)
        {
            if (target is Card)
            {
                Card aux = null;
                bool destroy = false;

                Card targetCard = (Card)target;
                int reboundValue = Mathf.Min(source.Stats.defense, targetCard.Stats.strength);

                if (targetCard.Effects.HasEffect(SubType.REBOUND))
                {
                    if (_storedValue == -1)
                        _storedValue = reboundValue + source.Stats.strength;
                    else
                    {
                        if (targetCard.ReceiveDamage(reboundValue + source.Stats.strength)) SetDestroy(ref aux, ref destroy, targetCard);
                        if (source.ReceiveDamage(_storedValue)) SetDestroy(ref aux, ref destroy, source);
                        _storedValue = -1;
                    }
                }
                else
                {
                    if (targetCard.ReceiveDamage(reboundValue + source.Stats.strength)) SetDestroy(ref aux, ref destroy, targetCard);
                    if (source.ReceiveDamage(targetCard.Stats.strength)) SetDestroy(ref aux, ref destroy, source);
                }

                if (source.IsPlayerCard)
                {
                    CardGameManager.Instance.alternateWinConditionParameter += reboundValue;
                }

                if (aux == null) aux = source;

                if (destroy) yield return new WaitUntil(() => aux == null);
                else yield return new WaitWhile(() => aux.CardUI.IsPlayingAnimation);
            }

            SetEffectApplied();
        }

        private void SetDestroy(ref Card aux, ref bool destroy, Card card)
        {
            destroy = true;
            aux = card;
        }

        #endregion

        #region Trample

        public void Trample(Card source, object target)
        {
            StartCoroutine(TrampleCoroutine(source, target));
        }

        private IEnumerator TrampleCoroutine(Card source, object target)
        {
            if (target is Card)
            {
                Card targetCard = (Card)target;
                int lifeValue = source.Stats.strength - targetCard.Stats.defense;
                CardGameManager.Instance.GetOtherContender(source.contender).ReceiveDamage(lifeValue);

                yield return new WaitUntil(() => UIManager.Instance.statsUpdated);
            }

            SetEffectApplied();
        }

        #endregion

        #region Sponge

        public void Sponge(Card source, object target)
        {
            StartCoroutine(SpongeCoroutine(source, target));
        }

        private IEnumerator SpongeCoroutine(Card source, object target)
        {
            source.Hit(target);
            if (target is Card)
            {
                Card targetCard = (Card)target;
                targetCard.Hit(source);
                source.BoostStats(targetCard.Stats.strength, 0);
            }

            yield return new WaitWhile(() => source.CardUI.IsPlayingAnimation);

            SetEffectApplied();
        }

        #endregion

        #region Compartmentalize

        public void Compartmentalize(Card source, object target)
        {
            StartCoroutine(CompartmentalizeCoroutine(source, target));
        }

        private IEnumerator CompartmentalizeCoroutine(Card source, object target)
        {
            if (target is Contender)
            {
                Contender contender = (Contender)target;
                contender.deck.DiscardCards(source.Stats.strength);

                yield return new WaitWhile(() => contender.deck.busy);
            }

            SetEffectApplied();
        }

        #endregion

        #endregion

        #region Stat Boost

        public void StatBoost(Card source, object target, Target targetType, int strengthValue, int defenseValue)
        {
            StartCoroutine(StatBoostCoroutine(source, target, targetType, strengthValue, defenseValue));
        }

        private IEnumerator StatBoostCoroutine(Card source, object target, Target targetType, int strengthValue, int defenseValue)
        {
            Card aux = null;

            switch (targetType)
            {
                case Target.ALLY:
                case Target.ENEMY:
                    aux = (Card)target;
                    aux.BoostStats(strengthValue, defenseValue);
                    break;

                case Target.AALLY:
                    foreach (Card card in Board.Instance.GetCardsOnTable(source.contender))
                    {
                        card.BoostStats(strengthValue, defenseValue);
                        if (aux == null) aux = card;
                    }
                    break;

                case Target.AENEMY:
                    Contender otherContender = CardGameManager.Instance.GetOtherContender(source.contender);
                    foreach (Card card in Board.Instance.GetCardsOnTable(otherContender))
                    {
                        card.BoostStats(strengthValue, defenseValue);
                        if (aux == null) aux = card;
                    }
                    break;

                case Target.SELF:
                    aux = source;
                    source.BoostStats(strengthValue, defenseValue);
                    break;
            }

            if (aux != null) yield return new WaitWhile(() => aux.CardUI.IsPlayingAnimation);

            SetEffectApplied();
        }

        #endregion

        #region Stat Decrease

        public void StatDecrease(Card source, object target, Target targetType, int strengthValue, int defenseValue)
        {
            StartCoroutine(StatDecreaseCoroutine(source, target, targetType, strengthValue, defenseValue));
        }

        private IEnumerator StatDecreaseCoroutine(Card source, object target, Target targetType, int strengthValue, int defenseValue)
        {
            Card aux = null;

            switch (targetType)
            {
                case Target.ALLY:
                case Target.ENEMY:
                    aux = (Card)target;
                    aux.DecreaseStats(strengthValue, defenseValue);
                    break;

                case Target.AALLY:
                    foreach (Card card in Board.Instance.GetCardsOnTable(source.contender))
                    {
                        card.DecreaseStats(strengthValue, defenseValue);
                        if (aux == null) aux = card;
                    }
                    break;

                case Target.AENEMY:
                    Contender otherContender = CardGameManager.Instance.GetOtherContender(source.contender);
                    foreach (Card card in Board.Instance.GetCardsOnTable(otherContender))
                    {
                        card.DecreaseStats(strengthValue, defenseValue);
                        if (aux == null) aux = card;
                    }
                    break;

                case Target.SELF:
                    aux = source;
                    source.DecreaseStats(strengthValue, defenseValue);
                    break;
            }

            if (aux != null) yield return new WaitWhile(() => aux.CardUI.IsPlayingAnimation);

            SetEffectApplied();
        }

        #endregion

        #region Add Effect

        public void AddEffect(Card source, object target, Target targetType, CardEffect effect)
        {
            StartCoroutine(AddEffectCoroutine(source, target, targetType, effect));
        }

        private IEnumerator AddEffectCoroutine(Card source, object target, Target targetType, CardEffect effect)
        {
            Card aux = null;

            switch (targetType)
            {
                case Target.ALLY:
                case Target.ENEMY:
                case Target.CARD:
                    aux = (Card)target;
                    aux.Effects.AddEffect(effect);
                    break;

                case Target.AALLY:
                    foreach (Card card in Board.Instance.GetCardsOnTable(source.contender))
                    {
                        card.Effects.AddEffect(effect);
                        if (aux == null) aux = card;
                    }
                    break;
            }

            if (aux != null) yield return new WaitWhile(() => aux.CardUI.IsPlayingAnimation);

            SetEffectApplied();
        }

        #endregion

        #endregion

        #region Tactical Effects 

        #region Create Card

        public void CreateCard(Card source, object target, CardsData data)
        {
            StartCoroutine(CreateCardCoroutine(source, target, data));
        }

        private IEnumerator CreateCardCoroutine(Card source, object target, CardsData data)
        {
            CardZone emptyCardZone = Board.Instance.GetEmptyCardZone(source.contender);
            if (emptyCardZone != null)
            {
                Card card = InstantiateCard(source.contender, source.transform.position, data, cardRevealed: true);
                emptyCardZone.AddCard(card);

                yield return new WaitUntil(() => emptyCardZone.cardsAtPosition);
            }

            SetEffectApplied();
        }

        #endregion

        #region Swap Position

        public void SwapPosition(object target, Target targetType)
        {
            StartCoroutine(SwapPositionCoroutine(target, targetType));
        }

        private IEnumerator SwapPositionCoroutine(object target, Target targetType)
        {
            CardZone aux = null;

            if (targetType == Target.CARD)
            {
                Card targetCard = (Card)target;

                // Get origin position
                int pos = Board.Instance.GetPositionFromCard(targetCard);

                // Get destination
                List<int> possibleCardZones = new List<int>();
                if (pos > 0) possibleCardZones.Add(pos - 1);
                if (pos < 3) possibleCardZones.Add(pos + 1);

                int dest = possibleCardZones[UnityEngine.Random.Range(0, possibleCardZones.Count)];

                Swap(pos, dest, targetCard.contender);
                aux = targetCard.contender.cardZones[dest];
            }
            else if (targetType == Target.AENEMY)
            {
                Contender contender = CardGameManager.Instance.otherPlayer;
                List<CardZone> cardZones = contender.cardZones;
                int pos = -1;

                // Find any card
                foreach (CardZone cardZone in cardZones)
                {
                    if (cardZone.isNotEmpty)
                    {
                        pos = cardZones.IndexOf(cardZone);
                        break;
                    }
                }

                List<int> possibleCardZones = new List<int> { 0, 1, 2, 3 };
                possibleCardZones.Remove(pos);
                int destPosition = possibleCardZones[UnityEngine.Random.Range(0, possibleCardZones.Count)];

                Swap(pos, destPosition, contender);
                aux = contender.cardZones[destPosition];
            }

            yield return new WaitUntil(() => aux.cardsAtPosition);

            SetEffectApplied();
        }

        private void Swap(int pos1, int pos2, Contender contender)
        {
            CardZone originCardZone = contender.cardZones[pos1];
            CardZone destCardZone = contender.cardZones[pos2];

            Card originCard = originCardZone.GetCard();
            originCardZone.RemoveCard(originCard.gameObject);
            if (destCardZone.isNotEmpty)
            {
                Card temp = destCardZone.GetCard();
                destCardZone.RemoveCard(temp.gameObject);
                originCardZone.AddCard(temp);
            }
            destCardZone.AddCard(originCard);
        }

        #endregion

        #region Swap Contender

        public void SwapContender(Card source, Target targetType)
        {
            StartCoroutine(SwapContenderCoroutine(source, targetType));
        }

        private IEnumerator SwapContenderCoroutine(Card source, Target targetType)
        {
            if (targetType == Target.SELF)
            {
                CardZone emptyCardZone = Board.Instance.GetEmptyCardZone(CardGameManager.Instance.GetOtherContender(source.contender));
                if (emptyCardZone != null)
                {
                    SwapContender(source, source, emptyCardZone);

                    yield return new WaitUntil(() => emptyCardZone.cardsAtPosition);
                }
            }

            SetEffectApplied();
        }

        #endregion

        #region Draw Card

        public void DrawCard(int value, Contender contender)
        {
            StartCoroutine(DrawCardCoroutine(value, contender));
        }

        private IEnumerator DrawCardCoroutine(int value, Contender contender)
        {
            contender.deck.DrawCards(value);

            yield return new WaitWhile(() => contender.deck.busy);

            SetEffectApplied();
        }

        #endregion

        #region Discard Card

        public void DiscardCard(int value, Contender contender)
        {
            StartCoroutine(DiscardCardCoroutine(value, contender));
        }

        private IEnumerator DiscardCardCoroutine(int value, Contender contender)
        {
            contender.hand.DiscardCards(value);

            yield return new WaitWhile(() => contender.hand.busy);

            SetEffectApplied();
        }

        #endregion

        #region Return Card

        public void ReturnCard(object target)
        {
            StartCoroutine(ReturnCardCoroutine(target));
        }

        private IEnumerator ReturnCardCoroutine(object target)
        {
            if (target is Card)
            {
                Card card = (Card)target;
                card.ReturnToHand();

                yield return new WaitUntil(() => card.contender.hand.cardsAtPosition);
            }

            SetEffectApplied();
        }

        #endregion

        #region FreeMana

        public void FreeMana(Contender contender)
        {
            contender.SetFreeMana(true);
        }

        #endregion

        #region Wheel

        public void Wheel()
        {
            StartCoroutine(WheelCoroutine());
        }

        private IEnumerator WheelCoroutine()
        {
            Contender player = CardGameManager.Instance.player;
            Contender opponent = CardGameManager.Instance.opponent;

            int playerNumCards = player.hand.numCards;
            int opponentNumCards = opponent.hand.numCards;

            player.hand.DiscardAll();
            opponent.hand.DiscardAll();

            yield return new WaitUntil(() => Board.Instance.EmptyHands());

            player.deck.DrawCards(playerNumCards);
            opponent.deck.DrawCards(opponentNumCards);

            yield return new WaitUntil(() => player.hand.numCards == playerNumCards && opponent.hand.numCards == opponentNumCards);

            SetEffectApplied();
        }

        #endregion

        #region Skip Combat

        public void SkipCombat()
        {
            TurnManager.Instance.SkipCombat();
        }

        #endregion

        #region Mirror

        public void Mirror(Contender contender)
        {
            TurnManager.Instance.SetMirror(contender, true);
        }

        #endregion

        #region Steal Mana

        public void StealMana(Contender contender)
        {
            StartCoroutine(StealManaCoroutine(contender));
        }

        private IEnumerator StealManaCoroutine(Contender contender)
        {
            Contender otherContender = CardGameManager.Instance.GetOtherContender(contender);
            int manaValue = otherContender.currentMana;

            otherContender.SubstractMana(manaValue);
            contender.RestoreMana(manaValue);

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            SetEffectApplied();
        }

        #endregion

        #region Add Card To Deck

        public void AddCardToDeck(Card source, CardsData data)
        {
            StartCoroutine(AddCardToDeckCoroutine(source, data));
        }

        private IEnumerator AddCardToDeckCoroutine(Card source, CardsData data)
        {
            Contender owner = source.contender;

            int number = source.Stats.manaCost;
            if (source.Stats.manaCost == 0)
            {
                number = owner.currentMana;
                owner.SubstractMana(number);
            }

            List<Card> cards = new List<Card>();
            GameObject cardPrefab = owner.deck.GetCardPrefab(data.type);

            for (int i = 0; i < number; i++)
            {
                GameObject cardObj = Instantiate(cardPrefab, source.transform.position, cardPrefab.transform.rotation);
                cardObj.SetActive(false);   

                CardsData newData = new CardsData(data);
                Card card = cardObj.GetComponent<Card>();
                card.Initialize(owner, newData, cardRevealed: true);

                cards.Add(card);
            }

            owner.deck.AddCards(cards);

            yield return new WaitWhile(() => owner.deck.busy);

            SetEffectApplied();
        }

        #endregion

        #region Discard Card From Deck

        public void DiscardCardFromDeck(object target, int value, Contender contender)
        {
            StartCoroutine(DiscardCardFromDeckCoroutine(target, value, contender));
        }

        private IEnumerator DiscardCardFromDeckCoroutine(object target, int value, Contender contender)
        {
            int number = value;
            if (target != null) number = ((Card)target).Stats.manaCost;

            contender.deck.DiscardCards(number);

            yield return new WaitWhile(() => contender.deck.busy);

            SetEffectApplied();
        }

        #endregion

        #region Steal Card

        public void StealCard(Card source, object target)
        {
            StartCoroutine(StealCardCoroutine(source, target));
        }

        private IEnumerator StealCardCoroutine(Card source, object target)
        {
            if (target is Card)
            {
                Card targetCard = (Card)target;
                CardZone destCardZone = null;

                if (source.IsArgument && targetCard.Stats.strength >= source.Stats.defense)
                {
                    int position = Board.Instance.GetPositionFromCard(source);
                    destCardZone = source.contender.cardZones[position];
                }
                else if (source.IsAction)
                {
                    destCardZone = Board.Instance.GetEmptyCardZone(source.contender);
                }

                if (destCardZone != null)
                {
                    SwapContender(source, targetCard, destCardZone);
                    source.contender.stolenCards++;

                    yield return new WaitUntil(() => destCardZone.cardsAtPosition);
                }
            }

            SetEffectApplied();
        }

        #endregion

        #region Steal Card From Hand

        public void StealCardFromHand(Card source, int value)
        {
            StartCoroutine(StealCardFromHandCoroutine(source, value));
        }

        private IEnumerator StealCardFromHandCoroutine(Card source, int value)
        {
            Contender otherContender = CardGameManager.Instance.GetOtherContender(source.contender);
            int loops = Mathf.Min(value, otherContender.hand.numCards);

            int finalValue = source.contender.hand.numCards + loops;

            for (int i = 0; i < loops; i++)
            {
                Card card = otherContender.hand.StealCard();
                Card newCard = InstantiateCard(source.contender, card.transform.position, card.data, source.IsPlayerCard);
                source.contender.hand.AddCard(newCard);
                source.contender.stolenCards++;
                card.DestroyCard(instant: true);
            }

            yield return new WaitUntil(() => source.contender.hand.numCards == finalValue && source.contender.hand.cardsAtPosition);

            SetEffectApplied();
        }

        #endregion

        #region Steal Card From Deck

        public void StealCardFromDeck(Card source, int value, Contender contender)
        {
            StartCoroutine(StealCardFromDeckCoroutine(source, value, contender));
        }

        private IEnumerator StealCardFromDeckCoroutine(Card source, int value, Contender otherContender)
        {
            UIManager.Instance.ShowStealCardsFromDeck(otherContender.deck, value);

            yield return new WaitWhile(() => UIManager.Instance.stealing);
            yield return new WaitWhile(() => source.contender.deck.busy);

            SetEffectApplied();
        }

        #endregion

        private Card InstantiateCard(Contender newOwner, Vector3 position, CardsData data, bool cardRevealed)
        {
            GameObject cardPrefab = newOwner.deck.GetCardPrefab(data.type);
            GameObject cardObj = Instantiate(cardPrefab, position, cardPrefab.transform.rotation);
            CardsData newData = new CardsData(data);
            Card card = cardObj.GetComponent<Card>();
            card.Initialize(newOwner, newData, cardRevealed);
            return card;
        }

        private void SwapContender(Card source, Card target, CardZone dest)
        {
            // Get origin card zone
            int position = Board.Instance.GetPositionFromCard(target);
            CardZone originCardZone = target.contender.cardZones[position];

            //Create card with other prefab
            Card newCard = InstantiateCard(source.contender, target.transform.position, target.data, cardRevealed: true);

            // Empty destination zone
            if (dest.isNotEmpty)
                dest.GetCard().RemoveFromContainer();

            // Destroy original
            Card originCard = originCardZone.GetCard();
            originCard.DestroyCard(instant: true);

            dest.AddCard(newCard);

            newCard.Effects.ManageEffects();
        }

        #endregion

    }
}