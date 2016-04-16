using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    float range;
    float damage;
    float speed;
    float distanceFromStart;

    Vector3 startPos;

	// Use this for initialization
	void Start ()
    {
        startPos = transform.position;
        distanceFromStart = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    if(distanceFromStart < range)
        {
            transform.position += transform.forward * speed;
            distanceFromStart = Vector3.Distance(startPos, transform.position);
        }
        else
        {
            Destroy(this.gameObject);
        }
	}

    public void initBullet(float _range, float _damage, float _speed)
    {
        range = _range;
        damage = _damage;
        speed = _speed;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("HIT! " + other.GetType());
        if (other.tag != Tags.Player && other.GetType().ToString() != "UnityEngine.SphereCollider")
        {
            //Debug.Log("HIT OBJECT");
            if (other.tag == Tags.Enemy && other.GetType().ToString() != "UnityEngine.SphereCollider")
            {
                //Debug.Log("HIT! ENEMY");
                other.gameObject.GetComponent<Enemy_Controller>().takeDamage(damage);
                Destroy(this.gameObject);
            }
            else if (other.GetType().ToString() == "UnityEngine.SphereCollider")
            {
               // Debug.Log("HIT! ENEMY FOV COLLIDER");
                return;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        else if (other.tag == Tags.Player && other.GetType().ToString() != "UnityEngine.SphereCollider")
        {
            //Debug.Log("HIT! Player");
            other.gameObject.GetComponent<PlayerController>().takeDamage(damage);
            Destroy(this.gameObject);
        }
        else
        {
            //Debug.Log("HIT PLAYER");
        }
    }
}
