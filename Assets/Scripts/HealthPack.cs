using UnityEngine;

public class HealthPack : MonoBehaviour, IItem
{
    // 체력 회복 수치
    public float health = 50;

    public void Use(GameObject target)
    {
        // 전달받은 게임 오브젝트로부터 LivingEntity 컴포넌트 가져오기 시도
        var life = target.GetComponent<LivingEntity>();

        // LivingEntity 컴포넌트가 있다면
        if (life != null)
        {
            life.RestoreHealth(health);
        }

        Destroy(gameObject);

    }
}