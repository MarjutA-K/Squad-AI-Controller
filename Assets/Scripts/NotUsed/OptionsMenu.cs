using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        optionsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OpenMenu();
        }
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
        isOpen = false;
    }
}
