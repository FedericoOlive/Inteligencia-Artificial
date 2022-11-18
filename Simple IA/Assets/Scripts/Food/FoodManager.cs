using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FoodManager : MonoBehaviour
{
    [SerializeField] private GameObject prefabFood;
    [SerializeField] private LevelSettings levelSettings;
    [SerializeField] private List<Food> foods = new List<Food>();
    [SerializeField] private bool locateFood;
    [SerializeField] private BoxCollider coll;

    private void Update ()
    {
        if (locateFood)
        {
            locateFood = false;
            Init();
        }
    }

    public void Init ()
    {
        DeInit();
        transform.position = levelSettings.autoRePositionFoodCenter;
        coll.center = new Vector3(-0.5f, 0, -0.5f);
        coll.size = new Vector3(levelSettings.size, 0.01f, levelSettings.size);
        Vector3 pos = Vector3.zero;

        for (int i = 0; i < levelSettings.maxFood; i++)
        {
            pos = GetRandomAvailablePos();
            GameObject foodGo = Instantiate(prefabFood, pos, Quaternion.identity, transform);
            Food food = foodGo.GetComponent<Food>();
            foods.Add(food);
            food.OnEated += RelocateFood;
        }
    }

    public void DeInit ()
    {
        if (!Application.isPlaying)
        {
            for (int i = 0; i < foods.Count; i++)
            {
                if (foods[i])
                {
                    foods[i].OnEated -= RelocateFood;
                    DestroyImmediate(foods[i].gameObject);
                }
            }
        }
        else
        {
            for (int i = 0; i < foods.Count; i++)
            {
                if (foods[i])
                {
                    foods[i].OnEated -= RelocateFood;
                    Destroy(foods[i].gameObject);
                }
            }
        }

        foods.Clear();
    }

    public void RelocateFoods ()
    {
        for (int i = 0; i < foods.Count; i++)
        {
            RelocateFood(foods[i]);
        }
    }

    public void RelocateFood (Food food)
    {
        Vector3 pos = GetRandomAvailablePos();
        food.transform.position = pos;
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

            if (!posDisable && Application.isPlaying)
            {
                List<PopulationManager> populations = GameManager.Get().GetPopulations();
                for (int i = 0; i < populations.Count; i++)
                {
                    List<Villager> villagers = GameManager.Get().GetPopulations()[i].village.populationGOs;
                    for (int j = 0; j < villagers.Count; j++)
                    {
                        Vector3 posVillager = villagers[j].transform.position;

                        if (posVillager == randomPos)
                        {
                            posDisable = true;
                        }
                    }
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

    public Food GetNearFood (Vector3 pos)
    {
        float minDistance = float.MaxValue;
        int index = 0;

        for (int i = 0; i < foods.Count; i++)
        {
            float distance = Vector3.Distance(foods[i].transform.position, pos);
            if (distance < minDistance)
            {
                index = i;
                minDistance = distance;
            }
        }

        return foods[index];
    }

    // Can be Null
    public Food GetFoodInPosition (Vector3 pos)
    {
        for (int i = 0; i < foods.Count; i++)
        {
            if (foods[i].transform.position == pos)
                return foods[i];
        }

        return null;
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