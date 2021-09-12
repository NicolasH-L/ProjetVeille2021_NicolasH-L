﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{
    private const string KeyMoveRight = "d";
    private const string KeyMoveLeft = "a";
    private const float PlayerSpeed = 3f;
    private const float DelayTime = 0.6f;
    private const string AttackInputKey = "space";

    [SerializeField] private GameObject judahWeapon;
    private Animator _animatorPlayer;
    private List<Animator> _liste;
    private bool _isMovingRight;
    private bool _hasAttacked;

    private static readonly int IsIdle = Animator.StringToHash("isIdle");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");


    public delegate void GameFinished(bool isDead);

    public event GameFinished OnGameEnded;

    private void Start()
    {
        // if (GameManager.GameManagerInstance != null)
        // {
        //     OnGameEnded += GameManager.GameManagerInstance.GameOver;
        // }
        _isMovingRight = true;
        _liste = new List<Animator>();
        _liste.AddRange(GetComponentsInChildren<Animator>());
        _animatorPlayer = _liste[1];
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

    private void Attack()
    {
        if (_hasAttacked)
            return;
        _animatorPlayer.SetTrigger(AttackTrigger);

        judahWeapon.SetActive(true);
        _hasAttacked = true;
        StartCoroutine(ResetDelayNextAttack());
    }

    private IEnumerator ResetDelayNextAttack()
    {
        yield return new WaitForSeconds(DelayTime);
        _hasAttacked = false;
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyMoveRight) && !_isMovingRight && !Input.GetKey(KeyMoveLeft))
            ChangeDirection();
        else if (Input.GetKey(KeyMoveLeft) && _isMovingRight && !Input.GetKey(KeyMoveRight))
            ChangeDirection();
    }

    private void SetIdle(bool isIdle)
    {
        _animatorPlayer.SetBool(IsIdle, isIdle);
    }

    private void ChangeDirection()
    {
        transform.Rotate(0, 180, 0);
        _isMovingRight = !_isMovingRight;
    }
}