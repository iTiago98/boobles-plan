using Booble.CardGame.Cards;
using Booble.CardGame.Managers;
using Booble.Interactables.Dialogues;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Dialogues
{
    public class TutorialDialogue : InterviewDialogue
    {
        [SerializeField] private TutorialAnimation _tutorialAnimation;

        public TutorialAnimation GetTutorialAnimation() => _tutorialAnimation;

        [Header("Extra dialogues")]
        [SerializeField] private Dialogue _tutorialDialogue;
        [SerializeField] private Dialogue _repeatTutorialDialogue;
        [SerializeField] private Dialogue _postTutorialDialogue;

        [Header("Options")]
        [SerializeField] private List<Option> _tutorialOptions;
        [SerializeField] private List<Option> _repeatTutorialOptions;
        [SerializeField] private List<Option> _postTutorialOptions;

        [Header("Extra cards")]
        [SerializeField] private Dialogue _granFinalCardDialogue;

        private bool _granFinalCardDialogueShown;


        public override void ThrowStartDialogue()
        {
            ThrowDialogue(_startDialogue, null, _tutorialOptions, hideBackOption: true);
        }

        public void ThrowTutorial()
        {
            Debug.Log("Tutorial");
            StartCoroutine(ThrowTutorialCoroutine());
        }

        private IEnumerator ThrowTutorialCoroutine()
        {
            _tutorialAnimation.StartTutorial();
            yield return new WaitWhile(() => CardGameManager.Instance.tutorial);

            ThrowDialogue(_tutorialDialogue, null, _postTutorialOptions, hideBackOption: true);
        }

        public void ThrowRepeatTutorial()
        {
            Debug.Log("RepeatTutorial");
            ThrowDialogue(_repeatTutorialDialogue, null, _repeatTutorialOptions, hideBackOption: true);
        }

        public void ThrowPostTutorial()
        {
            Debug.Log("PostTutorial");
            ThrowDialogue(_postTutorialDialogue, CardGameManager.Instance.StartGame);
        }

        override public bool CheckDialogue(Card cardPlayed)
        {
            if (cardPlayed.name.Contains("Gran final") && !_granFinalCardDialogueShown)
            {
                ThrowDialogue(_granFinalCardDialogue);
                _granFinalCardDialogueShown = true;
                return true;
            }

            return false;
        }
    }
}