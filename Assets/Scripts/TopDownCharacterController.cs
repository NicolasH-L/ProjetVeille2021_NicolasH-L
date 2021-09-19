using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{
    private static TopDownCharacterController _player;
    public static TopDownCharacterController GetPlayerInstance => _player;
    private const string KeyMoveRight = "d";
    private const string KeyMoveLeft = "a";
    private const string Bullet = "Bullet";
    private const string TeleportationPoint = "TeleportationPoint";
    private const string AttackInputKey = "space";
    private const float PlayerSpeed = 3f;
    private const float DelayTime = 0.6f;
    private const int MaxHealth = 100;
    private const int SoundEffectPlayerHit = 0;
    private const int SoundEffectMeleeHit = 1;
    private const int PlayerScreamSfx = 2;
    private const int PlayerScreamSfx2 = 3;

    [SerializeField] private GameObject judahWeapon;
    [SerializeField] private HealthBar healthBar;

    private Animator _animatorPlayer;
    private List<Animator> _liste;
    private AudioSource[] _audioSource;
    private int _currentMaxHealth;
    private int _currentHealth;
    private int _audioClipIndex;
    private int _currentPlayerSfx;
    private bool _isMovingRight;
    private bool _hasAttacked;
    private bool _isHurtSoundPlayed;

    private static readonly int IsIdle = Animator.StringToHash("isIdle");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");

    public delegate void GameFinished(bool isDead);

    public event GameFinished OnGameEnded;

    private void Awake()
    {
        if (_player != null && _player != this)
            Destroy(gameObject);
        else
            _player = this;
    }

    private void Start()
    {
        if (GameManager.GameManagerInstance != null)
            OnGameEnded += GameManager.GameManagerInstance.GameOver;
        _isMovingRight = true;
        _liste = new List<Animator>();
        _liste.AddRange(GetComponentsInChildren<Animator>());
        _animatorPlayer = _liste[1];
        _audioSource = GetComponents<AudioSource>();
        _currentPlayerSfx = PlayerScreamSfx;
        _currentMaxHealth = MaxHealth;
        _currentHealth = MaxHealth;
        healthBar.SetMaxValue(_currentHealth);
        healthBar.SetValue(_currentHealth);
    }

    private void Update()
    {
        Vector2 dir = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
            dir.x = -1;
        else if (Input.GetKey(KeyCode.D))
            dir.x = 1;

        if (Input.GetKey(KeyCode.W))
            dir.y = 1;
        else if (Input.GetKey(KeyCode.S))
            dir.y = -1;
        dir.Normalize();
        GetComponent<Rigidbody2D>().velocity = PlayerSpeed * dir;

        if (Input.GetKey(AttackInputKey) && !_hasAttacked)
            Attack();
        else if (Input.GetKeyUp(AttackInputKey) ||
                 !Input.GetKey(AttackInputKey))
            judahWeapon.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyMoveRight) && !_isMovingRight && !Input.GetKey(KeyMoveLeft))
            ChangeDirection();
        else if (Input.GetKey(KeyMoveLeft) && _isMovingRight && !Input.GetKey(KeyMoveRight))
            ChangeDirection();
    }

    private void Attack()
    {
        if (_hasAttacked)
            return;
        _animatorPlayer.SetTrigger(AttackTrigger);
        _audioSource[SoundEffectMeleeHit].Play();
        ChangeAttackAudio();
        judahWeapon.SetActive(true);
        _hasAttacked = true;
        StartCoroutine(ResetDelayNextAttack());
    }

    private void ChangeAttackAudio()
    {
        _currentPlayerSfx = _currentPlayerSfx == PlayerScreamSfx ? PlayerScreamSfx2 : PlayerScreamSfx;
        _audioSource[_currentPlayerSfx].Play();
    }

    private IEnumerator ResetDelayNextAttack()
    {
        yield return new WaitForSeconds(DelayTime);
        _hasAttacked = false;
    }

    private void TakeDamage(int damage)
    {
        if (!_isHurtSoundPlayed)
        {
            _audioSource[SoundEffectPlayerHit].Play();
            StartCoroutine(DelayHurtSound());
        }

        _currentHealth -= damage;
        if (_currentHealth == 0)
            OnGameEnded?.Invoke(true);
        healthBar.SetValue(_currentHealth);
    }

    private IEnumerator DelayHurtSound()
    {
        yield return new WaitForSeconds(DelayTime);
        _isHurtSoundPlayed = false;
    }

    private void GainHP(int value)
    {
        _currentHealth = healthBar.GetCurrentValue() + value >= healthBar.GetCurrentMaxValue()
            ? healthBar.GetCurrentMaxValue()
            : healthBar.GetCurrentValue() + value;
        healthBar.SetValue(_currentHealth);
    }

    private void ChangeDirection()
    {
        transform.Rotate(0, 180, 0);
        _isMovingRight = !_isMovingRight;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case Bullet:
                transform.GetComponent<SpriteRenderer>().color = Color.red;
                TakeDamage(10);
                break;
            case TeleportationPoint:
                var manager = GameManager.GameManagerInstance;
                if (GameManager.GameManagerInstance == null)
                    return;
                manager.OnLevelEndReached += manager.NextLevel;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
            transform.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            transform.GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            transform.GetComponent<SpriteRenderer>().color = Color.white;
    }
}