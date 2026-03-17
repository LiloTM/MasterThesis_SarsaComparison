using UnityEngine;

public class S_Tasks : MonoBehaviour
{
    public Enum_Desires desire;
    public void SetRandomPosition()
    {
        transform.position = new Vector3(Random.Range(1, 6), this.transform.position.y, Random.Range(1, 6));
    }
}
