using System;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float speed;
    public float shootCooldown;
    private float _shootCooldown;

    public void Move(Vector3 direction)
    {
        transform.transform.position += direction * (speed * Time.deltaTime);
    }

    private void Update()
    {
        if (_shootCooldown > 0)
        {
            _shootCooldown -= Time.deltaTime;
        }
    }

    public void Shoot()
    {
        if(_shootCooldown <= 0)
        {
            _shootCooldown = shootCooldown;
            //shoot
            Debug.Log("pew pew");
        }
        
    }
}