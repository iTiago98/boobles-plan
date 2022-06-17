using CardGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
    public interface IClickable
    {
        public bool clickable { get; set; }
        public GameObject gameObject { protected set; get; }

        virtual public void OnMouseLeftClickDown(MouseController mouseController)
        {
            //OnMouseHoverExit();
        }

        virtual public void OnMouseLeftClickUp(MouseController mouseController)
        {
            //OnMouseHoverExit();
        }

        abstract public void OnMouseRightClick();
        abstract public void OnMouseHoverEnter();
        abstract public void OnMouseHoverExit();

        virtual public void SetClickable(bool clickable)
        {
            this.clickable = clickable;
        }

    }
}