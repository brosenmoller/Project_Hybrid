﻿using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public const string ROOM_SAVE_STRING = "ROOM";
    public bool CanMove { get; set; } = true;
    public bool CanJump { get; set; } = true;

    [Header("References")]
    [SerializeField] private Transform rooms;

    private int roomIndex;

    private void Start()
    {
        roomIndex = PlayerPrefs.HasKey(ROOM_SAVE_STRING) ? PlayerPrefs.GetInt(ROOM_SAVE_STRING) : 0;
        transform.position = rooms.GetChild(roomIndex).GetComponent<RoomController>().playerSpawnPoint.position;
    }

    public void SetRoomIndex(int roomIndex)
    {
        if (roomIndex < rooms.childCount)
        {
            this.roomIndex = roomIndex;
        }
    }

    public void Death()
    {
        PlayerPrefs.SetInt(ROOM_SAVE_STRING, roomIndex);
        GameManager.Instance.ReloadScene();
    }
}

