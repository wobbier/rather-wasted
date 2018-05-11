using System.Collections;
using System.Timers;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Constants
    public const string LeftStick_HorizontalAxisName = "LeftX";
    public const string LeftStick_VerticalAxisName = "LeftY";
    public const string RightStick_HorizontalAxisName = "RightX";
    public const string RightStick_VerticalAxisName = "RightY";
    public const string MainAbilityButtonName = "MainAbility";
    public const string SecondaryAbilityButtonName = "SecondaryAbility";
    public const string LeftStick_PoliticalStance = "Radical";
    public const string RightStick_PoliticalStance = "Radical";


    public const string PlayerOneName = "P1";
    public const string PlayerTwoName = "P2";
    #endregion

    #region Public Members
    public Camera ChildCamera;

    public string PlayerName;
    public bool ApplyMovement = true;
    #endregion

    #region Private Delegate Declarations
    public delegate void MovementBeginDelegate();
    public delegate void MovementEndDelegate();
    public delegate void RotationBeginDelegate(float val);

    public delegate void MainAbilityButtonDelegate();
    public delegate void SecondaryAbilityButtonDelegate();

    public delegate void AttackSuccessDelegate();
    #endregion

    #region Public Events
    public event MovementBeginDelegate OnMovementBegin;
    public event MovementEndDelegate OnMovementEnd;
    public event RotationBeginDelegate OnRotationBegin;

    public event MainAbilityButtonDelegate OnMainAbility;
    public event SecondaryAbilityButtonDelegate OnSecondaryAbility;

    public event AttackSuccessDelegate OnAttackSuccess;
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
    private bool bIsMoving = false;
    #endregion

    #region Controller State
    BitArray LeftStickState = new BitArray(2, false);
    BitArray RightStickState = new BitArray(2, false);
    BitArray MainAbilityButtonState = new BitArray(2, false);
    BitArray SecondaryAbilityButtonState = new BitArray(2, false);
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
        stepValues.MoveStep = MoveSpeed * Time.deltaTime;
        stepValues.RotStep = RotateSpeed * Time.deltaTime;
        stepValues.OrbitStep = OrbitSpeed * Time.deltaTime;
        stepValues.OrbitReturnStep = OrbitReturnSpeed * Time.deltaTime;

        LeftStickState[0] = LeftStickState[1];
        RightStickState[0] = RightStickState[1];

        MainAbilityButtonState[0] = MainAbilityButtonState[1];
        SecondaryAbilityButtonState[0] = SecondaryAbilityButtonState[1];
        
        LeftStickState[1] = Input.GetAxis(PlayerName + "_" + LeftStick_HorizontalAxisName) != 0 || Input.GetAxis(PlayerName + "_" + LeftStick_VerticalAxisName) != 0;
        RightStickState[1] = Input.GetAxis(PlayerName + "_" + RightStick_HorizontalAxisName) != 0 || Input.GetAxis(PlayerName + "_" + RightStick_VerticalAxisName) != 0;
        
        MainAbilityButtonState[1] = Input.GetButton(PlayerName + "_" + MainAbilityButtonName);
        SecondaryAbilityButtonState[1] = Input.GetButton(PlayerName + "_" + SecondaryAbilityButtonName);

        HandleInputShunting(stepValues);

        if (bResetCamera)
        {
            TryOrbitReturn(stepValues.OrbitReturnStep);
        }
        else if (!IsRightStickActive())
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

    private bool IsMainAbilityButtonActive()
    {
        return MainAbilityButtonState[1];
    }

    private bool WasMainAbilityButtonActive()
    {
        return MainAbilityButtonState[0];
    }

    private bool IsSecondaryAbilityButtonActive()
    {
        return MainAbilityButtonState[1];
    }

    private bool WasSecondaryAbilityButtonActive()
    {
        return MainAbilityButtonState[0];
    }

    // Function to determine when to use keyboard or controller
    private void HandleInputShunting(StepValues stepValues)
    {
        int numControllers = Input.GetJoystickNames().Length;
        if (PlayerName == PlayerOneName)
        {
            if (numControllers == 2)
            {
                if (Input.anyKey)
                {
                    DoKeyboardInput(stepValues);
                }
                else
                {
                    DoControllerInput(stepValues);
                }
            }
            else
            {
                DoKeyboardInput(stepValues);
            }
        }
        else
        {
            DoControllerInput(stepValues);
        }
    }

    private void BeginMovement()
    {
        bIsMoving = true;
        if (OnMovementBegin != null)
        {
            OnMovementBegin.Invoke();
        }
    }

    private void FinishMovement()
    {
        bIsMoving = false;
        if (OnMovementEnd != null)
        {
            OnMovementEnd.Invoke();
        }
    }

    private void StepMovement(float xAxisVal, float yAxisVal, float step)
    {
        if (ApplyMovement)
        {
            Vector3 scaledCamForward = Vector3.Scale(CamTransform.forward, new Vector3(1, 0, 1));
            Vector3 desiredForward = scaledCamForward * yAxisVal;
            Vector3 desiredRight = CamTransform.right * xAxisVal;

            Vector3 steppedPosition = GOTransform.position + (desiredForward + desiredRight).normalized;
            Vector3 newDir = (steppedPosition - GOTransform.position).normalized;

            Quaternion slerpedRot = Quaternion.Slerp(GOTransform.rotation, Quaternion.LookRotation(newDir), RotateSpeed * Time.deltaTime);
            if (slerpedRot != GOTransform.rotation)
            {
                if (OnRotationBegin != null)
                {
                    OnRotationBegin.Invoke(xAxisVal);
                }
            }

            GOTransform.rotation = Quaternion.Slerp(GOTransform.rotation, Quaternion.LookRotation(newDir), RotateSpeed * Time.deltaTime);
            GOTransform.position = Vector3.Slerp(GOTransform.position, steppedPosition, step);
        }
    }
    
    private void OrbitCam(float xAxis, float yAxis, float step)
    {
        Vector3 curPos = GOTransform.position;
        Vector3 oldCamPos = ChildCamera.transform.position;
        
        CamTransform.RotateAround(curPos, CamTransform.right, step * -yAxis);
        CamTransform.RotateAround(curPos, CamTransform.up, step * xAxis);

        Vector3 between = GOTransform.position - CamTransform.position;
        Vector3 desiredPos = GOTransform.position + between.normalized * -CameraDistance;

        float cameraYMin = (GOTransform.position + GOTransform.up * 2.0f).y;
        float cameraYMax = (GOTransform.position + GOTransform.up * 12.0f).y;
        if (desiredPos.y < cameraYMin)
        {
            desiredPos.y = cameraYMin;
        }
        if (desiredPos.y > cameraYMax)
        {
            desiredPos.y = cameraYMax;
        }
        
        CamTransform.position = desiredPos;
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
        Vector3 between = CamTransform.position - GOTransform.position;
        Vector3 desiredPos = GOTransform.position + between.normalized * CameraDistance;

        desiredPos.y = CamTransform.position.y;

        CamTransform.position = Vector3.Slerp(CamTransform.position, desiredPos, 0.075f);
        CamTransform.LookAt(GOTransform);
    }

    private void DoKeyboardInput(StepValues stepValues)
    {
        bool bDidMoveThisFrame = false;
        // Keyboard
        if (Input.GetKey(KeyCode.W))
        {
            bDidMoveThisFrame = true;
            StepMovement(0.0f, 1.0f, stepValues.MoveStep);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            bDidMoveThisFrame = true;
            StepMovement(0.0f, -1.0f, stepValues.MoveStep);
        }

        if (Input.GetKey(KeyCode.D))
        {
            bDidMoveThisFrame = true;
            StepMovement(1.0f, 0.0f, stepValues.MoveStep);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            bDidMoveThisFrame = true;
            StepMovement(-1.0f, 0.0f, stepValues.MoveStep);
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (OnMainAbility != null)
            {
                OnMainAbility.Invoke();
            }
        }

        if (bDidMoveThisFrame && !bIsMoving)
        {
            BeginMovement();
        }
        else if (!bDidMoveThisFrame && bIsMoving)
        {
            FinishMovement();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (OnSecondaryAbility != null)
            {
                OnSecondaryAbility.Invoke();
            }
        }

        // Mouse
        if (Input.GetMouseButton(1))
        {
            float xAxis = Input.GetAxis("MouseX");
            float yAxis = Input.GetAxis("MouseY");
            
            if (xAxis != 0 || yAxis != 0)
            {
                OrbitCam(xAxis, -yAxis, stepValues.OrbitStep);
            }
        }
    }

    private void DoControllerInput(StepValues stepValues)
    {
        // Left stick
        float yAxisLeft = Input.GetAxis(PlayerName + "_" + LeftStick_VerticalAxisName);
        float xAxisLeft = Input.GetAxis(PlayerName + "_" + LeftStick_HorizontalAxisName);
        // Right stick
        float yAxisRight = Input.GetAxis(PlayerName + "_" + RightStick_VerticalAxisName);
        float xAxisRight = Input.GetAxis(PlayerName + "_" + RightStick_HorizontalAxisName);
        
        Vector3 curPos = gameObject.transform.position;

        /*
         * Movement
         * 
         */
        if (IsLeftStickActive())
        {
            if (OnMovementBegin != null && ApplyMovement)
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

            if (!IsRightStickActive())
            {
                CameraResetTimer.Start();
            }
        }
        // orbit
        if (IsRightStickActive())
        {
            if (!WasRightStickActive())
            {
                CameraResetTimer.Stop();
                bResetCamera = false;
            }

            //if (!IsLeftStickActive())
            {
                OrbitCam(xAxisRight, yAxisRight, stepValues.OrbitStep * ControllerSensitivity);
            }
        }
        else if (WasRightStickActive() && !IsLeftStickActive())
        {
            CameraResetTimer.Start();
        }
        
        if (IsMainAbilityButtonActive() && !WasMainAbilityButtonActive())
        {
            if (OnMainAbility != null)
            {
                OnMainAbility.Invoke();
            }
        }

        if (IsSecondaryAbilityButtonActive() && !WasSecondaryAbilityButtonActive())
        {
            if (OnSecondaryAbility != null)
            {
                OnSecondaryAbility.Invoke();
            }
        }
    }

    protected void AttackSuccess()
    {
        if (OnAttackSuccess != null)
        {
            OnAttackSuccess.Invoke();
        }
    }
}
