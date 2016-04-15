using UnityEngine;
using System.Collections;

public class GuardState : EnemyState
{
    
    public GuardState(Enemy_Controller enemyController) : base(enemyController)
    {
        enemy = enemyController;
    }

    public override void updateState()
    {

    }
}
