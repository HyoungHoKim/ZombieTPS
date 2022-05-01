using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{   
    // 화면상 크로스헤어를 나타내는 이미지
    public Image aimPointReticle;
    // 실제로 맞게되는 크로스헤어를 나타내는 이미지
    public Image hitPointReticle;

    // 실제로 맞게 되는 크로스 헤어는 화면상에서 임의적으로 smooth하게 움직이게 할건데 그 때의 지연시간
    public float smoothTime = 0.2f;
    
    // 크로스헤어를 알맞는 위치에 그리려면 실제로 맞게 되는 월드 좌표계 위치 카메라 화면상에서 어디 위치하는지 알아야한다. 
    private Camera screenCamera;
    // hitPointReticle 이미지의 RectTransform
    private RectTransform crossHairRectTransform;

    // smoothing에 사용할 값의 변화량
    private Vector2 currentHitPointVelocity;
    // 월드 좌표계의 위치를 화면상 위치로 변환한 좌표가 될 것
    private Vector2 targetPoint;

    private void Awake()
    {
        screenCamera = Camera.main;
        crossHairRectTransform = hitPointReticle.GetComponent<RectTransform>();
    }

    public void SetActiveCrosshair(bool active)
    {
        hitPointReticle.enabled = active;
        aimPointReticle.enabled = active;
    }

    // 월드 좌표계 위치를 입력으로 받아서 화면상 좌표계로 변환하여 targetPoint에 대입
    public void UpdatePosition(Vector3 worldPoint)
    {
        targetPoint = screenCamera.WorldToScreenPoint(worldPoint);
    }

    // 매 프레임마다 hitPointRecticle 이미지와 위치를 실제로 총이 맞는 위치에 그려주는 역할
    private void Update()
    {
        // 비활성화 시 return 
        if (!hitPointReticle.enabled) return;
        // 기존의 크로스 헤어 위치를 targetPoint로 스무스하게 갱신
        crossHairRectTransform.position = Vector2.SmoothDamp(crossHairRectTransform.position, targetPoint,
            ref currentHitPointVelocity, smoothTime * Time.deltaTime);
    }
}