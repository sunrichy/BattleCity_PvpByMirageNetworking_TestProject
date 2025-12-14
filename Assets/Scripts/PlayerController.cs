using UnityEngine;

public static class InGameTool
{
    public static Vector3 MoveDirectionToVector3(this MoveDirection moveDirection) 
    {
        switch (moveDirection) 
        {
            case MoveDirection.Up:
                return Vector3.up;

            case MoveDirection.Down:
                return Vector3.down;

            case MoveDirection.Right:
                return Vector3.right;

            case MoveDirection.Left:
                return Vector3.left;

            default :
                return Vector3.zero;
        }
    }
}

public class PlayerController : MonoBehaviour
{
    public MoveDirection MoveDir { get; private set; }
    public bool Shoot { get; private set; }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            Shoot = true;
        }

        if (MoveDir != MoveDirection.Idle)
        {
            return;
        }

        if (Input.GetKey(KeyCode.W)) 
        {
            MoveDir = MoveDirection.Up;
            return;
        }

        if (Input.GetKey(KeyCode.D)) 
        {
            MoveDir = MoveDirection.Right;
            return;
        }

        if (Input.GetKey(KeyCode.S)) 
        {
            MoveDir = MoveDirection.Down;
            return;
        }

        if (Input.GetKey(KeyCode.A)) 
        {
            MoveDir = MoveDirection.Left;
            return;
        }
    }

    public void ResetInput() 
    {
        MoveDir = MoveDirection.Idle;
        Shoot = false;
    }
}