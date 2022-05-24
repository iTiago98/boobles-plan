using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Santi.AI
{
	public abstract class State
	{
		public void Execute(AgentAI agent)
		{
			StateAction(agent);

			if(CheckTransitionConditions(agent))
			{
				OnStateExit(agent);
				agent.State.OnStateEnter(agent);
			}
		}

		protected abstract void StateAction(AgentAI agent);

		protected abstract bool CheckTransitionConditions(AgentAI agent);

		protected virtual void OnStateEnter(AgentAI agent) { }

		protected virtual void OnStateExit(AgentAI agent) { }
	}
}
