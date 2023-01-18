using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Santi.Utils;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using Booble.Managers;
using Booble.Interactables.Dialogues;

namespace Booble.Flags
{
	public class FlagManager : Singleton<FlagManager>
	{
		[SerializeField] private bool _deleteSaveFileOnAwake;

        private void Awake()
        {
#if UNITY_EDITOR
			if (_deleteSaveFileOnAwake)
			{
				ResetFlags();
			}
#endif
        }

        [ContextMenu("Reset Flags")]
        public void ResetFlags()
        {
	        Debug.Log("RESET FLAGS"); 
	        PlayerPrefs.DeleteAll();
			PlayerConfig.SetPlayerPrefs();
        }

		public bool GetFlag(Flag.Reference flagRef)
        { 
			if(PlayerPrefs.HasKey(flagRef.ToString()))
			{
				return PlayerPrefs.GetInt(flagRef.ToString()) == (int)Flag.State.True;
			}
			
			return false;
        }

		public void SetFlag(Flag.Reference flagRef, bool toTrue = true)
        {
			if(toTrue)
			{
				PlayerPrefs.SetInt(flagRef.ToString(), (int)Flag.State.True);
			}
			else
			{
				PlayerPrefs.SetInt(flagRef.ToString(), (int)Flag.State.False);
			}
        }

		public bool FlagsSatisfied(List<Flag.Reference> trueFlags, List<Flag.Reference> falseFlags)
        {
			bool result = true;

			if (trueFlags != null)
			{
				foreach (Flag.Reference reference in trueFlags)
				{
					result = result && GetFlag(reference);
				}
			}

			if (falseFlags != null)
			{
				foreach (Flag.Reference reference in falseFlags)
				{
					result = result && !GetFlag(reference);
				}
			}

			return result;
		}
    }
}
