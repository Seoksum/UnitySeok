using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintEffector : MonoBehaviour
{
    [Range(0.0f,1.0f)]
    public float weight =0.5f;

    private void Update()
    {
        var material = GetComponent<Renderer>().material;
        Color color = Color.magenta;
        material.color = SampleUtility.FadeEffectorColorByWeight(color, weight);
    }
}
