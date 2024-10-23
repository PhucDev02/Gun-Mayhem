using UnityEngine;

namespace Foundation.Game.Core.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : Object
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (!instance) instance = FindObjectOfType<T>();
                return instance;
            }
        }
        protected virtual void OnDestroy()
        {
            instance = null;
        }
    }
}