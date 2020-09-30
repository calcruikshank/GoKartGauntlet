using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject kart;
    public float carX;
    public float carY;
    public float carZ;


    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopFollowingRotation()
    {
        carX = kart.transform.eulerAngles.x;
        carY = kart.transform.eulerAngles.y;
        carZ = kart.transform.eulerAngles.z;

        transform.eulerAngles = new Vector3(carX - carX, carY - carY, carZ - carZ);
    }
}
