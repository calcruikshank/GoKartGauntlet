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
    public CameraFollow cameraFollow;

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
    float xWhenLanding;
    public GameObject backLeftSparks;
    public GameObject backRightSparks;
    public GameObject leftBooster;
    public GameObject rightBooster;

    public LayerMask mask;
    public Vector3 newUp;

    public bool didPressDrift; //this is only used to make it so the player cant press drift again after pressing it once


    public GameObject kartBase;
    public float kartBaseX;
    public float kartBaseY;
    public float kartBaseZ;


    public float angleX;
    public float angleZ;

    public float timerT;
    public float timeBoostingTimer;
    public float boostAmount;
    public float timeBoosting;
    public float totalCapSpeed = 85;
    public float topSpeedMinimum;
    public bool isBoosting = false;

    public Transform rearRightTransform;
    public Transform rearLeftTransform; //used for raycasts and dont rotate like the other tire transforms
    public Transform frontLeftTransform;
    public Transform frontRightTransform;

    public float greatestAngleX;

    public float rotationOnTransformMismatch;

    public int boostStage = 0;
    // Start is called before the first frame update
    void Start()
    {
        state = DrivingStates.GROUNDEDDRIVING;
        currentSpeed = 0;
        topSpeedMinimum = topSpeed;
        float greatestAngleX = 0;

        frontLeftWheelTransform = frontLeftWheel.gameObject.transform;
        frontRightWheelTransform = frontRightWheel.gameObject.transform;
        backLeftWheelTransform = backLeftWheel.gameObject.transform;
        backRightWheelTransform = backRightWheel.gameObject.transform;

        frontLeftWheelTransformOnStart = frontLeftWheel.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        

        timeBoostingTimer += Time.deltaTime;
        GetAllignment();

        //kartBodyTransform.localRotation = Quaternion.Euler(angleX, 0f, angleZ);
        //groundCheck.rotation = Quaternion.Euler(0f, newUp.y, 0f);
        //Debug.Log(angleX + " angle x " + angleZ + " angle z ");
        //Debug.Log(state);
        //Debug.Log(angleX);
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

        yRotation = Mathf.Clamp(yRotation, -30f, 30f);
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

        frontLeftWheelTransform.Rotate(Vector3.right * currentSpeed * 8.5f);
        frontRightWheelTransform.Rotate(Vector3.right * currentSpeed * 8.5f);
        backLeftWheelTransform.Rotate(Vector3.right * currentSpeed * 8.5f);
        backRightWheelTransform.Rotate(Vector3.right * currentSpeed * 8.5f);

        kartBaseX = kart.transform.up.x;
        kartBaseY = kart.transform.up.y;
        kartBaseZ = kart.transform.up.z;

        if (timeBoostingTimer >= timeBoosting)
        {
            leftBooster.SetActive(false);
            rightBooster.SetActive(false);
        }
    }


    public void FixedUpdate()
    {

    }
    public void HandleDriving()
    {
        
        timerT = 0;
        if (topSpeed > topSpeedMinimum && timeBoostingTimer > timeBoosting)
        {
            
            topSpeed -= 40 * Time.deltaTime;
        }

        //kartBodyTransform.localRotation = Quaternion.Euler(angleX, 0f, angleZ);


        RotateBodyBack();
       
        

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -3f;
            //GetAllignment();
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
                turnSpeedMultiplier = 60 / (currentSpeed + 200);
                kartBody.Rotate(Vector3.up * x * turnSpeedMultiplier);
            }
            if (currentSpeed < 0)
            {
                turnSpeedMultiplier = 60 / (currentSpeed - 100);
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
            kartClampWhenDrifting = 0;
            didPressDrift = false;
            state = DrivingStates.INTHEAIR;

        }

        velocity.y += gravity * Time.deltaTime; //gravity move on y axis
        kart.Move(velocity * Time.deltaTime); //timedeltatime^2 thats why its twice
    }

    public void HandleDrifting()
    {
        if (topSpeed > topSpeedMinimum && timeBoostingTimer > timeBoosting)
        {
            topSpeed -= 20 * Time.deltaTime;
        }

        backLeftSparks.SetActive(true);
        backRightSparks.SetActive(true);

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


        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -3f;
        }

        if (xWhenJumping > 0)
        {
            xWhenLanding = .6f;
            xWhenLanding = Mathf.Clamp(xWhenLanding, .4f, .8f);
            kartClampWhenDrifting = 30;
        }
        if (xWhenJumping < 0)
        {
            xWhenLanding = -.6f;
            xWhenLanding = Mathf.Clamp(xWhenLanding, -.8f, -.4f);
            kartClampWhenDrifting = -30;
        }
        if (currentSpeed < 10)
        {
            backLeftSparks.SetActive(false);
            backRightSparks.SetActive(false);

            cameraFollow.CatchUpToTarget(kartClampWhenDrifting);

            state = DrivingStates.GROUNDEDDRIVING;
        }

        turnSpeedMultiplier = 60 / (currentSpeed + 100);

        xWhenLanding += x / 2;
        //Debug.Log(xWhenLanding);

        kartBody.Rotate(Vector3.up * xWhenLanding * turnSpeedMultiplier);
        //kartBodyTransform.localRotation = Quaternion.Euler(0f, kartClampWhenDrifting, 0f);

        Vector3 forwardDirection = transform.forward;

        kart.Move(forwardDirection.normalized * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime; //gravity move on y axis
        kart.Move(velocity * Time.deltaTime); //timedeltatime^2 thats why its twice

        if (Input.GetAxisRaw("Jump") == 0)
        {
            backLeftSparks.SetActive(false);
            backRightSparks.SetActive(false);

            float xWhenLandingAbsClamped = Mathf.Abs(xWhenLanding);
            xWhenLandingAbsClamped = Mathf.Clamp(xWhenLandingAbsClamped, .6f, 1f);
            //kartBody.Rotate(Vector3.up.x, kartClampWhenDrifting, Vector3.up.z);

            //cameraFollow.CatchUpToTarget(kartClampWhenDrifting);
            


            if (topSpeed < totalCapSpeed)
            {

                topSpeed += boostAmount;


            }

            if (topSpeed > totalCapSpeed)
            {
                topSpeed = totalCapSpeed;
            }
            currentSpeed = topSpeed;
            timeBoostingTimer = 0;

            //velocity.y = Mathf.Sqrt(jumpHeight * -1 * gravity);
            if (boostStage >= 1)
            {
                leftBooster.SetActive(true);
                rightBooster.SetActive(true);
            }
            
            state = DrivingStates.GROUNDEDDRIVING;
        }

        timerT += Time.deltaTime;


        if (timerT >= 1)
        {

            boostAmount = 10f;
            if (timeBoosting - timeBoostingTimer < .75f)
            {
                timeBoosting = .75f;
            }
            boostStage = 1;
        }
        if (timerT >= 2)
        {
            backLeftSparks.GetComponent<ParticleSystem>().startColor = new Color(233f / 255f, 79f / 255f, 55f / 255f);
            backRightSparks.GetComponent<ParticleSystem>().startColor = new Color(233f / 255f, 79f / 255f, 55f / 255f);
            backLeftSparks.GetComponent<ParticleSystem>().startSpeed = 9;
            backRightSparks.GetComponent<ParticleSystem>().startSpeed = 9;
            boostAmount = 15f;

            boostStage = 2;
            if (timeBoosting - timeBoostingTimer < 1.75f)
            {
                timeBoosting = 1.75f;
            }

        }
        if (timerT >= 3)
        {
            backLeftSparks.GetComponent<ParticleSystem>().startSpeed = 11;
            backRightSparks.GetComponent<ParticleSystem>().startSpeed = 11;
            backLeftSparks.GetComponent<ParticleSystem>().startColor = new Color(183 / 255f, 38f / 255f, 193f / 255f);
            backRightSparks.GetComponent<ParticleSystem>().startColor = new Color(183 / 255f, 38f / 255f, 193f / 255f);

            boostStage = 3;
            boostAmount = 20f;
            if (timeBoosting - timeBoostingTimer < 2.75f)
            {
                timeBoosting = 2.75f;
               
            }

        }
        if (timerT < 1)
        {
            backLeftSparks.GetComponent<ParticleSystem>().startSpeed = 7;
            backRightSparks.GetComponent<ParticleSystem>().startSpeed = 7;
            backLeftSparks.GetComponent<ParticleSystem>().startColor = new Color(79f / 255f, 219f / 255f, 255f / 255f);
            backRightSparks.GetComponent<ParticleSystem>().startColor = new Color(79f / 255f, 219f / 255f, 255f / 255f);


        }
        if (timerT < 1)
        {
            boostAmount = 0f;
            boostStage = 0;
        }

    }

    public void HandleAirMovement()
    {
        timerT = 0;
        if (topSpeed > topSpeedMinimum && timeBoostingTimer > timeBoosting)
        {
            topSpeed -= 20 * Time.deltaTime;
        }

        bool justPressedJump = false;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        xWhenJumping = Input.GetAxisRaw("Horizontal");
        if (Input.GetAxis("Jump") != 0 && justPressedJump == false && didPressDrift == false) //this is confusing i know the did press jump is used to see if the player is jumping from the drift button after being on the ground the just pressed jump checks to see if player pressed jump
        {


            justPressedJump = true;
        }

        if (Input.GetAxis("Jump") != 0)
        {
            kartClampWhenDrifting += xWhenJumping / 2;
            kartClampWhenDrifting = Mathf.Clamp(kartClampWhenDrifting, -30, 30);

            kartBodyTransform.localRotation = Quaternion.Euler(0f, kartClampWhenDrifting, 0f);
        }

        if (Input.GetAxis("Jump") == 0)
        {
            RotateBodyBack();
        }
        if (isGrounded && Input.GetAxis("Jump") != 0 && Mathf.Abs(kartClampWhenDrifting) > 0 && currentSpeed > 30 && kartBodyTransform.localRotation.y != 0)
        {

            state = DrivingStates.DRIFTING;

        }

        
        if (isGrounded && Input.GetAxis("Jump") == 0 || didPressDrift == true && isGrounded)
        {
            //kartBody.Rotate(Vector3.up.x, kartClampWhenDrifting, Vector3.up.z);
            state = DrivingStates.GROUNDEDDRIVING;
        }

        kart.Move(forwardDirectionOnJump * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime; //gravity move on y axis
        kart.Move(velocity * Time.deltaTime); //timedeltatime^2 thats why its twice


    }

    public void HandleBeginDrifting()
    {

        timerT = 0;
        x = Input.GetAxisRaw("Horizontal");

        //kartBodyTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -3f;
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


        if (xWhenJumping > .2f)
        {
            kartClampWhenDrifting += 1 * .4f;
        }
        if (xWhenJumping < -.2f)
        {
            kartClampWhenDrifting -= 1 * .4f;
        }
        //kartClampWhenDrifting += 1 * xWhenJumping / 2;
        kartClampWhenDrifting = Mathf.Clamp(kartClampWhenDrifting, -30, 30);

        kartBodyTransform.localRotation = Quaternion.Euler(0f, kartClampWhenDrifting, 0f);
        //Debug.Log(kartClampWhenDrifting);
        if (justJumped && isGrounded && pressingJump && Mathf.Abs(kartClampWhenDrifting) > 0 && currentSpeed > 30) //you can also do Mathf.Abs(kartClampWhenDrifting) > 0 instead of x 
        {
            timerT = 0;
            state = DrivingStates.DRIFTING;
        }

        if (justJumped && isGrounded && Mathf.Abs(kartClampWhenDrifting) <= 25)
        {
            cameraFollow.CatchUpToTarget(kartClampWhenDrifting);
            kartBody.Rotate(Vector3.up.x, kartClampWhenDrifting, Vector3.up.z);
            state = DrivingStates.GROUNDEDDRIVING;
        }

        if (pressingJump == false && isGrounded)
        {
            cameraFollow.CatchUpToTarget(kartClampWhenDrifting);
            kartBody.Rotate(Vector3.up.x, kartClampWhenDrifting, Vector3.up.z);
            state = DrivingStates.GROUNDEDDRIVING;
        }
        if (pressingJump == false && !isGrounded)
        {
            backLeftSparks.SetActive(false);
            backRightSparks.SetActive(false);
            //kartBodyTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            //kartBody.Rotate(Vector3.up.x, kartClampWhenDrifting, Vector3.up.z);


            //cameraFollow.CatchUpToTarget(kartClampWhenDrifting);
            kartClampWhenDrifting = 0;
            didPressDrift = true;
            state = DrivingStates.INTHEAIR;

        }

        if (isGrounded && pressingJump == true && justJumped == true && xWhenJumping == 0)
        {

            state = DrivingStates.GROUNDEDDRIVING;
        }

        kart.Move(forwardDirectionOnJump.normalized * currentSpeed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime; //gravity move on y axis
        kart.Move(velocity * Time.deltaTime); //timedeltatime^2 thats why its twice


    }

    public void GetAllignment()
    {

        RaycastHit hit;
        RaycastHit hit2;
        RaycastHit hit3;
        RaycastHit hit4;




        Physics.Raycast(frontRightTransform.position, -transform.up, out hit2, .1f, mask);
        Physics.Raycast(rearLeftTransform.position, -transform.up, out hit3, .100f, mask);
        Physics.Raycast(rearRightTransform.position, -transform.up, out hit4, .100f, mask);
        Physics.Raycast(frontLeftTransform.position, -transform.up, out hit, .100f, mask);



        //transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal + hit2.normal + hit3.normal + hit4.normal) * transform.rotation; //none of the bs below really matters this is the sauce it adds new transform to current

        //newUp = hit.normal + hit2.normal + hit3.normal + hit4.normal;
        /*if (Physics.Raycast(frontLeftTransform.position, -transform.up, out hit4, .1f, mask))
        {
            


           
            
            
        }*/
        newUp.x = hit.normal.x + hit2.normal.x + hit3.normal.x + hit4.normal.x;
        //newUp.x = kartBaseX;
        newUp.y = hit.normal.y + hit2.normal.y + hit3.normal.y + hit4.normal.y;
        //newUp.y = kartBaseY;
        newUp.z = hit.normal.z + hit2.normal.z + hit3.normal.z + hit4.normal.z;
        //newUp.z = kartBaseZ;
        transform.localRotation = Quaternion.FromToRotation(transform.up, newUp) * transform.rotation;

        //transform.rotation = Quaternion.FromToRotation(transform.up, newAllignment) * transform.rotation;



        /*if (Physics.Raycast(frontLeftTransform.position, -transform.up, out hit, 101f, mask))
        {
            Physics.Raycast(frontRightTransform.position, -transform.up, out hit2, 100f, mask);
            Physics.Raycast(rearLeftTransform.position, -transform.up, out hit3, 100f, mask);
            Physics.Raycast(rearRightTransform.position, -transform.up, out hit4, 100f, mask);

        }*/



        /*f (Physics.Raycast(frontLeftTransform.position, -transform.up, out hit2, 0.1f, mask))
        {
            Physics.Raycast(frontRightTransform.position, -transform.up, out hit, 100f, mask);
            Physics.Raycast(rearLeftTransform.position, -transform.up, out hit3, 100f, mask);
            Physics.Raycast(rearRightTransform.position, -transform.up, out hit4, 100f, mask);

        }
        if (Physics.Raycast(frontLeftTransform.position, -transform.up, out hit3, 0.1f, mask))
        {
            Physics.Raycast(frontRightTransform.position, -transform.up, out hit, 100f, mask);
            Physics.Raycast(rearLeftTransform.position, -transform.up, out hit2, 100f, mask);
            Physics.Raycast(rearRightTransform.position, -transform.up, out hit4, 100f, mask);

        }
        if (Physics.Raycast(frontLeftTransform.position, -transform.up, out hit4, 0.1f, mask))
        {
            Physics.Raycast(frontRightTransform.position, -transform.up, out hit, 100f, mask);
            Physics.Raycast(rearLeftTransform.position, -transform.up, out hit2, 100f, mask);
            Physics.Raycast(rearRightTransform.position, -transform.up, out hit3, 100f, mask);

        }*/



        //newUp.x = hit.normal.x;
        //float radiansX = Mathf.Atan(newUp.x);
        //angleX = radiansX * (180 / Mathf.PI);

        //newUp.y = kartBaseY;
        //groundCheck.localRotation = Quaternion.Euler(0f, newUp.y, 0f);

        //newUp.z = hit.normal.z;
        // float radiansZ = Mathf.Atan(newUp.z);
        //angleZ = radiansZ * (180 / Mathf.PI);



        //if (Mathf.Abs(angleX) > greatestAngleX)
        //{
        // greatestAngleX = Mathf.Abs(angleX);
        //}
        //transform.up = newUp;

        //Debug.Log(angleX + " angle x       " + angleZ + "angle zz");
        //Debug.Log(timeBoosting - timeBoostingTimer);
        //Debug.Log(greatestAngleX);
    }

    
    public void RotateBodyBack()
    {
        if (kartBodyTransform.localEulerAngles != new Vector3(0f, 0f, 0f))
        {

            rotationOnTransformMismatch = kartBodyTransform.localEulerAngles.y;
            if (rotationOnTransformMismatch < 180)
            {
                rotationOnTransformMismatch -= 200 * Time.deltaTime;
                yRotation = -30;
            }
            if (rotationOnTransformMismatch > 180)
            {
                rotationOnTransformMismatch += 200 * Time.deltaTime;
                yRotation = 30;
            }

            if (rotationOnTransformMismatch < 5 && rotationOnTransformMismatch > -5)
            {
                kartBodyTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
                yRotation = x * 15;
                yRotation = Mathf.Clamp(yRotation, -30f, 30f);
            }
            Debug.Log("transforms dont match " + kartBodyTransform.localRotation.y + " " + rotationOnTransformMismatch);
            //kartBodyTransform.localRotation = Quaternion.Euler(0f, kartBodyTransform.localRotation.y, 0f);

            kartBodyTransform.localEulerAngles = new Vector3(0f, rotationOnTransformMismatch, 0f);

        }

        if (kartBodyTransform.localEulerAngles.y < 5 && kartBodyTransform.localEulerAngles.y > -5)
        {
            kartBodyTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
            yRotation = x * 15;
            yRotation = Mathf.Clamp(yRotation, -30f, 30f);
        }
    }
}
