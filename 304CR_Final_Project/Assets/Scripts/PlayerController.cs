using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public SphereCollider soundSphere;
    public GameObject bullet;
    public float range;
    public float damage;

    UnityStandardAssets.Characters.FirstPerson.FirstPersonController movementScript;
    bool isWalking;
    Vector2 m_move;
    static int standardSize = 1;
    static int walkSize = 2;
    static int runSize = 5;

    Camera camera;
    float fireRateTimer; 
    
    // Use this for initialization
    void Start ()
    {
        movementScript = this.gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        soundSphere.radius = standardSize;
        camera = GameObject.FindGameObjectWithTag(Tags.MainCamera).GetComponent<Camera>();
        fireRateTimer = 0;
	}
	
    void Update()
    {
        UpdateTimers();
        isWalking = movementScript.m_IsWalking;
        m_move = movementScript.m_Input;

        if(Input.GetButtonDown("Fire1") && fireRateTimer >= 0.3f)
        {
            fire();
            fireRateTimer = 0;
            Debug.Log("FIRE");
        }
    }

    void UpdateTimers()
    {
        fireRateTimer += Time.deltaTime;
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

    void fire()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
        ray.origin += transform.forward;
        RaycastHit hit;
        Physics.Raycast(ray, out hit, range);
        fireBullet(ray, hit);

        if(hit.collider.tag == Tags.Enemy)
        {
            //hit.collider.GetComponent<Enemy_Controller>().takeDamage(damage);
        }
        Debug.DrawLine(ray.origin, hit.point, Color.red, 10.0f, false);
    }

    void fireBullet(Ray ray, RaycastHit hit)
    {
        Camera playerCamera = this.transform.FindChild("FirstPersonCharacter").GetComponent<Camera>();
        GameObject newBullet = (GameObject) Instantiate(bullet, ray.origin, playerCamera.transform.rotation);
        newBullet.GetComponent<Bullet>().initBullet(range, damage, 1.0f);
        
    }
}

