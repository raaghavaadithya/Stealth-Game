using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static event System.Action OnReachedFinishPoint;

   public float speed = 9;
   public float smoothMoveTime = 0.1f;
   public float turnSpeed = 8;

   Rigidbody thisRigidBody;
   Vector3 velocity;

   bool disabled  = false;

   void Start() {
       thisRigidBody = GetComponent<Rigidbody>();
       Gaurd.OnGuardHasSpottedPlayer += disablePlayer;
   }

   float smoothInputMagnitude;
   float smoothMoveVelocity;
   float angle;
    void Update()
    {
        Vector3 inputDir = Vector3.zero;
        if(!disabled) inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        float inputMagnitude = inputDir.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);
        float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);
        velocity = transform.forward * smoothInputMagnitude * speed;
    }

    void disablePlayer() {
        disabled = true;
    }

    void FixedUpdate() {
        thisRigidBody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        thisRigidBody.MovePosition(thisRigidBody.position + velocity * Time.deltaTime);
    }

    void OnDestroy() {
        Gaurd.OnGuardHasSpottedPlayer -= disablePlayer;
    }

    void OnTriggerEnter(Collider finishPoint) {
        if(finishPoint.tag == "Finish") {
            disablePlayer();
            if(OnReachedFinishPoint != null) {
                OnReachedFinishPoint();
            }
        }
    }
}
