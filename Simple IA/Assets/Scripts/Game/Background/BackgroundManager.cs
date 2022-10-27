using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject[] frames;
    private Camera cam;
    float lastCameraPos;
    float accumPos;

    private static BackgroundManager instance = null;

    public static BackgroundManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<BackgroundManager>();

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
        cam = Camera.main;
    }

    public void Reset()
    {
        this.transform.position = new Vector3(0, 5, 10);
        lastCameraPos = 0;
        accumPos = 0;

        float posx = -5.5f;

        foreach (GameObject go in frames)
        {
            Vector3 pos = go.transform.position;
            pos.x = posx;
            go.transform.position = pos;
            posx += 7.2f;
        }
    }

    void Update()
    {
        Vector3 camPos = cam.transform.position;

        float delta = camPos.x - lastCameraPos;

        Vector3 parallax = transform.position;
        parallax.x += delta * 0.2f;
        transform.position = parallax;

        delta -= delta * 0.2f;

        lastCameraPos = camPos.x;
        accumPos += delta;

        if (accumPos >= 7.2f)
        {
            foreach (GameObject go in frames)
            {
                Vector3 pos = go.transform.position;
                pos.x += 7.2f;
                go.transform.position = pos;
            }
            accumPos -= 7.2f;
        }
    }
}
