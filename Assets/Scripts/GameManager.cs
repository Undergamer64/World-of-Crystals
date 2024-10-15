using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private IEnumerator _endGameCoroutine;

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

    public void EndLevel()
    {
        if (_endGameCoroutine != null)
        {
            return;
        }
        Debug.Log("Started the timer");
        _endGameCoroutine = EndLevelCoroutine(5f);
        StartCoroutine(_endGameCoroutine);
    }

    public void CancelEndLevel()
    {
        if (_endGameCoroutine == null)
        {
            return;
        }
        StopCoroutine(_endGameCoroutine);
        _endGameCoroutine = null;
        Debug.Log("Stopped the timer");
    }

    private IEnumerator EndLevelCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        Test();
    }

    private void Test()
    {
        Debug.Log("End");
        _endGameCoroutine = null;
    }
}
