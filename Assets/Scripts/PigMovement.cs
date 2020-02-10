using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMovement : MonoBehaviour
{
    Rigidbody2D rb;
    GameObject suelo;
    Transform player;
    float sueloAncho;
    float centroSuelo;
    Vector2 start, end, target,direccion;
    public bool movement, guardia;
    public float speed;
    float salto;
    [SerializeField]
    int vidas;
    int randomStart;
    Animator animator;
    RaycastHit hit;
    private bool gameIsPaused;
    GameManager gameManager;



    void Start()
    {
        speed = 0.75f;
        salto = 6f;
        rb = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
    }
   
    void FixedUpdate()
    {
        gameIsPaused = gameManager.IsPaused();
        if (gameIsPaused == false)
        {
            rb.WakeUp();
            Debug.DrawLine(start, end, Color.blue);
            if (movement)
            {
                gameObject.transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
                animator.SetBool("Movement", true);
                CerdoAlAtaque();
                if (target == end)
                {
                    gameObject.GetComponent<SpriteRenderer>().flipX = true;
                }
                else if (target == start)
                {
                    gameObject.GetComponent<SpriteRenderer>().flipX = false;
                }
                if (transform.position.x == target.x)
                {

                    StartCoroutine(CambioDireccionRoutine());
                }
            }
            else
            {
                animator.SetBool("Movement", false);
            }
        }
        else
        {
            rb.Sleep();
        }
        

        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Suelo" && vidas > 0)
        {
            StopAllCoroutines();
            suelo = collision.gameObject;
            sueloAncho = suelo.transform.lossyScale.x;
            centroSuelo = collision.transform.position.x;
            start = new Vector2(centroSuelo - ((sueloAncho / 2) - 0.4f), transform.position.y);
            end = new Vector2(centroSuelo + ((sueloAncho / 2) - 0.4f), transform.position.y);
            randomStart = Random.Range(0, 2);
            if (randomStart == 0)
            {
                target = start;
            }
            else
            {
                target = end;
            }

            if (transform.rotation.eulerAngles.z < 0 || transform.rotation.eulerAngles.z > 0)
            {
                StartCoroutine(CerdoEnPieRoutine());
            }
            else
            {
                StartCoroutine(CerdoCamina());
                guardia = true;
            }
        }
        if (collision.transform.tag == "Disparo")
        {
            vidas--;
            movement = false;
            StopAllCoroutines();
            if (vidas <=0)
            {                
                StartCoroutine(CerdoMuerto());
            }
            else
            {
                StartCoroutine(CerdoHerido());
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Suelo")
        {
            movement = false;
        }
    }
    
    private IEnumerator CambioDireccionRoutine () 
    {
        movement = false;
        yield return new WaitForSeconds(1.5f);        
        movement = true;
        target = (target == start) ? end : start;


    }
    private IEnumerator CerdoEnPieRoutine()
    {
        
        if (transform.rotation.eulerAngles.z < -1 || transform.rotation.eulerAngles.z > 1)
        {
            
            movement = false;
            Debug.Log("cerdo en pie");
            rb.AddForce(Vector2.up * salto, ForceMode2D.Impulse);
            animator.SetTrigger("CerdoEnPie");
            yield return new WaitForSeconds(0.15f);            
            rb.rotation = -transform.rotation.z;

            StartCoroutine(CerdoCamina());
            guardia = true;
        }
        else
        {
            guardia = true;
            StartCoroutine(CerdoCamina());
        }
    }
    private IEnumerator CerdoHerido()
    {            
        rb.AddForce(Vector2.up * salto, ForceMode2D.Impulse);
        guardia = false;
        yield return new WaitForSeconds(0.15f);

        if (player != null)
        {
            if (player.position.x < gameObject.transform.position.x)
            {
                //rb.MoveRotation(-90);
                rb.rotation = -90;
            }
            else
            {
                //rb.MoveRotation(90);
                rb.rotation = 90;
            }
        }
        
    }
    private IEnumerator CerdoMuerto()
    {
        guardia = false;        
        //rb.AddForce(Vector2.up * salto, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.15f);
        //rb.MoveRotation(-transform.rotation.z);
        rb.rotation = -transform.rotation.z;
        animator.SetTrigger("CerdoMuerto");
    }
    private void CerdoDestroy()
    {
        Destroy(this.gameObject);
    }
    private void CerdoAlAtaque()
    {
        int layerMask = LayerMask.GetMask("Player","Disparo"); ;
        if (transform.position.x > target.x)
        {
            direccion = Vector2.left;
        }
        else if (transform.position.x < target.x)
        {
            direccion = Vector2.right;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direccion, 5f, layerMask);
        Debug.DrawRay(transform.position, Vector2.left, Color.red, 5f);

        if (hit.collider != null)
        {

            if (hit.transform.tag == "Player")
            {
                speed = 3;
                animator.speed = 2;
            }
            else if (hit.transform.tag == "Disparo")
            {
                speed = 3;
                animator.speed = 2;
                rb.AddForce(Vector2.up *  salto, ForceMode2D.Impulse);
                print("SaltoDisparo");
            }

        }
        else
        {
            speed = 0.75f;
            animator.speed = 1;
        }
    }
    private IEnumerator CerdoCamina()
    {
        yield return new WaitForSeconds(0.5f);

        movement = true;
    }
    public void Pause()
    {
        gameIsPaused = true;
    }
    public void Resume()
    {
        gameIsPaused = false;
    }
}
