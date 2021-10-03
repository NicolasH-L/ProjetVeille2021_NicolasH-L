using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private const float ResetHitDelay = 0.5f;
    private const float SelfDestructDelay = 0.5f;
    private const string WeaponTag = "Weapon";
    private const int StartingHealthPoint = 100;
    private bool _isHit;
    private bool _isAlive;
    private int _healthPoint;

    public delegate void GameFinished(bool isDead);

    public event GameFinished OnGameEnded;

    private void Awake()
    {
        if (GameManager.GameManagerInstance == null)
            return;
        OnGameEnded += GameManager.GameManagerInstance.GameOver;
    }

    void Start()
    {
        _healthPoint = StartingHealthPoint;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag(WeaponTag) || GameManager.GameManagerInstance == null)
            return;
        TakeDamage(GameManager.GameManagerInstance.GetPlayerDamage());
    }

    private void TakeDamage(int damage)
    {
        _isHit = true;
        if (_healthPoint - damage <= 0)
        {
            _isAlive = false;
            Destroy(GetComponent<PolygonCollider2D>());
            Destroy(GetComponent<SpriteRenderer>());
            StartCoroutine(DelayDeath());
            return;
        }

        _healthPoint -= damage;
        StartCoroutine(ResetIsHit());
    }

    private IEnumerator ResetIsHit()
    {
        yield return new WaitForSeconds(ResetHitDelay);
        _isHit = false;
    }

    private IEnumerator DelayDeath()
    {
        yield return new WaitForSeconds(SelfDestructDelay);
        OnGameEnded?.Invoke(false);
        Destroy(gameObject);
    }
}