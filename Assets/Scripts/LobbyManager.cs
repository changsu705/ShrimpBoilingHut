using UnityEngine;
using UnityEngine.SceneManagement; 

public class LobbyManager : MonoBehaviour
{
    public void OnClickGameStart() 
    {
        SceneManager.LoadScene("GameInPlayScene");
    }
   
    public void OnClickQuit()
    {
      
        Application.Quit();
    }
}