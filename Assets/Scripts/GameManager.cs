using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    UIManager UIManager;
    [SerializeField]
    public bool gameIsPaused;
    [SerializeField]
    int level;
    public int vidas;
    public int coins;


    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        level = 1;
        vidas = 4;
        coins = 100;
        UIManager.ActualizarVidasUI(vidas);
        UIManager.ActualizarCoinsUI(coins);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        UIManager.Resume();        
        gameIsPaused = false;        
    }
    void Pause()
    {
        UIManager.Pause();
        
        gameIsPaused = true;


    }
    public void Retry()
    {
        UIManager.Retry();
        SceneManager.LoadScene(level, LoadSceneMode.Single);
        gameIsPaused = false;        
        //vidas --
    }
    public void ExitGame()
    {
        Time.timeScale = 1;
        //gameOver
        //puntuacion
        //Pantalla titulo
    }
    public bool IsPaused()
    {
        return gameIsPaused;
    }
    public void LoadNextScene()
    {
        level++;
        print("Paco");
        SceneManager.LoadScene(level, LoadSceneMode.Single);
        SceneManager.sceneLoaded += InicializaGameManager;

    }
    public void InicializaGameManager(Scene scene, LoadSceneMode mode)
    {
        UIManager = FindObjectOfType<UIManager>();
        Debug.Log("El nivel ha sido cargado");

    }

    public void Vidas(int vidas)
    {
        this.vidas = vidas;
        UIManager.ActualizarVidasUI(this.vidas);
    }
    public void Coins(int coins)
    {
        this.coins = coins;
        UIManager.ActualizarCoinsUI(this.coins);
    }

}
