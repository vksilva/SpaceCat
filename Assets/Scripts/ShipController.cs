using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float speed;
    
    public void Move(Vector3 direction)
    {
        transform.transform.position += direction * speed * Time.deltaTime;
    }
}
