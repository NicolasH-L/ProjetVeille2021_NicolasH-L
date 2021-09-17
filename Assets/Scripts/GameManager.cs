using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;
    public static GameManager GameManagerInstance => _gameManager;

    private const int FinalLevelScene = 3;
    private const int GameEndSceneIndex = 4;
    private const int GameOverSceneIndex = 5;
    private const string MainCamera = "MainCamera";
    private const string PlayerSpawnLocationTag = "PlayerSpawn";
    private const string PauseMenuTag = "PauseMenu";
    private const string PlayerUiTag = "PlayerUI";

    private Camera _playerCamera;
    private Canvas _pauseMenu;
    private Canvas _canvas;
    private GameObject _playerSpawnLocation;
    private TopDownCharacterController _player;
    private int _currentLevel;
    private bool _isEndReached;

    public delegate void LoadNextLevel();

    public event LoadNextLevel OnLevelEndReached;

    private void Awake()
    {
        if (_gameManager != null && _gameManager != this)
            Destroy(gameObject);
        else
            _gameManager = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        OnLevelEndReached?.Invoke();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.sceneLoaded += GetPlayer;
        ++_currentLevel;

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NextLevel()
    {
        OnLevelEndReached -= NextLevel;
        if (_isEndReached)
            return;
        _isEndReached = true;
        ++_currentLevel;
        DontDestroyOnLoad(_player);
        DontDestroyOnLoad(_playerSpawnLocation);
        DontDestroyOnLoad(_playerCamera);
        // DontDestroyOnLoad(_pauseMenu);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        _player.transform.position = _playerSpawnLocation.transform.position;
        StartCoroutine(DelayEndReachedReset());
    }

    private IEnumerator DelayEndReachedReset()
    {
        yield return new WaitForSeconds(5f);
        _isEndReached = false;
    }

    private void GetPlayer(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex > FinalLevelScene ||
            SceneManager.GetActiveScene().buildIndex == 0) return;
        _player = TopDownCharacterController.GetPlayerInstance;
        _playerCamera = Camera.main;
        _playerSpawnLocation = GameObject.FindGameObjectWithTag(PlayerSpawnLocationTag);
        _canvas = GameObject.FindGameObjectWithTag(PlayerUiTag).GetComponent<Canvas>();
        _pauseMenu = GameObject.FindGameObjectWithTag(PauseMenuTag).GetComponent<Canvas>();
    }
    public void GameOver(bool isDead)
    {
        var index = GameEndSceneIndex;
        if (isDead)
            index = GameOverSceneIndex;
        Destroy(GameObject.FindGameObjectWithTag(MainCamera));
    }
    
    private IEnumerator LoadSc(int index)
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(index);
    }
}