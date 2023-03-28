using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float sensitivity;
    [SerializeField] private float limiteAngle;
    [SerializeField] private float jumpPower;
    [SerializeField] private Transform camera;
	[SerializeField] private Animator anim;

    private bool isWarking;
	private float rotateX;
    private float rotateY;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
        //RotateHorizontal();
        //RotateVertical();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if(rigidbody.velocity.y < 0)
        {
            JumpDown();
        }
    }

    private void Move()
    {
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");

		Vector3 dir = new Vector3(horizontal, 0, vertical);

		transform.Translate(dir * Time.deltaTime * speed);
		isWarking = dir != Vector3.zero ? true : false;
		anim.SetBool("IsWalking", isWarking);
	}

    private void RotateHorizontal()
    {
        float mouseX = Input.GetAxis("Mouse X");

        rotateY += mouseX * Time.deltaTime * sensitivity;


		transform.localRotation = Quaternion.Euler(0, rotateY, 0);
    }

    private void RotateVertical()
    {
		float mouseY = Input.GetAxis("Mouse Y");

        rotateX -= mouseY * Time.deltaTime * sensitivity;

        rotateX = Mathf.Clamp(rotateX, -limiteAngle, limiteAngle);

		camera.localRotation = Quaternion.Euler(rotateX, 0, 0);
	}

    private void Jump()
    {
        rigidbody.velocity = Vector3.up * jumpPower;
		anim.SetBool("IsJumping", true);
	}

    private void JumpDown()
    {
        anim.SetBool("IsDown", true);
    }

    private void OnCollisionEnter(Collision collision)
    {
		anim.SetBool("IsJumping", false);
		anim.SetBool("IsDown", false);
	}
}
