using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

    public class MainMenu : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider slider;
    public TMP_Text progressText;

    public void GoToMainMenu() 
    {
    // timescale set back to 1 in case the user exits to the main menu from the pause menu, ensures game time isn't perma-slow
    // until the escape button is hit again
        Time.timeScale = 1f;
        ResetAll();
        StartCoroutine(LoadAsynchronously(0));


    }
    public void PlayGame()
    {
        // Using the SceneManager, get the current scene's index and increment to go to the next scene: playable game    
        // Note: ONLY do this within the Main Menu Scene (this is the only case it makes sense)
        
        StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().buildIndex + 1));
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReloadGame() 
    {
        ResetAll();
        StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().buildIndex));
    }

    void ResetAll() {
        GameManager.Instance.ResetInstance();
        UIManager.Instance.ResetInstance();
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingScreen.SetActive(true);

        while(!operation.isDone) 
        {
            //operation.progress (float from 0 to 1 where from 0 - 0.9 it is actually loading and from 0.9 - 1 it is activating)
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // This is what we want to use since we get values from 0 to 1 instead of 0 to .9 and then the activation happens
            slider.value = progress;
            progressText.text = (int)(progress * 100f) + "%";
            yield return null; // wait until the next frame to continue
        }
    } 
}