using UnityEngine;

public class Mine : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Team team;
    private Color colorTeam;

    public void SetTeam (Team newTeam)
    {
        team = newTeam;
        meshRenderer.material.color = Village.GetColorCiv(team);
    }
}