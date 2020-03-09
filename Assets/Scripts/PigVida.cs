using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigVida : MonoBehaviour
{
    public int vida;
    public float velocidad;

    [Range(0, 20)] public int default_vida;
    [Range(0f, 500)] public float default_velocidad;
    // Start is called before the first frame update

    void Start()
    {
        InicializarCerdo();
    }

    // Update is called once per frame
    public void InicializarCerdo()
    {
        if (vida != default_vida) vida = default_vida;
        if (velocidad != default_velocidad) velocidad = default_velocidad;
    }

    /// <summary>
    /// Establece la vida
    /// </summary>
    /// <param name="val">cantidad, por defecto es 5</param>
    public void SetVida(int val = 5) {
        vida = Mathf.Clamp(val, 0, 20);
    }

    public void SetVelocidad(float val = 3f) {
        velocidad = Mathf.Clamp(val, 0f, 5f);
    }

    public int GetVida()
    {
        return vida;
    }    

    public float GetVelocidad()
    {
        return velocidad;
    }


    public bool isAlive()
    {
        return vida > 0;
    }



}
