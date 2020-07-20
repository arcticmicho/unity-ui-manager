using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartySystems.UIParty
{
    public abstract class BaseUITransitionAnimation : MonoBehaviour
    {
        private enum EUITransitionState
        {
            None = 0,
            PlayingIntro = 1,
            PlayingClose = 2,
        }

        private EUITransitionState m_currentState;

        public BaseUITransitionAnimation()
        {
            m_currentState = EUITransitionState.None;
        }

        public void PlayIntroAnimation(Action<bool> onFinishIntroAnimation)
        {
            if (m_currentState == EUITransitionState.None)
            {
                m_currentState = EUITransitionState.PlayingIntro;
                PlayIntroAnimation_Local((interrupted) =>
                {
                    onFinishIntroAnimation?.Invoke(interrupted);
                    m_currentState = EUITransitionState.None;
                });
            }
            else
            {
                onFinishIntroAnimation?.Invoke(true);
            }
        }

        public void PlayCloseAnimation(Action<bool> onFinishCloseAnimation)
        {
            if (m_currentState == EUITransitionState.None)
            {
                m_currentState = EUITransitionState.PlayingClose;
                PlayCloseAnimation_Local((interrupted) =>
                {
                    onFinishCloseAnimation?.Invoke(interrupted);
                    m_currentState = EUITransitionState.None;
                });
            }
            else
            {
                onFinishCloseAnimation?.Invoke(true);
            }
        }

        protected abstract void PlayIntroAnimation_Local(Action<bool> onFinishIntroAnimation);

        protected abstract void PlayCloseAnimation_Local(Action<bool> onFinishCloseAnimation);
    }

}
