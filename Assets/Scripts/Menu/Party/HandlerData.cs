using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Handler Data", menuName = "Critter Creation/Handler Data")]
public class HandlerData : CritterData
{
    [Header("Handler Data")]
    [SerializeField] private Vector3 _location;

    public Vector3 HandlerLocation => _location;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
