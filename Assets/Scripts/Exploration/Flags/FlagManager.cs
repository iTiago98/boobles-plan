using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Santi.Utils;

namespace Booble.Flags
{
	public class FlagManager : Singleton<FlagManager>
	{
		[SerializeField] private FlagManagerScriptable _flagManager;

		[ContextMenu("Initialize Flags")]
		public void InitializeFlags()
        {
			_flagManager.InitializeFlags();
        }

		public bool GetFlag(Flag.Reference flagRef)
        {
			return _flagManager.GetFlag(flagRef);
        }

		public void SetFlag(Flag.Reference flagRef)
        {
			_flagManager.SetFlag(flagRef);
        }

		public bool FlagsSatisfied(List<Flag.Reference> trueFlags, List<Flag.Reference> falseFlags)
        {
			return _flagManager.FlagsSatisfied(trueFlags, falseFlags);
        }
	}
}
