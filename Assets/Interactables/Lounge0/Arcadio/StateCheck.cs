using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using UnityEngine;

namespace Booble.Interactables.Arcadio
{
    public class StateCheck : MonoBehaviour
    {
        [SerializeField] private Animator _anim;
        [SerializeField] private Flag.Reference _arcadioPartida;
        [SerializeField] private List<Flag.Reference> _coins;

        private void Start()
        {
            if(!FlagManager.Instance.GetFlag(_arcadioPartida))
                return;

            if (FlagManager.Instance.FlagsSatisfied(_coins, new List<Flag.Reference>()))
            {
                Destroy(gameObject);
            }
            else
            {
                _anim.SetTrigger("Wait");
            }
        }
    }
}
