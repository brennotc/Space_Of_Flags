using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerFlagSelection : MonoBehaviour
{
    public GameObject menu;
    public GameObject frame;
    public GameObject[] flagsButton;
    public Text selectedText;
    public static int selectedIndex = 0;

    private List<KeyValuePair<string, Vector3>> flags = new List<KeyValuePair<string, Vector3>>();
    private GameObject frameObj;
    private Transform transformBase;
    private bool changeSelected;

    private GameObject[] flagSelectionObjs;

    // Start is called before the first frame update
    void Start()
    {
        menu.GetComponent<Button>().onClick.AddListener(() => menuScene());

        for (int i = 0; i < flagsButton.Length; i++)
        {
            int realIndex = i;
            flagsButton[i].GetComponent<Button>().onClick.AddListener(() => flagButton(realIndex));
        }

        flags.Add( new KeyValuePair< string, Vector3 >( "BRASIL", new Vector3(-25, -16, 0) ) );
        flags.Add( new KeyValuePair< string, Vector3 >( "COLÔMBIA", new Vector3(25, -16, 0) ) );

        changeSelected = false;

        frameObj = Instantiate(frame, transformBase);
        frameObj.transform.position = flags[selectedIndex].Value;

        selectedText.text = "SELECIONADA: " + flags[selectedIndex].Key;
        verifyButtons();
    } 

    void FixedUpdate() 
    {
        // change informations if the flag changes
        if (changeSelected)
        {
            frameObj.transform.position = flags[selectedIndex].Value;
            selectedText.text = "SELECIONADA: " + flags[selectedIndex].Key;
            changeSelected = false;            
        }       

    }

    public void flagButton (int buttonIndex) 
    {
        selectedIndex = buttonIndex;
        verifyButtons();

        changeSelected = true;
    }

    void menuScene()
    {
        SceneManager.LoadScene("Menu");
    }

    // verify buttons for each flag. If the flag is selected, deactivate the button, if no, activate
    void verifyButtons() 
    {
        for (int i = 0; i < flagsButton.Length; i++)
        {
            flagsButton[i].GetComponent<Button>().interactable = (i != selectedIndex);
        }

    }
}
