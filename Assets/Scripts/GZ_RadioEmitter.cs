using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GZ_RadioEmitter : MonoBehaviour {

    public static GZ_RadioEmitter Instance;
    public GameObject WavePrefab;

    void Awake()
    {
        Instance = this;
    }

    public void Bang()
    {
        Instantiate(WavePrefab);
        GZ_AudioModule.Instance.Play(1);
        GZ_RecordModule.Instance.Bang();
    }
}
