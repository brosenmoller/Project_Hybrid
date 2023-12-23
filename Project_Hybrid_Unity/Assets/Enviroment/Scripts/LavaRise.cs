using UnityEngine;

public class LavaRise : MonoBehaviour
{
    [Header("Lava Settings")]
    [SerializeField] private float startPosition;
    [SerializeField] private float endPostion;
    [SerializeField] private float riseSpeed;
    [SerializeField] private bool autoStart;

    private bool riseIsStopped = true;

    private void Awake()
    {
        riseIsStopped = !autoStart;
        transform.localPosition = new Vector3(transform.localPosition.x, startPosition, transform.localPosition.z);
    }

    public void StartRise()
    {
        riseIsStopped = false;
    }

    private void Update()
    {
        Rise();
    }

    private void Rise()
    {
        if (riseIsStopped || transform.localPosition.y > endPostion) { return; }

        transform.Translate(new Vector3(0, riseSpeed * Time.deltaTime, 0));
    }
}
