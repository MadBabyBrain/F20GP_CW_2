using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLogic : MonoBehaviour
{
    public void StartGame()
    {
        // transform.Find("Playing").gameObject.SetActive(true);
        // transform.Find("MainMainMenu").gameObject.SetActive(false);
        transform.Find("MainMenu").gameObject.SetActive(true);
        GameObject.Find("Main").GetComponent<MapGenerator>().begin();
    }

    public void OpenOptions()
    {
        transform.Find("MainMainMenu").gameObject.SetActive(false);
        transform.Find("MainMenu").gameObject.SetActive(true);
    }

    public void goBack()
    {
        transform.Find("MainMainMenu").gameObject.SetActive(true);
        transform.Find("MainMenu").gameObject.SetActive(false);
    }
    public void QuitGame()
    {
        Debug.Log("quit game");
        Application.Quit();
    }
}
