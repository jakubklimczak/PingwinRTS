using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Pause_UI_logic : MonoBehaviour
{

    public GameObject PauseUI;
    public GameObject GameplayUI;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Pause start");
        ReturnToGameplay();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ReturnToGameplay() 
    {
        UI_logic temp = FindObjectOfType<UI_logic>().gameObject.GetComponent<UI_logic>();
        temp.IsPaused = false;
        Time.timeScale = 1.0f;
        PauseUI.SetActive(false);
        GameplayUI.SetActive(true);
        Debug.Log("returning to gameplay");
    }

    public void ReturnToMenu()
    {
        UI_logic temp = FindObjectOfType<UI_logic>().gameObject.GetComponent<UI_logic>();
        temp.IsPaused = false;

        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
