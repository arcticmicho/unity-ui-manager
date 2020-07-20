using System;
using System.Collections;
using System.Collections.Generic;
using PartySystems.UIParty.Utils;
using UnityEngine;


namespace PartySystems.UIParty
{
    public class LegacyAnimationUITransition : BaseUITransitionAnimation
    {
        [SerializeField]
        private Animation m_animation;

        [SerializeField]
        private AnimationClip m_introAnim;

        [SerializeField]
        private AnimationClip m_closeAnim;

        private LegacyAnimationSync m_animationSync;


        private void Awake()
        {
            m_animation.AddClip(m_introAnim, m_introAnim.name);
            m_animation.AddClip(m_closeAnim, m_closeAnim.name);
            m_animationSync = new LegacyAnimationSync(m_animation);
        }

        protected override void PlayCloseAnimation_Local(Action<bool> onFinishCloseAnimation)
        {
            if (!m_animationSync.PlayAnim(m_closeAnim, onFinishCloseAnimation))
            {
                onFinishCloseAnimation?.Invoke(true);
            }
        }

        protected override void PlayIntroAnimation_Local(Action<bool> onFinishIntroAnimation)
        {
            if (!m_animationSync.PlayAnim(m_introAnim, onFinishIntroAnimation))
            {
                onFinishIntroAnimation?.Invoke(true);
            }
        }

        private void Update()
        {
            m_animationSync.Update(Time.deltaTime);
        }
    }

}


