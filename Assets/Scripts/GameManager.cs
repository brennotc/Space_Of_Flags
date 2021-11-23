using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
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

    private int countEnd = 0;

    // Right Side

    private int colorIndexRight;
    private GameObject colorRight;
    private bool colorActiveRight = false;
    private float halfSizeYColorRight;
    private float xRight;
    private float zRight;

    public GameObject upButtonRight;
    public GameObject downButtonRight;

    private GameObject[] emptyFlagsRight = new GameObject[3];

    // matrix to know wich color is in wich flag
    private bool[][] filledColorsByFlagRight = { 
        // order:{ color 0, color 1, color 2 }
        new bool[] { false, false, false}, //flag 0
        new bool[] { false, false, false}, //flag 1
        new bool[] { false, false, false}  //flag 2
    };
    private bool emptyFlagIndexFreeRight = false;  
    private int flagIndexRight = -1;

    private float positionYRight = -100;

    private List<KeyValuePair<int, GameObject>> colorsListRight = new List<KeyValuePair<int, GameObject>>();

    // Left Side

    private int colorIndexLeft;
    private GameObject colorLeft;
    private bool colorActiveLeft = false;
    private float halfSizeYColorLeft;
    private float xLeft;
    private float zLeft;

    public GameObject upButtonLeft;
    public GameObject downButtonLeft;

    private GameObject[] emptyFlagsLeft = new GameObject[3];

    // matrix to know wich color is in wich flag
    private bool[][] filledColorsByFlagLeft = { 
        // order:{ color 0, color 1, color 2 }
        new bool[] { false, false, false}, //flag 0
        new bool[] { false, false, false}, //flag 1
        new bool[] { false, false, false}  //flag 2
    };
    private bool emptyFlagIndexFreeLeft = false;  
    private int flagIndexLeft = -1;

    private float positionYLeft = -100;

    private List<KeyValuePair<int, GameObject>> colorsListLeft = new List<KeyValuePair<int, GameObject>>();    

    // Start is called before the first frame update
    void Start()
    {
        // get wich flag the game will show
        int selectedFlagIndex = GameManagerFlagSelection.selectedIndex;
        
        firstColor = Resources.Load<GameObject>("Prefabs/Countries/" + selectedFlagIndex + "/First Color");
        secondColor = Resources.Load<GameObject>("Prefabs/Countries/" + selectedFlagIndex + "/Second Color");
        thirdColor = Resources.Load<GameObject>("Prefabs/Countries/" + selectedFlagIndex + "/Third Color");
        completeFlag = Resources.Load<GameObject>("Prefabs/Countries/" + selectedFlagIndex + "/Complete Flag");

        GameObject[] colorsList = { firstColor, secondColor, thirdColor };

        // initialize the places where the colors must group to make the flag and the list of colors of each side
        for (int i = 0; i < 3; i++)
        {
            emptyFlagsLeft[i] = Instantiate(emptyFlag, transformBase); 
            emptyFlagsLeft[i].transform.position += new Vector3(-25, yLocation[i], 0);

            emptyFlagsRight[i] = Instantiate(emptyFlag, transformBase); 
            emptyFlagsRight[i].transform.position += new Vector3(25, yLocation[i], 0);

            colorsListLeft.Add( new KeyValuePair< int, GameObject >( i, colorsList[i] ) );
            colorsListRight.Add( new KeyValuePair< int, GameObject >( i, colorsList[i] ) );
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

        // finish the game 100 frames after there is no more flags to make
        if (!colorActiveRight && !colorActiveLeft)
        {
            if (countEnd > 100)
            {
                finished();
            }
            
            countEnd++;
        }
        else
        {
            countEnd = 0;
        }

        // Right Side

        if (!colorActiveRight)
        {
            // create new color piece to move if there is no active one and the list has still
            if (colorsListRight != null && colorsListRight.Count > 0) 
            {
                colorIndexRight = Random.Range(0, colorsListRight.Count);

                colorRight = Instantiate(colorsListRight[colorIndexRight].Value, transformBase);

                halfSizeYColorRight = colorRight.GetComponent<Renderer>().bounds.size.x/2;
                xRight = beginX + halfSizeYColorRight;

                zRight = colorRight.transform.position.z;

                colorRight.transform.position = new Vector3(xRight, 0, zRight);
                colorActiveRight = true;
            }
            else
            {
                // deactivate the buttons if there are no more colors in the list
                upButtonRight.SetActive(false);
                downButtonRight.SetActive(false);
            }
        } 
        else 
        {
            // move and check if the color is in the place where the colors must group to make the flag
            colorRight.transform.position -= new Vector3(colorVelocity * dt, 0, 0);

            if (colorRight.transform.position.x <= 25 && colorRight.transform.position.x > 24.9) 
            {
                positionYRight = inPosition (emptyFlagsRight, colorRight);

                if (positionYRight != -100) 
                {
                    flagIndexRight = flagIndex (positionYRight, emptyFlagsRight);

                    emptyFlagIndexFreeRight = verifyColorInFlag (filledColorsByFlagRight, colorsListRight, colorIndexRight, flagIndexRight);

                    if (emptyFlagIndexFreeRight) 
                    {
                        colorActiveRight = false;
                        verifyEmptyFlags (filledColorsByFlagRight, emptyFlagsRight, flagIndexRight);

                        colorsListRight = verifyColors (filledColorsByFlagRight, colorsListRight);
                        emptyFlagIndexFreeRight = false;
                        flagIndexRight = -1;
                    }
                }

            } 
            else if ( colorRight.transform.position.x <= 24 )
            {
                // blink the color if it passes for the position
                colorBlink (colorRight, zRight);
            }

            if (colorRight.transform.position.x <= halfSizeYColorRight)
            {
                // destroy the color if it passes for the position
                Destroy(colorRight);
                colorActiveRight = false;
            }

        }

        // Left Side

        if (!colorActiveLeft)
        {
            // create new color piece to move if there is no active one and the list has still
            if (colorsListLeft != null && colorsListLeft.Count > 0) 
            {
                colorIndexLeft = Random.Range(0, colorsListLeft.Count);

                colorLeft = Instantiate(colorsListLeft[colorIndexLeft].Value, transformBase);

                halfSizeYColorLeft = colorLeft.GetComponent<Renderer>().bounds.size.x/2;
                xLeft = (beginX + halfSizeYColorLeft)*-1;

                zLeft = colorLeft.transform.position.z;

                colorLeft.transform.position = new Vector3(xLeft, 0, zLeft);
                colorLeft.transform.Rotate(0, 0, 180);
                colorActiveLeft = true;
            }
            else
            {
                // deactivate the buttons if there are no more colors in the list
                upButtonLeft.SetActive(false);
                downButtonLeft.SetActive(false);
            }
        } 
        else 
        {
            // move and check if the color is in the place where the colors must group to make the flag
            colorLeft.transform.position += new Vector3(colorVelocity * dt, 0, 0);

            if (colorLeft.transform.position.x >= -25 && colorLeft.transform.position.x < -24.9) 
            {
                positionYLeft = inPosition (emptyFlagsLeft, colorLeft);

                if (positionYLeft != -100) 
                {
                    flagIndexLeft = flagIndex (positionYLeft, emptyFlagsLeft);

                    emptyFlagIndexFreeLeft = verifyColorInFlag (filledColorsByFlagLeft, colorsListLeft, colorIndexLeft, flagIndexLeft);

                    if (emptyFlagIndexFreeLeft) 
                    {
                        colorActiveLeft = false;
                        verifyEmptyFlags (filledColorsByFlagLeft, emptyFlagsLeft, flagIndexLeft);

                        colorsListLeft = verifyColors (filledColorsByFlagLeft, colorsListLeft);
                        emptyFlagIndexFreeLeft = false;
                        flagIndexLeft = -1;
                    }
                } 
            } 
            else if ( colorLeft.transform.position.x >= -24 )
            {
                // blink the color if it passes for the position
                colorBlink (colorLeft, zLeft);
            }

            if (colorLeft.transform.position.x >= (halfSizeYColorLeft)*-1)
            {
                // destroy the color if it passes for the position
                Destroy(colorLeft);
                colorActiveLeft = false;
            }

        }
        
    }

    public void buttonRightClicked(float buttonVelocityRight) {
        if (colorActiveRight)
        {
            colorRight.transform.position += new Vector3(0, buttonVelocityRight, 0);
        }        
    }

    void buttonLeftClicked(float buttonVelocityLeft) {
        if (colorActiveLeft)
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //return the index of yLocation wich value is equal to positionY
    int flagIndex (float positionY, GameObject[] emptyFlags)
    {
        for (int i = 0; i < 3; i++)
        {
            if ( positionY == yLocation[i] && emptyFlags[i] != null ) 
            {
                return i;
            } 
        }

        return -1;
    }

    // return the position Y if the color is in the same Y position of any place where the colors must group to make the flag
    float inPosition (GameObject[] emptyFlags, GameObject color)
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
    bool verifyColorInFlag (bool[][] filledColorsByFlag, List<KeyValuePair<int, GameObject>> colorsList, int colorIndex, int flagIndex)
    {
        if (!filledColorsByFlag[flagIndex][colorsList[colorIndex].Key])
        {
            filledColorsByFlag[flagIndex][colorsList[colorIndex].Key] = true;
            return true;
        }

        return false;
    }

    // verify if the color is in all flags. If yes, remove the color from the list of possible colors to make
    List<KeyValuePair<int, GameObject>> verifyColors (bool[][] filledColorsByFlag, List<KeyValuePair<int, GameObject>> colorsList)
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
    void verifyEmptyFlags (bool[][] filledColorsByFlag, GameObject[] emptyFlags, int flagIndex)
    {
        if ( filledColorsByFlag[flagIndex][0] && filledColorsByFlag[flagIndex][1] && filledColorsByFlag[flagIndex][2] )
        {
            GameObject completeFlagObj = Instantiate(completeFlag, transformBase);
            Vector3 position = emptyFlags[flagIndex].transform.position;
            completeFlagObj.transform.position += new Vector3(position.x, position.y, 0);

            if (position.x < 0)
            {
                completeFlagObj.transform.Rotate(0, 0, 180);
            }

            Destroy(emptyFlags[flagIndex]);
        }         
    }
}
