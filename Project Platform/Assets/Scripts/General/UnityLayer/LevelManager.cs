using Assets.Scripts.General.UnityLayer.AI;
using Assets.Scripts.General.UnityLayer.UI.LevelEditor;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    public class LevelManager : MonoBehaviour
    {

        public GameObject playerPrefab;
        public GameObject aiPrefab;

        private WorldController worldController;

        public void Start()
        {
            worldController = FindObjectOfType<WorldController>();
            worldController.Load(EditorPlayModeTransition.PlayModeLevel, EditorPlayModeTransition.PlayModeLevelData);
        }

        public void Update()
        {
            if(GameObject.FindGameObjectWithTag("Player") == null)
            {
                SpawnPlayer();
            }

            if(GameObject.FindGameObjectWithTag("AI") == null)
            {
                SpawnAI();
            }

            if(worldController.GetCoinCount() == 0)
            {
                OnPlayerCollectsAllCoins();
            }
        }

        public void SpawnPlayer()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
            {
                Instantiate(playerPrefab, World.Current.PlayerSpawnPosition, Quaternion.identity);
            }
            else
            {
                playerObject.transform.position = World.Current.PlayerSpawnPosition;
            }
        }

        public void SpawnAI()
        {
            var aiObject = GameObject.FindGameObjectWithTag("AI");
            if(aiObject == null)
            {
                aiObject = Instantiate(aiPrefab, World.Current.AISpawnPosition, Quaternion.identity);
            }
            else
            {
                aiObject.transform.position = World.Current.AISpawnPosition;
            }

            //var player = GameObject.FindGameObjectWithTag("Player");

            //if(player != null)
            //{
            //    aiObject.GetComponent<PathfinderAgent>().StartPathing(aiObject.transform.position, player.transform);
            //}
        }

        /// <summary>
        /// Called when the player is caught by the AI.
        /// </summary>
        public void OnPlayerCaught()
        {
            // TODO: Display some kind of gameover UI as the player was caught by the AI. After a few seconds restart the level.
        }

        /// <summary>
        /// Called when the player has collected every coin in the level.
        /// </summary>
        public void OnPlayerCollectsAllCoins()
        {
            // TODO: Display payer won UI of some kind and after a few seconds load the next level if there is another to load.
            // For loading next level, keep track of the current level (e.g. level 1 will set this val to 1 level 2 set to 2 etc).
        }

        public void OnReturnToEditorButtonPress()
        {
            EditorPlayModeTransition.ReturnToEditor();
        }
    }
}
