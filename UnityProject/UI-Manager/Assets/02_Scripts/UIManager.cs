using System;
using System.Collections;
using System.Collections.Generic;
using PartySystems.Utils;
using UnityEngine;

namespace PartySystems.UIParty
{
    [RequireComponent(typeof(Canvas))]
    public class UIManager : MonoSingleton<UIManager>
    {
        public enum EViewPriority
        {
            LowRenderPriority = 0,
            MediumRenderPriority = 1,
            HighRenderPriority = 2,
            UltraRenderPriority = 3
        }

        [SerializeField]
        private List<UIViewLibrary> m_initialViews;        

        [SerializeField]
        private Transform m_poolingObjectsParent;

        private Canvas m_uiCanvas;

        public Canvas UICanvas
        {
            get
            {
                if (m_uiCanvas == null)
                {
                    m_uiCanvas = GetComponentInParent<Canvas>();
                }
                return m_uiCanvas;
            }
        }

        private GenericPool<UIView> m_viewPool;

        private List<UIViewLibrary> m_activeViewLibraries = new List<UIViewLibrary>();

        private Dictionary<Type, UIView> m_viewsMap = new Dictionary<Type, UIView>();

        private List<NullableReference<UIView>> m_activeViews = new List<NullableReference<UIView>>();

        private Dictionary<EViewPriority, Transform> m_priorityParentDict = new Dictionary<EViewPriority, Transform>();

        private List<NullableReference<UIView>> m_closingViews = new List<NullableReference<UIView>>();

        public void InitUIManager()
        {
            m_uiCanvas = GetComponent<Canvas>();

            InitPriorityContainers();

            for (int i = 0, count = m_initialViews.Count; i < count; i++)
            {
                AddViewLibrary(m_initialViews[i]);
            }

            if(m_poolingObjectsParent == null)
            {
                GameObject poolingGO = new GameObject();
                poolingGO.transform.SetParent(transform);
                m_poolingObjectsParent = poolingGO.transform;
            }

            m_viewPool = new GenericPool<UIView>(m_poolingObjectsParent);
        }

        public override void Init()
        {
            base.Init();
            InitUIManager();
        }

        private void InitPriorityContainers()
        {
            if(m_priorityParentDict.Count > 0)
            {
                return;
            }

            foreach( EViewPriority priority in (EViewPriority[]) Enum.GetValues(typeof(EViewPriority)))
            {
                GameObject newPriorityObject = new GameObject(priority.ToString());
                newPriorityObject.transform.SetParent(transform, false);
                newPriorityObject.transform.SetSiblingIndex((int)priority);

                m_priorityParentDict.Add(priority, newPriorityObject.transform);
            }
        }

        private void AddViewLibrary(UIViewLibrary views)
        {
            m_activeViewLibraries.Add(views);
            foreach (UIView view in views.Views)
            {
                if (!m_viewsMap.ContainsKey(view.GetType()))
                {
                    m_viewsMap.Add(view.GetType(), view);
                }
            }
        }

        private void UnloadViewLibrary(UIViewLibrary views)
        {
            for(int i=0, count=views.Views.Count; i<count; i++)
            {
                List<NullableReference<UIView>> foundRefs = m_activeViews.FindAll((nr) => nr.Reference.GetType() == views.Views[i].GetType());
                if(foundRefs.Count > 0)
                {
                    for(int m=0, count2=foundRefs.Count; m<count2; m++)
                    {
                        foundRefs[i].Reference.CloseView();
                    }
                }

                if(m_viewsMap.ContainsKey(views.Views[i].GetType()))
                {
                    m_viewsMap.Remove(views.Views[i].GetType());
                }
            }

            m_activeViewLibraries.Remove(views);
            
        }

        private void Update()
        {
            for (int i = m_activeViews.Count - 1; i >= 0; i--)
            {
                m_activeViews[i].Reference.UpdateView();
            }

            for(int i = m_closingViews.Count - 1; i >= 0; i--)
            {
                m_closingViews[i].Reference.ClearViewListeners();
                m_viewPool.ReturnObject(m_closingViews[i].Reference);
                m_closingViews[i].NullifyRef();
                m_closingViews.RemoveAt(i);
            }
        }

        /// <summary>
        /// Get an Active UIView of type T. If it doesn't exist, it will create the view with the given priority (MediumRender if priority is null)
        /// If the View exist and a priority was given, the method will update the View priority.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SharedNullableReference<T> GetHUD<T>(EViewPriority? newPriority = null) where T : UIView
        {
            Type viewType = typeof(T);
            NullableReference<T> requestedView = m_activeViews.Find((x) => x.Reference.GetType() == viewType) as NullableReference<T>;
            if(requestedView != null)
            {
                if(newPriority != null)
                {
                    requestedView.Reference.transform.SetParent(m_priorityParentDict[newPriority.Value], false);
                }
                return requestedView.MakeSharedRef();
            }else
            {
                return RequestView<T>(newPriority != null ? newPriority.Value : EViewPriority.MediumRenderPriority);
            }
        }

        /// <summary>
        /// Will instantiate (from a pool) a UIView of type T (if exist) and it will return a SharedRef of the View.
        /// When the UIView is closed, the SharedRef will be automaticaly set to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="priority">Render order for the View</param>
        /// <returns></returns>
        public SharedNullableReference<T> RequestView<T>(EViewPriority priority = EViewPriority.MediumRenderPriority) where T : UIView
        {
            if (m_viewsMap.ContainsKey(typeof(T)))
            {
                T requestedView = m_viewPool.GetObject<T>(m_viewsMap[typeof(T)] as T);
                requestedView.transform.SetParent(m_priorityParentDict[priority], false);
                requestedView.gameObject.SetActive(false);                
                NullableReference<UIView> newNullableRef = new NullableReference<UIView>(requestedView);
                m_activeViews.Add(newNullableRef);
                requestedView.RegisterOnFinishClosing(OnViewClosed);
                return newNullableRef.MakeSharedRefAs<T>();
            }

            return null;
        }

        private void OnViewClosed(UIView view)
        {
            NullableReference<UIView> nullableView = m_activeViews.Find((nv) => nv.Reference == view);
            if (nullableView != null)
            {
                m_activeViews.Remove(nullableView);
                m_closingViews.Add(nullableView);
            }
        }


    }

}

