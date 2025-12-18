using UnityEngine;
using UnityEngine.UI;

public class ButtonUiSfx : MonoBehaviour
{
    private Button _button { get; set; }

    private void Start()
    {
        if(gameObject.TryGetComponent(out Button component)) 
        {
            _button = component;
            _button.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (MusicBox.Instacne) 
        {
            MusicBox.Instacne.PlaySfx("Click_UI");
        }
    }
}
