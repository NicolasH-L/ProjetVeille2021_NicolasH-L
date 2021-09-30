using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        private const string DestroyGameManager = "Game Manager";
        private const string DestroyPlayer = "PlayerTeri";
        private const string DestroyPlayerUI = "PlayerUI";
        private const string WelcomeScreenMenu = "WelcomeScreenMenu";
        private const int FinalLevelScene = 3;
        [SerializeField] private GameObject pauseMenuUI;
        private bool _isGamePaused;

        private void Start()
        {
            Time.timeScale = 1f;
            _isGamePaused = false;
            pauseMenuUI.SetActive(false);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            if (_isGamePaused)
                Resume();
            else
                Pause();
        }

        public void Resume()
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _isGamePaused = false;
        }

        public void Pause()
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            _isGamePaused = true;
        }

        public void LoadMenu()
        {
            Destroy(GameObject.Find(DestroyGameManager));
            Destroy(pauseMenuUI);
            Destroy(GameObject.Find(DestroyPlayer));
            Destroy(GameObject.Find(DestroyPlayerUI));
            SceneManager.LoadScene(WelcomeScreenMenu);
        }

        public void QuitGame()
        {
            Application.Quit();
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
            Destroy(gameObject);
        }
    }
}