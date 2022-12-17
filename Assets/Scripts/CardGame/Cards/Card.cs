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
        #region Card Data
        public int manaCost => data.cost;
        public int strength => data.strength;
        public int defense => data.defense;
        public int defaultStrength => data.defaultStrength;
        public int defaultDefense => data.defaultDefense;
        public CardType type => data.type;
        public List<CardEffect> effects { private set { data.effects = value; } get { return data.effects; } }
        public CardEffect effect => effects[0];
        public bool hasEffect => effects.Count > 0;

        public CardsData data { set; get; }

        #endregion

        #region UI

        [SerializeField] private TextMeshPro nameText;
        [SerializeField] private TextMeshPro descriptionText;
        [SerializeField] private TextMeshPro strengthText;
        [SerializeField] private TextMeshPro defenseText;

        [SerializeField] private Color altColor;
        [SerializeField] private GameObject highlight;

        private Sprite cardBack;
        private bool _cardFront = true;

        #endregion

        #region Hover

        private float _hoverPosY => CardGameManager.Instance.settings.hoverPosY;
        private float _hoverScale => CardGameManager.Instance.settings.hoverScale;

        #endregion

        private float _highlightScale => CardGameManager.Instance.settings.highlightScale;
        private float _defaultScale => CardGameManager.Instance.settings.defaultScale;

        #region Hit animation

        private float _hitScale => CardGameManager.Instance.settings.hitScale;

        #endregion

        [HideInInspector]
        public CardContainer container;
        [HideInInspector]
        public Contender contender;
        private Hand _hand;

        public bool IsInHand => container == _hand;
        public bool IsPlayerCard => contender.isPlayer;

        private bool _clickable;

        bool IClickable.clickable { get => _clickable; set => _clickable = value; }

        private SpriteRenderer _spriteRenderer;

        private Action _playArgumentEffect;
        private List<Action> _endTurnEffects;
        private Deck.DrawCardEffects _drawCardEffect;

        private Card _storedTarget;
        private bool _swapped;

        public bool isHighlighted { private set; get; }

        private void Start()
        {
            _clickable = true;
            _endTurnEffects = new List<Action>();
        }

        #region Initialize

        public void Initialize(Contender contender, CardsData data, bool cardRevealed)
        {
            this.contender = contender;
            this._hand = contender.hand;
            this.data = new CardsData(data); ;

            name = data.name;
            if (nameText != null) nameText.text = data.name;
            if (descriptionText != null) descriptionText.text = GetDescriptionText();

            cardBack = contender.GetCardBack();

            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (cardRevealed || IsPlayerCard)
            {
                if (data.sprite != null) _spriteRenderer.sprite = data.sprite;
            }
            else FlipCard();

            if (type == CardType.ARGUMENT)
            {
                UpdateStatsUI();
            }
        }

        #endregion

        #region UI

        private void UpdateStatsUI()
        {
            UpdateStatUI(strengthText, strength, defaultStrength, Color.black);
            UpdateStatUI(defenseText, defense, defaultDefense, Color.white);
        }

        private void UpdateStatUI(TextMeshPro text, int value, int defaultValue, Color defaultColor)
        {
            if (text != null)
            {
                text.text = value.ToString();

                if (value != defaultValue)
                {
                    text.color = altColor;
                }
                else
                {
                    text.color = defaultColor;
                }
            }
        }

        private string GetDescriptionText()
        {
            string temp = "";

            foreach (CardEffect effect in effects)
            {
                temp += effect.ToString() + "\n";
            }

            return temp;
        }

        public void ShowExtendedDescription()
        {
            UIManager.Instance.ShowExtendedDescription(NameToString(), TypeToString(), DescriptionToString());
        }

        public void HideExtendedDescription()
        {
            UIManager.Instance.HideExtendedDescription();
        }

        private string NameToString()
        {
            return name.ToUpper();
        }

        private string TypeToString()
        {
            return "TIPO: " + type.ToString();
        }

        private string DescriptionToString()
        {
            string s = "";
            foreach (CardEffect effect in effects)
            {
                s += effect.ToStringExtended(type) + "\n";
            }

            return s;
        }

        public void FlipCard()
        {
            if (_cardFront) _spriteRenderer.sprite = cardBack;
            else _spriteRenderer.sprite = data.sprite;

            _cardFront = !_cardFront;

            nameText.gameObject.SetActive(_cardFront);
            descriptionText.gameObject.SetActive(_cardFront);

            if (type == CardType.ARGUMENT)
            {
                strengthText.gameObject.SetActive(_cardFront);
                defenseText.gameObject.SetActive(_cardFront);
            }

            transform.Rotate(new Vector3(0, 0, 180));
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
                    DestroyCard(continueFlow: false);
                    _hand.CheckDiscarding(numCards);
                }
                else if (TurnManager.Instance.isPlayerTurn && IsPlayerCard && !mouseController.IsHoldingCard)
                {
                    if (type == CardType.ACTION && !effect.IsAppliable()) return;

                    if (EnoughMana())
                    {
                        // Deattach from parent
                        RemoveFromContainer();
                        MoveToWaitingSpot();

                        if (type == CardType.ARGUMENT || type == CardType.FIELD)
                        {
                            Board.Instance.HighlightZoneTargets(type, contender, show: true);
                            UIManager.Instance.SetEndTurnButtonInteractable(false);
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
            int mana = contender.currentMana;
            if (TurnManager.Instance.IsStealMana(contender))
                mana += CardGameManager.Instance.GetOtherContender(contender).currentMana;

            return contender.freeMana || manaCost <= mana;
        }

        public void OnMouseHoverEnter()
        {
            if (!_clickable || contender == null) return;

            if (IsInHand && IsPlayerCard)
            {
                transform.DOLocalMoveY(_hoverPosY, 0.2f);
                transform.DOScale(_hoverScale, 0.2f);
            }

            if (_cardFront) ShowExtendedDescription();
        }

        public void OnMouseHoverExit()
        {
            if (!_clickable || contender == null) return;

            if (this != null && gameObject != null && IsInHand && IsPlayerCard)
            {
                transform.DOLocalMoveY(0f, 0.2f);
                if (_hand.isDiscarding) transform.DOScale(_highlightScale, 0.2f);
                else transform.DOScale(_defaultScale, 0.2f);
            }

            HideExtendedDescription();
        }

        public void SetClickable(bool clickable)
        {
            this._clickable = clickable;
        }

        #endregion

        #region Play

        public void Play(CardZone cardZone)
        {
            if (!IsPlayerCard) FlipCard();

            PlayCard(cardZone);
            CardGameManager.Instance.CheckDialogue(this);
        }

        private void PlayCard(CardZone cardZone)
        {
            switch (type)
            {
                case CardType.ARGUMENT:
                    SubstractMana();
                    AddToContainer(cardZone);
                    CheckEffect();
                    TurnManager.Instance.ApplyPlayArgumentEffects();
                    break;

                case CardType.FIELD:
                    SubstractMana();
                    AddToContainer(cardZone);
                    CheckEffect();
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
                    UIManager.Instance.SetEndTurnButtonInteractable(false);
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
                        //int index = new System.Random().Next(0, possibleTargets.Count);
                        Card bestTarget = CardGameManager.Instance.opponentAI.GetBestTarget(effect, possibleTargets);

                        Board.Instance.HighlightTargets(new List<Card>() { bestTarget });
                        _storedTarget = bestTarget;
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

        private void CheckEffect()
        {
            if (hasEffect)
            {
                foreach (CardEffect effect in effects)
                {
                    if (effect.applyTime == ApplyTime.ENTER)
                    {
                        ApplyEffect(effect);
                    }
                    else if (effect.applyTime == ApplyTime.END)
                    {
                        Action action = new Action(ApplyEndTurnEffect);
                        _endTurnEffects.Add(action);
                        TurnManager.Instance.AddEndTurnEffect(action, data.name);
                    }
                    else if (effect.applyTime == ApplyTime.DRAW_CARD)
                    {
                        _drawCardEffect = new Deck.DrawCardEffects(ApplyEffect);
                        contender.deck.AddDrawCardEffects(_drawCardEffect);
                    }
                    else if (effect.applyTime == ApplyTime.PLAY_ARGUMENT)
                    {
                        _playArgumentEffect = new Action(ApplyEffect);
                        TurnManager.Instance.AddPlayArgumentEffects(_playArgumentEffect);
                    }
                }
            }
        }

        private void MoveToWaitingSpot()
        {
            // Move card to board waiting spot
            Transform dest = Board.Instance.waitingSpot;
            MouseController.Instance.SetHolding(this);
            UIManager.Instance.SetEndTurnButtonInteractable(false);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DOMove(dest.position, 0.5f));
            sequence.Join(transform.DOScale(_defaultScale, 0.5f));

            sequence.Play();
        }

        public void ContinuePlay()
        {
            StartCoroutine(ContinuePlayCoroutine());
        }

        private IEnumerator ContinuePlayCoroutine()
        {
            TurnManager.Instance.StopFlow();

            SubstractMana();
            ApplyEffect();

            yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

            DestroyCard();
            MouseController.Instance.SetHolding(null);
            UIManager.Instance.SetEndTurnButtonInteractable(true);
        }

        public void ContinuePlayOpponent()
        {
            SubstractMana();

            ApplyEffect(effect, _storedTarget);
            DestroyCard();

            _storedTarget = null;
            CardGameManager.Instance.opponentAI.enabled = true;
            MouseController.Instance.SetHolding(null);
        }

        public void SubstractMana()
        {
            int manaCostLeft = manaCost;

            if (TurnManager.Instance.IsStealMana(contender))
            {
                Contender otherContender = CardGameManager.Instance.GetOtherContender(contender);
                manaCostLeft -= otherContender.currentMana;
                otherContender.SubstractMana(manaCost, continueFlow: false);
            }

            if (manaCostLeft > 0)
            {
                if (contender.freeMana) contender.SetFreeMana(false);
                else contender.SubstractMana(manaCostLeft, continueFlow: false);
            }
        }

        public void CancelPlay()
        {
            // Return card to hand
            _hand.AddCard(this);
            MouseController.Instance.SetHolding(null);
        }

        #endregion

        #region Clash

        public void Hit(object target)
        {
            if (target is Card)
            {
                Card card = (Card)target;
                card.ReceiveDamage(strength);
            }
            else
            {
                Contender targetContender = (Contender)target;

                if (HasEffect(SubType.COMPARTMENTALIZE) && targetContender.deck.numCards > 0) return;

                if (TurnManager.Instance.IsMirror(targetContender))
                    contender.ReceiveDamage(strength);
                else
                    targetContender.ReceiveDamage(strength);
            }
        }

        public Sequence HitSequence(object target)
        {
            Sequence hitSequence = DOTween.Sequence();

            if (strength != 0)
            {
                bool isCard = target is Card;
                Vector3 targetPosition = (isCard) ? ((Card)target).transform.position : ((Contender)target).transform.position;

                float approachFactor = isCard ? 0.2f : 0.8f;
                Vector3 targetDir = (targetPosition - transform.position) * approachFactor;
                targetDir = new Vector3(targetDir.x, targetDir.y, 0);

                Vector3 previousRotation = transform.rotation.eulerAngles;
                Quaternion rotation;
                if(contender.isPlayer)
                    rotation = Quaternion.FromToRotation(transform.up, targetDir);
                else
                    rotation = Quaternion.FromToRotation(transform.up * -1, targetDir);

                Vector3 rotationEuler = rotation.eulerAngles;
                if (rotationEuler.z > 180) rotationEuler = new Vector3(rotationEuler.x, rotationEuler.y, rotationEuler.z - 360);

                // Enlarge and rotate
                hitSequence.Append(transform.DOScale(_hitScale, 0.5f));
                if (contender.isPlayer)
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
                hitSequence.Append(transform.DOScale(_defaultScale, 0.2f));
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
            ReceiveDamage(strength, showParticles: false);
        }

        public void ReceiveDamage(int strength, bool showParticles)
        {
            if (showParticles) UIManager.Instance.ShowParticlesEffectTargetNegative(transform);

            data.defense -= strength;
            if (data.defense < 0) data.defense = 0;
            UpdateStatsUI();
            CheckDestroy();
        }

        public void ApplyCombatEffects(object target)
        {
            foreach (CardEffect effect in effects)
            {
                if (effect.applyTime == ApplyTime.COMBAT)
                {
                    ApplyEffect(effect, target);
                }
            }
        }

        #endregion

        #region Destroy

        public void DestroyCard()
        {
            DestroyCard(false, false);
        }

        public void DestroyCard(bool continueFlow)
        {
            DestroyCard(instant: false, continueFlow);
        }

        public void DestroyCard(bool instant, bool continueFlow)
        {
            //Play destroy animation
            Debug.Log(name + " destroyed");
            StartCoroutine(DestroyCardCoroutine(instant, continueFlow));
        }

        private IEnumerator DestroyCardCoroutine(bool instant, bool continueFlow)
        {
            _clickable = false;
            HideExtendedDescription();

            CheckDelegateEffects();
            if (!instant) CheckDestroyEffects();

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

        public void CheckDestroy()
        {
            if (TurnManager.Instance.combat) return;
            if (defense <= 0) DestroyCard();
        }

        #endregion

        #region Effects

        public void AddEffect(CardEffect effect)
        {
            if (!HasEffect(effect.subType))
            {
                effects.Add(effect);
                if (descriptionText != null) descriptionText.text = GetDescriptionText();
            }
        }

        public bool HasEffect(SubType subType)
        {
            foreach (CardEffect e in effects)
            {
                if (e.subType == subType) return true;
            }

            return false;
        }

        public void ApplyEffect()
        {
            ApplyEffect(effect);
        }

        public void ApplyEffect(object target)
        {
            ApplyEffect(effect, target);
        }

        private void ApplyEffect(CardEffect effect)
        {
            ApplyEffect(effect, null);
        }

        public void ApplyEffect(CardEffect effect, object target)
        {
            if (effect.IsAppliable()) effect.Apply(this, target);
        }

        public void ApplyEndTurnEffect()
        {
            StartCoroutine(ApplyEndTurnEffectCoroutine());
        }

        private IEnumerator ApplyEndTurnEffectCoroutine()
        {
            GameObject applyEffectParticles = UIManager.Instance.ShowParticlesEffectApply(transform);

            yield return new WaitUntil(() => applyEffectParticles == null);

            ApplyEffect(effect);
        }

        private void CheckDestroyEffects()
        {
            if (hasEffect)
            {
                foreach (CardEffect effect in effects)
                {
                    if (effect.applyTime == ApplyTime.DESTROY && !IsInHand)
                    {
                        ApplyEffect(effect);
                    }
                }
            }
        }

        private void CheckDelegateEffects()
        {
            if (hasEffect)
            {
                foreach (CardEffect effect in effects)
                {
                    // End Turn Effects
                    if (effect.applyTime == ApplyTime.END && _endTurnEffects?.Count > 0)
                    {
                        TurnManager.Instance.RemoveEndTurnEffect(_endTurnEffects[_endTurnEffects.Count - 1]);
                    }
                    // Draw Card Effects
                    else if (effect.applyTime == ApplyTime.DRAW_CARD && _drawCardEffect != null)
                    {
                        contender.deck.RemoveDrawCardEffect(_drawCardEffect);
                    }
                    // Play Argument Effects
                    else if (effect.applyTime == ApplyTime.PLAY_ARGUMENT && _playArgumentEffect != null)
                    {
                        TurnManager.Instance.RemovePlayArgumentEffect(_playArgumentEffect);
                    }

                    // Guard Card
                    if (effect.subType == SubType.GUARD)
                    {
                        TurnManager.Instance.RemoveGuardCard(contender);
                    }
                    else if (effect.subType == SubType.MIRROR)
                    {
                        TurnManager.Instance.SetMirror(contender, false);
                    }
                }
            }
        }

        public void ManageEffects()
        {
            if (hasEffect)
            {
                foreach (CardEffect effect in effects)
                {
                    if (effect.subType == SubType.GUARD) TurnManager.Instance.SetGuardCard(this);
                    else if (effect.subType == SubType.MIRROR) TurnManager.Instance.SetMirror(contender, true);
                }
            }
        }

        public void BoostStats(int strengthBoost, int defenseBoost)
        {
            data.strength += strengthBoost;
            data.defense += defenseBoost;

            if (type == CardType.ARGUMENT)
                UpdateStatsUI();

            UIManager.Instance.ShowParticlesEffectTargetPositive(transform);
        }

        public void DecreaseStats(int strengthDecrease, int defenseDecrease)
        {
            data.strength -= strengthDecrease;
            data.defense -= defenseDecrease;
            if (data.strength < 0) data.strength = 0;
            if (type == CardType.ARGUMENT)
                UpdateStatsUI();

            UIManager.Instance.ShowParticlesEffectTargetNegative(transform);
            CheckDestroy();
        }

        public bool IsBoosted()
        {
            return strength > defaultStrength || defense > defaultDefense;
        }

        public int GetBoost()
        {
            int boost = 0;
            if (strength > defaultStrength) boost += strength - defaultStrength;
            if (defense > defaultDefense) boost += defense - defaultDefense;
            return boost;
        }

        public bool IsDamaged()
        {
            return defense < defaultDefense;
        }

        public int GetDamage()
        {
            return defaultDefense - defense;
        }

        public void SwapContender()
        {
            if (contender.isPlayer)
                contender = CardGameManager.Instance.opponent;
            else
                contender = CardGameManager.Instance.player;

            _hand = contender.hand;
            _swapped = !_swapped;
        }

        public void ShowHighlight(bool show)
        {
            highlight.SetActive(show);
            isHighlighted = show;
        }

        public bool IsAlternateWinConditionCard()
        {
            return type == CardType.ACTION && hasEffect && effect.type == EffectType.ALTERNATE_WIN_CONDITION;
        }
        
        #endregion

        #region Containers

        private void AddToContainer(CardZone cardZone)
        {
            cardZone.AddCard(this);
            transform.DOScale(_defaultScale, 0.2f);
        }

        public void SetContainer(CardContainer container)
        {
            this.container = container;
            if (_hand == null && container is Hand)
            {
                _hand = (Hand)container;
                if (contender == null) contender = _hand.contender;
            }
        }

        public void RemoveFromContainer()
        {
            if (container != null)
            {
                container.RemoveCard(gameObject);
                container = null;
                transform.parent = null;
            }
        }

        public void ReturnToHand()
        {
            RemoveFromContainer();
            CheckDelegateEffects();

            data.strength = defaultStrength;
            data.defense = defaultDefense;

            if (_swapped) SwapContender();
            _hand.AddCard(this);
            if (!contender.isPlayer) FlipCard();
            if (type == CardType.ARGUMENT) UpdateStatsUI();
        }

        #endregion

    }
}