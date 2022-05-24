using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{

    #region Public variables

    public static TurnManager Instance { private set; get; }

    public enum Turn
    {
        PLAYER, OPPONENT
    }

    public CardGameSettings settings;

    public Board board;

    [Header("Contenders")]
    public Contender player;
    public Contender opponent;

    public OpponentAI opponentAI;

    [Header("UI")]
    public Text playerStats;
    public Text opponentStats;

    public MyButton endTurnButton;
    public EloquenceBar eloquenceBar;

    public bool isPlayerTurn => _turn == Turn.PLAYER;
    public Contender currentPlayer => (_turn == Turn.PLAYER) ? player : opponent;

    #endregion

    #region Private variables

    private Turn _turn;

    private const string PLAYER_TURN_BUTTON_TEXT = "End turn";
    private const string OPPONENT_TURN_BUTTON_TEXT = "Enemy turn";

    #endregion

    #region Public methods

    public void FinishTurn()
    {
        ChangeTurn();
        CheckEndTurnButtonState();
    }

    public void UpdateUI()
    {
        playerStats.text = "Health: " + player.eloquence + "\nMana: " + player.currentMana + "/" + player.currentMaxMana;
        opponentStats.text = "Health: " + opponent.eloquence + "\nMana: " + opponent.currentMana + "/" + opponent.currentMaxMana;
    }

    #endregion

    #region Private methods

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeDecks();
        StartGame();
    }

    private void InitializeDecks()
    {
        board.InitializeDecks(player.deckCards.cards, opponent.deckCards.cards);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            FinishTurn();
        }
    }

    private void CheckEndTurnButtonState()
    {
        if (_turn == Turn.OPPONENT)
        {
            endTurnButton.SetInteractable(false);
            endTurnButton.SetText(OPPONENT_TURN_BUTTON_TEXT);
        }
        else
        {
            endTurnButton.SetInteractable(true);
            endTurnButton.SetText(PLAYER_TURN_BUTTON_TEXT);
        }
    }

    private void ChangeTurn()
    {
        if (_turn == Turn.OPPONENT)
        {
            opponentAI.enabled = false;
            SetTurn(Turn.PLAYER);
        }
        else
        {
            SetTurn(Turn.OPPONENT);
            FinishRound();
        }
    }

    private void FinishRound()
    {
        Clash();
        FillMana();
        DrawCards(1);
        StartRound();
    }

    private void StartRound()
    {
        opponentAI.enabled = true;
    }

    private void DrawCards(int cardNumber)
    {
        board.DrawCards(cardNumber);
    }

    private void FillMana()
    {
        player.FillMana();
        opponent.FillMana();
        UpdateUI();
    }

    private void Clash()
    {
        for (int i = 0; i < board.playerCardZone.Count; i++)
        {
            Card playerCard = board.playerCardZone[i].GetCard();
            Card opponentCard = board.opponentCardZone[i].GetCard();

            if (playerCard != null && opponentCard != null)
            {
                playerCard.Hit(opponentCard.strength);
                opponentCard.Hit(playerCard.strength);

                if (playerCard.defense <= 0) playerCard.Destroy();
                if (opponentCard.defense <= 0) opponentCard.Destroy();
            }
            else if (playerCard != null)
            {
                opponent.Hit(playerCard.strength);
            }
            else if (opponentCard != null)
            {
                player.Hit(opponentCard.strength);
            }
        }
    }

    private void StartGame()
    {
        InitializeContenders();
        SetTurn(Turn.OPPONENT);
        DrawCards(settings.initialCardNumber);
        CheckEndTurnButtonState();
    }

    private void SetTurn(Turn turn)
    {
        _turn = turn;
    }

    private void InitializeContenders()
    {
        player.Initialize(board.playerHand, board.playerCardZone);
        player.InitializeStats(settings.initialEloquence, settings.initialManaCounter);

        opponent.Initialize(board.opponentHand, board.opponentCardZone);
        opponent.InitializeStats(settings.initialEloquence, settings.initialManaCounter);

        opponentAI.Initialize(opponent);

        UpdateUI();
    }

    #endregion
}
