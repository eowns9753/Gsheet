using UnityEngine;

namespace Localize.Elements
{
    public abstract class GameObjectSingleton<T> : MonoBehaviour  where T : GameObjectSingleton<T>
    {
        private static T _instance;
        public abstract bool DonDestroyedObject { get; }

        public static bool HasUse => _instance != null; 
        
        public static  T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var find = FindObjectOfType<GameObjectSingleton<T>>();
                    find.OnRegisterInstance();
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            OnRegisterInstance();
        }

        protected void OnDestroy()
        {
            _instance = null;
        }

        protected virtual void OnRegisterInstance()
        {
            _instance = (T)this;
            //Debug.Log($"Initialize Singleton {GetType().FullName}");
            if(DonDestroyedObject)
                DontDestroyOnLoad(this);
        }
    }
}