using Booble.CardGame.Cards;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Cards.DataModel.Effects;
using Booble.CardGame.Managers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Level
{
    public class Deck : MonoBehaviour
    {
        [SerializeField] private GameObject argumentCardPrefab;
        [SerializeField] private GameObject actionCardPrefab;

        [SerializeField] private CardsData emptyDeckCardData;

        private int _maxCardNumber;
        private Contender _contender;
        private Hand _hand;
        private List<CardsData> _deckCards;

        public int numCards => _deckCards.Count;

        public bool busy { private set; get; }

        private List<Card> _listToAdd = new List<Card>();
        private List<Card> _listToDiscard = new List<Card>();

        private SpriteRenderer _spriteRenderer;

        #region Initialize

        public void Initialize(Contender contender, Hand hand, List<CardsData> deckCards)
        {
            _contender = contender;
            _hand = hand;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = contender.GetCardBack();

            CopyCardsList(deckCards);
            UIManager.Instance.ShowRemainingCards(contender);
            UpdateRemainingCards();
        }

        private void CopyCardsList(List<CardsData> cards)
        {
            _deckCards = new List<CardsData>();
            foreach (CardsData data in cards)
            {
                CardsData temp = new CardsData();
                temp.name = data.name;
                temp.sprite = data.sprite;
                temp.cost = data.cost;
                temp.strength = data.strength;
                temp.defense = data.defense;
                temp.type = data.type;
                temp.effects = data.effects;

                _deckCards.Add(temp);
            }

            _maxCardNumber = numCards;
        }

        #endregion

        #region Draw Cards

        public void DrawCards(int numCardsToAdd)
        {
            busy = true;

            int numCardsStart = numCards;

            for (int i = 0; i < numCardsToAdd; i++)
            {
                GameObject cardObj;
                CardsData data;

                if (numCards == 0)
                {
                    // Instantiate card
                    cardObj = Instantiate(argumentCardPrefab, transform.position, argumentCardPrefab.transform.rotation, _hand.transform);

                    // Take data
                    data = new CardsData(emptyDeckCardData);
                }
                else
                {
                    // Take data from scriptable
                    int index = Random.Range(0, numCards);
                    data = _deckCards[index];
                    _deckCards.RemoveAt(index);

                    // Instantiate card
                    GameObject cardPrefab = GetCardPrefab(data.type);
                    cardObj = Instantiate(cardPrefab, transform.position, cardPrefab.transform.rotation, _hand.transform);
                }

                // Add card to hand
                Card card = cardObj.GetComponent<Card>();
                card.Initialize(_contender, data, cardRevealed: false);
                card.gameObject.SetActive(false);

                _listToAdd.Add(card);
            }

            StartCoroutine(DrawCardsCoroutine(_hand, numCardsStart));
        }

        private IEnumerator DrawCardsCoroutine(Hand hand, int numCardsStart)
        {
            int loops = _listToAdd.Count;
            for (int i = 0; i < loops; i++)
            {
                Card card = _listToAdd[0];
                card.gameObject.SetActive(true);
                hand.AddCard(card.gameObject);

                // Apply end round effects
                CardEffectsManager.Instance.ApplyDrawCardEffects();

                UpdateRemainingCards(--numCardsStart);
                CheckDeckSprite();

                yield return new WaitUntil(() => hand.cardsAtPosition);

                _listToAdd.RemoveAt(0);
            }

            _listToAdd.Clear();

            busy = false;
            TurnManager.Instance.ContinueFlow();
        }

        #endregion

        #region Discard Cards


        public void DiscardCards(int numCardsToDiscard)
        {
            busy = true;
            int numCardsStart = numCards;

            for (int i = 0; i < numCardsToDiscard; i++)
            {
                if (numCards > 0)
                {
                    // Take data from scriptable
                    int index = Random.Range(0, numCards);
                    CardsData data = _deckCards[index];
                    _deckCards.RemoveAt(index);

                    Vector3 position = transform.position + new Vector3(0, 0, -0.1f);
                    GameObject cardPrefab = GetCardPrefab(data.type);
                    GameObject cardObj = Instantiate(cardPrefab, position, cardPrefab.transform.rotation);

                    Card card = cardObj.GetComponent<Card>();
                    card.Initialize(_contender, data, cardRevealed: true);
                    card.gameObject.SetActive(false);

                    if (card.Effects.HasEffect(SubType.INCOMPARTMENTABLE))
                    {
                        _listToAdd.Add(card);
                        break;
                    }
                    else
                    {
                        _listToDiscard.Add(card);
                    }
                }
            }

            StartCoroutine(DiscardCardsCoroutine(numCardsStart));
        }

        private IEnumerator DiscardCardsCoroutine(int numCardsStart)
        {
            int loops = _listToDiscard.Count;

            for (int i = 0; i < loops; i++)
            {
                Card card = _listToDiscard[0];
                card.gameObject.SetActive(true);
                card.DestroyCard();

                UpdateRemainingCards(--numCardsStart);
                CheckDeckSprite();

                yield return new WaitUntil(() => card.destroyed);

                _listToDiscard.RemoveAt(0);
            }

            _listToDiscard.Clear();

            if (_listToAdd.Count > 0) StartCoroutine(DrawCardsCoroutine(_hand, numCardsStart));
            else busy = false;
        }

        #endregion

        #region AddCards

        public void AddCards(List<Card> cards)
        {
            busy = true;
            StartCoroutine(AddCardCoroutine(cards));
        }

        private IEnumerator AddCardCoroutine(List<Card> cards)
        {
            int loops = cards.Count;
            for (int i = 0; i < loops; i++)
            {
                Card card = cards[0];
                card.gameObject.SetActive(true);

                Sequence sequence = DOTween.Sequence();
                sequence.Append(card.transform.DOMove(transform.position + new Vector3(0, 0, -0.2f), 0.5f));
                sequence.Append(card.transform.DOScale(0, 0.5f));
                sequence.Play();

                yield return new WaitForSeconds(0.5f);

                _deckCards.Add(card.data);
                card.DestroyCard();
                UpdateRemainingCards();

                yield return new WaitUntil(() => card.destroyed);

                cards.RemoveAt(0);
            }

            busy = false;
        }

        #endregion

        #region Steal Cards

        public void StealCards(List<int> cardsToSteal)
        {
            busy = true;

            int numCardsStart = numCards;

            Contender otherContender = CardGameManager.Instance.GetOtherContender(_contender);

            cardsToSteal.Sort();
            cardsToSteal.Reverse();
            for (int i = 0; i < cardsToSteal.Count; i++)
            {
                GameObject cardObj;
                CardsData data;

                // Take data from scriptable
                int index = cardsToSteal[i];
                data = _deckCards[index];
                _deckCards.RemoveAt(index);

                // Instantiate card
                GameObject cardPrefab = GetCardPrefab(data.type);
                cardObj = Instantiate(cardPrefab, transform.position, cardPrefab.transform.rotation, _hand.transform);

                // Add card to list
                Card card = cardObj.GetComponent<Card>();
                card.Initialize(otherContender, data, cardRevealed: false);
                card.gameObject.SetActive(false);

                _listToAdd.Add(card);
            }

            StartCoroutine(DrawCardsCoroutine(otherContender.hand, numCardsStart));
        }

        #endregion

        #region UI

        private void CheckDeckSprite()
        {
            if (numCards <= 0)
            {
                _spriteRenderer.sprite = null;
            }
        }

        private void UpdateRemainingCards()
        {
            UpdateRemainingCards(numCards);
        }

        private void UpdateRemainingCards(int numCards)
        {
            UIManager.Instance.UpdateRemainingCards(numCards, _maxCardNumber, _contender);
        }

        #endregion

        public List<CardsData> GetDeckCards() { return _deckCards; }
        public GameObject GetCardPrefab(CardType type)
        {
            if (type == CardType.ARGUMENT) return argumentCardPrefab;
            else return actionCardPrefab;
        }
    }
}