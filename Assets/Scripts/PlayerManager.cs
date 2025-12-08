using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float speed = 5f;

    [SerializeField] private Bullet bullet;

    private MoveDirection currentDir = MoveDirection.Up;

    private void FixedUpdate()
    {
        Vector2 dir = playerController.MoveDir.MoveDirectionToVector3();

        if(dir != Vector2.zero) 
        {
            transform.position += (Vector3) (speed * dir) * Time.deltaTime;
            currentDir = playerController.MoveDir;
        }

        if (bullet) 
        {
            if (playerController.Shoot) 
            {
                Bullet b = Instantiate(bullet, null);
                b.transform.position = transform.position;
                b.Init(currentDir);
            }
        }
    }

}
