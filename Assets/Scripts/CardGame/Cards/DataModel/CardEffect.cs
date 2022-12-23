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
        STEAL_REWARD, STEAL_CARD_FROM_DECK, STAT_DECREASE
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
        NONE, LIFELINK, REBOUND, TRAMPLE, STAT_BOOST, ADD_EFFECT, GUARD, COMPARTMENTALIZE, INCOMPARTMENTABLE, SPONGE, STAT_DECREASE
    }

    public enum TacticalType
    {
        NONE, DUPLICATE_CARD, CREATE_CARD, SWAP_POSITION, SWAP_CONTENDER, DRAW_CARD, DISCARD_CARD, RETURN_CARD,
        FREE_MANA, WHEEL, SKIP_COMBAT, ADD_CARD_TO_DECK, DISCARD_CARD_FROM_DECK, STEAL_CARD, STEAL_CARD_FROM_HAND,
        MIRROR, STEAL_MANA, STEAL_CARD_FROM_DECK
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

        public bool effectApplied { private set; get; }

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

        public void SetEffectApplied()
        {
            effectApplied = true;
        }

        #region Apply

        public void Apply(Card source, object target)
        {
            effectApplied = false;
            CardEffectsManager.Instance.SetCurrentEffect(this);

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
        }

        private void ApplyDefensiveEffect(Card source, object target)
        {
            switch (subType)
            {
                case SubType.NONE:
                    break;

                case SubType.RESTORE_LIFE:
                    CardEffectsManager.Instance.RestoreLife(intParameter1, source.contender); break;

                case SubType.INCREASE_MAX_MANA:
                    CardEffectsManager.Instance.IncreaseMaxMana(intParameter1, source.contender); break;

                case SubType.STEAL_REWARD:
                    CardEffectsManager.Instance.RestoreLife(source.contender.stolenCards * 5, source.contender); break;
            }
        }

        private void ApplyOffensiveEffect(Card source, object target)
        {
            switch (subType)
            {
                case SubType.NONE:
                    break;
                case SubType.DESTROY_CARD:
                    CardEffectsManager.Instance.DestroyCard(source, target, targetType); break;

                case SubType.DEAL_DAMAGE:
                    CardEffectsManager.Instance.DealDamage(source, target, targetType, intParameter1); break;

                case SubType.DECREASE_MANA:
                    CardEffectsManager.Instance.DecreaseMana(source, intParameter1); break;
            }
        }

        private void ApplyBoostEffect(Card source, object target)
        {
            switch (subType)
            {
                case SubType.NONE:
                    break;
                case SubType.LIFELINK:
                    CardEffectsManager.Instance.Lifelink(source, target); break;

                case SubType.REBOUND:
                    CardEffectsManager.Instance.Rebound(source, target); break;

                case SubType.TRAMPLE:
                    CardEffectsManager.Instance.Trample(source, target); break;

                case SubType.SPONGE:
                    CardEffectsManager.Instance.Sponge(source, target); break;

                case SubType.GUARD:
                    TurnManager.Instance.SetGuardCard(source); break;

                case SubType.COMPARTMENTALIZE:
                    CardEffectsManager.Instance.Compartmentalize(source, target); break;

                case SubType.STAT_BOOST:
                    CardEffectsManager.Instance.StatBoost(source, target, targetType, intParameter1, intParameter2); break;

                case SubType.STAT_DECREASE:
                    CardEffectsManager.Instance.StatDecrease(source, target, targetType, intParameter1, intParameter2); break;

                case SubType.ADD_EFFECT:
                    CardEffect effect = new CardEffect()
                    {
                        type = EffectType.BOOST,
                        subType = cardParameter_Effect,
                        applyTime = ApplyTime.COMBAT
                    };

                    CardEffectsManager.Instance.AddEffect(source, target, targetType, effect); break;
            }
        }

        private void ApplyTacticalEffect(Card source, object target)
        {
            switch (subType)
            {
                case SubType.NONE:
                    break;
                case SubType.CREATE_CARD:
                    CardEffectsManager.Instance.CreateCard(source, target, GetDataFromParameters()); break;

                case SubType.DUPLICATE_CARD:
                    CardsData data = ((Card)target).data;
                    CardEffectsManager.Instance.CreateCard(source, target, data); break;

                case SubType.SWAP_POSITION:
                    CardEffectsManager.Instance.SwapPosition(target, targetType); break;

                case SubType.SWAP_CONTENDER:
                    CardEffectsManager.Instance.SwapContender(source, targetType); break;

                case SubType.DRAW_CARD:
                    CardEffectsManager.Instance.DrawCard(intParameter1, source.contender); break;

                case SubType.DISCARD_CARD:
                    CardEffectsManager.Instance.DiscardCard(intParameter1, CardGameManager.Instance.GetOtherContender(source.contender)); break;

                case SubType.RETURN_CARD:
                    CardEffectsManager.Instance.ReturnCard(target); break;

                case SubType.FREE_MANA:
                    CardEffectsManager.Instance.FreeMana(source.contender); break;

                case SubType.WHEEL:
                    CardEffectsManager.Instance.Wheel(); break;

                case SubType.SKIP_COMBAT:
                    CardEffectsManager.Instance.SkipCombat(); break;

                case SubType.MIRROR:
                    CardEffectsManager.Instance.Mirror(source.contender); break;

                case SubType.STEAL_MANA:
                    CardEffectsManager.Instance.StealMana(source.contender); break;

                case SubType.ADD_CARD_TO_DECK:
                    CardEffectsManager.Instance.AddCardToDeck(source, GetDataFromParameters()); break;

                case SubType.DISCARD_CARD_FROM_DECK:
                    CardEffectsManager.Instance.DiscardCardFromDeck(target, intParameter1, CardGameManager.Instance.GetOtherContender(source.contender)); break;

                case SubType.STEAL_CARD:
                    CardEffectsManager.Instance.StealCard(source, target); break;

                case SubType.STEAL_CARD_FROM_HAND:
                    CardEffectsManager.Instance.StealCardFromHand(source, intParameter1); break;

                case SubType.STEAL_CARD_FROM_DECK:
                    CardEffectsManager.Instance.StealCardFromDeck(source, intParameter1, CardGameManager.Instance.GetOtherContender(source.contender)); break;
            }
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


        private void ApplyAlternateWinCondition(Card source, object target)
        {
            // win interview
            TurnManager.Instance.AlternateWinCondition();
        }

        #endregion

        #region IsAppliable

        public bool IsAppliable(Card source)
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
                Contender contender = source.contender;
                Contender otherContender = CardGameManager.Instance.GetOtherContender(contender);

                bool contenderHasCards = Board.Instance.AreCardsOnTable(contender);
                bool otherContenderHasCards = Board.Instance.AreCardsOnTable(otherContender);

                switch (targetType)
                {
                    case Target.ALLY:
                    case Target.AALLY:
                        CardZone emptyCardZone = Board.Instance.GetEmptyCardZone(contender);
                        if (subType == SubType.DUPLICATE_CARD)
                            return emptyCardZone != null && contenderHasCards;
                        else if (subType == SubType.CREATE_CARD)
                            return emptyCardZone;
                        else
                            return contenderHasCards;

                    case Target.ENEMY:
                    case Target.AENEMY:
                        if (subType == SubType.SWAP_POSITION)
                            return otherContenderHasCards;
                        else
                            return otherContenderHasCards || otherContender.fieldCardZone.isNotEmpty;

                    case Target.CARD:
                    case Target.ACARD:
                        return contenderHasCards || otherContenderHasCards
                            || contender.fieldCardZone.isNotEmpty
                            || otherContender.fieldCardZone.isNotEmpty;

                    case Target.FIELDCARD:
                        return otherContender.fieldCardZone.isNotEmpty;

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
                case SubType.STAT_DECREASE:
                    return "Reducción de parámetros (" + intParameter1 + "/" + intParameter2 + ")";
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
                case SubType.STEAL_CARD_FROM_DECK:
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
                    case ApplyTime.DESTROY:
                        s += "Al destruirse, ";
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
                        case Target.SELF: s += "inflige " + intParameter1 + ((intParameter1 > 1) ? " puntos" : " punto") + " de daño al jugador."; break;
                        case Target.ALL: s += "inflige " + intParameter1 + ((intParameter1 > 1) ? " puntos" : " punto") + " de daño a cartas y jugadores."; break;
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
                case SubType.STAT_DECREASE:
                    switch (targetType)
                    {
                        case Target.SELF: s += "reduce los parámetros en " + intParameter1 + " / " + intParameter2 + "."; break;
                        case Target.ALLY: s += "el objetivo reduce sus parámetros en " + intParameter1 + " / " + intParameter2 + "."; break;
                        case Target.AALLY: s += "todos los argumentos aliados reducen sus parámetros en " + intParameter1 + "/" + intParameter2 + "."; break;
                        case Target.AENEMY: s += "todos los argumentos del oponente reducen sus parámetros en " + intParameter1 + "/" + intParameter2 + "."; break;
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
                case SubType.STEAL_CARD_FROM_DECK:
                    s += "selecciona " + intParameter1 + ((intParameter1 > 1) ? " cartas" : " carta") + " de la baraja del oponente y "
                        + ((intParameter1 > 1) ? "añádelas" : "añádela") + " tu mano."; break;

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