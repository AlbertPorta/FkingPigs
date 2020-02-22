using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMovement : MonoBehaviour
{
    // Start is called before the first frame update
    #region ATRIBUTOS
    
    PigVida pigVida;    
    Transform player;
    GameManager gameManager;    
    public SpriteRenderer spriteCerdo;  
    Animator animator;
    Rigidbody2D rb;
    RaycastHit2D hit;
    GameObject suelo;


    #region ESTADOSCerdo
    private enum EstadoEnemigo
    {
        Patrulla,
        Persigue,
        Dañado,
        Salta,
        Cae, 
        Null
    }

    [SerializeField]
    private EstadoEnemigo estadoActual;
    #endregion

    
    [SerializeField]
    float salto;
    bool gameIsPaused;
    [SerializeField]
    public bool isGuardia;
    [SerializeField]
    bool isMoviendo;
    [SerializeField]
    bool isTocado;
    [SerializeField]
    bool isLanding;
    [SerializeField]
    bool isPlayerVisto;
    
    float sueloAncho;
    float centroSuelo;
    public Vector2 start, end, target, direccion;
    int randomStart;
    int layerMask;
    float maxvelocity;



    #endregion

    #region METODOS

    #region START
    private void Start()
    {
        pigVida = GetComponent<PigVida>();
        player = GameObject.Find("Player").GetComponent<Transform>();
        gameManager = FindObjectOfType<GameManager>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteCerdo = GetComponent<SpriteRenderer>();
        layerMask = LayerMask.GetMask("Player", "Disparo");
        maxvelocity = 4;
        estadoActual = EstadoEnemigo.Null;
    }
    #endregion

    #region UPDATE

    private void Update()
    {
        gameIsPaused = gameManager.IsPaused();
        if (gameIsPaused == false)
        {
            rb.WakeUp();
            switch (estadoActual)
            {
                case EstadoEnemigo.Patrulla:
                    PatrullaUpdate();
                    break;
                case EstadoEnemigo.Persigue:
                    PersigueUpdate();
                    break;
                case EstadoEnemigo.Dañado:
                    CerdoDañadoUpdate();
                    break;
                case EstadoEnemigo.Cae:
                    CaeUpdate();
                    break;
                case EstadoEnemigo.Salta:

                    break;
            }
        }
        else
        {
            rb.Sleep();
        }
    }
    #endregion
    #region FIXEDUPDATE

    private void FixedUpdate()
    {
        if (gameIsPaused == false)
        {
            rb.WakeUp();
            switch (estadoActual)
            {
                case EstadoEnemigo.Patrulla:
                    PatrullaFixedUpdate();
                    Debug.DrawLine(start, end, Color.blue);
                    break;
                case EstadoEnemigo.Persigue:
                    PersigueFixedUpdate();
                    break;
                case EstadoEnemigo.Dañado:
                    break;
                case EstadoEnemigo.Cae:
                    CaeFixedUpdate();
                    break;
                case EstadoEnemigo.Salta:
                    SaltaFixedUpdate();
                    break;
            }
        }
        else
        {
            rb.Sleep();
        }
    }
    #endregion
    #endregion    

    #region PATRULLA 
    private void PatrullaUpdate()
    {
        isGuardia = true;
        pigVida.SetVelocidad(1);
        animator.speed = 1;
        MirandoTarget();

        if (isMoviendo)
        {
            if (transform.position.x == target.x)
            {
                animator.SetBool("Movement", false);
                StartCoroutine(CambioDireccionRoutine());
            }
            else
            {
                animator.SetBool("Movement", true);
                transform.position = Vector2.MoveTowards(transform.position, target, pigVida.GetVelocidad() * Time.deltaTime);
            }
        }

        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("Player"))
            {
                estadoActual = EstadoEnemigo.Persigue;
            }            
            if (hit.transform.CompareTag("Disparo") && isLanding)
            {
                estadoActual = EstadoEnemigo.Salta;
            }
           
        }
        if (isTocado)
        {
            estadoActual = EstadoEnemigo.Dañado;
        }
    }    
    private void PatrullaFixedUpdate()
    {
        CerdoVigilaRayCastHit();        
    }
    #endregion

    #region PERSIGUE 
    private void PersigueFixedUpdate()
    {
        CerdoVigilaRayCastHit(); 
        
        LimitVelocity();  //LIMITA VELOCIDAD A maxvelocity
        if (transform.position.x > player.position.x + 0.2f || transform.position.x < player.position.x - 0.2f)
        {
            rb.AddForce(direccion * pigVida.GetVelocidad() * 120 * Time.fixedDeltaTime, ForceMode2D.Force);
        }        
    }    
    private void PersigueUpdate()
    {
        isGuardia = true;

        pigVida.SetVelocidad(3);
        animator.speed = 2;
        MirandoTarget(player.position.x);

        if (transform.position.x > player.position.x + 0.2f || transform.position.x < player.position.x - 0.2f)
        {
            StopAllCoroutines();
            animator.SetBool("Movement", true);

        }
        else
        {
            animator.SetBool("Movement", false);
            StartCoroutine(MisionFallidaCorroutine()); // mas de 4 segundos en idle vuelve a Patrulla
        }

        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("Disparo") && isLanding)
            {
                StopAllCoroutines();                
                estadoActual = EstadoEnemigo.Salta;
                                
            }
        }
        if (isTocado)
        {
            StopAllCoroutines();
            estadoActual = EstadoEnemigo.Dañado;
        }
    }
    #endregion

    #region SALTA
    private void SaltaFixedUpdate()
    {
        animator.SetBool("Movement", false);
        animator.SetTrigger("Salta");
        isGuardia = true;
        isMoviendo = false;
        pigVida.SetVelocidad(1);
        animator.speed = 1;
        rb.velocity = new Vector2(rb.velocity.x, salto);
        
    }
        
    #endregion

    #region DAÑADO
    private void CerdoDañadoUpdate()
    {
        isGuardia = false;
        isTocado = false;
        isLanding = false;
        StopAllCoroutines();



        if (player != null)
        {
            pigVida.SetVida(pigVida.GetVida() - 1);


            if (pigVida.GetVida() <= 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, salto);
                animator.SetTrigger("CerdoMuerto");
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, salto);

                if (player.position.x < gameObject.transform.position.x)
                {
                    rb.rotation = -90;
                }
                else
                {
                    rb.rotation = 90;
                }
                if (isLanding)
                {
                    rb.velocity = new Vector2(rb.velocity.x, salto);
                    rb.rotation = -transform.rotation.z;
                    estadoActual = EstadoEnemigo.Persigue;
                }
            }
        }
        else
        {
            isLanding = true;
            isMoviendo = true;
            estadoActual = EstadoEnemigo.Patrulla;
        }
        
    }
    #endregion

    #region CAE
    private void CaeUpdate()
    {

    }
    private void CaeFixedUpdate()
    {

    }
    #endregion

    #region OnCollisionEnter

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Suelo"))
        {
            isLanding = true;            
            if (collision.gameObject != suelo)
            {
                print("collisiono y registro suelo");
                MedirSueloOnCollider(collision);
                RandomStartEnd();
                isMoviendo = true;                
                MirandoTarget();
                estadoActual = EstadoEnemigo.Patrulla;
            }
        }

        if (collision.gameObject.CompareTag("Disparo")  && estadoActual != EstadoEnemigo.Dañado)
        {
            isTocado = true;            
        }
        if (collision.transform.CompareTag("Player") && isGuardia)
        {
            estadoActual = EstadoEnemigo.Patrulla;
            print("Jajajajaja");
        }
    }
    #endregion

    #region OnCollisionExit

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Suelo")
        {
            isLanding = false;
        }

    }
    #endregion
    private void MedirSueloOnCollider(Collision2D collision)
    {
        suelo = collision.gameObject;
        sueloAncho = suelo.transform.lossyScale.x;
        centroSuelo = collision.transform.position.x;
        start = new Vector2(centroSuelo - ((sueloAncho / 2) - 0.4f), transform.position.y);
        end = new Vector2(centroSuelo + ((sueloAncho / 2) - 0.4f), transform.position.y);
    }
    private void RandomStartEnd()
    {
        randomStart = Random.Range(0, 2);
        if (randomStart == 0)
        {
            target = start;
        }
        else
        {
            target = end;
        }
    }
    IEnumerator CambioDireccionRoutine()
    {
        isMoviendo = false;
        yield return new WaitForSeconds(1.5f);
        target = (target == start) ? end : start;
        isMoviendo = true;
    }
    IEnumerator MisionFallidaCorroutine()
    {
        yield return new WaitForSeconds(4f);
        estadoActual = EstadoEnemigo.Patrulla;
    }

    private void MirandoTarget()
    {
        if (target.x > transform.position.x + 0.2f)
        {
            direccion = Vector2.right;
            //spriteCerdo.flipX = true;
        }
        else if (target.x < transform.position.x - 0.2)
        {
            direccion = Vector2.left;
            //spriteCerdo.flipX = false;
        }
        print("estoy mirandotarget");
    }
    private void MirandoTarget(float playerX)
    {

        if (playerX > transform.position.x + 0.2f)
        {
            direccion = Vector2.right;
            spriteCerdo.flipX = true;
        }
        else if (playerX < transform.position.x - 0.2)
        {
            direccion = Vector2.left;
            spriteCerdo.flipX = false;
        }

        print("estoy mirando Player");
    }

    private void CerdoVigilaRayCastHit()
    {
        hit = Physics2D.Raycast(transform.position, direccion, 5f, layerMask);
        Debug.DrawRay(transform.position, Vector2.left, Color.red, 5f);
        Debug.DrawRay(transform.position, Vector2.right, Color.red, 5f);
    }

    private void LimitVelocity()
    {
        if (rb.velocity.x > maxvelocity)
        {
            rb.velocity = new Vector2(maxvelocity, rb.velocity.y);
        }
        else if (rb.velocity.x < -maxvelocity)
        {
            rb.velocity = new Vector2(-maxvelocity, rb.velocity.y);
        }

    }

    public void CerdoDestroy()
    {
        print("Dile a mama que la quiero");
        Destroy(this.gameObject);
    }

    

}
