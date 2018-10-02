using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAVECameraRig : MonoBehaviour
{
    // Whether or not there should be a raycast from the viewpoint position.
    [SerializeField] private bool enableRaycast = false;

    // Prefab of a single CAVE screen.
    [SerializeField] private Object screenPrefab;

    // Static bool to keep track of whether or not we're in 3D mode.
    public static bool is3D = false;
    private bool using3D
    {
        get
        {
            return GameManager.instance.caveSettings.runIn3D;
        }
    }

    public static bool isCAVE
    {
        get
        {
            if (CAVECameraRig.instance != null)
            {
                return true;
            }

            return false;
        }
    }

    // Lists of left and right eye cameras.
    public static List<Camera> leftEyeCameras;
    public static List<Camera> rightEyeCameras;
    public static List<Camera> allCameras
    {

        get
        {
            List<Camera> allCameras = new List<Camera>();

            if (leftEyeCameras != null && rightEyeCameras != null)
            {
                allCameras.AddRange(leftEyeCameras);
                allCameras.AddRange(rightEyeCameras);
                return allCameras;
            }
            else
            {

                allCameras.Add(Camera.main);
            }

            return allCameras;
        }
    }

    public static Camera containerCamera;
    public static Camera frustumCamera{
        get{
            return containerCamera ? containerCamera : Camera.main;
        }
    }

    // Offset between the cameras (total offset, not halfway).
    [SerializeField]
    private float cameraEyeOffset = 0.6f;

    // Delegate that is called whenever 3D mode is toggled.
    public delegate void Toggle3DDelegate(bool is3D);
    public static Toggle3DDelegate on3DToggled;

    private static Transform CAVECameraRigTransform;

    public static Vector3 playerViewpoint
    {

        get
        {
            if (instance != null)
            {
                if (instance.CAVEViewpoint != null)
                {
                    return instance.CAVEViewpoint.transform.position;
                }
            }

            if (Camera.main != null)
            {
                return Camera.main.transform.position;
            }

            Debug.LogError("Could not find any camera as a viewpoint. Please either use a CAVE config or create a Main Camera");
            return Vector3.zero;

        }
    }

    [HideInInspector]
    public CameraViewpoint CAVEViewpoint;

    public static CAVECameraRig instance;

    // Called before first frame. Prepares the displays and loads cameras.
    private void Awake()
    {
        // Things here should only ever happen once.
        if (instance != null)
        {
            GameObject.Destroy(instance.gameObject);
        }

        instance = this;
        CAVEViewpoint = GetComponentInChildren<CameraViewpoint>();

    }

    // Ensures the cameras are in the right positions.
    void Start()
    {

        // Set up two lists for cameras.
        leftEyeCameras = new List<Camera>();
        rightEyeCameras = new List<Camera>();

        //Create the CAVE screens.
        SetupScreens();

        // Activate CAVE screens.
        ActivateDisplays();

        // Make sure the cameras have this as a parent.
        CAVECameraRigTransform = this.transform;

        // Set the cameras to default positions.
        ResetCameraPositions();

        StartCoroutine(UpdateScreens());

    }

    private void SetupScreens()
    {

        // Get the screen config loader and load the CAVE screens.
        ScreenConfigLoader screenLoader = GetComponentInChildren<ScreenConfigLoader>();
        screenLoader.LoadScreenConfig();

        // Create a parent for the screen planes.
        GameObject screenPlanes = new GameObject("Screen Planes");
        screenPlanes.transform.parent = this.transform;
        screenPlanes.transform.localPosition = Vector3.zero;
        screenPlanes.transform.localRotation = Quaternion.identity;


        // Set the position of the screen.
        Vector3 viewerPosition = screenLoader.viewerPosition.vector;
        float zVal = viewerPosition.z;
        viewerPosition.z = viewerPosition.y;
        viewerPosition.y = zVal;

        screenPlanes.transform.position -= viewerPosition;


        // Get a list of all screens found in the config.
        List<CAVEScreenConfig> screens = screenLoader.screens;

        // Set up GameObjects for the screens.
        GameObject leftEyeCameraParent = new GameObject("Left Eye Cameras");
        GameObject rightEyeCameraParent = new GameObject("Right Eye Cameras");

        // Set up the cameras in the hierarchy. Make sure their parents are the left/right camera objects.
        leftEyeCameraParent.transform.parent = CAVEViewpoint.transform;
        rightEyeCameraParent.transform.parent = CAVEViewpoint.transform;

        // Set camera parents in the same position and rotation.
        leftEyeCameraParent.transform.localPosition = Vector3.zero;
        rightEyeCameraParent.transform.localPosition = Vector3.zero;
        leftEyeCameraParent.transform.localRotation = Quaternion.identity;
        rightEyeCameraParent.transform.localRotation = Quaternion.identity;
        

        GameObject camera = new GameObject("Container Camera");
        camera.transform.parent = CAVEViewpoint.transform;
        camera.transform.localPosition = Vector3.zero;
        camera.transform.localRotation = Quaternion.identity;
        Camera camComponent = camera.AddComponent<Camera>();
        containerCamera = camComponent;
        camComponent.enabled = false;
        ContainerScreen screen = camera.AddComponent<ContainerScreen>();
        screen.projectionScreens = new List<GameObject>();


        // Iterate through all screens found in the config and load them.
        for (int i = 0; i < screens.Count; i++)
        {

            // Note we assume that this rotation either 0, 90, 180, or -90.
            float screenRotation = -float.Parse(screens[i].r);

            // Create the screen plane, name it, and set it's parent.
            GameObject newPlane = GameObject.Instantiate(screenPrefab) as GameObject;
            newPlane.transform.parent = screenPlanes.transform;
            newPlane.name = "Screen " + i;

            // Set the rotation of the screen based on heading from config file.
            Vector3 newRotation = newPlane.transform.rotation.eulerAngles;
            newRotation += new Vector3(0.0f, -float.Parse(screens[i].h), 0.0f);

            // Set the position of the screens to 0 (local space) and rotation to the heading.
            newPlane.transform.localPosition = Vector3.zero;
            newPlane.transform.localRotation = Quaternion.Euler(newRotation);

            // Move the screen from the origin to it's correct position.
            Vector3 newPos = newPlane.transform.localPosition;
            newPos += new Vector3(float.Parse(screens[i].originX), float.Parse(screens[i].originZ), float.Parse(screens[i].originY));

            newPlane.transform.localPosition = newPos;

            GameObject targetPlane = newPlane.GetComponentsInChildren<Transform>()[1].gameObject;
            
            screen.projectionScreens.Add(targetPlane);

            Camera newLeftCam = AddCamera(StereoTargetEyeMask.Left, i, leftEyeCameraParent.transform, targetPlane);
            newLeftCam.rect = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

            Camera newRightCam = AddCamera(StereoTargetEyeMask.Right, i, rightEyeCameraParent.transform, targetPlane);

            // Here we set the camera viewports based on screen rotations. The screens can't actually be "rotated" in Unity since that would rotate the cameras
            //   (Which would result in some upsidedown/sideway images on the cave. Instead we just scale each "screen" plane accordingly and set viewports.

            // If the screen rotation is -90. We use the halfway threshholds to account for the fact that floating point numbers have rounding errors.
            if (screenRotation < 0 && screenRotation < -45.0f)
            {

                if (using3D)
                {
                    newRightCam.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
                    newLeftCam.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
                }
                else
                {
                    newRightCam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                }
                // Reversed values since rotation is reversed.


                // Create the screen plane by swapping height and width, since it's rotated.
                newPlane.transform.localScale = new Vector3(float.Parse(screens[i].height), float.Parse(screens[i].width), 1.0f);

            }

            // If the screen rotation is 90. Normal for UCSD's CAVEkiosk.
            else if (screenRotation > 45.0f && screenRotation < 135.0f)
            {

                // Normal values, no reversal.
                if (using3D)
                {
                    newLeftCam.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
                    newRightCam.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
                }
                else
                {
                    newRightCam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

                }


                // Create the screen plane by swapping height and width, since it's rotated.
                newPlane.transform.localScale = new Vector3(float.Parse(screens[i].height), float.Parse(screens[i].width), 1.0f);

            }

            // If the screen rotation is 0. (Untested)
            else if (screenRotation > -45.0f && screenRotation < 45.0f)
            {

                // UNTESTED ASSUMPTION: Assuming LEFT EYE is supposed to be UPPER HALF of screen.
                // If this is NOT the case, swap the below code with the code in the 180 degree if statement.

                if (using3D)
                {
                    newLeftCam.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                    newRightCam.rect = new Rect(0.0f, 0.5f, 0.0f, 0.5f);
                }

                else
                {
                    newRightCam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                }


            }

            // If the screen rotation is 180 (Untested).
            else if (screenRotation > 135.0f && screenRotation < 225.0f)
            {

                // UNTESTED ASSUMPTION: Assuming LEFT EYE is supposed to be UPPER HALF of screen normally (without rotation).
                
                if (using3D)
                {
                    newRightCam.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                    newLeftCam.rect = new Rect(0.0f, 0.5f, 0.0f, 0.5f);
                }
                else
                {
                    newRightCam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

                }

            }

            // Set the parents of each camera.
            newLeftCam.transform.parent = leftEyeCameraParent.transform;
            newRightCam.transform.parent = rightEyeCameraParent.transform;

            // Add these cameras to the list
            leftEyeCameras.Add(newLeftCam);
            rightEyeCameras.Add(newRightCam);

        }
    }

    // Creates a new CAVE camera to render to a specific screen.
    public Camera AddCamera(StereoTargetEyeMask targetEye, int displayIndex, Transform parent, GameObject targetPlane)
    {

        // Create a camera and update it's settings.
        GameObject camera = new GameObject("Display" + displayIndex + "_" + targetEye.ToString());
        camera.transform.parent = parent;
        camera.transform.localPosition = Vector3.zero;
        camera.transform.localRotation = Quaternion.identity;
        Camera camComponent = camera.AddComponent<Camera>();
        camComponent.stereoTargetEye = targetEye;
        camComponent.targetDisplay = displayIndex;

        camera.AddComponent<CAVEScreen>().projectionScreen = targetPlane;

        return camComponent;

    }

    // Toggles between 3D and 2D mode, moves cameras appropriately.
    public void Toggle3D()
    {

        if (is3D)
        {
            Make2D();
        }

        else
        {
            Make3D();
        }

        // If there are any methods in the delegate, call the delegate for 3D transition.
        if (on3DToggled != null)
        {
            on3DToggled(is3D);
        }
    }

    public void Set3D(bool shouldBe3D)
    {

        if (shouldBe3D != is3D)
        {

            Toggle3D();

        }
    }

    // Activates each display currently hooked up to the computer, to ensure we're displaying across all screens.
    private void ActivateDisplays()
    {
        foreach (Display display in Display.displays)
        {
            display.Activate();
        }
    }

    // Shifts the cameras back to 2D mode, by setting both left and right cameras at the origin.
    private void Make2D()
    {
        ShiftCamerasByXOffset(leftEyeCameras, cameraEyeOffset / 2);
        ShiftCamerasByXOffset(rightEyeCameras, -cameraEyeOffset / 2);
        is3D = false;

        Debug.Log("Making 2D");
    }

    // Shifts the cameras to 3D mode, by offsetting left and right cameras.
    private void Make3D()
    {
        ShiftCamerasByXOffset(leftEyeCameras, -cameraEyeOffset / 2);
        ShiftCamerasByXOffset(rightEyeCameras, cameraEyeOffset / 2);
        is3D = true;

        Debug.Log("Making 3D");
    }

    // Moves the cameras back to where they should be for 2D/3D modes.
    private void ResetCameraPositions()
    {
        if (!is3D)
        {
            MoveCamerasToXPosition(leftEyeCameras, 0.0f);
            MoveCamerasToXPosition(rightEyeCameras, 0.0f);
        }
        else
        {
            MoveCamerasToXPosition(leftEyeCameras, -cameraEyeOffset / 2);
            MoveCamerasToXPosition(rightEyeCameras, cameraEyeOffset / 2);
        }
    }

    // Shifts the cameras by a specific x offset.
    private void ShiftCamerasByXOffset(List<Camera> cameras, float offset)
    {
        foreach (Camera camera in cameras)
        {
            Vector3 newPosition = camera.transform.localPosition;
            newPosition.x += offset;
            camera.transform.localPosition = newPosition;
        }
    }

    // Moves the cameras to a specific x position.
    private void MoveCamerasToXPosition(List<Camera> cameras, float xVal)
    {
        foreach (Camera camera in cameras)
        {
            Vector3 newPosition = camera.transform.localPosition;
            newPosition.x = xVal;
            camera.transform.localPosition = newPosition;
        }
    }

    public IEnumerator UpdateScreens()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);
            ActivateDisplays();
        }
    }
}
