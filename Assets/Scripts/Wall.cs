using UnityEngine;

public class Wall : MonoBehaviour, ITakeDamage
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
            Destroy(gameObject);
        }
    }
}
