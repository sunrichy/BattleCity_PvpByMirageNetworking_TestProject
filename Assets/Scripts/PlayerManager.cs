using Mirage;
using UnityEngine;

public class PlayerManager : NetworkBehaviour, ITakeDamage
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float speed = 5f;

    [SerializeField] private Bullet bullet;

    [SyncVar]
    private MoveDirection currentDir = MoveDirection.Up;

    [SyncVar]
    private float _speedSyncVar;

    [SyncVar]
    [SerializeField] private int hp = 5;
    [SerializeField] private Transform _frontTransform;

    [SerializeField] private Color _enemyColor = Color.red;

    private void Start()
    {
        NetworkIdentity identity = gameObject.GetComponent<NetworkIdentity>();
        
        if (!IsLocalPlayer)
        {
            if (gameObject.TryGetComponent(out SpriteRenderer spriteRenderer)) 
            {
                spriteRenderer.color = _enemyColor;
            }
            Destroy(playerController);
        }
    }

    [ClientRpc]
    public void TakeDamage()
    {
        hp -= 1;
        if(hp <= 0) 
        {
            gameObject.SetActive(false);
            GameManager.Instacne.networkManager.Client.Send(new StartGame.PlayerDead());
        }
    }

    private void FixedUpdate()
    {
        if (!playerController) 
        {
            UpdateOnLocalPlayer();
            return;
        }

        Vector2 dir = playerController.MoveDir.MoveDirectionToVector3();

        if(dir != Vector2.zero) 
        {
            _speedSyncVar = speed * Time.deltaTime;
            Vector3 newPos = transform.position + ( (Vector3) dir * _speedSyncVar );
            Vector2 d = Camera.main.WorldToScreenPoint(newPos);

            if (d.x < -10f || d.x > Screen.width + 10f ||
                    d.y < -10f || d.y > Screen.height + 10f)
            {

            }
            else 
            {
                transform.position = newPos;
            }



            currentDir = playerController.MoveDir;
            transform.localRotation = Quaternion.Euler(90f * ((int) currentDir - 1) * Vector3.back);
        }
        else 
        {
            _speedSyncVar = 0f;
        }

        if (bullet) 
        {
            if (playerController.Shoot) 
            {
                CmdFire();
            }
        }

        playerController.ResetInput();
    }

    private void UpdateOnLocalPlayer()
    {
        transform.localRotation = Quaternion.Euler(90f * ((int) currentDir - 1) * Vector3.back);

        Vector2 dir = currentDir.MoveDirectionToVector3();
        Vector3 newPos = transform.position + ((Vector3) dir * _speedSyncVar);
        transform.position = newPos;
    }

    [ServerRpc]
    private void CmdFire()
    {
        Bullet b = Instantiate(bullet, null);
        b.transform.position = _frontTransform.position;
        b.Init(currentDir);
        GameManager.Instacne.networkManager.ServerObjectManager.Spawn(b.gameObject.GetComponent<NetworkIdentity>());
    }
}
