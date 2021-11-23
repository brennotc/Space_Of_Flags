using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerPhaseSelection : MonoBehaviour
{
    public GameObject phase02;
    public GameObject phase03;
    public GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        phase02.GetComponent<Button>().onClick.AddListener(() => play("Phase02"));
        phase03.GetComponent<Button>().onClick.AddListener(() => play("Phase03"));
        menu.GetComponent<Button>().onClick.AddListener(() => menuScene());
    } 

    void play (string phase)
    {
        SceneManager.LoadScene(phase);
    }

    void menuScene ()
    {
        SceneManager.LoadScene("Menu");
    }
}
