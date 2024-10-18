using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private GameObject _timerText;

    [SerializeField]
    private GameObject _finishedGame;

    private bool _isTimerActive = false;
    private float _timer = 5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }

        _timerText.SetActive(_isTimerActive);
        if (!_isTimerActive)
        {
            _timer = 5;
            _timerText.SetActive(false);
            return;
        }
        _timer -= Time.deltaTime;
        _timerText.GetComponent<TextMeshProUGUI>().text = ((int)_timer+1).ToString();
        if (_timer <= 0)
        {
            NextLevel();
        }
    }

    public void EndLevel()
    {
        _isTimerActive = true; 
        _timerText.SetActive(_isTimerActive);
    }

    public void CancelEndLevel()
    {
        _isTimerActive = false;
    }

    private void NextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex < 3)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            _finishedGame.SetActive(true);
            _isTimerActive = false;
        }
    }
}
