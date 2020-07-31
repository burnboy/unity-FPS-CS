using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float hitPoint = 100f;

    public void ApplyDamage(float damage)
    {
        hitPoint -= damage;
        if (hitPoint <= 0)
        {
            Destroy(gameObject);
        }
    }
}
