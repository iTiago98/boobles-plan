using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CardGame.Managers;
using CardGame.Level;
using CardGame.Cards.DataModel;
using TMPro;
using CardGame.Cards.DataModel.Effects;
using System.Collections;
using System;

namespace CardGame.Cards
{
    public class Card : MonoBehaviour, IClickable
    {
        public CardsData data { set; get; }
        public CardUI CardUI { private set; get; }
        public CardStats Stats { private set; get; }
        public CardEffects Effects { private set; get; }

        public Contender contender { private set; get; }

        public bool IsInHand => _container == _hand;
        public bool IsPlayerCard => contender.isPlayer;

        public bool IsArgument => Stats.type == CardType.ARGUMENT;
        public bool IsAction => Stats.type == CardType.ACTION;
        public bool IsField => Stats.type == CardType.FIELD;

        public CardEffect effect => Effects.firstEffect;
        public bool HasEffect => Effects.effectsList.Count > 0;

        private bool _clickable;

        [HideInInspector] public Card storedTarget;
        private bool _swapped;

        private CardContainer _container;
        private Hand _hand;

        private void Start()
        {
            _clickable = true;
        }

        #region Initialize

        public void Initialize(Contender contender, CardsData data, bool cardRevealed)
        {
            this.contender = contender;
            this._hand = contender.hand;
            this.data = new CardsData(data); ;

            name = data.name;

            Stats = GetComponent<CardStats>();
            Effects = GetComponent<CardEffects>();
            CardUI = GetComponent<CardUI>();

            Stats.Initialize(this);
            Effects.Initialize(this);
            CardUI.Initialize(this, cardRevealed);
        }

        #endregion

        #region UI

        public void UpdateStatsUI()
        {
            if (IsArgument) CardUI.UpdateStatsUI();
        }

        public void ShowExtendedDescription()
        {
            CardUI.ShowExtendedDescription();
        }

        public void HideExtendedDescription()
        {
            CardUI.HideExtendedDescription();
        }

        public void ShowHighlight(bool show)
        {
            CardUI.ShowHighlight(show);
        }

        #endregion

        #region IClickable methods

        public void OnMouseLeftClickUp(MouseController mouseController)
        {
            if (!_clickable) return;

            if (IsInHand)
            {
                if (_hand.isDiscarding)
                {
                    int numCards = _hand.numCards - 1;
                    DestroyCard();
                    _hand.CheckDiscarding(numCards);
                }
                else if (TurnManager.Instance.isPlayerTurn && IsPlayerCard && !mouseController.IsHoldingCard)
                {
                    if (IsAction && !effect.IsAppliable()) return;

                    if (EnoughMana())
                    {
                        // Deattach from parent
                        RemoveFromContainer();
                        MoveToWaitingSpot();

                        if (IsArgument || IsField)
                        {
                            Board.Instance.HighlightCardZones(this, show: true);
                        }
                        else
                        {
                            PlayAction();
                        }

                        UIManager.Instance.ShowCancelPlayButton();
                    }
                }
            }
        }

        private bool EnoughMana()
        {
            return contender.freeMana || Stats.manaCost <= contender.currentMana;
        }

        public void OnMouseHoverEnter()
        {
            if (!_clickable || contender == null) return;

            if (IsInHand && IsPlayerCard)
            {
                CardUI.HoverOn();
            }

            ShowExtendedDescription();
        }

        public void OnMouseHoverExit()
        {
            if (!_clickable || contender == null) return;

            if (this != null && gameObject != null && IsInHand && IsPlayerCard)
            {
                CardUI.HoverOff(_hand);
            }

            HideExtendedDescription();
        }

        #endregion

        #region Play

        public void Play(CardZone cardZone)
        {
            if (!IsPlayerCard) CardUI.FlipCard();

            PlayCard(cardZone);
            CardGameManager.Instance.CheckDialogue(this);
        }

        private void PlayCard(CardZone cardZone)
        {
            switch (Stats.type)
            {
                case CardType.ARGUMENT:
                    Stats.SubstractMana();
                    AddToContainer(cardZone);
                    Effects.CheckEffect();
                    TurnManager.Instance.ApplyPlayArgumentEffects();
                    break;

                case CardType.FIELD:
                    Stats.SubstractMana();
                    AddToContainer(cardZone);
                    Effects.CheckEffect();
                    break;

                case CardType.ACTION:
                    PlayAction();
                    break;
            }
        }

