using UnityEngine;

public class PlayerCostume : MonoBehaviour
{
    [SerializeField] private TargetData[] _swapSpriteDatas;
    [SerializeField] private GameObject[] _hpGameObjects;

    [System.Serializable]
    private struct TargetData
    {
        public SpriteRenderer spriteRenderer;
        public Sprite localSprite;
        public Sprite nonLocalSprite;

    }

    public void SetCostume(bool isLocal) 
    {
        for (int i = 0; i < _swapSpriteDatas.Length; i++) 
        {
            var data = _swapSpriteDatas[i];
            if (isLocal)
            {
                data.spriteRenderer.sprite = data.localSprite;
            }
            else 
            {
                data.spriteRenderer.sprite = data.nonLocalSprite;
            }
        }
    }

    public void SetHp(int hp)
    {
        for (int i = 0; i < _hpGameObjects.Length; i++) 
        {
            _hpGameObjects[i].SetActive(i < hp);
        }
    }
}
