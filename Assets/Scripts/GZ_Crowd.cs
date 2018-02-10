using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GZ_Crowd : MonoBehaviour {
    public static GZ_Crowd Instance;

    public GameObject[] Backgrounds;
    public GameObject[] HumanModels;
    

    public Transform[] Indicators;
    public Material[] HamanMaterials;

    public float PlaceAngleInterval;
    public float PlaceRadiusInterval;

    private List<GameObject> _crowds = new List<GameObject>();
    private Renderer[] _crowdRenderers;

    private float[] _timers;
    private float[] _nodAngles;
    private float[] _nodePeriod;

	void Start () {
        Instance = this;
        //CreateCrowd();
    }

    public void DestroyCrowd()
    {
        if (_crowds != null)
        {
            for (int i = 0; i < _crowds.Count; i++)
            {
                Destroy(_crowds[i].gameObject);
            }
        }

        _crowds = null;
    }

    public void CreateCrowd()
    {

        DestroyCrowd();

        _crowds = new List<GameObject>();

        int bgRand = Random.Range(0, Backgrounds.Length);
        for(int b = 0; b < Backgrounds.Length; b++)
        {
            if (b == bgRand)
                Backgrounds[b].SetActive(true);
            else
                Backgrounds[b].SetActive(false);
        }

        for (int i = 0; i < Indicators.Length; i++)
            Indicators[i].gameObject.SetActive(false);

        PlaceCrowd();

        _crowdRenderers = new Renderer[_crowds.Count];
        _timers = new float[_crowds.Count];
        _nodAngles = new float[_crowds.Count];
        _nodePeriod = new float[_crowds.Count];
    }


    private void Update()
    {
        if (_crowds == null)
            return;

        for (int i =0; i < _crowds.Count; i++)
        {
            if (_timers[i] == 0)
            {
                _timers[i] = Random.Range(0f, 0.1f);
                _crowdRenderers[i] = _crowds[i].GetComponent<Renderer>();
            }
               
            if (_nodePeriod[i] == 0)
                _nodePeriod[i] = Random.Range(0.1f, 0.5f);

            if (_nodAngles[i] == 0)
                _nodAngles[i] = Random.Range(5f, 10f);

            _timers[i] += Time.deltaTime;
            float phase = Mathf.Sin(_timers[i] / _nodePeriod[i]);

            Quaternion shakeRot = Quaternion.Euler(new Vector3(0, 0, phase * _nodAngles[i]));
            _crowds[i].transform.rotation = Quaternion.LookRotation(this.transform.forward) * shakeRot;
        }
    }

    public void ApplyColor()
    {
        //Debug.Log(_crowds.Count + "," + GZ_GameSystem.Instance.OccupiedTileCount + "," + GZ_GameSystem.Instance.TotalTileCount);

        int appliedNumber = (int)( ((float)GZ_GameSystem.Instance.OccupiedTileCount / (float)GZ_GameSystem.Instance.TotalTileCount) * (float)_crowds.Count);
        
        int redCIndex = -1;
        int blueCIndex = -1;

        if (GZ_GameSystem.TotalScores[0] == 0 || GZ_GameSystem.TotalScores[1] == 0)
            return;

        float redPrecentage = (float)GZ_GameSystem.TotalScores[0]/ (float)(GZ_GameSystem.TotalScores[0] + GZ_GameSystem.TotalScores[1]);
        GZ_RecordModule.Instance.SetVolume(1, redPrecentage);
        int redNumber = (int)(appliedNumber * redPrecentage);
        int blueNumber = (int)(appliedNumber * (1 - redPrecentage));

        GZ_GameSystem.Instance.SetScoreBoard(0, redNumber);
        GZ_GameSystem.Instance.SetScoreBoard(1, blueNumber);

        //Debug.LogFormat("a:{0}, r: {1} b: {2}, t: {3}", appliedNumber, redNumber, blueNumber, _crowds.Count);
        int count = _crowds.Count;

        for (int i = 0; i < _crowds.Count ; i += 2)
        {
            if (redCIndex >= redNumber && blueCIndex >= blueNumber)
            {
                break;
            }
                
            if (++redCIndex < redNumber)
            {
                _crowdRenderers[i].material = HamanMaterials[1];
            }

            if (++blueCIndex < blueNumber)
            {
                _crowdRenderers[i + 1].material = HamanMaterials[2];
            }
            
        }
    }

    IEnumerator SpawnCrowd(float spawnTime)
    {
        float timer = 0;

        while (timer < spawnTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }

    void PlaceCrowd()
    {
        _crowds.Clear();

        Vector3 indiPos = Indicators[1].localPosition;
        indiPos.y = 0;
        Vector3 dir = indiPos - Vector3.zero;

        float radius = dir.magnitude;

        for(float r= 0.3f; r < radius; r+= PlaceRadiusInterval)
        {
            for (int a = 0; a < 180 / PlaceAngleInterval ; a++)
            {
                var direction = dir.normalized * (r + Random.Range(0, 0.05f));
                SpawnModel(a * PlaceAngleInterval, direction);
                SpawnModel(a * PlaceAngleInterval * -1, direction);
            }
        }
    }

    void SpawnModel(float angle, Vector3 direction)
    {
        Vector3 position = Quaternion.AngleAxis(angle, this.transform.up) * direction;
        var model = Instantiate(HumanModels[Random.Range(0, HumanModels.Length)], this.transform);
        position.y = Indicators[1].localPosition.y;
        model.transform.localPosition = position;
        _crowds.Add(model);
    }
}
