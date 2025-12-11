using Mirage;
using UnityEngine;

public class Wall : NetworkBehaviour, ITakeDamage
{
    [SerializeField] private int hp;
    [SerializeField] private bool invisible;

    public void TakeDamage()
    {
        if (invisible) 
        {
            return;
        }

        hp -= 1;
        if (hp <= 0) 
        {
            if (GameManager.Instacne.networkManager) 
            {
                GameManager.Instacne.networkManager.ServerObjectManager.Destroy(gameObject, true);
            }
            else 
            {
                Destroy(gameObject);
            }
        }
    }
}