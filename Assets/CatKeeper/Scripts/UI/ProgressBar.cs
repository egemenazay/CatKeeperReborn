using System;
using UnityEngine;
using UnityEngine.UI;

namespace CatKeeper.Scripts
{
    public class ProgressBar : MonoBehaviour
    {
        public float minimum;
        public float maximum;
        public float current;
        public Image mask;

        private void Update()
        {
            GetCurrentFill();
        }

        private void GetCurrentFill()
        {
            float fillAmount = current / maximum;
            mask.fillAmount = fillAmount;
        }
    }
    
}
