using UnityEngine;

public class PlayerManager : MonoBehaviour, ITakeDamage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float speed = 5f;

    [SerializeField] private Bullet bullet;

    private MoveDirection currentDir = MoveDirection.Up;
    [SerializeField] private int hp = 5;
    [SerializeField] private Transform _frontTransform;

    public void TakeDamage()
    {
        hp -= 1;
        if(hp <= 0) 
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (!playerController) 
        {
            return;
        }

        Vector2 dir = playerController.MoveDir.MoveDirectionToVector3();

        if(dir != Vector2.zero) 
        {
            transform.position += (Vector3) (speed * dir) * Time.deltaTime;
            currentDir = playerController.MoveDir;
            transform.localRotation = Quaternion.Euler(90f * ((int) currentDir - 1) * Vector3.back);
        }

        if (bullet) 
        {
            if (playerController.Shoot) 
            {
                Bullet b = Instantiate(bullet, null);
                b.transform.position = _frontTransform.position;
                b.Init(currentDir);
            }
        }

        playerController.ResetInput();
    }

}
