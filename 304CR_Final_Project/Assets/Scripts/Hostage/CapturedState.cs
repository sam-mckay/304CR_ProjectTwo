using UnityEngine;
using System.Collections;

public class CapturedState : HostageState
{

    public CapturedState(HostageController hostageController)  : base(hostageController)
    {
        hostage = hostageController;
    }

    public override void updateState()
    {
        //Debug.Log("Captured");
        
    }

    public override void toReleasedState()
    {
        hostage.releasedState.escape();
        hostage.currentState = hostage.releasedState;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other == player.GetComponent<Collider>())
        {
            toReleasedState();
        }
    }
}
