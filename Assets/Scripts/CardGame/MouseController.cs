using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{

    #region Public variables

    public LayerMask selectingLayerMask;
    public LayerMask holdingLayerMask;

    [HideInInspector]
    public Card holding = null;

    #endregion

    #region Private variables

    private bool _playerTurn => TurnManager.Instance.isPlayerTurn;

    private IClickable _hovering;
    //private IClickable _closeUp;

    #endregion

    #region Private methods

    void Update()
    {
        IClickable clickableObject = GetRaycastObject();

        CheckHovering(clickableObject);
        CheckClick(clickableObject);
    }

    private void CheckHovering(IClickable clickableObject)
    {
        if (clickableObject != _hovering)
        {
            HoverExit();
            if (clickableObject != null)
            {
                clickableObject.OnMouseHoverEnter();
                _hovering = clickableObject;
            }
        }
    }

    private void CheckClick(IClickable clickableObject)
    {
        bool leftClickDown = Input.GetMouseButtonDown(0);
        bool leftClickUp = Input.GetMouseButtonUp(0);
        //bool rightClick = Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1);

        //if ((leftClickDown || rightClick) && (_closeUp != null) && (_closeUp != clickableObject))
        //{
        //    if (!_closeUp.clickable) return;

        //    _closeUp.OnMouseRightClick();
        //    _closeUp = null;
        //}

        if (leftClickDown)
        {
            if (clickableObject != null && _playerTurn)
            {
                clickableObject.OnMouseLeftClickDown(this);
            }
        }
        else if (leftClickUp)
        {
            if (clickableObject != null)
            {
                clickableObject.OnMouseLeftClickUp(this);
            }
            else if (holding != null)
            {
                holding.OnMouseLeftClickUp(this);
            }
        }
        //else if (rightClick)
        //{
        //    if (clickableObject != null)
        //    {
        //        clickableObject.OnMouseRightClick();
        //        _closeUp = (_closeUp == clickableObject) ? null : clickableObject;
        //    }
        //}
    }

    private IClickable GetRaycastObject()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask layerMask = (holding == null) ? selectingLayerMask : holdingLayerMask;
        var rayResult = Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);

        IClickable clickableObject = null;
        if (rayResult)
        {
            GameObject obj = hit.collider.gameObject;
            clickableObject = obj.GetComponent<IClickable>();
        }

        return clickableObject;
    }

    private void HoverExit()
    {
        if (_hovering != null)
        {
            _hovering.OnMouseHoverExit();
            _hovering = null;
        }
    }

    #endregion
}