        private void PlayAction()
        {
            if (IsPlayerCard)
            {
                if (effect.HasTarget())
                {
                    List<Card> possibleTargets = effect.FindPossibleTargets();

                    Board.Instance.HighlightTargets(possibleTargets);

                    MouseController.Instance.SetApplyingEffect(this);
                }
                else
                {
                    Board.Instance.HighlightMultipleTargets(contender, effect.subType, effect.targetType);
                    UIManager.Instance.ShowContinuePlayButton();
                }
            }
            else
            {
                if (effect.HasTarget())
                {
                    List<Card> possibleTargets = effect.FindPossibleTargets();

                    if (possibleTargets.Count > 0)
                    {
                        Card bestTarget = CardGameManager.Instance.opponentAI.GetBestTarget(effect, possibleTargets);

                        Board.Instance.HighlightTargets(new List<Card>() { bestTarget });
                        storedTarget = bestTarget;
                    }
                }
                else
                {
                    Board.Instance.HighlightMultipleTargets(contender, effect.subType, effect.targetType);
                }

                MoveToWaitingSpot();
                CardGameManager.Instance.opponentAI.enabled = false;
                UIManager.Instance.ShowContinuePlayButton();
            }
        }

        private void MoveToWaitingSpot()
        {
            Transform dest = Board.Instance.waitingSpot;
            MouseController.Instance.SetHolding(this);
            UIManager.Instance.SetEndTurnButtonInteractable(false);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DOMove(dest.position, 0.5f));
            sequence.Join(transform.DOScale(CardUI.defaultScale, 0.5f));

            sequence.Play();
        }

        //public void ContinuePlay()
        //{
        //    StartCoroutine(ContinuePlayCoroutine());
        //}

        //private IEnumerator ContinuePlayCoroutine()
        //{
        //    TurnManager.Instance.StopFlow();

        //    Stats.SubstractMana();
        //    Effects.ApplyEffect();

        //    yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

        //    DestroyCard();

        //    yield return new WaitUntil(() => gameObject == null);

        //    MouseController.Instance.SetHolding(null);
        //    UIManager.Instance.SetEndTurnButtonInteractable(true);
        //}

        //public void ContinuePlayOpponent()
        //{
        //    Stats.SubstractMana();

        //    Effects.ApplyEffect(effect, storedTarget);
        //    DestroyCard();

        //    storedTarget = null;
        //    CardGameManager.Instance.opponentAI.enabled = true;
        //    MouseController.Instance.SetHolding(null);
        //}

        //public void CancelPlay()
        //{
        //    // Return card to hand
        //}

        #endregion

        #region Clash

        public void Hit(object target)
        {
            if (target is Card)
            {
                Card card = (Card)target;
                card.ReceiveDamage(Stats.strength);
            }
            else
            {
                Contender targetContender = (Contender)target;

                if (Effects.HasEffect(SubType.COMPARTMENTALIZE) && targetContender.deck.numCards > 0) return;

                if (TurnManager.Instance.IsMirror(targetContender))
                    contender.ReceiveDamage(Stats.strength);
                else
                    targetContender.ReceiveDamage(Stats.strength);
            }
        }

        public Sequence HitSequence(object target)
        {
            Sequence hitSequence = DOTween.Sequence();

            if (Stats.strength != 0)
            {
                bool isCard = target is Card;
                Vector3 targetPosition = (isCard) ? ((Card)target).transform.position : ((Contender)target).transform.position;

                float approachFactor = isCard ? 0.2f : 0.8f;
                Vector3 targetDir = (targetPosition - transform.position) * approachFactor;
                targetDir = new Vector3(targetDir.x, targetDir.y, 0);

                Vector3 previousRotation = transform.rotation.eulerAngles;
                Quaternion rotation;
                if (IsPlayerCard)
                    rotation = Quaternion.FromToRotation(transform.up, targetDir);
                else
                    rotation = Quaternion.FromToRotation(transform.up * -1, targetDir);

                Vector3 rotationEuler = rotation.eulerAngles;
                if (rotationEuler.z > 180) rotationEuler = new Vector3(rotationEuler.x, rotationEuler.y, rotationEuler.z - 360);

                // Enlarge and rotate
                hitSequence.Append(transform.DOScale(CardUI.hitScale, 0.5f));
                if (IsPlayerCard)
                    hitSequence.Join(transform.DOLocalMoveZ(-0.1f, 0.5f));
                else
                    hitSequence.Join(transform.DOLocalMoveZ(-0.05f, 0.5f));
                hitSequence.Join(transform.DORotate(rotationEuler, 0.2f).SetRelative());

                // Hit
                hitSequence.Append(transform.DOMove(targetDir, 0.2f).SetRelative());

                // Hit callback
                hitSequence.AppendCallback(() => TurnManager.Instance.ApplyCombatActions(this, target));

                // Back
                hitSequence.Append(transform.DOLocalMove(Vector3.zero, 0.2f));
                hitSequence.Append(transform.DOScale(CardUI.defaultScale, 0.2f));
                hitSequence.Join(transform.DORotate(previousRotation, 0.2f));
            }
            else
            {
                hitSequence.AppendInterval(0.7f);
                // Hit callback
                hitSequence.AppendCallback(() => TurnManager.Instance.ApplyCombatActions(this, target));
            }

            return hitSequence;
        }

