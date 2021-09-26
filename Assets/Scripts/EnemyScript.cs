using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public delegate int GetPlayerDamage();

    public event GetPlayerDamage OnPlayerWeaponDamage;

    public delegate void DamagePlayer(int damage);

    public event DamagePlayer OnPlayerCollide;

    [SerializeField] private GameObject projectile;
    [SerializeField] private int healthPoint;
    [SerializeField] private int damagePoint;
    private const float Speed = 1F;
    private const float TimeBetweenShots = 1f;
    private const float CollisionAttackDelay = 1f;
    private const string WeaponTag = "Weapon";
    private const string PlayerTag = "Player";
    private Transform _target;
    private SpriteRenderer _sprite;
    private List<Collider2D> _colliders;
    private bool _hasShot;
    private bool _isLeft;
    private bool _isRight;
    private bool _isHit;
    private bool _isCollidedWithPlayer;

    private void Start()
    {
        if (GameManager.GameManagerInstance != null && TopDownCharacterController.GetPlayerInstance != null)
        {
            OnPlayerWeaponDamage += GameManager.GameManagerInstance.GetPlayerDamage;
            OnPlayerCollide += TopDownCharacterController.GetPlayerInstance.TakeDamage;
        }

        _colliders = new List<Collider2D>();
        _colliders.AddRange(GetComponents<Collider2D>());
        _hasShot = false;
        _isLeft = true;
        _isRight = false;
        _sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_target == null) return;
        float step = Speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, _target.position, step);
        if (_target.position.x < gameObject.transform.position.x && _isRight)
        {
            _sprite.flipX = false;
            _isLeft = true;
            _isRight = false;
        }
        else if (_target.position.x > gameObject.transform.position.x && _isLeft)
        {
            _sprite.flipX = true;
            _isLeft = false;
            _isRight = true;
        }

        if (_hasShot) return;
        StartCoroutine(DelayNextShot());
    }

    private IEnumerator DelayNextShot()
    {
        _hasShot = true;
        Instantiate(projectile, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(TimeBetweenShots);
        _hasShot = false;
    }

    private void TakeDamage(int damage)
    {
        if (gameObject == null || _isHit) return;
        _isHit = true;
        if (healthPoint - damage <= 0)
        {
            foreach (var colliderEnemy in _colliders)
                Destroy(colliderEnemy);

            Destroy(GetComponent<Rigidbody2D>());
            Destroy(gameObject);
        }

        healthPoint -= damage;
        StartCoroutine(ResetHit());
    }

    private IEnumerator ResetHit()
    {
        yield return new WaitForSeconds(0.5f);
        _isHit = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _target = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _target = null;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(WeaponTag) && OnPlayerWeaponDamage != null)
        {
            TakeDamage(OnPlayerWeaponDamage());
            _sprite.color = Color.red;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(WeaponTag))
            _sprite.color = Color.white;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(WeaponTag) && OnPlayerWeaponDamage != null)
            TakeDamage(OnPlayerWeaponDamage());
        
        if (!other.gameObject.CompareTag(PlayerTag) || _isCollidedWithPlayer) return;
        _isCollidedWithPlayer = true;
        OnPlayerCollide?.Invoke(damagePoint);
        StartCoroutine(DelayCollisionDamage());
    }

    private IEnumerator DelayCollisionDamage()
    {
        yield return new WaitForSeconds(CollisionAttackDelay);
        _isCollidedWithPlayer = false;
    }
}