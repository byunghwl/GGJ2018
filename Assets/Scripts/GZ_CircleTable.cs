using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GZ_CircleTable : MonoBehaviour {
    public static GZ_CircleTable Instance;

    public float ArcOffset;
    public float ArcStartRadius;

    public int RowCount;
    public int ColCount;

    public float AngleOffset;

    public GameObject ArcTilePrefab;
    const float TOTALRADIUS = 0.25f;

    private GZ_ArcTile[,] _tileArray;
  
#if UNITY_EDITOR
    Vector3 _debugHittedPos = Vector3.zero;
#endif

    // Use this for initialization
    void Awake() {
        Instance = this;
    }

    public GZ_ArcTile[,] GetTileArray()
    {
        return _tileArray;
    }

    public void SpawnTable(int rowNum, int colNum)
    {
        ClearTable();

        RowCount = rowNum;
        ColCount = colNum;

        AngleOffset = 360f / ColCount;
        _tileArray = new GZ_ArcTile[RowCount, ColCount];

        float totalRadius = TOTALRADIUS;
       
        float arcWidth = (totalRadius - (RowCount * ArcOffset)) / RowCount;

        for (int r = 0; r < RowCount; r++)
        {
            for(int c = 0; c < ColCount; c++)
            {
                var tileObject  = Instantiate(ArcTilePrefab, this.transform);
                var tile = tileObject.GetComponent<GZ_ArcTile>();

                float startRadius = ArcStartRadius + (arcWidth + ArcOffset) * r;
                tile.Initialize(AngleOffset, startRadius, arcWidth, new int[]{r,c});

                _tileArray[r,c] = tile;
            }
        }
    }

    public void ClearTable()
    {
        if (_tileArray == null) return;

        for (int r = 0; r < _tileArray.GetLength(0); r++)
        {
            for (int c = 0; c < _tileArray.GetLength(1); c++)
            {
                DestroyObject(_tileArray[r, c].gameObject);
            }
        }

        _tileArray = null;
    }

    public bool SelectTile(ref GZ_Player player)
    {
#if UNITY_EDITOR
        Debug.DrawLine(Vector3.zero, _debugHittedPos, Color.red);
#endif
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool result = false;

        if (Physics.Raycast(ray, out hit))
        {

#if UNITY_EDITOR
            _debugHittedPos = hit.point;
#endif

            //find proper column
            var tarVec = hit.point - this.transform.position;
            var dirNum = GZ_Math.GetAngleDir(this.transform.up, tarVec, this.transform.forward);
            float angle = Mathf.Acos(Vector2.Dot(tarVec.normalized, this.transform.up)) * Mathf.Rad2Deg;

            if (dirNum == 1)
                angle = 360 - angle;

            int matchColumn = (int)(angle / AngleOffset);

            //find Row
            
            for (int r = 0; r < _tileArray.GetLength(0); r++)
            {
                result = _tileArray[r, matchColumn].CheckDistance(tarVec.magnitude);
                if (result)
                {
                    _tileArray[r, matchColumn].SetInfomation(ref player);
                    return true;
                }
                    
            }
        }
        return result;
    }

   
}
