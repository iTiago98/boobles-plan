using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Managers;
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

        private List<Card> _listToDiscard = new List<Card>();

        public delegate void DrawCardEffects();
        private DrawCardEffects drawCardEffectsDelegate;

        public void Initialize(Contender contender, Hand hand, List<CardsData> deckCards)
        {
            _contender = contender;
            _hand = hand;
            CopyCardsList(deckCards);
            UIManager.Instance.ShowRemainingCards(contender);
            UpdateRemainingCards();
        }

        public void DrawCards(int numCards)
        {
            for (int i = 0; i < numCards; i++)
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
                _hand.AddCard(card);

                // Apply end round effects
                drawCardEffectsDelegate?.Invoke();
            }

            CheckCardNumber();
        }

        public void DiscardCards(int numCardsToDiscard)
        {
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

                    _listToDiscard.Add(card);
                }
            }

            StartCoroutine(DiscardCardsCoroutine(numCardsStart));
        }

        private IEnumerator DiscardCardsCoroutine(int numCardsStart)
        {
            foreach(Card card in _listToDiscard)
            {
                card.gameObject.SetActive(true);
                card.Destroy();
                yield return new WaitUntil(() => card == null);
                UpdateRemainingCards(--numCardsStart);
            }

            _listToDiscard.Clear();
            CheckCardNumber();

            if (TurnManager.Instance.clashing) TurnManager.Instance.continueClash = true; ;
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

        private void CheckCardNumber()
        {
            if (numCards <= 0)
            {
                GetComponent<SpriteRenderer>().sprite = null;
            }

            UpdateRemainingCards();
        }

        private void UpdateRemainingCards()
        {
            UpdateRemainingCards(numCards);
        }

        private void UpdateRemainingCards(int numCards)
        {
            UIManager.Instance.UpdateRemainingCards(numCards, _maxCardNumber, _contender);
        }
    }
}