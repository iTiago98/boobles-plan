using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using UnityEngine;

namespace Booble.Interactables.Arcadio
{
    public class StateCheck : MonoBehaviour
    {
        [SerializeField] private Collider2D _arcadeCollider;
        [SerializeField] private Animator _anim;
        [SerializeField] private Flag.Reference _arcadioPartida;
        [SerializeField] private List<Flag.Reference> _coins;

        private void Start()
        {
            if(!FlagManager.Instance.GetFlag(_arcadioPartida))
                return;

            int cc = 0;
            for (int i = 0; i < _coins.Count; i++)
            {
                if (FlagManager.Instance.GetFlag(_coins[i]))
                {
                    cc++;
                }
            }

            if (cc >= 3)
            {
                _arcadeCollider.enabled = true;
                gameObject.SetActive(false);
            }
            else if (cc == 1)
            {
                _anim.SetTrigger("Lose");
            }
            else
            {
                _anim.SetTrigger("Wait");
            }
            //
            // if (FlagManager.Instance.FlagsSatisfied(_coins, new List<Flag.Reference>()))
            // {
            //     gameObject.SetActive(false);
            // }
            // else
            // {
            //     _anim.SetTrigger("Wait");
            // }
        }
    }
}
