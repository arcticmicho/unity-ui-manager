using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartySystems.UIParty
{
    public abstract class UIView : MonoBehaviour
    {
        [SerializeField]
        private BaseUITransitionAnimation m_transitioner;

        private Action<UIView> m_onFinishClosing;
        private Action<UIView> m_onFinishShowing;

        public void ShowView()
        {
            gameObject.SetActive(true);
            OnStartShowing();

            if (m_transitioner != null)
            {
                m_transitioner.PlayIntroAnimation((interrupted) =>
                {
                    OnFinishShowing();
                    m_onFinishShowing?.Invoke(this);
                });
            }
            else
            {
                OnFinishShowing();
                m_onFinishShowing?.Invoke(this);
            }
        }

        public void CloseView()
        {
            OnStarClosing();

            if (m_transitioner != null)
            {
                m_transitioner.PlayCloseAnimation((interrupted) =>
                {
                    OnFinishClosing();
                    m_onFinishClosing?.Invoke(this);
                    gameObject.SetActive(false);
                });
            }
            else
            {
                OnFinishClosing();
                m_onFinishClosing?.Invoke(this);
                gameObject.SetActive(false);
            }
        }

        public void UpdateView()
        {
            OnUpdate();
        }

        public void RegisterOnFinishClosing(Action<UIView> onFinishClosing)
        {
            m_onFinishClosing += onFinishClosing;
        }

        public void RegisterOnFinishShowing(Action<UIView> onFinishShowing)
        {
            m_onFinishShowing += onFinishShowing;
        }

        public void ClearViewListeners()
        {
            m_onFinishClosing = null;
            m_onFinishShowing = null;
        }

        protected virtual void OnUpdate() { }

        protected virtual void OnStartShowing() { }

        protected virtual void OnFinishShowing() { }

        protected virtual void OnStarClosing() { }

        protected virtual void OnFinishClosing() { }

    }

}

