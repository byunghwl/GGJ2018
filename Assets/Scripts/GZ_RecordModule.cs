using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GZ_RecordModule : MonoBehaviour {

    public static GZ_RecordModule Instance;

    public GameObject[] Backgrounds;
    public GameObject[] ConfirmButtons;
    public GameObject CanvasObject;
    public GameObject SpriteObject;

    private int _selectedPlayer;

    AudioClip[] Clips = new AudioClip[2];
    public AudioSource[] Sources;

    void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void OnClickedPlay()
    {
        ME_AudioEditModule.Instance.Play();
    }

    public void OnClickedRecord()
    {
        ME_MicModule.Instance.Record();
    }

    public void OnClickedStop()
    {
        ME_MicModule.Instance.StopRecord();
        ME_AudioEditModule.Instance.Play();
    }

    public void OnClickedConfirm()
    {
        Clips[_selectedPlayer] = ME_AudioEditModule.Instance.CreatedClip;
        Sources[_selectedPlayer].clip = Clips[_selectedPlayer];
    }

    public void Bang()
    {
        Sources[0].Play();
        Sources[1].Play();
    }

    public void SetVolume(float defaultV, float redPercentage)
    {
        Sources[0].volume = defaultV  * redPercentage;
        Sources[1].volume = defaultV * (1 - redPercentage);
    }

    public void SetUpPlayer(int index)
    {
        HideAll();
        _selectedPlayer = index;
        SpriteObject.SetActive(true);
        CanvasObject.SetActive(true);
        this.gameObject.SetActive(true);
        Backgrounds[index].SetActive(true);
        ConfirmButtons[index].SetActive(true);
    }

    public void HideAll()
    {
        SpriteObject.SetActive(false);
        CanvasObject.SetActive(false);

        for (int i =0; i < 2; i++)
        {
            Backgrounds[i].SetActive(false);
            ConfirmButtons[i].SetActive(false);
        }
    }

    private void Update()
    {
        
      
    }
}
