using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DrivingStates { GROUNDEDDRIVING, BEGINDRIFTING, DRIFTING, INTHEAIR }

public class KartContoller : MonoBehaviour
{
    public DrivingStates state;
    public CharacterController kart;
    public Transform kartBody;
    public Transform kartBodyTransform;

    public CapsuleCollider frontLeftWheel;
    public CapsuleCollider frontRightWheel;
    public CapsuleCollider backLeftWheel;
    public CapsuleCollider backRightWheel;
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform backLeftWheelTransform;
    public Transform backRightWheelTransform;
    public Transform frontLeftWheelTransformOnStart;


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
    public Vector3 forwardDirectionOnDrift;

    public float jumpHeight = 3f;
    Vector3 velocity;
    public float gravity = -9.81f;


    public bool beganDrifting = false;

    public float directionOfKartOnImpact;

    public bool pressingJump;
    public bool justJumped;

    float yRotation;
    float kartClampWhenDrifting;
    float xWhenJumping;

    // Start is called before the first frame update
    void Start()
    {
        state = DrivingStates.GROUNDEDDRIVING;
        currentSpeed = 0;
        groundCheck = backRightWheel.gameObject.transform;

        frontLeftWheelTransform = frontLeftWheel.gameObject.transform;
        frontRightWheelTransform = frontRightWheel.gameObject.transform;
        backLeftWheelTransform = backLeftWheel.gameObject.transform;
        backRightWheelTransform = backRightWheel.gameObject.transform;

        frontLeftWheelTransformOnStart = frontLeftWheel.gameObject.transform; 
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

            case DrivingStates.INTHEAIR:
                HandleAirMovement();
                break;

            case DrivingStates.BEGINDRIFTING:
                HandleBeginDrifting();
                break;
        }

        if (x != 0)
        {
            yRotation = x * 15;
        }

        x = Input.GetAxis("Horizontal");

        yRotation = Mathf.Clamp(yRotation, -25f, 25f);
        frontLeftWheelTransform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        frontRightWheelTransform.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        if (currentSpeed > topSpeed)
        {
            currentSpeed = topSpeed;
        }

        
        if (Input.GetKeyUp("joystick button 0"))
        {
            z = 0;
        }
        if (Input.GetKey("joystick button 1"))
        {
            z = -1;
        }
        if (Input.GetKey("joystick button 0"))
        {
            z = 1;
        }
        if (Input.GetKeyUp("joystick button 1"))
        {
            z = 0;
        }

    }

    public void HandleDriving()
    {
        kartBodyTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        

        //z = Input.GetAxis("Vertical");

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
            if (currentSpeed > 0)
            {
                turnSpeedMultiplier = 60 / (currentSpeed + 500);
                kartBody.Rotate(Vector3.up * x * turnSpeedMultiplier);
            }
            if (currentSpeed < 0)
            {
                turnSpeedMultiplier = 60 / (currentSpeed - 300);
                kartBody.Rotate(Vector3.up * x * turnSpeedMultiplier);
                
            }


        }

        if (Input.GetAxis("Jump") != 0 && isGrounded && currentSpeed > 30)
        {
            xWhenJumping = Input.GetAxisRaw("Horizontal");
            forwardDirectionOnJump = transform.forward;
            pressingJump = true;
            justJumped = false;
            kartClampWhenDrifting = 0;
            state = DrivingStates.BEGINDRIFTING;

        }


        if (isGrounded)
        {
            kart.Move(forwardDirection.normalized * currentSpeed * Time.deltaTime); //move on x axis
        }

        if (!isGrounded)
        {
            forwardDirectionOnJump = transform.forward;
            state = DrivingStates.INTHEAIR;

        }

        velocity.y += gravity * Time.deltaTime; //gravity move on y axis
        kart.Move(velocity * Time.deltaTime); //timedeltatime^2 thats why its twice
    }

    public void HandleDrifting()
    {
        if (z == 0)
        {
            currentSpeed -= neutralDeceleration;
        }
        if (z == 1)
        {
            currentSpeed += acceleration;
        }
        if (z == -1)
        {
            currentSpeed -= deceleration;
        }
        if (currentSpeed < 10)
        {
            state = DrivingStates.GROUNDEDDRIVING;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (xWhenJumping > 0)
        {
            x = Mathf.Clamp(x, .2f, 1);
            kartClampWhenDrifting = Mathf.Clamp(kartClampWhenDrifting, 30, 35);
        }
        if (xWhenJumping < 0)
        {
            x = Mathf.Clamp(x, -1f, -.2f);
            kartClampWhenDrifting = Mathf.Clamp(kartClampWhenDrifting, -35, -30);
        }
        
        turnSpeedMultiplier = 60 / (currentSpeed + 250);
        
        
        

        kartBody.Rotate(Vector3.up * x * turnSpeedMultiplier);
        kartBodyTransform.localRotation = Quaternion.Euler(0f, kartClampWhenDrifting, 0f);

        Vector3 forwardDirection = transform.forward;
        
        kart.Move(forwardDirection.normalized * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime; //gravity move on y axis
        kart.Move(velocity * Time.deltaTime); //timedeltatime^2 thats why its twice

        if (Input.GetAxisRaw("Jump") == 0)
        {
            state = DrivingStates.GROUNDEDDRIVING;
        }
    }

    public void HandleAirMovement()
    {
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            state = DrivingStates.GROUNDEDDRIVING;
        }

        kart.Move(forwardDirectionOnJump * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime; //gravity move on y axis
        kart.Move(velocity * Time.deltaTime); //timedeltatime^2 thats why its twice



    }

    public void HandleBeginDrifting()
    {

        x = Input.GetAxisRaw("Horizontal");
        
        kartBodyTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }



        if (isGrounded && pressingJump == true && justJumped == false)
        {
            
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            xWhenJumping = Input.GetAxisRaw("Horizontal");

        }

        if (Input.GetAxisRaw("Jump") == 0)
        {
            pressingJump = false;
        }

        if (!isGrounded)
        {
            
            justJumped = true;
        }

        if (justJumped && isGrounded && pressingJump && xWhenJumping != 0 && currentSpeed > 30)
        {
            
            state = DrivingStates.DRIFTING;
        }

        if (pressingJump == false && isGrounded)
        {
            state = DrivingStates.GROUNDEDDRIVING;
        }

        if (isGrounded && pressingJump == true && justJumped == true && xWhenJumping == 0)
        {
            state = DrivingStates.GROUNDEDDRIVING;
        }

        kart.Move(forwardDirectionOnJump.normalized * currentSpeed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime; //gravity move on y axis
        kart.Move(velocity * Time.deltaTime); //timedeltatime^2 thats why its twice

       
        kartClampWhenDrifting += xWhenJumping / 2;
        kartClampWhenDrifting = Mathf.Clamp(kartClampWhenDrifting, -35, 35);

        kartBodyTransform.localRotation = Quaternion.Euler(0f, kartClampWhenDrifting, 0f);
        
    }

}
