using Booble.CardGame.Cards;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Cards.DataModel.Effects;
using Booble.CardGame.Level;
using Booble.CardGame.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.AI
{
    abstract public class OpponentAI : MonoBehaviour
    {
        protected Contender _contender;

        private float _waitTime = 1.5f;
        private float _timer;

        public void Initialize(Contender contender)
        {
            _contender = contender;
        }

        private void OnEnable()
        {
            _timer = 0;
        }

        private void Update()
        {
            if (_contender == null) return;
            if (!TurnManager.Instance.continueFlow) return;
            if (CardGameManager.Instance.playingCard) return;

            _timer += Time.deltaTime;
            if (_timer > _waitTime)
            {
                _timer = 0;
                Play();
            }
        }

        #region Play

        private void Play()
        {
            if (HasMana())
            {
                List<Card> playableCards = new List<Card>();
                List<Card> goodCards = new List<Card>();

                CardZone emptyCardZone = Board.Instance.GetEmptyCardZone(_contender);
                CardZone fieldCardZone = GetFieldCardZone();

                GetCards(ref playableCards, ref goodCards, emptyCardZone, fieldCardZone);

                if (goodCards.Count > 0)
                    PlayCard(goodCards, emptyCardZone);
                else
                {
                    if (playableCards.Count > 0
                        && _contender.currentMana == _contender.currentMaxMana
                        && _contender.hand.numCards > CardGameManager.Instance.settings.handCapacity)
                    {
                        PlayCard(playableCards, emptyCardZone);
                    }
                    else SkipTurn();
                }
            }
            else SkipTurn();
        }

        private void GetCards(ref List<Card> playableCards, ref List<Card> goodCards, bool emptyCardZone, bool fieldCardZone)
        {
            foreach (GameObject cardObj in _contender.hand.cards)
            {
                Card card = cardObj.GetComponent<Card>();

                if (!EnoughMana(card.Stats.manaCost)) continue;

                switch (card.Stats.type)
                {
                    case CardType.ARGUMENT:

                        if (emptyCardZone)
                        {
                            playableCards.Add(card);
                            if (IsGoodChoice(card)) goodCards.Add(card);
                        }
                        break;

                    case CardType.ACTION:

                        if (IsAppliable(card))
                        {
                            playableCards.Add(card);
                            if (IsGoodChoice(card)) goodCards.Add(card);
                        }
                        break;

                    case CardType.FIELD:

                        if (fieldCardZone) goodCards.Add(card); break;
                }
            }
        }

        private void PlayCard(List<Card> cards, CardZone emptyCardZone)
        {
            int index = Random.Range(0, cards.Count);
            Card card = cards[index];
            CardZone cardZone = null;

            switch (card.Stats.type)
            {
                case CardType.ARGUMENT:
                    CardZone bestCardZone = GetBestPosition(card);
                    cardZone = (bestCardZone != null) ? bestCardZone : emptyCardZone;
                    break;

                case CardType.FIELD:
                    cardZone = _contender.fieldCardZone;
                    break;
            }

            PlayCard(card, cardZone);
        }

        private void PlayCard(Card card, CardZone cardZone)
        {
            card.Play(cardZone);
        }

        #region Aux

        private bool HasMana()
        {
            return _contender.freeMana
                || _contender.currentMana > 0;
        }

        private bool EnoughMana(int manaCost)
        {
            if (_contender.freeMana) return true;

            return _contender.currentMana >= manaCost;
        }

        private bool IsAppliable(Card action)
        {
            return action.HasEffect && action.effect.IsAppliable(action);
        }

        private void SkipTurn()
        {
            TurnManager.Instance.FinishTurn();
        }

        private CardZone GetFieldCardZone()
        {
            return (_contender.fieldCardZone.isEmpty) ? _contender.fieldCardZone : null;
        }

        #endregion

        #endregion

        #region AI

        private bool IsGoodChoice(Card card)
        {
            if (card.IsArgument) return ArgumentIsGoodChoice(card);
            else if (card.IsAction) return ActionIsGoodChoice(card);
            else return true;
        }

        #region Arguments

        abstract protected bool ArgumentIsGoodChoice(Card argument);

        private CardZone GetBestPosition(Card argument)
        {
            CardZone bestCardZone = null;
            Card bestOppositeCard = null;

            Contender player = CardGameManager.Instance.player;

            for (int i = 0; i < _contender.cardZones.Count; i++)
            {
                CardZone cardZone = _contender.cardZones[i];
                if (cardZone.isNotEmpty) continue;

                Card oppositeCard = player.cardZones[i].GetCard();

                if (GetBestPosition(argument, cardZone, oppositeCard, bestCardZone, bestOppositeCard))
                {
                    bestCardZone = cardZone;
                    bestOppositeCard = oppositeCard;
                }
            }

            return bestCardZone;
        }

        abstract protected bool GetBestPosition(Card argument, CardZone cardZone, Card oppositeCard, CardZone bestCardZone, Card bestOppositeCard);

        protected bool DefaultGetBestPosition(Card oppositeCard, Card bestOppositeCard)
        {
            Contender player = CardGameManager.Instance.player;

            if (player.life > _contender.life)
            {
                if (oppositeCard != null)
                {
                    return GetStats(oppositeCard) > GetStats(bestOppositeCard);
                }
            }
            else
            {
                return oppositeCard == null;
            }

            return false;
        }

        protected bool GetStrongestOppositeCard(Card oppositeCard, Card bestOppositeCard)
        {
            if (oppositeCard != null)
            {
                bool betterStats = GetStats(oppositeCard) > GetStats(bestOppositeCard);
                return betterStats;
            }
            return false;
        }

        protected bool GetWeakestOppositeCard(Card oppositeCard, Card bestOppositeCard)
        {
            if (oppositeCard != null)
            {
                bool lowerStrength = oppositeCard.Stats.strength < ((bestOppositeCard != null) ? bestOppositeCard.Stats.strength : 0);
                return lowerStrength && oppositeCard.Stats.strength <= 1;
            }
            return false;
        }

        #endregion

        #region Actions

        public Card GetBestTarget(CardEffect effect, List<Card> possibleTargets)
        {
            Card bestTarget = possibleTargets[new System.Random().Next(0, possibleTargets.Count)];

            int bestStats = 0;
            foreach (Card card in possibleTargets)
            {
                switch (effect.subType)
                {
                    case SubType.DESTROY_CARD:
                        {
                            if (!card.IsPlayerCard) continue;

                            if (card.IsField)
                                return card;
                            else
                            {
                                Card oppositeCard = Board.Instance.GetOppositeCard(card);
                                int temp = (oppositeCard != null) ? -3 : 0;
                                GetBestStats(card, temp, ref bestTarget, ref bestStats);
                            }
                        }
                        break;

                    case SubType.DEAL_DAMAGE:
                        {
                            Card oppositeCard = Board.Instance.GetOppositeCard(card);

                            if (card != null)
                            {
                                int temp = GetStats(card);
                                int damage = effect.intParameter1;
                                if (oppositeCard != null) damage += oppositeCard.Stats.strength;

                                if ((card.Stats.defense <= damage) && (temp > bestStats))
                                {
                                    bestStats = temp;
                                    bestTarget = card;
                                };
                            }
                        }
                        break;

                    case SubType.RETURN_CARD:
                        {
                            int temp = 0;
                            if (card.Stats.IsBoosted())
                            {
                                if (card.IsPlayerCard) temp += card.Stats.GetBoost();
                                else temp -= card.Stats.GetBoost();
                            }
                            if (card.Stats.IsDamaged())
                            {
                                if (card.IsPlayerCard) temp -= card.Stats.GetDamage();
                                else temp += card.Stats.GetDamage();
                            }

                            Card oppositeCard = Board.Instance.GetOppositeCard(card);
                            if (oppositeCard != null) temp -= 3;

                            GetBestStats(card, temp, ref bestTarget, ref bestStats);
                        }
                        break;

                    case SubType.STEAL_CARD:
                    case SubType.DUPLICATE_CARD:
                        GetBestStats(card, ref bestTarget, ref bestStats); break;

                    case SubType.SWAP_POSITION:
                        GetSwapPositionTarget(card, ref bestTarget, ref bestStats); break;

                    case SubType.STAT_BOOST:
                        {
                            Card oppositeCard = Board.Instance.GetOppositeCard(card);

                            if (oppositeCard != null)
                            {
                                CardStats cardStats = card.Stats;
                                CardStats oppositeStats = oppositeCard.Stats;

                                bool savedCard = cardStats.defense <= oppositeStats.strength && (cardStats.defense + effect.intParameter2) > oppositeStats.strength;
                                bool killedCard = cardStats.strength < oppositeStats.defense && (cardStats.strength + effect.intParameter1) >= oppositeStats.defense;

                                int cardStatsSum = GetStats(card);
                                int oppositeCardStatsSum = GetStats(oppositeCard);

                                if (killedCard && oppositeCardStatsSum > bestStats)
                                {
                                    bestTarget = card;
                                    bestStats = oppositeCardStatsSum;
                                }
                                else if (savedCard && cardStatsSum > bestStats)
                                {
                                    bestTarget = card;
                                    bestStats = cardStatsSum;
                                }
                            }
                        }
                        break;
                }
            }

            return bestTarget;
        }

        private bool ActionIsGoodChoice(Card source)
        {
            Contender player = CardGameManager.Instance.player;

            List<CardZone> playerCardZones = player.cardZones;
            List<CardZone> opponentCardZones = _contender.cardZones;

            CardEffect effect = source.effect;

            switch (effect.subType)
            {
                case SubType.RESTORE_LIFE:
                    return true;

                case SubType.DESTROY_CARD:
                    if (effect.targetType == Target.CARD)
                    {
                        if (player.fieldCardZone.isNotEmpty) return true;

                        return Board.Instance.NumCardsOnTable(player) > Board.Instance.NumCardsOnTable(_contender)
                            && Board.Instance.NumCardsOnTable(player) > 0;
                    }
                    else if (effect.targetType == Target.AENEMY)
                    {
                        return Board.Instance.NumCardsOnTable(player) > Board.Instance.NumCardsOnTable(_contender)
                            && Board.Instance.NumCardsOnTable(player) > 2;
                    }
                    else if (effect.targetType == Target.ACARD)
                    {
                        return GetStatsSummary(player, playerCardZones, _contender, opponentCardZones) > 0
                            && Board.Instance.NumCardsOnTable(player) > Board.Instance.NumCardsOnTable(_contender)
                            && Board.Instance.NumCardsOnTable(player) >= 2;
                    }
                    break;

                case SubType.DEAL_DAMAGE:
                    if (effect.targetType == Target.ENEMY)
                    {
                        foreach (CardZone cardZone in playerCardZones)
                        {
                            if (cardZone.isNotEmpty)
                            {
                                Card playerCard = cardZone.GetCard();
                                Card oppositeCard = Board.Instance.GetOppositeCard(playerCard);
                                int temp = effect.intParameter1;
                                if (oppositeCard != null) temp += oppositeCard.Stats.strength;
                                if (playerCard.Stats.strength <= temp) return true;
                            }
                        }
                        return false;
                    }
                    else if (effect.targetType == Target.PLAYER) return true;
                    break;

                case SubType.DECREASE_MANA:
                    return (Board.Instance.NumCardsOnTable(_contender) - Board.Instance.NumCardsOnTable(player)) >= 2;

                case SubType.STAT_BOOST:
                    int statsSummary = GetStatsSummary(player, playerCardZones, _contender, opponentCardZones);
                    if (effect.targetType == Target.ALLY) return statsSummary > 0;
                    else if (effect.targetType == Target.AALLY)
                        return (statsSummary < 0) && Board.Instance.NumCardsOnTable(_contender) >= 2;
                    break;

                case SubType.DUPLICATE_CARD:
                    {
                        int bestStats = 0;
                        Card bestTarget = null;
                        foreach (CardZone cardZone in opponentCardZones)
                        {
                            if (cardZone.isNotEmpty)
                            {
                                Card card = cardZone.GetCard();
                                GetBestStats(card, ref bestTarget, ref bestStats);
                            }
                        }

                        return bestStats > 5;
                    }
                case SubType.SWAP_POSITION:
                    {
                        List<CardZone> cardZones = (_contender.life < player.life) ? playerCardZones : opponentCardZones;

                        int bestStats = 0;
                        Card bestTarget = null;
                        foreach (CardZone cardZone in cardZones)
                        {
                            if (cardZone.isNotEmpty)
                            {
                                Card card = cardZone.GetCard();
                                GetBestStats(card, ref bestTarget, ref bestStats);
                            }
                        }

                        if (bestTarget != null)
                        {
                            int position = Board.Instance.GetPositionFromCard(bestTarget);
                            if (_contender.life < player.life)
                            {
                                if (_contender.cardZones[position].isEmpty)
                                {
                                    if (position > 0 && opponentCardZones[position - 1].isNotEmpty) return true;
                                    else if (position < 3 && opponentCardZones[position + 1].isNotEmpty) return true;
                                }
                            }
                            else
                            {
                                if (playerCardZones[position].isNotEmpty)
                                {
                                    if (position > 0 && playerCardZones[position - 1].isEmpty) return true;
                                    else if (position < 3 && playerCardZones[position + 1].isEmpty) return true;
                                }
                            }
                        }

                        return false;
                    }

                case SubType.DRAW_CARD:
                    return _contender.hand.numCards <= 3;

                case SubType.DISCARD_CARD:
                    return player.hand.numCards >= effect.intParameter1;

                case SubType.RETURN_CARD:
                    return Board.Instance.NumCardsOnTable(player) > Board.Instance.NumCardsOnTable(_contender);

                case SubType.SKIP_COMBAT:
                    return !TurnManager.Instance.GetSkipCombat() && Board.Instance.NumCardsOnTable(player) > Board.Instance.NumCardsOnTable(_contender);

                case SubType.STEAL_CARD:
                    return Board.Instance.NumCardsOnTable(player) > Board.Instance.NumCardsOnTable(_contender);

                case SubType.STEAL_MANA:
                    {
                        int startMana = _contender.currentMana;
                        int finalMana = Mathf.Min(_contender.currentMana - source.Stats.manaCost + player.currentMana, _contender.currentMaxMana);

                        foreach (GameObject cardObj in _contender.hand.cards)
                        {
                            CardStats cardStats = cardObj.GetComponent<Card>().Stats;
                            if (cardStats.manaCost > startMana && cardStats.manaCost <= finalMana) return true;
                        }
                    }
                    return false;

                case SubType.STEAL_CARD_FROM_HAND:
                    return _contender.hand.numCards <= 3 && player.hand.numCards >= effect.intParameter1;

                case SubType.STEAL_REWARD:
                    return _contender.stolenCards >= 3 || (_contender.stolenCards >= 1 && _contender.life <= 5);

                default:
                    return true;
            }

            return true;
        }

        private void GetSwapPositionTarget(Card card, ref Card bestTarget, ref int bestStats)
        {
            Card oppositeCard = Board.Instance.GetOppositeCard(card);

            int temp = card.Stats.strength;
            if (temp > bestStats)
            {
                if ((card.IsPlayerCard && oppositeCard == null)
                    || (!card.IsPlayerCard && oppositeCard != null))
                {
                    bestTarget = card;
                    bestStats = temp;
                }
            }
        }

        private int GetStatsSummary(Contender player, List<CardZone> playerCardZones, Contender opponent, List<CardZone> opponentCardZones)
        {
            int temp = 0;
            foreach (CardZone cardZone in playerCardZones) temp += GetStats(cardZone.GetCard());
            if (player.fieldCardZone.isNotEmpty) temp += 5;

            foreach (CardZone cardZone in opponentCardZones) temp -= GetStats(cardZone.GetCard());
            if (opponent.fieldCardZone.isNotEmpty) temp -= 5;

            return temp;
        }

        #endregion

        private void GetBestStats(Card card, ref Card bestTarget, ref int bestStats)
        {
            GetBestStats(card, 0, ref bestTarget, ref bestStats);
        }

        private void GetBestStats(Card card, int baseStats, ref Card bestTarget, ref int bestStats)
        {
            int temp = baseStats + GetStats(card);
            if (temp > bestStats)
            {
                bestTarget = card;
                bestStats = temp;
            }
        }

        protected int GetStats(Card card)
        {
            if (card != null) return card.Stats.strength + card.Stats.defense;
            else return 0;
        }

        #endregion

    }
}