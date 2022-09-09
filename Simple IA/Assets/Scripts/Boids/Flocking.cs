using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    [SerializeField] private List<Flocking> boids = new List<Flocking>();

    private Vector3 baseRotation = Vector3.zero;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxForce;
    [SerializeField] private float checkRadius;

    [SerializeField] private float separationMultiplayer;
    [SerializeField] private float cohesionMultiplayer;
    [SerializeField] private float aligmentMultiplayer;

    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 acceleration;

    private Vector3 Position
    {
        get { return gameObject.transform.position; }
        set { gameObject.transform.position = value; }
    }

    private void Start ()
    {
        //baseRotation.x = Random.Range(0, 180);
        //baseRotation.y = Random.Range(0, 180);
        //baseRotation.z = Random.Range(0, 180);

        float angle = Random.Range(0, 2 * Mathf.PI);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
        velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    private void Update ()
    {
        boids.Clear();
        Collider[] others = Physics.OverlapSphere(Position, checkRadius);
        if (others != null)
        {
            foreach (Collider other in others)
            {
                Flocking boid = other.GetComponent<Flocking>();
                if (boid)
                    boids.Add(boid);
            }

            boids.Remove(this);
        }

        if (boids.Any())
            acceleration = Alignment(boids) * aligmentMultiplayer + Separation(boids) * separationMultiplayer + Cohesion(boids) * cohesionMultiplayer;
        else
            acceleration = Vector3.zero;

        velocity += acceleration;
        velocity = LimitMagnitude(velocity, maxSpeed);
        Position += velocity * Time.deltaTime;
        Quaternion to = Quaternion.Euler(velocity);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, to, 2);
    }

    private Vector3 Alignment (IEnumerable<Flocking> boids)
    {
        Vector3 velocity = Vector3.zero;

        foreach (Flocking boid in boids)
        {
            velocity += boid.velocity;
        }

        velocity /= boids.Count();

        return Steer(velocity.normalized * maxSpeed);
    }

    private Vector3 Cohesion (IEnumerable<Flocking> boids)
    {
        Vector3 sumPositions = Vector3.zero;
        foreach (Flocking boid in boids)
        {
            sumPositions += boid.Position;
        }

        Vector3 average = sumPositions / boids.Count();
        Vector3 direction = average - Position;

        return Steer(direction.normalized * maxSpeed);
    }

    private Vector3 Separation (IEnumerable<Flocking> boids)
    {
        Vector3 direction = Vector3.zero;
        boids = boids.Where(o => DistanceTo(o) <= checkRadius / 2);

        foreach (var boid in boids)
        {
            Vector3 difference = Position - boid.Position;
            direction += difference.normalized / difference.magnitude;
        }

        direction /= boids.Count();

        return Steer(direction.normalized * maxSpeed);
    }

    private Vector3 Steer (Vector3 desired)
    {
        Vector3 steer = desired - velocity;
        return LimitMagnitude(steer, maxForce);
    }

    private float DistanceTo (Flocking boid)
    {
        return Vector3.Distance(boid.transform.position, Position);
    }

    private Vector3 LimitMagnitude (Vector3 baseVector, float maxMagnitude)
    {
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }

        return baseVector;
    }

    private void OnDrawGizmosSelected ()
    {
        Color newColor = Color.red;
        newColor.a = 0.3f;
        Gizmos.color = newColor;
        Gizmos.DrawSphere(transform.position, checkRadius);
        if (boids != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Flocking boid in boids)
            {
                Gizmos.DrawLine(transform.position, boid.transform.position);
            }
        }
    }
}