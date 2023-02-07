using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using Booble.CardGame.Level;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Cards.DataModel.Effects;
using Booble.CardGame.Managers;
using System;

namespace Booble.CardGame.Cards
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

        public bool IsInWaitingSpot => MouseController.Instance.holdingCard == this;

        public bool IsArgument => Stats.type == CardType.ARGUMENT;
        public bool IsAction => Stats.type == CardType.ACTION;
        public bool IsField => Stats.type == CardType.FIELD;

        public CardEffect effect => Effects.firstEffect;
        public bool HasEffect => Effects.effectsList.Count > 0;

        private bool _clickable;

        [HideInInspector] public Card storedTarget;

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

        public void ShowHighlight(bool show = true)
        {
            CardUI.ShowHighlight(show);
        }

        #endregion

        #region IClickable methods

        public void OnMouseLeftClickUp(MouseController mouseController)
        {
            if (!_clickable) return;

            bool isTutorial = CardGameManager.Instance.tutorial;
            if (isTutorial && data.name != mouseController.GetTutorialCard()) return;

            if (IsInHand)
            {
                if (_hand.isDiscarding)
                {
                    if (CardUI.IsHighlighted) _hand.SubstractDiscarding(this);
                    else _hand.AddDiscarding(this);
                    return;
                }

                if (TurnManager.Instance.IsPlayerTurn || isTutorial)
                {
                    if (!IsPlayerCard) return;
                    if (IsAction && !effect.IsAppliable(this)) return;

                    if (EnoughMana())
                    {
                        if (mouseController.IsHoldingCard) UIManager.Instance.OnCancelPlayButtonClick();
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
            if (CardGameManager.Instance.tutorial && CardUI.HasHeartbeat) CardUI.SetHeartbeat(false);

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
                if (CardGameManager.Instance.tutorial && CardUI.HasHeartbeat)
                    CardUI.HoverOff(_hand, () => CardUI.SetHeartbeat(true));
                else
                    CardUI.HoverOff(_hand);
            }

            HideExtendedDescription();
        }

        #endregion

        #region Play

        public void Play(CardZone cardZone)
        {
            if (!IsPlayerCard) CardUI.FlipCard();

            CardGameManager.Instance.SetPlayingCard(true);
            PlayCard(cardZone);
        }

        private void PlayCard(CardZone cardZone)
        {
            switch (Stats.type)
            {
                case CardType.ARGUMENT:
                case CardType.FIELD:
                    PlayArgumentOrField(cardZone);
                    break;

                case CardType.ACTION:
                    PlayAction();
                    break;
            }
        }

        private void PlayArgumentOrField(CardZone cardZone)
        {
            StartCoroutine(PlayArgumentOrFieldCoroutine(cardZone));
        }

        private IEnumerator PlayArgumentOrFieldCoroutine(CardZone cardZone)
        {
            RemoveFromContainer();

            Stats.SubstractMana();
            Effects.CheckEffects();

            if (IsField && cardZone.isNotEmpty) cardZone.GetCard().DestroyCard();

            AddToContainer(cardZone);

            yield return new WaitUntil(() => cardZone.cardsAtPosition);

            if (Effects.hasEnterEffects)
            {
                Effects.ApplyEnterEffects();
                yield return new WaitWhile(() => Effects.applyingEffects);
            }

            if (IsArgument && CardEffectsManager.Instance.HasPlayArgumentEffects)
            {
                CardEffectsManager.Instance.ApplyPlayArgumentEffects();
                yield return new WaitUntil(() => CardEffectsManager.Instance.effectsApplied);
            }

            CardGameManager.Instance.SetPlayingCard(false);
            CardGameManager.Instance.CheckDialogue(this);
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
                UIManager.Instance.ShowContinuePlayButton();
            }
        }

        public void ContinueAction(Card target = null)
        {
            StartCoroutine(ContinueActionCoroutine(target));
        }

        private IEnumerator ContinueActionCoroutine(Card target)
        {
            UIManager.Instance.HidePlayButtons();
            Board.Instance.RemoveTargetsHighlight();

            if (target == null) target = storedTarget;

            Stats.SubstractMana();
            Effects.ApplyFirstEffect(target);

            yield return new WaitUntil(() => effect.effectApplied);

            bool dialogue = false;
            dialogue = CardGameManager.Instance.CheckDialogue(this);

            if (dialogue) yield return new WaitUntil(() => CardGameManager.Instance.dialogueEnd);

            DestroyCard(ContinueActionPost);

            yield return new WaitUntil(() => destroyed);
        }

        private void ContinueActionPost()
        {
            storedTarget = null;
            CardGameManager.Instance.SetPlayingCard(false);
            MouseController.Instance.ResetApplyingEffect();
        }

        public void MoveToWaitingSpot()
        {
            RemoveFromContainer();

            Transform dest = Board.Instance.waitingSpot;
            MouseController.Instance.SetMoving();
            UIManager.Instance.SetEndTurnButtonInteractable(false);

            DOTween.Kill(transform);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DOMove(dest.position, 0.5f));
            sequence.Join(transform.DOScale(CardUI.defaultScale, 0.5f));
            sequence.OnComplete(() => MouseController.Instance.SetHolding(this));

            sequence.Play();
        }

        #endregion

        #region Clash

        public void Hit(object target)
        {
            if (target is Card)
            {
                Card targetCard = (Card)target;
                targetCard.ReceiveDamage(Stats.strength);
            }
            else
            {
                Contender targetContender = (Contender)target;

                if (Effects.HasEffect(SubType.COMPARTMENTALIZE) && targetContender.deck.numCards > 0) return;

                if (targetContender.hasMirrorCards)
                    contender.ReceiveDamage(Stats.strength);
                else
                    targetContender.ReceiveDamage(Stats.strength);
            }
        }

        public Sequence HitSequence(object target, bool combatActionsApplied)
        {
            Sequence hitSequence = DOTween.Sequence();

            if (!combatActionsApplied) TurnManager.Instance.GetClashManager().SetCombatActionsApplied();

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
                if (!combatActionsApplied) hitSequence.AppendCallback(TurnManager.Instance.ApplyCombatActions);

                // Back
                hitSequence.Append(transform.DOLocalMove(Vector3.zero, 0.2f));
                hitSequence.Append(transform.DOScale(CardUI.defaultScale, 0.2f));
                hitSequence.Join(transform.DORotate(previousRotation, 0.2f));
            }
            else
            {
                hitSequence.AppendInterval(0.7f);
                // Hit callback
                if (!combatActionsApplied) hitSequence.AppendCallback(TurnManager.Instance.ApplyCombatActions);
            }

            return hitSequence;
        }

        public bool ReceiveDamage(int strength, bool checkDestroy = false)
        {
            if (strength > 0) StartCoroutine(ReceiveDamageCoroutine(strength, checkDestroy));
            return Stats.defense <= strength && checkDestroy;
        }

        private IEnumerator ReceiveDamageCoroutine(int strength, bool checkDestroy)
        {
            CardUI.ShowDamagedAnimation();
            Stats.ReceiveDamage(strength);
            UpdateStatsUI();

            yield return new WaitWhile(() => CardUI.IsPlayingAnimation);

            if (checkDestroy) CheckDestroy();
        }

        #endregion

        #region Destroy

        public bool destroyed { private set; get; }

        public void DestroyCard(Action onDestroy = null)
        {
            StartCoroutine(DestroyCardCoroutine(onDestroy));
        }

        private IEnumerator DestroyCardCoroutine(Action onDestroy)
        {
            _clickable = false;
            HideExtendedDescription();

            if (!IsInHand)
            {
                RemoveFromContainer();
                Effects.CheckRemoveEffects(contender);
                if (Effects.hasDestroyEffects) Effects.ApplyDestroyEffects();

                yield return new WaitWhile(() => Effects.applyingEffects);
            }

            Sequence destroySequence = DOTween.Sequence();

            destroySequence.Append(transform.DOScale(0, 1));
            destroySequence.AppendCallback(() =>
            {
                if (onDestroy != null) onDestroy();

                if (IsInHand) RemoveFromContainer();
                destroyed = true;
                Destroy(gameObject);
            });

            destroySequence.Play();
        }

        public bool CheckDestroy()
        {
            bool destroy = Stats.defense <= 0;
            if (destroy) DestroyCard();
            return destroy;
        }

        #endregion

        #region Effects

        public bool AddEffect(CardEffect effect)
        {
            if (Effects.AddEffect(effect))
            {
                CardUI.ShowBoostEffectAnimation();
                CardUI.UpdateDescriptionText();
                return true;
            }
            return false;
        }

        public void BoostStats(int strengthBoost, int defenseBoost)
        {
            CardUI.ShowBoostAnimation();
            Stats.BoostStats(strengthBoost, defenseBoost);

            UpdateStatsUI();
        }

        public void DecreaseStats(int strengthDecrease, int defenseDecrease)
        {
            StartCoroutine(DecreaseStatsCoroutine(strengthDecrease, defenseDecrease));
        }

        private IEnumerator DecreaseStatsCoroutine(int strengthDecrease, int defenseDecrease)
        {
            CardUI.ShowDecreaseAnimation();
            Stats.DecreaseStats(strengthDecrease, defenseDecrease);
            UpdateStatsUI();

            yield return new WaitWhile(() => CardUI.IsPlayingAnimation);

            if (!TurnManager.Instance.combat) CheckDestroy();
        }

        public void SwapContender()
        {
            Contender oldContender = contender;

            if (IsPlayerCard)
                contender = CardGameManager.Instance.opponent;
            else
                contender = CardGameManager.Instance.player;

            _hand = contender.hand;

            Effects.CheckPersistentEffects(oldContender, contender);
        }

        public bool IsAlternateWinConditionCard()
        {
            return IsAction && HasEffect && effect.type == EffectType.ALTERNATE_WIN_CONDITION;
        }

        #endregion

        #region Containers

        private void AddToContainer(CardZone cardZone)
        {
            cardZone.AddCard(gameObject);
            transform.DOScale(CardUI.defaultScale, 0.2f);
        }

        public void SetContainer(CardContainer container)
        {
            this._container = container;
            if (_container is Hand)
            {
                _hand = (Hand)container;
                contender = _hand.contender;
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
            Effects.CheckRemoveEffects(contender);

            _hand.AddCard(gameObject);
            Stats.ReturnToHand();
            CardUI.ReturnToHand();
        }

        #endregion

    }
}