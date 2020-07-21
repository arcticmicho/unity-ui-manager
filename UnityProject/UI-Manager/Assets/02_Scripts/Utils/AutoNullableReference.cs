using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartySystems.Utils
{
    public class NullableReference<T> where T : class
    {
        private T m_ref;

        private Action m_sharedRefSignals;

        private List<SharedNullableReference<T>> m_sharedRefs = new List<SharedNullableReference<T>>();

        public T Reference
        {
            get { return m_ref; }
        }

        public NullableReference(T objRef)
        {
            m_ref = objRef;
        }

        public void NullifyRef()
        {
            m_ref = null;
            m_sharedRefSignals?.Invoke();
        }

        public SharedNullableReference<T> MakeSharedRef()
        {
            return new SharedNullableReference<T>(m_ref, m_sharedRefSignals);
        }

        public SharedNullableReference<P> MakeSharedRefAs<P>() where P : class, T
        {
            return new SharedNullableReference<P>(m_ref as P, m_sharedRefSignals);
        }

        public static implicit operator T(NullableReference<T> reference)
        {
            return reference.Reference;
        }
    }

    public class SharedNullableReference<T> where T : class
    {
        private T m_ref;

        public T Reference
        {
            get { return m_ref; }
        }

        public SharedNullableReference(T objRef, Action registarableAction)
        {
            m_ref = objRef;
            registarableAction += NullRef;
        }

        private void NullRef()
        {
            m_ref = null;
        }

        public static implicit operator T(SharedNullableReference<T> reference)
        {
            return reference.Reference;
        }
    }
}

