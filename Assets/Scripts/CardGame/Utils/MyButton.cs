using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Booble.CardGame.Utils
{
    public class MyButton : MonoBehaviour
    {
        public TextMeshProUGUI text;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void SetText(string s)
        {
            text.text = s;
        }

        public void SetInteractable(bool b)
        {
            _button.interactable = b;
        }

        public bool IsInteractable()
        {
            return _button.interactable;
        }
    }
}