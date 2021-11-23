using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager2 : MonoBehaviour
{
    public float colorVelocity;

    private GameObject firstColor;
    private GameObject secondColor;
    private GameObject thirdColor;
    private GameObject completeFlag;
    private GameObject flagCompleteInvisible;    
    
    private int buttonVelocity = 2;
    private int beginX = 100;
    private Transform transformBase;

    private float dt;

    // array for y position of emptyFlag object (place where the colors must group to make the flag)
    private int[] yLocation = new int[4]{-40, -14, 14, 40};

    // matrix to know wich color is visible in wich flag and wich side the flag is visible
    private bool[][] visibleFlag = { 
        // order: { color1, color2, color3, left, right }
        new bool[] { false, false, false, false, false}, // flag 0
        new bool[] { false, false, false, false, false}, // flag 1
        new bool[] { false, false, false, false, false}, // flag 2
        new bool[] { false, false, false, false, false}  // flag 3
    };

    private GameObject[] colorsList;

    private int countEnd = 0;

    private string[] colorNames = new string[3]{"First Color", "Second Color", "Third Color"};

    // Right Side

    private int colorIndexRight;
    private GameObject colorRight;
    private bool colorActiveRight = false;
    private float halfSizeYColorRight;
    private float xRight;
    private float zRight;

    public GameObject upButtonRight;
    public GameObject downButtonRight;

    private GameObject[] emptyFlagsRight = new GameObject[4];
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

    private GameObject[] emptyFlagsLeft = new GameObject[4];
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
        flagCompleteInvisible = Resources.Load<GameObject>("Prefabs/Countries/" + selectedFlagIndex + "/Flag Complete Invisible");

        GameObject[] list = { firstColor, secondColor, thirdColor };

        colorsList = list;

        // initialize the places where the colors must group to make the flag and wich flags are visible in each side
        for (int i = 0; i < 4; i++)
        {
            emptyFlagsRight[i] = Instantiate(flagCompleteInvisible, transformBase); 
            emptyFlagsRight[i].transform.position += new Vector3(25, yLocation[i], 0);

            emptyFlagsLeft[i] = Instantiate(flagCompleteInvisible, transformBase); 
            emptyFlagsLeft[i].transform.position += new Vector3(-25, yLocation[i], 0);
            emptyFlagsLeft[i].transform.Rotate(0, 0, 180);

            if ( yLocation[i] < 0)
            {
                // visible in left
                visibleFlag [i][3] = true;
            } 
            else
            {
                // visible in right
                visibleFlag [i][4] = true;
            }

        }

        verifyVisibilityOfFlags();

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

        if (!colorActiveRight && verifyEmptyFlagsVisible(4))
        {
            colorsListRight = verifyListOfColor (emptyFlagsRight);

            // create new color piece to move if there is no active one, there is empty spaces for color and the list has color still
            if (colorsListRight != null && colorsListRight.Count > 0) 
            {
                colorIndexRight = Random.Range(0, colorsListRight.Count);
                colorRight = Instantiate(colorsListRight[colorIndexRight].Value, transformBase);

                halfSizeYColorRight = colorRight.GetComponent<Renderer>().bounds.size.x/2;
                xRight = beginX + halfSizeYColorRight;

                zRight = colorRight.transform.position.z;

                colorRight.transform.position = new Vector3(xRight, 0, zRight);
                colorActiveRight = true;

                if (!upButtonRight.activeSelf)
                {
                    // activate the buttons if they aren't actives
                    upButtonRight.SetActive(true);
                    downButtonRight.SetActive(true);
                }
            }
        }
        else if (verifyEmptyFlagsVisible(4))
        {
            // if there is color piece active and empty spaces for color
            // move and check if the color is in the place where the colors must group to make the flag
            colorRight.transform.position -= new Vector3(colorVelocity * dt, 0, 0);

            if (colorRight.transform.position.x <= 25 && colorRight.transform.position.x > 24.9) 
            {
                positionYRight = inPosition (emptyFlagsRight, colorRight);

                if (positionYRight != -100) 
                {
                    flagIndexRight = flagIndex (positionYRight, emptyFlagsRight);

                    emptyFlagIndexFreeRight = verifyColorInFlag (flagIndexRight, colorsListRight, colorIndexRight);

                    if (emptyFlagIndexFreeRight) 
                    {
                        colorActiveRight = false;
                        Destroy(colorRight);
                        verifyEmptyFlags(flagIndexRight);

                        changePosition(flagIndexRight);

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
        else if (!verifyEmptyFlagsVisible(4))
        {
            // deactivate the buttons if there are no empty spaces for color
            upButtonRight.SetActive(false);
            downButtonRight.SetActive(false);
        }

        // Left Side

        if (!colorActiveLeft && verifyEmptyFlagsVisible(3))
        {
            colorsListLeft = verifyListOfColor (emptyFlagsLeft);
            
            // create new color piece to move if there is no active one, there is empty spaces for color and the list has color still
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

                if (!upButtonLeft.activeSelf)
                {
                    // activate the buttons if they aren't actives
                    upButtonLeft.SetActive(true);
                    downButtonLeft.SetActive(true);
                }
            }
        } 
        else if (verifyEmptyFlagsVisible(3))
        {
            // if there is color piece active and empty spaces for color
            // move and check if the color is in the place where the colors must group to make the flag
            colorLeft.transform.position += new Vector3(colorVelocity * dt, 0, 0);

            if (colorLeft.transform.position.x >= -25 && colorLeft.transform.position.x < -24.9) 
            {
                positionYLeft = inPosition (emptyFlagsLeft, colorLeft);

                if (positionYLeft != -100) 
                {
                    flagIndexLeft = flagIndex (positionYLeft, emptyFlagsLeft);

                    emptyFlagIndexFreeLeft = verifyColorInFlag (flagIndexLeft, colorsListLeft, colorIndexLeft);

                    if (emptyFlagIndexFreeLeft) 
                    {
                        colorActiveLeft = false;
                        Destroy(colorLeft);
                        verifyEmptyFlags(flagIndexLeft);

                        changePosition(flagIndexLeft);

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
        else if (!verifyEmptyFlagsVisible(3))
        {
            // deactivate the buttons if there are no empty spaces for color
            upButtonLeft.SetActive(false);
            downButtonLeft.SetActive(false);
        }
    }

    void buttonLeftClicked(float buttonVelocityLeft) 
    {
        if (colorActiveLeft)
        {
            colorLeft.transform.position += new Vector3(0, buttonVelocityLeft, 0);
        }
    }

    void buttonRightClicked(float buttonVelocityRight) 
    {
        if (colorActiveRight)
        {
            colorRight.transform.position += new Vector3(0, buttonVelocityRight, 0);  
        }    
    }

    // change the visibility of the flags. If is visible in left, turns to visibile in right and vice versa.
    void changePosition (int flagIndex)
    {
        if (visibleFlag[flagIndex][3])
        {
            visibleFlag[flagIndex][3] = false;
            visibleFlag[flagIndex][4] = true;
        } 
        else if (visibleFlag[flagIndex][4])
        {
            visibleFlag[flagIndex][4] = false;
            visibleFlag[flagIndex][3] = true;
        }

        verifyVisibilityOfFlags();
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

    // return the index of yLocation wich value is equal to positionY
    int flagIndex (float positionY, GameObject[] emptyFlags)
    {
        for (int i = 0; i < 4; i++)
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
        for (int i = 0; i < 4; i++)
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
    bool verifyColorInFlag (int flagIndex, List<KeyValuePair<int, GameObject>> colorsListSide, int colorIndex)
    {
        if (flagIndex != -1)
        {
            if (!visibleFlag[flagIndex][colorsListSide[colorIndex].Key])
            {
                visibleFlag[flagIndex][colorsListSide[colorIndex].Key] = true;
                return true;
            }
        }

        return false;
    }    

    // verify if the flag is with all the colors. If yes, instantiate the object with the flag complete and a frame
    void verifyEmptyFlags(int flagIndex)
    {
        int x = -100;

        if ( visibleFlag[flagIndex][0] && visibleFlag[flagIndex][1] && visibleFlag[flagIndex][2] )
        {
            if (visibleFlag[flagIndex][3]) {
                x = -25;
            } else if (visibleFlag[flagIndex][4]) {
                x = 25;
            }

            GameObject completeFlagObj = Instantiate(completeFlag, transformBase); 
            completeFlagObj.transform.position += new Vector3(x, yLocation[flagIndex], 0);

            if (x == -25)
            {
                completeFlagObj.transform.Rotate(0, 0, 180);
            }

            visibleFlag[flagIndex][3] = false;
            visibleFlag[flagIndex][4] = false;

            verifyVisibilityOfFlags();
        }

    }

    // verify if there is at leat one empty space for color in one side
    bool verifyEmptyFlagsVisible (int indexSide)
    {
        return visibleFlag[0][indexSide] || visibleFlag[1][indexSide] || visibleFlag[2][indexSide] || visibleFlag[3][indexSide];
    }


    // verify if the color is in all flags. If yes, remove the color from the list of possible colors to make
    List<KeyValuePair<int, GameObject>> verifyListOfColor (GameObject[] emptyFlags)
    {
        Transform[] objTr = new Transform[4];

        List<KeyValuePair<int, GameObject>> newList = new List<KeyValuePair<int, GameObject>>();

        bool[] colorVisible = { true, true, true };

        for (int i = 0; i < 4; i++)
        {
            objTr = emptyFlags[i].transform.GetComponentsInChildren<Transform>(true);

            if (objTr[0].gameObject.activeSelf)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!visibleFlag[i][j])
                    {
                        if (colorVisible[j])
                        {
                            colorVisible[j] = false;
                            newList.Add(new KeyValuePair< int, GameObject >( j, colorsList[j] ));
                        }
                    }
                }
            }
        }
        return newList;
    }    

    // verify wich colors are visible and wich side the flags are visible to show them
    void verifyVisibilityOfFlags ()
    {
        Transform[] objTrLeft = new Transform[4];
        Transform[] objTrRight = new Transform[4];

        Transform trLeft;
        Transform trRight;

        for (int i = 0; i < 4; i++)
        {
            objTrLeft = emptyFlagsLeft[i].transform.GetComponentsInChildren<Transform>(true);
            objTrRight = emptyFlagsRight[i].transform.GetComponentsInChildren<Transform>(true);

            for (int j = 1; j < 4; j++)
            {
                if (visibleFlag[i][j-1]) 
                {
                    trLeft = emptyFlagsLeft[i].transform.Find(colorNames[j-1]).GetComponent<Transform>();
                    trRight = emptyFlagsRight[i].transform.Find(colorNames[j-1]).GetComponent<Transform>();

                    trLeft.gameObject.SetActive(true);
                    trRight.gameObject.SetActive(true);
                }
            }

            objTrLeft[0].gameObject.SetActive(visibleFlag[i][3]);
            objTrRight[0].gameObject.SetActive(visibleFlag[i][4]);
        }
    }
}
