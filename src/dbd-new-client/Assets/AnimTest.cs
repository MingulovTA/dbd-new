using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class AnimTest : MonoBehaviour
{
    [SerializeField] private Animation _animation;
    [SerializeField] private List<AnimationClip> _clips;
    private int _animIndex;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animIndex++;
            if (_animIndex >= _clips.Count)
                _animIndex = 0;
            _animation.CrossFade(_clips[_animIndex].name);
        }
    }
}
