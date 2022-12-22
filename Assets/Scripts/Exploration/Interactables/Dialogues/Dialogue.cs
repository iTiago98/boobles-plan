using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Booble.Characters;
using UnityEditor;

namespace Booble.Interactables.Dialogues
{
	[CreateAssetMenu(fileName = "NewDialogue", menuName = "Scriptables/Dialogue")]
	public class Dialogue : ScriptableObject
	{
		private const int MAX_CHAR_COUNT = 100;
		
		[System.Serializable]
		public class Sentence
        {
	        public string Content
	        {
		        get
		        {
			        return _content;
		        }
		        
		        set
		        {
			        _content = value;
		        }
	        }
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

		public string GetLastSentence(out CharacterList.Character character)
        {
			_currentIndex = _sentences.Count - 1;
			character = _characters.GetCharacter(_sentences[_currentIndex].CloseUp);
			return _sentences[_currentIndex].Content;
        }

		[MenuItem("Assets/Dialogue/Break Apart")]
		public void BreakApart()
		{
			int i = 0;
			while (i < _sentences.Count)
			{
				if (_sentences[i].Content.Length > MAX_CHAR_COUNT)
				{
					_sentences.Insert(i+1, _sentences[i]);
					int separatorIndex = FindSeparatorIndex(_sentences[i].Content);
					_sentences[i].Content = _sentences[i].Content.Substring(0, separatorIndex+1);
					_sentences[i + 1].Content = _sentences[i + 1].Content.Substring(separatorIndex + 1);
				}

				i++;
			}
		}

		private int FindSeparatorIndex(string content)
		{
			int i = MAX_CHAR_COUNT-1;
			while (content[i] != '.' && content[i] != '!' && content[i] != '?')
			{
				i--;
			}

			return i;
		}
	}
}
