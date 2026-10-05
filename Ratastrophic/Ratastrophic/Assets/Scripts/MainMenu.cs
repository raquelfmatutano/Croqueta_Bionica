using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Controls()
    {
        //SceneManager.LoadScene("Controls_screen");
    }
}