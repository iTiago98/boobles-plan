using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using CardGame.Managers;
using CardGame.Level;
using CardGame.Cards.DataModel;
using TMPro;
using CardGame.Cards.DataModel.Effects;

namespace CardGame.Cards
{
    public class Card : MonoBehaviour, IClickable
    {
        #region Card Data
        public int manaCost => data.cost;
        public int strength => data.strength;
        public int defense => data.defense;
        public CardType type => data.type;
        public List<CardEffect> effects => data.effects;
        public CardEffect effect => effects[0];
        public bool hasEffect => effects.Count > 0;

        public CardsData data { private set; get; }
        private int defaultStrength;
        private int defaultDefense;

        #endregion

        #region UI

        public TextMeshPro nameText;
        public TextMeshPro descriptionText;
        public TextMeshPro strengthText;
        public TextMeshPro defenseText;

        public Sprite cardBack;
        public Color altColor;
        public GameObject shadow;
        public GameObject highlight;

        private bool _cardFront = true;

        #endregion

        #region Close up

        //private bool _closeUp;
        //private Vector3 _defaultPos;
        //private Transform _closeUpPosition;
        //private Vector3 _defaultScale;
        //private Vector3 _augmentedScale;
        //private float _closeUpTime;

        #endregion

        #region Hover

        private float _hoverPosY;
        private float _hoverScale;

        #endregion

        #region Move with mouse 

        [HideInInspector]
        public bool moveWithMouse;

        private float _movePositionZ;
        private float _moveScale;

        #endregion

        private float _highlightScale;
        private float _defaultScale;

        #region Hit animation

        private float _hitScale;

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
        GameObject IClickable.gameObject { get => gameObject; set => Debug.Log(""); }

        private SpriteRenderer _spriteRenderer;

        private TurnManager.PlayArgumentEffects _playArgumentEffect;
        private List<TurnManager.EndTurnEffects> _endTurnEffects;
        private Deck.DrawCardEffects _drawCardEffect;

        private Card _storedTarget;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _clickable = true;

            _hoverPosY = CardGameManager.Instance.settings.hoverPosY;
            _hoverScale = CardGameManager.Instance.settings.hoverScale;

            _movePositionZ = CardGameManager.Instance.settings.movePositionZ;
            _moveScale = CardGameManager.Instance.settings.moveScale;

            _highlightScale = CardGameManager.Instance.settings.highlightScale;
            _defaultScale = CardGameManager.Instance.settings.defaultScale;

            _hitScale = CardGameManager.Instance.settings.hitScale;

            _endTurnEffects = new List<TurnManager.EndTurnEffects>();
        }

        public void Initialize(Contender contender, Hand hand, CardsData data, bool cardRevealed)
        {
            this.contender = contender;
            _hand = hand;
            this.data = data;

            name = data.name;

            if (cardRevealed || IsPlayerCard)
            {
                if (data.sprite != null) _spriteRenderer.sprite = data.sprite;
            }
            else
            {
                FlipCard();
            }

            if (nameText != null) nameText.text = data.name;
            if (descriptionText != null) descriptionText.text = GetDescriptionText();

            if (type == CardType.ARGUMENT)
            {
                defaultStrength = data.strength;
                defaultDefense = data.defense;
                UpdateStatsUI();
            }
        }

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

        public void FlipCard()
        {
            if (_cardFront)
            {
                _spriteRenderer.sprite = cardBack;
                nameText.gameObject.SetActive(false);
                descriptionText.gameObject.SetActive(false);
                strengthText.gameObject.SetActive(false);
                defenseText.gameObject.SetActive(false);
            }
            else
            {
                _spriteRenderer.sprite = data.sprite;
                nameText.gameObject.SetActive(true);
                descriptionText.gameObject.SetActive(true);
                strengthText.gameObject.SetActive(true);
                defenseText.gameObject.SetActive(true);
            }

            transform.Rotate(new Vector3(0, 0, 180));
            _cardFront = !_cardFront;
        }

        #endregion

        #region IClickable methods

        public void OnMouseLeftClickDown(MouseController mouseController)
        {
        }

