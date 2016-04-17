using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_GroupController : MonoBehaviour
{

    LinkedList<GameObject> enemies;

    // Use this for initialization
    void Start ()
    {
        //init list of enemies
        enemies = new LinkedList<GameObject>();
        GameObject[] enemiesGameObject = GameObject.FindGameObjectsWithTag(Tags.Enemy);
        foreach(GameObject enemy in enemiesGameObject)
        {
            enemies.AddLast(enemy);
        }


	}
	
	// Update is called once per frame
	void Update ()
    {
        	
	}

    public void enemyAttacked(GameObject enemy)
    {
        GameObject nearestEnemy = findNearestEnemy(enemy);
        if(nearestEnemy == null)//no remaining enemies
        {
            return;
        }
        nearestEnemy.GetComponent<Enemy_Controller>().currentState.newRoute(new Location(Mathf.FloorToInt(enemy.transform.position.x), Mathf.FloorToInt(enemy.transform.position.z)));
        nearestEnemy.GetComponent<Enemy_Controller>().currentState = nearestEnemy.GetComponent<Enemy_Controller>().patrolState;
    }

    GameObject findNearestEnemy(GameObject enemy)
    {
        float nearestDist = -1;
        GameObject nearestEnemy = null;
        foreach(GameObject currentEnemy in enemies)
        {
            if(currentEnemy != enemy)
            {
                float currentDistance = Vector3.Distance(currentEnemy.transform.position, enemy.transform.position);
                if (currentDistance < nearestDist || nearestDist == -1)
                {
                    nearestDist = currentDistance;
                    nearestEnemy = currentEnemy;
                } 

            }
        }
        if (nearestDist == -1)
        {
            return null;
        }
        else
        {
            return nearestEnemy;
        }
    }
}
