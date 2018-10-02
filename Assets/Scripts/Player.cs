using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public static Player instance = null;

    // Modifiers for speed and max/min tilt angles.
    [SerializeField] private float xRotationSpeed = 30;
    [SerializeField] private float yRotationSpeed = -30;
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private float maxTiltAngle = 45.0f;

    // Input options for different movement.
    private InputOption horizontalRotationInput = InputOption.RIGHT_STICK_HORIZONTAL;
    private InputOption verticalRotationInput = InputOption.RIGHT_STICK_VERTICAL;
    private InputOption horizontalMovementInput = InputOption.LEFT_STICK_HORIZONTAL;
    private InputOption forwardMovementInput = InputOption.LEFT_STICK_VERTICAL;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            
        }
        else
        {
            GameObject.Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }


    }

    // Update is called once per frame
    void Update ()
    {

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {

            CheckForMove();

        }
	}

    public void CheckForMove()
    {
        if(GamepadInput.Get(InputOption.RIGHT_STICK_HORIZONTAL)){
            OnRightHorizontal(GamepadInput.GetInputValue(InputOption.RIGHT_STICK_HORIZONTAL));
        }
        if(GamepadInput.Get(InputOption.RIGHT_STICK_VERTICAL)){
            OnRightVertical(GamepadInput.GetInputValue(InputOption.RIGHT_STICK_VERTICAL));
        }
        if(GamepadInput.Get(InputOption.LEFT_STICK_HORIZONTAL)){
            OnLeftHorizontal(GamepadInput.GetInputValue(InputOption.LEFT_STICK_HORIZONTAL));
        }
        if(GamepadInput.Get(InputOption.LEFT_STICK_VERTICAL)){
            OnLeftVertical(GamepadInput.GetInputValue(InputOption.LEFT_STICK_VERTICAL));
        }
    }

    public void OnRightHorizontal(float inputAxisValue){
        RotatePlayerHorizontally(inputAxisValue);
    }
    public void OnRightVertical(float inputAxisValue){
        RotatePlayerVertically(inputAxisValue);
    }
    public void OnLeftHorizontal(float inputAxisValue){
        MovePlayer(inputAxisValue, transform.right);
    }
    public void OnLeftVertical(float inputAxisValue){
        MovePlayer(inputAxisValue, transform.forward);
    }

    // Moves the platform in a specified direction, based on an input value.
    private void MovePlayer(float inputAxisValue, Vector3 direction)
    {

        transform.position += (Time.deltaTime * inputAxisValue * movementSpeed * direction);

    }

    // Rotates the platform horizontally, based on an input value.
    private void RotatePlayerHorizontally(float inputAxisValue)
    {

        float rotateAngle = Time.deltaTime * inputAxisValue * xRotationSpeed;

        this.transform.RotateAround(transform.position, Vector3.up, rotateAngle);

    }

    // Rotates the platform vertically, with bounds to prevent full rotation.
    private void RotatePlayerVertically(float inputAxisValue)
    {

        float rotateAngle = Time.deltaTime * inputAxisValue * yRotationSpeed;

        // Rotates the platform initially.
        transform.Rotate(rotateAngle, 0, 0, Space.Self);

        // Stores the rotation of the platform, as euler angles because Quaternions are complex.
        Vector3 platformRotation = transform.localRotation.eulerAngles;

        // Keep the rotations between -180 and 180. Easier to think about.
        if (platformRotation.x > 180.0f)
        {
            platformRotation.x -= 360.0f;
        }

        // If the user attempted to tilt below the min tilt angle, snap to the min.
        if (platformRotation.x < -maxTiltAngle)
        {
            platformRotation.x = -maxTiltAngle;
        }

        // If the user attempted to tilt above the max tilt angle, snap to the max.
        else if (platformRotation.x > maxTiltAngle)
        {
            platformRotation.x = maxTiltAngle;
        }

        // Z rotation should always be 0, regardless of rotations in other axes.
        platformRotation.z = 0.0f;

        // Updates the rotation of the platform with the correct snapped rotation.
        transform.localRotation = Quaternion.Euler(platformRotation);

    }

}


