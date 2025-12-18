using UnityEngine;

public class LoadingUi : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _scaleTo = 0.7f;
    [SerializeField] private float _scaleDuration = 1f;

    float n_scaleDuration;
    bool scaleTo;
    float speedScale;

    private void Awake()
    {
        n_scaleDuration = _scaleDuration;
        speedScale = _scaleTo - transform.localScale.x;
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y, 
            transform.rotation.eulerAngles.z + (_rotationSpeed * Time.deltaTime) );

        if (n_scaleDuration > 0f) 
        {
            transform.localScale += Vector3.one * speedScale * Time.deltaTime * (scaleTo ? -1f: 1f);
        }
        else 
        {
            scaleTo = !scaleTo;
            n_scaleDuration = _scaleDuration;
        }

        n_scaleDuration -= Time.deltaTime;
    }
}
