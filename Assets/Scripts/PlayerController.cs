using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Rigidbody2D theRB;

    public float moveSpeed = 5f;

    private Vector2 moveInput;
    private Vector2 mouseInput;

    public float mouseSensitivity = 1f;

    public Camera viewCam;
    public GameObject bulletImpact;
    public int currentAmmo;

    private void Awake()
    {
        instance = this;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Movimiento
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 moveHorizontal = transform.up * -moveInput.x;
        Vector3 moveVertical = transform.right * moveInput.y;

        theRB.linearVelocity = (moveHorizontal + moveVertical) * moveSpeed;

        //control de vista
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z - mouseInput.x);


        // Disparo
        if (Input.GetMouseButtonDown(0)) // Detecta un solo clic
        {
            if (currentAmmo > 0)
            {
                Ray ray = viewCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // Debug.Log("Estoy viendo a " + hit.transform.name);
                    Instantiate(bulletImpact, hit.point, transform.rotation);
                }
                else
                {
                    Debug.Log("No estoy viendo nada");
                }
                currentAmmo--;
            }
        }


    }
}
