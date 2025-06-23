using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartStop
{
    public void HandleStartClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void HandleStopClick()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_STANDALONE
        
        Application.Quit();

#endif
    }
}
