using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BTN : MonoBehaviour
{
    void BTNStart()
    {
        SceneManager.LoadScene("Play");
    }
    void BTNQuit()
    {
        Application.Quit();
    }
}
