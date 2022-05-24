using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.Interactables.Dialogues
{
	[CreateAssetMenu(fileName = "NewDialogue", menuName = "Scriptables/Dialogue")]
	public class Dialogue : ScriptableObject
	{
		[System.Serializable]
		public class Sentence
        {
			[TextArea]
			[SerializeField] private string _content;
			[SerializeField] private string _speaker;

			public string PrintSentence()
            {
				return (_speaker != "" ? (_speaker + ": ") : "") + _content;
            }
        }

		public bool Empty => _sentences.Count == 0;

		[SerializeField] private List<Sentence> _sentences;

		private int _currentIndex;

		public bool GetNextSentence(out string sentence, bool firstSentence = false)
        {
			if(firstSentence)
            {
				_currentIndex = 0;
            }
            else
            {
				_currentIndex++;
            }

			if(_currentIndex < _sentences.Count)
            {
				sentence = _sentences[_currentIndex].PrintSentence();
				return true;
            }
			else
            {
				sentence = "END OF DIALOGUE";
				return false;
            }
		}

		public string GetLastSentence()
        {
			_currentIndex = _sentences.Count - 1;
			return _sentences[_currentIndex].PrintSentence();
        }
	}
}
