using System;
using UnityEngine;

// 플레이어 + 적 AI 등등 모든 '생명체'인 오브젝트들이 공유하는 기능들을 정의한 기반 클래스
// 데미지를 입을 수 있으므로 IDamageable을 상속
public class LivingEntity : MonoBehaviour, IDamageable
{
    // 기본 시작 체력
    public float startingHealth = 100f;
    // 현재 체력
    public float health { get; protected set; }
    // 사망 여부
    public bool dead { get; protected set; }
    
    // 생명체가 사망하면 실행할 처리들
    public event Action OnDeath;
    
    // 공격과 공격 사이의 최소 대기시간
    private const float minTimeBetDamaged = 0.1f;
    // 최근에 공격을 당한 시점
    private float lastDamagedTime;

    // 무적, 공격을 받을 수 있는 상태인지 여부
    // true면 무적, false면 공격을 받을 수 있는 상태 
    protected bool IsInvulnerabe
    {
        get
        {
            // lastDamagedTime + minTimeBetDamaged 시간 내에 공격을 당한 경우에는 무시할 것이다. 
            if (Time.time >= lastDamagedTime + minTimeBetDamaged) return false;

            return true;
        }
    }
    
    protected virtual void OnEnable()
    {
        dead = false;
        health = startingHealth;
    }

    // IDamageable의 자식으로서 반드시 구현해야하는 함수
    public virtual bool ApplyDamage(DamageMessage damageMessage)
    {
        // 무적상태, 데미지 주는게 나 자신, 이미 죽은 상태에서는 false
        if (IsInvulnerabe || damageMessage.damager == gameObject || dead) return false;

        lastDamagedTime = Time.time;
        health -= damageMessage.amount;
        
        if (health <= 0) Die();

        return true;
    }
    
    // 체력 회복
    public virtual void RestoreHealth(float newHealth)
    {
        if (dead) return;
        
        health += newHealth;
    }
    
    public virtual void Die()
    {
        // 즉, OnDeath 이벤트에 최소 하나 이상의 함수가 등록되어 있다면 OnDeath()함수 실행
        if (OnDeath != null) OnDeath();
        
        dead = true;
    }
}