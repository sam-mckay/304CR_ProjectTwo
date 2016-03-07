using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public SphereCollider soundSphere;


    UnityStandardAssets.Characters.FirstPerson.FirstPersonController movementScript;
    bool isWalking;
    Vector2 m_move;
    static int standardSize = 1;
    static int walkSize = 2;
    static int runSize = 5;
    // Use this for initialization
    void Start ()
    {
        movementScript = this.gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        soundSphere.radius = standardSize;
	}
	
    void Update()
    {
        isWalking = movementScript.m_IsWalking;
        m_move = movementScript.m_Input;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        

        if(m_move.x == 0 && m_move.y == 0)
        {
            soundSphere.radius = standardSize;
        }
        else if(isWalking)
        {
            soundSphere.radius = walkSize;
        }
        else
        {
            soundSphere.radius = runSize;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == Tags.Enemy)
        {
            other.GetComponent<Enemy_Controller>().playerheard();
        }
    }
}
