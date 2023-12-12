using UnityEngine;
using System.Collections;

public class Smasher : MonoBehaviour
{
    [Header("Smasher")]
    [SerializeField] private float stompTime;
    [SerializeField] private float resetTime;
    [SerializeField] private float pauseTime;
    [SerializeField] private float startDelay;
    [SerializeField] private Vector2 endPos;
    [SerializeField] private AudioObject smashSound;
    private Vector2 startPos;

    private bool smashing = true;
    public void SetSmashing(bool smashing)
    {
        this.smashing = smashing;
    }

    private void Awake()
    {
        if (!smashing) gameObject.tag = "";
        startPos = transform.localPosition;
    }

    private void OnEnable()
    {
        if (smashing) StartCoroutine(Smashing());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        transform.localPosition = startPos;
    }

    private IEnumerator Smashing()
    {
        yield return new WaitForSeconds(startDelay);

        while (smashing)
        {
            yield return MoveY(endPos.y, stompTime);

            if (smashSound != null ) { smashSound.Play(); }

            yield return MoveY(startPos.y, resetTime);

            yield return new WaitForSeconds(pauseTime);
        }
    }

    private IEnumerator MoveY(float targetY, float time)
    {
        float elapsed_time = 0;
        float startY = transform.localPosition.y;

        while (elapsed_time <= time)
        {
            float newYPos = Mathf.Lerp(startY, targetY, Mathf.Pow((elapsed_time / time), 2));

            transform.localPosition = new Vector2(transform.localPosition.x, newYPos);

            yield return null;

            elapsed_time += Time.deltaTime;
        }
    }
}
