using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Rigidbody2D theRB;

    public int health = 10;
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Vector2 mouseInput;
    public float mouseSensitivity = 1f;

    public Camera viewCam;
    public Animator weaponAnimator; // Referencia al Animator para las animaciones de disparo, recarga, idle y caminar
    public int currentAmmo = 2; // Munición inicial
    private bool isReloading = false; // Verifica si está recargando
    private bool isShooting = false; // Verifica si está disparando

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // Movimiento
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 moveHorizontal = transform.up * -moveInput.x;
        Vector3 moveVertical = transform.right * moveInput.y;
        theRB.linearVelocity = (moveHorizontal + moveVertical) * moveSpeed;

        // Control de vista
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - mouseInput.x);

        // Manejo de animaciones basado en input de movimiento
        if (!isShooting && !isReloading) // Solo activar Idle o Walk si no está disparando o recargando
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) // Detecta movimiento del input directamente
            {
                weaponAnimator.SetBool("IsWalking", true);
                weaponAnimator.SetBool("IsIdle", false);
            }
            else // Sin input, el jugador está quieto
            {
                weaponAnimator.SetBool("IsWalking", false);
                weaponAnimator.SetBool("IsIdle", true);
            }
        }

        // Disparo
        if (Input.GetMouseButtonDown(0) && currentAmmo > 0 && !isReloading && !isShooting)
        {
            StartCoroutine(Shoot()); // Maneja el disparo con animación
        }
    }

    private IEnumerator Shoot()
    {
        isShooting = true; // Bloquea nuevos disparos
        weaponAnimator.SetTrigger("Shoot"); // Activa la animación de disparo

        // Detiene las animaciones de Idle y Walk durante el disparo
        weaponAnimator.SetBool("IsWalking", false);
        weaponAnimator.SetBool("IsIdle", false);

        // Disparo inmediato
        Ray ray = viewCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Impacto en: " + hit.transform.name);

            if (hit.transform.tag == "Enemy")
            {
                hit.transform.parent.GetComponent<EnemyController>().TakeDamage();
            }
        }

        currentAmmo--; // Reduce la munición

        // Espera una duración fija que coincida con el tiempo necesario para permitir otro disparo
        float shootAnimationTime = weaponAnimator.GetCurrentAnimatorStateInfo(0).length; // Duración completa de la animación
        yield return new WaitForSeconds(shootAnimationTime * 0.5f); // Permitir disparar antes de que finalice completamente el tiempo muerto

        isShooting = false; // Permite disparar nuevamente, aunque la animación puede seguir

        // Reactiva las animaciones de movimiento según el estado actual
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            weaponAnimator.SetBool("IsWalking", true);
            weaponAnimator.SetBool("IsIdle", false);
        }
        else
        {
            weaponAnimator.SetBool("IsWalking", false);
            weaponAnimator.SetBool("IsIdle", true);
        }

        // Si no hay munición, recargar automáticamente
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true; // Bloquea disparos durante la recarga
        weaponAnimator.SetTrigger("Reload"); // Activa la animación de recarga

        // Detiene las animaciones de movimiento durante la recarga
        weaponAnimator.SetBool("IsWalking", false);
        weaponAnimator.SetBool("IsIdle", false);

        // Espera una duración fija basada en el tiempo de la animación
        float reloadAnimationTime = weaponAnimator.GetCurrentAnimatorStateInfo(0).length; // Duración completa de la animación
        yield return new WaitForSeconds(reloadAnimationTime * 0.5f); // Permitir disparar antes de que finalice completamente

        currentAmmo = 2; // Restaura la munición
        isReloading = false; // Permite disparar nuevamente

        // Reactiva las animaciones de movimiento según el estado actual
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            weaponAnimator.SetBool("IsWalking", true);
            weaponAnimator.SetBool("IsIdle", false);
        }
        else
        {
            weaponAnimator.SetBool("IsWalking", false);
            weaponAnimator.SetBool("IsIdle", true);
        }
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Debug.Log("El jugador esta muerto");
        }
    }
}