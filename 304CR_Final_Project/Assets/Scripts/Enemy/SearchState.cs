using UnityEngine;
using System.Collections;

public class SearchState : EnemyState
{
    float rotationAmount;
    

    public SearchState(Enemy_Controller enemyController) : base(enemyController)
    {
        enemy = enemyController;
        rotationAmount = 90.0f;
    }

    public override void updateState()
    {
        Vector3 angle = enemy.transform.rotation.eulerAngles;
        Debug.Log("SEARCHING: "+ angle.y);
        angle.y = angle.y + rotationAmount * Time.deltaTime;
        enemy.transform.eulerAngles = angle;
        if (angle.y >= 360.0f)
        {
            //check if patorl or guard
            if(enemy.patrolPoints.Length == 0)
            {
                toGuardState();
            }
            else
            {
                toPatrolState();
            }
        }
        if(isInLineOfSight() && Vector3.Distance(enemy.transform.position, player.transform.position) < enemy.weaponRange)
        {
            toAttackState();
        }
        else if(isInLineOfSight())
        {
            toChaseState();
        }
    }

    public override void toChaseState()
    {
        enemy.currentState = enemy.chaseState;
    }

    public override void toPatrolState()
    {
        enemy.patrolState.patrol();
        enemy.currentState = enemy.patrolState;
    }

    public override void toAttackState()
    {
        enemy.currentState = enemy.attackState;    
    }

    public override void toGuardState()
    {
        enemy.guardState.moveToGuardPoint();
        enemy.currentState = enemy.guardState;
    }
}