        public void OnMouseLeftClickUp(MouseController mouseController)
        {
            if (!_clickable) return;

            if (IsInHand)
            {
                if (_hand.isDiscarding)
                {
                    int numCards = _hand.numCards - 1;
                    _hand.CheckDiscarding(numCards);
                    Destroy();
                }
                else if (IsPlayerCard && !mouseController.IsHoldingCard)
                {
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
            return contender.freeMana || manaCost <= contender.currentMana;
        }

        public void OnMouseHoverEnter()
        {
            if (!_clickable) return;

            if (IsInHand && IsPlayerCard)
            {
                transform.DOLocalMoveY(_hoverPosY, 0.2f);
                transform.DOScale(_hoverScale, 0.2f);
            }

            if (_cardFront) ShowExtendedDescription();
        }

        public void OnMouseHoverExit()
        {
            if (!_clickable) return;

            if (this != null && gameObject != null && !moveWithMouse && IsInHand && IsPlayerCard)
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
                    UIManager.Instance.ShowContinuePlayButton();
                }
            }
            else
            {
                List<Card> possibleTargets = effect.FindPossibleTargets();

                if (possibleTargets.Count > 0)
                {
                    int index = new System.Random().Next(0, possibleTargets.Count);
                    Board.Instance.HighlightTargets(new List<Card>() { possibleTargets[index] });
                    _storedTarget = possibleTargets[index];
                }

                MoveToWaitingSpot();
                CardGameManager.Instance.opponentAI.enabled = false;
                UIManager.Instance.ShowContinuePlayButton();
            }
        }

        private void AddToContainer(CardZone cardZone)
        {
            cardZone.AddCard(this);
            transform.DOScale(_defaultScale, 0.2f);
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
                        _endTurnEffects.Add(new TurnManager.EndTurnEffects(ApplyEffect));
                        TurnManager.Instance.AddEndTurnEffect(_endTurnEffects[_endTurnEffects.Count - 1]);
                    }
                    else if (effect.applyTime == ApplyTime.DRAW_CARD)
                    {
                        _drawCardEffect = new Deck.DrawCardEffects(ApplyEffect);
                        Board.Instance.GetDeck(contender).AddDrawCardEffects(_drawCardEffect);
                    }
                    else if (effect.applyTime == ApplyTime.PLAY_ARGUMENT)
                    {
                        _playArgumentEffect = new TurnManager.PlayArgumentEffects(ApplyEffect);
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

            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DOMove(dest.position, 0.5f));
            sequence.Join(transform.DOScale(_defaultScale, 0.5f));
            //sequence.AppendInterval(0.5f);
            //sequence.OnComplete(onCompleteCallback);

