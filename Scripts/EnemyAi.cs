using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Main References")]
    public Transform player;
    private Rigidbody2D rb;

    [Header("Chase & Orbit Settings")]
    public float chaseSpeed = 3f;         // Speed when approaching player
    public float circleSpeed = 2f;        // Speed when orbiting around player
    public float dashSpeed = 8f;         // Speed during dash
    public float chaseDistance = 10f;    // Stop chasing if farther than this
    public float orbitDistance = 2.5f;   // Begin orbiting if closer than this
    public float orbitExitBuffer = 1.2f; // If orbiting and distance > orbitDistance+buffer, exit orbit

    [Header("Attack Settings")]
    public float attackInterval = 2f;
    public float dashDuration = 0.2f;
    public float returnSpeed = 6f;

    [Header("Separation Settings")]
    public float separationRadius = 1.5f;  // How close before enemies repel each other
    public float separationForce = 1f;     // How strongly they push apart
    public float separationWeight = 1f;    // How much to weigh separation in final velocity

    [Header("Player Repulsion (Optional)")]
    public bool repelFromPlayer = false;  // Toggle if you want to push away from player
    public float playerRepelDistance = 1f; // If inside this distance, push away
    public float playerRepelForce = 2f;   // Strength of that push

    [Header("Stuck Check")]
    public float stuckThreshold = 0.1f;   // If speed < this, we consider ourselves “stuck”
    public float stuckTimeLimit = 2f;     // If stuck this many seconds, apply a random nudge
    public float unstickForce = 5f;       // Strength of that nudge

    [Header("Speed Clamping")]
    public float maxSpeed = 10f;  // Prevent final velocity from exceeding this

    // Internal tracking
    private bool isOrbiting = false;
    private float attackTimer = 0f;
    private Coroutine orbitRoutine;
    private float stuckTimer = 0f;


    public float originalChaseSpeed;
    public float originalCircleSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalChaseSpeed = chaseSpeed;       // Speed when approaching player
        originalCircleSpeed = circleSpeed;        // Speed when orbiting around player
    }

    void FixedUpdate()
    {
        if (!player) return;

        Vector2 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;

        // Face the player
        float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;

        // Decide: stop, chase, or start orbit
        if (distance > chaseDistance)
        {
            // Too far – stop
            SetVelocityWithSeparation(Vector2.zero);
            StopOrbiting();
        }
        else if (!isOrbiting && distance > orbitDistance)
        {
            // Chase player
            Vector2 dir = toPlayer.normalized;
            SetVelocityWithSeparation(dir * chaseSpeed);
        }
        else if (!isOrbiting)
        {
            // Begin orbit
            orbitRoutine = StartCoroutine(OrbitAndAttackLoop());
        }
    }

    IEnumerator OrbitAndAttackLoop()
    {
        isOrbiting = true;
        attackTimer = 0f;

        while (true)
        {
            if (!player) yield break;

            Vector2 toPlayer = player.position - transform.position;
            float distance = toPlayer.magnitude;

            // Exit orbit if too far
            if (distance > orbitDistance + orbitExitBuffer)
            {
                StopOrbiting();
                yield break;
            }

            // Face player
            Vector2 dirToPlayer = toPlayer.normalized;
            float angle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;

            // Orbit: perpendicular to player direction
            Vector2 orbitDir = new Vector2(-dirToPlayer.y, dirToPlayer.x);
            SetVelocityWithSeparation(orbitDir * circleSpeed);

            // Handle attack
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                yield return StartCoroutine(DashAttackThenReturnToOrbit());
                attackTimer = 0f;
            }

            yield return null;
        }
    }

    IEnumerator DashAttackThenReturnToOrbit()
    {
        if (!player) yield break;

        // Phase 1: Dash
        Vector2 toPlayer = (player.position - transform.position).normalized;
        SetVelocityWithSeparation(toPlayer * dashSpeed);
        yield return new WaitForSeconds(dashDuration);

        // Phase 2: Return to orbit position
        Vector2 fromPlayer = (transform.position - player.position).normalized;
        Vector2 orbitTarget = (Vector2)player.position + fromPlayer * orbitDistance;

        float timer = 0f;
        float maxTime = 0.5f;
        while (timer < maxTime)
        {
            if (!player) yield break;

            // Move toward orbitTarget
            Vector2 moveDir = (orbitTarget - (Vector2)transform.position).normalized;
            SetVelocityWithSeparation(moveDir * returnSpeed);

            // If close enough, break
            if (Vector2.Distance(transform.position, orbitTarget) < 0.1f)
                break;

            timer += Time.deltaTime;
            yield return null;
        }

        // Resume orbit
        Vector2 toPlayerNow = (player.position - transform.position).normalized;
        Vector2 orbitDirection = new Vector2(-toPlayerNow.y, toPlayerNow.x);
        SetVelocityWithSeparation(orbitDirection * circleSpeed);

        float angle = Mathf.Atan2(toPlayerNow.y, toPlayerNow.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    void StopOrbiting()
    {
        if (orbitRoutine != null)
        {
            StopCoroutine(orbitRoutine);
            orbitRoutine = null;
        }
        isOrbiting = false;
    }

    // ----------------------------------------------------
    //  HELPER: Sets the final rb.velocity, applying:
    //    1) Separation from other enemies
    //    2) Optional push away from the player
    //    3) Speed clamping
    //    4) Unstick nudge if enemy is pinned
    // ----------------------------------------------------
    private void SetVelocityWithSeparation(Vector2 baseVelocity)
    {
        // 1) Compute separation
        Vector2 separation = ComputeSeparationVelocity() * separationWeight;

        // 2) Optional push away from the player if too close
        Vector2 repel = Vector2.zero;
        if (repelFromPlayer && player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist < playerRepelDistance && dist > 0.001f)
            {
                Vector2 away = (transform.position - player.position).normalized;
                // The closer we are, the stronger the push
                float ratio = 1f - (dist / playerRepelDistance);
                repel = away * (playerRepelForce * ratio);
            }
        }

        // Combine
        Vector2 finalVel = baseVelocity + separation + repel;

        // 3) Clamp speed
        if (finalVel.magnitude > maxSpeed)
        {
            finalVel = finalVel.normalized * maxSpeed;
        }

        rb.linearVelocity = finalVel;

        // 4) Check if stuck
        if (rb.linearVelocity.magnitude < stuckThreshold)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stuckTimeLimit)
            {
                // Apply random nudge
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                rb.linearVelocity += randomDir * unstickForce;
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f; // reset if we’re moving
        }
    }

    public void ApplySlow(){
        
        chaseSpeed = 1f;        // Speed when approaching player
        circleSpeed = 1f;        // Speed when orbiting around player
        dashSpeed = 1f;



    }
    public void resetStats(){

        chaseSpeed = originalChaseSpeed;        // Speed when approaching player
        circleSpeed = originalCircleSpeed;        // Speed when orbiting around player
    }

    // ----------------------------------------------------
    //  Computes a repulsion velocity away from all other enemies
    // ----------------------------------------------------
    private Vector2 ComputeSeparationVelocity()
    {
        EnemyAI[] allEnemies = FindObjectsOfType<EnemyAI>();
        Vector2 repulsion = Vector2.zero;
        int count = 0;

        foreach (EnemyAI other in allEnemies)
        {
            if (other == this) continue; // skip self

            Vector2 diff = (transform.position - other.transform.position);
            float dist = diff.magnitude;

            if (dist < separationRadius && dist > 0.001f)
            {
                // The closer the other enemy is, the stronger the push
                Vector2 push = diff.normalized / dist;
                repulsion += push;
                count++;
            }
        }

        if (count > 0)
        {
            repulsion /= count; // average
            repulsion *= separationForce; // scale by force
        }

        return repulsion;
    }
}
