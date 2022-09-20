using System.Collections.Generic;
using UnityEngine;
using Booble.Flags;

namespace Booble.Interactables.Events
{
	public class SetFlags : DialogueEvent
	{
        [SerializeField] private List<Flag.Reference> _flags;

    	public override void Execute()
        {
            foreach(Flag.Reference flagRef in _flags)
            {
                FlagManager.Instance.SetFlag(flagRef);
            }
        }
	}
}