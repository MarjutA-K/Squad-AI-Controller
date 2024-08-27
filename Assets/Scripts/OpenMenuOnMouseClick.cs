using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMenuOnMouseClick : MonoBehaviour
{
    public GameObject commandMenu1;
    public GameObject commandMenu2;
    public GameObject commandMenu3;
    public GameObject commandMenu4;

    // Start is called before the first frame update
    void Start()
    {
       commandMenu1.SetActive(false);
       commandMenu2.SetActive(false);
       commandMenu3.SetActive(false);
       commandMenu4.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { // if left button pressed
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.CompareTag("Squad1"))
                {
                    OpenCommandMenu1();
                }
                
                if(hit.collider.CompareTag("Squad2"))
                {
                    OpenCommandMenu2();
                }
        
                if (hit.collider.CompareTag("Squad3"))
                {
                    OpenCommandMenu3();
                }

                if (hit.collider.CompareTag("Squad4"))
                {
                    OpenCommandMenu4();
                }
            }
        }
    }

    public void OpenCommandMenu1()
    {
        commandMenu1.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenCommandMenu2()
    {
        commandMenu2.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenCommandMenu3()
    {
        commandMenu3.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenCommandMenu4()
    {
        commandMenu4.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMenu()
    {
        commandMenu1.SetActive(false);
        commandMenu2.SetActive(false);
        commandMenu3.SetActive(false);
        commandMenu4.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }
}
