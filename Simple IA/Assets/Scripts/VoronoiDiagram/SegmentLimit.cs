using UnityEngine;

[System.Serializable]
public class SegmentLimit
{
    [SerializeField] private Transform origin;
    [SerializeField] private Transform final;
    [SerializeField] private DirectionLimit directionLimit = DirectionLimit.None;
    private Vector3 opositePosition;

    public Vector3 Origin => origin.position;
    public Vector3 Final => final.position;

    public Vector3 GetOpositePosition (Vector3 pos)
    {
        Vector3 newPos = Vector3.zero;
        float distanceX = Mathf.Abs(Mathf.Abs(pos.x) - Mathf.Abs(origin.position.x)) * 2;
        float distanceZ = Mathf.Abs(Mathf.Abs(pos.z) - Mathf.Abs(origin.position.z)) * 2;

        switch (directionLimit)
        {
            case DirectionLimit.None:
                Debug.LogWarning("Está en None el Limite.");
                break;
            case DirectionLimit.Left:
                newPos.x = pos.x - distanceX;
                newPos.y = pos.y;
                newPos.z = pos.z;
                break;
            case DirectionLimit.Up:
                newPos.x = pos.x;
                newPos.y = pos.y;
                newPos.z = pos.z + distanceZ;
                break;
            case DirectionLimit.Right:
                newPos.x = pos.x + distanceX;
                newPos.y = pos.y;
                newPos.z = pos.z;
                break;
            case DirectionLimit.Down:
                newPos.x = pos.x;
                newPos.y = pos.y;
                newPos.z = pos.z - distanceZ;
                break;
            default:
                Debug.LogWarning("Default el Limite.");
                break;
        }

        opositePosition = newPos;
        return newPos;
    }
}

enum DirectionLimit
{
    None,
    Left,
    Up,
    Right,
    Down
}