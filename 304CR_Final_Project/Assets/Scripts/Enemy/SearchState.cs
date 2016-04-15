using UnityEngine;
using System.Collections;

public class SearchState : EnemyState
{
    
    public SearchState(Enemy_Controller enemyController) : base(enemyController)
    {
        enemy = enemyController;
    }

    public override void updateState()
    {

    }
}
