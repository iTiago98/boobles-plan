using Booble.CardGame.Cards;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Cards.DataModel.Effects;
using Booble.CardGame.Level;
using Santi.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Booble.CardGame.Managers.TurnManager;

namespace Booble.CardGame.Managers
{
    public class CardEffectsManager : Singleton<CardEffectsManager>
    {
        #region Deffensive Effects

        #region Restore Life

        public void RestoreLife(CardEffect effect, int life, Contender contender)
        {
            StartCoroutine(RestoreLifeCoroutine(effect, life, contender));
        }

        private IEnumerator RestoreLifeCoroutine(CardEffect effect, int life, Contender contender)
        {
            contender.RestoreLife(life);

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            effect.SetEffectApplied();
        }

        #endregion

        #region Increase Max Mana

        public void IncreaseMaxMana(CardEffect effect, int mana, Contender contender)
        {
            StartCoroutine(IncreaseMaxManaCoroutine(effect, mana, contender));
        }

        private IEnumerator IncreaseMaxManaCoroutine(CardEffect effect, int mana, Contender contender)
        {
            contender.IncreaseMaxMana(mana);

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            effect.SetEffectApplied();
        }

        #endregion

        #endregion

        #region Offensive Effects

        #region Destroy Card

        public void DestroyCard(CardEffect effect, Card source, object target, Target targetType)
        {
            StartCoroutine(DestroyCardCoroutine(effect, source, target, targetType));
        }

        private IEnumerator DestroyCardCoroutine(CardEffect effect, Card source, object target, Target targetType)
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

            yield return new WaitUntil(() => aux.destroyed);

            effect.SetEffectApplied();
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

        public void DealDamage(CardEffect effect, Card source, object target, Target targetType, int value)
        {
            StartCoroutine(DealDamageCoroutine(effect, source, target, targetType, value));
        }

        private IEnumerator DealDamageCoroutine(CardEffect effect, Card source, object target, Target targetType, int value)
        {
            Card aux = null;

            switch (targetType)
            {
                case Target.ENEMY:
                    Card card = (Card)target;
                    card.ReceiveDamage(value, checkDestroy: true);
                    aux = card;
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

                        break;
                    }
            }

            if (aux != null)
            {
                yield return new WaitWhile(() => aux.CardUI.IsPlayingAnimation);
                if (aux.Stats.defense == 0)
                    yield return new WaitUntil(() => aux.destroyed);
            }

            effect.SetEffectApplied();
        }

        private bool HasParaguas(Card source, Contender contender)
        {
            return source.name == "¡Va a llover!" && Board.Instance.HasCard(contender, "Abro paraguas");
        }

        private Card HitCards(Contender contender, int value)
        {
            Card aux = null;
            bool found = false;

            foreach (CardZone cardZone in contender.cardZones)
            {
                if (cardZone.isNotEmpty)
                {
                    Card card = cardZone.GetCard();
                    bool destroy = card.ReceiveDamage(value, checkDestroy: true);

                    if (!found || destroy)
                    {
                        aux = card;
                        found = true;
                    }
                }
            }

            return aux;
        }

        #endregion

        #region Decrease Mana

        public void DecreaseMana(CardEffect effect, Card source, int value)
        {
            StartCoroutine(DecreaseManaCoroutine(effect, source, value));
        }

        private IEnumerator DecreaseManaCoroutine(CardEffect effect, Card source, int value)
        {
            Contender contender = CardGameManager.Instance.GetOtherContender(source.contender);
            if (value == 0) value = contender.currentMana;

            contender.SubstractMana(value);

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            effect.SetEffectApplied();
        }

        #endregion

        #endregion

        #region Boost Effects

        #region Combat

        private bool _next = true;

        public void CombatEffects(CardEffect effect, Card source, object target)
        {
            StartCoroutine(CombatEffectsCoroutine(effect, source, target));
        }

