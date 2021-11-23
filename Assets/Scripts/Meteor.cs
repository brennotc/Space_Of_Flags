using UnityEngine;

public class Meteor : MonoBehaviour
{
    public Vector3 offset;

    void Update() {
       transform.position += offset; 

       if(transform.position.y <= -71)
       {
          transform.position += new Vector3(0, 133, 0);
       }     
    }
}
