using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace PartySystems.UIParty
{
    [CreateAssetMenu(fileName = "UIViewLibrary", menuName = "UI")]
    public class UIViewLibrary : ScriptableObject
    {
        [SerializeField]
        private List<UIView> m_views;

        public ReadOnlyCollection<UIView> Views
        {
            get { return m_views.AsReadOnly(); }
        }

        public Dictionary<Type, UIView> GetViewMap()
        {
            Dictionary<Type, UIView> viewMap = new Dictionary<Type, UIView>();

            for (int i = 0, count = m_views.Count; i < count; i++)
            {
                if (!viewMap.ContainsKey(m_views[i].GetType()))
                {
                    viewMap.Add(m_views[i].GetType(), m_views[i]);
                }
            }

            return viewMap;
        }
    }

}
