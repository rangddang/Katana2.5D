using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float shakePower = 0.1f;
    [SerializeField] private float shakeTime = 0.2f;
	[SerializeField] private float cutPower = 0.3f;
	[SerializeField] private float cutTime = 0.3f;
	[SerializeField] private float rotateSize = 5;
    private Camera camera;
    private float cameraRotate;
    private Vector2 shakeRotate;
    private Vector2 cutRotate;

    private Vector2 cutDir;

    private void Awake()
    {
        camera = Camera.main;
    }

    private void Update()
    {
		transform.localRotation = Quaternion.Euler(shakeRotate.y + cutRotate.y, shakeRotate.x + cutRotate.x, cameraRotate * rotateSize);
	}

    public void ShakeCamera()
    {
        StopCoroutine("ShakeCam");
        StartCoroutine("ShakeCam");
    }

    public void MovingCamera(float x)
    {
        cameraRotate = Mathf.Lerp(cameraRotate, x, Time.deltaTime * 10);
        
    }

    public void CutCamera(Vector2 dir)
    {
        cutDir = dir.normalized;
		StopCoroutine("CutCam");
		StartCoroutine("CutCam");

    }

    private IEnumerator ShakeCam()
    {
        float currentTime = 0;
        float randX;
        float randY;

        while(true)
        {
            currentTime += Time.deltaTime;
            randX = Random.Range(-1, 1 + 1);
			randY = Random.Range(-1, 1 + 1);

			shakeRotate = new Vector2(randX, randY).normalized * shakePower;

            if(currentTime >= shakeTime)
            {
                shakeRotate = Vector2.zero;
                yield break;
            }
			yield return null;
        }
    }

    private IEnumerator CutCam()
    {
		float currentTime = 0;

        Vector2 startPos = Vector2.zero;
        Vector2 endPos = cutDir * cutPower;

		while (true)
        {
            currentTime += Time.deltaTime;
            if (currentTime < cutTime / 2)
            {
                cutRotate = Vector2.Lerp(startPos, endPos, currentTime / (cutTime / 2));
            }
            else
            {
                cutRotate = Vector2.Lerp(endPos, startPos, (currentTime - (cutTime / 2)) / (cutTime / 2));
            }
            if(currentTime > cutTime)
            {
                yield break;
            }
            yield return null;
        }
    }
}