        private IEnumerator CombatEffectsCoroutine(CardEffect effect, Card source, object target)
        {
            if (target is Card)
            {
                Card targetCard = (Card)target;

                _next &= targetCard.Effects.hasAppliableManagedCombatEffects;

                //if (source.Effects.singleHit) _next = false;

                if (_next) _next = false;
                else
                {
                    ApplyEffectValues(source, targetCard);
                    ResetEffectStoredValues();

                    yield return new WaitUntil(() => UIManager.Instance.statsUpdated);
                    yield return new WaitWhile(() => source.CardUI.IsPlayingAnimation);
                    yield return new WaitWhile(() => targetCard.CardUI.IsPlayingAnimation);
                }
            }
            else if (target is Contender)
            {
                if (source.Effects.HasEffect(SubType.LIFELINK))
                {
                    source.contender.RestoreLife(source.Stats.strength);

                    yield return new WaitUntil(() => UIManager.Instance.statsUpdated);
                }
            }

            effect.SetEffectApplied();
        }

        public void GetEffectValues(Card source, Card target, ref int lifeValue, ref int reboundValue, ref int trampleValue, ref int spongeValue)
        {
            lifeValue = 0;
            reboundValue = 0;
            trampleValue = 0;
            spongeValue = 0;

            if (source.Effects.HasEffect(SubType.LIFELINK))
            {
                lifeValue = Mathf.Min(source.Stats.strength, target.Stats.defense);
            }

            if (source.Effects.HasEffect(SubType.REBOUND))
            {
                reboundValue = Mathf.Min(source.Stats.defense, target.Stats.strength);
                if (source.IsPlayerCard) CardGameManager.Instance.alternateWinConditionParameter += reboundValue;
            }

            if (source.Effects.HasEffect(SubType.TRAMPLE))
            {
                trampleValue = source.Stats.strength - target.Stats.defense;
            }

            if (source.Effects.HasEffect(SubType.SPONGE))
            {
                spongeValue = target.Stats.strength;
            }
        }

        private void ApplyEffectValues(Card source, Card target)
        {
            source.Effects.ApplyEffectValues(target);
            target.Effects.ApplyEffectValues(source, source.Effects.singleHit);
        }

        private void ResetEffectStoredValues()
        {
            _next = true;
        }

        #region Compartmentalize

        public void Compartmentalize(CardEffect effect, Card source, object target)
        {
            StartCoroutine(CompartmentalizeCoroutine(effect, source, target));
        }

        private IEnumerator CompartmentalizeCoroutine(CardEffect effect, Card source, object target)
        {
            if (target is Contender)
            {
                Contender contender = (Contender)target;

                if (contender.deck.numCards == 0)
                {
                    contender.ReceiveDamage(source.Stats.strength);
                    yield return new WaitUntil(() => UIManager.Instance.statsUpdated);
                }
                else
                {
                    contender.deck.DiscardCards(source.Stats.strength);
                    yield return new WaitWhile(() => contender.deck.busy);
                }
            }

            effect.SetEffectApplied();
        }

        #endregion

        #endregion

        #region Guard

        public void Guard(CardEffect effect, Card source)
        {
            source.contender.AddGuardCard(source);
            effect.SetEffectApplied();
        }

        #endregion

        #region Stat Boost

        public void StatBoost(CardEffect effect, Card source, object target, Target targetType, int strengthValue, int defenseValue)
        {
            StartCoroutine(StatBoostCoroutine(effect, source, target, targetType, strengthValue, defenseValue));
        }

        private IEnumerator StatBoostCoroutine(CardEffect effect, Card source, object target, Target targetType, int strengthValue, int defenseValue)
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

