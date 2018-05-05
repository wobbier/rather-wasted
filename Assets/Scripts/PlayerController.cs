using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Constants
    private string LeftStick_HorizontalAxisName = "HorizontalLeft";
    private string LeftStick_VerticalAxisName = "VerticalLeft";
    private string RightStick_HorizontalAxisName = "HorizontalRight";
    private string RightStick_VerticalAxisName = "VerticalRight";
    private string LeftStick_PoliticalStance = "Radical";
    private string RightStick_PoliticalStance = "Radical";
    #endregion

    #region Public Members
    public Camera ChildCamera;
    #endregion

    #region Private Delegate Declarations
    public delegate void MovementStepDelegate(float val);
    public delegate void MovementRotateDelegate(float val);

    //public delegate void MashDelegate();
    public delegate void AttackDelegate();
    #endregion

    #region Public Events
    public event MovementStepDelegate OnMovementStep;
    public event MovementRotateDelegate OnMovementRotate;

    //public event MashDelegate OnMash;
    public event AttackDelegate OnAttack;
    #endregion

    #region Public Movement Members
    [Space(10)]
    public float MoveSpeed = 1.0f;
    public float RotateSpeed = 1.0f;
    public float OrbitSpeed = 1.0f;
    public float OrbitReturnSpeed = 1.0f;

    [Space(10)]
    public float CameraDistance = 1.0f;
    public float CameraAngle = 1.0f;

    [Space(10)]
    public float ControllerSensitivity = 1.0f;
    #endregion

    #region Private Structures
    struct StepValues
    {
        public float MoveStep;
        public float RotStep;
        public float OrbitStep;
        public float OrbitReturnStep;
    }
    #endregion

    #region Private Members
    private Transform GOTransform;
    private Transform CamTransform;
    #endregion

    #region Controller State
    BitArray LeftStickState = new BitArray(2, false);
    BitArray RightStickState = new BitArray(2, false);
    #endregion

    // Use this for initialization
    void Start()
    {
        GOTransform = gameObject.transform;
        CamTransform = ChildCamera.transform;
    }

    // Update is called once per frame
    protected void Update()
    {
        StepValues stepValues;
        stepValues.MoveStep              = MoveSpeed * Time.deltaTime;
        stepValues.RotStep               = RotateSpeed * Time.deltaTime;
        stepValues.OrbitStep             = OrbitSpeed * Time.deltaTime;
        stepValues.OrbitReturnStep       = OrbitReturnSpeed * Time.deltaTime;

        //DoKeyboardInput(stepValues);

        LeftStickState[0] = LeftStickState[1];
        RightStickState[0] = RightStickState[1];

        LeftStickState[1] = Input.GetAxis(LeftStick_HorizontalAxisName) != 0 || Input.GetAxis(LeftStick_VerticalAxisName) != 0;
        RightStickState[1] = Input.GetAxis(RightStick_HorizontalAxisName) != 0 || Input.GetAxis(RightStick_VerticalAxisName) != 0;

        Debug.Log(LeftStickState[0] + "     " + LeftStickState[1]);

        DoControllerInput(stepValues);
        //DoKeyboardInput(stepValues);
    }

    private bool IsRightStickActive()
    {
        return RightStickState[1];
    }

    private bool WasRightStickActive()
    {
        return RightStickState[0];
    }

    private bool IsLeftStickActive()
    {
        return LeftStickState[1];
    }

    private bool WasLeftStickActive()
    {
        return LeftStickState[0];
    }

    private void StepMovement(float xAxisVal, float yAxisVal, float step)
    {
        if (OnMovementStep != null)
        {
            OnMovementStep.Invoke(step);
        }
        
        Vector3 desiredForward = GOTransform.forward * yAxisVal;
        Vector3 desiredRight = GOTransform.right * xAxisVal;

        Vector3 steppedPosition = GOTransform.position + (desiredForward + desiredRight);
        Vector3 newDir = (steppedPosition - GOTransform.position).normalized;

        GOTransform.rotation = Quaternion.Slerp(GOTransform.rotation, Quaternion.LookRotation(newDir), RotateSpeed * Time.deltaTime);
        GOTransform.position = Vector3.Slerp(GOTransform.position, steppedPosition, step);
    }
    
    private void OrbitCam(float xAxis, float yAxis, float step)
    {
        Vector3 curPos = GOTransform.position;
        Vector3 oldCamPos = ChildCamera.transform.position;

        ChildCamera.transform.RotateAround(curPos, Vector3.right, step * yAxis);
        ChildCamera.transform.RotateAround(curPos, Vector3.up, step * xAxis);

        Vector3 newCamPos = ChildCamera.transform.position;
        Vector3 origCamPos = GOTransform.transform.forward * -CameraDistance;

        if (Physics.Raycast(ChildCamera.transform.position, -GOTransform.up, 1.0f))
        {
            oldCamPos.x = newCamPos.x;
            oldCamPos.z = newCamPos.z;
            ChildCamera.transform.position = oldCamPos;
        }
        else
        {
            if (Vector3.Angle(origCamPos, newCamPos) >= 90.0f)
            {
                // #TODO(Josh) Don't just stop input for x and y when limited by only one
//                 if (Vector3.Dot(newCamPos.normalized, GOTransform.right) > 0)
//                 {
//                 }

                ChildCamera.transform.position = oldCamPos;
            }
        }

        ChildCamera.transform.LookAt(curPos);
    }
    
    private void TryOrbitReturn(float step)
    {
        Vector3 cameraReturnPos = gameObject.transform.position + ((Quaternion.AngleAxis(CameraAngle, ChildCamera.transform.right) * gameObject.transform.forward) * -CameraDistance);

        Vector3 dist = cameraReturnPos - ChildCamera.transform.position;
        if (dist.sqrMagnitude >= Vector3.kEpsilon * Vector3.kEpsilon)
        {
            ChildCamera.transform.position = Vector3.Slerp(ChildCamera.transform.position, cameraReturnPos, step);
            ChildCamera.transform.LookAt(GOTransform.position);
        }
    }

    private void DoKeyboardInput(StepValues stepValues)
    {
        // Keyboard
        if (Input.GetKey(KeyCode.W))
        {
            StepMovement(0.0f, 1.0f, stepValues.MoveStep);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            StepMovement(0.0f, -1.0f, stepValues.MoveStep);
        }

        if (Input.GetKey(KeyCode.D))
        {
            StepMovement(1.0f, 0.0f, stepValues.MoveStep);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            StepMovement(-1.0f, 0.0f, stepValues.MoveStep);
        }
        
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            if (OnAttack != null)
            {
                OnAttack.Invoke();
            }
        }

        // Mouse
        if (Input.GetMouseButton(1))
        {
            float xAxis = Input.GetAxis("MouseX");
            float yAxis = Input.GetAxis("MouseY");
            
            if (xAxis != 0 || yAxis != 0)
            {
                OrbitCam(xAxis, yAxis, stepValues.OrbitStep);
            }
        }
        else
        {
            TryOrbitReturn(stepValues.OrbitReturnStep);
        }
    }

    private void DoControllerInput(StepValues stepValues)
    {
        // Left stick
        float yAxisLeft = Input.GetAxis(LeftStick_VerticalAxisName);
        float xAxisLeft = Input.GetAxis(LeftStick_HorizontalAxisName);
        // Right stick
        float yAxisRight = Input.GetAxis(RightStick_VerticalAxisName);
        float xAxisRight = Input.GetAxis(RightStick_HorizontalAxisName);

        Vector3 curPos = gameObject.transform.position;

        /*
         * Movement
         * 
         */
        if (IsLeftStickActive())
        {
            StepMovement(xAxisLeft, yAxisLeft, stepValues.MoveStep);
            

//             Vector3 steppedRotation = StepRotation(gameObject.transform.right, stepValues.RotStep * xAxisLeft);
//             gameObject.transform.rotation = Quaternion.LookRotation(steppedRotation);
        }
        
        // orbit
        if (IsRightStickActive())
        {
            OrbitCam(xAxisRight, yAxisRight, stepValues.OrbitStep * ControllerSensitivity);
        }
        else
        {
            TryOrbitReturn(stepValues.OrbitReturnStep);
        }
    }
}
