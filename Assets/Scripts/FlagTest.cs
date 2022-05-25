using TMPro;
using UnityEngine;

namespace Booble.Flags
{
	public class FlagTest : MonoBehaviour
	{
        [SerializeField] private TextMeshProUGUI _text;

    	public void SetAnaIntroFlag()
        {
            FlagManager.Instance.SetFlag(Flag.Reference.ArcadioIntro);
        }

        public void GetAnaIntroFlag()
        {
            _text.text = "ArcadioIntro Flag State =\n" + FlagManager.Instance.GetFlag(Flag.Reference.ArcadioIntro);
        }

        public void ResetAllFlags()
        {
            FlagManager.Instance.ResetFlags();
        }
	}
}