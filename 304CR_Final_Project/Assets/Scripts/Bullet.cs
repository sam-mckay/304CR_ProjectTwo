using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    float range;
    float damage;
    float speed;

    public Bullet(float _range, float _damage, float _speed)
    {
        range = _range;
        damage = _damage;
        speed = _speed;
    }

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
