using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
    public AnimationLoader animationLoader;
    public TestAnimationLoader testAnimationLoader;
    public Animator 加载的人物amiator;
    public void Tsestt()
    {
        animationLoader.Creat_anim("A person stands for few seconds and picks up his arms and shakes them.", 100,"荧");
    }
    public void Tsestt2()
    {
        testAnimationLoader.Creat_anim();
    }
}
