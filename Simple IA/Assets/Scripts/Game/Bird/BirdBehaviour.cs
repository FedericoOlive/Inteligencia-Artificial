using System;
using UnityEngine;

public class BirdBehaviour : MonoBehaviour
{
    const float GRAVITY = 20.0f;
    const float MOVEMENT_SPEED = 3.0f;
    const float FLAP_SPEED = 7.5f;
    private BirdBase birdBase;

    private void Awake ()
    {
        birdBase = GetComponent<BirdBase>();
    }

    public Vector3 speed
    {
        get; private set;
    }

    public void Reset()
    {
        speed = Vector3.zero;
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
    }

    public void Flap()
    {
        Vector3 newSpeed = speed;
        newSpeed.y = FLAP_SPEED;
        speed = newSpeed;
    }

    public bool Shoot ()
    {
        Debug.DrawRay(transform.position, Vector3.right * 10, Color.red, 1);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right*10, 10, LayerMask.GetMask("Obstacle"));
        if (hit)
        {
            ObstacleBase obstacleBase = hit.transform.GetComponentInParent<ObstacleBase>();
            if (obstacleBase)
                return obstacleBase.Disable(birdBase);
        }

        return false;
    }

    public void UpdateBird(float dt)
    {
        Vector3 newSpeed = speed;
        newSpeed.x = MOVEMENT_SPEED;
        newSpeed.y -= GRAVITY * dt;
        speed = newSpeed;

        this.transform.rotation = Quaternion.AngleAxis(speed.y * 5f, Vector3.forward);

        this.transform.position += speed * dt;
    }
}