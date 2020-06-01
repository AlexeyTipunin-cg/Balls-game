using UnityEngine;

namespace Code
{
    public static class UnityUtils
    {
        public static T getOrAddComponent<T>(this GameObject gameObject) where T:Component
        {
            T component = gameObject.GetComponent<T>();
            if (component != null )
            {
                return component;
            }
            return gameObject.AddComponent<T>();
        }

        public static GameObject findChildWithName(this GameObject gameObj, string name)
        {
            int childCount = gameObj.transform.childCount;
            if (childCount == 0)
            {
                return null;
            }
            
            
            for (int count = 0; count < childCount; count++)
            {
                Transform child = gameObj.transform.GetChild(count);
                if (child.name == name)
                {
                    return child.gameObject;
                }
            }

            return null;
        }
        
        public static UIBase createUI (this GameObject ui)
        {
            UIBase component = ui.GetComponent<UIBase>();
            component.StartUI();
            return component;
        }
    }
}