using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PartySystems.Utils
{
    /// <summary>
    /// Mono singleton Class. Extend this class to make singleton component.
    /// Example: 
    /// <code>
    /// public class Foo : MonoSingleton<Foo>
    /// </code>. To get the instance of Foo class, use <code>Foo.instance</code>
    /// Override <code>Init()</code> method instead of using <code>Awake()</code>
    /// from this class.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T m_instance = null;
        public static T Instance
        {
            get
            {
                // Instance requiered for the first time, we look for it
                if (m_instance == null)
                {
                    m_instance = GameObject.FindObjectOfType(typeof(T)) as T;

                    // Object not found, we create a temporary one
                    if (m_instance == null)
                    {
                        Debug.LogWarning("No instance of " + typeof(T).ToString());
                        return null;
                    }
                    if (!s_isInitialized)
                    {
                        s_isInitialized = true;
                        m_instance.Init();
                    }
                }
                return m_instance;
            }
        }

        public static bool IsTemporaryInstance { private set; get; }

        private static bool s_isInitialized;

        // If no other monobehaviour request the instance in an awake function
        // executing before this one, no need to search the object.
        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this as T;
            }
            else if (m_instance != this)
            {
                Debug.LogError("Another instance of " + GetType() + " is already exist! Destroying self...");
                DestroyImmediate(gameObject);
                return;
            }
            if (!s_isInitialized)
            {
                DontDestroyOnLoad(gameObject);
                s_isInitialized = true;
                m_instance.Init();
            }
        }


        /// <summary>
        /// This function is called when the instance is used the first time
        /// Put all the initializations you need here, as you would do in Awake
        /// </summary>
        public virtual void Init() { }

        /// Make sure the instance isn't referenced anymore when the user quit, just in case.
        private void OnApplicationQuit()
        {
            m_instance = null;
        }
    }

}

