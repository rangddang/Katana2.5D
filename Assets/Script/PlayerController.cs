using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float currentMoveSpeed = 5;
    [SerializeField] private float sensitivity = 10;

    [SerializeField] private float clampAngle = 70f;

    [SerializeField] private Transform head;
    [SerializeField] private Transform katanaPos;
    [SerializeField] private KatanaController katana;

    private float rotateX;
    private float runSin;



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
		katanaPos.localRotation = Quaternion.Lerp(katanaPos.localRotation, Quaternion.identity, Time.deltaTime * 15);
    }

    private void Move(Vector3 dir)
    {
        //if(dir == Vector3.zero) return;

        Vector3 moveDir = ((dir.x * transform.right) + (dir.z * transform.forward)).normalized;

		transform.position += moveDir * currentMoveSpeed * Time.deltaTime;

        if (dir != Vector3.zero)
        {
            runSin += Time.deltaTime * currentMoveSpeed;
            katanaPos.localPosition += new Vector3(dir.x, 2, 0).normalized * -0.0025f;
            katanaPos.localPosition += new Vector3(Mathf.Sin(runSin) * 0.0003f, Mathf.Abs(Mathf.Sin(runSin)) * 0.001f, 0);
		}
        else
        {
            runSin = 0;
        }
        katanaPos.localPosition = Vector3.Lerp(katanaPos.localPosition, Vector3.zero, Time.deltaTime * 15);
		katanaPos.localRotation *= Quaternion.Euler(0, 0, -dir.x * 0.25f);
	}
}
