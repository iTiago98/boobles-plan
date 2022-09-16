using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Santi.Utils;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace Booble.Flags
{
	public class FlagManager : Singleton<FlagManager>
	{
		// public static string DataDirectory { get { return "Data"; } }
		// public static string FlagFilePath { get { return DataDirectory+"/flags.dat"; } }

		[SerializeField] private bool _deleteSaveFileOnAwake;

		// private FlagList _flags;

        private void Awake()
        {
#if UNITY_EDITOR
			// if (_deleteSaveFileOnAwake) DeleteSaveFile();
			if(_deleteSaveFileOnAwake) PlayerPrefs.DeleteAll();
#endif

			// if(File.Exists(FlagFilePath))
            // {
			// 	LoadFlags();
            // }
			// else
            // {
			// 	Directory.CreateDirectory(DataDirectory);
			// 	_flags = new FlagList();
			// 	SaveFlags();
            // }
        }

        // private void LoadFlags()
        // {
		// 	using (FileStream fs = new FileStream(FlagFilePath, FileMode.Open))
		// 	{
		// 		BinaryFormatter bf = new BinaryFormatter();
		// 		_flags = (FlagList)bf.Deserialize(fs);
		// 	}
		// }

        // private void SaveFlags()
        // {
		// 	using (FileStream fs = new FileStream(FlagFilePath, FileMode.Create))
		// 	{
		// 		BinaryFormatter bf = new BinaryFormatter();
		// 		bf.Serialize(fs, _flags);
		// 	}
		// }

		// [ContextMenu("Delete flag.dat file")]
		// public void DeleteSaveFile()
        // {
		// 	if(File.Exists(FlagFilePath))
		// 	{
		// 		File.Delete(FlagFilePath);
		// 	}
        // }

		public bool GetFlag(Flag.Reference flagRef)
        { 
			if(PlayerPrefs.HasKey(flagRef.ToString()))
			{
				return PlayerPrefs.GetInt(flagRef.ToString()) == (int)Flag.State.True;
			}
			
			return false;
			// return (_flags.Flags.Find(f => f.FlagReference == flagRef).FlagState) == Flag.State.True;
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
			// Flag flag = _flags.Flags.Find(f => f.FlagReference == flagRef);
			// if(toTrue)
            // {
			// 	if (flag.FlagState == Flag.State.False)
            //     {
			// 		flag.FlagState = Flag.State.True;
			// 		SaveFlags();
            //     }
            // }
			// else
            // {
			// 	if (flag.FlagState == Flag.State.True)
			// 	{
			// 		flag.FlagState = Flag.State.False;
			// 		SaveFlags();
			// 	}
			// }
        }

		public bool FlagsSatisfied(List<Flag.Reference> trueFlags, List<Flag.Reference> falseFlags)
        {
			bool result = true;

			if (trueFlags != null)
			{
				foreach (Flag.Reference reference in trueFlags)
				{
					result = result && GetFlag(reference);
					// result = result && (_flags.Flags.Find(flag => flag.FlagReference == reference).FlagState == Flag.State.True);
				}
			}

			if (falseFlags != null)
			{
				foreach (Flag.Reference reference in falseFlags)
				{
					result = result && !GetFlag(reference);
					// result = result && (_flags.Flags.Find(flag => flag.FlagReference == reference).FlagState == Flag.State.False);
				}
			}

			return result;
		}
    }
}
