using Booble.CardGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Booble.CardGame.UI
{
    public class StatsUI : MonoBehaviour
    {
        [Header("Contenders Stats")]

        [SerializeField] private GameObject playerHealthBar;
        [SerializeField] private GameObject playerManaCounter;

        [SerializeField] private Image playerHealthImage;
        [SerializeField] private Image playerExtraHealthImage;
        [SerializeField] private Image playerExtraHealthImage2;
        [SerializeField] private List<Image> playerManaList;

        [SerializeField] private Image opponentHealthImage;
        [SerializeField] private Image opponentExtraHealthImage;
        [SerializeField] private Image opponentExtraHealthImage2;
        [SerializeField] private List<Image> opponentManaList;

        [SerializeField] private Sprite fullManaCristal;
        [SerializeField] private Sprite emptyManaCristal;
        [SerializeField] private Sprite fullExtraManaCristal;
        [SerializeField] private Sprite noManaCristal;

        private int _shownPlayerLife;
        private int _shownPlayerMana;

        private int _shownOpponentLife;
        private int _shownOpponentMana;

        private int _defaultMaxLife;
        private int _defaultMaxMana;

        private void Start()
        {
            _defaultMaxLife = CardGameManager.Instance.settings.initialLife;
            _defaultMaxMana = CardGameManager.Instance.settings.maxManaCounter;
        }


        public GameObject GetPlayerHealthBar() => playerHealthBar;
        public GameObject GetPlayerManaCounter() => playerManaCounter;

        public bool statsUpdated { private set; get; }
        private bool _hideEmptyCristals;

        public void UpdateUIStats(bool hideEmptyCristals)
        {
            _hideEmptyCristals = hideEmptyCristals;
            StartCoroutine(UpdateUIStatsCoroutine());
        }

        private IEnumerator UpdateUIStatsCoroutine()
        {
            Contender player = CardGameManager.Instance.player;
            Contender opponent = CardGameManager.Instance.opponent;

            statsUpdated = false;

            int loops = Mathf.Max(
                Mathf.Abs(player.life - _shownPlayerLife),
                Mathf.Abs(player.currentMana - _shownPlayerMana),
                Mathf.Abs(opponent.life - _shownOpponentLife),
                Mathf.Abs(opponent.currentMana - _shownOpponentMana)
                );

            for (int i = 0; i < loops; i++)
            {
                SetStats(player.life, player.currentMana, player.currentMaxMana, player.extraMana,
                    opponent.life, opponent.currentMana, opponent.currentMaxMana, opponent.extraMana);

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitUntil(() =>
                _shownPlayerLife == player.life && _shownPlayerMana == player.currentMana
                && _shownOpponentLife == opponent.life && _shownOpponentMana == opponent.currentMana);

            statsUpdated = true;
        }

        private void SetStats(int playerCurrentLife, int playerCurrentMana, int playerCurrentMaxMana, int playerExtraMana,
            int opponentCurrentLife, int opponentCurrentMana, int opponentCurrentMaxMana, int opponentExtraMana)
        {
            if (_shownPlayerLife != playerCurrentLife)
                SetHealth(playerHealthImage, playerExtraHealthImage, playerExtraHealthImage2, ref _shownPlayerLife, playerCurrentLife);
            if (_shownOpponentLife != opponentCurrentLife)
                SetHealth(opponentHealthImage, opponentExtraHealthImage, opponentExtraHealthImage2, ref _shownOpponentLife, opponentCurrentLife);

            if (_shownPlayerMana != playerCurrentMana)
                SetMana(playerManaList, ref _shownPlayerMana, playerCurrentMana, playerCurrentMaxMana, playerExtraMana);
            if (_shownOpponentMana != opponentCurrentMana)
                SetMana(opponentManaList, ref _shownOpponentMana, opponentCurrentMana, opponentCurrentMaxMana, opponentExtraMana);
        }

        private void SetHealth(Image healthImage, Image extraHealthImage, Image extraHealthImage2, ref int shownLife, int currentLife)
        {
            if (shownLife < currentLife) shownLife++;
            else shownLife--;

            healthImage.fillAmount = (float)shownLife / _defaultMaxLife;

            if (shownLife >= _defaultMaxLife) extraHealthImage.fillAmount = (float)(shownLife - _defaultMaxLife) / _defaultMaxLife;
            if (shownLife >= (_defaultMaxLife * 2)) extraHealthImage2.fillAmount = (float)(shownLife - (_defaultMaxLife * 2)) / _defaultMaxLife;
        }

        private void SetMana(List<Image> manaList, ref int shownMana, int currentMana, int currentMaxMana, int extraMana)
        {
            if (shownMana < currentMana)
            {
                if (IsExtraMana(extraMana, currentMaxMana, shownMana))
                    manaList[_defaultMaxMana + (shownMana - currentMaxMana + extraMana)].sprite = fullExtraManaCristal;

                else manaList[shownMana].sprite = fullManaCristal;

                shownMana++;
            }
            else
            {
                shownMana--;
                int index;
                if (IsExtraMana(extraMana, currentMaxMana, shownMana)) index = _defaultMaxMana + (shownMana - currentMaxMana + extraMana);
                else index = shownMana;

                manaList[index].sprite = _hideEmptyCristals ? noManaCristal : emptyManaCristal;
            }
        }

        private bool IsExtraMana(int extraMana, int currentMaxMana, int shownMana)
        {
            return extraMana > 0
                && (currentMaxMana - shownMana) <= extraMana
                && (currentMaxMana - extraMana) <= _defaultMaxMana;
        }

        public void UpdateMaxMana(Contender contender, int newMaxMana)
        {
            List<Image> manaList = (contender.isPlayer) ? playerManaList : opponentManaList;
            manaList[newMaxMana - 1].sprite = emptyManaCristal;
        }

        public void SetShownMana(int mana)
        {
            _shownPlayerMana = mana;
            _shownOpponentMana = mana;
        }
    }
}