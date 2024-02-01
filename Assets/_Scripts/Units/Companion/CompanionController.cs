using System.Collections;
using System.Data.Common;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.U2D;

[RequireComponent(typeof(NavMeshAgent))]

public class CompanionController : MonoBehaviour
{

    // if(idle){
    //      faceOutward()
    // }
    // if(enemyInRange){
    //     Aim()
    //     Shoot()
    //     if(playerFarAway){
    //         stopShootingAndDashToPlayer()
    //     }
    //     else if(playerOutOfRange){
    //         moveWhileShooting()
    //     }
    // }




    // private NavMeshAgent _agent;
    // [SerializeField] private Player _player;
    // [SerializeField] Companion _companion;

    // [Header("Idle Configs")]
    // [SerializeField][Range(0f, 10f)] private float _rotationSpeed = 2f;

    // [Header("Follow Configs")]
    // [SerializeField] private float _followRadius = 2f;

    // private Coroutine MovementCoroutine;
    // private Coroutine StateChangeCoroutine;

    // private void Awake()
    // {
    //     _agent = GetComponent<NavMeshAgent>();
    //     _player.OnStateChange += HandleStateChange;
    // }

    // private void HandleStateChange(PlayerState oldState, PlayerState newState)
    // {
    //     if (StateChangeCoroutine != null)
    //     {
    //         StopCoroutine(StateChangeCoroutine);
    //     }

    //     switch (newState)
    //     {
    //         case PlayerState.Idle:
    //             StateChangeCoroutine = StartCoroutine(HandleIdlePlayer());
    //             break;
    //         case PlayerState.Moving:
    //             HandleMovingPlayer();
    //             break;
    //     }
    // }

    // private IEnumerator HandleIdlePlayer()
    // {
    //     switch (_companion.State)
    //     {
    //         case CompanionState.Follow:
    //             yield return null;
    //             yield return null;
    //             yield return new WaitUntil(() => _companion.State == CompanionState.Idle);
    //             goto case CompanionState.Idle;
    //         case CompanionState.Idle:
    //             if (MovementCoroutine != null)
    //             {
    //                 StopCoroutine(MovementCoroutine);
    //             }
    //             _agent.enabled = false;
    //             MovementCoroutine = StartCoroutine(RotateAroundPlayer());
    //             break;
    //     }
    // }

    // private void HandleMovingPlayer()
    // {
    //     _companion.ChangeState(CompanionState.Follow);
    //     if (MovementCoroutine != null)
    //     {
    //         StopCoroutine(MovementCoroutine);
    //     }

    //     if (!_agent.enabled)
    //     {
    //         _agent.enabled = true;
    //         _agent.Warp(transform.position);
    //     }
    //     MovementCoroutine = StartCoroutine(FollowPlayer());
    // }

    // private IEnumerator RotateAroundPlayer()
    // {
    //     WaitForFixedUpdate wait = new WaitForFixedUpdate();
    //     while (true)
    //     {
    //         Debug.Log(" TEST");
    //         transform.RotateAround(_player.transform.position, Vector3.up, _rotationSpeed);
    //         yield return wait;
    //     }
    // }

    // private IEnumerator FollowPlayer()
    // {
    //     yield return null;

    //     NavMeshAgent playerAgent = _player.GetComponentInChildren<NavMeshAgent>();
    //     Vector3 playerDestination = playerAgent.destination;
    //     Vector3 positionOffset = _followRadius * new Vector3(Mathf.Cos(2 * Mathf.PI * Random.value),
    //                                                          0,
    //                                                          Mathf.Sin(2 * Mathf.PI * Random.value)).normalized;

    //     _agent.SetDestination(playerDestination + positionOffset);

    //     yield return null;
    //     yield return new WaitUntil(() => _agent.remainingDistance <= _agent.stoppingDistance);

    //     _companion.ChangeState(CompanionState.Idle);
    // }
}
