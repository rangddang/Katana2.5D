using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float shakePower;
    private float shakeTime;
	private float cutPower = 0.3f;
	[SerializeField] private float cutTime = 0.3f;
	[SerializeField] private float rotateSize = 5;
    [SerializeField] private float dashZoom = 80f;

    private Camera camera;
    private Camera subCam;

    private Vector2 shakeRotate;
    private Vector2 cutRotate;
    private Vector2 cutDir;

    private float cameraRotate;
    private float cameraZoom;
    private float offsetZoom;



    private void Awake()
    {
        camera = Camera.main;
        subCam = transform.GetChild(0).GetComponent<Camera>();
    }

    private void Start()
    {
        offsetZoom = camera.fieldOfView;
        cameraZoom = offsetZoom;
    }

    private void Update()
    {
		transform.localRotation = Quaternion.Euler(shakeRotate.y + cutRotate.y, shakeRotate.x + cutRotate.x, cameraRotate * rotateSize);
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, cameraZoom, Time.deltaTime * 20);
        subCam.fieldOfView = camera.fieldOfView;
	}

    public void ZoomInCamera(float zoomSize)
    {
        cameraZoom = zoomSize;
    }

    public void ZoomOutCamera()
    {
        cameraZoom = offsetZoom;
    }

    public void ShakeCamera(float time, float power)
    {
        shakeTime = time;
        shakePower = power;
        StopCoroutine("ShakeCam");
        StartCoroutine("ShakeCam");
    }

    public void MovingCamera(float x)
    {
        cameraRotate = Mathf.Lerp(cameraRotate, x, Time.deltaTime * 10);
        
    }

    public void CutCamera(Vector2 dir, float power)
    {
        cutDir = dir.normalized;
        cutPower = power;

        //StopCoroutine("Zoom");
        //StartCoroutine("Zoom");

        StopCoroutine("CutCam");
		StartCoroutine("CutCam");

    }

    private IEnumerator ShakeCam()
    {
        float time = shakeTime;
        float power = shakePower;
        float currentTime = 0;
        float randX;
        float randY;

        while(true)
        {
            currentTime += Time.deltaTime;
            randX = Random.Range(-1, 1 + 1);
			randY = Random.Range(-1, 1 + 1);

			shakeRotate = new Vector2(randX, randY).normalized * shakePower;

            if(currentTime >= time)
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

    private IEnumerator Zoom()
    {
        ZoomInCamera(65f);
        yield return new WaitForSeconds(0.1f);
        ZoomOutCamera();
    }
}