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

    [SerializeField]
    bool isGuardia;
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
                    Debug.DrawLine(start, end, Color.blue);
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

    #region OnCollisionEnter

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Suelo" && suelo != collision.gameObject ){
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

        if (collision.transform.tag == "Disparo"){
            pigVida.SetVida(pigVida.GetVida()-1);            
        }
    }
    #endregion

    #region PATRULLA
    private void PatrullaUpdate()
    {
        pigVida.SetVelocidad(1);
        animator.speed = 1;
        hit = Physics2D.Raycast(transform.position, direccion, 5f, layerMask);
        Debug.DrawRay(transform.position, Vector2.left, Color.red, 5f);
        Debug.DrawRay(transform.position, Vector2.right, Color.red, 5f);

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

        if (transform.position.x > target.x)
        {
            direccion = Vector2.left;
        }
        else if (transform.position.x < target.x)
        {
            direccion = Vector2.right;
        }       

        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("Player"))
            {
                estadoActual = EstadoEnemigo.Persigue;
            }
            else if (hit.transform.CompareTag("Disparo"))
            {
                SaltoUpdate();
            }
        }        
    }
    IEnumerator CambioDireccionRoutine()
    {
        isMoviendo = false;
        yield return new WaitForSeconds(1.5f);
        target = (target == start) ? end : start;

        MirandoTarget();

        isMoviendo = true;
    }
    
    #endregion

    #region PERSIGUE
    private void PersigueUpdate()
    {
        target = new Vector2(player.position.x,transform.position.y);

        pigVida.SetVelocidad(3);
        animator.speed = 2;
        transform.position = Vector2.MoveTowards(transform.position, target, pigVida.GetVelocidad() * Time.deltaTime);
        MirandoTarget();

        if (hit.transform.CompareTag("Disparo"))
        {
            SaltoUpdate();
        }
    }
    #endregion
    
    #region DAÑADO
    private void DañadoUpdate()
    {

    }
    #endregion

    #region Salto
    private void SaltoUpdate()
    {
        rb.AddForce(Vector2.up * salto, ForceMode2D.Impulse);
    }
    #endregion

    #region MirandoTarget
    private void MirandoTarget()
    {
        if (target.x > transform.position.x)
        {
            spriteCerdo.flipX = true;
        }
        else if (target.x < transform.position.x)
        {
            spriteCerdo.flipX = false;
        }
        print("estoy mirando");
    }
    
    #endregion
    #endregion
}
