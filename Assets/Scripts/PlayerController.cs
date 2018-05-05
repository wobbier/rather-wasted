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

    #region Public Exposed Movement Members
    public float MoveSpeed = 1.0f;
    public float RotateSpeed = 1.0f;
    public float OrbitSpeed = 1.0f;
    public float CameraDistance = 1.0f;
    #endregion

    #region Private Structures
    struct StepValues
    {
        public float MoveStep;
        public float RotStep;
        public float OrbitStep;
    }
    #endregion

    #region Private Members
    float lastX;
    float lastY;
    #endregion

    // Use this for initialization
    void Start()
    {
        ChildCamera.transform.parent = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        StepValues stepValues;
        stepValues.MoveStep     = MoveSpeed * Time.deltaTime;
        stepValues.RotStep      = RotateSpeed * Time.deltaTime;
        stepValues.OrbitStep    = OrbitSpeed * Time.deltaTime;

        DoKeyboardInput(stepValues);
        DoControllerInput(stepValues);
    }

    bool test = false;
    private void DoKeyboardInput(StepValues stepValues)
    {
        // Keyboard
        if (Input.GetKey(KeyCode.W))
        {
            gameObject.transform.position += gameObject.transform.forward * stepValues.MoveStep;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            gameObject.transform.position += -gameObject.transform.forward * stepValues.MoveStep;
        }

        if (Input.GetKey(KeyCode.D))
        {
            Vector3 curPos = gameObject.transform.position;
            Vector3 desiredPos = curPos + gameObject.transform.right;
            Vector3 targetPos = desiredPos - curPos;

            // rotation
            Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, targetPos, stepValues.RotStep, 0.25f).normalized;
            gameObject.transform.rotation = Quaternion.LookRotation(newDir);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            // rotation
            Vector3 curPos = gameObject.transform.position;
            Vector3 desiredPos = curPos + -gameObject.transform.right;
            Vector3 targetPos = desiredPos - curPos;

            // rotation
            Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, targetPos, stepValues.RotStep, 0.25f).normalized;
            gameObject.transform.rotation = Quaternion.LookRotation(newDir);
        }
        
        // Mouse
        if (Input.GetMouseButton(1))
        {
            float xAxis = Input.GetAxis("MouseX");
            float yAxis = Input.GetAxis("MouseY");
            
            if (xAxis != 0 || yAxis != 0)
            {
                Vector3 curPos = gameObject.transform.position;

                ChildCamera.transform.RotateAround(curPos, Vector3.right, stepValues.OrbitStep * yAxis * 10);
                ChildCamera.transform.RotateAround(curPos, Vector3.up, stepValues.OrbitStep * xAxis * 10);
                ChildCamera.transform.LookAt(curPos);
            }
        }
        else
        {
            Vector3 cameraReturnPos = gameObject.transform.position + (gameObject.transform.forward * -CameraDistance);
            Vector3 dist = cameraReturnPos - ChildCamera.transform.position;
            if (dist.sqrMagnitude >= Vector3.kEpsilon * Vector3.kEpsilon)
            {
                Vector3 diff = cameraReturnPos - ChildCamera.transform.position;
                Vector3 vec = gameObject.transform.position;


//                 ChildCamera.transform.position = Vector3.Lerp(ChildCamera.transform.position, cameraReturnPos, stepValues.OrbitStep);
//                 ChildCamera.transform.LookAt(gameObject.transform.position);
            }
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
            gameObject.transform.position += gameObject.transform.forward * stepValues.MoveStep * yAxisLeft;
        }

        // rotation
        {
            Vector3 desiredPos = curPos + gameObject.transform.right;
            Vector3 targetPos = desiredPos - curPos;

            Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, targetPos, stepValues.RotStep * xAxisLeft, 0.25f).normalized;
            gameObject.transform.rotation = Quaternion.LookRotation(newDir);
        }

        // orbit
        {
            ChildCamera.transform.RotateAround(curPos, Vector3.right, stepValues.OrbitStep * yAxisRight * 10);
            ChildCamera.transform.RotateAround(curPos, Vector3.up, stepValues.OrbitStep * xAxisRight * 10);
            ChildCamera.transform.LookAt(curPos);
        }
    }
}
