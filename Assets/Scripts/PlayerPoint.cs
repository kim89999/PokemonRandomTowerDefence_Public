using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPoint : MonoBehaviour
{
    [SerializeField]
    private int currentPoint = 100;

    public int CurrentPoint
    {
        set => currentPoint = Mathf.Max(0, value);
        get => currentPoint;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
