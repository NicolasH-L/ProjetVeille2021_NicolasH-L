using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;
    private Camera _playerCamera;
    private Canvas _pauseMenu;
    private GameObject _playerSpawnLocation;
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
        // SceneManager.sceneLoaded += GetPlayer;
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
        // DontDestroyOnLoad(_player);
        DontDestroyOnLoad(_playerSpawnLocation);
        DontDestroyOnLoad(_playerCamera);
        DontDestroyOnLoad(_pauseMenu);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        // _player.transform.position = _playerSpawnLocation.transform.position;
        StartCoroutine(DelayEndReachedReset());
    }

    private IEnumerator DelayEndReachedReset()
    {
        yield return new WaitForSeconds(5f);
        _isEndReached = false;
    }

    public void GameOver(bool isDead)
    {
        
    }
}