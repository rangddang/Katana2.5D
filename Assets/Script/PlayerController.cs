using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float sensitivity = 5;
    [SerializeField] private float currentMoveSpeed = 5;

    [SerializeField] private float clampAngle = 70f;

    [SerializeField] private Transform head;
    [SerializeField] private Transform katanaPos;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private KatanaController katana;
    [SerializeField] private UIController ui;
    [SerializeField] protected Animator dashEffect;

    private CameraController camera;
    private CharacterController character;

    public Status status;

    public bool isInvincibility;
    public bool isMove;
    public bool isDash;
    public float dashDelay = 1.5f;

    private float rotateX;
    private float runSin;
    private float runSize;
    private float idleSin;
    private float hor;

    private bool dashCheck;

    private Vector3 dashDir;
    private Quaternion saveRotate;

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
        if (gameManager.isBreakEffect) return;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");


        if (!isDash)
        {
            if (dashCheck)
            {
                isInvincibility = false;
                dashCheck = false;
                camera.ZoomOutCamera();
            }
            dashDir = transform.forward;
            if (isMove)
            {
                dashDir = new Vector3(horizontal, 0, vertical);
                dashDir = ((dashDir.x * transform.right) + (dashDir.z * transform.forward)).normalized;
            }
            Move(new Vector3(horizontal, 0, vertical));
        }

        if (!gameManager.isReverse)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Rotate(mouseX * sensitivity, mouseY * sensitivity);

            saveRotate = transform.rotation;
        }
        else
        {
            LookEnemy();
        }

        if (isDash)
        {
            if (!dashCheck)
            {
                isInvincibility = true;
                dashCheck = true;
                camera.ZoomInCamera(75f);
                dashEffect.SetTrigger("Play");
            }
            Dash();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OffCursor();
        }
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

        if (!isMove)
        {
            katanaPos.localRotation *= Quaternion.Euler(clampY, clampX, 0);
            katanaPos.localRotation = Quaternion.Lerp(katanaPos.localRotation, Quaternion.Euler(0, 0, katanaPos.localRotation.z), Time.deltaTime * 15);
        }
    }

    private void LookEnemy()
    {
        Vector3 playerPos = transform.position;
        Vector3 enemyPos = gameManager.boss.transform.position;

        Quaternion lookRotate = Quaternion.LookRotation(enemyPos - playerPos);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotate, Time.unscaledDeltaTime * 10f);
        rotateX = Mathf.Lerp(rotateX, 0, Time.unscaledDeltaTime * 10);
        head.localRotation = Quaternion.Euler(rotateX, 0, 0);
    }

    private void Move(Vector3 dir)
    {
        hor = Mathf.Lerp(hor, dir.x, Time.deltaTime * 10);

        currentMoveSpeed = katana.katanaOn ? status.speed : status.speed * 2.5f;

        Vector3 moveDir = ((dir.x * transform.right) + (dir.z * transform.forward)).normalized;

		//transform.position += moveDir * currentMoveSpeed * Time.deltaTime;
        character.Move(moveDir * currentMoveSpeed * Time.deltaTime);

        if (dir != Vector3.zero)
        {
            isMove = true;
            idleSin = 0;
            runSin += Time.deltaTime * currentMoveSpeed;
            runSize = Mathf.Lerp(runSize, 1, Time.deltaTime * 10);
            katanaPos.localPosition = (new Vector3(hor, 0, 0) * -0.01f) + new Vector3(0,-0.04f * runSize,0) + new Vector3(Mathf.Sin(runSin) * 0.002f, Mathf.Abs(Mathf.Sin(runSin)) * 0.01f, 0);
            katanaPos.localRotation = Quaternion.Euler(katanaPos.localRotation.x, katanaPos.localRotation.y, hor * -4.5f);
        }
        else
        {
            isMove = false;
            runSin = 0;
            idleSin += Time.deltaTime * 1.3f;
            runSize = Mathf.Lerp(runSize, 0, Time.deltaTime * 20);
            katanaPos.localPosition = Vector3.Lerp(katanaPos.localPosition, new Vector3(0, -Mathf.Sin(idleSin) * 0.007f, 0), Time.deltaTime * 15);
        }
        camera.MovingCamera(-dir.x);
	}

    private void Dash()
    {
        float dashSpeed = currentMoveSpeed == status.speed ? 2.5f : 2f;
        character.Move(dashDir * currentMoveSpeed * Time.deltaTime * dashSpeed);
    }

    public void Hit(float damage)
    {
        if (isInvincibility) return;

        if (katana.parryingType == ParryingType.None)
        {
            status.health -= damage;
            ui.HitEffect(1);
            camera.ShakeCamera(0.4f, 0.5f);
        }
        else if (katana.parryingType == ParryingType.Parrying)
        {
            status.health -= damage * 0.3f;
            ui.ParryingEffect(0.1f);
            ui.HitEffect(0.4f);
			camera.ShakeCamera(0.25f, 0.5f);
        }
        else if (katana.parryingType == ParryingType.PerfectParrying)
        {
            ui.ParryingEffect(0.5f);
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
