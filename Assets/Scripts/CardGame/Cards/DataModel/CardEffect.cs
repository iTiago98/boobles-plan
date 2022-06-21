using CardGame.Level;
using CardGame.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Cards.DataModel.Effects
{
    #region Enums

    public enum EffectType
    {
        NONE, DEFENSIVE, OFFENSIVE, BOOST, TACTICAL
    }

    public enum SubType
    {
        NONE,
        RESTORE_LIFE, INCREASE_MAX_MANA,
        DESTROY_CARD, DEAL_DAMAGE, DECREASE_MANA,
        LIFELINK, REBOUND, TRAMPLE, STAT_BOOST, ADD_EFFECT, GUARD,
        CREATE_CARD, SWAP_POSITION, SWAP_CONTENDER, DRAW_CARD, DISCARD_CARD, RETURN_CARD
    }

    public enum DefensiveType
    {
        NONE, RESTORE_LIFE, INCREASE_MAX_MANA
    }

    public enum OffensiveType
    {
        NONE, DESTROY_CARD, DEAL_DAMAGE, DECREASE_MANA
    }

    public enum BoostType
    {
        NONE, LIFELINK, REBOUND, TRAMPLE, STAT_BOOST, ADD_EFFECT, GUARD
    }

    public enum TacticalType
    {
        NONE, CREATE_CARD, SWAP_POSITION, SWAP_CONTENDER, DRAW_CARD, DISCARD_CARD, RETURN_CARD
    }

    public enum ApplyTime
    {
        NONE, START, ENTER, COMBAT, END
    }

    public enum Target
    {
        NONE, ALLY, ENEMY, CARD, CARDZONE
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
        public GameObject gObjParameter;

        #region Apply

        public void Apply(Card source, object target)
        {
            Debug.Log(type + " " + subType + " effect applied");

            if (target != null && target is Card)
                ((Card)target).highlight.SetActive(false);

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
            }
        }

        private void ApplyOffensiveEffect(Card source, object target)
        {
            switch (subType)
            {
                case SubType.NONE:
                    break;
                case SubType.DESTROY_CARD:
                    if (targetType == Target.ENEMY) ((Card)target).Destroy();
                    else Board.Instance.DestroyAll();
                    break;
                case SubType.DEAL_DAMAGE:
                    if (targetType == Target.ENEMY) ((Card)target).ReceiveDamage(intParameter1);
                    // else daño a todos
                    break;
                case SubType.DECREASE_MANA:
                    TurnManager.Instance.currentPlayer.SubstractMana(intParameter1);
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
                    int lifeValue = 0;
                    if (target is Card)
                    {
                        Card targetCard = (Card)target;
                        lifeValue = (source.strength >= targetCard.defense) ? targetCard.defense : source.strength;
                    }
                    else if (target is Contender)
                    {
                        lifeValue = source.strength;
                    }
                    source.contender.RestoreLife(lifeValue);
                    break;
                case SubType.REBOUND:
                    break;
                case SubType.TRAMPLE:
                    break;
                case SubType.STAT_BOOST:
                    if (targetType == Target.ALLY) ((Card)target).BoostStats(intParameter1, intParameter2);
                    else
                    {
                        foreach (Card card in Board.Instance.CardsOnTable(TurnManager.Instance.currentPlayer))
                        {
                            card.BoostStats(intParameter1, intParameter2);
                        }
                    }
                    break;
                case SubType.ADD_EFFECT:
                    break;
                case SubType.GUARD:
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
                    break;
                case SubType.SWAP_POSITION:

                    List<CardZone> cardZones = null;
                    int pos = -1;

                    foreach (CardZone cardZone in Board.Instance.playerCardZone)
                    {
                        Card card = cardZone.GetCard();
                        if (card != null && card == (Card)target)
                        {
                            cardZones = Board.Instance.playerCardZone;
                            pos = cardZones.IndexOf(cardZone);
                        }
                    }

                    if (cardZones == null)
                    {
                        foreach (CardZone cardZone in Board.Instance.opponentCardZone)
                        {
                            Card card = cardZone.GetCard();
                            if (card != null && card == (Card)target)
                            {
                                cardZones = Board.Instance.opponentCardZone;
                                pos = cardZones.IndexOf(cardZone);
                            }
                        }
                    }

                    List<int> possibleCardZones = new List<int>();
                    if (pos > 0) possibleCardZones.Add(pos - 1);
                    if (pos < 3) possibleCardZones.Add(pos + 1);

                    int dest = possibleCardZones[UnityEngine.Random.Range(0, possibleCardZones.Count)];
                    CardZone destCardZone = cardZones[dest];
                    CardZone originCardZone = cardZones[pos];

                    originCardZone.RemoveCard(originCardZone.GetCard().gameObject);
                    if(destCardZone.GetCard() != null)
                    {
                        Card temp = destCardZone.GetCard();
                        destCardZone.RemoveCard(temp.gameObject);
                        originCardZone.AddCard(temp);
                    }
                    destCardZone.AddCard((Card)target);

                    break;

                case SubType.SWAP_CONTENDER:
                    break;
                case SubType.DRAW_CARD:
                    Board.Instance.DrawCards(intParameter1, TurnManager.Instance.turn);
                    break;
                case SubType.DISCARD_CARD:
                    Board.Instance.DiscardCards(intParameter1, TurnManager.Instance.otherPlayer);
                    break;
                case SubType.RETURN_CARD:
                    ((Card)target).ReturnToHand();
                    break;
            }
        }


        #endregion

        #region IsAppliable

        public bool IsAppliable()
        {
            switch (type)
            {
                case EffectType.NONE:
                    return false;
                case EffectType.DEFENSIVE:
                    return DefensiveIsAppliable();
                case EffectType.OFFENSIVE:
                    return OffensiveIsAppliable();
                case EffectType.BOOST:
                    return BoostIsAppliable();
                case EffectType.TACTICAL:
                    return TacticalIsAppliable();
                default:
                    return false;
            }
        }

        private bool DefensiveIsAppliable()
        {
            switch (subType)
            {
                case SubType.NONE:
                    return false;
                case SubType.RESTORE_LIFE:
                case SubType.INCREASE_MAX_MANA:
                default:
                    return true;
            }
        }

        private bool OffensiveIsAppliable()
        {
            switch (subType)
            {
                case SubType.NONE:
                    return false;
                case SubType.DESTROY_CARD:
                    return Board.Instance.NumCardsOnTable(TurnManager.Instance.otherPlayer) > 0;
                case SubType.DEAL_DAMAGE:
                case SubType.DECREASE_MANA:
                default:
                    return true;
            }
        }

        private bool BoostIsAppliable()
        {
            switch (subType)
            {
                case SubType.NONE:
                    break;
                case SubType.LIFELINK:
                    break;
                case SubType.REBOUND:
                    break;
                case SubType.TRAMPLE:
                    break;
                case SubType.STAT_BOOST:
                case SubType.ADD_EFFECT:
                    return Board.Instance.NumCardsOnTable(TurnManager.Instance.currentPlayer) > 0;
                case SubType.GUARD:
                    break;
            }

            return false;
        }

        private bool TacticalIsAppliable()
        {
            int currentNumCardsOnTable = Board.Instance.NumCardsOnTable(TurnManager.Instance.currentPlayer);
            int otherNumCardsOnTable = Board.Instance.NumCardsOnTable(TurnManager.Instance.otherPlayer);
            int maxCardNumber = Board.Instance.MaxCardNumber();

            switch (subType)
            {
                case SubType.NONE:
                case SubType.DRAW_CARD:
                case SubType.DISCARD_CARD:
                    return true;
                case SubType.CREATE_CARD: // Empty player card zone
                    return currentNumCardsOnTable < maxCardNumber;
                case SubType.SWAP_POSITION: // Cards on table (player or opponent)
                    return currentNumCardsOnTable > 0 || otherNumCardsOnTable > 0;
                case SubType.RETURN_CARD: // Cards on table (player or opponent)
                    return otherNumCardsOnTable > 0;
                case SubType.SWAP_CONTENDER: // Empty card zone other contender
                    return otherNumCardsOnTable < maxCardNumber;
                default:
                    return false;
            }
        }

        #endregion

        #region Target

        public bool HasTarget()
        {
            return targetType != Target.NONE;
        }

        public List<Card> FindPossibleTargets()
        {
            List<Card> possibleTargets = new List<Card>();

            switch (targetType)
            {
                case Target.NONE:
                    break;
                case Target.ALLY:
                    possibleTargets.AddRange(Board.Instance.CardsOnTable(TurnManager.Instance.currentPlayer));
                    break;
                case Target.ENEMY:
                    possibleTargets.AddRange(Board.Instance.CardsOnTable(TurnManager.Instance.otherPlayer));
                    break;
                case Target.CARD:
                    possibleTargets.AddRange(Board.Instance.CardsOnTable(TurnManager.Instance.currentPlayer));
                    possibleTargets.AddRange(Board.Instance.CardsOnTable(TurnManager.Instance.otherPlayer));
                    break;
                case Target.CARDZONE:
                    break;
            }

            return possibleTargets;
        }

        #endregion

        public override string ToString()
        {
            switch (subType)
            {
                case SubType.NONE:
                    return "";

                case SubType.RESTORE_LIFE:
                    return "El jugador recupera " + intParameter1 + " puntos de vida.";
                case SubType.INCREASE_MAX_MANA:
                    return "Consigues " + intParameter1 + " cristal extra de maná.";

                case SubType.DESTROY_CARD:
                    if (targetType == Target.ENEMY) return "Destruye la carta objetivo.";
                    else return "Destruye todas las cartas sobre el campo.";
                case SubType.DEAL_DAMAGE:
                    if (targetType == Target.ENEMY) return "Inflige " + intParameter1 + "puntos de daño a la carta objetivo.";
                    else return "Inflige " + intParameter1 + "puntos de daño a todas las cartas del oponente.";
                case SubType.DECREASE_MANA:
                    return "Reduce el maná del oponente en " + intParameter1 + " puntos.";

                case SubType.LIFELINK:
                    return "El jugador recupera el daño infligido por la carta.";
                case SubType.REBOUND:
                    break;
                case SubType.TRAMPLE:
                    break;
                case SubType.STAT_BOOST:
                    if (targetType == Target.ALLY) return "El objetivo recibe un bonificador de " + intParameter1 + "/" + intParameter2 + ".";
                    else return "Todas las cartas aliadas obtienen un bonificador de " + intParameter1 + "/" + intParameter2 + ".";
                case SubType.ADD_EFFECT:
                    break;
                case SubType.GUARD:
                    break;

                case SubType.CREATE_CARD:
                    return "Invoca la carta " + gObjParameter.name + " en la zona objetivo.";
                case SubType.SWAP_POSITION:
                    return "Mueve la carta objetivo a la zona objetivo. Si ya hay una carta, se intercambian.";
                case SubType.SWAP_CONTENDER:
                    return "La carta pasa al control del oponente.";
                case SubType.DRAW_CARD:
                    return "Roba " + intParameter1 + " cartas.";
                case SubType.DISCARD_CARD:
                    return "El oponente descarta " + intParameter1 + " cartas.";
                case SubType.RETURN_CARD:
                    return "Devuelve la carta objetivo a la mano de su propietario.";
            }

            return "";
        }
    }

}