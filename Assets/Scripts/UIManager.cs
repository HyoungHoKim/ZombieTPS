using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // 싱글톤
    private static UIManager instance;
    
    // 이 프로퍼티를 통해서 UIManager의 싱글톤 private 인스턴스를 리턴 받음    
    public static UIManager Instance
    {
        get
        {
            // 인스턴스가 이미 존재할 땐 받지 않는다. 즉, null 일때만 받음
            if (instance == null) instance = FindObjectOfType<UIManager>();

            return instance;
        }
    }

    // UHD Canvas의 구성 UI 요소들
    [SerializeField] private GameObject gameoverUI;
    [SerializeField] private Crosshair crosshair;

    [SerializeField] private Text healthText;
    [SerializeField] private Text lifeText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text waveText;

    // 'ammoText' 남은 탄창 UI 갱신
    public void UpdateAmmoText(int magAmmo, int remainAmmo)
    {
        ammoText.text = magAmmo + "/" + remainAmmo;
    }
    // 'scoreText' 점수 UI 갱신
    public void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score : " + newScore;
    }
    // 'lifeText' 남은 생명 수 UI 갱신
    public void UpdateWaveText(int waves, int count)
    {
        waveText.text = "Wave : " + waves + "\nEnemy Left : " + count;
    }
    // 게임 오버시 'GameOver' UI 활성화
    public void UpdateLifeText(int count)
    {
        lifeText.text = "Life : " + count;
    }
    // 해당 위치에 크로스 헤어 UI를 표시
    public void UpdateCrossHairPosition(Vector3 worldPosition)
    {
        crosshair.UpdatePosition(worldPosition);
    }
    // 해당 위치에 크로스 헤어 UI를 표시
    public void UpdateHealthText(float health)
    {
        healthText.text = Mathf.Floor(health).ToString();
    }
    // 크로스 헤어 UI 활성화
    public void SetActiveCrosshair(bool active)
    {
        crosshair.SetActiveCrosshair(active);
    }
    // 해당 위치에 크로스헤어 UI를 표시 
    public void SetActiveGameoverUI(bool active)
    {
        gameoverUI.SetActive(active);
    }
    // 게임 Over 상태에서 Restart 버튼을 눌렀을 때 실행시킬 함수. 게임 재시작
    public void GameRestart()
    {
        // 현재 씬의 이름을 넘겨 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}