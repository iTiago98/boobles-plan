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
        private float _defaultScale;
        private float _moveScale;

        #endregion

        #region Hit animation

        private float _hitScale;

        #endregion

        [HideInInspector]
        public CardContainer container;
        public Contender contender => TurnManager.Instance.GetContenderFromHand(_hand);

        private Hand _hand;

        private bool _clickable;

        bool IClickable.clickable { get => _clickable; set => _clickable = value; }
        GameObject IClickable.gameObject { get => gameObject; set => Debug.Log(""); }

        private SpriteRenderer _spriteRenderer;


        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _clickable = true;

            _hoverPosY = TurnManager.Instance.settings.hoverPosY;
            _hoverScale = TurnManager.Instance.settings.hoverScale;

            _movePositionZ = TurnManager.Instance.settings.movePositionZ;
            _defaultScale = TurnManager.Instance.settings.defaultScale;
            _moveScale = TurnManager.Instance.settings.moveScale;

            _hitScale = TurnManager.Instance.settings.hitScale;
        }

        private void Update()
        {
            if (moveWithMouse)
            {
                Move();
            }
        }

        public void Initialize(Hand hand, CardsData data)
        {
            _hand = hand;
            _data = data;

            name = data.name;

            if (contender.role == Contender.Role.PLAYER)
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
                temp += effect.ToString();
            }

            return temp;
        }

        #endregion

        #region Drag

        /// <summary>
        /// Maps mouse position in the screen to world coordinates to move the holding card.
        /// </summary>
        private void Move()
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(worldPos.x, worldPos.y, _movePositionZ);
        }

        public void SetMoveWithMouse(bool move)
        {
            if (move && !moveWithMouse)
            {
                // Increase scale
                transform.DOScale(_moveScale, 0.2f);
                shadow.SetActive(true);
            }
            else if (!move && moveWithMouse)
            {
                // Decrease scale
                //transform.DOScale(_defaultScale, 0.2f);
                shadow.SetActive(false);
            }

            moveWithMouse = move;
        }

        #endregion

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
            if (!moveWithMouse && mouseController.holdingCard == null && container == _hand)
            {
                // Deattach from parent
                RemoveFromContainer();

                // Stick to mouse
                SetMoveWithMouse(true);
                mouseController.SetHolding(this);
            }
        }

        public void OnMouseLeftClickUp(MouseController mouseController)
        {
            if (mouseController.holdingCard == this)
            {
                // Return card to hand
                _hand.AddCard(this);
                mouseController.SetHolding(null);
                SetMoveWithMouse(false);
            }
        }

        public void OnMouseRightClick()
        {
            //if (!_clickable) return;

            //CheckCloseUp();
        }

        public void OnMouseHoverEnter()
        {
            if (container == _hand)
            {
                transform.DOLocalMoveY(_hoverPosY, 0.2f);
                transform.DOScale(_hoverScale, 0.2f);
            }
        }

        public void OnMouseHoverExit()
        {
            if (this != null && gameObject != null && container == _hand && !moveWithMouse)
            {
                transform.DOLocalMoveY(0f, 0.2f);
                transform.DOScale(_defaultScale, 0.2f);
            }
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
            contender.SubstractMana(manaCost);
            if (contender.role == Contender.Role.OPPONENT) FlipCard();
            SetMoveWithMouse(false);

            //if (hasEffect) CloseUp(() => PlayCard(cardZone));
            //else PlayCard(cardZone);

            PlayCard(cardZone);
        }

        private void PlayCard(CardZone cardZone)
        {
            switch (type)
            {
                case CardType.ARGUMENT:
                    PlayArgument(cardZone);

                    break;
                case CardType.ACTION:
                    PlayAction();

                    break;
                case CardType.FIELD:
                    break;
            }
        }

        private void PlayArgument(CardZone cardZone)
        {
            // Add to container
            cardZone.AddCard(this);
            transform.DOScale(_defaultScale, 0.2f);

            // Apply enter effect
            if (hasEffect && effect.IsAppliable())
            {
                effect.Apply(this, null);
            }
        }

        private void PlayAction()
        {
            if (effect.HasTarget())
            {
                List<Card> possibleTargets = effect.FindPossibleTargets();

                if (contender.role == Contender.Role.PLAYER)
                {
                    MoveToWaitingSpot(null);

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
                }
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

        private void FlipCard()
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

        private void MoveToWaitingSpot(TweenCallback onCompleteCallback)
        {
            // Move card to board waiting spot
            Transform dest = Board.Instance.waitingSpot;
            SetMoveWithMouse(false);

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

        #endregion

        #region Clash

        public Sequence Hit(object target)
        {
            return HitAnimation(target);
        }

        private Sequence HitAnimation(object target)
        {
            Sequence hitSequence = DOTween.Sequence();

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
            hitSequence.Join(transform.DORotate(rotationEuler, 0.2f).SetRelative());
            // Hit
            hitSequence.Append(transform.DOMove(targetDir, 0.2f).SetRelative());
            hitSequence.AppendCallback(() =>
            {
                ApplyCombatEffects(target);
                if (isCard)
                {
                    Card card = (Card)target;
                    card.ReceiveDamage(strength);
                }
                else
                {
                    Contender contender = (Contender)target;
                    contender.ReceiveDamage(strength);
                }
            });
            // Back
            hitSequence.Append(transform.DOLocalMove(Vector3.zero, 0.2f));
            hitSequence.AppendCallback(() =>
            {
                CheckDestroy();
            });
            hitSequence.Append(transform.DOScale(_defaultScale, 0.2f));
            hitSequence.Join(transform.DORotate(previousRotation, 0.2f));

            return hitSequence;
        }

        public void ReceiveDamage(int strength)
        {
            _data.defense -= strength;
            if (_data.defense < 0) _data.defense = 0;
            UpdateStatsUI();
        }

        private void ApplyCombatEffects(object target)
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
            Sequence destroySequence = DOTween.Sequence();
            destroySequence.Append(transform.DOScale(0, 1));
            destroySequence.AppendCallback(() =>
            {
                container.RemoveCard(gameObject);
                Destroy(gameObject);
            });

            destroySequence.Play();
        }

        private void CheckDestroy()
        {
            if (defense <= 0) Destroy();
        }

        #endregion

        public void BoostStats(int strengthBoost, int defenseBoost)
        {
            _data.strength += strengthBoost;
            _data.defense += defenseBoost;
            //Update card strength and defense number
            if (type == CardType.ARGUMENT)
                UpdateStatsUI();
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
            _data.strength = defaultStrength;
            _data.defense = defaultDefense;
            _hand.AddCard(this);
            if (contender.role == Contender.Role.OPPONENT) FlipCard();
            UpdateStatsUI();
        }


    }
}