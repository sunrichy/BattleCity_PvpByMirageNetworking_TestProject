using Mirage;
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

        if (!networkIdentity.IsHost) 
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
        networkIdentity = GetComponent<NetworkIdentity>();
        init = true;
    }

    NetworkIdentity networkIdentity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!networkIdentity || !networkIdentity.IsHost)
        {
            return;
        }

        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Wall")
        {
            GameObject target = collision.gameObject;
            if (target.TryGetComponent(out ITakeDamage takeDamage)) 
            {
                takeDamage.TakeDamage();
            }
            // Destroy(gameObject);
            GameManager.Instacne.networkManager.ServerObjectManager.Destroy(gameObject, true);
        }
    }
}

public interface ITakeDamage
{
    public void TakeDamage();
}