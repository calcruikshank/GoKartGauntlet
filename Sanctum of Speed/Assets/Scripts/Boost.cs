using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    public CharacterController kart;
    public KartContoller kartController; //I just realized its kart contoller not kart controller but its not a huge deal
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        //Debug.Log(collision + " Collided");
        if (collision == kart)
        {
            Debug.Log(collision + " Collided");
            kartController.state = DrivingStates.BOOSTING;
            kartController.justJumped = false;
        }
    }
    void OnTriggerExit(Collider collision)
    {
        //Debug.Log(collision + " Collided");
        if (collision == kart)
        {
            Debug.Log(collision + " Collided");
            kartController.state = DrivingStates.BOOSTING;
            kartController.justJumped = false;
        }
    }
}
