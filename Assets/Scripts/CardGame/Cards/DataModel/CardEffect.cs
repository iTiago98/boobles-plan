using CardGame.Level;
using CardGame.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Cards.DataModel.Effects
{
    [Serializable]
    public class CardEffect
    {
        public enum Type
        {
            NONE, DEFENSIVE, OFFENSIVE, BOOST, TACTICAL
        }
        public Type type { get; set; }

        #region Sub Types

        public enum SubType
        {
            NONE,
            RESTORE_LIFE, INCREASE_MAX_MANA,
            DESTROY_CARD, DEAL_DAMAGE, DECREASE_MANA,
            LIFELINK, REBOUND, TRAMPLE, STAT_BOOST, ADD_EFFECT, GUARD,
            CREATE_CARD, SWAP_POSITION, SWAP_CONTENDER, DRAW_CARD, DISCARD_CARD, RETURN_CARD
        }

        public SubType subType;

        public enum Defensive
        {
            NONE, RESTORE_LIFE, INCREASE_MAX_MANA
        }

        public enum Offensive
        {
            NONE, DESTROY_CARD, DEAL_DAMAGE, DECREASE_MANA
        }

        public enum Boost
        {
            NONE, LIFELINK, REBOUND, TRAMPLE, STAT_BOOST, ADD_EFFECT, GUARD
        }

        public enum Tactical
        {
            NONE, CREATE_CARD, SWAP_POSITION, SWAP_CONTENDER, DRAW_CARD, DISCARD_CARD, RETURN_CARD
        }

        public void SetSubTypeFromChild(Enum childSubType)
        {
            subType = (SubType)Enum.Parse(typeof(SubType), childSubType.ToString());
        }

        #endregion

        public enum ApplyTime
        {
            NONE, START, ENTER, COMBAT, END
        }

        public ApplyTime applyTime;
        public int intParameter1;
        public int intParameter2;
        public bool boolParameter; // Target
        public GameObject gObjParameter;

        #region Apply

        public void Apply(Card source, object target)
        {
            switch (type)
            {
                case Type.NONE:
                    break;
                case Type.DEFENSIVE:
                    ApplyDefensiveEffect(source, target);
                    break;
                case Type.OFFENSIVE:
                    ApplyOffensiveEffect(source, target);
                    break;
                case Type.BOOST:
                    ApplyBoostEffect(source, target);
                    break;
                case Type.TACTICAL:
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
                    if (boolParameter) ((Card)target).Destroy();
                    break;
                case SubType.DEAL_DAMAGE:
                    if (boolParameter) ((Card)target).ReceiveDamage(intParameter1);
                    // else daño a todos
                    break;
                case SubType.DECREASE_MANA:
                    TurnManager.Instance.currentPlayer.MinusMana(intParameter1);
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
                    if (boolParameter) ((Card)target).BoostStats(intParameter1, intParameter2);
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
                    break;
                case SubType.SWAP_CONTENDER:
                    break;
                case SubType.DRAW_CARD:
                    Board.Instance.DrawCards(intParameter1, TurnManager.Instance.currentPlayer);
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
                case Type.NONE:
                    return false;
                case Type.DEFENSIVE:
                    return DefensiveIsAppliable();
                case Type.OFFENSIVE:
                    return OffensiveIsAppliable();
                case Type.BOOST:
                    return BoostIsAppliable();
                case Type.TACTICAL:
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
                    return false;
            }
        }

        private bool OffensiveIsAppliable()
        {
            switch (subType)
            {
                case SubType.NONE:
                    return false;
                case SubType.DESTROY_CARD:
                    return Board.Instance.NumCardsOnTable(TurnManager.Instance.otherPlayer);
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
                    break;
                case SubType.ADD_EFFECT:
                    break;
                case SubType.GUARD:
                    break;
            }

            return false;
        }

        private bool TacticalIsAppliable()
        {
            switch (subType)
            {
                case SubType.NONE:
                case SubType.DRAW_CARD:
                case SubType.DISCARD_CARD:
                    return true;
                case SubType.CREATE_CARD: // Empty player card zone
                case SubType.SWAP_POSITION: // Cards on table (player or opponent)
                case SubType.SWAP_CONTENDER: // Empty card zone other contender
                case SubType.RETURN_CARD: // Cards on table (player or opponent)
                default:
                    return false;
            }
        }

        #endregion

        #region GetTarget

        public string GetTarget()
        {
            switch (subType)
            {
                case SubType.NONE:

                case SubType.RESTORE_LIFE:
                case SubType.INCREASE_MAX_MANA:
                case SubType.LIFELINK:
                case SubType.REBOUND:
                case SubType.TRAMPLE:
                case SubType.GUARD:
                case SubType.DRAW_CARD:
                case SubType.DISCARD_CARD:
                case SubType.SWAP_CONTENDER:
                    return "";
                case SubType.DESTROY_CARD:
                    break;
                case SubType.DEAL_DAMAGE:
                    break;
                case SubType.DECREASE_MANA:
                    break;

                case SubType.STAT_BOOST:
                    break;
                case SubType.ADD_EFFECT:
                    break;

                case SubType.CREATE_CARD:
                    return "CardZone";
                case SubType.SWAP_POSITION:
                    return "Card(AllyOrEnemy) and Card/CardZone";
                case SubType.RETURN_CARD:
                    return "Card(AllyOrEnemy)";
                default:
                    return "";
            }

            return "";
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
                    if (boolParameter) return "Destruye la carta objetivo.";
                    else return "Destruye todas las cartas sobre el campo.";
                case SubType.DEAL_DAMAGE:
                    if (boolParameter) return "Inflige " + intParameter1 + "puntos de daño a la carta objetivo.";
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
                    return "El objetivo recibe un bonificador de " + intParameter1 + "/" + intParameter2 + ".";
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