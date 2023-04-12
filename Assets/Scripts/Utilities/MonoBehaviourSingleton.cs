using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            var objs = FindObjectsOfType(typeof(T)) as T[];

            if (objs.Length > 0)
            {
                _instance = objs[0];
            }

            if (objs.Length > 1)
            {
                Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
            }

            if (_instance != null)
            {
                return _instance;
            }

            GameObject obj = new()
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            _instance = obj.AddComponent<T>();

            return _instance;
        }
    }
}