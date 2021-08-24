using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MUCOPlayer
{
    public int ID = 0;

    // TODO: This should be replaced by the unity transform component
    public Vector3 Position = Vector3.zero;
    public Quaternion Rotation = Quaternion.identity;

    public MUCOPlayer(int id, Vector3 spawnPosition)
    {
        ID = id;
        Position = spawnPosition;
    }
}
