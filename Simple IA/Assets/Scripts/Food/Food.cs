using System;
using UnityEngine;

public class Food : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public event Action<Food> OnEated;

    public void EatFood ()
    {
        OnEated?.Invoke(this);
    }
}