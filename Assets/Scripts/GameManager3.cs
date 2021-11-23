using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager3 : MonoBehaviour
{
    public float colorVelocity;
    public GameObject emptyFlag;

    private GameObject firstColor;
    private GameObject secondColor;
    private GameObject thirdColor;
    private GameObject completeFlag;
    
    private int buttonVelocity = 2;
    private int beginX = 100;
    private Transform transformBase;

    // array for y position of emptyFlag object (place where the colors must group to make the flag)
    private int[] yLocation = new int[3]{-40, 0, 40};

    private float dt;

    private int colorIndex;
    private float halfSizeYColor;
    private bool colorActive = false;
    private float z;

    private GameObject[] emptyFlags = new GameObject[3];

    // matrix to know wich color is visible in wich flag
    private bool[][] filledColorsByFlag = { 
        // order:{ color 0, color 1, color 2 }
        new bool[] { false, false, false}, //flag 0
        new bool[] { false, false, false}, //flag 1
        new bool[] { false, false, false}  //flag 2
    };

    private bool emptyFlagIndexFree = false;
    private int flagIndexGlobal = -1;

    private List<KeyValuePair<int, GameObject>> colorsList = new List<KeyValuePair<int, GameObject>>();

    private int countEnd = 0;

    // Right Side

    private GameObject colorRight;
    private float xRight;

    public GameObject upButtonRight;
    public GameObject downButtonRight;

    private float positionYRight = -100;

    // Left Side

    private GameObject colorLeft;
    private float xLeft;

    public GameObject upButtonLeft;
    public GameObject downButtonLeft;

    private float positionYLeft = -100;   

    // Start is called before the first frame update
    void Start()
    {
        // get wich flag the game will show
        int selectedFlagIndex = GameManagerFlagSelection.selectedIndex;
        
        firstColor = Resources.Load<GameObject>("Prefabs/Countries/" + selectedFlagIndex + "/First Color");
        secondColor = Resources.Load<GameObject>("Prefabs/Countries/" + selectedFlagIndex + "/Second Color");
        thirdColor = Resources.Load<GameObject>("Prefabs/Countries/" + selectedFlagIndex + "/Third Color");
        completeFlag = Resources.Load<GameObject>("Prefabs/Countries/" + selectedFlagIndex + "/Complete Flag");
        
        GameObject[] colorsListObj = { firstColor, secondColor, thirdColor };

        // initialize the places where the colors must group to make the flag and the list of colors
        for (int i = 0; i < 3; i++)
        {
            emptyFlags[i] = Instantiate(emptyFlag, transformBase); 
            emptyFlags[i].transform.position += new Vector3(0, yLocation[i], 0);

            emptyFlags[i].transform.Rotate(0, 0, -90);

            colorsList.Add( new KeyValuePair< int, GameObject >( i, colorsListObj[i] ) );
        }

        upButtonRight.GetComponent<Button>().onClick.AddListener(() => buttonRightClicked(buttonVelocity));
        downButtonRight.GetComponent<Button>().onClick.AddListener(() => buttonRightClicked(buttonVelocity*-1));

        upButtonLeft.GetComponent<Button>().onClick.AddListener(() => buttonLeftClicked(buttonVelocity));
        downButtonLeft.GetComponent<Button>().onClick.AddListener(() => buttonLeftClicked(buttonVelocity*-1));
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        dt = Time.deltaTime;

        if (!colorActive)
        {
            // create new colors pieces to move if there is no active ones and the list has color still
            if (colorsList != null && colorsList.Count > 0) 
            {
                colorIndex = Random.Range(0, colorsList.Count);

                colorRight = Instantiate(colorsList[colorIndex].Value, transformBase);
                colorLeft = Instantiate(colorsList[colorIndex].Value, transformBase);

                halfSizeYColor = colorRight.GetComponent<Renderer>().bounds.size.x/2;
                xRight = beginX + halfSizeYColor;
                xLeft = (beginX + halfSizeYColor)*-1;

                z = colorRight.transform.position.z;

                colorRight.transform.position = new Vector3(xRight, 0, z);
                colorLeft.transform.position = new Vector3(xLeft, 0, z);
                colorRight.transform.Rotate(0, 0, -90);
                colorLeft.transform.Rotate(0, 0, -90);
                colorActive = true;
            }
            else
            {
                // finish the game 100 frames after there is no more flags to make
                if (countEnd > 100)
                {
                    finished();
                }
                
                countEnd++;
            }
        } 
        else 
        {
            // move and check if the colors are in the same place where the colors must group to make the flag
            colorRight.transform.position -= new Vector3(colorVelocity * dt, 0, 0);
            colorLeft.transform.position += new Vector3(colorVelocity * dt, 0, 0);

            if (colorRight.transform.position.x <= 0 && colorRight.transform.position.x > -0.1 && colorLeft.transform.position.x >= 0 && colorLeft.transform.position.x < 0.1) 
            {
                positionYRight = inPosition (colorRight);
                positionYLeft = inPosition (colorLeft);

                if (positionYRight != -100 && positionYLeft != -100 && positionYRight == positionYLeft) 
                {
                    flagIndexGlobal = flagIndex ();

                    emptyFlagIndexFree = verifyColorInFlag ();

                    if (emptyFlagIndexFree) 
                    {
                        colorActive = false;
                        verifyEmptyFlags ();

                        colorsList = verifyColors ();
                        emptyFlagIndexFree = false;
                        flagIndexGlobal = -1;
                    }
                }

            } 
            else if ( colorRight.transform.position.x <= -0.1 || colorLeft.transform.position.x >= 0.1 )
            {
                // blink the colors if they passes for the position
                colorBlink (colorRight, z);
                colorBlink (colorLeft, z);
            }

            if (colorRight.transform.position.x <= -20 && colorLeft.transform.position.x >= 20)
            {
                // destroy the colors if they passes for the position
                Destroy(colorRight);
                Destroy(colorLeft);
                colorActive = false;
            }

            countEnd = 0;
        }
        
    }

    public void buttonRightClicked(float buttonVelocityRight) {
        if (colorActive)
        {
            colorRight.transform.position += new Vector3(0, buttonVelocityRight, 0);
        }        
    }

    void buttonLeftClicked(float buttonVelocityLeft) {
        if (colorActive)
        {
            colorLeft.transform.position += new Vector3(0, buttonVelocityLeft, 0);
        }
    }

    void colorBlink (GameObject color, float z)
    {
        if (color.transform.position.z >= 0)
        {
            color.transform.position = new Vector3(color.transform.position.x, color.transform.position.y, -189);
        } 
        else
        {
            color.transform.position = new Vector3(color.transform.position.x, color.transform.position.y, z);
        }
    }

    void finished()
    {
        SceneManager.LoadScene(0);
    }

    // return the index of yLocation wich value is equal to positionY
    int flagIndex ()
    {
        for (int i = 0; i < 3; i++)
        {
            if ( positionYRight == yLocation[i] && emptyFlags[i] != null ) 
            {
                return i;
            } 
        }

        return -1;
    }

    // return the position Y if the color is in the same Y position of any place where the colors must group to make the flag
    float inPosition (GameObject color)
    {
        for (int i = 0; i < 3; i++)
        {
            if ( emptyFlags[i] != null)
            {
                if (emptyFlags[i].transform.position.y + 1 > color.transform.position.y && color.transform.position.y > emptyFlags[i].transform.position.y - 1)
                {
                    return emptyFlags[i].transform.position.y;
                }
            }
        }

        return -100;
    }

    // verify if the color already is in the flag to know if it is possible to put it. If no, returns true, if yes, false
    bool verifyColorInFlag ()
    {
        if (!filledColorsByFlag[flagIndexGlobal][colorsList[colorIndex].Key])
        {
            filledColorsByFlag[flagIndexGlobal][colorsList[colorIndex].Key] = true;
            return true;
        }

        return false;
    }

    // verify if the color is in all flags. If yes, remove the color from the list of possible colors to make
    List<KeyValuePair<int, GameObject>> verifyColors ()
    {
        List<KeyValuePair<int, GameObject>> newList = colorsList;

        for (int i = 0; i < newList.Count; i++) 
        {
            if ( filledColorsByFlag[0][newList[i].Key] && filledColorsByFlag[1][newList[i].Key] && filledColorsByFlag[2][newList[i].Key] )
            {
                newList.Remove(newList[i]);
                break;
            }
        }

        return newList;
    }

    // verify if the flag is with all the colors. If yes, instantiate the object with the flag complete and a frame
    void verifyEmptyFlags ()
    {
        if ( filledColorsByFlag[flagIndexGlobal][0] && filledColorsByFlag[flagIndexGlobal][1] && filledColorsByFlag[flagIndexGlobal][2] )
        {
            GameObject completeFlagObj = Instantiate(completeFlag, transformBase);
            Vector3 position = emptyFlags[flagIndexGlobal].transform.position;
            completeFlagObj.transform.position += new Vector3(position.x, position.y, 0);
            completeFlagObj.transform.Rotate(0, 0, -90);

            Destroy(emptyFlags[flagIndexGlobal]);
        }         
    }
}
