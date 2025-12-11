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
    [SerializeField] private int hp = 5;
    [SerializeField] private Transform _frontTransform;

    private void Start()
    {
        NetworkIdentity identity = gameObject.GetComponent<NetworkIdentity>();
        
        if (!IsLocalPlayer)
        {
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
            Vector3 newPos = transform.position + (Vector3)(speed * dir) * Time.deltaTime;
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

        if (bullet) 
        {
            if (playerController.Shoot) 
            {
                CmdFire();
            }
        }

        playerController.ResetInput();
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
