using UnityEngine;
using Cinemachine;

public class RoomController : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject roomContents;
    [SerializeField] private PolygonCollider2D frameCollider;
    public Transform playerSpawnPoint;

    private Collider2D playerCollider;
    private PlayerController playerController;

    private int roomIndex;

    private void Awake()
    {
       playerController = FindObjectOfType<PlayerController>();
       playerCollider = playerController.GetComponent<Collider2D>();

        roomIndex = transform.GetSiblingIndex();
    }

    private void Update()
    {
        if (frameCollider.IsTouching(playerCollider))
        {
            virtualCamera.Priority = 1;
            playerController.SetRoomIndex(roomIndex);
        }
        else { virtualCamera.Priority = 0; }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(frameCollider.bounds.center, frameCollider.bounds.size);

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (frameCollider.bounds.Contains(playerController.transform.position)) { virtualCamera.Priority = 1; }
        else { virtualCamera.Priority = 0; }
    }
}
