using System.Collections.Generic;
using UnityEngine;
using Booble.Flags;

namespace Booble.Interactables.Events
{
	public class SetFlags : MonoBehaviour
	{
        [SerializeField] private List<Flag.Reference> _flags;

    	public void StartInteraction()
        {
            foreach(Flag.Reference flagRef in _flags)
            {
                FlagManager.Instance.SetFlag(flagRef);
            }
        }
	}
}