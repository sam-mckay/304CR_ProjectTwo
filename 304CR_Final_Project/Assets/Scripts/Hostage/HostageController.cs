using UnityEngine;
using System.Collections;

public class HostageController : MonoBehaviour
{
    public float health;
    public float speed;

    //stateMachine
    [HideInInspector] public HostageState currentState;
    [HideInInspector] public ReleasedState releasedState;
    [HideInInspector] public CapturedState capturedState;

    // Use this for initialization
    void Start ()
    {
        currentState = new HostageState(this);
        releasedState = new ReleasedState(this);
        capturedState = new CapturedState(this);

        currentState = capturedState;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void FixedUpdate()
    {
        currentState.updateState();
    }

    public void takeDamage(float hitDamage)
    {
        health -= hitDamage;
        //Debug.Log("HOSTAGE: TOOK DAMAGE: " + hitDamage + " CURRENT HEALTH: " + health);
        if (health < 0.0f)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }
}
