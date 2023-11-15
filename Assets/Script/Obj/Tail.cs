using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TailType
{
    Horizontal,
    Vertical,
}
public class Tail : MonoBehaviour
{
    [SerializeField]
    private TailType tailType;

    public TailType GetTailType()
    {
        return tailType;
    }

    public void SetTailType(TailType type)
    {
        tailType = type;
    }
}
