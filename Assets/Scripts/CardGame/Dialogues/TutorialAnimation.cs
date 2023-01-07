using Booble.CardGame.Cards;
using Booble.CardGame.Level;
using Booble.CardGame.Managers;
using Booble.Interactables.Dialogues;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Dialogues
{
    public class TutorialAnimation : MonoBehaviour
    {
        [Header("Dialogues")]
        [SerializeField] private Dialogue _introduction;
        [SerializeField] private Dialogue _healthBarExplanation;
        [SerializeField] private Dialogue _manaCounterExplanation;
        [SerializeField] private Dialogue _drawCardExplanation;
        [SerializeField] private Dialogue _drawCardExplanation2;
        [SerializeField] private Dialogue _drawCardExplanation3;
        [SerializeField] private Dialogue _drawCardExplanation4;
        [SerializeField] private Dialogue _playCardExplanation;
        [SerializeField] private Dialogue _playCardExplanation2;
        [SerializeField] private Dialogue _clashExplanation;
        [SerializeField] private Dialogue _combatEffectsExplanation;
        [SerializeField] private Dialogue _betterCardsExplanation;
        [SerializeField] private Dialogue _enterEffectsExplanation;
        [SerializeField] private Dialogue _actionsExplanation;
        [SerializeField] private Dialogue _actionsExplanation2;
        [SerializeField] private Dialogue _fieldCardsExplanation;
        [SerializeField] private Dialogue _fieldCardsExplanation2;
        [SerializeField] private Dialogue _endExplanation;

        private DialogueManager _dialogueManager;

        private Contender _player;
        private Contender _opponent;

        private bool _continue;
        private bool _coroutine;

        private void Continue() => _continue = true;

        private List<Card> _cards = new List<Card>();

        public void StartTutorial()
        {
            _dialogueManager = DialogueManager.Instance;

            CardGameManager.Instance.StartTutorial();

            StartCoroutine(TutorialCoroutine());
        }

        private IEnumerator TutorialCoroutine()
        {
            _player = CardGameManager.Instance.player;
            _opponent = CardGameManager.Instance.opponent;

            _coroutine = true;
            StartCoroutine(IntroductionCoroutine());
            yield return new WaitWhile(() => _coroutine);

            _coroutine = true;
            StartCoroutine(DrawCardCoroutine());
            yield return new WaitWhile(() => _coroutine);

            _coroutine = true;
            StartCoroutine(PlayCardCoroutine());
            yield return new WaitWhile(() => _coroutine);

            _coroutine = true;
            StartCoroutine(ClashCoroutine(_combatEffectsExplanation));
            yield return new WaitWhile(() => _coroutine);

            _coroutine = true;
            StartCoroutine(TurnCoroutine(_opponent, "Razonamiento coherente", "Afirmación reconfortante", 3));
            yield return new WaitWhile(() => _coroutine);

            _coroutine = true;
            StartCoroutine(TurnCoroutine(_player, "Técnicamente correcto", "Técnicamente correcto", 2, _betterCardsExplanation));
            yield return new WaitWhile(() => _coroutine);

            _coroutine = true;
            StartCoroutine(ClashCoroutine());
            yield return new WaitWhile(() => _coroutine);

            _coroutine = true;
            StartCoroutine(TurnCoroutine(_opponent, "Y por ende...", "Y por ende...", 1, null, null, _enterEffectsExplanation));
            yield return new WaitWhile(() => _coroutine);

            _coroutine = true;
            StartCoroutine(TurnCoroutine(_player, "Tu argumento claritamente es inválido", "Tu argumento claritamente es inválido", 1, _actionsExplanation, _actionsExplanation2));
            yield return new WaitWhile(() => _coroutine);

            _coroutine = true;
            StartCoroutine(ClashCoroutine());
            yield return new WaitWhile(() => _coroutine);

            _coroutine = true;
            StartCoroutine(TurnCoroutine(_opponent, "Técnicamente correcto", "Técnicamente correcto", 4));
            yield return new WaitWhile(() => _coroutine);

            _coroutine = true;
            StartCoroutine(TurnCoroutine(_player, "Cambiar la perspectiva", "Hablar con convencimiento", -1, _fieldCardsExplanation));
            yield return new WaitWhile(() => _coroutine);

            _coroutine = true;
            StartCoroutine(ClashCoroutine(null, _fieldCardsExplanation2));
            yield return new WaitWhile(() => _coroutine);

            _player.InitializeStats();
            _opponent.InitializeStats();
            UIManager.Instance.UpdateUIStats(hideEmptyCristals: true);

            CardGameManager.Instance.InitializeDecks();

            Card card = _player.cardZones[1].GetCard();
            card.DestroyCard();
            _player.fieldCardZone.GetCard().DestroyCard();
            _opponent.cardZones[2].GetCard().DestroyCard();
            _opponent.cardZones[3].GetCard().DestroyCard();

            _player.hand.DiscardAll();
            _opponent.hand.DiscardAll();

            yield return new WaitUntil(() => card.destroyed);
            yield return new WaitWhile(() => _player.hand.busy || _opponent.hand.busy);
            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            StartDialogue(_endExplanation);
            yield return new WaitUntil(() => _continue);
            yield return new WaitForSeconds(0.5f);

            CardGameManager.Instance.FinishTutorial();
        }

        private IEnumerator IntroductionCoroutine()
        {
            StartDialogue(_introduction);
            yield return new WaitUntil(() => _continue);

            UIManager.Instance.TurnAnimation(TurnManager.Turn.INTERVIEW_START);
            yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

            UIManager.Instance.TurnAnimation(TurnManager.Turn.ROUND_START);
            yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

            _player.FillMana();
            _opponent.FillMana();
            UIManager.Instance.UpdateUIStats();
            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            UIManager.Instance.GetPlayerHealthBar().transform.DOScale(2, 0.5f);
            yield return new WaitForSeconds(0.5f);

            StartDialogue(_healthBarExplanation);
            yield return new WaitUntil(() => _continue);

            UIManager.Instance.GetPlayerHealthBar().transform.DOScale(1, 0.5f);
            yield return new WaitForSeconds(0.5f);

            _player.SetFullMana();
            UIManager.Instance.UpdateUIStats();
            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            UIManager.Instance.GetPlayerManaCounter().transform.DOScale(2, 0.5f);
            yield return new WaitForSeconds(0.5f);

            StartDialogue(_manaCounterExplanation);
            yield return new WaitUntil(() => _continue);

            _player.SetMana(1);
            UIManager.Instance.UpdateUIStats(hideEmptyCristals: true);

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            UIManager.Instance.GetPlayerManaCounter().transform.DOScale(1, 0.5f);
            yield return new WaitForSeconds(0.5f);

            _coroutine = false;
        }

        private IEnumerator DrawCardCoroutine()
        {
            _player.deck.DrawCards(new List<string>()
            {
                "Afirmación reconfortante",
                "Premisa",
                "Ganar tiempo con una falacia",
                "Hablar con convencimiento"
            });

            _opponent.deck.DrawCards(new List<string>()
            {
                "Afirmación reconfortante",
                "Premisa",
                "Ganar tiempo con una falacia",
                "Hablar con convencimiento"
            });

            yield return new WaitWhile(() => _player.deck.busy || _opponent.deck.busy);

            UIManager.Instance.TurnAnimation(TurnManager.Turn.OPPONENT);
            yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

            _opponent.deck.DrawCards(new List<string>() { "Técnicamente correcto" });
            yield return new WaitWhile(() => _opponent.deck.busy);

            StartDialogue(_drawCardExplanation);
            yield return new WaitUntil(() => _continue);
            yield return new WaitForSeconds(0.5f);

            _player.hand.GetCard("Afirmación reconfortante").transform.DOScale(0.5f, 0.5f);

            StartDialogue(_drawCardExplanation2);
            yield return new WaitUntil(() => _continue);
            yield return new WaitForSeconds(0.5f);

            _player.hand.GetCard("Afirmación reconfortante").transform.DOScale(0.41f, 0.5f);
            _player.hand.GetCard("Ganar tiempo con una falacia").transform.DOScale(0.5f, 0.5f);

            StartDialogue(_drawCardExplanation3);
            yield return new WaitUntil(() => _continue);
            yield return new WaitForSeconds(0.5f);

            _player.hand.GetCard("Ganar tiempo con una falacia").transform.DOScale(0.41f, 0.5f);
            _player.hand.GetCard("Hablar con convencimiento").transform.DOScale(0.5f, 0.5f);

            StartDialogue(_drawCardExplanation4);
            yield return new WaitUntil(() => _continue);
            yield return new WaitForSeconds(0.5f);

            _player.hand.GetCard("Hablar con convencimiento").transform.DOScale(0.41f, 0.5f);
            yield return new WaitForSeconds(1f);

            CardZone cardZone = PlayCard(_opponent.hand.GetCard("Premisa"), 2);
            yield return new WaitWhile(() => CardGameManager.Instance.playingCard);

            _coroutine = false;
        }

        private IEnumerator PlayCardCoroutine()
        {
            UIManager.Instance.TurnAnimation(TurnManager.Turn.PLAYER);
            yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

            _player.deck.DrawCards(new List<string>() { "Razonamiento coherente" });
            yield return new WaitWhile(() => _player.deck.busy);

            StartDialogue(_playCardExplanation);
            yield return new WaitUntil(() => _continue);
            yield return new WaitForSeconds(0.5f);

            CardGameManager.Instance.EnableMouseController();
            
            Card card = _player.hand.GetCard("Afirmación reconfortante");
            card.transform.DOScale(0.5f, 0.5f);

            yield return new WaitUntil(() => card.IsInWaitingSpot);

            CardGameManager.Instance.DisableMouseController();

            card.transform.DOScale(0.41f, 0.5f);
            _player.cardZones[1].ShowHighlight(true);

            StartDialogue(_playCardExplanation2);
            yield return new WaitUntil(() => _continue);
            yield return new WaitForSeconds(0.5f);

            CardGameManager.Instance.EnableMouseController();

            yield return new WaitWhile(() => MouseController.Instance.IsHoldingCard);
            
            _player.cardZones[1].ShowHighlight(false);
            CardGameManager.Instance.DisableMouseController();

            yield return new WaitWhile(() => CardGameManager.Instance.playingCard);

            StartDialogue(_clashExplanation);
            yield return new WaitUntil(() => _continue);
            yield return new WaitForSeconds(0.5f);

            _coroutine = false;
        }

        private IEnumerator TurnCoroutine(Contender contender, string cardDrawed, string cardPlayed, int zone,
            Dialogue dialogueAfterDraw = null, Dialogue dialogueAfterWaitingSpot = null, Dialogue dialogueAfterPlay = null)
        {
            if (contender.isPlayer) UIManager.Instance.TurnAnimation(TurnManager.Turn.PLAYER);
            else UIManager.Instance.TurnAnimation(TurnManager.Turn.OPPONENT);

            yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

            contender.deck.DrawCards(new List<string>()
            {
                cardDrawed
            });

            yield return new WaitWhile(() => contender.deck.busy);

            if (dialogueAfterDraw != null)
            {
                StartDialogue(dialogueAfterDraw);
                yield return new WaitUntil(() => _continue);
                yield return new WaitForSeconds(0.5f);
            }

            Card card = contender.hand.GetCard(cardPlayed);

            if (dialogueAfterWaitingSpot != null)
            {
                card.MoveToWaitingSpot();
                if (card.IsArgument || card.IsField)
                    Board.Instance.HighlightCardZones(card, true);
                else
                    Board.Instance.HighlightTargets(card.effect.FindPossibleTargets());

                yield return new WaitForSeconds(1f);
                yield return new WaitUntil(() => contender.hand.cardsAtPosition);

                StartDialogue(dialogueAfterWaitingSpot);
                yield return new WaitUntil(() => _continue);
                yield return new WaitForSeconds(0.5f);

                if (card.IsArgument || card.IsField)
                    Board.Instance.RemoveCardZonesHighlight(card);
                else
                    Board.Instance.RemoveTargetsHighlight();
            }

            if (card.IsArgument || card.IsField)
            {
                CardZone cardZone = PlayCard(card, zone);
                yield return new WaitWhile(() => CardGameManager.Instance.playingCard);
            }
            else
            {
                Card target = CardGameManager.Instance.GetOtherContender(contender).cardZones[zone - 1].GetCard();
                card.ContinueAction(target);
                yield return new WaitUntil(() => card.destroyed);
            }

            if (dialogueAfterPlay != null)
            {
                StartDialogue(dialogueAfterPlay);
                yield return new WaitUntil(() => _continue);
                yield return new WaitForSeconds(0.5f);
            }

            _coroutine = false;
        }

        private IEnumerator ClashCoroutine(Dialogue dialogueAfterClash = null, Dialogue dialogueAfterRound = null)
        {
            UIManager.Instance.TurnAnimation(TurnManager.Turn.CLASH);
            yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

            TurnManager.Instance.GetClashManager().Clash(changeTurn: false);
            yield return new WaitWhile(() => TurnManager.Instance.GetClashManager().clash);
            yield return new WaitForSeconds(0.5f);

            if (dialogueAfterClash != null)
            {
                StartDialogue(dialogueAfterClash);
                yield return new WaitUntil(() => _continue);
            }

            UIManager.Instance.TurnAnimation(TurnManager.Turn.ROUND_END);
            yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

            CardEffectsManager.Instance.ApplyEndTurnEffects(changeTurn: false);
            yield return new WaitUntil(() => CardEffectsManager.Instance.effectsApplied);

            if (dialogueAfterRound != null)
            {
                StartDialogue(dialogueAfterRound);
                yield return new WaitUntil(() => _continue);
            }

            UIManager.Instance.TurnAnimation(TurnManager.Turn.ROUND_START);
            yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

            CardGameManager.Instance.FillMana();
            UIManager.Instance.UpdateUIStats();

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            _coroutine = false;
        }

        private void StartDialogue(Dialogue dialogue)
        {
            _continue = false;
            _dialogueManager.StartDialogue(dialogue);
            _dialogueManager.OnEndDialogue.RemoveAllListeners();
            _dialogueManager.OnEndDialogue.AddListener(Continue);
        }

        private CardZone PlayCard(Card card, int zone)
        {
            CardZone cardZone;
            if (zone == -1) cardZone = card.contender.fieldCardZone;
            else cardZone = card.contender.cardZones[zone - 1];

            card.Play(cardZone);

            return cardZone;
        }


    }
}