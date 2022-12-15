using CardGame.Level;
using CardGame.Managers;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Cards.DataModel.Effects
{
    #region Enums

    public enum EffectType
    {
        NONE, DEFENSIVE, OFFENSIVE, BOOST, TACTICAL, ALTERNATE_WIN_CONDITION
    }

    public enum SubType
    {
        NONE,
        RESTORE_LIFE, INCREASE_MAX_MANA,
        DESTROY_CARD, DEAL_DAMAGE, DECREASE_MANA,
        LIFELINK, REBOUND, TRAMPLE, STAT_BOOST, ADD_EFFECT, GUARD,
        DUPLICATE_CARD, CREATE_CARD, SWAP_POSITION, SWAP_CONTENDER, DRAW_CARD, DISCARD_CARD, RETURN_CARD,
        FREE_MANA, WHEEL, COMPARTMENTALIZE, SKIP_COMBAT, CITRIANO_WIN_CONDITION, PINPONBROS_WIN_CONDITION, SECRETARY_WIN_CONDITION,
        INCOMPARTMENTABLE, ADD_CARD_TO_DECK, DISCARD_CARD_FROM_DECK, SPONGE, STEAL_CARD, STEAL_CARD_FROM_HAND, MIRROR, STEAL_MANA,
        STEAL_REWARD
    }

    public enum DefensiveType
    {
        NONE, RESTORE_LIFE, INCREASE_MAX_MANA, STEAL_REWARD
    }

    public enum OffensiveType
    {
        NONE, DESTROY_CARD, DEAL_DAMAGE, DECREASE_MANA
    }

    public enum BoostType
    {
        NONE, LIFELINK, REBOUND, TRAMPLE, STAT_BOOST, ADD_EFFECT, GUARD, COMPARTMENTALIZE, INCOMPARTMENTABLE, SPONGE
    }

    public enum TacticalType
    {
        NONE, DUPLICATE_CARD, CREATE_CARD, SWAP_POSITION, SWAP_CONTENDER, DRAW_CARD, DISCARD_CARD, RETURN_CARD,
        FREE_MANA, WHEEL, SKIP_COMBAT, ADD_CARD_TO_DECK, DISCARD_CARD_FROM_DECK, STEAL_CARD, STEAL_CARD_FROM_HAND,
        MIRROR, STEAL_MANA
    }

    public enum AlternateWinConditionType
    {
        NONE, CITRIANO_WIN_CONDITION, PINPONBROS_WIN_CONDITION, SECRETARY_WIN_CONDITION
    }

    public enum ApplyTime
    {
        NONE, /*START,*/ ENTER, COMBAT, END, DRAW_CARD, PLAY_ARGUMENT, DESTROY
    }

    // ALLY - ally card
    // ENEMY - enemy card
    // CARD - any card
    // AALLY - all allies
    // AENEMY - all enemies
    // ACARD - all cards
    // CARDZONE
    // PLAYER - other player
    public enum Target
    {
        NONE, ALLY, ENEMY, CARD, AALLY, AENEMY, ACARD, /*CARDZONE,*/ PLAYER, SELF, FIELDCARD, ALL
    }

    #endregion

    [Serializable]
    public class CardEffect
    {
        #region Type

        public EffectType type;
        public SubType subType;

        public void SetSubTypeFromChild(Enum childSubType)
        {
            subType = (SubType)Enum.Parse(typeof(SubType), childSubType.ToString());
        }

        #endregion

        public ApplyTime applyTime;
        public Target targetType;

        public int intParameter1;
        public int intParameter2;

        public CardsDataSimple cardParameter;

        public SubType cardParameter_Effect;

        private int _storedValue = -1;

        private EffectType GetTypeFromSubType(SubType subType)
        {
            foreach (string s in Enum.GetNames(typeof(DefensiveType)))
            {
                if (subType.ToString().Equals(s)) return EffectType.DEFENSIVE;
            }

            foreach (string s in Enum.GetNames(typeof(OffensiveType)))
            {
                if (subType.ToString().Equals(s)) return EffectType.OFFENSIVE;
            }

            foreach (string s in Enum.GetNames(typeof(BoostType)))
            {
                if (subType.ToString().Equals(s)) return EffectType.BOOST;
            }

            foreach (string s in Enum.GetNames(typeof(TacticalType)))
            {
                if (subType.ToString().Equals(s)) return EffectType.TACTICAL;
            }

            return EffectType.DEFENSIVE;
        }

        #region Apply

        public void Apply(Card source, object target)
        {
            //Debug.Log(type + " " + subType + " effect applied");

            if (target != null && target is Card)
                ((Card)target).ShowHighlight(false);

            switch (type)
            {
                case EffectType.NONE:
                    break;
                case EffectType.DEFENSIVE:
                    ApplyDefensiveEffect(source, target);
                    break;
                case EffectType.OFFENSIVE:
                    ApplyOffensiveEffect(source, target);
                    break;
                case EffectType.BOOST:
                    ApplyBoostEffect(source, target);
                    break;
                case EffectType.TACTICAL:
                    ApplyTacticalEffect(source, target);
                    break;
                case EffectType.ALTERNATE_WIN_CONDITION:
                    ApplyAlternateWinCondition(source, target);
                    break;
            }

            switch (subType)
            {
                case SubType.INCREASE_MAX_MANA:
                case SubType.DEAL_DAMAGE:
                case SubType.STAT_BOOST:
                case SubType.ADD_EFFECT:
                case SubType.SWAP_POSITION:
                case SubType.SWAP_CONTENDER:
                case SubType.RETURN_CARD:
                case SubType.FREE_MANA:
                case SubType.SKIP_COMBAT:
                case SubType.MIRROR:
                case SubType.STEAL_MANA:
                case SubType.STEAL_CARD_FROM_HAND:
                    TurnManager.Instance.ContinueFlow(); break;
            }
        }

        private void ApplyDefensiveEffect(Card source, object target)
        {
            switch (subType)
            {
                case SubType.NONE:
                    break;
                case SubType.RESTORE_LIFE:
                    source.contender.RestoreLife(intParameter1);
                    break;
                case SubType.INCREASE_MAX_MANA:
                    source.contender.IncreaseMaxMana(intParameter1);
                    break;
                case SubType.STEAL_REWARD:
                    source.contender.RestoreLife(source.contender.stolenCards * 5);
                    break;
            }
        }

        private void ApplyOffensiveEffect(Card source, object target)
        {
            switch (subType)
            {
                case SubType.NONE:
                    break;
                case SubType.DESTROY_CARD:
                    switch (targetType)
                    {
                        case Target.ENEMY:
                        case Target.CARD:
                            ((Card)target).DestroyCard(continueFlow: true);
                            break;

                        case Target.AENEMY: Board.Instance.DestroyCards(CardGameManager.Instance.otherPlayer); break;
                        case Target.ACARD: Board.Instance.DestroyAll(); break;

                        case Target.FIELDCARD:
                            CardGameManager.Instance.otherPlayer.fieldCardZone.GetCard()?.DestroyCard(continueFlow: true);
                            break;
                    }
                    break;

                case SubType.DEAL_DAMAGE:
                    switch (targetType)
                    {
                        case Target.ENEMY: ((Card)target).ReceiveDamage(intParameter1); break;
                        case Target.PLAYER: CardGameManager.Instance.otherPlayer.ReceiveDamage(intParameter1); break;
                        case Target.SELF: CardGameManager.Instance.currentPlayer.ReceiveDamage(intParameter1); break;

                        case Target.ALL:
                            {
                                Contender player = CardGameManager.Instance.player;
                                Contender opponent = CardGameManager.Instance.opponent;

                                if (source.name != "¡Va a llover!" || !Board.Instance.HasCard(player, "Abro paraguas"))
                                {
                                    Board.Instance.HitCards(player, intParameter1);
                                    player.ReceiveDamage(intParameter1);
                                }

                                if (source.name != "¡Va a llover!" || !Board.Instance.HasCard(opponent, "Abro paraguas"))
                                {
                                    Board.Instance.HitCards(opponent, intParameter1);
                                    opponent.ReceiveDamage(intParameter1);
                                }
                            }
                            break;
                    }
                    break;

                case SubType.DECREASE_MANA:
                    Contender otherPlayer = CardGameManager.Instance.otherPlayer;
                    int manaValue = intParameter1;
                    if (intParameter1 == 0)
                    {
                        manaValue = otherPlayer.currentMana;
                    }

                    otherPlayer.SubstractMana(manaValue);
                    break;
            }
        }

        private void ApplyBoostEffect(Card source, object target)
        {
            switch (subType)
            {
                case SubType.NONE:
                    break;
                case SubType.LIFELINK:
                    {
                        int lifeValue = 0;
                        if (target is Card)
                        {
                            Card targetCard = (Card)target;
                            lifeValue = Mathf.Min(source.strength, targetCard.defense);
                        }
                        else if (target is Contender)
                        {
                            lifeValue = source.strength;
                        }
                        source.contender.RestoreLife(lifeValue);
                    }
                    break;

                case SubType.REBOUND:
                    {
                        if (target is Card)
                        {
                            Card targetCard = (Card)target;
                            int reboundValue = Mathf.Min(source.defense, targetCard.strength);

                            if (targetCard.HasEffect(SubType.REBOUND))
                            {
                                if (_storedValue == -1)
                                    _storedValue = reboundValue + source.strength;
                                else
                                {
                                    targetCard.ReceiveDamage(reboundValue + source.strength);
                                    source.ReceiveDamage(_storedValue);
                                    _storedValue = -1;
                                }
                            }
                            else
                            {
                                targetCard.ReceiveDamage(reboundValue + source.strength);
                                source.ReceiveDamage(targetCard.strength);
                            }

                            if (source.contender.role == Contender.Role.PLAYER)
                            {
                                CardGameManager.Instance.alternateWinConditionParameter += reboundValue;
                            }
                        }
                    }
                    break;

                case SubType.TRAMPLE:
                    {
                        if (target is Card)
                        {
                            Card targetCard = (Card)target;
                            int lifeValue = source.strength - targetCard.defense;
                            CardGameManager.Instance.otherPlayer.ReceiveDamage(lifeValue);
                        }
                    }
                    break;

                case SubType.COMPARTMENTALIZE:

                    if (target is Contender)
                    {
                        ((Contender)target).deck.DiscardCards(source.strength);
                    }
                    break;

                case SubType.SPONGE:

                    source.Hit(target);
                    if (target is Card)
                    {
                        Card targetCard = (Card)target;
                        targetCard.Hit(source);
                        source.BoostStats(targetCard.strength, 0);
                    }
                    break;

                case SubType.STAT_BOOST:
                    switch (targetType)
                    {
                        case Target.ALLY:
                        case Target.ENEMY:
                            ((Card)target).BoostStats(intParameter1, intParameter2);
                            break;
                        case Target.AALLY:
                            foreach (Card card in Board.Instance.GetCardsOnTable(source.contender))
                            {
                                card.BoostStats(intParameter1, intParameter2);
                            }
                            break;
                        case Target.AENEMY:
                            foreach (Card card in Board.Instance.GetCardsOnTable(CardGameManager.Instance.GetOtherContender(source.contender)))
                            {
                                card.BoostStats(intParameter1, intParameter2);
                            }
                            break;
                        case Target.SELF:
                            source.BoostStats(intParameter1, intParameter2);
                            break;
                    }
                    break;

                case SubType.ADD_EFFECT:
                    CardEffect effect = new CardEffect();
                    effect.type = EffectType.BOOST;
                    effect.subType = cardParameter_Effect;
                    effect.applyTime = ApplyTime.COMBAT;

                    switch (targetType)
                    {
                        case Target.ALLY:
                        case Target.ENEMY:
                        case Target.CARD:
                            ((Card)target).AddEffect(effect);
                            break;
                        case Target.AALLY:
                            foreach (Card card in Board.Instance.GetCardsOnTable(source.contender))
                            {
                                card.AddEffect(effect);
                            }
                            break;
                    }
                    break;

                case SubType.GUARD:
                    TurnManager.Instance.SetGuardCard(source);
                    break;
            }
        }

        private void ApplyTacticalEffect(Card source, object target)
        {
            switch (subType)
            {
                case SubType.NONE:
                    break;
                case SubType.CREATE_CARD:
                    {
                        CardZone emptyCardZone = Board.Instance.GetEmptyCardZone(source.contender);
                        if (emptyCardZone != null)
                        {
                            Card card = InstantiateCard(source.contender, source.transform.position, GetDataFromParameters(), cardRevealed: true);
                            emptyCardZone.AddCard(card);
                        }
                    }
                    break;

                case SubType.DUPLICATE_CARD:
                    {
                        CardZone emptyCardZone = Board.Instance.GetEmptyCardZone(source.contender);
                        if (emptyCardZone != null)
                        {
                            Card card = InstantiateCard(source.contender, source.transform.position, ((Card)target).data, cardRevealed: true);
                            emptyCardZone.AddCard(card);
                        }
                    }
                    break;

                case SubType.SWAP_POSITION:
                    SwapPosition((Card)target);
                    break;

                case SubType.SWAP_CONTENDER:
                    if (targetType == Target.SELF)
                    {
                        CardZone emptyCardZone = Board.Instance.GetEmptyCardZone(CardGameManager.Instance.GetOtherContender(source.contender));
                        if (emptyCardZone != null) SwapContender(source, source, emptyCardZone);
                    }
                    break;

                case SubType.DRAW_CARD:
                    Board.Instance.DrawCards(intParameter1, TurnManager.Instance.turn);
                    break;

                case SubType.DISCARD_CARD:
                    CardGameManager.Instance.otherPlayer.hand.DiscardCards(intParameter1);
                    break;

                case SubType.RETURN_CARD:
                    ((Card)target).ReturnToHand();
                    break;

                case SubType.FREE_MANA:
                    source.contender.SetFreeMana(true);
                    break;

                case SubType.WHEEL:
                    Board.Instance.WheelEffect();
                    break;

                case SubType.SKIP_COMBAT:
                    TurnManager.Instance.SkipCombat();
                    break;

                case SubType.MIRROR:
                    TurnManager.Instance.SetMirror(source.contender, true);
                    break;

                case SubType.STEAL_MANA:
                    TurnManager.Instance.SetStealMana(source.contender);
                    break;

                case SubType.ADD_CARD_TO_DECK:
                    {
                        int number = source.manaCost;
                        if (source.manaCost == 0)
                        {
                            number = source.contender.currentMana;
                            source.contender.SubstractMana(number);
                        }
                        AddCardToDeck(source, GetDataFromParameters(), number);
                    }
                    break;

                case SubType.DISCARD_CARD_FROM_DECK:
                    {
                        int number = intParameter1;
                        if (HasTarget()) number = ((Card)target).manaCost;

                        CardGameManager.Instance.GetOtherContender(source.contender).deck.DiscardCards(number);
                    }
                    break;

                case SubType.STEAL_CARD:
                    if (target is Card)
                    {
                        Card targetCard = (Card)target;
                        CardZone dest = null;

                        if (applyTime == ApplyTime.COMBAT && targetCard.strength >= source.defense)
                        {
                            int position = Board.Instance.GetPositionFromCard(source);
                            dest = source.contender.cardZones[position];
                        }
                        else if (source.type == CardType.ACTION)
                        {
                            dest = Board.Instance.GetEmptyCardZone(source.contender);
                        }

                        if (dest != null)
                        {
                            SwapContender(source, targetCard, dest);
                            source.contender.stolenCards++;
                        }
                    }
                    break;

                case SubType.STEAL_CARD_FROM_HAND:
                    Contender otherContender = CardGameManager.Instance.GetOtherContender(source.contender);
                    int loops = Mathf.Min(intParameter1, otherContender.hand.numCards);

                    for (int i = 0; i < loops; i++)
                    {
                        Card card = otherContender.hand.StealCard();
                        Card newCard = InstantiateCard(source.contender, card.transform.position, card.data, source.contender.isPlayer);
                        source.contender.hand.AddCard(newCard);
                        source.contender.stolenCards++;
                        card.DestroyCard(instant: true, continueFlow: false);
                    }

                    break;
            }
        }

        private void AddCardToDeck(Card source, CardsData data, int number)
        {
            List<Card> cards = new List<Card>();

            Contender owner = source.contender;
            GameObject cardPrefab = owner.deck.cardPrefab;

            for (int i = 0; i < number; i++)
            {
                GameObject cardObj = UnityEngine.Object.Instantiate(cardPrefab, source.transform.position, cardPrefab.transform.rotation);
                cardObj.SetActive(false);

                CardsData newData = new CardsData(data);
                Card card = cardObj.GetComponent<Card>();
                card.Initialize(owner, newData, cardRevealed: true);
                cards.Add(card);
            }

            owner.deck.AddCards(cards);
        }

        private Card InstantiateCard(Contender newOwner, Vector3 position, CardsData data, bool cardRevealed)
        {
            GameObject cardPrefab = newOwner.deck.cardPrefab;
            GameObject cardObj = UnityEngine.Object.Instantiate(cardPrefab, position, cardPrefab.transform.rotation);
            CardsData newData = new CardsData(data);
            Card card = cardObj.GetComponent<Card>();
            card.Initialize(newOwner, newData, cardRevealed);
            return card;
        }

        private CardsData GetDataFromParameters()
        {
            CardsData data = new CardsData
            {
                name = cardParameter.name,
                sprite = cardParameter.sprite,
                strength = cardParameter.strength,
                defense = cardParameter.defense,
                cost = cardParameter.manaCost
            };

            if (cardParameter_Effect != SubType.NONE)
            {
                data.AddEffect(new CardEffect()
                {
                    type = GetTypeFromSubType(cardParameter_Effect),
                    subType = cardParameter_Effect,
                    applyTime = ApplyTime.COMBAT //TODO other effects not combat
                });
            }

            return data;
        }

        private void SwapPosition(Card target)
        {
            if (targetType == Target.CARD)
            {
                // Get origin position
                int pos = Board.Instance.GetPositionFromCard(target);

                // Get destination
                List<int> possibleCardZones = new List<int>();
                if (pos > 0) possibleCardZones.Add(pos - 1);
                if (pos < 3) possibleCardZones.Add(pos + 1);

                int dest = possibleCardZones[UnityEngine.Random.Range(0, possibleCardZones.Count)];

                Swap(pos, dest, target.contender);
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
                int dest = possibleCardZones[UnityEngine.Random.Range(0, possibleCardZones.Count)];

                Swap(pos, dest, contender);
            }
        }

        private void SwapContender(Card source, Card target, CardZone dest)
        {
            // Get origin card zone
            int position = Board.Instance.GetPositionFromCard(target);
            CardZone originCardZone = target.contender.cardZones[position];

            // Create card with other prefab 
            Card newCard = InstantiateCard(source.contender, target.transform.position, target.data, cardRevealed: true);

            // Empty destination zone
            dest.GetCard()?.RemoveFromContainer();

            // Destroy original
            Card originCard = originCardZone.GetCard();
            originCard.DestroyCard(instant: true, continueFlow: true);

            dest.AddCard(newCard);
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

        private void ApplyAlternateWinCondition(Card source, object target)
        {
            // win interview
            TurnManager.Instance.AlternateWinCondition();
        }

        #endregion

        #region IsAppliable

        public bool IsAppliable()
        {
            if (type == EffectType.ALTERNATE_WIN_CONDITION)
            {
                switch (subType)
                {
                    case SubType.CITRIANO_WIN_CONDITION:
                        return DeckManager.Instance.GetOpponentName() == Opponent_Name.Citriano
                            && CardGameManager.Instance.player.life >= 30;
                    case SubType.PINPONBROS_WIN_CONDITION:
                        return DeckManager.Instance.GetOpponentName() == Opponent_Name.PingPongBros
                            && CardGameManager.Instance.alternateWinConditionParameter >= 15;
                    case SubType.SECRETARY_WIN_CONDITION:
                        return DeckManager.Instance.GetOpponentName() == Opponent_Name.Secretary
                            && CardGameManager.Instance.opponent.deck.numCards == 0;
                    default:
                        return false;
                }
            }
            else
            {

                bool currentPlayerHasCards = Board.Instance.AreCardsOnTable(CardGameManager.Instance.currentPlayer);
                bool otherPlayerHasCards = Board.Instance.AreCardsOnTable(CardGameManager.Instance.otherPlayer);

                switch (targetType)
                {
                    case Target.ALLY:
                    case Target.AALLY:
                        CardZone emptyCardZone = Board.Instance.GetEmptyCardZone(CardGameManager.Instance.currentPlayer);
                        if (subType == SubType.DUPLICATE_CARD)
                            return emptyCardZone != null && currentPlayerHasCards;
                        else if (subType == SubType.CREATE_CARD)
                            return emptyCardZone;
                        else
                            return currentPlayerHasCards;

                    case Target.ENEMY:
                    case Target.AENEMY:
                        if (subType == SubType.SWAP_POSITION)
                            return otherPlayerHasCards;
                        else
                            return otherPlayerHasCards || CardGameManager.Instance.otherPlayer.fieldCardZone.isNotEmpty;

                    case Target.CARD:
                    case Target.ACARD:
                        return currentPlayerHasCards || otherPlayerHasCards
                            || CardGameManager.Instance.player.fieldCardZone.isNotEmpty
                            || CardGameManager.Instance.opponent.fieldCardZone.isNotEmpty;

                    case Target.FIELDCARD:
                        return CardGameManager.Instance.otherPlayer.fieldCardZone.isNotEmpty;

                    case Target.NONE:
                    case Target.PLAYER:
                    case Target.SELF:
                    case Target.ALL:
                    default:
                        return true;
                }
            }
        }

        #endregion

        #region Target

        public bool HasTarget()
        {
            return targetType == Target.ALLY
                || targetType == Target.ENEMY
                || targetType == Target.CARD;
        }

        public List<Card> FindPossibleTargets()
        {
            List<Card> possibleTargets = new List<Card>();

            switch (targetType)
            {
                case Target.ALLY:
                    possibleTargets.AddRange(Board.Instance.GetCardsOnTable(CardGameManager.Instance.currentPlayer));
                    CheckAddFieldCard(subType, possibleTargets, CardGameManager.Instance.currentPlayer);
                    break;

                case Target.ENEMY:
                    possibleTargets.AddRange(Board.Instance.GetCardsOnTable(CardGameManager.Instance.otherPlayer));
                    CheckAddFieldCard(subType, possibleTargets, CardGameManager.Instance.otherPlayer);
                    break;

                case Target.CARD:
                    possibleTargets.AddRange(Board.Instance.GetCardsOnTable(CardGameManager.Instance.currentPlayer));
                    possibleTargets.AddRange(Board.Instance.GetCardsOnTable(CardGameManager.Instance.otherPlayer));
                    CheckAddFieldCard(subType, possibleTargets, CardGameManager.Instance.currentPlayer);
                    CheckAddFieldCard(subType, possibleTargets, CardGameManager.Instance.otherPlayer);
                    break;
            }

            return possibleTargets;
        }

        private void CheckAddFieldCard(SubType subType, List<Card> possibleTargets, Contender contender)
        {
            if (subType == SubType.DESTROY_CARD || subType == SubType.RETURN_CARD)
            {
                if (contender.fieldCardZone.isNotEmpty)
                    possibleTargets.Add(contender.fieldCardZone.GetCard());
            }
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            switch (subType)
            {
                case SubType.NONE:
                    return "";

                case SubType.RESTORE_LIFE:
                    return "Recuperar vida (" + intParameter1 + ")";
                case SubType.INCREASE_MAX_MANA:
                    return "Incrementar maná máximo (" + intParameter1 + ")";

                case SubType.DESTROY_CARD:
                    return "Destruir carta";
                case SubType.DEAL_DAMAGE:
                    return "Infligir daño (" + intParameter1 + ")";
                case SubType.DECREASE_MANA:
                    return "Reducir maná (" + intParameter1 + ")";
                case SubType.STEAL_REWARD:
                    return "Recompensa de ladrón";

                case SubType.LIFELINK:
                    return "Vínculo vital";
                case SubType.REBOUND:
                    return "Rebote";
                case SubType.TRAMPLE:
                    return "Arrollar";
                case SubType.STAT_BOOST:
                    return "Bonificador de parámetros (" + intParameter1 + "/" + intParameter2 + ")";
                case SubType.ADD_EFFECT:
                    return "Añadir efecto";
                case SubType.GUARD:
                    return "Guardia";

                case SubType.DUPLICATE_CARD:
                    return "Duplicar carta";
                case SubType.CREATE_CARD:
                    return "Invocar carta (" + cardParameter?.name + ")";
                case SubType.SWAP_POSITION:
                    return "Cambio de posición";
                case SubType.SWAP_CONTENDER:
                    return "Cambio de jugador";
                case SubType.DRAW_CARD:
                    return "Robar cartas (" + intParameter1 + ")";
                case SubType.DISCARD_CARD:
                    return "Descartar cartas (" + intParameter1 + ")";
                case SubType.RETURN_CARD:
                    return "Devolver a la mano";
                case SubType.FREE_MANA:
                    return "Juega carta gratis";
                case SubType.WHEEL:
                    return "Rueda";
                case SubType.COMPARTMENTALIZE:
                    return "Compartimentar";
                case SubType.SKIP_COMBAT:
                    return "Omitir combate";
                case SubType.MIRROR:
                    return "Espejo";
                case SubType.STEAL_MANA:
                    return "Robar maná";
                case SubType.ADD_CARD_TO_DECK:
                    return "Añadir al mazo";
                case SubType.DISCARD_CARD_FROM_DECK:
                    return "Descartar del mazo";
                case SubType.STEAL_CARD:
                case SubType.STEAL_CARD_FROM_HAND:
                    return "Robar carta";

                case SubType.CITRIANO_WIN_CONDITION:
                case SubType.PINPONBROS_WIN_CONDITION:
                case SubType.SECRETARY_WIN_CONDITION:
                    return "Condición de victoria alternativa";
            }

            return "";
        }

        public string ToStringExtended(CardType type)
        {
            string s = "";

            if (type != CardType.ACTION)
            {
                switch (applyTime)
                {
                    case ApplyTime.ENTER:
                        s += "Al entrar en el campo, ";
                        break;
                    case ApplyTime.END:
                        s += "Al final del turno, ";
                        break;
                    case ApplyTime.COMBAT:
                        s += "Al combatir, ";
                        break;
                    case ApplyTime.DRAW_CARD:
                        s += "Al robar una carta, ";
                        break;
                    case ApplyTime.PLAY_ARGUMENT:
                        s += "Al jugar un argumento, ";
                        break;
                }
            }
            switch (subType)
            {
                case SubType.NONE:
                    return "";

                case SubType.RESTORE_LIFE:
                    s += "el jugador recupera " + intParameter1 + ((intParameter1 > 1) ? " puntos" : " punto") + " de vida."; break;
                case SubType.INCREASE_MAX_MANA:
                    s += "consigues " + intParameter1 + " cristal extra de maná."; break;
                case SubType.STEAL_REWARD:
                    s += "el jugador recupera 5 puntos de vida por cada carta robada."; break;

                case SubType.DESTROY_CARD:
                    switch (targetType)
                    {
                        case Target.ENEMY:
                        case Target.CARD: s += "destruye la carta objetivo."; break;
                        case Target.AENEMY: s += "destruye todas las cartas del oponente."; break;
                        case Target.ACARD: s += "destruye todas las cartas sobre el campo."; break;
                        case Target.FIELDCARD: s += "destruye la carta de campo del oponente."; break;
                    }
                    break;
                case SubType.DEAL_DAMAGE:
                    switch (targetType)
                    {
                        case Target.ENEMY: s += "inflige " + intParameter1 + ((intParameter1 > 1) ? " puntos" : " punto") + " de daño al argumento objetivo."; break;
                        case Target.AENEMY: s += "inflige " + intParameter1 + ((intParameter1 > 1) ? " puntos" : " punto") + " de daño a todos los argumentos del oponente."; break;
                        case Target.PLAYER: s += "inflige " + intParameter1 + ((intParameter1 > 1) ? " puntos" : " punto") + " de daño al oponente."; break;
                    }
                    break;
                case SubType.DECREASE_MANA:
                    if (intParameter1 == 0) s += "reduce el maná del oponente a 0.";
                    else s += "reduce el maná del oponente en " + intParameter1 + " puntos."; break;

                case SubType.LIFELINK:
                    s += "el jugador recupera el daño infligido por el argumento enemigo."; break;
                case SubType.REBOUND:
                    s += "el daño recibido es infligido al argumento enemigo."; break;
                case SubType.TRAMPLE:
                    s += "si la fuerza del argumento es mayor que la defensa del argumento rival, el daño restante es infligido al oponente."; break;
                case SubType.STAT_BOOST:
                    switch (targetType)
                    {
                        case Target.SELF: s += "obtiene un bonificador de " + intParameter1 + " / " + intParameter2 + " al final del turno."; break;
                        case Target.ALLY: s += "el objetivo recibe un bonificador de " + intParameter1 + " / " + intParameter2 + "."; break;
                        case Target.AALLY: s += "todos los argumentos aliados obtienen un bonificador de " + intParameter1 + "/" + intParameter2 + "."; break;
                    }
                    break;
                case SubType.ADD_EFFECT:
                    switch (targetType)
                    {
                        case Target.AALLY: s += "todos los argumentos aliados obtienen el efecto " + cardParameter_Effect + "."; break;
                    }
                    break;
                case SubType.GUARD:
                    s += "esta carta recibe todo el daño infligido por el oponente."; break;
                case SubType.COMPARTMENTALIZE:
                    s += "el oponente descarta el número de cartas de la baraja equivalente al daño que fuera a recibir."; break;
                case SubType.INCOMPARTMENTABLE:
                    s += "si este argumento fuera a ser descartado del mazo, pasa a la mano del jugador. Corta el efecto de descarte."; break;
                case SubType.SPONGE:
                    s += "suma a la fuerza el daño recibido."; break;

                case SubType.DUPLICATE_CARD:
                    if (targetType == Target.ALLY) s += "duplica el argumento objetivo.";
                    else if (targetType == Target.ENEMY) s += "duplica el argumento rival objetivo.";
                    break;
                case SubType.CREATE_CARD:
                    s += "invoca el argumento " + cardParameter.name + " en una zona libre."; break;
                case SubType.SWAP_POSITION:
                    s += "mueve el argumento objetivo a una zona adyacente. Si ya hay un argumento, se intercambian."; break;
                case SubType.SWAP_CONTENDER:
                    s += "la carta pasa al control del oponente."; break;
                case SubType.DRAW_CARD:
                    s += "roba " + intParameter1 + ((intParameter1 > 1) ? " cartas." : " carta."); break;
                case SubType.DISCARD_CARD:
                    s += "el oponente descarta " + intParameter1 + ((intParameter1 > 1) ? " cartas." : " carta."); break;
                case SubType.RETURN_CARD:
                    s += "devuelve el argumento objetivo a la mano de su propietario."; break;
                case SubType.FREE_MANA:
                    s += "la próxima carta que juegues tendrá coste 0."; break;
                case SubType.WHEEL:
                    s += "los jugadores descartan las cartas de su mano y roban la misma cantidad."; break;
                case SubType.SKIP_COMBAT:
                    s += "se omite la ronda de combate."; break;
                case SubType.MIRROR:
                    s += "el daño que fuera a recibir el jugador, lo recibe el oponente."; break;
                case SubType.STEAL_MANA:
                    s += "puedes utilizar el maná del oponente durante el turno."; break;
                case SubType.ADD_CARD_TO_DECK:
                    s += "se añaden tantas cartas " + cardParameter.name + " al mazo como el maná restante."; break;
                case SubType.DISCARD_CARD_FROM_DECK:
                    s += "se descartan tantas cartas del mazo del oponente como el coste de maná del argumento objetivo."; break;
                case SubType.STEAL_CARD:
                    if (applyTime == ApplyTime.COMBAT) s += "roba el argumento que lo destruya.";
                    else s += "roba la carta objetivo.";
                    break;
                case SubType.STEAL_CARD_FROM_HAND:
                    s += "roba " + intParameter1 + ((intParameter1 > 1) ? " cartas" : " carta") + " de la mano del oponente."; break;


                case SubType.CITRIANO_WIN_CONDITION:
                    s += "si tienes 30 o más vidas, ganas la partida. (" + CardGameManager.Instance.player.life + "/30)"; break;
                case SubType.PINPONBROS_WIN_CONDITION:
                    s += "si has rebotado 15 o más puntos de vida, ganas la partida. (" + CardGameManager.Instance.alternateWinConditionParameter + "/15)"; break;
                case SubType.SECRETARY_WIN_CONDITION:
                    s += "si el oponente no tiene cartas en el mazo, ganas la partida. (" + CardGameManager.Instance.opponent.deck.numCards + " cartas restantes)"; break;
            }

            if (s.Length > 0) s = s[0].ToString().ToUpper() + s.Substring(1, s.Length - 1);

            return s;
        }

        #endregion
    }

}