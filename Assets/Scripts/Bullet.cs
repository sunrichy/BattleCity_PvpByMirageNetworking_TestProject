using UnityEngine;

public class Bullet : MonoBehaviour
{
    // MoveDirection moveDirection;
    [SerializeField] private float speed;
    Vector3 vector2Dir;
    bool init;

    private void FixedUpdate()
    {
        if (!init)
        {
            return;
        }

        transform.position += vector2Dir * speed * Time.deltaTime;

        Vector2 d = Camera.main.WorldToScreenPoint(transform.position);

        if (d.x < -100f || d.x > Screen.width + 100f ||
                d.y < -100f || d.y > Screen.height + 100f) 
        {
            Destroy(gameObject);
        }
    }

    public void Init(MoveDirection direction) 
    {
        if (init) 
        {
            return;
        }

        vector2Dir = direction.MoveDirectionToVector3();
        init = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Wall")
        {
            GameObject target = collision.gameObject;
            if (target.TryGetComponent(out ITakeDamage takeDamage)) 
            {
                takeDamage.TakeDamage();
            }
            Destroy(gameObject);
        }
    }
}

public interface ITakeDamage
{
    public void TakeDamage();
}