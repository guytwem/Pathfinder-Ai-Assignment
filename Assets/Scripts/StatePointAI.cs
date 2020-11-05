using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePointAI : MonoBehaviour
{
    [Header("Ai Stats")]
    [SerializeField, Tooltip("Speed of the Ai")]
    private float speed = 5.0f;
    [SerializeField, Tooltip("Ai Chase Distance")]
    private float chasePlayerDistance = 5f;
    [SerializeField, Tooltip("Distance the AI flees from the player")]
    private float fleeDistance = 10f;
    [SerializeField, Tooltip("Minimum Distance to Waypoint")]
    private float minDistanceToWaypoint;
    [SerializeField, Tooltip("Minimum distance for the AI to attack")]
    private float minAttackDistance = 1f;

    [Header("Game Obects")]
    [SerializeField, Tooltip("Ai GameObject")]
    private GameObject AiSprite;
    [SerializeField, Tooltip("Player GameObject")]
    private GameObject player;
    [SerializeField, Tooltip("Player controller Script")]
    public PlayerController playerController;

    [Header("Waypoints")]
    [SerializeField]
    private GameObject[] waypointarray;
    private int currentWaypoint = 0;
    

    [Header("Player Stats")]
    [SerializeField, Tooltip("The health at which the AI will Flee")]
    private float lowHealth = 25f;
    [SerializeField, Tooltip("Max player health")]
    private float maxHealth = 100f;
    [SerializeField, Tooltip("Health when the player dies")]
    private float minHealth = 0f;
    [SerializeField, Tooltip("Current Health")]
    private float currentHealth = 100f;
    

    /// <summary>
    /// On start go to next state
    /// </summary>
    public void Start()
    {
        Debug.Log("start");
        NextState();
    }

    /// <summary>
    /// The four states of the state machine
    /// </summary>
    public enum State
    {
        Patrol,
        Chase,
        Attacking,
        Flee,
       
    }

    public State state;

   
    /// <summary>
    /// patrol in patrol state
    /// if the distance between ai and player is less than chase distance
    /// swap to chase state
    /// </summary>
    /// <returns></returns>
    private IEnumerator PatrolState()
    {
        Debug.Log("Patrolling");
        while (state == State.Patrol)
        {
            Patrol();
            yield return null;
            if (Vector2.Distance(player.transform.position, AiSprite.transform.position) < chasePlayerDistance)
            {
                state = State.Chase;
            }
        }
        Debug.Log("No longer Patrolling");
        NextState();
    }

    /// <summary>
    /// While in Chase state, Chase
    /// if the distance between player and ai is less than attack distance
    /// change to attack state
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChaseState()
    {
        Debug.Log("Chasing");
        while (state == State.Chase)
        {
            Chase();
            yield return null;
            if (Vector2.Distance(player.transform.position, AiSprite.transform.position) < minAttackDistance)
            {
                state = State.Attacking;
            }

        }
        Debug.Log("Stop Chasing");
        NextState();
    }

    /// <summary>
    /// Attack in attack state
    /// if health reaches low health change to flee state
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackingState()
    {
        Debug.Log("attacking");
        while (state == State.Attacking)
        {
            Attacked();
            yield return null;
            if (currentHealth == lowHealth)
            {
                state = State.Flee;
            }
        }
        Debug.Log("Stop Attacking");
        NextState();
    }

    /// <summary>
    /// While in flee state flee
    /// if the ai flees from the player for the flee distance 
    /// change to patrol state
    /// </summary>
    /// <returns></returns>
    private IEnumerator FleeState()
    {
        Debug.Log("Fleeing");
        while (state == State.Flee)
        {
            Flee();
            yield return null;
            if (Vector2.Distance(player.transform.position, AiSprite.transform.position) > fleeDistance)
            {
                state = State.Patrol;
            }
        }
        Debug.Log("Stop Fleeing");
        NextState();
    }

    /// <summary>
    /// if the ai distance to the next waypoint is less than minDistanceToWaypoint
    /// go to the next waypoint
    /// if ai reaches last waypoint go back to first waypoint
    /// </summary>
    private void Patrol()
    {
        float distance = Vector2.Distance(AiSprite.transform.position, waypointarray[currentWaypoint].transform.position);
        //Are we at the target location
        if (distance < minDistanceToWaypoint)
        {
            currentWaypoint++;
        }

        //if we reached the last waypoint, start again
        if (currentWaypoint >= waypointarray.Length)
        {
            currentWaypoint = 0;
        }


        MoveAi(waypointarray[currentWaypoint].transform.position);

    }


    /// <summary>
    /// if distance between player and ai is less than chase distance
    /// </summary>
    private void Chase()
    {
        if (Vector2.Distance(player.transform.position, AiSprite.transform.position) < chasePlayerDistance)
        {
            MoveAi(player.transform.position);
        }
        else
        {
            Patrol();
        }
    }

    /// <summary>
    /// if player health reaches 25% move the ai away from player
    /// otherwise keep attacking
    /// </summary>
    private void Flee()
    {
        if (currentHealth == lowHealth)
        {
            MoveAi(AiSprite.transform.position - player.transform.position);
        }
        else
        {
            Attacked();
        }
    }

    
    /// <summary>
    /// move ai towards target position at speed * time.deltatime
    /// </summary>
    /// <param name="targetPosition"></param>
    private void MoveAi(Vector2 targetPosition)
    {
        //move ai
        AiSprite.transform.position = Vector2.MoveTowards(AiSprite.transform.position,
                                                        targetPosition,
                                                            speed * Time.deltaTime);
    }

    /// <summary>
    /// Changes to next state
    /// </summary>
    void NextState()
    {
        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info =
            GetType().GetMethod(methodName,
                                   System.Reflection.BindingFlags.NonPublic |
                                   System.Reflection.BindingFlags.Instance);

        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }

    /// <summary>
    /// if ai is close to player attack player
    /// player loses health 
    /// if player loses all health player dies
    /// </summary>
    public void Attacked()
    {
        if (Vector2.Distance(AiSprite.transform.position, player.transform.position) < minAttackDistance)
        {
            currentHealth--;

        }

        if (currentHealth == minHealth)
        {
            

            currentHealth = maxHealth;

        }
        
    }



}
