using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sledPositioner : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private GameObject sled;
    private Rigidbody sledBody;
    [SerializeField]
    private float spawnHeight;
    // Start is called before the first frame update
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        sledBody = sled.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Instantiate(particle, transform.position, transform.rotation);
                sledBody.velocity = Vector3.zero;
                sledBody.transform.position = hit.point + Vector3.up * spawnHeight;
            }

        }
    }
}
