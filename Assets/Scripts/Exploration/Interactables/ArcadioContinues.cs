using Booble.Flags;
using Booble.Interactables.Dialogues;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.Interactables
{
	public class ArcadioContinues : MonoBehaviour
	{
        public int CoinCount => _coinCount - 1;

		[SerializeField] private Interactable _interactable;
        [SerializeField] private List<Flag.Reference> _flagRefs;
        [SerializeField] private List<Dialogue> _dialogues;

		private int _coinCount
        {
            get
            {
                int value = 0;
                foreach(Flag.Reference f in _flagRefs)
                {
                    if(FlagManager.Instance.GetFlag(f))
                    {
                        value++;
                    }
                }
                return value;
            }
        }

        private void Start()
        {
            UpdateContinueDialogue();
        }

        public void UpdateContinueDialogue()
        {
            int cc = _coinCount;
            if (cc > 0)
            {
                _interactable.ChangeDialogue(_dialogues[cc - 1]);
            }
        }
    }
}