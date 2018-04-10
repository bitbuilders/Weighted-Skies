using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        Game.Instance.LoadScene("MainWorld");
    }

    public void Quit()
    {
        Game.Instance.QuitGame();
    }
}
