using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLogic : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject CreditsMenu;
    public GameObject OptionsMenu;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuButton();
    }

    public void NewGameButton()
    {
        //we will initialize our game here (so load a new scene), like:
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void LoadGameButton()
    {
        //we will initialize our game here (so load a new scene), like:
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void CreditsButton()
    {
        // shows Credits Menu
        MainMenu.SetActive(false);
        CreditsMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        MainMenu.SetActive(true);
        CreditsMenu.SetActive(false);
        OptionsMenu.SetActive(false);
    }

    public void OptionsMenuButton()
    {
        // Show Options Menu
        MainMenu.SetActive(false);
        CreditsMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }

    public void ExitButton()
    {
        //quits Game
        Application.Quit();
    }
}