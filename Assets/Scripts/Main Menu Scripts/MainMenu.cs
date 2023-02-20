using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class MainMenu : MonoBehaviour
{
    public void GoToMainMenu() 
    {
        ResetAll();
        SceneManager.LoadScene("MainMenu");
    }
    public void PlayGame()
    {
        // Using the SceneManager, get the current scene's index and increment to go to the next scene: playable game    
        // Note: ONLY do this within the Main Menu Scene (this is the only case it makes sense)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReloadGame() 
    {
        ResetAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ResetAll() {
        GameManager.Instance.ResetInstance();
        UIManager.Instance.ResetInstance();
    }
}