using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CardGame.Cards;

namespace CardGame.Level
{
    public class CardContainer : MonoBehaviour
    {
        public float cardSeparationX;
        public float cardSeparationZ;
        public float animationTime;

        public int numCards => (cards == null) ? 0 : cards.Count;

        public List<GameObject> cards { private set; get; }

        private void Awake()
        {
            cards = new List<GameObject>();
        }

        public void AddCard(Card card, Transform parent)
        {
            card.transform.parent = transform;
            card.moveWithMouse = false;
            card.container = this;

            cards.Add(card.gameObject);
            UpdateCardsPosition();
        }

        public void RemoveCard(GameObject card)
        {
            cards.Remove(card);
            UpdateCardsPosition();
        }

        private void UpdateCardsPosition()
        {
            float posX = CalculateInitialPosition();
            float posZ = 0f;

            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < cards.Count; i++)
            {
                posX += cardSeparationX;
                //posZ += cardSeparationZ;

                MoveCard(sequence, cards[i], new Vector3(posX, 0, posZ));
            }

            sequence.Play();
        }

        private void MoveCard(Sequence seq, GameObject card, Vector3 pos)
        {
            seq.Join(card.transform.DOLocalMove(pos, animationTime));
            seq.Join(card.transform.DORotate(transform.rotation.eulerAngles, animationTime));
        }

        private float CalculateInitialPosition()
        {
            return -(cardSeparationX * (numCards - 1) / 2) - cardSeparationX;
        }

    }
}