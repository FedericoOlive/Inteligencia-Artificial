using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private static CameraFollow instance = null;

    public static CameraFollow Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<CameraFollow>();

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public void Reset()
    {
        this.transform.position = new Vector3(0, 5, transform.position.z);
    }

    public void UpdateCamera ()
    {
        Vector3 follow = Vector3.zero;

        if (PopulationManager.Instance != null)
        {
            if (PopulationManager.Instance.GetBestAgent() != null)
            {
                Vector3 pos = transform.position;
                pos.x += 3 * Time.deltaTime;
                transform.position = pos;
            }
        }
    }
}
