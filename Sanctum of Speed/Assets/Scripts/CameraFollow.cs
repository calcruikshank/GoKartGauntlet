using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject kart;
    public float carX;
    public float carY;
    public float carZ;
    public GameObject kartBase;
    public float kartBaseX;
    public float kartBaseY;
    public float kartBaseZ;
    public float differenceInY;
    
    public float differenceInYOnCatchUp;

    public bool goRight = false;
    public bool goLeft = false;

    public float cameraRotationIncrement;
    // Update is called once per frame

    public void Start()
    {
        
    }
    void Update()
    {
        kartBaseX = kartBase.transform.eulerAngles.x;
        kartBaseY = kartBase.transform.eulerAngles.y;
        kartBaseZ = kartBase.transform.eulerAngles.z;

        carX = kart.transform.eulerAngles.x;
        carY = kart.transform.eulerAngles.y;
        carZ = kart.transform.eulerAngles.z;

        
        differenceInY = kartBaseY - carY;
        transform.eulerAngles = new Vector3(0f, kartBaseY, 0f);
       /* if ((transform.eulerAngles.y - kartBaseY) <= 1 && (transform.eulerAngles.y - kartBaseY) >= -1)
        {
            transform.eulerAngles = new Vector3(kartBaseX, kartBaseY, kartBaseZ);
            goRight = false;
            goLeft = false;
            transform.eulerAngles = new Vector3(kartBaseX - kartBaseX, kartBaseY, kartBaseZ - kartBaseZ);

        }
        if (goRight)
        {
            
            cameraRotationIncrement -= -.03f;
            //cameraRotationIncrement = Mathf.Clamp(cameraRotationIncrement, .2f, 3f);
            transform.eulerAngles = new Vector3(kartBaseX - kartBaseX, transform.eulerAngles.y + cameraRotationIncrement, kartBaseZ - kartBaseZ);
            
        }
        if (goLeft)
        {
            
            cameraRotationIncrement -= -.03f;
            //cameraRotationIncrement = Mathf.Clamp(cameraRotationIncrement, .2f, 3f);
            transform.eulerAngles = new Vector3(kartBaseX - kartBaseX, transform.eulerAngles.y - cameraRotationIncrement, kartBaseZ - kartBaseZ);
            
        }*/

        

        
    }

    public void StopFollowingRotation()
    {
        

        transform.eulerAngles = new Vector3(carX - carX, carY - carY, carZ - carZ);
    }

    public void CatchUpToTarget(float direction)
    {
       /* differenceInYOnCatchUp = kartBaseY - carY;
        if ((int)differenceInYOnCatchUp != 0 && direction > 0)
        {
            goRight = true;
            cameraRotationIncrement = .5f;
        }
        if ((int)differenceInYOnCatchUp != 0 && direction < 0)
        {
            goLeft = true;
            cameraRotationIncrement = .5f;
        }


        

        if ((int)transform.eulerAngles.y - (int)carY <= 2 && (int)transform.eulerAngles.y - (int)carY >= -2)
        {
            transform.eulerAngles = new Vector3(kartBaseX - kartBaseX, kartBaseY, kartBaseZ - kartBaseZ);
            
        }*/

    }
}
