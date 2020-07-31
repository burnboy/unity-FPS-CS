using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public float hitPoint = 100f;
    private float maxHitPoint;

    public Text hpText;
    public RectTransform hpBar;

    // Start is called before the first frame update
    void Start()
    {
        maxHitPoint = hitPoint;
        hpText.text = hitPoint + "/" + maxHitPoint;
        hpBar.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ApplyDamage(Random.Range(1, 20));
        }

    }

    public void ApplyDamage(float damage)
    {
        UpdateHP();
        hitPoint -= damage;
        if (hitPoint <= 0)
        {
            hitPoint = 0;
            Debug.Log("Die");
        }

    }

    private void UpdateHP()
    {
        hpText.text = hitPoint + "/" + maxHitPoint;
        hpBar.localScale = new Vector3(hitPoint / maxHitPoint, 1, 1);
    }

}
