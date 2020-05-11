using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{

    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color dotHighlightColor;
    Color originalDotColor;
    // Update is called once per frame

     void Start()
    {
        originalDotColor = dot.color;
    }
    void Update()
    {
        transform.Rotate(Vector3.up * 40 * Time.deltaTime);
    }

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlightColor;
        }
        else
        {
            dot.color = originalDotColor;
        }
    }

    public void ActivateCrosshair(bool c)
    {
        this.gameObject.SetActive(c);
    }
}
