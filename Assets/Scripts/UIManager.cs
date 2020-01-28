using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text vidasUI;
    [SerializeField]
    private Text coinsUI;    
    [SerializeField]
    private GameObject menuPause;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

    }
    public void Resume() 
    {
        menuPause.SetActive(false);
        
    }
    public void Pause()
    {
        menuPause.SetActive(true);        
    }
    public void Retry()
    {
        menuPause.SetActive(false);
    }
    public void ExitGame()
    {
        menuPause.SetActive(false);
        // Pantalla titulo
    }

    public void ActualizarVidasUI(int cantidadVidas)
    {
        vidasUI.text = "Lives: x" + cantidadVidas;
    }
    public void ActualizarCoinsUI(int cantidadCoins)
    {
        coinsUI.text = "Coins: " + cantidadCoins;
    }

}
