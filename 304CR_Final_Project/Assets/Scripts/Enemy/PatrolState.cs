using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrolState : EnemyState
{
    //private Enemy_Controller enemy;
    //GameObject player;
    
    public PatrolState(Enemy_Controller enemyController) : base(enemyController)
    {
        grid = GameObject.FindGameObjectWithTag(Tags.World).GetComponent<World>().grid;
        player = GameObject.FindGameObjectWithTag(Tags.Player);
        enemy = enemyController;
        nextPatrolPoint = 1;
        patrol();
        previousPos = enemy.transform.position;
    }

    public override void updateState()
    {
        distance += speed * Time.deltaTime;
        move();
    }
        
    public override void toChaseState()
    {
        enemy.currentState = enemy.chaseState;
    }
}
