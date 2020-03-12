using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cofre : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Sprite spriteCofreAbierto;
    [SerializeField]
    GameObject bonusPrefab;
    bool isAbierto;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAbierto)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                spriteRenderer.sprite = spriteCofreAbierto;
                Instantiate(bonusPrefab, new Vector2(transform.position.x, transform.position.y + 1.2f), Quaternion.identity);
                isAbierto = true;
            }
        }
        
    }
}
