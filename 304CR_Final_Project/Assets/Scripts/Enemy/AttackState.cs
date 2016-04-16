using UnityEngine;
using System;
using System.Collections;

public class AttackState : EnemyState
{
    bool inCover;
    float fireRateTimer;

    public AttackState(Enemy_Controller enemyController) : base(enemyController)
    {
        enemy = enemyController;
        inCover = true; // set to true until cover mechanic implemented and the default to false
        fireRateTimer = 0;
        
    }

    public override void updateState()
    {
        //Debug.Log("ATTACKING");
        UpdateTimers();
        //find cover
        //return fire
        attack();
        if (enemy.health < 25.0f)
        {
            toFleeState();
        }
    }

    void UpdateTimers()
    {
        fireRateTimer += Time.deltaTime;
    }

    public override void toFleeState()
    {
        enemy.fleeState.flee();
        enemy.currentState = enemy.fleeState;
    }

    public override void toChaseState()
    {
        enemy.currentState = enemy.chaseState;
    }

    public override void toSearchState()
    {
        enemy.currentState = enemy.searchState;
    }

    void attack()
    {
        //if target in range
            //fire
        //else chase
        if(Vector3.Distance(player.transform.position, enemy.transform.position) < enemy.weaponRange)
        {
            if (fireRateTimer >= 0.5f)
            {
                //Debug.Log("SHOOTING");
                fire();
                fireRateTimer = 0;
            }
        }
        else if(isInLineOfSight())
        {
            toChaseState();
        }
        else
        {
            toSearchState();
        }
    }
    
    
    void fire()
    {
        //ray point at player add small rand val
        Ray ray = new Ray();
        ray.origin = enemy.transform.position + enemy.transform.forward + new Vector3(0,1,0);
        ray.direction = player.transform.position - (enemy.transform.position + enemy.transform.forward);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, enemy.weaponRange);
        enemy.fireBullet(ray, hit);
        Debug.DrawLine(ray.origin, hit.point, Color.blue, 10.0f, false);
    }

    public override void attacked()
    {
        
    }

}
