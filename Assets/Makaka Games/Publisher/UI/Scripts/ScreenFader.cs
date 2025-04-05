/*
================================
Assets for Unity by Makaka Games
================================
 
[Online  Docs -> Updated]: https://makaka.org/unity-assets
[Offline Docs - PDF file]: find it in the package folder.

[Support]: https://makaka.org/support
*/

using UnityEngine;
using UnityEngine.Events;

public class ScreenFader : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [Space]
    [SerializeField]
    private UnityEvent OnInitialized;

    private readonly string animationTriggerNameFadeIn = "FadeIn";
    private readonly string animationTriggerNameFadeInLongStart =
        "FadeInLongStart";

    private void Awake()
    {
        OnInitialized.Invoke();
    }

    public void FadeIn()
    {
        animator.SetTrigger(animationTriggerNameFadeIn);
    }

    public void FadeInLongStart()
    {
        animator.SetTrigger(animationTriggerNameFadeInLongStart);
    }
}
