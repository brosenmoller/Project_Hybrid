using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;

    private AudioSource source;
   
    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    public void PlaySound(int clipNumber)
    {
        AudioClip CurrentClip = clips[clipNumber -1];
        if(CurrentClip != null)
        {
            source.clip = CurrentClip;
            source.Play();
        }
        
    }





}
