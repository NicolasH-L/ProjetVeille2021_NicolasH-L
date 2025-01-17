﻿using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Player
{
    public class TopDownCharacterController : MonoBehaviour
    {
        private static TopDownCharacterController _player;
        public static TopDownCharacterController GetPlayerInstance => _player;
        private const string KeyMoveRight = "d";
        private const string KeyMoveLeft = "a";
        private const string BulletAlien = "BulletAlien";
        private const string BulletTroll = "BulletTroll";
        private const string BulletWizard = "BulletWizard";
        private const string SpecialBullet = "SpecialBullet";
        private const string Food = "Food";
        private const string TeleportationPoint = "TeleportationPoint";
        private const string AttackInputKey = "space";
        private const string AlienEnemyTag = "AlienEnemy";
        private const string TrollEnemyTag = "TrollEnemy";
        private const string WizardEnemyTag = "WizardEnemy";
        private const string SpecialEnemyTag = "SpecialEnemy";
        private const string BossEnemyTag = "Boss";
        private const float DelayTime = 0.6f;
        private const int WeaponBaseDamage = 30;
        private const int ContactDamage = 10;
        private const int TrollDamage = 5;
        private const int AlienDamage = 10;
        private const int WizardDamage = 10;
        private const int SpecialEnemyDamage = 20;
        private const int AlienBulletDamage = 8;
        private const int TrollBulletDamage = 5;
        private const int WizardBulletDamage = 10;
        private const int SpecialBulletDamage = 20;
        private const int MaxHealth = 100;
        private const int FoodHealth = 20;
        private const int SoundEffectPlayerHit = 0;
        private const int SoundEffectMeleeHit = 1;
        private const int PlayerScreamSfx = 2;
        private const int PlayerScreamSfx2 = 3;

        [SerializeField] private GameObject judahWeapon;
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private float playerSpeed = 3f;

        private Animator _animatorPlayer;
        private List<Animator> _liste;
        private AudioSource[] _audioSource;
        private int _currentHealth;
        private int _weaponDamage;
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
            _currentHealth = MaxHealth;
            _weaponDamage = WeaponBaseDamage;
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
            GetComponent<Rigidbody2D>().velocity = playerSpeed * dir;

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

        public int GetWeaponDamage()
        {
            return _weaponDamage;
        }

        public void TakeDamage(int damage)
        {
            if (!_isHurtSoundPlayed)
            {
                transform.GetComponent<SpriteRenderer>().color = Color.red;
                _audioSource[SoundEffectPlayerHit].Play();
                StartCoroutine(DelayHurtSound());
            }

            _currentHealth -= damage;
            if (_currentHealth <= 0)
                OnGameEnded?.Invoke(true);
            healthBar.SetValue(_currentHealth);
        }

        private IEnumerator DelayHurtSound()
        {
            yield return new WaitForSeconds(DelayTime);
            _isHurtSoundPlayed = false;
        }

        private void GainHp(int value)
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            switch (other.gameObject.tag)
            {
                case AlienEnemyTag:
                    TakeDamage(AlienDamage);
                    break;
                case WizardEnemyTag:
                    TakeDamage(WizardDamage);
                    break;
                case TrollEnemyTag:
                    TakeDamage(TrollDamage);
                    break;
                case SpecialEnemyTag:
                    TakeDamage(SpecialEnemyDamage);
                    break;
                case BossEnemyTag:
                    TakeDamage(ContactDamage);
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            switch (other.gameObject.tag)
            {
                case BulletAlien:
                    TakeDamage(AlienBulletDamage);
                    break;
                case BulletTroll:
                    TakeDamage(TrollBulletDamage);
                    break;
                case BulletWizard:
                    TakeDamage(WizardBulletDamage);
                    break;
                case SpecialBullet:
                    TakeDamage(SpecialBulletDamage);
                    break;
                case Food:
                    GainHp(FoodHealth);
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
            switch (other.gameObject.tag)
            {
                case BulletAlien:
                case BulletTroll:
                case BulletWizard:
                case SpecialBullet:
                    transform.GetComponent<SpriteRenderer>().color = Color.white;
                    break;
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            switch (other.gameObject.tag)
            {
                case AlienEnemyTag:
                case WizardEnemyTag:
                case TrollEnemyTag:
                case SpecialEnemyTag:
                case BossEnemyTag:
                    transform.GetComponent<SpriteRenderer>().color = Color.red;
                    break;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            switch (other.gameObject.tag)
            {
                case AlienEnemyTag:
                case WizardEnemyTag:
                case TrollEnemyTag:
                case SpecialEnemyTag:
                case BossEnemyTag:
                    transform.GetComponent<SpriteRenderer>().color = Color.white;
                    break;
            }
        }
    }
}