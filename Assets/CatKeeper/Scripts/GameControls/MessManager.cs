using UnityEngine;
using UnityEngine.UI;

namespace CatKeeper.Scripts
{
    public class MessManager : MonoBehaviour
    {
        public static MessManager Instance;

        [Header("References")]
        [SerializeField] private Slider messSlider; 
        
        [Header("Mess Settings")]
        public float maxMessPoints = 100f; 
        
        private float currentMessPoints = 0f;

        public float NormalizedMessLevel { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (messSlider != null)
            {
                messSlider.minValue = 0f;
                messSlider.maxValue = 1f;
                messSlider.value = 0f;
            }
            
            UpdateUI();
        }
        public void AddMess(float amount)
        {
            currentMessPoints = Mathf.Clamp(currentMessPoints + amount, 0f, maxMessPoints);

            NormalizedMessLevel = currentMessPoints / maxMessPoints;
            
            UpdateUI();
            
            if (NormalizedMessLevel >= 1f)
            {
                Debug.Log("Ev %100 dağıldı! (Değer: 1.0)");
            }
        }

        private void UpdateUI()
        {
            if (messSlider != null)
            {
                // Slider'a artık 45 değil, 0.45 veriyoruz
                messSlider.value = NormalizedMessLevel; 
            }
        }
    }
}