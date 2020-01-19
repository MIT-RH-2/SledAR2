using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sledController : MonoBehaviour
{
    [SerializeField]
    private bool walking = true;
    private Vector3 movementDir;
    [SerializeField]
    private float movementForce;
    [SerializeField]
    private Rigidbody body;
    private float rotation;
    [SerializeField]
    private float maxRotationAngle;
    [SerializeField]
    private float steeringSpeed = 0.1f;
    [SerializeField]
    private float rightingSpeed = 0.1f;
    // Start is called before the first frame update
    private Vector3 xzVel;
    [SerializeField]
    private float xzRightingThreshold = 1f;
    [SerializeField]
    private float rightingThreshold = 1f;
    [SerializeField]
    private float maxRightingDistance = .1f;
    [SerializeField]
    private float maxVel;

    private Collider collider;

    private bool grounded = false;
    void Start()
    {
        body = gameObject.GetComponent<Rigidbody>();
        collider = gameObject.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        movementDir = Input.GetAxis("Vertical") * body.transform.forward;
        //rotation += Input.GetAxis
        Debug.DrawRay(gameObject.transform.position, body.transform.forward.normalized * 5);
    }

    void FixedUpdate()
    {
        xzVel = Vector3.ClampMagnitude(body.velocity.x * Vector3.right + body.velocity.z * Vector3.forward, maxVel);
        body.velocity = xzVel + body.velocity.y * Vector3.up;
        RaycastHit groundChecker;
        LayerMask ground = LayerMask.GetMask("Ground");
        bool hit = Physics.Raycast(gameObject.transform.position, Vector3.down, out groundChecker, 1.0f, ground);
        if (hit == true)
        {
            Vector3 targetUp = body.transform.up;
            Debug.DrawRay(gameObject.transform.position, groundChecker.point);
            if(!hit && !grounded)
            {
                targetUp = body.transform.up;
            }
            //body.AddForce(movementDir * movementForce, ForceMode.Acceleration);
            Vector3 targetPos = body.position + body.transform.forward;
            if ((body.velocity.normalized - body.transform.forward).magnitude > steeringSpeed && xzVel.magnitude > xzRightingThreshold)
            {
                targetPos = Vector3.Slerp(body.transform.forward, body.velocity, steeringSpeed);
                //targetPos = Vector3.slerp()
            }
            if ((body.transform.up - groundChecker.normal).magnitude > rightingSpeed && Mathf.Abs(Vector3.Angle(body.transform.up, groundChecker.normal)) < 90)
            {
                //targetUp = body.transform.up + (groundChecker.normal - body.transform.up).normalized * rightingSpeed;
                targetUp = Vector3.Slerp(body.transform.up, groundChecker.normal, rightingSpeed);
            }
            //body.transform.LookAt(targetPos, targetUp);
            //body.transform.RotateAround()
            body.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(body.transform.forward, body.velocity, steeringSpeed), targetUp);
            Debug.DrawRay(body.position + targetUp, targetUp);
            Debug.DrawRay(body.position + body.transform.up, body.transform.up, Color.blue);
            if(hit)
            {
                Debug.DrawRay(body.position + groundChecker.normal, groundChecker.normal, Color.red);
            }
            
            if ((body.transform.forward - body.velocity.normalized).magnitude > rightingSpeed) {

            }

            Debug.Log(hit);

            //Debug.DrawRay(body.transform.position, Vector3.Slerp(body.transform.up, groundChecker.normal, rightingSpeed)*5);
            float angle = Vector3.Angle(body.transform.up, Vector3.Slerp(body.transform.up, groundChecker.normal, steeringSpeed));
            //body.AddForce(Vector3.up * (Mathf.Cos(angle) * body.velocity.magnitude) + Vector3.forward * (Mathf.Sin(angle) * body.velocity.magnitude), ForceMode.Acceleration);


            //apply righting and steering forces
            //oof
            /*
            float angleXY = Vector3.Angle((body.transform.up - body.transform.up.z * Vector3.forward).normalized, (groundChecker.normal - groundChecker.normal.z * Vector3.forward).normalized);
            float angleYZ = Vector3.Angle((body.transform.up - body.transform.up.x * Vector3.right).normalized, (groundChecker.normal - groundChecker.normal.x * Vector3.right).normalized);
            float angleXZ = Vector3.Angle((body.transform.up - body.transform.up.y * Vector3.up).normalized, (groundChecker.normal - groundChecker.normal.y * Vector3.up).normalized);

            //body.AddForceAtPosition(Vector3.up * (Mathf.Sin(angleYZ) * body.velocity.magnitude) + Vector3.forward * (Mathf.Sin(angleYZ) * body.velocity.magnitude), body.velocity, ForceMode.Acceleration);
            //body.AddForceAtPosition(Vector3.right * (Mathf.Sin(angleXY) * body.velocity.magnitude) + Vector3.up * (Mathf.Cos(angleXY) * body.velocity.magnitude), body.velocity, ForceMode.Acceleration);
            //body.AddForceAtPosition(Vector3.forward * (Mathf.Sin(angleXZ) * body.velocity.magnitude) + Vector3.forward * (Mathf.Sin(angleYZ) * body.velocity.magnitude), body.velocity, ForceMode.Acceleration);
            /*
            body.AddForceAtPosition(Vector3.Slerp(body.transform.up, groundChecker.normal, rightingSpeed) - body.transform.up, body.transform.position + body.transform.up, ForceMode.Acceleration);
            Debug.DrawRay(body.transform.position + body.transform.up, (Vector3.Slerp(body.transform.up, groundChecker.normal, rightingSpeed) - body.transform.up) * 5, Color.green);
            body.AddForceAtPosition(Vector3.Slerp(body.transform.forward, body.velocity.normalized, steeringSpeed * Mathf.Abs(xzVel.magnitude / maxVel)) - body.transform.forward, body.transform.position + body.transform.forward, ForceMode.Acceleration);
            */


        }
        /*
        if(xzVel.magnitude > xzRightingThreshold)
        {
            body.AddForce(movementDir * movementForce, ForceMode.Acceleration);
            body.transform.LookAt(body.transform.position + Vector3.Slerp(body.transform.forward, body.velocity.normalized, steeringSpeed), groundChecker.normal);
        }
        */

    }

    void OnCollisionEnter(Collision collision)
    {
        grounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }
}
