using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class MainMenu : MonoBehaviour
{
    public void goToTutorial()
    {
        SceneManager.LoadScene("TutorialMenu");
    }
    public void goToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void playGame()
    {
        // Using the SceneManager, get the current scene's index and increment to go to the next scene: playable game    
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void quitGame()
    {
        Application.Quit();
    }
}