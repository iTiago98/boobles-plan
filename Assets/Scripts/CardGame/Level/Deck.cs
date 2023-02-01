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

            InitializeCards(deckCards);
        }

        private void InitializeCards(List<CardsData> deckCards)
        {
            CopyCardsList(deckCards);
            UIManager.Instance.ShowRemainingCards(_contender);
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

        public void DrawCards(List<string> names)
        {
            busy = true;
            int numCardsStart = numCards;

            foreach (string name in names)
            {
                int index = GetIndexFromName(name);
                CardsData data = _deckCards[index];
                _deckCards.RemoveAt(index);

                GameObject cardPrefab = GetCardPrefab(data.type);
                GameObject cardObj = Instantiate(cardPrefab, transform.position, cardPrefab.transform.rotation, _hand.transform);

                Card card = cardObj.GetComponent<Card>();
                card.Initialize(_contender, data, cardRevealed: false);
                card.gameObject.SetActive(false);

                _listToAdd.Add(card);
            }

            StartCoroutine(DrawCardsCoroutine(_hand, numCardsStart));
        }

        private int GetIndexFromName(string name)
        {
            for (int i = 0; i < numCards; i++)
            {
                CardsData data = _deckCards[i];

                if (data.name == name) return i;
            }

            return -1;
        }

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

                if (CardEffectsManager.Instance.HasDrawCardEffects)
                    CardEffectsManager.Instance.ApplyDrawCardEffects();

                UpdateRemainingCards(--numCardsStart);

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

            List<int> indexList = GetIndexList();

            int number = Mathf.Min(numCardsToDiscard, indexList.Count);

            for (int i = 0; i < number; i++)
            {
                // Take data from scriptable
                int index = indexList[Random.Range(0, indexList.Count)];
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
                    int indexPosition = indexList.IndexOf(index);
                    indexList.Remove(index);
                    UpdateIndexList(ref indexList, indexPosition);
                }
            }

            StartCoroutine(DiscardCardsCoroutine(numCardsStart));
        }

        private List<int> GetIndexList()
        {
            List<int> indexList = new List<int>();
            for (int j = 0; j < numCards; j++) indexList.Add(j);

            if (HasAlternateWinConditionCard())
            {
                indexList.Remove(GetAlternateWinConditionCardIndex());
            }

            return indexList;
        }

        private void UpdateIndexList(ref List<int> indexList, int indexRemoved)
        {
            for (int i = 0; i < indexList.Count; i++)
            {
                if (i >= indexRemoved)
                {
                    indexList[i]--;
                }
            }
        }

        private IEnumerator DiscardCardsCoroutine(int numCardsStart)
        {
            int loops = _listToDiscard.Count;

            for (int i = 0; i < loops; i++)
            {
                Card card = _listToDiscard[0];
                card.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                card.DestroyCard();

                UpdateRemainingCards(--numCardsStart);

                yield return new WaitUntil(() => card.destroyed);

                _listToDiscard.RemoveAt(0);
            }

            _listToDiscard.Clear();

            if (_listToAdd.Count > 0) StartCoroutine(DrawCardsCoroutine(_hand, numCardsStart));
            else busy = false;
        }

        #endregion

        #region Add Cards

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

                yield return new WaitForSeconds(1f);

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

            Contender otherContender = CardGameManager.Instance.GetOtherContender(_contender);
            List<Card> cardsToAdd = new List<Card>();

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
                cardObj = Instantiate(cardPrefab, transform.position, cardPrefab.transform.rotation);

                // Add card to list
                Card card = cardObj.GetComponent<Card>();
                card.Initialize(otherContender, data, cardRevealed: false);
                card.gameObject.SetActive(false);

                cardsToAdd.Add(card);
            }

            otherContender.deck.AddCards(cardsToAdd);
            busy = false;
            //StartCoroutine(AddCardCoroutine(cardsToAdd));
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
            CheckDeckSprite();
        }

        #endregion

        public List<CardsData> GetDeckCards() { return _deckCards; }
        public GameObject GetCardPrefab(CardType type)
        {
            if (type == CardType.ARGUMENT) return argumentCardPrefab;
            else return actionCardPrefab;
        }

        public bool HasAlternateWinConditionCard()
        {
            foreach (CardsData card in _deckCards)
            {
                if (IsAlternateWinConditionCard(card)) return true;
            }

            return false;
        }

        private int GetAlternateWinConditionCardIndex()
        {
            foreach (CardsData card in _deckCards)
            {
                if (IsAlternateWinConditionCard(card)) return _deckCards.IndexOf(card);
            }

            return -1;
        }

        private bool IsAlternateWinConditionCard(CardsData data)
        {
            return data.type == CardType.ACTION && data.effects.Count > 0 && data.effects[0].type == EffectType.ALTERNATE_WIN_CONDITION;
        }
    }
}