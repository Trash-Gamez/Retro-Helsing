using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int health = 2;
    public Animator enemyAnimator; // Referencia al Animator del enemigo

    [Header("Rango y Disparo")]
    public float detectionRange = 5f; // Rango de detecci�n del jugador
    public GameObject bulletPrefab; // Prefab del proyectil
    public Transform firePoint; // Punto desde donde se disparan los proyectiles
    public float timeBetweenShots = 2f; // Tiempo personalizable entre disparos

    private GameObject player; // Referencia al jugador
    private bool canShoot = true; // Controla si el enemigo puede disparar

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Busca al jugador por etiqueta
    }

    void Update()
    {
        // Comprueba si el jugador este en rango
        if (player != null && Vector2.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            // Apunta hacia el jugador
            Vector2 direction = (player.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Ataca si puede disparar
            if (canShoot)
            {
                StartCoroutine(Attack());
            }
        }
    }

    private IEnumerator Attack()
    {
        canShoot = false; // Bloquea nuevos disparos mientras se ejecuta el ataque

        // Reproduce la animaci�n de ataque
        enemyAnimator.SetTrigger("Attack");
        Debug.Log("El enemigo esta disparando");

        // Espera a que se sincronice con la animaci�n de ataque antes de disparar
        yield return new WaitForSeconds(0.5f); // Ajusta seg�n la duraci�n de la animaci�n de ataque

        // Dispara el proyectil en direcci�n al jugador
        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;

            // Instancia el proyectil en el firePoint con el eje Z en -0.5
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.transform.position = new Vector3(bullet.transform.position.x, bullet.transform.position.y, -0.5f);

            // Aplica movimiento al proyectil (puedes personalizarlo seg�n la l�gica de tu proyectil)
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * 5; // Ajusta la velocidad del proyectil
            }
        }

        // Espera antes de permitir el siguiente disparo
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    public void TakeDamage()
    {
        health--;

        if (health <= 0)
        {
            StartCoroutine(HandleDeath()); // Maneja la muerte con animaci�n
        }
    }

    private IEnumerator HandleDeath()
    {
        // Activa la animaci�n de muerte
        enemyAnimator.SetTrigger("Die");

        // Obtiene la duraci�n de la animaci�n
        float deathAnimationTime = enemyAnimator.GetCurrentAnimatorStateInfo(0).length;

        // Espera a que termine la animaci�n
        yield return new WaitForSeconds(deathAnimationTime);

        // Destruye el objeto
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // Cambia el color del Gizmo
        Gizmos.color = Color.red;

        // Dibuja un círculo que represente el rango de detección
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}