using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MUCOProceduralRoom : MonoBehaviour
{
    private void Awake()
    {
        Mesh mesh = new Mesh();
        mesh.name = "MUCOProceduralRoom";

        List<Vector3> points = new List<Vector3>() { 
            new Vector3(-0.5f, 0.5f),
            new Vector3(0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f),
            new Vector3(0.5f, -0.5f),
        };
    }
}
