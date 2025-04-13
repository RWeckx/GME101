using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver;
    private string _currentScene;

    private void Start()
    {
        _currentScene = SceneManager.GetActiveScene().name;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
            SceneManager.LoadSceneAsync(_currentScene);
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

}
