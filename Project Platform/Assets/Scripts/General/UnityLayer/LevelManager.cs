using Assets.Scripts.General.UnityLayer.AI;
using Assets.Scripts.General.UnityLayer.UI.LevelEditor;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    public class LevelManager : MonoBehaviour
    {

        public GameObject playerPrefab;
        public GameObject aiPrefab;

        public void Start()
        {
            FindObjectOfType<WorldController>().Load(EditorPlayModeTransition.PlayModeLevel, EditorPlayModeTransition.PlayModeLevelData);
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

            var player = GameObject.FindGameObjectWithTag("Player");

            if(player != null)
            {
                aiObject.GetComponent<PathfinderAgent>().StartPathing(aiObject.transform.position, player.transform);
            }
        }

        public void OnReturnToEditorButtonPress()
        {
            EditorPlayModeTransition.ReturnToEditor();
        }
    }
}