        public void ReceiveDamage(int strength)
        {
            StartCoroutine(ReceiveDamageCoroutine(strength));
        }

        private IEnumerator ReceiveDamageCoroutine(int strength)
        {
            bool combat = TurnManager.Instance.combat;

            float length = CardUI.ShowDamagedAnimation();
            Stats.ReceiveDamage(strength);
            UpdateStatsUI();

            yield return new WaitForSeconds(length);
            CardUI.SetPlayingAnimation(false);
            if (!combat) CheckDestroy();
        }

        #endregion

        #region Destroy

        public void DestroyCard()
        {
            DestroyCard(instant: false);
        }

        public void DestroyCard(bool instant)
        {
            //Play destroy animation
            Debug.Log(name + " destroyed");
            StartCoroutine(DestroyCardCoroutine(instant));
        }

        private IEnumerator DestroyCardCoroutine(bool instant)
        {
            _clickable = false;
            HideExtendedDescription();

            Effects.CheckDelegateEffects();
            if (!instant) Effects.CheckDestroyEffects();

            if (!IsInHand) RemoveFromContainer();

            Sequence destroySequence = DOTween.Sequence();

            if (instant) destroySequence.AppendCallback(() => gameObject.SetActive(false));

            destroySequence.Append(transform.DOScale(0, 1));
            destroySequence.AppendCallback(() =>
            {
                if (IsInHand) RemoveFromContainer();
                Destroy(gameObject);
            });

            destroySequence.Play();

            yield return new WaitUntil(() => this == null);
        }

        public bool CheckDestroy()
        {
            bool destroy = Stats.defense <= 0;
            if (destroy) DestroyCard();
            return destroy;
        }

        #endregion

        #region Effects

        public void BoostStats(int strengthBoost, int defenseBoost)
        {
            Stats.BoostStats(strengthBoost, defenseBoost);

            UpdateStatsUI();
        }

        public void DecreaseStats(int strengthDecrease, int defenseDecrease)
        {
            Stats.DecreaseStats(strengthDecrease, defenseDecrease);
            UpdateStatsUI();
            CheckDestroy();
        }

        public void SwapContender()
        {
            if (IsPlayerCard)
                contender = CardGameManager.Instance.opponent;
            else
                contender = CardGameManager.Instance.player;

            _hand = contender.hand;
            _swapped = !_swapped;
        }

        public bool IsAlternateWinConditionCard()
        {
            return IsAction && HasEffect && effect.type == EffectType.ALTERNATE_WIN_CONDITION;
        }

        #endregion

        #region Containers

        private void AddToContainer(CardZone cardZone)
        {
            cardZone.AddCard(this);
            transform.DOScale(CardUI.defaultScale, 0.2f);
        }

        public void SetContainer(CardContainer container)
        {
            this._container = container;
            if (_hand == null && container is Hand)
            {
                _hand = (Hand)container;
                if (contender == null) contender = _hand.contender;
            }
        }

        public void RemoveFromContainer()
        {
            if (_container != null)
            {
                _container.RemoveCard(gameObject);
                _container = null;
                transform.parent = null;
            }
        }

        public void ReturnToHand()
        {
            RemoveFromContainer();
            Effects.CheckDelegateEffects();
            if (_swapped) SwapContender();

            _hand.AddCard(this);
            CardUI.ReturnToHand();
            Stats.ReturnToHand();
        }

        #endregion

    }
}