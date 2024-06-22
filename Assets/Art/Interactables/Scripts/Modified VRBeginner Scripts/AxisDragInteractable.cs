using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MikeNspired.UnityXRHandPoser
{
    /// <summary>
    /// 축을 따라 드래그할 수 있는 사용자 지정 상호작용 가능한 클래스입니다. 연속적으로 또는 정수 단계로 스냅할 수 있습니다.
    /// </summary>
    public class AxisDragInteractable : XRBaseInteractable
    {
        [Tooltip("이동시킬 Rigidbody입니다. null이면 해당 개체나 해당 자식에서 하나를 찾아보려고 합니다.")]
        public Rigidbody MovingRigidbody;

        public Vector3 LocalAxis; // 로컬 축
        public float AxisLength; // 축 길이

        [Tooltip("0이면 부동 소수점 [0,1] 범위 슬라이더이며, 그렇지 않으면 정수 슬라이더입니다.")]
        public int Steps = 0;

        public bool SnapOnlyOnRelease = true; // 릴리스 시에만 스냅하는지 여부

        public bool ReturnOnFree; // 비어있을 때 돌아가는지 여부
        public float ReturnSpeed; // 돌아가는 속도

        public AudioClip SnapAudioClip; // 스냅 사운드 클립
        public AudioSource AudioSource; // 오디오 소스
        public UnityEventFloat OnDragDistance; // 드래그 거리 이벤트
        public UnityEventInt OnDragStep; // 드래그 스텝 이벤트

        private Vector3 m_EndPoint; // 끝 지점
        private Vector3 m_StartPoint; // 시작 지점
        private Vector3 m_GrabbedOffset; // 잡은 오프셋
        private float m_CurrentDistance; // 현재 거리
        private int m_CurrentStep; // 현재 스텝
        private XRBaseInteractor m_GrabbingInteractor; // 잡고 있는 상호작용기

        private float m_StepLength; // 스텝 길이

        // Start is called before the first frame update
        void Start()
        {
            LocalAxis.Normalize();

            // 길이가 음수일 수 없으므로 음수 길이를 수정합니다.
            if (AxisLength < 0)
            {
                LocalAxis *= -1;
                AxisLength *= -1;
            }

            // 스텝이 0이면 스텝 길이는 0입니다.
            if (Steps == 0)
            {
                m_StepLength = 0.0f;
            }
            else
            {
                m_StepLength = AxisLength / Steps;
            }

            m_StartPoint = transform.position;
            m_EndPoint = transform.position + transform.TransformDirection(LocalAxis) * AxisLength;

            if (MovingRigidbody == null)
            {
                MovingRigidbody = GetComponentInChildren<Rigidbody>();
            }

            m_CurrentStep = 0;
            AudioSource.clip = SnapAudioClip;
        }

        private void OnValidate()
        {
            if (!AudioSource)
                AudioSource = GetComponent<AudioSource>();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (isSelected)
            {
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
                {
                    Vector3 WorldAxis = transform.TransformDirection(LocalAxis);

                    Vector3 distance = m_GrabbingInteractor.transform.position - transform.position - m_GrabbedOffset;
                    float projected = Vector3.Dot(distance, WorldAxis);

                    // 스텝이 있고 릴리스 시에만 스냅하는 경우 스텝을 조정합니다.
                    if (Steps != 0 && !SnapOnlyOnRelease)
                    {
                        int steps = Mathf.RoundToInt(projected / m_StepLength);
                        projected = steps * m_StepLength;
                    }

                    Vector3 targetPoint;
                    if (projected > 0)
                        targetPoint = Vector3.MoveTowards(transform.position, m_EndPoint, projected);
                    else
                        targetPoint = Vector3.MoveTowards(transform.position, m_StartPoint, -projected);

                    if (Steps > 0)
                    {
                        int posStep = Mathf.RoundToInt((targetPoint - m_StartPoint).magnitude / m_StepLength);
                        if (posStep != m_CurrentStep)
                        {
                            AudioSource.Play();
                            OnDragStep.Invoke(posStep);
                        }

                        m_CurrentStep = posStep;
                    }

                    OnDragDistance.Invoke((targetPoint - m_StartPoint).magnitude);

                    Vector3 move = targetPoint - transform.position;

                    if (MovingRigidbody != null)
                        MovingRigidbody.MovePosition(MovingRigidbody.position + move);
                    else
                        transform.position = transform.position + move;
                }
            }
        }

        protected override void OnSelectEntered(XRBaseInteractor interactor)
        {
            m_GrabbedOffset = interactor.transform.position - transform.position;
            m_GrabbingInteractor = interactor;
            base.OnSelectEntered(interactor);
        }

        protected override void OnSelectExited(XRBaseInteractor interactor)
        {
            base.OnSelectExited(interactor);

            if (SnapOnlyOnRelease && Steps != 0)
            {
                // 거리를 계산하여 스텝에 맞춰 위치를 조정합니다.
                float dist = (transform.position - m_StartPoint).magnitude;
                int step = Mathf.RoundToInt(dist / m_StepLength);
                dist = step * m_StepLength;

                transform.position = m_StartPoint + transform.TransformDirection(LocalAxis) * dist;

                // 스텝이 변경되었을 때 이벤트를 호출합니다.
                if (step != m_CurrentStep)
                {
                    OnDragStep.Invoke(step);
                }
            }
        }

        // 선택된 상호작용기가 없을 때, Gizmos를 그려줍니다.
        void OnDrawGizmosSelected()
        {
            // 축 끝점을 계산합니다.
            Vector3 end = transform.position + transform.TransformDirection(LocalAxis.normalized) * AxisLength;

            // 축과 끝점 사이에 선을 그리고, 끝점을 표시합니다.
            Gizmos.DrawLine(transform.position, end);
            Gizmos.DrawSphere(end, 0.01f);
        }
    }
}