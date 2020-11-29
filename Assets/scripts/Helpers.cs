using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers {

    public static void SetPositionX(Transform whichTransform, float xpos)
    {
        whichTransform.position = new Vector3(xpos, whichTransform.position.y, whichTransform.position.z);
        //vectorxyz

    }
    public static void SetPositionY(Transform whichTransform, float ypos)
    {
        whichTransform.position = new Vector3(whichTransform.position.x, ypos, whichTransform.position.z);
        //vectorxyz

    }
    public static void SetPositionZ(Transform whichTransform, float zpos)
    {
        whichTransform.position = new Vector3(whichTransform.position.x, whichTransform.position.y, zpos);
        //vectorxyz

    }
    
}
