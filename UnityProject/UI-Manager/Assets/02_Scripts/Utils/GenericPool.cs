using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace PartySystems.Utils
{
    public class GenericPool<T> where T: MonoBehaviour
    {
        private Transform m_objectsParent;

        private Dictionary<Type, List<T>> m_pooledObjects;

        private Dictionary<Type, List<T>> m_borrowedObjects;

        public GenericPool(Transform objectsParent)
        {
            m_objectsParent = objectsParent;
            m_pooledObjects = new Dictionary<Type, List<T>>();
            m_borrowedObjects = new Dictionary<Type, List<T>>();
        }

        public T2 GetObject<T2>(T2 template = null) where T2 : T
        {
            Type objType = typeof(T2);
            if(m_pooledObjects.ContainsKey(objType) && m_pooledObjects[objType].Count > 0)
            {
                T2 pooledObject = m_pooledObjects[objType][0] as T2;
                m_pooledObjects[objType].Remove(pooledObject);

                if(!m_borrowedObjects.ContainsKey(objType))
                {
                    m_borrowedObjects.Add(objType, new List<T>());
                }

                m_borrowedObjects[objType].Add(pooledObject);
                m_pooledObjects[objType].Remove(pooledObject);
                return pooledObject;
            }else
            {
                if (template == null)
                {
                    return null;
                }

                if (!m_pooledObjects.ContainsKey(objType))
                {
                    m_pooledObjects.Add(objType, new List<T>());
                    m_borrowedObjects.Add(objType, new List<T>());
                }

                T2 newObject = GameObject.Instantiate<T2>(template);
                m_borrowedObjects[objType].Add(newObject);
                return newObject;
            }
        }

        public bool ReturnObject(T borrowedObj)
        {
            Type objType = borrowedObj.GetType();
            if (m_borrowedObjects.ContainsKey(objType) && m_borrowedObjects[objType].Contains(borrowedObj))
            {
                borrowedObj.gameObject.SetActive(false);
                borrowedObj.transform.SetParent(m_objectsParent, false);
                m_borrowedObjects[objType].Remove(borrowedObj);
                m_pooledObjects[objType].Add(borrowedObj);
                return true;
            }else
            {
                return false;
            }
        }
    }
}

