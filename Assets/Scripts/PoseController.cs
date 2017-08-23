
using UnityEngine;
using System.Collections;
using Vuforia;

public class PoseController : MonoBehaviour
{
    #region PUBLIC_MEMBERS

    #endregion //PUBLIC_MEMBERS

public GameObject [] FlagSelect;
    #region PRIVATE_MEMBERS
    [SerializeField] private GameObject[] Models;
    [SerializeField] private UDTEventHandler udtEventHandler;
    [SerializeField] private ProximityDetector proximityDetector;
    [SerializeField] private Collider CheckCollider;
    
    [SerializeField] private GameObject msgTapTheCircle;
    [SerializeField] private GameObject msgTryAgain;
    [SerializeField] private GameObject msgGetCloser;
   

    private enum TrackingMode
    {
        CONSTRAINED_TO_CAMERA,
        UDT_BASED,
    }

    // initial mode
    private TrackingMode mTrackingMode = TrackingMode.CONSTRAINED_TO_CAMERA;

    private Vector3 mPosOffsetAtTargetCreation;

    private const float mInitialDistance = 2.5f;

    private bool mBuildingUDT = false;

    private Camera cam;

    public  GameObject SelectModel;
    
    private Animator animPet;
	public string[] anim;
	public int NumberAnimation;
    #endregion //PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
private bool flag;
    void Awake()
    {
      
        msgTapTheCircle.SetActive(true);
        msgTryAgain.SetActive(false);
        msgGetCloser.SetActive(false);

    }

    void Start()
    {
        flag=true;
        mTrackingMode = TrackingMode.CONSTRAINED_TO_CAMERA;

        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);

      
        anim=new string[3];
		anim[0] = "Idle";
		anim[1] = "Rest";
		anim[2] = "Eating";
        ChangedModel("CheetahModel");
    }

    void Update()
    {
        if (CheckTapOnObject()) {
            ChangeMode();
          
        }

        
    }

    void LateUpdate()
    {

        switch (mTrackingMode) {
        
        case TrackingMode.CONSTRAINED_TO_CAMERA:
            {
                // In this phase, the Pet is constrained to remain
                // in the camera view, so it follows the user motion
                Vector3 constrainedPos = cam.transform.position + cam.transform.forward * mInitialDistance;
                this.transform.position = constrainedPos;
                    
                // Update object rotation so that it always look towards the camera
                // and its "up vector" is always aligned with the gravity direction.
                // NOTE: since we are using DeviceTracker, the World up vector is guaranteed 
                // to be aligned (approximately) with the real world gravity direction 
                RotateToLookAtCamera();

                // Check if we were waiting for a UDT creation,
                // and switch mode if UDT was created
                if (mBuildingUDT && udtEventHandler && udtEventHandler.TargetCreated) {

                    ImageTargetBehaviour trackedTarget = GetActiveTarget();

                    if (trackedTarget != null) {
                        mBuildingUDT = false;

                        // Switch mode to UDT based tracking
                        mTrackingMode = TrackingMode.UDT_BASED;

                        // Update header text
                        DisplayMessage(msgGetCloser);
                    

                        // Hide quality indicator
                        udtEventHandler.ShowQualityIndicator(false);

                        // Show the penguin
                        ShowModel(true);

                        // Wake up the proximity detector
                        if (proximityDetector) {
                            proximityDetector.Sleep(false);
                        }

                        // Save a snapshot of the current position offset
                        // between the object and the target center
                        mPosOffsetAtTargetCreation = this.transform.position - trackedTarget.transform.position;
                    }
                }
            }
            break;
        case TrackingMode.UDT_BASED:
            {
                // Update the object world position according to the UDT target position
                ImageTargetBehaviour trackedTarget = GetActiveTarget();
                if (trackedTarget != null) {
                    this.transform.position = trackedTarget.transform.position + mPosOffsetAtTargetCreation;
                }

                // Update object rotation so that it always look towards the camera
                // and its "up vector" is always aligned with the gravity direction.
                // NOTE: since we are using DeviceTracker, the World up vector is guaranteed 
                // to be aligned (approximately) with the real world gravity direction 
                RotateToLookAtCamera();
            }
            break;
        }
    }

    #endregion //MONOBEHAVIOUR_METHODS



    #region PUBLIC_METHODS

    public void ResetState()
    {
         mTrackingMode = TrackingMode.CONSTRAINED_TO_CAMERA;
         mBuildingUDT = false;

      for (int i = 0; i < Models.Length; i++)
       {
           Models[i].SetActive(false);
       }

        // Update message and mode text
        DisplayMessage(msgTapTheCircle);
     

        // Hide the quality indicator
        udtEventHandler.ShowQualityIndicator(true);

        // Show the penguin
         ShowModel(false);

        // Make the proximity detector sleep
        if (proximityDetector) {
            proximityDetector.Sleep(true);
        }
       
    }

    #endregion //PUBLIC_METHODS


    #region PRIVATE_METHODS

    // Callback called when Vuforia has started
    private void OnVuforiaStarted()
    {
        cam = Vuforia.DigitalEyewearARController.Instance.PrimaryCamera ?? Camera.main;

        StartCoroutine(ResetAfter(0.5f));
    }

    private IEnumerator ResetAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ResetState();
        
    }

    private void ChangeMode()
    {
        
        if (mTrackingMode == TrackingMode.CONSTRAINED_TO_CAMERA) {
            SwitchToUDTMode();
        }
    }

    

    private void SwitchToUDTMode()
    {
        if (mTrackingMode == TrackingMode.CONSTRAINED_TO_CAMERA) {
            // check if UDT frame quality is medium or high
            if (udtEventHandler.IsFrameQualityHigh() || udtEventHandler.IsFrameQualityMedium()) {
                // Build a new UDT
                // Note that this may take more than one frame
                CreateUDT();

            } else {
                DisplayMessage(msgTryAgain);
               
            }
        }
    }

    private void CreateUDT()
    {
        float fovRad = cam.fieldOfView * Mathf.Deg2Rad;
        float halfSizeY = mInitialDistance * Mathf.Tan(0.5f * fovRad);
        float targetWidth = 2.0f * halfSizeY; // portrait
        if (Screen.width > Screen.height) { // landscape
            float screenAspect = Screen.width / (float)Screen.height;
            float halfSizeX = screenAspect * halfSizeY;
            targetWidth = 2.0f * halfSizeX;
        }

        mBuildingUDT = true;
        udtEventHandler.BuildNewTarget(targetWidth);
    }

    private void RotateToLookAtCamera()
    {
        Vector3 objPos = this.transform.position;
        Vector3 objGroundPos = new Vector3(objPos.x, 0, objPos.z); // y = 0
        Vector3 camGroundPos = new Vector3(cam.transform.position.x, 0, cam.transform.position.z);
        Vector3 objectToCam = camGroundPos - objGroundPos;
        objectToCam.Normalize();
        this.transform.rotation *= Quaternion.FromToRotation(this.transform.forward, objectToCam);
    }

    private void DisplayMessage(GameObject messageObj)
    {
       
        msgTapTheCircle.SetActive((msgTapTheCircle == messageObj));
        msgTryAgain.SetActive((msgTryAgain == messageObj));
        msgGetCloser.SetActive((msgGetCloser == messageObj));
    }

    

    private bool CheckTapOnObject()
    {
        if (CheckCollider == null)
            return false;

        // Test picking to check if user tapped on penguin
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit)) {
                return (hit.collider == CheckCollider);
            }
        }

        return false;
    }

    private ImageTargetBehaviour GetActiveTarget()
    {
        StateManager stateManager = TrackerManager.Instance.GetStateManager();
        foreach (var tb in stateManager.GetActiveTrackableBehaviours()) {
            if (tb is ImageTargetBehaviour) {
                // found target
                return (ImageTargetBehaviour)tb;
            }
        }
        return null;
    }

    private void ShowModel(bool isVisible)
    {
        // if (SelectModel != null && ModelShadow != null) {
        //     SelectModel.GetComponent<Renderer>().enabled = isVisible;
        //     ModelShadow.GetComponent<Renderer>().enabled = isVisible;
        // }
        if (SelectModel != null) {
            SelectModel.SetActive(isVisible);
            
        }
    }
