﻿using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class Ball : MonoBehaviour, IColorChangeable
{
    [SerializeField] float _respawnDelay;
    [SerializeField] byte _level;

    Rigidbody2D _rb;
    SpriteRenderer _spriteRenderer;
    Vector3 _startingPosition;
    bool _collidedWithBox;

    public byte Level { get; private set; }

    public void ChangeColor(Color32 color)
    {
        _spriteRenderer.color = color;
    }

    void Awake()
    {
        Level = _level;
        _rb = GetComponent<Rigidbody2D>();
        _startingPosition = transform.position;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Physics2D.IgnoreLayerCollision(8, 8);
        _collidedWithBox = false;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Box>())
        {
            _collidedWithBox = true;
            StartCoroutine(DoAfterDelay(Reset, _respawnDelay));
        }
        else if (!_collidedWithBox)
        {
            StartCoroutine(DoAfterDelay(Die, _respawnDelay));
        }
    }

    void Die()
    {
        if (!_collidedWithBox)
        {
            gameObject.SetActive(false);
            BallBelt.EnqueueDeadBall(this);
        }
    }

    public void Reset()
    {
        _rb.gravityScale = 0f;

        transform.position = _startingPosition;
        _rb.velocity = Vector3.zero;
    }

    IEnumerator DoAfterDelay(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
        _collidedWithBox = false;
    }
}