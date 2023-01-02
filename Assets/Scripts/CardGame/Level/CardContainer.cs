using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Booble.CardGame.Cards;

namespace Booble.CardGame.Level
{
    abstract public class CardContainer : MonoBehaviour
    {
        [SerializeField] protected float cardSeparationX;
        [SerializeField] private float animationTime;

        public bool cardsAtPosition { private set; get; }

        public int numCards => (cards == null) ? 0 : cards.Count;
        public bool isEmpty => numCards == 0;
        public bool isNotEmpty => numCards > 0;

        public List<GameObject> cards { private set; get; }

        private void Awake()
        {
            cards = new List<GameObject>();
        }

        public void AddCard(GameObject cardObj)
        {
            cardsAtPosition = false;
            cardObj.transform.parent = transform;

            Card card = cardObj.GetComponent<Card>();
            if (card != null) card.SetContainer(this);

            cards.Add(cardObj);
            UpdateCardsPosition();
        }

        public void RemoveCard(GameObject card)
        {
            cardsAtPosition = false;
            cards.Remove(card);
            UpdateCardsPosition();
        }

        Sequence updateCardsPositionSequence = null;

        private void UpdateCardsPosition()
        {
            float initialX = CalculateInitialPosition();
            float posZ = 0f;

            if (updateCardsPositionSequence != null)
            {
                updateCardsPositionSequence.Kill();
            }

            updateCardsPositionSequence = DOTween.Sequence();

            for (int i = 0; i < cards.Count; i++)
            {
                float posX = GetXPosition(i, initialX);
                float posY = GetYPosition(i);

                MoveCard(updateCardsPositionSequence, cards[i], new Vector3(posX, posY, posZ), i == cards.Count - 1);
            }

            updateCardsPositionSequence.OnComplete(() =>
            {
                cardsAtPosition = true;
                updateCardsPositionSequence = null;
            });

            updateCardsPositionSequence.Play();
        }

        virtual protected float GetXPosition(int index, float initialPosition)
        {
            return initialPosition + cardSeparationX * index;
        }

        virtual protected float GetYPosition(int index) { return 0f; }

        virtual protected float CalculateInitialPosition()
        {
            return -((float)numCards - 1) / 2 * cardSeparationX;
        }

        private void MoveCard(Sequence seq, GameObject card, Vector3 pos, bool last)
        {
            seq.Join(card.transform.DOLocalMoveX(pos.x, animationTime));
            seq.Join(card.transform.DOLocalMoveY(pos.y, animationTime));
            if (last) seq.Append(card.transform.DOLocalMoveZ(pos.z, animationTime));
            else seq.Join(card.transform.DOLocalMoveZ(pos.z, animationTime));
        }
    }
}