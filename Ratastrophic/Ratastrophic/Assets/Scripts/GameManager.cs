using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool charging_station_available = true;
    public bool timer_finished = true;
    public bool timer_on = false;
    private float timer_end;

    [Header("Pantallas UI")]
    public GameObject winScreen;
    public GameObject loseScreen;


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

    void Update() {
        if(timer_on) {
            if (Time.time > timer_end) {
                timer_on = false;
                timer_finished = true;
            }
        }
    }

    public void ChargingTimer() {
        timer_on = true;
        timer_finished = false;
        float timer_start = Time.time;
        float timer_duration = Random.Range(10f, 15f);
        timer_end = timer_start + timer_duration;
    }

    public void TriggerWin()
    {
        Debug.Log("Has escapado con exito!");
        Time.timeScale = 1f;
        SceneManager.LoadScene("WinScreen");
    }

    public void TriggerGameOver()
    {
        Debug.Log("Un enemigo te ha atrapado!");
        Time.timeScale = 1f;
        SceneManager.LoadScene("LoseScreen");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }
}