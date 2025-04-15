using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Transform rpm;
    public void ReplayButton(){
        rpm.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void PlayGame(){
        SceneManager.LoadScene("Pong");
    }

    public void QuitGame(){
        Debug.Log("Quit");
        Application.Quit();
    }
}
