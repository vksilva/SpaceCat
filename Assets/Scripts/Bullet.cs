using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float damage;
    private Vector3 _direction;
    public float speed;

    public void Init(Sprite sprite, Vector3 position, Vector3 direction)
    {
        spriteRenderer.sprite = sprite;
        var tr = transform;
        tr.position = position;
        tr.up = direction;
        _direction = direction;
    }

    private void Update()
    {
        transform.position += _direction * (Time.deltaTime * speed);
    }
}
