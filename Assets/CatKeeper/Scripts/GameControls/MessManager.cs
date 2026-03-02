using UnityEngine;
using UnityEngine.UI;

namespace CatKeeper.Scripts
{
    public class MessManager : MonoBehaviour
    {
        public static MessManager Instance;

        [Header("UI Ayarları")]
        public Slider messSlider; 
        
        [Header("Dağınıklık Kapasitesi")]
        [Tooltip("Evin alabileceği maksimum dağınıklık puanı")]
        public float maxMessPoints = 100f; 
        
        // Arka planda tutulan gerçek puan (Örn: 45 puan)
        private float currentMessPoints = 0f;

        // DIŞARIDAN OKUNABİLİR NORMALİZE DEĞER (0.0 ile 1.0 arası)
        public float NormalizedMessLevel { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            // Hata yapmamak için Slider'ın 0-1 aralığında olmasını kodla garanti altına alıyoruz
            if (messSlider != null)
            {
                messSlider.minValue = 0f;
                messSlider.maxValue = 1f;
                messSlider.value = 0f;
            }
            
            UpdateUI();
        }

        // Eşyalar (ItemMessState) hala 10, 20 gibi raw (ham) puanlar gönderecek
        public void AddMess(float amount)
        {
            // 1. Ham puanı ekle ve sınırla (0 ile 100 puan arası)
            currentMessPoints = Mathf.Clamp(currentMessPoints + amount, 0f, maxMessPoints);
            
            // 2. NORMALİZASYON İŞLEMİ (Mevcut / Maksimum)
            // Örnek: 45 / 100 = 0.45f
            NormalizedMessLevel = currentMessPoints / maxMessPoints;
            
            UpdateUI();
            
            // Normalize değer 1'e ulaştıysa bar tam dolmuş demektir
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