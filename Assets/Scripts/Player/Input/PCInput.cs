using UnityEngine;

public class PCInput : IPlayerInput
{
    public Vector3 MoveDitection()
    {
        Vector3 ditection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        return ditection;
    }
    public Vector2 CameraDitection()
    {
        Vector2 ditection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        return ditection;
    }

    public bool InteractionButton()
    {
        return Input.GetMouseButtonDown(0);
    }

    public bool RotateLeft()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        return scroll < 0f;
    }

    public bool RotateRight()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        return scroll > 0f;
    }
}
