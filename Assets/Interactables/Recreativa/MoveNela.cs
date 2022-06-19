using UnityEngine;
using Booble.Player;

namespace Booble.Interactables.Arcade
{
	public class MoveNela : MonoBehaviour
	{
		[SerializeField] private Transform _toArcade;
		[SerializeField] private Transform _fromArcade;

    	public void MoveToArcade()
		{
			Controller.Instance.transform.position = _toArcade.position;
		}

		public void MoveFromArcade()
		{
			Controller.Instance.transform.position = _fromArcade.position;
		}
	}
}