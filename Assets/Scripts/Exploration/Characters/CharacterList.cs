using System.Collections.Generic;
using UnityEngine;

namespace Booble.Characters
{
	[CreateAssetMenu(fileName = "NewCharacterList", menuName = "Scriptables/CharacterList")]
	public class CharacterList : ScriptableObject
	{
		public enum Name
        {
			None,
			Nela,
			Ana,
			Citriano,
			Dennis,
			Arcadio,
			Queca,
			eMilin,
			F3RR4N,
			Anselmo,
			Pedro,
			Pablo,
			Romualdez,
			Rosalinda,
			RomualdezExplicito
        }
		
		[System.Serializable]
		public class Character
        {
			public Name Identifier => _identifier;
			public Sprite CloseUp => _closeUp;

			[SerializeField] private string _name;
			[SerializeField] private Name _identifier;
			[SerializeField] private Sprite _closeUp;
        }

		[SerializeField] private List<Character> _characters;

		public Character GetCharacter(Name identifier)
        {
            //foreach (Character ch in _characters)
            //{
            //    Debug.Log("iter: " + ch.Identifier + ", match: " + identifier + " (" + (ch.Identifier == identifier) + ")");
            //    if (ch.Identifier == identifier)
            //    {
            //        Debug.Log("found");
            //        return ch.CloseUp;
            //    }
            //}
            return _characters.Find(ch => ch.Identifier == identifier);
        }
	}
}