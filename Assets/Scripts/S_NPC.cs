using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class S_NPC : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private Vector3 startPosition;
    private GameObject currentlyCollidingItem;
    public GameObject GetCurrentlyCollidingItem() { return currentlyCollidingItem; }
    private float decreaseWaitTime;
    public void SetDecreaseWaitTime(float t) { decreaseWaitTime = t; }

    [SerializeField] private List<S_Stat> stats; 
    public List<S_Stat> GetStats() { return stats; }

    private void Start()
    {
        startPosition = this.gameObject.transform.position;
        StartCoroutine(DecreaseStats());
    }

    public void ResetAgent()
    {
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        transform.position = startPosition;
        foreach(S_Stat stat in stats)
        {
            stat.currentValue = stat.startingValue;
        }
    }

    IEnumerator DecreaseStats()
    {
        yield return new WaitForSeconds(decreaseWaitTime);
        foreach (S_Stat stat in stats)
        {
            stat.currentValue = stat.currentValue + stat.increaseAmount;
        }
        StartCoroutine(DecreaseStats());
    }

    public void MoveNPC(Vector3 pos, bool isTesting)
    {
        if(isTesting) agent.transform.position = pos;
        else agent.SetDestination(pos);
    }

    // Method called to use a needs representing action, like eating or drinking
    public void UseNeedsAction(bool randomiseLocation, Enum_Desires curTag)
    {
        Debug.Log("Currently executing the action for the stat: " + curTag);
        if (currentlyCollidingItem == null) return;
        if (currentlyCollidingItem?.GetComponent<S_Tasks>().desire == curTag)
        {
            foreach (S_Stat stat in stats)
            {   
                if(stat.id == curTag) stat.currentValue = stat.currentValue > 11 ? stat.currentValue = stat.currentValue - 10 : stat.currentValue = 1;
                if (randomiseLocation) currentlyCollidingItem.GetComponent<S_Tasks>().SetRandomPosition();
            }
        }                
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out S_Tasks task))
        {
            foreach (S_Stat stat in stats)
            {
                if (task.desire == stat.id)
                {
                    currentlyCollidingItem = other.gameObject;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        currentlyCollidingItem = null;
    }
}
