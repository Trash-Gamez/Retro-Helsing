using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private SpriteRenderer theSR;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();
        theSR.flipX = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(PlayerController.instance.transform.position, -Vector3.forward);
    }
}
