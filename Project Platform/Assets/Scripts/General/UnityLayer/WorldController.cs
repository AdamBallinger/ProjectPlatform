using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.General.UnityLayer.UI.LevelEditor;

namespace Assets.Scripts.General.UnityLayer
{
    public class WorldController : MonoBehaviour
    {

        public Sprite platformSprite;
        public Sprite gridSprite;

        public int worldWidth = 128;
        public int worldHeight = 128;

        public Vector2 worldGravity = new Vector2(0f, 9.807f);

        public void Start()
        {
            World.Create(worldWidth, worldHeight);
            World.Current.InitPhysicsWorld(worldGravity);

            // Setup the Unity Gameobjects for the Tiles.
            for(var x = 0; x < World.Current.Width; x++)
            {
                for(var y = 0; y < World.Current.Height; y++)
                {
                    var tileGO = new GameObject();
                    // Set each tile as a child to the WorldController object.
                    tileGO.transform.SetParent(transform);
                    tileGO.name = string.Format("Tile:   X: {0}    Y: {1}", x, y);

                    var tileData = World.Current.GetTileAt(x, y);
                    tileData.RegisterTileTypeChangeCallback(tile => { OnTileTypeChanged(tileGO, tile); });

                    var tileSR = tileGO.AddComponent<SpriteRenderer>();

                    // Only set air tiles to grid tile sprite if in the editor to aid seeing where tiles are.
                    if(SceneManager.GetActiveScene().name == "level_editor")
                    {
                        tileSR.sprite = gridSprite;
                    }

                    tileGO.transform.position = new Vector2(tileData.X, tileData.Y);
                }
            }
        }

        public void OnTileTypeChanged(GameObject _tileGO, Tile _tileData)
        {
            switch(_tileData.Type)
            {
                case TileType.Empty:
                    // Clear sprite if empty.
                    _tileGO.GetComponent<SpriteRenderer>().sprite = null;

                    if(_tileData.OldType == TileType.Platform)
                    {
                        World.Current.PlatformCount--;
                    }

                    // If in level editor then display a grid tile to aid in seeing individual tile locations.
                    if(SceneManager.GetActiveScene().name == "level_editor")
                    {
                        _tileGO.GetComponent<SpriteRenderer>().sprite = gridSprite;
                    }
                    break;

                case TileType.Platform:
                    _tileGO.GetComponent<SpriteRenderer>().sprite = platformSprite;
                    World.Current.PlatformCount++;
                    break;
            }

            FindObjectOfType<LevelEditorUIController>().OnWorldModified();
        }

        /// <summary>
        /// Use Unity fixed update for stepping physics world. By default Unity calls FixedUpdate 50 times per second.
        /// </summary>
        public void FixedUpdate()
        {
            if(World.Current != null && World.Current.PhysicsWorld != null)
            {
                World.Current.PhysicsWorld.Step();
            }
        }
    }
}
