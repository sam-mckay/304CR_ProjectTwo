using UnityEngine;
using System.Collections;

public class SearchState : IEnemyState
{
    private readonly Enemy_Controller enemy;

    public SearchState(Enemy_Controller enemyController)
    {
        enemy = enemyController;
    }

    public void updateState()
    {

    }

    public void onTriggerEnter(Collider other)
    {

    }

    public void toPatrolState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public void toGuardState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public void toSearchState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public void toAttackState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public void toChaseState()
    {
        Debug.Log("Cannot Transition to this state");
    }

    public void toFleeState()
    {
        Debug.Log("Cannot Transition to this state");
    }
}
