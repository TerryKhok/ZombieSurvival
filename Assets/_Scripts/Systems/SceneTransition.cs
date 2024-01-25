using UnityEngine;


public class SceneTransition : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.SetTrigger("startSceneTransitionStart");
    }

    public void StartSceneTransitionMain()
    {
        _animator.SetTrigger("startSceneTransitionEndMain");
    }

    public void StartSceneTransitionResult()
    {
        _animator.SetTrigger("startSceneTransitionEndResult");
    }
}
