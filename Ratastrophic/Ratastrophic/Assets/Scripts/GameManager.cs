using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Pantallas UI")]
    public GameObject winScreen;
    public GameObject loseScreen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TriggerWin()
    {
        Debug.Log("¡Has escapado con exito!");
        if (winScreen != null) winScreen.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void TriggerGameOver()
    {
        Debug.Log("¡Un enemigo te ha atrapado!");
        if (loseScreen != null) loseScreen.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}