            effect.SetEffectApplied();
        }

        #endregion

        #region Stat Decrease

        public void StatDecrease(CardEffect effect, Card source, object target, Target targetType, int strengthValue, int defenseValue)
        {
            StartCoroutine(StatDecreaseCoroutine(effect, source, target, targetType, strengthValue, defenseValue));
        }

        private IEnumerator StatDecreaseCoroutine(CardEffect effect, Card source, object target, Target targetType, int strengthValue, int defenseValue)
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

            effect.SetEffectApplied();
        }

        #endregion

        #region Add Effect

        public void AddEffect(CardEffect effect, Card source, object target, Target targetType, CardEffect effectParameter)
        {
            StartCoroutine(AddEffectCoroutine(effect, source, target, targetType, effectParameter));
        }

        private IEnumerator AddEffectCoroutine(CardEffect effect, Card source, object target, Target targetType, CardEffect effectParameter)
        {
            Card aux = null;

            switch (targetType)
            {
                case Target.ALLY:
                case Target.ENEMY:
                case Target.CARD:
                    Card targetCard = (Card)target;
                    if (targetCard.AddEffect(effectParameter))
                    {
                        aux = targetCard;
                    }
                    break;

                case Target.AALLY:
                    foreach (Card card in Board.Instance.GetCardsOnTable(source.contender))
                    {
                        bool added = card.AddEffect(effectParameter);
                        if (aux == null && added) aux = card;
                    }
                    break;
            }

            if (aux != null) yield return new WaitWhile(() => aux.CardUI.IsPlayingAnimation);

            effect.SetEffectApplied();
        }

        #endregion

        #endregion

        #region Tactical Effects 

        #region Create Card

        public void CreateCard(CardEffect effect, Card source, CardsData data)
        {
            StartCoroutine(CreateCardCoroutine(effect, source, data));
        }

        private IEnumerator CreateCardCoroutine(CardEffect effect, Card source, CardsData data)
        {
            CardZone emptyCardZone = Board.Instance.GetEmptyCardZone(source.contender);
            if (emptyCardZone != null)
            {
                Card card = InstantiateCard(source.contender, source.transform.position, data, cardRevealed: true);
                card.Effects.CheckEffects();
                emptyCardZone.AddCard(card.gameObject);

                yield return new WaitUntil(() => emptyCardZone.cardsAtPosition);
            }

            effect.SetEffectApplied();
        }

        #endregion

        #region Swap Position

        public void SwapPosition(CardEffect effect, object target, Target targetType)
        {
            StartCoroutine(SwapPositionCoroutine(effect, target, targetType));
        }

        private IEnumerator SwapPositionCoroutine(CardEffect effect, object target, Target targetType)
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

            effect.SetEffectApplied();
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
                originCardZone.AddCard(temp.gameObject);
            }
            destCardZone.AddCard(originCard.gameObject);
        }

        #endregion

        #region Swap Contender

        public void SwapContender(CardEffect effect, Card source, Target targetType)
        {
            StartCoroutine(SwapContenderCoroutine(effect, source, targetType));
        }

        private IEnumerator SwapContenderCoroutine(CardEffect effect, Card source, Target targetType)
        {
            if (targetType == Target.SELF)
            {
                CardZone emptyCardZone = Board.Instance.GetEmptyCardZone(CardGameManager.Instance.GetOtherContender(source.contender));
                if (emptyCardZone != null)
                {
                    SwapContender(source, emptyCardZone);

                    yield return new WaitUntil(() => emptyCardZone.cardsAtPosition);
                }
            }

            effect.SetEffectApplied();
        }

        #endregion

        #region Draw Card

        public void DrawCard(CardEffect effect, int value, Contender contender)
        {
            StartCoroutine(DrawCardCoroutine(effect, value, contender));
        }

        private IEnumerator DrawCardCoroutine(CardEffect effect, int value, Contender contender)
        {
            contender.deck.DrawCards(value);

            yield return new WaitWhile(() => contender.deck.busy);

            effect.SetEffectApplied();
        }

        #endregion

        #region Discard Card

        public void DiscardCard(CardEffect effect, int value, Contender contender)
        {
            StartCoroutine(DiscardCardCoroutine(effect, value, contender));
        }

        private IEnumerator DiscardCardCoroutine(CardEffect effect, int value, Contender contender)
        {
            contender.hand.DiscardCards(value);

            yield return new WaitWhile(() => contender.hand.busy);

            effect.SetEffectApplied();
        }

        #endregion

        #region Return Card

        public void ReturnCard(CardEffect effect, object target)
        {
            StartCoroutine(ReturnCardCoroutine(effect, target));
        }

        private IEnumerator ReturnCardCoroutine(CardEffect effect, object target)
        {
            if (target is Card)
            {
                Card card = (Card)target;
                card.ReturnToHand();

                yield return new WaitUntil(() => card.contender.hand.cardsAtPosition);
            }

            effect.SetEffectApplied();
        }

        #endregion

        #region FreeMana

        public void FreeMana(CardEffect effect, Contender contender)
        {
            contender.SetFreeMana(true);
            effect.SetEffectApplied();
        }

        #endregion

        #region Wheel

        public void Wheel(CardEffect effect)
        {
            StartCoroutine(WheelCoroutine(effect));
        }

        private IEnumerator WheelCoroutine(CardEffect effect)
        {
            Contender player = CardGameManager.Instance.player;
            Contender opponent = CardGameManager.Instance.opponent;

            int playerNumCards = player.hand.numCards;
            int opponentNumCards = opponent.hand.numCards;

            player.hand.DiscardAll();
            opponent.hand.DiscardAll();

            yield return new WaitUntil(() => Board.Instance.EmptyHands());

            if (player.hand.HasAlternateWinConditionCard()) playerNumCards--;
            if (opponent.hand.HasAlternateWinConditionCard()) opponentNumCards--;

            player.deck.DrawCards(playerNumCards);
            opponent.deck.DrawCards(opponentNumCards);

            yield return new WaitUntil(() => player.hand.numCards == playerNumCards && opponent.hand.numCards == opponentNumCards);

            effect.SetEffectApplied();
        }

        #endregion

        #region Skip Combat

        public void SkipCombat(CardEffect effect)
        {
            TurnManager.Instance.SkipCombat();
            effect.SetEffectApplied();
        }

        #endregion

        #region Mirror

        public void Mirror(CardEffect effect, Contender contender)
        {
            contender.AddMirrorCard();
            effect.SetEffectApplied();
        }

        #endregion

        #region Steal Mana

        public void StealMana(CardEffect effect, Contender contender)
        {
            StartCoroutine(StealManaCoroutine(effect, contender));
        }

        private IEnumerator StealManaCoroutine(CardEffect effect, Contender contender)
        {
            Contender otherContender = CardGameManager.Instance.GetOtherContender(contender);
            int manaValue = otherContender.currentMana;

            otherContender.SubstractMana(manaValue);
            contender.RestoreMana(manaValue);

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            effect.SetEffectApplied();
        }

        #endregion

        #region Add Card To Deck

        public void AddCardToDeck(CardEffect effect, Card source, CardsData data)
        {
            StartCoroutine(AddCardToDeckCoroutine(effect, source, data));
        }

        private IEnumerator AddCardToDeckCoroutine(CardEffect effect, Card source, CardsData data)
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

            effect.SetEffectApplied();
        }

        #endregion

        #region Discard Card From Deck

        public void DiscardCardFromDeck(CardEffect effect, object target, int value, Contender contender)
        {
            StartCoroutine(DiscardCardFromDeckCoroutine(effect, target, value, contender));
        }

        private IEnumerator DiscardCardFromDeckCoroutine(CardEffect effect, object target, int value, Contender contender)
        {
            int number = value;
            if (target != null) number = ((Card)target).Stats.manaCost;

            contender.deck.DiscardCards(number);

            yield return new WaitWhile(() => contender.deck.busy);

            effect.SetEffectApplied();
        }

        #endregion

        #region Steal Card

        public void StealCard(CardEffect effect, Card source, object target)
        {
            StartCoroutine(StealCardCoroutine(effect, source, target));
        }

        private IEnumerator StealCardCoroutine(CardEffect effect, Card source, object target)
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
                    SwapContender(targetCard, destCardZone);
                    source.contender.stolenCards++;

                    yield return new WaitUntil(() => destCardZone.cardsAtPosition);
                }
            }

            effect.SetEffectApplied();
        }

        #endregion

        #region Steal Card From Hand

        public void StealCardFromHand(CardEffect effect, Card source, int value)
        {
            StartCoroutine(StealCardFromHandCoroutine(effect, source, value));
        }

        private IEnumerator StealCardFromHandCoroutine(CardEffect effect, Card source, int value)
        {
            Contender otherContender = CardGameManager.Instance.GetOtherContender(source.contender);
            int loops = Mathf.Min(value, otherContender.hand.numCards);

            int finalValue = source.contender.hand.numCards + loops;

            for (int i = 0; i < loops; i++)
            {
                Card card = otherContender.hand.StealCard();
                card.CardUI.FlipCard();

                card.RemoveFromContainer();
                source.contender.hand.AddCard(card.gameObject);

                source.contender.stolenCards++;
            }

            yield return new WaitUntil(() => source.contender.hand.numCards == finalValue && source.contender.hand.cardsAtPosition);

            effect.SetEffectApplied();
        }

        #endregion

        #region Steal Card From Deck

        [SerializeField] private GameObject _stealCardsFromDeckObj;
        [SerializeField] private CardContainer _stealCardsFromDeckContainer;
        [SerializeField] private GameObject _cardStealPrefab;

        private List<int> _cardsToSteal = new List<int>();
        private int _numCardsToSteal;

        public void StealCardFromDeck(CardEffect effect, Card source, int value, Contender contender)
        {
            StartCoroutine(StealCardFromDeckCoroutine(effect, source, value, contender));
        }

        private IEnumerator StealCardFromDeckCoroutine(CardEffect effect, Card source, int value, Contender otherContender)
        {
            Deck deck = otherContender.deck;

            _stealCardsFromDeckObj.SetActive(true);
            UIManager.Instance.SetStealing();
            MouseController.Instance.SetStealing();

            _numCardsToSteal = (deck.numCards >= value) ? value : deck.numCards;

            List<CardsData> deckCards = deck.GetDeckCards();

            foreach (CardsData cardData in deckCards)
            {
                GameObject cardObj = Instantiate(_cardStealPrefab, Vector3.zero, Quaternion.identity);
                CardSteal cardImageUI = cardObj.GetComponent<CardSteal>();
                cardImageUI.Initialize(cardData, deckCards.IndexOf(cardData));
                _stealCardsFromDeckContainer.AddCard(cardObj);
            }

            yield return new WaitWhile(() => UIManager.Instance.stealing);

            deck.StealCards(_cardsToSteal);
            _stealCardsFromDeckObj.SetActive(false);

            yield return new WaitWhile(() => source.contender.deck.busy);

            effect.SetEffectApplied();
        }

        public bool AddStolenCard(int index)
        {
            if (_cardsToSteal.Count == _numCardsToSteal) return false;

            _cardsToSteal.Add(index);
            if (_cardsToSteal.Count == _numCardsToSteal)
            {
                UIManager.Instance.ShowStealCardsFromDeckButton(true);
            }
            return true;
        }

        public bool RemoveStolenCard(int index)
        {
            _cardsToSteal.Remove(index);
            UIManager.Instance.ShowStealCardsFromDeckButton(false);
            return false;
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

        private void SwapContender(Card target, CardZone dest)
        {
            if (dest.isNotEmpty)
                dest.GetCard().RemoveFromContainer();

            target.RemoveFromContainer();
            target.SwapContender();

            dest.AddCard(target.gameObject);
        }

        #endregion

        #region Permanent Effects

        private struct PermanentEffect
        {
            public Action effect;
            public Card card;
        }

        public void AddPermanentEffect(Action method, Card card, ApplyTime applyTime)
        {
            PermanentEffect effect = new PermanentEffect();
            effect.effect = method;
            effect.card = card;

            switch (applyTime)
            {
                case ApplyTime.END:
                    AddPermanentEffect(effect, ref _endTurnEffects, ref _endTurnEffectsToAdd); break;

                case ApplyTime.DRAW_CARD:
                    AddPermanentEffect(effect, ref _drawCardEffects, ref _drawCardEffectsToAdd); break;

                case ApplyTime.PLAY_ARGUMENT:
                    AddPermanentEffect(effect, ref _playArgumentEffects, ref _playArgumentEffectsToAdd); break;
            }
        }

        private void AddPermanentEffect(PermanentEffect effect, ref List<PermanentEffect> effectsList, ref List<PermanentEffect> effectsToAdd)
        {
            if (TurnManager.Instance.turn == Turn.ROUND_END) effectsToAdd.Add(effect);
            else effectsList.Add(effect);
        }

        public void RemovePermanentEffect(Card card, ApplyTime applyTime)
        {
            switch (applyTime)
            {
                case ApplyTime.END:
                    RemovePermanentEffect(card, ref _endTurnEffects, ref _endTurnEffectsToRemove); break;

                case ApplyTime.DRAW_CARD:
                    RemovePermanentEffect(card, ref _drawCardEffects, ref _drawCardEffectsToRemove); break;

                case ApplyTime.PLAY_ARGUMENT:
                    RemovePermanentEffect(card, ref _playArgumentEffects, ref _playArgumentEffectsToRemove); break;
            }
        }

        private void RemovePermanentEffect(Card card, ref List<PermanentEffect> effectsList, ref List<int> effectsToRemove)
        {
            int index = GetIndex(effectsList, card);
            if (TurnManager.Instance.turn == Turn.ROUND_END) effectsToRemove.Add(index);
            else effectsList.RemoveAt(index);
        }

        private int GetIndex(List<PermanentEffect> effects, Card card)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i].card == card) return i;
            }

            return -1;
        }

        public bool effectsApplied { private set; get; }

        private void UpdateEffectsToRemove(List<PermanentEffect> effectsList, List<int> effectsToRemove)
        {
            effectsToRemove.Sort();
            effectsToRemove.Reverse();
            for (int i = 0; i < effectsToRemove.Count; i++)
            {
                effectsList.RemoveAt(effectsToRemove[i]);
            }
            effectsToRemove.Clear();
        }

        private void UpdateEffectsToAdd(List<PermanentEffect> effectsList, List<PermanentEffect> effectsToAdd)
        {
            foreach (PermanentEffect effect in effectsToAdd)
            {
                effectsList.Add(effect);
            }
            effectsToAdd.Clear();
        }

        private IEnumerator ApplyPermanentEffectsCoroutine(List<PermanentEffect> effectsList, List<PermanentEffect> effectsToAdd, List<int> effectsToRemove)
        {
            if (effectsList.Count > 0)
            {
                for (int i = 0; i < effectsList.Count; i++)
                {
                    if (effectsToRemove.Contains(i)) continue;

                    PermanentEffect permanentEffect = effectsList[i];
                    permanentEffect.card.effect.SetEffectApplied(false);
                    permanentEffect.effect();
                    yield return new WaitUntil(() => permanentEffect.card.effect.effectApplied);
                }
            }

            effectsApplied = true;

            if (effectsToRemove.Count > 0) UpdateEffectsToRemove(effectsList, effectsToRemove);
            if (effectsToAdd.Count > 0) UpdateEffectsToAdd(effectsList, effectsToAdd);
        }

        #region End Turn Effects

        private List<PermanentEffect> _endTurnEffects = new List<PermanentEffect>();

        private List<PermanentEffect> _endTurnEffectsToAdd = new List<PermanentEffect>();
        private List<int> _endTurnEffectsToRemove = new List<int>();

        public void ApplyEndTurnEffects()
        {
            StartCoroutine(ApplyEndTurnEffectsCoroutine());
        }

        private IEnumerator ApplyEndTurnEffectsCoroutine()
        {
            if (_endTurnEffects.Count > 0)
            {
                effectsApplied = false;

                StartCoroutine(ApplyPermanentEffectsCoroutine(_endTurnEffects, _endTurnEffectsToAdd, _endTurnEffectsToRemove));
                yield return new WaitUntil(() => effectsApplied);
            }

            TurnManager.Instance.ChangeTurn();
        }

        #endregion

        #region Play Argument Effects

        private List<PermanentEffect> _playArgumentEffects = new List<PermanentEffect>();

        private List<PermanentEffect> _playArgumentEffectsToAdd = new List<PermanentEffect>();
        private List<int> _playArgumentEffectsToRemove = new List<int>();

        public void ApplyPlayArgumentEffects()
        {
            StartCoroutine(ApplyPlayArgumentEffectsCoroutine());
        }

        private IEnumerator ApplyPlayArgumentEffectsCoroutine()
        {
            UIManager.Instance.SetEndTurnButtonInteractable(false);
            effectsApplied = false;

            StartCoroutine(ApplyPermanentEffectsCoroutine(_playArgumentEffects, _playArgumentEffectsToAdd, _playArgumentEffectsToRemove));
            yield return new WaitUntil(() => effectsApplied);

            UIManager.Instance.SetEndTurnButtonInteractable(TurnManager.Instance.IsPlayerTurn);
        }


        #endregion

        #region Draw Card Effects

        private List<PermanentEffect> _drawCardEffects = new List<PermanentEffect>();

        private List<PermanentEffect> _drawCardEffectsToAdd = new List<PermanentEffect>();
        private List<int> _drawCardEffectsToRemove = new List<int>();

        public void ApplyDrawCardEffects()
        {
            StartCoroutine(ApplyDrawCardEffectsCoroutine());
        }

        private IEnumerator ApplyDrawCardEffectsCoroutine()
        {
            bool previousState = UIManager.Instance.IsEndTurnButtonInteractable();
            UIManager.Instance.SetEndTurnButtonInteractable(false);
            effectsApplied = false;

            StartCoroutine(ApplyPermanentEffectsCoroutine(_drawCardEffects, _drawCardEffectsToAdd, _drawCardEffectsToRemove));
            yield return new WaitUntil(() => effectsApplied);

            UIManager.Instance.SetEndTurnButtonInteractable(previousState);
        }

        #endregion

        #endregion
    }
}