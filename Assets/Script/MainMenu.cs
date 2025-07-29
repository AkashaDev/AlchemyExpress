using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
        
    }
    
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("game sudah diexit!");
    }
}
