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
		public static string DataDirectory { get { return "Data"; } }
		public static string FlagFilePath { get { return DataDirectory+"/flags.dat"; } }

		private FlagList _flags;

        private void Awake()
        {
            if(File.Exists(FlagFilePath))
            {
				LoadFlags();
            }
			else
            {
				Directory.CreateDirectory(DataDirectory);
				_flags = new FlagList();
				SaveFlags();
            }
        }

        private void LoadFlags()
        {
			using (FileStream fs = new FileStream(FlagFilePath, FileMode.Open))
			{
				BinaryFormatter bf = new BinaryFormatter();
				_flags = (FlagList)bf.Deserialize(fs);
			}
		}

        private void SaveFlags()
        {
			using (FileStream fs = new FileStream(FlagFilePath, FileMode.Create))
			{
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(fs, _flags);
			}
		}

        [ContextMenu("Reset Flags")]
		public void ResetFlags()
		{
			foreach (Flag flag in _flags.List)
			{
				flag.FlagState = Flag.State.False;
			}
		}

		[ContextMenu("Delete flag.dat file")]
		public void DeleteSaveFile()
        {
			File.Delete(FlagFilePath);
        }

		public bool GetFlag(Flag.Reference flagRef)
        {
			return (_flags.List.Find(f => f.FlagReference == flagRef).FlagState) == Flag.State.True;
        }

		public void SetFlag(Flag.Reference flagRef)
        {
			_flags.List.Find(f => f.FlagReference == flagRef).FlagState = Flag.State.True;
			SaveFlags();
        }

		public bool FlagsSatisfied(List<Flag.Reference> trueFlags, List<Flag.Reference> falseFlags)
        {
			bool result = true;

			if (trueFlags != null)
			{
				foreach (Flag.Reference reference in trueFlags)
				{
					result = result && (_flags.List.Find(flag => flag.FlagReference == reference).FlagState == Flag.State.True);
				}
			}

			if (falseFlags != null)
			{
				foreach (Flag.Reference reference in falseFlags)
				{
					result = result && (_flags.List.Find(flag => flag.FlagReference == reference).FlagState == Flag.State.False);
				}
			}

			return result;
		}
    }
}
