using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ME_UISlider : MonoBehaviour {

    public SpriteRenderer ConstraintSprite; 
    public Vector3 Position;
    public Vector2 Percentage = Vector2.zero;
    public bool IsDragging = false;

    private void OnMouseUp()
    {
        IsDragging = false;
    }

    void OnMouseDrag()
    {
        IsDragging = true;
        SetPosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9f)));
    }

    public void SetPosition(Vector3 pos)
    {
        Position = pos;
       
        if (ConstraintSprite != null) 
            ApplyConstrainPosition(ref Position);
       
        this.transform.position = Position;
    }

    void ApplyConstrainPosition(ref Vector3 worldPosition)
    {
        worldPosition.x = Mathf.Clamp(worldPosition.x, ConstraintSprite.bounds.min.x, ConstraintSprite.bounds.max.x);
        Percentage.x = (worldPosition.x - ConstraintSprite.bounds.min.x) / (ConstraintSprite.bounds.max.x - ConstraintSprite.bounds.min.x);

        worldPosition.y = Mathf.Clamp(worldPosition.y, ConstraintSprite.bounds.min.y, ConstraintSprite.bounds.max.y);
        Percentage.y = (worldPosition.y - ConstraintSprite.bounds.min.y) / (ConstraintSprite.bounds.max.y - ConstraintSprite.bounds.min.y);
    } 
}
