
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GZ_ArcTile: MonoBehaviour
{
    public Material TMarterial;

    public GameObject ArcObject;
    public SpriteRenderer ArcRenderer;

    public float ArcAngle;
    public float ArcRadius;
    public float OuterArcRadius;

    static float DEFAULTSCALE = 3.14f;

    public int AcquireIndex = -1;

    public void Initialize(float angle, float arcStartRad, float arcWidth, int[] indexs)
    {
        ArcRenderer = ArcObject.GetComponent<SpriteRenderer>();
        ArcRenderer.material = new Material(TMarterial);

        float whiteValue = Random.Range(0.8f, 1f);
        float alphaValue = Random.Range(0.6f, 0.8f);
        ArcRenderer.material.SetColor("_Color", new Color(whiteValue, whiteValue, whiteValue, alphaValue));
        //ArcRenderer.material.SetColor("_Color", new Color(0.8f, 0.8f, 0.8f, 0.5f));

        ArcAngle = angle;
        ArcRenderer.material.SetFloat("_Angle", angle);

        ArcRadius       = arcStartRad;
        OuterArcRadius  = arcStartRad + arcWidth;

        angle = angle * indexs[1] * -1;
        this.transform.localRotation = Quaternion.AngleAxis(angle, this.transform.forward);

        // Give little offset to detach
        this.transform.localPosition = this.transform.localPosition + (this.transform.right + this.transform.up) * 0.001f;
    }

    private void Update()
    {
        //ArcRenderer.material.SetFloat("_Angle", ArcAngle );
        ArcRenderer.material.SetFloat("_InnerRadius", ArcRadius);
        ArcRenderer.material.SetFloat("_OuterRadius", OuterArcRadius);
    }

    public bool CheckDistance(float distance)
    {
        float tt = GZ_CircleTable.Instance.transform.localScale.x / DEFAULTSCALE;
        //Debug.LogFormat("d: {0}, i: {1}, o: {2}", distance, ArcRadius * tt, OuterArcRadius * tt);

        if ( (ArcRadius * tt <= distance  && distance <= OuterArcRadius* tt) )
        {
            if (AcquireIndex != -1) //if alread selected
                return false;
            else
                return true;
        }
        return false;
    }

    public void SetInfomation(ref GZ_Player player)
    {
        GZ_AudioModule.Instance.Play(0);

        AcquireIndex = player.Index;
        player.TileScore++;
        ArcRenderer.material.SetColor("_Color", player.PColor);
        GZ_GameSystem.Instance.OccupiedTileCount++;
    }


}

