using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Booble.CardGame.Cards;

namespace Booble.CardGame.Level
{
    public class CardContainer : MonoBehaviour
    {
        [SerializeField] private float cardSeparationX;
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

        public void AddCard(Card card, Transform parent)
        {
            cardsAtPosition = false;
            card.transform.parent = transform;
            card.SetContainer(this);

            cards.Add(card.gameObject);
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
            float posX = CalculateInitialPosition();
            float posZ = 0f;

            if (updateCardsPositionSequence != null)
            {
                updateCardsPositionSequence.Kill();
            }

            updateCardsPositionSequence = DOTween.Sequence();

            for (int i = 0; i < cards.Count; i++)
            {
                posX += cardSeparationX;

                MoveCard(updateCardsPositionSequence, cards[i], new Vector3(posX, 0, posZ), i == cards.Count - 1);
            }

            updateCardsPositionSequence.OnComplete(() =>
            {
                cardsAtPosition = true;
                updateCardsPositionSequence = null;
            });

            updateCardsPositionSequence.Play();
        }

        private void MoveCard(Sequence seq, GameObject card, Vector3 pos, bool last)
        {
            seq.Join(card.transform.DOLocalMoveX(pos.x, animationTime));
            seq.Join(card.transform.DOLocalMoveY(pos.y, animationTime));
            if (last) seq.Append(card.transform.DOLocalMoveZ(pos.z, animationTime));
            else seq.Join(card.transform.DOLocalMoveZ(pos.z, animationTime));
        }

        private float CalculateInitialPosition()
        {
            return -(cardSeparationX * (numCards - 1) / 2) - cardSeparationX;
        }

    }
}