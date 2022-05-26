using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Booble.Characters;

namespace Booble.Interactables.Dialogues
{
	[CreateAssetMenu(fileName = "NewDialogue", menuName = "Scriptables/Dialogue")]
	public class Dialogue : ScriptableObject
	{
		[System.Serializable]
		public class Sentence
        {
			public string Content => _content;
			public CharacterList.Name CloseUp => _closeUp;

			[TextArea]
			[SerializeField] private string _content;
			//[SerializeField] private string _speaker;
			[SerializeField] private CharacterList.Name _closeUp;

			//public string PrintSentence()
   //         {
			//	return (_speaker != "" ? (_speaker + ": ") : "") + _content;
   //         }
        }

		public bool Empty => _sentences.Count == 0;

		[SerializeField] private CharacterList _characters;
		[SerializeField] private List<Sentence> _sentences;

		private int _currentIndex;

		public bool GetNextSentence(out string sentence, out CharacterList.Character character, bool firstSentence = false)
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
				//CharacterList _characters = ScriptableObject.CreateInstance<CharacterList>();
				character = _characters.GetCharacter(_sentences[_currentIndex].CloseUp);
				
				sentence = _sentences[_currentIndex].Content;
				
				return true;
            }
			else
            {
				sentence = "END OF DIALOGUE";
				character = null;
				return false;
            }
		}

		public string GetLastSentence()
        {
			_currentIndex = _sentences.Count - 1;
			return _sentences[_currentIndex].Content;
        }
	}
}
