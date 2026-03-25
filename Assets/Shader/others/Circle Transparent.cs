using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTransparent : MonoBehaviour
{
    public static int PosID = Shader.PropertyToID("_Position");
    public static int SizeID = Shader.PropertyToID("_Size");

    public Material targetMaterial;
    private Camera Camera;
    public LayerMask Mask;

    void Start()
    {
        Camera = Camera.main;
    }
    void Update()
    {
        var dir = Camera.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, 3000, Mask))
        {
            targetMaterial.SetFloat(SizeID,1);
        }
        else
        {
            targetMaterial.SetFloat(SizeID, 0);
        }
        var view = Camera.WorldToViewportPoint(transform.position);
        targetMaterial.SetVector(PosID, view);

    }
}
