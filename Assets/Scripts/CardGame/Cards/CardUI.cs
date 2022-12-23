using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;
using CardGame.Level;
using CardGame.Managers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CardGame.Cards
{
    public class CardUI : MonoBehaviour
    {
        public bool IsHighlighted => highlight.activeSelf;

        [SerializeField] private TextMeshPro nameText;
        [SerializeField] private TextMeshPro descriptionText;
        [SerializeField] private TextMeshPro strengthText;
        [SerializeField] private TextMeshPro defenseText;

        [SerializeField] private Color altColor;
        [SerializeField] private GameObject highlight;

        [Header("Animations")]
        [SerializeField] private Animator VFX_Animator;
        [SerializeField] private AnimationClip effectClip;
        [SerializeField] private AnimationClip damagedClip;
        [SerializeField] private AnimationClip boostClip;
        [SerializeField] private AnimationClip decreaseClip;

        private Sprite cardBack;
        private bool _cardFront = true;

        private float _defaultScale;
        private float _hitScale;
        private float _hoverPosY;
        private float _hoverScale;
        private float _highlightScale;

        public float defaultScale => _defaultScale;
        public float hitScale => _hitScale;

        private bool _playingAnimation;
        public bool IsPlayingAnimation => _playingAnimation;

        private Card _card;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _defaultScale = CardGameManager.Instance.settings.defaultScale;
            _hitScale = CardGameManager.Instance.settings.hitScale;

            _hoverPosY = CardGameManager.Instance.settings.hoverPosY;
            _hoverScale = CardGameManager.Instance.settings.hoverScale;

            _highlightScale = CardGameManager.Instance.settings.highlightScale;
        }

        #region Initialize

        public void Initialize(Card card, bool cardRevealed)
        {
            _card = card;

            nameText.text = _card.data.name;
            descriptionText.text = GetDescriptionText();

            cardBack = _card.contender.GetCardBack();

            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (cardRevealed || _card.IsPlayerCard)
            {
                SetSprite(_card.data.sprite);
            }
            else FlipCard();

            if (_card.IsArgument)
            {
                UpdateStatsUI();
            }
        }

        private void SetSprite(Sprite sprite)
        {
            if (sprite != null) _spriteRenderer.sprite = sprite;
        }

        #endregion

        #region Stats

        public void UpdateStatsUI()
        {
            UpdateStatUI(strengthText, _card.Stats.strength, _card.Stats.defaultStrength, Color.black);
            UpdateStatUI(defenseText, _card.Stats.defense, _card.Stats.defaultDefense, Color.white);
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

        #endregion

        #region Description

        public void UpdateDescriptionText()
        {
            descriptionText.text = GetDescriptionText();
        }

        private string GetDescriptionText()
        {
            string temp = "";

            foreach (CardEffect effect in _card.Effects.effectsList)
            {
                temp += effect.ToString() + "\n";
            }

            return temp;
        }

        #endregion

        #region Extended Description

        public void ShowExtendedDescription()
        {
            if (_cardFront) UIManager.Instance.ShowExtendedDescription(NameToString(), TypeToString(), DescriptionToString());
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
            return "TIPO: " + _card.Stats.type.ToString();
        }

        private string DescriptionToString()
        {
            string s = "";
            foreach (CardEffect effect in _card.Effects.effectsList)
            {
                s += effect.ToStringExtended(_card.Stats.type) + "\n";
            }

            return s;
        }

        #endregion

        #region Highlight

        public void ShowHighlight(bool show)
        {
            highlight.SetActive(show);
        }

        #endregion

        #region Hover

        public void HoverOn()
        {
            transform.DOLocalMoveY(_hoverPosY, 0.2f);
            transform.DOScale(_hoverScale, 0.2f);
        }

        public void HoverOff(Hand hand)
        {
            transform.DOLocalMoveY(0f, 0.2f);
            if (hand.isDiscarding) transform.DOScale(_highlightScale, 0.2f);
            else transform.DOScale(defaultScale, 0.2f);
        }

        #endregion

        #region VFX

        public void ShowEffectAnimation()
        {
            _playingAnimation = true;
            StartCoroutine(ShowAnimationCoroutine("VFX_Effect", effectClip.length));
        }

        public void ShowDamagedAnimation()
        {
            _playingAnimation = true;
            StartCoroutine(ShowAnimationCoroutine("VFX_Damage", damagedClip.length));
        }

        public void ShowBoostAnimation()
        {
            _playingAnimation = true;
            StartCoroutine(ShowAnimationCoroutine("VFX_Boost", boostClip.length));
        }

        public void ShowDecreaseAnimation()
        {
            _playingAnimation = true;
            StartCoroutine(ShowAnimationCoroutine("VFX_Decrease", decreaseClip.length));
        }

        private IEnumerator ShowAnimationCoroutine(string animationName, float length)
        {
            VFX_Animator.Play(animationName);

            yield return new WaitForSeconds(length);

            _playingAnimation = false;
        }

        #endregion

        public void FlipCard()
        {
            if (_cardFront) _spriteRenderer.sprite = cardBack;
            else _spriteRenderer.sprite = _card.data.sprite;

            _cardFront = !_cardFront;

            nameText.gameObject.SetActive(_cardFront);
            descriptionText.gameObject.SetActive(_cardFront);

            if (_card.IsArgument)
            {
                strengthText.gameObject.SetActive(_cardFront);
                defenseText.gameObject.SetActive(_cardFront);
            }

            transform.Rotate(new Vector3(0, 0, 180));
        }

        public void ReturnToHand()
        {
            if (!_card.IsPlayerCard) FlipCard();
            if (_card.IsArgument) UpdateStatsUI();
        }

    }
}