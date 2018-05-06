using System.Collections;
using System.Timers;
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
    public delegate void MovementBeginDelegate();
    public delegate void MovementEndDelegate();

    //public delegate void MashDelegate();
    public delegate void AttackDelegate();
    #endregion

    #region Public Events
    public event MovementBeginDelegate OnMovementBegin;
    public event MovementEndDelegate OnMovementEnd;

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

    private Timer CameraResetTimer;

    private bool bResetCamera = false;
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

        CamTransform.position = GOTransform.position + ((Quaternion.AngleAxis(CameraAngle, CamTransform.right) * GOTransform.forward) * -CameraDistance);
        CamTransform.LookAt(GOTransform);

        CameraResetTimer = new Timer(1000);
        CameraResetTimer.Elapsed += OnCameraResetTimerElapsed;
    }

    private void OnCameraResetTimerElapsed(object sender, ElapsedEventArgs e)
    {
        bResetCamera = true;
    }

    // Update is called once per frame
    protected void Update()
    {
        StepValues stepValues;
        stepValues.MoveStep              = MoveSpeed * Time.deltaTime;
        stepValues.RotStep               = RotateSpeed * Time.deltaTime;
        stepValues.OrbitStep             = OrbitSpeed * Time.deltaTime;
        stepValues.OrbitReturnStep       = OrbitReturnSpeed * Time.deltaTime;
        
        LeftStickState[0] = LeftStickState[1];
        RightStickState[0] = RightStickState[1];

        LeftStickState[1] = Input.GetAxis(LeftStick_HorizontalAxisName) != 0 || Input.GetAxis(LeftStick_VerticalAxisName) != 0;
        RightStickState[1] = Input.GetAxis(RightStick_HorizontalAxisName) != 0 || Input.GetAxis(RightStick_VerticalAxisName) != 0;
        
        DoControllerInput(stepValues);
        //DoKeyboardInput(stepValues);

        if (bResetCamera)
        {
            TryOrbitReturn(stepValues.OrbitReturnStep);
        }
        else
        {
            CameraTrackPlayer(stepValues);
        }
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
        Vector3 scaledCamForward = Vector3.Scale(CamTransform.forward, new Vector3(1, 0, 1));
        Vector3 desiredForward = scaledCamForward * yAxisVal;
        Vector3 desiredRight = CamTransform.right * xAxisVal;

        Vector3 steppedPosition = GOTransform.position + (desiredForward + desiredRight);
        Vector3 newDir = (steppedPosition - GOTransform.position).normalized;

        GOTransform.rotation = Quaternion.Slerp(GOTransform.rotation, Quaternion.LookRotation(newDir), RotateSpeed * Time.deltaTime);
        GOTransform.position = Vector3.Slerp(GOTransform.position, steppedPosition, step);
    }
    
    private void OrbitCam(float xAxis, float yAxis, float step)
    {
        Vector3 curPos = GOTransform.position;
        Vector3 oldCamPos = ChildCamera.transform.position;

        CamTransform.RotateAround(curPos, Vector3.right, step * yAxis);
        CamTransform.RotateAround(curPos, Vector3.up, step * xAxis);

        Vector3 newCamPos = CamTransform.position;

        if (Physics.Raycast(CamTransform.position, -GOTransform.up, 1.0f))
        {
            oldCamPos.x = newCamPos.x;
            oldCamPos.z = newCamPos.z;
            CamTransform.position = oldCamPos;
        }

        CamTransform.LookAt(curPos);
    }
    
    private void TryOrbitReturn(float step)
    {
        Vector3 cameraReturnPos = gameObject.transform.position + ((Quaternion.AngleAxis(CameraAngle, ChildCamera.transform.right) * gameObject.transform.forward) * -CameraDistance);

        Vector3 dist = cameraReturnPos - ChildCamera.transform.position;
        if (dist.sqrMagnitude >= 100 * Vector3.kEpsilon)
        {
            ChildCamera.transform.position = Vector3.Slerp(ChildCamera.transform.position, cameraReturnPos, step);
            ChildCamera.transform.LookAt(GOTransform.position);
        }
        else
        {
            Debug.Log("Resetting camera reset");
            bResetCamera = false;
            CameraResetTimer.Stop();
        }
    }

    private void CameraTrackPlayer(StepValues stepValues)
    {
        CamTransform.LookAt(GOTransform);

        Vector3 between = GOTransform.position - CamTransform.position;
        Vector3 desiredPos = GOTransform.position + between.normalized * -CameraDistance;
        desiredPos.y = CamTransform.position.y;
        
        if (!IsRightStickActive())
        {
            CamTransform.position = Vector3.Slerp(CamTransform.position, desiredPos, 0.075f);
        }
    }

    private void DoKeyboardInput(StepValues stepValues)
    {
        bool bDidMove = false;
        // Keyboard
        if (Input.GetKey(KeyCode.W))
        {
            bDidMove = true;
            if (OnMovementBegin != null)
            {
                OnMovementBegin.Invoke();
            }

            StepMovement(0.0f, 1.0f, stepValues.MoveStep);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            bDidMove = true;
            if (OnMovementBegin != null)
            {
                OnMovementBegin.Invoke();
            }

            StepMovement(0.0f, -1.0f, stepValues.MoveStep);
        }

        if (Input.GetKey(KeyCode.D))
        {
            bDidMove = true;
            if (OnMovementBegin != null)
            {
                OnMovementBegin.Invoke();
            }

            StepMovement(1.0f, 0.0f, stepValues.MoveStep);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            bDidMove = true;
            if (OnMovementBegin != null)
            {
                OnMovementBegin.Invoke();
            }

            StepMovement(-1.0f, 0.0f, stepValues.MoveStep);
        }
        
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
        {
            bDidMove = true;
            if (OnAttack != null)
            {
                OnAttack.Invoke();
            }
        }

        if (bDidMove)
        {
            if (OnMovementEnd != null)
            {
                OnMovementEnd.Invoke();
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
            if (OnMovementBegin != null)
            {
                OnMovementBegin.Invoke();
            }

            CameraResetTimer.Stop();
            bResetCamera = false;

            StepMovement(xAxisLeft, yAxisLeft, stepValues.MoveStep);
        }
        else if (WasLeftStickActive())
        {
            if (OnMovementEnd != null)
            {
                OnMovementEnd.Invoke();
            }

            CameraResetTimer.Start();
        }
        // orbit
        if (IsRightStickActive())
        {
            if (!WasRightStickActive())
            {
                CameraResetTimer.Stop();
                bResetCamera = false;
            }

            OrbitCam(xAxisRight, yAxisRight, stepValues.OrbitStep * ControllerSensitivity);
        }
        else
        {
            if (WasRightStickActive())
            {
                CameraResetTimer.Start();
            }
        }
    }
}
