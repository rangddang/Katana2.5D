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
    [SerializeField] private UIController ui;
    private CameraController camera;
    private CharacterController character;

    public Status status;

    private float rotateX;
    private float runSin;
    private float runSize;
    private float idleSin;
    private float hor;

    private void Awake()
    {
        camera = Camera.main.GetComponent<CameraController>();
        character = GetComponent<CharacterController>();
        status = GetComponent<Status>();
    }

    private void Start()
    {
        OffCursor();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Move(new Vector3(horizontal, 0, vertical));

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Rotate(mouseX * sensitivity, mouseY * sensitivity);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnCursor();
        }
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

        currentMoveSpeed = katana.katanaOn ? status.walkSpeed : status.runSpeed;

        Vector3 moveDir = ((dir.x * transform.right) + (dir.z * transform.forward)).normalized;

		//transform.position += moveDir * currentMoveSpeed * Time.deltaTime;
        character.Move(moveDir * currentMoveSpeed * Time.deltaTime);

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

    public void Hit(float damage)
    {
        if (katana.parryingType == ParryingType.None)
        {
            status.health -= damage;
            ui.HitEffect();
            camera.ShakeCamera(0.3f, 0.8f);
        }
        else if (katana.parryingType == ParryingType.Parrying)
        {
            status.health -= damage * 0.3f;
			ui.HitEffect();
			camera.ShakeCamera(0.3f, 0.5f);
        }
        else if (katana.parryingType == ParryingType.PerfectParrying)
        {
			camera.ShakeCamera(0.2f, 0.3f);
			katana.parryingSuccess = false;
			katana.parryingSuccess = true;
		}
    }

    private void OnCursor()
    {
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

    private void OffCursor()
    {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void SlowTime()
	{
		StartCoroutine("SlowTimeScale");
	}

	private IEnumerator SlowTimeScale()
	{
        float saveSensitivity = sensitivity;

		yield return new WaitForSecondsRealtime(0.06f);
        sensitivity = saveSensitivity * 0.2f;
		Time.timeScale = 0.2f;
		yield return new WaitForSecondsRealtime(0.1f);
		sensitivity = saveSensitivity;
		Time.timeScale = 1f;
	}
}
