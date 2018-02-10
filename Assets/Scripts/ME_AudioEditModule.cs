using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ME_AudioEditModule: MonoBehaviour
{
    public static ME_AudioEditModule Instance;

    public SpriteRenderer Renderer;
    public ME_UISlider[] EditSliders;

    private float[] _targetSamples;

    public GameObject SamplePrefab;

    public AudioClip CreatedClip;

    private void Start()
    {
        Instance = this;
    }

    public void SetEnabled(bool enabled)
    {
        this.gameObject.SetActive(enabled);
    }

    public void SetAudioSamples(float[] samples)
    {
        _targetSamples = samples;
    
        Renderer.sprite = GetWaveFormTexture(_targetSamples, 500, 100, Color.white, Color.grey, 0, _targetSamples.Length);
        EditSliders[0].SetPosition(new Vector3(-999, 0,9f));
        EditSliders[1].SetPosition(new Vector3(999, 0,9f));
    }

    private void Update()
    {
        if(EditSliders[0].IsDragging || EditSliders[1].IsDragging)
        {
            int trimStartIdx = ConvertWorldPos2SampleIndex(EditSliders[0].Percentage.x);
            int trimEndIdx = ConvertWorldPos2SampleIndex(EditSliders[1].Percentage.x);

            Renderer.sprite = GetWaveFormTexture(_targetSamples, 500, 100, Color.white, Color.grey, trimStartIdx, trimEndIdx);
        }
    }

    public void Trim()
    {
        int trimStartIdx = ConvertWorldPos2SampleIndex(EditSliders[0].Percentage.x);
        int trimEndIdx = ConvertWorldPos2SampleIndex(EditSliders[1].Percentage.x);

        int trimedLength = trimEndIdx - trimStartIdx;
        float[] trimedSamples = new float[trimedLength];
        Array.Copy(_targetSamples, trimStartIdx, trimedSamples, 0, trimedLength);

        SetAudioSamples(trimedSamples);
    }

    public void Play()
    {
        var audioSrc = this.GetComponent<AudioSource>();
        
        float length = _targetSamples.Length / 44100;

        var tempAudioClip= AudioClip.Create("RecordedSound", _targetSamples.Length, 1, 44100, false, false);
        tempAudioClip.SetData(_targetSamples, 0);

        CreatedClip = tempAudioClip;

        audioSrc.clip = tempAudioClip;
        audioSrc.Play();
    }

    public void CreateSample()
    {
        //var tempSample = Instantiate(SamplePrefab, null);
        //tempSample.GetComponent<ME_Sample>().Initialize(_targetSamples);
    }

    public void PlayInRange()
    {
        var audioSrc = this.GetComponent<AudioSource>();

        int trimStartIdx = ConvertWorldPos2SampleIndex(EditSliders[0].Percentage.x);
        int trimEndIdx = ConvertWorldPos2SampleIndex(EditSliders[1].Percentage.x);

        int trimedLength = trimEndIdx - trimStartIdx;
        float[] trimedSamples = new float[trimedLength];
        Array.Copy(_targetSamples, trimStartIdx, trimedSamples, 0, trimedLength);

        float length = trimedSamples.Length / 44100;

        var tempAudioClip = AudioClip.Create("RecordedSound", trimedSamples.Length, 1, 44100, false, false);
        tempAudioClip.SetData(trimedSamples, 0);

        audioSrc.clip = tempAudioClip;
        audioSrc.Play();
    }

    int ConvertWorldPos2SampleIndex(float percentage)
    {
        return (int)(_targetSamples.Length * percentage);
    }

    public void StopPlay()
    {
        var audioSrc = this.GetComponent<AudioSource>();
        audioSrc.Stop();
    }

    public static Sprite GetWaveFormTexture(float[] samples, int width, int height, Color bgColor, Color plotColor, int rangeStartIndex, int rangeEndIndex)
    {
        var texture = new Texture2D(width, height);

        // create a 'blank screen' image 
        var blank = new Color[width * height];

        for (int i = 0; i < blank.Length; i++)
            blank[i] = bgColor;

        // clear the texture 
        texture.SetPixels(blank, 0);
        
        // draw the waveform 
        for (int i = 0; i < samples.Length; i++)
        {
            if( rangeStartIndex <= i && i <= rangeEndIndex)
                texture.SetPixel((int)(width * i / samples.Length), (int)(height * (samples[i] + 1f) / 2f), Color.green);
            else
                texture.SetPixel((int)(width * i / samples.Length), (int)(height * (samples[i] + 1f) / 2f), plotColor);
        }
        // upload to the graphics card 
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
}

