using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeEffect : MonoBehaviour
{
    public float Duration
    {
        get
        {
            AnimationClip[] animations = GetComponent<Animator>().runtimeAnimatorController.animationClips;

            foreach (var animation in animations)
                if (animation.name == "Explode")
                    return animation.length;

            return 0;
        }
    }

    public void DestroyAfterPlay()
    {
        Destroy(gameObject);
    }
}
