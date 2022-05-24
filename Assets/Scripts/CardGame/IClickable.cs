using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IClickable
{
    public bool clickable { protected set; get; }
    public GameObject gameObject { protected set; get; }

    virtual public void OnMouseLeftClickDown(MouseController mouseController)
    {
        OnMouseHoverExit();
    }

    virtual public void OnMouseLeftClickUp(MouseController mouseController)
    {
        OnMouseHoverExit();
    }

    abstract public void OnMouseRightClick();
    abstract public void OnMouseHoverEnter();
    abstract public void OnMouseHoverExit();

}
