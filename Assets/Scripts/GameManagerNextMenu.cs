using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerNextMenu : MonoBehaviour
{
    public int index;
    public GameObject restart;
    public GameObject next;
    public GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        restart.GetComponent<Button>().onClick.AddListener(() => play(index));
        next.GetComponent<Button>().onClick.AddListener(() => play(index+2));
        menu.GetComponent<Button>().onClick.AddListener(() => menuScene());
    } 

    void play (int scene)
    {
        SceneManager.LoadScene(scene);
    }

    void menuScene ()
    {
        SceneManager.LoadScene("Menu");
    }
}
