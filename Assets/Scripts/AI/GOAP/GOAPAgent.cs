using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public sealed class GOAPAgent : MonoBehaviour {

	private FSM finateStateMachine;

	// FSM has 3 states to run for each action
	private FSM.FSMState idleState; 
	private FSM.FSMState moveToState;
	private FSM.FSMState performActionState; 
	
	private HashSet<GOAPAction> availableActions;
	private Queue<GOAPAction> currentActions;

	private IGOAP dataProvider; // this is the class that provides our world data and listens to feedback on planning

	private GOAPPlanner planner;


	void Start ()
	{
		finateStateMachine = new FSM ();
		availableActions = new HashSet<GOAPAction> ();
		currentActions = new Queue<GOAPAction> ();
		planner = new GOAPPlanner ();
		FindDataProvider ();
		createIdleState ();
		createMoveToState ();
		createPerformActionState ();
		finateStateMachine.pushState (idleState);
		LoadAvailableActions ();
	}
	

	void Update ()
	{
		finateStateMachine.Update (this.gameObject);
	}


	public void addAction(GOAPAction a)
	{
		availableActions.Add (a);
	}

	public GOAPAction getAction(Type action)
	{
		foreach (GOAPAction g in availableActions)
		{
			if (g.GetType().Equals(action) )
			    return g;
		}
		return null;
	}

	public void removeAction(GOAPAction action)
	{
		availableActions.Remove (action);
	}

	private bool hasActionPlan()
	{
		return currentActions.Count > 0;
	}

	private void createIdleState()
	{
		idleState = (fsm, gameObj) => {
			// GOAP planning

			// get the world state and the goal we want to plan for
			HashSet<KeyValuePair<string,object>> worldState = dataProvider.getWorldState();
			HashSet<KeyValuePair<string,object>> goal = dataProvider.createGoalState();

			Queue<GOAPAction> plan = planner.plan(gameObject, availableActions, worldState, goal);
			if (plan != null)
			{
				// there is a plan available to reach goal
				currentActions = plan;
				dataProvider.planFound(goal, plan);

				fsm.popState(); 
				fsm.pushState(performActionState);

			} 
			else
			{
				// Debug.Log(this.gameObject.name + " <color=red>Failed Plan:</color>"+DebugMessage(goal));
				dataProvider.planFailed(goal);
				fsm.popState (); 
				fsm.pushState (idleState);
			}

		};
	}
	
	private void createMoveToState()
    {
		moveToState = (fsm, gameObj) => 
		{

			GOAPAction action = currentActions.Peek();
			if (action.requiresInRange() && action.target == null)
			{
				fsm.popState(); // move
				fsm.popState(); // perform
				fsm.pushState(idleState);
				return;
			}

			// get the agent to move itself
			if ( dataProvider.moveAgent(action) ) 
			{
				fsm.popState();
			}

		};
	}
	
	private void createPerformActionState() 
	{

		performActionState = (fsm, gameObj) =>
		{
            // perform the action

            if (!hasActionPlan()) 
			{
				// no actions to perform
				Debug.Log("<color=red>Done actions</color>");
				fsm.popState();
				fsm.pushState(idleState);
				dataProvider.actionsFinished();
				return;
			}

			GOAPAction action = currentActions.Peek();
			if ( action.isActionFinished() ) 
			{
				// the action is done. Remove it so we can perform the next one
				currentActions.Dequeue();
			}

			if (hasActionPlan()) 
			{
				// perform the next action
				action = currentActions.Peek();
				bool inRange = action.requiresInRange() ? action.isInRange() : true;

				if ( inRange ) 
				{
					// performing action because in range
					bool success = action.perform(gameObj);

					if (!success) 
					{
						// action failed
						fsm.popState();
						fsm.pushState(idleState);
						dataProvider.planAborted(action);
					}
				} 
				else 
				{
					fsm.pushState(moveToState);
				}

			} 
			else 
			{
				// no actions left move to Plan state
				fsm.popState();
				fsm.pushState(idleState);
				dataProvider.actionsFinished();
			}

		};
	}

	private void FindDataProvider()
	{
		foreach (Component comp in gameObject.GetComponents(typeof(Component)))
		{
			if ( typeof(IGOAP).IsAssignableFrom(comp.GetType()) )
			{
				dataProvider = (IGOAP)comp;
				return;
			}
		}
	}

	private void LoadAvailableActions ()
	{
		GOAPAction[] actions = gameObject.GetComponents<GOAPAction>();
		foreach (GOAPAction a in actions)
		{
			availableActions.Add (a);
		}

	}

	public IGOAP GetDataProviderInterface()
    {
        return dataProvider;
    }




	// Debug message references
	public static string DebugMessage(HashSet<KeyValuePair<string,object>> state)
	{
		String s = "";
		foreach (KeyValuePair<string,object> kvp in state)
		{
			s += kvp.Key + ":" + kvp.Value.ToString();
			s += ", ";
		}
		return s;
	}

	public static string DebugMessage(Queue<GOAPAction> actions)
	{
		String s = "";
		foreach (GOAPAction a in actions)
		{
			s += a.GetType().Name;
			s += "-> ";
		}
		s += "GOAL!";
		return s;
	}

	public static string DebugMessage(GOAPAction[] actions)
	{
		String s = "";
		foreach (GOAPAction a in actions)
		{
			s += a.GetType().Name;
			s += ", ";
		}
		return s;
	}

	public static string DebugMessage(GOAPAction action)
	{
		String s = ""+action.GetType().Name;
		return s;
	}
}