using System.Collections;
using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private float endY;
    [SerializeField] private float lockTime;

    public void Lock()
    {
        StartCoroutine(Locking());
    }

    private IEnumerator Locking()
    {
        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = new(startPosition.x, endY, startPosition.z);

        float time = 0f;
        while (time < 1f) 
        {
            time += Time.deltaTime / lockTime;
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, time * time);
            yield return null;
        }
    }
}
