using UnityEngine;
using System.Collections;

public interface IEnemyState
{
    void updateState();

    void onTriggerEnter(Collider other);

    void toPatrolState();

    void toGuardState();

    void toSearchState();

    void toAttackState();

    void toChaseState();

    void toFleeState();
}