            sequence.Play();
        }

        //private void CloseUp(TweenCallback onCompleteCallback)
        //{
        //    Sequence sequence = DOTween.Sequence();
        //    Transform dest = Board.Instance.closeUpSpot;

        //    sequence.Append(transform.DOLocalMove(dest.position, 0.2f));
        //    sequence.Join(transform.DOScale(1, 0.2f));
        //    sequence.AppendInterval(0.5f);
        //    //sequence.onComplete += () => transform.position = dest.position;
        //    sequence.onComplete += onCompleteCallback;

        //    sequence.Play();
        //}

        public void ContinuePlay()
        {
            SubstractMana();

            ApplyEffect();
            Destroy();

            MouseController.Instance.SetHolding(null);
        }

        public void ContinuePlayOpponent()
        {
            SubstractMana();

            ApplyEffect(_storedTarget);
            Destroy();

            _storedTarget = null;
            CardGameManager.Instance.opponentAI.enabled = true;
            MouseController.Instance.SetHolding(null);
        }

        private void SubstractMana()
        {
            if (contender.freeMana) contender.SetFreeMana(false);
            else contender.SubstractMana(manaCost);
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
                if (hasEffect && effect.subType == SubType.COMPARTMENTALIZE) return;

                Contender contender = (Contender)target;
                contender.ReceiveDamage(strength);
            }
        }

        public Sequence HitSequence(object target, TweenCallback hitCallback)
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
                Quaternion rotation = Quaternion.FromToRotation(transform.up, targetDir);
                Vector3 rotationEuler = rotation.eulerAngles;
                if (rotationEuler.z > 180) rotationEuler = new Vector3(rotationEuler.x, rotationEuler.y, rotationEuler.z - 360);

                // Enlarge and rotate
                hitSequence.Append(transform.DOScale(_hitScale, 0.5f));
                if (contender.role == Contender.Role.PLAYER)
                    hitSequence.Join(transform.DOLocalMoveZ(-0.1f, 0.5f));
                else
                    hitSequence.Join(transform.DOLocalMoveZ(-0.05f, 0.5f));
                hitSequence.Join(transform.DORotate(rotationEuler, 0.2f).SetRelative());

                // Hit
                hitSequence.Append(transform.DOMove(targetDir, 0.2f).SetRelative());


                // Hit callback
                if (hitCallback != null)
                {
                    hitSequence.AppendCallback(hitCallback);
                }

                // Back
                hitSequence.Append(transform.DOLocalMove(Vector3.zero, 0.2f));
                hitSequence.AppendCallback(() =>
                {
                    CheckDestroy();
                });
                hitSequence.Append(transform.DOScale(_defaultScale, 0.2f));
                hitSequence.Join(transform.DORotate(previousRotation, 0.2f));
            }
            else
            {
                // Hit callback
                if (hitCallback != null)
                {
                    hitSequence.AppendCallback(hitCallback);
                }
            }

            return hitSequence;
        }

        public void ReceiveDamage(int strength)
        {
            data.defense -= strength;
            if (data.defense < 0) data.defense = 0;
            UpdateStatsUI();
        }

        public void ApplyCombatEffects(object target)
        {
            foreach (CardEffect effect in effects)
            {
                if (effect.applyTime == ApplyTime.COMBAT)
                {
                    effect.Apply(this, target);
                }
            }
        }

        #endregion

        #region Destroy

        public void Destroy()
        {
            //Play destroy animation
            Debug.Log(name + " destroyed");

            _clickable = false;
            if (!IsInHand) RemoveFromContainer();
            CheckRemoveDelegateEffect();

            Sequence destroySequence = DOTween.Sequence();
            destroySequence.Append(transform.DOScale(0, 1));
            destroySequence.AppendCallback(() =>
            {
                if (IsInHand) RemoveFromContainer();
                Destroy(gameObject);
            });

            destroySequence.Play();
        }

        public void CheckDestroy()
        {
            if (defense <= 0) Destroy();
        }

        #endregion

        #region Effects

        public void AddEffect(CardEffect effect)
        {
            if (!effects.Contains(effect))
            {
                effects.Add(effect);
                if (descriptionText != null) descriptionText.text = GetDescriptionText();
            }
        }

        public void ApplyEffect()
        {
            ApplyEffect(effect);
        }

        private void ApplyEffect(CardEffect effect)
        {
            if (effect.IsAppliable()) effect.Apply(this, null);
        }

        private void ApplyEffect(Card target)
        {
            if (effect.IsAppliable()) effect.Apply(this, target);
        }

        private void CheckRemoveDelegateEffect()
        {
            if (hasEffect)
            {
                foreach (CardEffect effect in effects)
                {
                    if (effect.applyTime == ApplyTime.END && _endTurnEffects.Count > 0)
                    {
                        TurnManager.Instance.RemoveEndTurnEffect(_endTurnEffects[_endTurnEffects.Count - 1]);
                    }
                    else if (effect.applyTime == ApplyTime.DRAW_CARD && _drawCardEffect != null)
                    {
                        Board.Instance.GetDeck(contender).RemoveDrawCardEffect(_drawCardEffect);
                    }
                    else if (effect.applyTime == ApplyTime.PLAY_ARGUMENT && _playArgumentEffect != null)
                    {
                        TurnManager.Instance.RemovePlayArgumentEffect(_playArgumentEffect);
                    }

                    if (effect.subType == SubType.GUARD)
                    {
                        TurnManager.Instance.RemoveGuardCard(contender);
                    }
                }
            }
        }

        public void BoostStats(int strengthBoost, int defenseBoost)
        {
            data.strength += strengthBoost;
            if (data.strength < 0) data.strength = 0;
            data.defense += defenseBoost;
            //Update card strength and defense number
            if (type == CardType.ARGUMENT)
                UpdateStatsUI();
            CheckDestroy();
        }

        public void SwapContender()
        {
            if (contender.role == Contender.Role.PLAYER)
            {
                _hand = Board.Instance.GetHand(CardGameManager.Instance.opponent);
            }
            else
            {
                _hand = Board.Instance.GetHand(CardGameManager.Instance.player);
            }
        }

        #endregion

        #region Containers

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
            CheckRemoveDelegateEffect();

            data.strength = defaultStrength;
            data.defense = defaultDefense;
            _hand.AddCard(this);
            if (contender.role == Contender.Role.OPPONENT) FlipCard();
            if (type == CardType.ARGUMENT) UpdateStatsUI();
        }

        #endregion

        #region ToString

        public void ShowExtendedDescription()
        {
            UIManager.Instance.ShowExtendedDescription(NameToString(), TypeToString(), DescriptionToString());
        }

        public void HideExtendedDescription()
        {
            UIManager.Instance.HideExtendedDescription();
        }

        public string NameToString()
        {
            return name.ToUpper();
        }

        public string TypeToString()
        {
            return "TIPO: " + type.ToString();
        }

        public string DescriptionToString()
        {
            if (hasEffect)
            {
                return effect.ToStringExtended(type);
            }

            return "";
        }

        #endregion
    }
}