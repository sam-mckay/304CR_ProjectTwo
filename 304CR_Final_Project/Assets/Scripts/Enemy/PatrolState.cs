﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrolState : EnemyState
{
    //private Enemy_Controller enemy;
    //GameObject player;
    
    public PatrolState(Enemy_Controller enemyController) : base(enemyController)
    {
        enemy = enemyController;
        nextPatrolPoint = 1;
        if (enemy.patrolPoints.Length > 0)
        {
            patrol();
        }
        previousPos = enemy.transform.position;
    }

    public override void updateState()
    {
        //Debug.Log("PATROLLING");
        distance += enemy.speed * Time.deltaTime;
        move();
    }
        
    public override void toChaseState()
    {
        enemy.currentState = enemy.chaseState;
    }
}
