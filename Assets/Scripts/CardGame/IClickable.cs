using Booble.CardGame.Managers;

namespace Booble.CardGame
{
    public interface IClickable
    {
        //public bool clickable { get; set; }

        virtual public void OnMouseLeftClickDown(MouseController mouseController)
        {
            //OnMouseHoverExit();
        }

        virtual public void OnMouseLeftClickUp(MouseController mouseController)
        {
            //OnMouseHoverExit();
        }

        abstract public void OnMouseHoverEnter();
        abstract public void OnMouseHoverExit();
    }
}