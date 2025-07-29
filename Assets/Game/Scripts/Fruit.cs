using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Fruit : MonoBehaviour
{
    private ObjectPool pool;
    public LayerMask groundLayer;
    public List<Sprite> Fruits;
    public SpriteRenderer SrpiteRenderer;
    public Animator Animator;
    private Ninja player;

    public int staminaGain;
    private bool canDamage = true;
    public void SetPool(ObjectPool poolRef, Ninja plyr)
    {
        pool = poolRef;
        player = plyr;

        int rnd = Random.Range(0, Fruits.Count);
        SrpiteRenderer.sprite = Fruits[rnd];

        canDamage = true;
    }

    public void DestroySelf()
    {
        // Optional: play effect before disabling

        player.AddStamina(staminaGain);
        
        if (pool != null)
            pool.ReturnToPool(gameObject);
        else
            Destroy(gameObject); // Fallback if no pool
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Animator.SetTrigger("Idle");

        if (other.gameObject.CompareTag("Ground") && canDamage)
        {
            player.TakeFruitHit();
            canDamage = false;
        }
    }
}