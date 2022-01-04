using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaurd : MonoBehaviour {

    public static event System.Action OnGuardHasSpottedPlayer;

    public Transform pathHolder;
    public LayerMask viewMask;
    public Light spotlight;

    public float guardSpeed = 5;
    public float waitTime = 0.3f;
    public float turnSpeed = 90;
    public float viewDistance;
    public float timeToSpotPlayer = 0.5f;
    
    Color originalSpotlightColor;
    Transform player;

    float viewAngle;
    float playerVisibleTimer;

    void Start() {
        originalSpotlightColor = spotlight.color;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        viewAngle = spotlight.spotAngle;
        Vector3[] pathPoints = new Vector3[pathHolder.childCount];

        for(int i = 0; i < pathPoints.Length; i++) {
            pathPoints[i] = pathHolder.GetChild(i).position;
            pathPoints[i] = new Vector3(pathPoints[i].x, transform.position.y, pathPoints[i].z); 
        }

        StartCoroutine(moveAlongPath(pathPoints));
    }

    void Update() {
        if(CanSeePlayer()) {
            playerVisibleTimer += Time.deltaTime;
        } else {
            playerVisibleTimer -= Time.deltaTime;
        }

        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotlight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer/timeToSpotPlayer);

        if(playerVisibleTimer >= timeToSpotPlayer) {
            if(OnGuardHasSpottedPlayer != null) OnGuardHasSpottedPlayer();
        }
    }

    bool CanSeePlayer() {
        if(Vector3.Distance(transform.position, player.position) < viewDistance) {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if(angleBetweenGuardAndPlayer < viewAngle / 2f) {
                if(!Physics.Linecast(transform.position, player.position, viewMask)) {
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator turnToFace(Vector3 target) {
        Vector3 dirToLookTarget = (target - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;
        while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed*Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    IEnumerator moveAlongPath(Vector3[] wayPoints) {
        transform.position = wayPoints[0];
        int targetWayPointIndex = 1;
        Vector3 targetWayPoint = wayPoints[targetWayPointIndex];
        transform.LookAt(targetWayPoint);

        while(true) {
            transform.position = Vector3.MoveTowards(transform.position,targetWayPoint, Time.deltaTime * guardSpeed);
            if(transform.position == targetWayPoint) {
                targetWayPointIndex = (targetWayPointIndex + 1) % wayPoints.Length;
                targetWayPoint = wayPoints[targetWayPointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(turnToFace(targetWayPoint));
            }
            yield return null;
        }
    }
    
    void OnDrawGizmos() {
        Vector3 startPos = pathHolder.GetChild(0).position, prevPos = startPos;

        foreach(Transform waypoint in pathHolder) {
            Gizmos.DrawLine(prevPos, waypoint.position);
            prevPos = waypoint.position;
            Gizmos.DrawSphere(waypoint.position, 0.3f);
        }

        Gizmos.DrawLine(prevPos, startPos);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}