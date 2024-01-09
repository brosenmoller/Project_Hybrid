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
        //loadCollider = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        if (frameCollider.IsTouching(playerCollider)) { roomContents.SetActive(true); }
        else { roomContents.SetActive(false); }
    }

    private void Update()
    {
        if (frameCollider.IsTouching(playerCollider))
        {
            virtualCamera.Priority = 1;
            roomContents.SetActive(true);
            playerController.SetRoomIndex(roomIndex);
            CancelInvoke(nameof(DeactivateRoom));
        }
        else 
        { 
            virtualCamera.Priority = 0;
            Invoke(nameof(DeactivateRoom), 2f);
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag(UniversalConstansts.PlayerTag))
    //    {
    //        roomContents.SetActive(true);
    //        CancelInvoke("DeactivateRoom");
    //    }
    //}
    //// Disable the roomContents on leaving the loadCollider
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag(UniversalConstansts.PlayerTag))
    //    {
    //        Invoke("DeactivateRoom", 2f);
    //    }
    //}
    //// Delayed disabling of the room to avoid disabling and re-enabling multiple times on borders
    private void DeactivateRoom()
    {
        roomContents.SetActive(false);
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
