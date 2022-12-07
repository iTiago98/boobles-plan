using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;
using CardGame.Managers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Level
{
    public class Deck : MonoBehaviour
    {
        public GameObject cardPrefab;
        public CardsData emptyDeckCardData;

        private int _maxCardNumber;
        private Contender _contender;
        private Hand _hand;
        private List<CardsData> _deckCards;

        public int numCards => _deckCards.Count;

        private List<Card> _listToAdd = new List<Card>();
        private List<Card> _listToDiscard = new List<Card>();

        #region Initialize

        public void Initialize(Contender contender, Hand hand, List<CardsData> deckCards)
        {
            _contender = contender;
            _hand = hand;
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
            TurnManager.Instance.StopFlow();
            int numCardsStart = numCards;

            for (int i = 0; i < numCardsToAdd; i++)
            {
                GameObject cardObj;
                CardsData data;

                if (numCards == 0)
                {
                    // Instantiate card
                    cardObj = Instantiate(cardPrefab, transform.position, cardPrefab.transform.rotation, _hand.transform);

                    // Take data
                    data = new CardsData(emptyDeckCardData);
                }
                else
                {
                    // Instantiate card
                    cardObj = Instantiate(cardPrefab, transform.position, cardPrefab.transform.rotation, _hand.transform);

                    // Take data from scriptable
                    int index = Random.Range(0, numCards);
                    data = _deckCards[index];
                    _deckCards.RemoveAt(index);
                }

                // Add card to hand
                Card card = cardObj.GetComponent<Card>();
                card.Initialize(_contender, data, cardRevealed: false);
                card.gameObject.SetActive(false);

                _listToAdd.Add(card);
            }

            StartCoroutine(DrawCardsCoroutine(numCardsStart));
        }

        private IEnumerator DrawCardsCoroutine(int numCardsStart)
        {
            int loops = _listToAdd.Count;
            for (int i = 0; i < loops; i++)
            {
                Card card = _listToAdd[0];
                card.gameObject.SetActive(true);
                _hand.AddCard(card);

                // Apply end round effects
                drawCardEffectsDelegate?.Invoke();

                UpdateRemainingCards(--numCardsStart);
                CheckDeckSprite();

                yield return new WaitUntil(() => _hand.cardsAtPosition);

                _listToAdd.RemoveAt(0);
            }

            _listToAdd.Clear();

            TurnManager.Instance.ContinueFlow();
        }

        #endregion

        #region Discard Cards

        public void DiscardCards(int numCardsToDiscard)
        {
            TurnManager.Instance.StopFlow();
            int numCardsStart = numCards;

            for (int i = 0; i < numCardsToDiscard; i++)
            {
                if (numCards > 0)
                {
                    Vector3 position = transform.position + new Vector3(0, 0, -0.1f);
                    GameObject cardObj = Instantiate(cardPrefab, position, cardPrefab.transform.rotation);

                    // Take data from scriptable
                    int index = Random.Range(0, numCards);
                    CardsData data = _deckCards[index];
                    _deckCards.RemoveAt(index);

                    Card card = cardObj.GetComponent<Card>();
                    card.Initialize(null, data, cardRevealed: true);
                    card.gameObject.SetActive(false);

                    if (card.HasEffect(SubType.INCOMPARTMENTABLE))
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
                card.Destroy();

                UpdateRemainingCards(--numCardsStart);
                CheckDeckSprite();

                yield return new WaitUntil(() => card == null);

                _listToDiscard.RemoveAt(0);
            }

            _listToDiscard.Clear();

            if (_listToAdd.Count > 0) StartCoroutine(DrawCardsCoroutine(numCardsStart));
            else TurnManager.Instance.ContinueFlow();
        }

        #endregion

        #region Draw Card Effects

        public delegate void DrawCardEffects();
        private DrawCardEffects drawCardEffectsDelegate;

        public void AddDrawCardEffects(DrawCardEffects method)
        {
            if (drawCardEffectsDelegate == null)
            {
                drawCardEffectsDelegate = method;
            }
            else
            {
                drawCardEffectsDelegate += method;
            }
        }

        public void RemoveDrawCardEffect(DrawCardEffects method)
        {
            drawCardEffectsDelegate -= method;
        }

        #endregion

        #region UI

        private void CheckDeckSprite()
        {
            if (numCards <= 0)
            {
                GetComponent<SpriteRenderer>().sprite = null;
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

        public void AddCards(List<Card> cards)
        {
            TurnManager.Instance.StopFlow();
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
                card.Destroy();
                UpdateRemainingCards();

                yield return new WaitUntil(() => card == null);

                cards.RemoveAt(0);
            }

            TurnManager.Instance.ContinueFlow();
        }
    }
}