public void ChangedModel(string name)
{
    for (int i = 0; i < Models.Length; i++)
       {
           Models[i].SetActive(true);
       }
    SelectModel = GameObject.Find(name);
    NumberAnimation = 0;
	animPet =SelectModel.GetComponentInChildren<Animator>();
    switch (name)
    {
        case "CheetahModel":
    for (int i = 0; i < FlagSelect.Length; i++)
    {
        if(i==0)
        {
            FlagSelect[i].SetActive(true);
        }
        else
        {
           FlagSelect[i].SetActive(false); 
        }
    }
        break;
        case "GiraffeModel":
    for (int i = 0; i < FlagSelect.Length; i++)
    {
        if(i==1)
        {
            FlagSelect[i].SetActive(true);
        }
        else
        {
           FlagSelect[i].SetActive(false); 
        }
    }
        break;
        case "TigerModel":
    for (int i = 0; i < FlagSelect.Length; i++)
    {
        if(i==2)
        {
            FlagSelect[i].SetActive(true);
        }
        else
        {
           FlagSelect[i].SetActive(false); 
        }
    }
        break;
       
    }
    ResetState();
}

public void ChangeAnim() {
		if (NumberAnimation != 2) 
		{
			NumberAnimation++;
		}
		else 
		{
			NumberAnimation = 0;
		}

		animPet.SetBool (anim[NumberAnimation], true);

		StartCoroutine("DoSomething");

		Debug.Log("check");
	}
	IEnumerator DoSomething() 
		{
		yield return new WaitForSeconds(0.1f);
		animPet.SetBool(anim[NumberAnimation],false);
		yield return null;
		}
    #endregion //PRIVATE_METHODS

}
