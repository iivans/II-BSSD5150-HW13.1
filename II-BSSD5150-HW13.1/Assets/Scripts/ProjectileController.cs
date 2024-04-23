using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    Rigidbody2D rb;
    private float lifetime = 10f; // Lifetime of the projectile

    // Start is called before the first frame update 
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Start the destroy timer
        StartCoroutine(DestroyAfterDelay());
    }

    // Update is called once per frame 
    void Update()
    {
        rb.velocity = Vector2.left * 10f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with a square
        if (collision.gameObject.CompareTag("Square"))
        {
            // Cancel the destroy timer
            StopCoroutine(DestroyAfterDelay());

            // Destroy the projectile
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(lifetime);

        // Destroy the projectile after the delay
        Destroy(gameObject);
    }
}