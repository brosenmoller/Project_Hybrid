using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineBrain))]
public class CameraSnapFix : MonoBehaviour
{
    [Header("BlendSettings")]
    [SerializeField] private CinemachineBlendDefinition.Style defaultBlend;
    [SerializeField] private float blendDuration;

    [Header("Fix Settings")]
    [SerializeField] private float blendActivationDelay;

    private CinemachineBrain cinemachineBrain;

    private void Awake()
    {
        cinemachineBrain = GetComponent<CinemachineBrain>();

        cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
        Invoke(nameof(ActivateBlend), blendActivationDelay);
    }

    private void ActivateBlend()
    {
        cinemachineBrain.m_DefaultBlend = new CinemachineBlendDefinition(defaultBlend, blendDuration);
    }
}
