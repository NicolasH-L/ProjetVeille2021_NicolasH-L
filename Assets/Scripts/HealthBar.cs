using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private const int FinalLevelScene = 3;
    private Slider _slider;
    private int _currentValue;
    private int _currentMaxValue;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SetMaxValue(int maxValue)
    {
        _slider.maxValue = maxValue;
        _currentMaxValue = maxValue;
    }

    public void SetValue(int value)
    {
        _slider.value = value;
        _currentValue = value;
    }

    public int GetCurrentMaxValue()
    {
        return _currentMaxValue;
    }

    public int GetCurrentValue()
    {
        return _currentValue;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().buildIndex <= FinalLevelScene) return;
        Destroy(gameObject);
        Destroy(this);
    }
}