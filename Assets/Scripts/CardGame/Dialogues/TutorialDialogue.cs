using Booble.CardGame.Cards;
using Booble.CardGame.Managers;
using Booble.Interactables.Dialogues;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Dialogues
{
    public class TutorialDialogue : InterviewDialogue
    {
        [Header("Extra dialogues")]
        [SerializeField] private Dialogue _tutorialDialogue;
        [SerializeField] private Dialogue _postTutorialDialogue;

        [Header("Options")]
        [SerializeField] private List<Option> _tutorialOptions;
        [SerializeField] private List<Option> _postTutorialOptions;

        [Header("Extra cards")]
        [SerializeField] private Dialogue _granFinalDialogue;

        public override void ThrowStartDialogue()
        {
            if (_startDialogue != null && _dialogueManager != null)
            {
                ThrowDialogue(_startDialogue, null, _tutorialOptions);
            }
            else
                CardGameManager.Instance.StartGame(); // TEMPORAL
        }

        public void ThrowTutorial()
        {
            Debug.Log("Tutorial");
            ThrowDialogue(_tutorialDialogue, null, _postTutorialOptions);
        }

        public void ThrowPostTutorial()
        {
            Debug.Log("PostTutorial");
            ThrowDialogue(_postTutorialDialogue, CardGameManager.Instance.StartGame);
        }

        override public void CheckDialogue(Card cardPlayed)
        {
            if (cardPlayed.name.Contains("Gran final")) ThrowDialogue(_granFinalDialogue);
        }
    }
}