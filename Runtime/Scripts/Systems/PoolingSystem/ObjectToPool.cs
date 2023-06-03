// © 2023 Marcello De Bonis. All rights reserved.

using System;
using UnityEngine;

namespace UnityGamesToolkit.Runtime
{
    /// <summary>
    /// Configuration class for objects to be pooled.
    /// </summary>
    [Serializable]
    public class ObjectToPool<T> where T : MonoBehaviour, IPoolable
    {
        [SerializeField]
        public GameObject objectPoolable;

        [Header("The parent objects to attach the poolable object")]
        public GameObject activatedParent;
        public GameObject deactivatedParent;

        [Header("Transform for the objects")]
        public Transform transformObject;

        [Header("The time after the object deactivates. Set to 0 if not used.")]
        public float dieTime;

        // Action to be executed by the object after the dieTime has passed.
        private Action actionToDoAfterDieTime;
        
        public void OnObjectPoolableChanged()
        {
            // Verifica se objectPoolable ha il componente T al suo interno
            if (objectPoolable.GetComponent<T>() == null)
            {
                Debug.Log(("Error! Use a GameObject with " + typeof(T).ToString() + " component attached " ));
                objectPoolable = null;
            }
        }
    }
}