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
        public int manaCost => _data.cost;
        public int strength => _data.strength;
        public int defense => _data.defense;
        public CardType type => _data.type;
        public List<CardEffect> effects => _data.effects;
        public CardEffect effect => effects[0];
        public bool hasEffect => effects.Count > 0;

        private CardsData _data;
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

        private void Update()
        {
            //if (moveWithMouse)
            //{
            //    Move();
            //}
        }

        public void Initialize(Contender contender, Hand hand, CardsData data, bool cardRevealed)
        {
            this.contender = contender;
            _hand = hand;
            _data = data;

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

        #endregion

        #region Drag

        /// <summary>
        /// Maps mouse position in the screen to world coordinates to move the holding card.
        /// </summary>
        //private void Move()
        //{
        //    Vector3 mousePos = Input.mousePosition;
        //    Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        //    transform.position = new Vector3(worldPos.x, worldPos.y, _movePositionZ);
        //}

        //public void SetMoveWithMouse(bool move)
        //{
        //    if (move && !moveWithMouse)
        //    {
        //        // Increase scale
        //        transform.DOScale(_moveScale, 0.2f);
        //        shadow.SetActive(true);
        //    }
        //    else if (!move && moveWithMouse)
        //    {
        //        // Decrease scale
        //        //transform.DOScale(_defaultScale, 0.2f);
        //        shadow.SetActive(false);
        //    }

        //    moveWithMouse = move;
        //}

        #endregion

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
                _spriteRenderer.sprite = _data.sprite;
                nameText.gameObject.SetActive(true);
                descriptionText.gameObject.SetActive(true);
                strengthText.gameObject.SetActive(true);
                defenseText.gameObject.SetActive(true);
            }

            transform.Rotate(new Vector3(0, 0, 180));
            _cardFront = !_cardFront;
        }

        //private void CheckCloseUp()
        //{
        //    Sequence sequence = DOTween.Sequence();
        //    sequence.AppendCallback(() => _clickable = false);

        //    if (_closeUp)
        //    {
        //        sequence.Join(transform.DOMove(_defaultPos, _closeUpTime));
        //        sequence.Join(transform.DOScale(_defaultScale, _closeUpTime));
        //    }
        //    else
        //    {
        //        _defaultPos = transform.position;
        //        sequence.Join(transform.DOMove(_closeUpPosition.position, _closeUpTime));
        //        sequence.Join(transform.DOScale(_augmentedScale, _closeUpTime));
        //    }

        //    sequence.AppendCallback(() => _clickable = true);
        //    sequence.Play();

        //    _closeUp = !_closeUp;
        //}


        #region IClickable methods

        public void OnMouseLeftClickDown(MouseController mouseController)
        {
            //if (!moveWithMouse && mouseController.holdingCard == null && IsInHand && IsPlayerCard)
            //{
            //    // Deattach from parent
            //    RemoveFromContainer();
            //}

            //    // Stick to mouse
            //    SetMoveWithMouse(true);
            //    mouseController.SetHolding(this);
            //}
        }

        public void OnMouseLeftClickUp(MouseController mouseController)
        {

            //if (mouseController.holdingCard == this && contender.role == Contender.Role.PLAYER)
            //{
            //    // Return card to hand
            //    _hand.AddCard(this);
            //    mouseController.SetHolding(null);
            //    SetMoveWithMouse(false);
            //}
            if (mouseController.holdingCard == null && IsInHand && IsPlayerCard)
            {
                if (EnoughMana())
                {
                    // Deattach from parent
                    RemoveFromContainer();

                    MoveToWaitingSpot(null);
                    mouseController.SetHolding(this);

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

            if (_hand.isDiscarding && IsInHand)
            {
                int numCards = _hand.numCards - 1;
                _hand.CheckDiscarding(numCards);
                Destroy();
            }
        }

        private bool EnoughMana()
        {
            return contender.freeMana || manaCost <= contender.currentMana;
        }

        public void OnMouseRightClick()
        {
            //if (!_clickable) return;

            //CheckCloseUp();
        }

        public void OnMouseHoverEnter()
        {
            if (IsInHand && IsPlayerCard)
            {
                transform.DOLocalMoveY(_hoverPosY, 0.2f);
                transform.DOScale(_hoverScale, 0.2f);
            }

            if (contender.role == Contender.Role.PLAYER || _cardFront)
            {
                UIManager.Instance.ShowExtendedDescription(NameToString(), TypeToString(), DescriptionToString());
            }
        }

        public void OnMouseHoverExit()
        {
            if (this != null && gameObject != null && !moveWithMouse && IsInHand && IsPlayerCard)
            {
                transform.DOLocalMoveY(0f, 0.2f);
                if (_hand.isDiscarding) transform.DOScale(_highlightScale, 0.2f);
                else transform.DOScale(_defaultScale, 0.2f);
            }

            UIManager.Instance.HideExtendedDescription();
        }

        public void SetClickable(bool clickable)
        {
            this._clickable = clickable;
        }

        #endregion

        #region Play

        public void Play(CardZone cardZone)
        {
            // Substract mana
            if (contender.freeMana) contender.SetFreeMana(false);
            else contender.SubstractMana(manaCost);

            if (!IsPlayerCard) FlipCard();
            //SetMoveWithMouse(false);

            //if (hasEffect) CloseUp(() => PlayCard(cardZone));
            //else PlayCard(cardZone);

            PlayCard(cardZone);
            CardGameManager.Instance.CheckDialogue(this);
        }

        private void PlayCard(CardZone cardZone)
        {
            switch (type)
            {
                case CardType.ARGUMENT:
                    AddToContainer(cardZone);
                    CheckEffect();
                    TurnManager.Instance.ApplyPlayArgumentEffects();
                    break;

                case CardType.FIELD:
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
            if (effect.HasTarget())
            {
                List<Card> possibleTargets = effect.FindPossibleTargets();

                if (IsPlayerCard)
                {
                    //MoveToWaitingSpot(null);

                    Board.Instance.HighlightTargets(possibleTargets);

                    MouseController.Instance.SetApplyingEffect(this);
                    UIManager.Instance.SetEndTurnButtonInteractable(false);
                }
                else
                {
                    if (possibleTargets.Count > 0)
                    {
                        int index = new System.Random().Next(0, possibleTargets.Count);
                        MoveToWaitingSpot(() =>
                        {
                            effect.Apply(this, possibleTargets[index]);
                            Destroy();
                        });
                    }
                    else
                    {
                        MoveToWaitingSpot(() =>
                        {
                            Destroy();
                        });
                    }
                }
            }
            else
            {
                if (contender.role == Contender.Role.PLAYER)
                {
                    UIManager.Instance.ShowContinuePlayButton();
                }
                else
                {
                    //If the effect has no target, we apply the effect and destroy the card
                    MoveToWaitingSpot(() =>
                    {
                        effect.Apply(this, null);
                        Destroy();
                    });
                    MouseController.Instance.SetHolding(null);
                }
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

        private void MoveToWaitingSpot(TweenCallback onCompleteCallback)
        {
            // Move card to board waiting spot
            Transform dest = Board.Instance.waitingSpot;
            //SetMoveWithMouse(false);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DOMove(dest.position, 0.5f));
            sequence.Join(transform.DOScale(_defaultScale, 0.5f));
            sequence.AppendInterval(0.5f);
            sequence.OnComplete(onCompleteCallback);

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
            // Substract mana
            if (contender.freeMana) contender.SetFreeMana(false);
            else contender.SubstractMana(manaCost);

            ApplyEffect();
            Destroy();

            MouseController.Instance.SetHolding(null);
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
            _data.defense -= strength;
            if (_data.defense < 0) _data.defense = 0;
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

            if (container != null /*&& !IsInHand*/) container.RemoveCard(gameObject);
            CheckRemoveDelegateEffect();

            Sequence destroySequence = DOTween.Sequence();
            destroySequence.Append(transform.DOScale(0, 1));
            destroySequence.AppendCallback(() =>
            {
                if (container != null && IsInHand) container.RemoveCard(gameObject);
                Destroy(gameObject);
            });

            destroySequence.Play();
        }

        public void CheckDestroy()
        {
            if (defense <= 0) Destroy();
        }

        #endregion


        public void ApplyEffect()
        {
            ApplyEffect(effect);
        }

        private void ApplyEffect(CardEffect effect)
        {
            if (effect.IsAppliable())
            {
                effect.Apply(this, null);
            }
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
            _data.strength += strengthBoost;
            if (_data.strength < 0) _data.strength = 0;
            _data.defense += defenseBoost;
            //Update card strength and defense number
            if (type == CardType.ARGUMENT)
                UpdateStatsUI();
            CheckDestroy();
        }

        public void AddEffect(CardEffect effect)
        {
            if (!effects.Contains(effect))
            {
                effects.Add(effect);
                if (descriptionText != null) descriptionText.text = GetDescriptionText();
            }
        }
        public void RemoveFromContainer()
        {
            if (container != null)
            {
                container.RemoveCard(gameObject);
                transform.parent = null;
            }
        }

        public void ReturnToHand()
        {
            RemoveFromContainer();
            CheckRemoveDelegateEffect();

            _data.strength = defaultStrength;
            _data.defense = defaultDefense;
            _hand.AddCard(this);
            if (contender.role == Contender.Role.OPPONENT) FlipCard();
            if (type == CardType.ARGUMENT) UpdateStatsUI();
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

        public CardsData GetData()
        {
            return _data;
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

    }
}