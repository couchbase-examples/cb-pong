using UnityEngine;
using UnityEngine.SceneManagement;

public class AfterReplay : MonoBehaviour
{
    public void ReplayAgain(){
        SceneManager.LoadScene("ReplayPong");
    }

    public void MainMenu(){
        SceneManager.LoadScene("MainMenu");
    }
}
