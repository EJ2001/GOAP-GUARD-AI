using System.Collections.Generic;


 /* This script is an interface to force the use of these variables,
 	providing information to GOAP planner to plan what actions to use. 
    Also provides feedback to the agent and report success/failure */

public interface IGOAP
{
	HashSet<KeyValuePair<string,object>> getWorldState ();

	HashSet<KeyValuePair<string,object>> createGoalState ();

	void planFailed (HashSet<KeyValuePair<string,object>> failedGoal);

	void planFound (HashSet<KeyValuePair<string,object>> goal, Queue<GOAPAction> actions);

	void actionsFinished ();

	void planAborted (GOAPAction aborter);

	bool moveAgent(GOAPAction nextAction);
}