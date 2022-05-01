using UnityEngine;

public class EffectManager : MonoBehaviour
{
    // 싱글톤
    private static EffectManager m_Instance;
    public static EffectManager Instance
    {
        get
        {
            if (m_Instance == null) m_Instance = FindObjectOfType<EffectManager>();
            return m_Instance;
        }
    }

    public enum EffectType
    {
        // 일반 피틴 이펙트 
        Common, 
        // 피가 튀는 이펙트
        Flesh
    }
    
    public ParticleSystem commonHitEffectPrefab;
    public ParticleSystem fleshHitEffectPrefab;
    
    // 매개변수 : pos(이펙트 위치), noraml(이펙트가 바라볼 방향), parent(이펙트에게 할당할 부모)
    public void PlayHitEffect(Vector3 pos, Vector3 normal, Transform parent = null, EffectType effectType = EffectType.Common)
    {
        var targetPrefab = commonHitEffectPrefab;

        if (effectType == EffectType.Flesh)
        {
            targetPrefab = fleshHitEffectPrefab;
        }

        var effect = Instantiate(targetPrefab, pos, Quaternion.LookRotation(normal));

        if (parent != null) effect.transform.SetParent(parent);

        effect.Play();
    }
}