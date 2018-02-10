using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GZ_RadioWave : MonoBehaviour {

    public Transform[] ObjectPool;

    public float[] WaveSize;
    public AnimationCurve GrowCurve;
    public AnimationCurve AlphaCurve;

    public float WaveInterval;
    public float EmitInterval;
    public float Speed;

    float _timer = 999;
    float _intervalTimer = 9999;
    int _position = -1;

    private List<Transform> _currentWave = new List<Transform>();

    public void Bang()
    {
        _timer = 0;
        _position = -1;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        _intervalTimer += Time.deltaTime;

        if (_intervalTimer > WaveInterval)
        {
            _intervalTimer = 0;
                
            if (++_position >= ObjectPool.Length )
            {
                if (_currentWave.Count == 0)
                {
                    _currentWave.Clear();
                    Destroy(this.gameObject);
                }
                return;
            }

            ObjectPool[_position].gameObject.SetActive(true);
            ObjectPool[_position].localScale = Vector3.one * WaveSize[0];
            _currentWave.Add(ObjectPool[_position]);
        }

        for( int w = _currentWave.Count -1; w >= 0; w--)
        {
            _currentWave[w].localScale += Vector3.one * Speed * Time.deltaTime;

            if (_currentWave[w].localScale.x > WaveSize[1])
            {
                _currentWave[w].gameObject.SetActive(false);
                _currentWave.RemoveAt(w);
            }
        }        
    }



}
