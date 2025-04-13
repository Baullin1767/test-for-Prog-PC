using UnityEngine;

public interface IPlayerInput
{
    public Vector3 MoveDitection();
    public Vector2 CameraDitection();
    public bool InteractionButton();
    public bool RotateLeft();
    public bool RotateRight();
}
