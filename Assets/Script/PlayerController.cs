using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
	public float hp;
	public float runMoveSpeed;
	public float walkMoveSpeed;
	public float attackDamage;
	public float attackLate;

}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float currentMoveSpeed = 5;
    [SerializeField] private float sensitivity = 10;

    [SerializeField] private float clampAngle = 70f;

    [SerializeField] private Transform head;
    [SerializeField] private Transform katanaPos;
    [SerializeField] private KatanaController katana;
    [SerializeField] private CameraController camera;
    private Status status;

    private float rotateX;
    private float runSin;
    private float runSize;
    private float idleSin;
    private float hor;

    private void Awake()
    {
        status = GetComponent<Status>();
    }


    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Move(new Vector3(horizontal, 0, vertical));

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Rotate(mouseX * sensitivity, mouseY * sensitivity);
	}

    private void Rotate(float mouseX, float mouseY)
    {
        transform.rotation *= Quaternion.Euler(0,mouseX,0);
        
        rotateX -= mouseY;
        rotateX = Mathf.Clamp(rotateX, -clampAngle, clampAngle);
		head.localRotation = Quaternion.Euler(rotateX, 0, 0);

        float clampRotate = 0.5f;

        float clampX = Mathf.Clamp(-mouseX * 0.1f, -clampRotate, clampRotate);
        float clampY = Mathf.Clamp(mouseY * 0.1f, -clampRotate, clampRotate);

		katanaPos.localRotation *= Quaternion.Euler(clampY , clampX, 0);
		katanaPos.localRotation = Quaternion.Lerp(katanaPos.localRotation, Quaternion.Euler(0,0, katanaPos.localRotation.z), Time.deltaTime * 15);
    }

    private void Move(Vector3 dir)
    {
        hor = Mathf.Lerp(hor, dir.x, Time.deltaTime * 10);

        currentMoveSpeed = katana.katanaOn ? status.walkMoveSpeed : status.runMoveSpeed;

        Vector3 moveDir = ((dir.x * transform.right) + (dir.z * transform.forward)).normalized;

		transform.position += moveDir * currentMoveSpeed * Time.deltaTime;

        if (dir != Vector3.zero)
        {
            idleSin = 0;
            runSin += Time.deltaTime * currentMoveSpeed;
            runSize = Mathf.Lerp(runSize, 1, Time.deltaTime * 10);
            katanaPos.localPosition = (new Vector3(hor, 0, 0) * -0.01f) + new Vector3(0,-0.04f * runSize,0) + new Vector3(Mathf.Sin(runSin) * 0.002f, Mathf.Abs(Mathf.Sin(runSin)) * 0.01f, 0);
            katanaPos.localRotation = Quaternion.Euler(katanaPos.localRotation.x, katanaPos.localRotation.y, hor * -4.5f);
        }
        else
        {
            runSin = 0;
            idleSin += Time.deltaTime * 1.3f;
            runSize = Mathf.Lerp(runSize, 0, Time.deltaTime * 20);
            katanaPos.localPosition = Vector3.Lerp(katanaPos.localPosition, new Vector3(0, -Mathf.Sin(idleSin) * 0.007f, 0), Time.deltaTime * 15);
        }
        camera.MovingCamera(-dir.x);
	}
}
