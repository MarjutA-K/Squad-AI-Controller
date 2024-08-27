using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RadialMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    bool isOpen;


    // Start is called before the first frame update
    void Start()
    {
        optionsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    public void OpenMenu()
    {
        optionsMenu.SetActive(true);
        Time.timeScale = 0f;
        isOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMenu()
    {
        optionsMenu.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    public void GoBack()
    {
        optionsMenu.SetActive(false);
        Time.timeScale = 0f;
        isOpen = false;
        Cursor.visible = true;
    }
}
