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
        FREE_MANA, WHEEL, COMPARTMENTALIZE, SKIP_COMBAT, MONDARORIANO_WIN_CONDITION
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
        NONE, DUPLICATE_CARD, CREATE_CARD, SWAP_POSITION, SWAP_CONTENDER, DRAW_CARD, DISCARD_CARD, RETURN_CARD, FREE_MANA, WHEEL, COMPARTMENTALIZE, SKIP_COMBAT
    }

    public enum AlternateWinConditionType
    {
        NONE, MONDARORIANO_WIN_CONDITION
    }

    public enum ApplyTime
    {
        NONE, /*START,*/ ENTER, COMBAT, END, DRAW_CARD, PLAY_ARGUMENT
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
        NONE, ALLY, ENEMY, CARD, AALLY, AENEMY, ACARD, /*CARDZONE,*/ PLAYER, SELF, FIELDCARD
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
        //public CardsDataNoSer cardParameter;

        public string cardParameter_Name;
        public Sprite cardParameter_Sprite;
        public int cardParameter_Strength;
        public int cardParameter_Defense;
        public SubType cardParameter_Effect;

        #region Apply

        public void Apply(Card source, object target)
        {
            //Debug.Log(type + " " + subType + " effect applied");

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
                    switch (targetType)
                    {
                        case Target.ENEMY: ((Card)target).Destroy(); break;
                        case Target.AENEMY: Board.Instance.DestroyCards(TurnManager.Instance.otherPlayer); break;
                        case Target.ACARD: Board.Instance.DestroyAll(); break;
                        case Target.FIELDCARD: Board.Instance.GetFieldCardZone(TurnManager.Instance.otherPlayer).GetCard().Destroy(); break;
                    }
                    break;
                case SubType.DEAL_DAMAGE:
                    switch (targetType)
                    {
                        case Target.ENEMY: ((Card)target).ReceiveDamage(intParameter1); ((Card)target).CheckDestroy(); break;
                        //case Target.AENEMY: Board.Instance.DestroyCards(TurnManager.Instance.otherPlayer); break;
                        //case Target.ACARD: Board.Instance.DestroyAll(); break;
                        case Target.PLAYER: TurnManager.Instance.otherPlayer.ReceiveDamage(intParameter1); break;
                        case Target.SELF: TurnManager.Instance.currentPlayer.ReceiveDamage(intParameter1); break;
                    }
                    break;
                case SubType.DECREASE_MANA:
                    Contender otherPlayer = TurnManager.Instance.otherPlayer;
                    int manaValue = intParameter1;
                    if (intParameter1 == -1)
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
                            int lifeValue = Mathf.Min(source.defense, targetCard.strength);
                            targetCard.ReceiveDamage(lifeValue);
                        }
                    }
                    break;
                case SubType.TRAMPLE:
                    {
                        if (target is Card)
                        {
                            Card targetCard = (Card)target;
                            int lifeValue = source.strength - targetCard.defense;
                            TurnManager.Instance.otherPlayer.ReceiveDamage(lifeValue);
                        }
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
                            foreach (Card card in Board.Instance.CardsOnTable(source.contender))
                            {
                                card.BoostStats(intParameter1, intParameter2);
                            }
                            break;
                        case Target.AENEMY:
                            foreach (Card card in Board.Instance.CardsOnTable(TurnManager.Instance.GetOtherContender(source.contender)))
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
                    InstantiateCard(source, new CardsData
                    {
                        name = cardParameter_Name,
                        sprite = cardParameter_Sprite,
                        strength = cardParameter_Strength,
                        defense = cardParameter_Defense
                    });
                    break;

                case SubType.DUPLICATE_CARD:
                    InstantiateCard(source, ((Card)target).GetData());
                    break;

                case SubType.SWAP_POSITION:
                    SwapPosition((Card)target);
                    break;

                case SubType.SWAP_CONTENDER:
                    if (targetType == Target.SELF) SwapContender(source);
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
                case SubType.FREE_MANA:
                    source.contender.SetFreeMana(true);
                    break;

                case SubType.WHEEL:
                    Contender player = TurnManager.Instance.player;
                    Contender opponent = TurnManager.Instance.opponent;

                    int playerNumCards = player.hand.numCards;
                    int opponentNumCards = opponent.hand.numCards;

                    Sequence sequence = DOTween.Sequence();

                    sequence.AppendCallback(() =>
                    {
                        Board.Instance.DiscardAll(player);
                        Board.Instance.DiscardAll(opponent);
                    });
                    sequence.AppendCallback(() =>
                    {
                        Board.Instance.DrawCards(playerNumCards, TurnManager.Turn.PLAYER);
                        Board.Instance.DrawCards(opponentNumCards, TurnManager.Turn.OPPONENT);
                    });

                    sequence.Play();

                    break;

                case SubType.COMPARTMENTALIZE:

                    if (target is Contender)
                    {
                        Board.Instance.GetDeck((Contender)target).DiscardCards(source.strength);
                    }
                    break;

                case SubType.SKIP_COMBAT:
                    TurnManager.Instance.SkipCombat();
                    break;
            }
        }

        private void InstantiateCard(Card source, CardsData data)
        {
            if (cardParameter_Effect != SubType.NONE)
            {
                data.AddEffect(new CardEffect()
                {
                    type = EffectType.BOOST, // TODO find type from subtype
                    subType = cardParameter_Effect
                });
            }

            Contender owner = source.contender;
            Board board = Board.Instance;

            CardZone emptyCardZone = board.GetEmptyCardZone(owner);

            if (emptyCardZone != null)
            {
                GameObject cardPrefab = board.GetDeck(owner).cardPrefab;
                GameObject card = UnityEngine.Object.Instantiate(cardPrefab, source.transform.position, cardPrefab.transform.rotation);
                CardsData newData = new CardsData(data);
                card.GetComponent<Card>().Initialize(board.GetHand(owner), newData, cardRevealed: true);
                emptyCardZone.AddCard(card.GetComponent<Card>());
            }
        }

        private void SwapPosition(Card target)
        {
            if (targetType == Target.CARD)
            {
                List<CardZone> cardZones = Board.Instance.GetCardZone(target.contender);
                int pos = -1;

                // Get card position
                foreach (CardZone cardZone in cardZones)
                {
                    Card card = cardZone.GetCard();
                    if (card != null && card == target)
                    {
                        pos = cardZones.IndexOf(cardZone);
                        break;
                    }
                }

                // Get destination
                List<int> possibleCardZones = new List<int>();
                if (pos > 0) possibleCardZones.Add(pos - 1);
                if (pos < 3) possibleCardZones.Add(pos + 1);

                int dest = possibleCardZones[UnityEngine.Random.Range(0, possibleCardZones.Count)];

                Swap(cardZones, pos, dest);
            }
            else if (targetType == Target.AENEMY)
            {
                List<CardZone> cardZones = Board.Instance.GetCardZone(TurnManager.Instance.otherPlayer);
                int pos = -1;

                // Find any card
                foreach (CardZone cardZone in cardZones)
                {
                    Card card = cardZone.GetCard();
                    if (card != null)
                    {
                        pos = cardZones.IndexOf(cardZone);
                        break;
                    }
                }

                List<int> possibleCardZones = new List<int> { 0, 1, 2, 3 };
                possibleCardZones.Remove(pos);
                int dest = possibleCardZones[UnityEngine.Random.Range(0, possibleCardZones.Count)];

                Swap(cardZones, pos, dest);
            }
        }

        private void SwapContender(Card target)
        {
            List<CardZone> cardZones = Board.Instance.GetCardZone(target.contender);
            CardZone originCardZone = null;

            // Get card position
            foreach (CardZone cardZone in cardZones)
            {
                Card card = cardZone.GetCard();
                if (card != null && card == target)
                {
                    originCardZone = cardZone;
                    break;
                }
            }

            // Get destination
            CardZone emptyCardZone = Board.Instance.GetEmptyCardZone(TurnManager.Instance.otherPlayer);
            if (emptyCardZone != null)
            {
                Card originCard = originCardZone.GetCard();
                originCardZone.RemoveCard(originCard.gameObject);

                originCard.SwapContender();

                emptyCardZone.AddCard(originCard);
            }
        }

        private void Swap(List<CardZone> cardZones, int pos1, int pos2)
        {
            CardZone originCardZone = cardZones[pos1];
            CardZone destCardZone = cardZones[pos2];

            Card originCard = originCardZone.GetCard();
            originCardZone.RemoveCard(originCard.gameObject);
            if (destCardZone.GetCard() != null)
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
                    case SubType.MONDARORIANO_WIN_CONDITION:
                        return DeckManager.Instance.GetOpponentName() == Opponent_Name.Mondaroriano 
                            && TurnManager.Instance.player.eloquence >= 30;
                    default:
                        return false;
                }
            }
            else
            {

                bool currentPlayerHasCards = Board.Instance.AreCardsOnTable(TurnManager.Instance.currentPlayer);
                bool otherPlayerHasCards = Board.Instance.AreCardsOnTable(TurnManager.Instance.otherPlayer);

                switch (targetType)
                {
                    case Target.ALLY:
                    case Target.AALLY:
                        CardZone emptyCardZone = Board.Instance.GetEmptyCardZone(TurnManager.Instance.currentPlayer);
                        if (subType == SubType.DUPLICATE_CARD)
                            return emptyCardZone != null && currentPlayerHasCards;
                        else if (subType == SubType.CREATE_CARD)
                            return emptyCardZone;
                        else
                            return currentPlayerHasCards;

                    case Target.ENEMY:
                    case Target.AENEMY:
                        return otherPlayerHasCards;

                    case Target.CARD:
                    case Target.ACARD:
                        return currentPlayerHasCards || otherPlayerHasCards;

                    case Target.FIELDCARD:
                        return Board.Instance.GetFieldCardZone(TurnManager.Instance.otherPlayer).GetCard() != null;

                    case Target.NONE:
                    case Target.PLAYER:
                    case Target.SELF:
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
                //case Target.CARDZONE:
                case Target.PLAYER:
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
                    return "Recuperar vida (" + intParameter1 + ")";
                case SubType.INCREASE_MAX_MANA:
                    return "Incrementar man� m�ximo (" + intParameter1 + ")";

                case SubType.DESTROY_CARD:
                    return "Destruir carta";
                case SubType.DEAL_DAMAGE:
                    return "Infligir da�o (" + intParameter1 + ")";
                case SubType.DECREASE_MANA:
                    return "Reducir man� (" + intParameter1 + ")";

                case SubType.LIFELINK:
                    return "V�nculo vital";
                case SubType.REBOUND:
                    return "Rebote";
                case SubType.TRAMPLE:
                    return "Arrollar";
                case SubType.STAT_BOOST:
                    return "Bonificador de par�metros (" + intParameter1 + "/" + intParameter2 + ")";
                case SubType.ADD_EFFECT:
                    break;
                case SubType.GUARD:
                    break;

                case SubType.CREATE_CARD:
                    return "Invocar carta (" + /*cardParameter.name +*/ ")";
                case SubType.SWAP_POSITION:
                    return "Cambio de posici�n";
                case SubType.SWAP_CONTENDER:
                    return "Cambio de jugador";
                case SubType.DRAW_CARD:
                    return "Robar cartas (" + intParameter1 + ")";
                case SubType.DISCARD_CARD:
                    return "El oponente descarta " + intParameter1 + " cartas.";
                case SubType.RETURN_CARD:
                    return "Devolver a la mano";
                case SubType.FREE_MANA:
                    return "Juega carta gratis";
                case SubType.WHEEL:
                    return "Rueda";
                case SubType.COMPARTMENTALIZE:
                    return "Compartimentar";
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
                }
            }
            switch (subType)
            {
                case SubType.NONE:
                    return "";

                case SubType.RESTORE_LIFE:
                    s += "el jugador recupera " + intParameter1 + ((intParameter1 > 1) ? " puntos" : " punto") + " de vida."; break;
                case SubType.INCREASE_MAX_MANA:
                    s += "consigues " + intParameter1 + " cristal extra de man�."; break;

                case SubType.DESTROY_CARD:
                    switch (targetType)
                    {
                        case Target.ENEMY: s += "destruye la carta objetivo."; break;
                        case Target.AENEMY: s += "destruye todas las cartas del oponente."; break;
                        case Target.ACARD: s += "destruye todas las cartas sobre el campo."; break;
                    }
                    break;
                case SubType.DEAL_DAMAGE:
                    switch (targetType)
                    {
                        case Target.ENEMY: s += "inflige " + intParameter1 + ((intParameter1 > 1) ? " puntos" : " punto") + " de da�o a la carta objetivo."; break;
                        case Target.AENEMY: s += "inflige " + intParameter1 + ((intParameter1 > 1) ? " puntos" : " punto") + " de da�o a todas las cartas del oponente."; break;
                        case Target.PLAYER: s += "inflige " + intParameter1 + ((intParameter1 > 1) ? " puntos" : " punto") + " de da�o al oponente."; break;
                    }
                    break;
                case SubType.DECREASE_MANA:
                    s += "reduce el man� del oponente en " + intParameter1 + " puntos."; break;

                case SubType.LIFELINK:
                    s += "el jugador recupera el da�o infligido por la carta."; break;
                case SubType.REBOUND:
                    s += "el da�o recibido es infligido al origen de ese da�o."; break;
                case SubType.TRAMPLE:
                    s += "si la fuerza de la carta es mayor que la defensa de la carta rival, el da�o restante es infligido al oponente."; break;
                case SubType.STAT_BOOST:
                    switch (targetType)
                    {
                        case Target.SELF: s += "obtiene un bonificador de " + intParameter1 + " / " + intParameter2 + " al final del turno."; break;
                        case Target.ALLY: s += "el objetivo recibe un bonificador de " + intParameter1 + " / " + intParameter2 + "."; break;
                        case Target.AALLY: s += "todas las cartas aliadas obtienen un bonificador de " + intParameter1 + "/" + intParameter2 + "."; break;
                    }
                    break;
                case SubType.ADD_EFFECT:
                    break;
                case SubType.GUARD:
                    break;

                case SubType.CREATE_CARD:
                    s += "invoca la carta " + cardParameter_Name + " en una zona libre."; break;
                case SubType.SWAP_POSITION:
                    s += "mueve la carta objetivo a la zona objetivo. Si ya hay una carta, se intercambian."; break;
                case SubType.SWAP_CONTENDER:
                    s += "la carta pasa al control del oponente."; break;
                case SubType.DRAW_CARD:
                    s += "roba " + intParameter1 + ((intParameter1 > 1) ? " cartas." : " carta."); break;
                case SubType.DISCARD_CARD:
                    s += "el oponente descarta " + intParameter1 + ((intParameter1 > 1) ? " cartas." : " carta."); break;
                case SubType.RETURN_CARD:
                    s += "devuelve la carta objetivo a la mano de su propietario."; break;
                case SubType.FREE_MANA:
                    s += "la pr�xima carta que juegues tendr� coste 0."; break;
                case SubType.WHEEL:
                    s += "los jugadores descartan las cartas de su mano y roban la misma cantidad."; break;
                case SubType.COMPARTMENTALIZE:
                    s += "el oponente descarta el n�mero de cartas de la baraja equivalente al da�o que fuera a recibir."; break;


                case SubType.MONDARORIANO_WIN_CONDITION:
                    s += "si tienes 30 o m�s vidas, ganas la partida."; break;
            }

            if(s.Length > 0) s = s[0].ToString().ToUpper() + s.Substring(1, s.Length - 1);

            return s;
        }
    }

}