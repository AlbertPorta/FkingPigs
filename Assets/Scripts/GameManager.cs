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
    int level;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);        
        level = 1;
        
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
        //reset level
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
        SceneManager.LoadScene(level, LoadSceneMode.Single);
        SceneManager.sceneLoaded += InicializaGameManager;
        
    }
    public void InicializaGameManager(Scene scene, LoadSceneMode mode)
    {
        UIManager = FindObjectOfType<UIManager>();
        //Debug.Log("El nivel ha sido cargado");

    }
    


}
