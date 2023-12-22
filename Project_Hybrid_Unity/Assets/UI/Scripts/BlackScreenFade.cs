using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenFade : MonoBehaviour
{
    [Header("BlackScreen Fade In")]
    [SerializeField] private float blackScreenFadeDuration;
    [SerializeField] private float fadeDelay;
    [SerializeField] private Image blackscreenImage;

    private void Awake()
    {
        blackscreenImage.color = Color.black;
    }

    private void Start()
    {
        StartBlackScreenFadeAway();
    }

    public void StartBlackScreenFadeAway()
    {
        blackscreenImage.color = Color.black;
        StartCoroutine(FadeColorAlpha(blackscreenImage, 0, blackScreenFadeDuration, fadeDelay));
    }

    public void BlackScreenFadeIn(float fadeDuration)
    {
        blackscreenImage.color = new Color(0, 0, 0, 0);
        StartCoroutine(FadeColorAlpha(blackscreenImage, 1, fadeDuration, 0));
    }

    private IEnumerator FadeColorAlpha(Image image, float target, float fadeDuration, float delay)
    {
        float elapsed_time = 0;
        Color startColor = image.color;

        yield return new WaitForSecondsRealtime(delay);

        while (elapsed_time <= fadeDuration)
        {
            float a = Mathf.Lerp(startColor.a, target, Mathf.Pow((elapsed_time / fadeDuration), 2));

            image.color = new Color(image.color.r, image.color.g, image.color.b, a);

            yield return null;

            elapsed_time += Time.deltaTime;
        }
    }
}
