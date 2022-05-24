using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using DataModel;

public class Card : MonoBehaviour, IClickable
{
    //public enum CardType
    //{
    //    ARGUMENT, COUNTERARGUMENT
    //}

    #region Public variables

    //public CardType type;

    public int manaCost => _data.cost;
    public int strength => _data.strength;
    public int defense => _data.defense;

    [HideInInspector]
    public bool moveWithMouse;
    [HideInInspector]
    public CardContainer container;

    #endregion

    #region Private variables

    private CardsData _data;
    private Hand _hand;

    //private bool _isPlayer => _player.role == Player.Role.PLAYER;

    // Close up functionality
    //private bool _closeUp;
    //private Vector3 _defaultPos;
    //private Transform _closeUpPosition;
    //private Vector3 _defaultScale;
    //private Vector3 _augmentedScale;
    //private float _closeUpTime;

    // Z Position for the card to move above the other card sprites.
    private float _movePositionZ;

    private bool _clickable;

    bool IClickable.clickable { get => _clickable; set => _clickable = value; }
    GameObject IClickable.gameObject { get => gameObject; set => Debug.Log(""); }

    private SpriteRenderer _spriteRenderer;

    #endregion

    #region Private methods

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _movePositionZ = TurnManager.Instance.settings.movePositionZ;
    }

    private void Start()
    {
        _clickable = true;

        //Player player = TurnManager.Instance.player;

        //_closeUpPosition = player.closeUpPosition;
        //_defaultScale = transform.localScale;
        //_augmentedScale = transform.localScale * player.closeUpScaleMultiplier;
        //_closeUpTime = player.closeUpTime;
    }

    private void Update()
    {
        if (moveWithMouse)
        {
            Move();
        }
    }

    /// <summary>
    /// Maps mouse position in the screen to world coordinates to move the holding card.
    /// </summary>
    private void Move()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(worldPos.x, worldPos.y, _movePositionZ);
    }




    //private void CheckCloseUp()
    //{
    //    Sequence sequence = DOTween.Sequence();
    //    sequence.AppendCallback(() => _clickable = false);

    //    if (_closeUp)
    //    {
    //        sequence.Join(transform.DOMove(_defaultPos, _closeUpTime));
    //        sequence.Join(transform.DOScale(_defaultScale, _closeUpTime));
    //    }
    //    else
    //    {
    //        _defaultPos = transform.position;
    //        sequence.Join(transform.DOMove(_closeUpPosition.position, _closeUpTime));
    //        sequence.Join(transform.DOScale(_augmentedScale, _closeUpTime));
    //    }

    //    sequence.AppendCallback(() => _clickable = true);
    //    sequence.Play();

    //    _closeUp = !_closeUp;
    //}

    #endregion

    #region Public methods

    public void OnMouseLeftClickDown(MouseController mouseController)
    {
        //if (_isPlayer)
        //{
        if (!moveWithMouse && mouseController.holding == null)
        {
            // Deattach from parent
            RemoveFromContainer();
            
            // Stick to mouse
            moveWithMouse = true;
            mouseController.holding = this;
        }
        //}
    }

    public void OnMouseLeftClickUp(MouseController mouseController)
    {
        if (mouseController.holding == this)
        {
            _hand.AddCard(this);
            mouseController.holding = null;
        }
    }

    public void RemoveFromContainer()
    {
        if (container != null)
        {
            container.RemoveCard(gameObject);
            transform.parent = null;
        }
    }

    public void OnMouseRightClick()
    {
        //if (!_clickable) return;

        //CheckCloseUp();
    }

    public void OnMouseHoverEnter()
    {
        // highlight card
        //_previousPosZ = transform.localPosition.z;
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, highlightPosZ);
    }

    public void OnMouseHoverExit()
    {
        // remove highlight
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _previousPosZ);
    }

    public void Initialize(Hand hand, CardsData data)
    {
        _hand = hand;
        _data = data;

        name = data.name;
        _spriteRenderer.sprite = data.sprite;
    }

    public void Hit(int strength)
    {
        _data.defense -= strength;
        //Update card defense number
    }

    public void Destroy()
    {
        //Play destroy animation
        Destroy(gameObject);
    }

    #endregion

}
