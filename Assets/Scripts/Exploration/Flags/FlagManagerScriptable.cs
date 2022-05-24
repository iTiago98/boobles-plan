using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.Flags
{
	[CreateAssetMenu(fileName = "Flag Manager", menuName = "Scriptables/Flag Manager")]
	public class FlagManagerScriptable : ScriptableObject
	{
		public List<Flag> _flags;

		public void InitializeFlags()
        {
			foreach(Flag flag in _flags)
            {
				flag.FlagState = Flag.State.False;
            }
        }

		public bool GetFlag(Flag.Reference flagRef)
        {
			return _flags.Find(flag => flag.FlagReference == flagRef).FlagState == Flag.State.True;
        }

		public void SetFlag(Flag.Reference flagRef)
        {
			_flags.Find(flag => flag.FlagReference == flagRef).FlagState = Flag.State.True;
        }

		public bool FlagsSatisfied(List<Flag.Reference> trueFlags, List<Flag.Reference> falseFlags)
		{
			bool result = true;

			if (trueFlags != null)
			{
				foreach (Flag.Reference reference in trueFlags)
				{
					result = result && (_flags.Find(flag => flag.FlagReference == reference).FlagState == Flag.State.True);
				}
			}

			if(falseFlags != null)
            {
				foreach (Flag.Reference reference in falseFlags)
				{
					result = result && (_flags.Find(flag => flag.FlagReference == reference).FlagState == Flag.State.False);
				}
			}

			return result;
		}
	}
}
