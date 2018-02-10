using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ME_MicModule : MonoBehaviour {
    public static ME_MicModule Instance;

    private AudioClip _recordClip;
    
    private float _timeSinceRecordStarted;

    private Action<float[]> _recordDoneCallback;

    // Use this for initialization
    void Start () {
        Instance = this;
        InitializeMicrophone();
    }
	
	void InitializeMicrophone()
    {
        if (Microphone.devices.Length <= 0)
            Debug.LogErrorFormat("There is no Mic");
    }

    public bool IsRecording()
    {
        return Microphone.IsRecording(null);
    }

    public void Record(Action<float[]> recordDoneCallback = null)
    {
        if(!Microphone.IsRecording(null))
        {
            _recordDoneCallback = recordDoneCallback;

            Debug.Log("STart recording");
            _recordClip = Microphone.Start(null, true, 10, 44100);
            _timeSinceRecordStarted = Time.time;
        }
    }

    public void StopRecord()
    {
        if (Microphone.IsRecording(null))
        {
            Debug.Log("Stop recording");
            Microphone.End(null);

            //Trim
            _timeSinceRecordStarted = Time.time - _timeSinceRecordStarted;

            Debug.LogFormat("Recorded seconds: {0}", _timeSinceRecordStarted);

            float lengthL = _recordClip.length;
            float samplesL = _recordClip.samples;
            float samplesPerSec = (float)samplesL / lengthL;
            float[] samples = new float[(int)(samplesPerSec * _timeSinceRecordStarted)];
            _recordClip.GetData(samples, 0);

            /*
            //create to new trimmed audio
            _recordClip = AudioClip.Create("RecordedSound", (int)(_timeSinceRecordStarted * samplesPerSec), 1, 44100, false, false);
            _recordClip.SetData(samples, 0);
            */

            if (_recordDoneCallback != null)
                _recordDoneCallback(samples);

            ME_AudioEditModule.Instance.SetEnabled(true);
            ME_AudioEditModule.Instance.SetAudioSamples(samples);
        }
    }

    
}
