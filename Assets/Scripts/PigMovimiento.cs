using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMovimiento : MonoBehaviour
{
    // Start is called before the first frame update
    #region ATRIBUTOS

    #region PigVida(script)
    PigVida pigVida;
    #endregion

    #region PLAYER
    Transform player;
    #endregion

    #region GameManager(script)
    GameManager gameManager;
    #endregion

    #region SpriteCerdo
    SpriteRenderer spriteCerdo;
    #endregion

    #region Animator

    Animator animator;
    #endregion

    #region RIGIDBODY
    Rigidbody2D rb;
    float maxvelocity;
    #endregion

    #region PAUSED?
    private bool gameIsPaused;
    #endregion

    #region ESTADOSCerdo
    private enum EstadoEnemigo
    {
        Patrulla,
        Persigue,
        Dañado
    }

    [SerializeField]
    private EstadoEnemigo estadoActual;
    #endregion

    #region GUARDIA?

    public bool isGuardia;
    #endregion

    #region MOVIENDO?

    [SerializeField]
    bool isMoviendo;
    #endregion

    #region HIT RAYCAST
    RaycastHit2D hit;
    int layerMask;
    [SerializeField]
    float salto;
    #endregion

    #region PLAYER VISTO?

    bool isPlayerVisto;
    #endregion

    #region DATOSSUELO
    GameObject suelo;
    float sueloAncho;
    float centroSuelo;
    Vector2 start, end, target, direccion;
    int randomStart;
    bool isLanding;
    #endregion

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
                    DañadoUpdate();
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
                    DañadoUpdate();
                    break;
            }
        }
        else
        {
            rb.Sleep();
        }
    }
    #endregion

    #region OnCollisionEnter

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Suelo")
        {
            isLanding = true;

            if (suelo != collision.gameObject)
            {
                suelo = collision.gameObject;
                sueloAncho = suelo.transform.lossyScale.x;
                centroSuelo = collision.transform.position.x;
                start = new Vector2(centroSuelo - ((sueloAncho / 2) - 0.4f), transform.position.y);
                end = new Vector2(centroSuelo + ((sueloAncho / 2) - 0.4f), transform.position.y);
                isMoviendo = true;
                estadoActual = EstadoEnemigo.Patrulla;
                randomStart = Random.Range(0, 2);
                if (randomStart == 0)
                {
                    target = start;
                }
                else
                {
                    target = end;
                }
                MirandoTarget();
            }
        }

        if (collision.transform.tag == "Disparo")
        {
            pigVida.SetVida(pigVida.GetVida() - 1);
        }
        if (collision.transform.tag == "Player")
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

    #region PATRULLA Update
    private void PatrullaUpdate()
    {
        isGuardia = true;
        pigVida.SetVelocidad(1);
        animator.speed = 1;       
        MirandoTarget();

        if (isMoviendo)
        {
            if (transform.position.x == target.x){                
                animator.SetBool("Movement", false);
                StartCoroutine(CambioDireccionRoutine());
            }
            else{                
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
        }        
    }
    IEnumerator CambioDireccionRoutine()
    {
        isMoviendo = false;
        yield return new WaitForSeconds(1.5f);
        target = (target == start) ? end : start;
        isMoviendo = true;
    }

    #endregion

    #region PATRULLA FixedUpdate
    private void PatrullaFixedUpdate()
    {
        hit = Physics2D.Raycast(transform.position, direccion, 5f, layerMask);
        Debug.DrawRay(transform.position, Vector2.left, Color.red, 5f);
        Debug.DrawRay(transform.position, Vector2.right, Color.red, 5f);

        if (hit.collider != null)
        {            
            if (hit.transform.CompareTag("Disparo"))
            {
                SaltoFixedUpdate();
            }
        }
    }
    #endregion

    #region PERSIGUE FixedUpdate
    private void PersigueFixedUpdate()
    {        
        //hit = Physics2D.Raycast(transform.position, direccion, 5f, layerMask);
        //Debug.DrawRay(transform.position, Vector2.left, Color.red, 5f);
        //Debug.DrawRay(transform.position, Vector2.right, Color.red, 5f);
        if (transform.position.x > player.position.x + 0.2f || transform.position.x < player.position.x - 0.2f)
        {
            rb.AddForce(direccion * pigVida.GetVelocidad() * 120 * Time.deltaTime, ForceMode2D.Force);
        }

        //////////////LIMITA VELOCIDAD A maxvelociti
        if (rb.velocity.x > maxvelocity)
        {
            rb.velocity = new Vector2(maxvelocity, rb.velocity.y);
        }
        else if (rb.velocity.x < -maxvelocity)
        {
            rb.velocity = new Vector2(-maxvelocity, rb.velocity.y);
        }
        /////////////

        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("Disparo"))
            {
                SaltoFixedUpdate();
            }
        }                
    }
    #endregion

    #region PERSIGUE Update
    private void PersigueUpdate()
    {
        isGuardia = true;
        pigVida.SetVelocidad(3);        
        animator.speed = 2;        
        MirandoTarget(player.position.x);

        if (transform.position.x > target.x + 0.2f || transform.position.x < target.x - 0.2f)
        {
            animator.SetBool("Movement", true);
        }
        else
        {
            animator.SetBool("Movement", false);
        }
    }
    #endregion


    #region DAÑADO
    private void DañadoUpdate()
    {
        isGuardia = false;
    }
    #endregion

    #region Salto
    private void SaltoFixedUpdate()
    {
        if (isLanding)
        {
            rb.velocity = new Vector2(rb.velocity.x, salto);
        }
    }      
    
    #endregion

    #region MirandoTarget
    private void MirandoTarget( )
    {

        if (target.x > transform.position.x + 0.2f)
        {
            direccion = Vector2.right;
            spriteCerdo.flipX = true;
        }
        else if (target.x < transform.position.x - 0.2)
        {
            direccion = Vector2.left;
            spriteCerdo.flipX = false;
        }
        
        print("estoy mirando");
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

        print("estoy mirando");
    }

    #endregion
    #endregion
}
