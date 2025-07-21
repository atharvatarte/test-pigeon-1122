using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUiManager : MonoBehaviour
{
    public void OnClickOnStartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickOnQuit()
    {
        Application.Quit(); 
    }
}
