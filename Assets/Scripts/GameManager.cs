using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;
    public static GameManager GameManagerInstance => _gameManager;

    private const int FinalLevelScene = 3;
    private const int GameEndSceneIndex = 4;
    private const int GameOverSceneIndex = 5;
    private const int IndexAudioSourceLevelBgm = 0;
    private const string MainCamera = "MainCamera";
    private const string PlayerSpawnLocationTag = "PlayerSpawn";
    private const string PauseMenuTag = "PauseMenu";
    private const string PlayerUiTag = "PlayerUI";
    [SerializeField] private List<AudioClip> listWelcomeBgm;
    [SerializeField] private List<AudioClip> listLevelBgm;
    private List<AudioSource> _listAudioSources;
    private AudioSource _levelAudioSource;
    private Camera _playerCamera;
    private Canvas _pauseMenu;
    private Canvas _canvas;
    private GameObject _playerSpawnLocation;
    private TopDownCharacterController _player;
    private int _currentLevel;
    private int _playingClipIndex;
    private bool _isEndReached;
    private bool _isMusicPaused;

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
        _listAudioSources = new List<AudioSource>();
        _listAudioSources.AddRange(GetComponents<AudioSource>());
        _levelAudioSource = _listAudioSources[IndexAudioSourceLevelBgm];
        _playingClipIndex = 0;
        PlayMusicWelcomeScreen();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        OnLevelEndReached?.Invoke();
    }

    public void StartGame()
    {
        _listAudioSources[IndexAudioSourceLevelBgm].Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.sceneLoaded += GetPlayer;
        ++_currentLevel;
        PlayMusic(listLevelBgm);
    }

    public void QuitGame()
    {
        _listAudioSources[IndexAudioSourceLevelBgm].Stop();
        Application.Quit();
    }

    private void PlayMusicWelcomeScreen()
    {
        if (_currentLevel != 0 || _levelAudioSource.isPlaying) return;
        var index = Random.Range(0, listWelcomeBgm.Count);
        _levelAudioSource.Stop();
        _levelAudioSource.clip = listWelcomeBgm[index];
        _levelAudioSource.Play();
        Invoke(nameof(PlayMusicWelcomeScreen), _levelAudioSource.clip.length);
    }

    private void PlayMusic(List<AudioClip> musicList)
    {
        if (musicList == null) return;

        var index = Random.Range(0, musicList.Count);
        if (_currentLevel > _playingClipIndex + 1 && !_isMusicPaused)
        {
            index = _currentLevel - 1;
            ++_playingClipIndex;
        }
        else if (_currentLevel == _playingClipIndex + 1)
            index = _currentLevel - 1;

        _levelAudioSource.clip = musicList[index];
        _levelAudioSource.loop = true;
        _levelAudioSource.Play();
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
        DontDestroyOnLoad(_canvas);
        // DontDestroyOnLoad(_pauseMenu);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        _player.transform.position = _playerSpawnLocation.transform.position;
        _playerCamera.transform.position = _playerSpawnLocation.transform.position;
        _player.gameObject.layer = 20;
        _player.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 1";
        _player.GetComponent<SpriteRenderer>().sortingOrder = 2;
        StartCoroutine(DelayEndReachedReset());
        if (!_isMusicPaused)
            PlayMusic(listLevelBgm);
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
        // _pauseMenu = GameObject.FindGameObjectWithTag(PauseMenuTag).GetComponent<Canvas>();
    }

    public int GetPlayerDamage()
    {
        return _player.GetWeaponDamage();
    }
    
    public void GameOver(bool isDead)
    {
        _listAudioSources[IndexAudioSourceLevelBgm].Stop();
        var index = GameEndSceneIndex;
        if (isDead)
            index = GameOverSceneIndex;
        
        Destroy(GameObject.FindGameObjectWithTag(MainCamera));
        // Destroy(GameObject.FindGameObjectWithTag(PauseMenuTag));
        Destroy(GameObject.FindGameObjectWithTag(PlayerUiTag));
        StartCoroutine(LoadSc(index));
    }
    
    private IEnumerator LoadSc(int index)
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(index);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().buildIndex <= FinalLevelScene) return;
        Destroy(_playerSpawnLocation);
        Destroy(gameObject);
        Destroy(this);
    }
}