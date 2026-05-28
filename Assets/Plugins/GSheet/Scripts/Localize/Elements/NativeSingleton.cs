using System;

namespace Localize.Elements
{
    public abstract class NativeSingleton<T> where T : NativeSingleton<T>
    {
        protected static T _instance; 
        
        public static  T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Activator.CreateInstance<T>();
                    _instance.OnCreateInstance();
                }
                return _instance;
            }
        }


        protected virtual void OnCreateInstance()
        {
         
        }

        void DestroyInstance()
        {
            _instance = null;
        }
    }
}