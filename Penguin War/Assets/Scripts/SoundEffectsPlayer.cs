using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsPlayer : MonoBehaviour
{
    public AudioSource src;
    public AudioClip spawned_sound, died_sound, builded_sound, collapsed_sound, cant_sound, lost_sound, won_sound;

    public void ply_spawned(){
        src.clip = spawned_sound;
        src.Play();
    }

    public void ply_died(){
        src.clip = died_sound;
        src.Play();
    }

    public void ply_build(){
        src.clip = builded_sound;
        src.Play();
    }

    public void ply_colapse(){
        src.clip = collapsed_sound;
        src.Play();
    }

    public void ply_cant(){
        src.clip = cant_sound;
        src.Play();
    }

    public void ply_lost(){
        src.clip = lost_sound;
        src.Play();
    }

    public void ply_won(){
        src.clip = won_sound;
        src.Play();
    }
}
