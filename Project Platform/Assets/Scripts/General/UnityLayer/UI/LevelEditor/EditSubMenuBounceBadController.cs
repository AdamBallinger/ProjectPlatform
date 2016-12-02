using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class EditSubMenuBounceBadController : MonoBehaviour
    {
        public LevelEditorUIController editorUIController;
        public GameObject bouncePadSettingsPanel;

        public Text stiffnessText;
        public Text restLengthText;
        public Text dampenText;

        public Slider stiffnessSlider;
        public Slider restLengthSlider;
        public Slider dampenSlider;

        public Button toggleButton;

        private const float defaultStiffness = 20000;
        private const float defaultRestLength = 0;
        private const float defaultDampen = 1000;

        public float Stiffness { get; private set; }
        public float RestLength { get; private set; }
        public float Dampen { get; private set; }

        public void Start()
        {
            OnDefaultButtonPress();
        }

        public void OnDefaultButtonPress()
        {
            stiffnessSlider.value = defaultStiffness;
            restLengthSlider.value = defaultRestLength;
            dampenSlider.value = defaultDampen;
        }

        public void OnBouncePadSettingsButtonPress()
        {
            editorUIController.OpenSubMenu(bouncePadSettingsPanel);
        }

        public void Update()
        {
            stiffnessText.text = "Stiffness: " + Stiffness;
            restLengthText.text = "Rest Length: " + RestLength;
            dampenText.text = "Dampening: " + Dampen;

            Stiffness = stiffnessSlider.value;
            RestLength = restLengthSlider.value;
            Dampen = dampenSlider.value;

            var buttonText = editorUIController.GetActiveSubMenu() == bouncePadSettingsPanel ? "<" : ">";
            toggleButton.GetComponentInChildren<Text>().text = buttonText;
        }
    }
}
