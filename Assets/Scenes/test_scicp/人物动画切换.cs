using UnityEngine;

public class 人物动画切换 : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // 获取Animator组件
        animator = GetComponent<Animator>();
    }
    public void 切换动画()
    {
        animator.enabled = true;
    }

}
