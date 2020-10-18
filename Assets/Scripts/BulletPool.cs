using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public int initSize;
    public Bullet bulletPrefab;
    private List<Bullet> _bullets;


    private void Start()
    {
        _bullets = new List<Bullet>(initSize);

        for (int i = 0; i < initSize; i++)
        {
            var bullet = Instantiate(bulletPrefab);
            Return(bullet);
        }
    }

    public Bullet Get()
    {
        if (_bullets.Count > 0)
        {
            var bullet = _bullets[0];
            _bullets.RemoveAt(0);
            return bullet;
        }

        Debug.LogWarning("Pool was empty. Creating a new one.");
        return Instantiate(bulletPrefab);
    }

    public void Return(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        _bullets.Add(bullet);
    }
}