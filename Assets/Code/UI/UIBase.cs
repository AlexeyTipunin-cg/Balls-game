using System;
using UnityEngine;

namespace Code
{
    [Serializable]

    public class UIBase : MonoBehaviour
    {      
        public virtual void StartUI()
        {
            
        }
        
        public virtual void UpdateUI()
        {
            
        }
        
        public virtual void ActivateUI()
        {
            gameObject.SetActive(true);
            OnOpen();
        }
        
        public virtual void DisableUI()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnOpen()
        {
            
        }
    }
}