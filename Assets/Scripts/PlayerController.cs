using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Constants
    #endregion

    #region Public Members
    public Camera MainCamera;
    #endregion

    #region Public Exposed Movement Members
    public float MoveSpeed = 1.0f;
    public float RotateSpeed = 1.0f;
    #endregion

    // Use this for initialization
    void Start()
    {
        MainCamera.transform.parent = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float moveStep = MoveSpeed * Time.deltaTime;
        float rotStep = RotateSpeed * Time.deltaTime;

        DoInput(moveStep, rotStep);
    }

    private void DoInput(float moveStep, float rotStep)
    {
        if (Input.GetKey(KeyCode.W))
        {
            gameObject.transform.position += gameObject.transform.forward * MoveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            gameObject.transform.position += -gameObject.transform.forward * MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            Vector3 curPos = gameObject.transform.position;
            Vector3 desiredPos = curPos + gameObject.transform.right;
            Vector3 targetPos = desiredPos - curPos;

            // rotation
            Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, targetPos, RotateSpeed * Time.deltaTime, 0.25f).normalized;
            gameObject.transform.rotation = Quaternion.LookRotation(newDir);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            // rotation
            Vector3 curPos = gameObject.transform.position;
            Vector3 desiredPos = curPos + -gameObject.transform.right;
            Vector3 targetPos = desiredPos - curPos;

            // rotation
            Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, targetPos, RotateSpeed * Time.deltaTime, 0.25f).normalized;
            gameObject.transform.rotation = Quaternion.LookRotation(newDir);
        }
    }
}
