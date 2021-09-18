using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    private const string WelcomeScreenSceneIndex = "WelcomeScreenMenu";
    [SerializeField] private AudioClip music;
    private AudioSource _audioSource;
    
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (music == null || _audioSource == null) return;
        _audioSource.clip = music;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    public void MainMenu()
    {
        _audioSource.Stop();
        SceneManager.LoadScene(WelcomeScreenSceneIndex);
    }

    public void QuitGame()
    {
        _audioSource.Stop();
        Application.Quit();
    }
}
