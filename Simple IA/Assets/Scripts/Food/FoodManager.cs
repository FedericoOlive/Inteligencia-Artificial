using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FoodManager : MonoBehaviour
{
    [SerializeField] private GameObject prefabFood;
    [SerializeField] private float maxFood = 10;
    [SerializeField] private List<Food> foods = new List<Food>();
    [SerializeField] private bool locateFood;
    [SerializeField] private Collider coll;
    
    private void Update ()
    {
        if (locateFood)
        {
            locateFood = false;
            InitFood();
        }
    }

    private void InitFood ()
    {
        if (!Application.isPlaying)
            for (int i = 0; i < foods.Count; i++)
                DestroyImmediate(foods[i].gameObject);
        else
            for (int i = 0; i < foods.Count; i++)
                Destroy(foods[i].gameObject);
        foods.Clear();

        Vector3 pos = Vector3.zero;

        for (int i = 0; i < maxFood; i++)
        {
            pos = GetRandomAvailablePos();
            GameObject foodGo = Instantiate(prefabFood, pos, Quaternion.identity, transform);
            Food food = foodGo.GetComponent<Food>();
            foods.Add(food);
        }
    }

    private void RelocateFood ()
    {
        for (int i = 0; i < foods.Count; i++)
        {
            Vector3 pos = GetRandomAvailablePos();
            foods[i].transform.position = pos;
        }
    }

    private Vector3 GetRandomAvailablePos ()
    {
        bool posDisable;
        Vector3 randomPos;
        int indexExit = 0;

        do
        {
            randomPos = transform.position;
            posDisable = false;
            randomPos.x += Random.Range(-(int) coll.bounds.size.x / 2, (int) coll.bounds.size.x / 2);
            randomPos.z += Random.Range(-(int) coll.bounds.size.z / 2, (int) coll.bounds.size.z / 2);

            for (int i = 0; i < foods.Count; i++)
            {
                if (foods[i].transform.position == randomPos)
                {
                    posDisable = true;
                }
            }

            indexExit++;
            if (indexExit > 100)
            {
                Debug.LogWarning("Error de Límites");
                return Vector3.zero;
            }

        } while (posDisable);

        return randomPos;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.blue;
        Vector3 pos = transform.position - Vector3.one / 2;
        Gizmos.DrawWireCube(pos, coll.bounds.size);
    }
#endif
}