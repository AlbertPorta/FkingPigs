using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int vidas;
    public int coins;   
    public float speed;
    public float maxSpeed;
    public float maxSpeedDown;
    public float fuerzaSalto;
    public float disparoUltimo;
    public float tiempoDisparo = 0.3f;
    public GameObject disparoPrefab;
    public GameObject startDoor;
    Quaternion direccionDisparo;
    Rigidbody2D RB;
    public bool isGrounded;
    public bool isHanging;
    public bool stopMove;
    public bool AgarradoCuerda;
    float horizontalInput ;
    float verticalInput ;
    SpriteRenderer spriteRenderer;
    Animator animator;    
    private CameraSeek mainCamera;
    private bool gameIsPaused;
    GameManager gameManager;


    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        RB = GetComponent<Rigidbody2D>();
        disparoUltimo = Time.time;
        maxSpeedDown = -10;        
        AgarradoCuerda = false;
        //UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        mainCamera = Camera.main.GetComponent<CameraSeek>();
        //UIManager.ActualizarVidasUI(vidas);
        //UIManager.ActualizarCoinsUI(coins);        
        transform.position = startDoor.transform.position;
        gameManager = FindObjectOfType<GameManager>();
        vidas = gameManager.vidas;
        coins = gameManager.coins;

    }
    
    private void Update()
    {
        gameIsPaused = gameManager.IsPaused();
        if (gameIsPaused == false)
        {
            RB.WakeUp();
            if (stopMove == false)
            {
                horizontalInput = Input.GetAxisRaw("Horizontal");
                verticalInput = Input.GetAxisRaw("Vertical");
            }
            else
            {
                horizontalInput = 0;
                verticalInput = 0;
            }

            Salto();
            Disparo();
            Animations();
        }
        else
        {
            RB.Sleep();
        }
        
        
    }
    private void FixedUpdate()
    {
        gameIsPaused = gameManager.IsPaused();
        if (gameIsPaused == false)
        {
            RB.WakeUp();
            if (!AgarradoCuerda)
            {
                RB.gravityScale = 3f;// conflicto con cuerda
            }
            
            

            Vector2 fixedVelocity = RB.velocity;
            fixedVelocity.x *= 0.75f;

            if (isGrounded || isHanging)
            {
                RB.velocity = fixedVelocity;
            }

            Movimiento();
        }
        else
        {
            RB.Sleep();
            RB.gravityScale = 0f;
        }

        

        
    }
    void Animations()
    {
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            spriteRenderer.flipX = false;
        }


        if (isHanging)
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    if (horizontalInput < 0 && verticalInput == 0)
                    {
                        animator.SetTrigger("DisparoFlipX");
                    }
                    else if (horizontalInput > 0 && verticalInput == 0)
                    {
                        animator.SetTrigger("DisparoFlipX");
                    }
                    else if (verticalInput < 0)
                    {
                        animator.SetTrigger("DisparoDown");

                    }
                    else if (verticalInput > 0)
                    {
                        animator.SetTrigger("DisparoUp");
                    }
                    

                }
                else
                {
                    animator.SetTrigger("ColgadoPared");
                }
            }
            else
            {
                animator.SetTrigger("CaidaLenta");
            }
        }
        else if (isGrounded)
        {
            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                if (Input.GetKey(KeyCode.X) && verticalInput == 0)
                {                                         
                    animator.SetTrigger("Disparo");
                }
                else if (verticalInput < 0)
                {
                    animator.SetTrigger("DisparoDown");
                }
                else if (verticalInput > 0)
                {
                    animator.SetTrigger("DisparoUp");
                }
                else
                {
                    animator.SetTrigger("Idle");
                }

            }
            
            else if (Input.GetAxisRaw("Horizontal") != 0)
            {
                if (Input.GetKeyDown(KeyCode.X) && verticalInput == 0)
                {
                    animator.SetTrigger("Disparo");
                }
                else if (Input.GetKeyDown(KeyCode.X) && verticalInput < 0)
                {
                    animator.SetTrigger("DisparoDown");
                    Debug.Log("DisparoAbajo");
                }
                else if (Input.GetKeyDown(KeyCode.X) && verticalInput > 0)
                {
                    animator.SetTrigger("DisparoUp");
                    Debug.Log("DisparoUp");
                }
                
                else
                {
                    animator.SetTrigger("Run");
                }
            }
        }
        else if (isGrounded == false && isHanging == false)
        {
            if (AgarradoCuerda)
            {
                if (Input.GetKeyDown(KeyCode.X) && verticalInput == 0)
                {
                    animator.SetTrigger("Disparo");
                }
                else if (Input.GetKeyDown(KeyCode.X) && verticalInput < 0)
                {
                    animator.SetTrigger("DisparoDown");
                    Debug.Log("DisparoAbajo");
                }
                else if (Input.GetKeyDown(KeyCode.X) && verticalInput > 0)
                {
                    animator.SetTrigger("DisparoUp");
                    Debug.Log("DisparoUp");
                }
                else
                {
                    animator.SetTrigger("PlayerEscala");
                }

                    
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.X) && verticalInput == 0)
                {
                    animator.SetTrigger("Disparo");
                }
                else if (Input.GetKeyDown(KeyCode.X) && verticalInput < 0)
                {
                    animator.SetTrigger("DisparoDown");
                    Debug.Log("DisparoAbajo");
                }
                else if (Input.GetKeyDown(KeyCode.X) && verticalInput > 0)
                {
                    animator.SetTrigger("DisparoUp");
                    Debug.Log("DisparoUp");
                }
                else
                {
                    if (RB.velocity.y >= 0)
                    {
                        animator.SetTrigger("Salto");
                    }
                    else
                    {
                        if (RB.velocity.y < 0f && RB.velocity.y > -10f)
                        {
                            animator.SetTrigger("CaidaLenta");
                        }
                        else
                        {
                            animator.SetTrigger("CaidaRapida");
                        }
                    }
                }
            }
            

            
        }
    }
    void Movimiento()
    {
        RB.AddForce(Vector2.right * speed * horizontalInput*Time.deltaTime);
        
        if (RB.velocity.x > maxSpeed)
        {
            RB.velocity = new Vector2(maxSpeed, RB.velocity.y);
        }
        else if (RB.velocity.x < -maxSpeed)
        {
            RB.velocity = new Vector2(-maxSpeed, RB.velocity.y);
        }

        if (RB.velocity.y < maxSpeedDown )
        {
            RB.velocity = new Vector2(RB.velocity.x , maxSpeedDown);
        }
        
        if (horizontalInput <0)
        {
            direccionDisparo = Quaternion.identity;
        }
        else if (horizontalInput > 0)
        {
            direccionDisparo = Quaternion.Euler(0, 0, 180);
        }
    }
    void Salto()
    {
        if (Input.GetKeyDown(KeyCode.Z) && isGrounded)
        {
            isGrounded = false;
            RB.velocity = new Vector2(RB.velocity.x, 0);
            RB.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
          
        }
        if (Input.GetKeyUp(KeyCode.Z) && RB.velocity.y > 0)
        {
            RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y * 0.55f);
        }

        if (Input.GetKeyDown(KeyCode.Z) && isHanging)
        {
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                isHanging = false;
                StartCoroutine(StopMoveRoutine());
                RB.velocity = new Vector2(RB.velocity.x, 0);

                RB.AddForce(new Vector2 (1f, 1f) * fuerzaSalto, ForceMode2D.Impulse);
            }
            else if (Input.GetAxisRaw("Horizontal") > 0)
            {
                isHanging = false;
                StartCoroutine(StopMoveRoutine());
                RB.velocity = new Vector2(RB.velocity.x, 0);
                RB.AddForce(new Vector2(-1f, 1f) * fuerzaSalto, ForceMode2D.Impulse);
            }            
        }
    }
    private void Disparo()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (Time.time - disparoUltimo > tiempoDisparo && coins > 0)
            {
                TirarMoneda();
                if (isHanging)
                {                    
                    if (horizontalInput < 0 && verticalInput == 0)
                    {
                        direccionDisparo = Quaternion.Euler(0, 0, 180);
                        Instantiate(disparoPrefab, gameObject.transform.position, direccionDisparo);
                    }
                    else if (horizontalInput > 0 && verticalInput == 0)
                    {
                        direccionDisparo = Quaternion.identity;
                        Instantiate(disparoPrefab, gameObject.transform.position, direccionDisparo);
                    }
                    else if (verticalInput < 0 )
                    {
                        Instantiate(disparoPrefab, gameObject.transform.position, Quaternion.Euler(0, 0, 90));
                    }
                    else if (verticalInput > 0)
                    {
                        Instantiate(disparoPrefab, gameObject.transform.position, Quaternion.Euler(0, 0, -90));
                    }
                    else
                    {
                        Instantiate(disparoPrefab, gameObject.transform.position, direccionDisparo);
                    }

                }
                else
                {
                    if (horizontalInput < 0 && verticalInput == 0 )
                    {
                        direccionDisparo = Quaternion.identity;
                        Instantiate(disparoPrefab, gameObject.transform.position, direccionDisparo);
                    }
                    else if (horizontalInput > 0 && verticalInput == 0 )
                    {
                        direccionDisparo = Quaternion.Euler(0, 0, 180);
                        Instantiate(disparoPrefab, gameObject.transform.position, direccionDisparo);
                    }
                    else if (verticalInput < 0 )
                    {
                        Instantiate(disparoPrefab, gameObject.transform.position, Quaternion.Euler(0, 0, 90));
                    }
                    else if (verticalInput > 0 )
                    {
                        Instantiate(disparoPrefab, gameObject.transform.position, Quaternion.Euler(0, 0, -90));
                    }
                    else
                    {                       
                        Instantiate(disparoPrefab, gameObject.transform.position, direccionDisparo);                
                    }
                }
                
                disparoUltimo = Time.time;

            }


        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pig"))
        {
            if (collision.gameObject.GetComponent<PigMovement>().guardia)
            {
                PerderVida();
                //Instanciar sangre
            }
        }
        if (collision.gameObject.CompareTag("Trampa") || collision.gameObject.CompareTag("Vacio"))
        {
            PerderVida();
            //Instanciar sangre

        }
        if (collision.gameObject.CompareTag("ExitDoor"))
        {
            gameManager.LoadNextScene();
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if ( collision.transform.tag == "Suelo" )
        {
            isGrounded = true;
            isHanging = false;
            
        }
        else if ( collision.transform.tag == "Pared" && isGrounded == false)
        {
            if (horizontalInput != 0)
            {
                if (RB.velocity.y < 0)
                {
                    RB.velocity = new Vector2(RB.velocity.x, -0.75f);
                }

                isHanging = true;        
            }
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Suelo")
        {
            isGrounded = false;
        }
        else if (collision.transform.tag == "Pared")
        {           
                isHanging = false;
        }           
        
    }
    private IEnumerator StopMoveRoutine()
    {
        stopMove = true;
        horizontalInput = 0;
        yield return new WaitForSeconds (0.075f);
        stopMove = false;
    }
    private void VolverChecpoint()
    {
        mainCamera.CheckPoint();
        gameObject.SetActive(false);
        stopMove = true;
        transform.position = startDoor.transform.position;
        RB.velocity = Vector2.zero;           
    }

    public void CogerMoneda(int valorMoneda)
    {
        coins += valorMoneda;
        gameManager.Coins(coins);
    }
    private void TirarMoneda()
    {
        coins--;
        gameManager.Coins(coins);

    }
    public void CogerVida()
    {
        vidas++;
        gameManager.Vidas(vidas);        
        // Sonido Vida
    }
    private void PerderVida()
    {
        
        vidas--;
        gameManager.Vidas(vidas);
        if (vidas <= 0)
        {
            Destroy(this.gameObject);
            //corrutina Game Over
        }
        else
        {
            VolverChecpoint();
        }
        
    }
}
