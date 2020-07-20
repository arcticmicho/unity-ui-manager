using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PartySystems.UIParty.Utils
{
    public class LegacyAnimationSync
    {
        private class AnimationSyncProcess
        {
            private float m_animationTime;

            private float m_elapsedTime;

            private Action<bool> m_animCallback;

            private AnimationClip m_clip;

            private bool m_finished;

            public AnimationSyncProcess(AnimationClip clip, Action<bool> callback)
            {
                m_clip = clip;
                m_elapsedTime = 0f;
                m_animationTime = clip.averageDuration;
                m_finished = false;
                m_animCallback = callback;
            }

            public bool Step(float deltaTime)
            {
                if (!m_finished && m_elapsedTime < m_animationTime)
                {
                    m_elapsedTime += deltaTime;
                    return false;
                }
                else
                {
                    m_finished = true;
                    return true;
                }
            }

            public void FinishProcess()
            {
                m_finished = true;
                m_animCallback?.Invoke(false);
            }
        }

        private Animation m_animation;

        private List<AnimationSyncProcess> m_processes;

        public LegacyAnimationSync(Animation anim)
        {
            m_animation = anim;
            m_processes = new List<AnimationSyncProcess>();
        }

        public bool PlayAnim(AnimationClip clip, Action<bool> callback)
        {
            AnimationClip clipAnimation = m_animation.GetClip(clip.name);
            if (clipAnimation == null)
            {
                m_animation.AddClip(clip, clip.name);
            }
            return PlayAnim(clip.name, callback);
        }

        public bool PlayAnim(string animName, Action<bool> callback)
        {
            AnimationClip clip = m_animation.GetClip(animName);
            if (clip != null)
            {
                m_animation.Play(animName);
                m_processes.Add(new AnimationSyncProcess(clip, callback));
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update(float deltaTime)
        {
            for (int i = m_processes.Count - 1; i >= 0; i--)
            {
                if (m_processes[i].Step(deltaTime))
                {
                    m_processes[i].FinishProcess();
                    m_processes.RemoveAt(i);
                }
            }
        }
    }

}
