using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    [SerializeField] List<Flocking> boids = new List<Flocking>();

    private Vector3 baseRotation;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxForce;
    [SerializeField] private float checkRadius;

    [SerializeField] private float separationMultiplayer;
    [SerializeField] private float cohesionMultiplayer;
    [SerializeField] private float aligmentMultiplayer;

    [SerializeField] private Vector3 velocity;
    private Vector3 acceleration;

    void Start ()
    {
        baseRotation.x = Random.Range(0, 180);
        baseRotation.y = Random.Range(0, 180);
        baseRotation.z = Random.Range(0, 180);

        float angle = Random.Range(0, 2.0f * Mathf.PI);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
        velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    void Update ()
    {
        Collider[] otherColliders = Physics.OverlapSphere(transform.position, checkRadius);
        boids.Clear();
        if (otherColliders != null)
        {
            for (int i = 0; i < otherColliders.Length; i++)
            {
                Flocking boid = otherColliders[i].GetComponent<Flocking>();
                if (boid)
                    boids.Add(boid);
            }

            boids.Remove(this);
        }

        if (boids.Any())
            acceleration = Alighmet(boids) * aligmentMultiplayer + Separation(boids) * separationMultiplayer + Cohesion(boids) * cohesionMultiplayer;
        else
            acceleration = Vector3.zero;

        velocity += acceleration;
        velocity = Clamp(velocity, maxSpeed);

        transform.position += velocity * Time.deltaTime;

        float newAngleX = Mathf.Atan2(velocity.y, velocity.z) * Mathf.Rad2Deg;
        float newAngleY = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Rad2Deg;
        float newAngleZ = Mathf.Atan2(velocity.x, velocity.y) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(newAngleX, newAngleY, newAngleZ) + baseRotation);
    }

    private Vector3 Cohesion (List<Flocking> boids)
    {
        Vector3 position = Vector3.zero;

        foreach (Flocking boid in boids)
        {
            position += boid.transform.position;
        }

        Vector3 avg = position / boids.Count();
        Vector3 direction = avg - transform.position;

        return Steer(direction.normalized, maxForce);
    }

    private Vector3 Separation (List<Flocking> boids)
    {
        Vector3 direction = Vector3.zero;

        foreach (Flocking boid in boids)
        {
            Vector3 diff = transform.position - boid.transform.position;
            direction += diff.normalized / diff.magnitude;
        }

        direction /= boids.Count;

        return Steer(direction.normalized, maxForce);
    }

    private Vector3 Alighmet (List<Flocking> boids)
    {
        Vector3 velocity = Vector3.zero;

        foreach (Flocking boid in boids)
        {
            velocity += boid.velocity;
        }

        velocity /= boids.Count;

        return Steer(velocity.normalized, maxForce);
    }

    private Vector3 Steer (Vector3 desired, float max)
    {
        Vector3 steer = desired - velocity;
        return Clamp(steer, max);
    }

    private Vector3 Clamp (Vector3 baseVector, float max)
    {
        if (baseVector.sqrMagnitude > max * max)
            baseVector = baseVector.normalized * max;
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