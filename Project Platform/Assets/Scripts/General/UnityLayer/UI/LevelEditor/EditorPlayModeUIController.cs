using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.General.UnityLayer.UI.LevelEditor
{
    public class EditorPlayModeUIController : MonoBehaviour
    {
        public Text playerScore;

        public PlayerController playerController;


        public void Update()
        {
            if(playerController != null)
            {
                playerScore.text = "Score: " + playerController.score + " / " + FindObjectOfType<WorldController>().GetTotalCoins();
            }
            else
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");

                if(playerObj != null)
                {
                    playerController = playerObj.GetComponent<PlayerController>();
                }
            }
        }

    }
}
