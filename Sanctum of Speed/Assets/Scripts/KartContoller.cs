using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DrivingStates { GROUNDEDDRIVING, DRIFTING, INTHEAIR }

public class KartContoller : MonoBehaviour
{
    public DrivingStates state;
    public CharacterController kart;
    public Transform kartBody;

    public CapsuleCollider frontLeftWheel;
    public CapsuleCollider frontRightWheel;
    public CapsuleCollider backLeftWheel;
    public CapsuleCollider backRightWheel;


    public float topSpeed = 60f;
    public float turnSpeedMultiplier;
    public float currentSpeed = 0;
    public float acceleration = 2;
    public float deceleration = 2;
    public float neutralDeceleration = .1f;
    public float x;
    public float z;

    public Transform groundCheck;
    public float groundDistance = 1f;
    public LayerMask groundMask;
    bool isGrounded;
    public Vector3 forwardDirectionOnJump;

    public float jumpHeight = 3f;
    Vector3 velocity;
    public float gravity = -9.81f;


    public bool beganDrifting = false;
    public bool isPressingDriftButton = false;
    public float directionOfKartOnImpact;
    public bool wasInTheAirBeforeGrounded;
    // Start is called before the first frame update
    void Start()
    {
        state = DrivingStates.GROUNDEDDRIVING;
        currentSpeed = 0;
        groundCheck = backRightWheel.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(state);
        switch (state)
        {
            case DrivingStates.GROUNDEDDRIVING:
                HandleDriving();
                break;

            case DrivingStates.DRIFTING:
                HandleDrifting();
                break;
        }



    }

    public void HandleDriving()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        Vector3 forwardDirection = transform.forward; //I dont multiply by x because I use speed to determine how fast i should move
        Vector3 rightDirection = transform.right * z;

        if (z > 0)
        {
            if (currentSpeed < topSpeed)
            {
                currentSpeed += acceleration;
            }


        }

        if (z == 0)
        {
            currentSpeed -= neutralDeceleration;
            if (currentSpeed <= 5)
            {
                currentSpeed = 0;
            }
        }

        if (z < 0)
        {
            if (currentSpeed > topSpeed * -1)
            {
                currentSpeed -= deceleration;
            }
        }

        if (currentSpeed != 0)
        {

            turnSpeedMultiplier = currentSpeed / 200;
            kartBody.Rotate(Vector3.up * x * turnSpeedMultiplier);

        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            forwardDirectionOnJump = transform.forward;
            
        }
        if (Input.GetButtonDown("Jump"))
        {
            isPressingDriftButton = true;
        }
        if (Input.GetButtonUp("Jump"))
        {
            isPressingDriftButton = false;
        }
        if (isGrounded)
        {

            kart.Move(forwardDirection * currentSpeed * Time.deltaTime); //move on x axis
            

            if (wasInTheAirBeforeGrounded && isPressingDriftButton == true && x != 0)
            {
                
                directionOfKartOnImpact = x;
                state = DrivingStates.DRIFTING;
            }

            wasInTheAirBeforeGrounded = false;
        }
        if (!isGrounded)
        {
            wasInTheAirBeforeGrounded = true;
            kart.Move(forwardDirectionOnJump * currentSpeed * Time.deltaTime);

        }

        velocity.y += gravity * Time.deltaTime; //gravity move on y axis
        kart.Move(velocity * Time.deltaTime); //timedeltatime^2 thats why its twice
    }

    public void HandleDrifting()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        Vector3 forwardDirection = transform.forward; 
        Vector3 rightDirection = transform.right * z;

        if (z > 0)
        {
            if (currentSpeed < topSpeed)
            {
                currentSpeed += acceleration;
            }


        }

        if (z == 0)
        {
            currentSpeed -= neutralDeceleration;
            if (currentSpeed <= 5)
            {
                currentSpeed = 0;
            }
        }

        if (z < 0)
        {
            if (currentSpeed > topSpeed * -1)
            {
                currentSpeed -= deceleration;
            }
        }

        if (currentSpeed != 0)
        {

            turnSpeedMultiplier = currentSpeed / 200;
            kartBody.Rotate(Vector3.up * x * turnSpeedMultiplier);

        }

        if (isGrounded)
        {

            kart.Move(forwardDirection * currentSpeed * Time.deltaTime); //move on x axis

        }

        velocity.y += gravity * Time.deltaTime; //gravity move on y axis
        kart.Move(velocity * Time.deltaTime); //timedeltatime^2 thats why its twice

        if (Input.GetButtonUp("Jump"))
        {
            state = DrivingStates.GROUNDEDDRIVING;
        }
    }

}
