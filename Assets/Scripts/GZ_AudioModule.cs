using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GZ_AudioModule : MonoBehaviour {
    public static GZ_AudioModule Instance;

    public AudioSource[] Sources;

    private void Awake()
    {
        Instance = this;
    }
    
	public void Play(int index)
    {
        Sources[index].Play();
    }
}
