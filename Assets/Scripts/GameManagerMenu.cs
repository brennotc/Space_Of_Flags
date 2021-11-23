using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerMenu : MonoBehaviour
{
    public GameObject play;
    public GameObject phases;
    public GameObject flag;
    public GameObject end;

    public int selectedIndex;

    // Start is called before the first frame update
    void Start()
    {
        play.GetComponent<Button>().onClick.AddListener(() => game());
        phases.GetComponent<Button>().onClick.AddListener(() => phaseSelection());
        flag.GetComponent<Button>().onClick.AddListener(() => flagSelection());
        end.GetComponent<Button>().onClick.AddListener(() => exit());
        
    } 

    void game ()
    {
        SceneManager.LoadScene("Phase01");
    }

    void phaseSelection ()
    {
        SceneManager.LoadScene("PhaseSelection");
    }

    void flagSelection ()
    {
        SceneManager.LoadScene("FlagSelection");
    }

    void exit ()
    {
        Application.Quit();
    }
}
