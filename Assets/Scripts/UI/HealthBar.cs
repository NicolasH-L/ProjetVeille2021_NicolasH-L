using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private const int FinalLevelScene = 3;
    private Slider _slider;
    private int _currentValue;
    private int _currentMaxValue;
    public Gradient gradient;
    public Image fill;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SetMaxValue(int maxValue)
    {
        _slider.maxValue = maxValue;
        _currentMaxValue = maxValue;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetValue(int value)
    {
        _slider.value = value;
        _currentValue = value;
        fill.color = gradient.Evaluate(_slider.normalizedValue);
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