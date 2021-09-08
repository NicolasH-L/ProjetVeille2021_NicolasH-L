using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{
    private const string KeyMoveRight = "d";
    private const string KeyMoveLeft = "a";
    private const float PlayerSpeed = 3f;
    private Animator _animatorPlayer;
    private bool _isMovingRight;

    private static readonly int IsIdle = Animator.StringToHash("isIdle");

    public delegate void GameFinished(bool isDead);

    public event GameFinished OnGameEnded;

    private void Start()
    {
        // if (GameManager.GameManagerInstance != null)
        // {
        //     OnGameEnded += GameManager.GameManagerInstance.GameOver;
        // }
        _isMovingRight = true;
        _animatorPlayer = GetComponent<Animator>();
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

        // if (Input.GetKeyUp(KeyMoveRight) || Input.GetKeyUp(KeyMoveLeft) || !Input.GetKey(KeyMoveRight) &&
        //     !Input.GetKey(KeyMoveLeft))
        //     SetIdle(true);
    }

    private void FixedUpdate()
    {
        var movementPlayerX = Input.GetAxis("Horizontal") * Time.deltaTime * PlayerSpeed;
        // if (movementPlayerX == 0) return;
        // SetIdle(false);
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