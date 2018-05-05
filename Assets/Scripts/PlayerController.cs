using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Constants
    #endregion

    #region Public Members
    public Camera ChildCamera;
    #endregion

    #region Private Delegate Declarations
    public delegate void MovementStepDelegate(float val);
    public delegate void MovementRotateDelegate(float val);
    #endregion

    #region Public Events
    public event MovementStepDelegate OnMovementStep;
    public event MovementRotateDelegate OnMovementRotate;
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
    #endregion

    // Use this for initialization
    void Start()
    {
        GOTransform = gameObject.transform;
        ChildCamera.transform.parent = GOTransform;
    }

    // Update is called once per frame
    void Update()
    {
        StepValues stepValues;
        stepValues.MoveStep              = MoveSpeed * Time.deltaTime;
        stepValues.RotStep               = RotateSpeed * Time.deltaTime;
        stepValues.OrbitStep             = OrbitSpeed * Time.deltaTime;
        stepValues.OrbitReturnStep       = OrbitReturnSpeed * Time.deltaTime;

        DoKeyboardInput(stepValues);
        //DoControllerInput(stepValues);
    }
    
    private Vector3 StepMovement(Vector3 dir, float step)
    {
        if (OnMovementStep != null)
        {
            OnMovementStep.Invoke(step);
        }

        return dir * step;
    }

    private Vector3 StepRotation(Vector3 dir, float step)
    {
        if (OnMovementRotate != null)
        {
            OnMovementRotate.Invoke(step);
        }

        Vector3 curPos = gameObject.transform.position;
        Vector3 desiredPos = curPos + dir;
        Vector3 targetPos = desiredPos - curPos;

        Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, targetPos, step, 0.25f).normalized;
        return newDir;
    }

    private void OrbitCam(float xAxis, float yAxis, float step)
    {
        if (Physics.Raycast(ChildCamera.transform.position, -GOTransform.up, 1.0f))
        {
            return;
        }

        Vector3 curPos = GOTransform.position;
        Vector3 oldCamPos = ChildCamera.transform.position;

        ChildCamera.transform.RotateAround(curPos, Vector3.right, step * yAxis);
        ChildCamera.transform.RotateAround(curPos, Vector3.up, step * xAxis);

        Vector3 newCamPos = ChildCamera.transform.position;
        Vector3 origCamPos = GOTransform.transform.forward * -CameraDistance;
        Vector3 diff = origCamPos - newCamPos;

        if (Vector3.Angle(newCamPos, origCamPos) >= 90.0f)
        {
            ChildCamera.transform.position = oldCamPos;
        }

        ChildCamera.transform.LookAt(curPos);
    }

    private void TryOrbitReturn(float step)
    {
        Vector3 cameraReturnPos = gameObject.transform.position + (gameObject.transform.forward * -CameraDistance);
        cameraReturnPos = Quaternion.AngleAxis(CameraAngle, new Vector3(1, 0, 0)) * cameraReturnPos;

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
            Vector3 steppedMovement = StepMovement(GOTransform.forward, stepValues.MoveStep);
            GOTransform.position += steppedMovement;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Vector3 steppedMovement = StepMovement(-GOTransform.forward, stepValues.MoveStep);
            GOTransform.position += steppedMovement;
        }

        if (Input.GetKey(KeyCode.D))
        {
            Vector3 steppedRotation = StepRotation(GOTransform.right, stepValues.RotStep);
            gameObject.transform.rotation = Quaternion.LookRotation(steppedRotation);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Vector3 steppedRotation = StepRotation(-GOTransform.right, stepValues.RotStep);
            gameObject.transform.rotation = Quaternion.LookRotation(steppedRotation);
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
        float yAxisLeft = Input.GetAxis("VerticalLeft");
        float xAxisLeft = Input.GetAxis("HorizontalLeft");
        // Right stick
        float yAxisRight = Input.GetAxis("VerticalRight");
        float xAxisRight = Input.GetAxis("HorizontalRight");

        Vector3 curPos = gameObject.transform.position;

        // translation
        {
            Vector3 steppedPosition = StepMovement(GOTransform.forward, stepValues.MoveStep * yAxisLeft);
            GOTransform.position += steppedPosition;
        }

        // rotation
        {
            Vector3 steppedRotation = StepRotation(gameObject.transform.forward, stepValues.RotStep * xAxisLeft);
            gameObject.transform.rotation = Quaternion.LookRotation(steppedRotation);
        }

        // orbit
        if (xAxisRight != 0.0f || yAxisRight != 0.0f)
        {
            OrbitCam(xAxisRight, yAxisRight, stepValues.OrbitStep * ControllerSensitivity);
        }
        else
        {
            TryOrbitReturn(stepValues.OrbitReturnStep);
        }
    }
}
