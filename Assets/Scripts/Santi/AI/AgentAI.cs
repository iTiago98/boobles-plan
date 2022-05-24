using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Santi.AI
{
    public class AgentAI : MonoBehaviour
    {
        public State State
        {
            get { return _state; }
            set { _state = value; }
        }
        private State _state;

	    protected virtual void Update()
	    {
		    State.Execute(this);
	    }
    }
}
