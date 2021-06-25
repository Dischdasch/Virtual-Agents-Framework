﻿using UnityEngine;
using VirtualAgentsFramework;

public class WalkToMe : MonoBehaviour
{
    [SerializeField] Agent agent;
    [SerializeField] GameObject object2;
    // Start is called before the first frame update
    void Start()
    {
        //agent.WalkTo(gameObject);
        agent.WalkTo(object2);
        agent.WalkTo(new Vector3(-7,0,-5));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
