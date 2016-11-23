using Assets.Scripts.Physics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class EditSubMenuPlatformController : MonoBehaviour
    {

        public Text restitutionText;
        public Slider restitutionSlider;

        public Text staticFrictionText;
        public Slider staticFrictionSlider;

        public Text dynamicFrictionText;
        public Slider dynamicFrictionSlider;

        public Button toggleButton;
        public Button defaultButton;

        /// <summary>
        /// Physics material object to use values for.
        /// </summary>
        public PhysicsMaterial Material { get; private set; }

        public void Start()
        {
            Material = new PhysicsMaterial();
            OnDefaultButtonPress();
        }

        public void OnEnable()
        {
            toggleButton.GetComponentInChildren<Text>().text = " < ";
        }

        public void OnDisable()
        {
            toggleButton.GetComponentInChildren<Text>().text = " > ";
        }

        public void Update()
        {
            restitutionText.text = "Restitution: " + restitutionSlider.value;
            staticFrictionText.text = "Static friction: " + staticFrictionSlider.value;
            dynamicFrictionText.text = "Dynamic friction: " + dynamicFrictionSlider.value;

            Material.Restitution = restitutionSlider.value;
            Material.StaticFriction = staticFrictionSlider.value;
            Material.DynamicFriction = dynamicFrictionSlider.value;
        }

        public void OnDefaultButtonPress()
        {
            Material.Default();
            restitutionSlider.value = Material.Restitution;
            staticFrictionSlider.value = Material.StaticFriction;
            dynamicFrictionSlider.value = Material.DynamicFriction;
        }
    }
}
