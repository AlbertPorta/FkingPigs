using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraSeek : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform exitDoor;    
    [SerializeField]
    private bool finalMovement;
    public float cameraSpeed;
    public float returnSpeed;
    public bool startMovement;
    public bool puntoGuardado;
    [SerializeField]
    Vector3 startCamara;
    public Vector3 startPosition;
    public float startTime;
    Vector3 velocity;
    public float duration;
    public float smoothTime;
    private bool gameIsPaused;
    GameManager gameManager;



    // Start is called before the first frame update
    void Start()
    {       
        InicializaCamera();
    }

    // Update is called once per frame
    void Update()
    {
        gameIsPaused = gameManager.IsPaused();
        if (gameIsPaused == false)
        {
            if (player != null)
            {
                if (player.position.y > Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.30f, 0)).y && finalMovement == false)
                {
                    startMovement = true;
                }
                if (startMovement)
                {
                    gameObject.transform.Translate(Vector3.up * cameraSpeed * Time.deltaTime);
                    if (player.position.y > Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.75f, 0)).y)
                    {
                        gameObject.transform.Translate(Vector3.up * cameraSpeed * Time.deltaTime);
                    }
                    else if (player.position.y < Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.3f, 0)).y)
                    {
                        gameObject.transform.Translate(Vector3.down * (cameraSpeed / 2) * Time.deltaTime);
                    }
                }

                if (exitDoor.position.y <= Camera.main.ViewportToWorldPoint(new Vector3(0.20f, 0.20f, 0)).y)
                {
                    startMovement = false;
                    finalMovement = true;
                    Debug.Log(Camera.main.ViewportToWorldPoint(new Vector3(0.25f, 0.25f, 0)).y + "mayor");
                }
                if (puntoGuardado)
                {
                    ReturnPuntoGuardado();

                    if (transform.position.y <= startCamara.y /*+ Mathf.Pow(10, -2)*/)
                    {
                        player.gameObject.SetActive(true);
                        player.GetComponent<Player>().stopMove = false;
                        returnSpeed = 0f;
                        puntoGuardado = false;
                    }
                }
            }
        }
        
    }
    public void ReturnPuntoGuardado()
    {                 
        returnSpeed += 0.25f * Time.deltaTime;
        float t = (Time.time - startTime) / duration;
        t = t * t * t * (t * (6f * t - 15f) + 10f);
        //float current = Mathf.SmoothDamp(startPosition.y, startCamara.y, ref velocity.y, smoothTime);
        ////transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(transform.position.y, startCamara.y, ref velocity.y, smoothTime), transform.position.z);
        ////transform.position = new Vector3(0, Mathf.SmoothStep(startPosition.y, startCamara.y, returnSpeed), transform.position.z);
        transform.position = new Vector3(0, Mathf.Lerp(startPosition.y, startCamara.y, t), transform.position.z);
        //transform.position = new Vector3(0, Mathf.SmoothStep(transform.position.y, startCamara.y, t), transform.position.z);
        finalMovement = false;
    }
    public IEnumerator CheckPointCoroutine()
    {
        startMovement = false;
        
        yield return new WaitForSeconds(2f);
        startTime = Time.time;
        startPosition = transform.position;
        puntoGuardado = true;
    }
    public void CheckPoint()
    {
        StartCoroutine(CheckPointCoroutine());
    }
    public void InicializaCamera()
    {
        startMovement = false;
        finalMovement = false;
        startCamara = new Vector3(0, 0, -10);
        transform.position = startCamara;
        velocity = Vector3.zero;
        gameManager = FindObjectOfType<GameManager>();
    }
}
