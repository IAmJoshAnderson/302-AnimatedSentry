using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    CharacterController pawn;
    NavMeshAgent agent;

    Transform navTarget;

    void Start()
    {
        pawn = GetComponent<CharacterController>(); // now the character controller and navmeshagent are being called in
        agent = GetComponent<NavMeshAgent>();

        PlayerTargeting player = FindObjectOfType<PlayerTargeting>();

        navTarget = player.transform;

        if (player) agent.destination = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (navTarget) agent.destination = navTarget.position;

        //pawn.SimpleMove(Vector3.zero);
    }
}
