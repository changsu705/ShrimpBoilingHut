using UnityEngine;
using UnityEngine.SceneManagement; 

public class LobbyManager : MonoBehaviour
{
    
    public GameObject settingsPanel; 

    public void OnClickGameStart() 
    {
        SceneManager.LoadScene("GameInPlayScene");
    }
   
    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }
}