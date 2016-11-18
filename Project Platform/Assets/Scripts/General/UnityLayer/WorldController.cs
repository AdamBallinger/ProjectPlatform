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

        public Vector2 worldGravity = new Vector2(0f, -9.807f);

        // Store private array of tile gameobjects so the unity gameobject for each tile can be modified if needed.
        private GameObject[,] tileGameObjects;

        public void Start()
        {
            World.Create(worldWidth, worldHeight);
            World.Current.InitPhysicsWorld(worldGravity);

            tileGameObjects = new GameObject[worldWidth, worldHeight];

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
                    tileGameObjects[x, y] = tileGO;
                }
            }
        }

        /// <summary>
        /// Callback function when a tile in the world has its type changed.
        /// </summary>
        /// <param name="_tileGO"></param>
        /// <param name="_tileData"></param>
        public void OnTileTypeChanged(GameObject _tileGO, Tile _tileData)
        {
            switch(_tileData.Type)
            {
                case TileType.Empty:
                    // Clear sprite if empty.
                    _tileGO.GetComponent<SpriteRenderer>().sprite = null;

                    // Remove rigidbody and collider from world.
                    if(_tileGO.GetComponent<RigidBodyComponent>() != null)
                    {
                        Destroy(_tileGO.GetComponent<BoxColliderComponent>());
                        Destroy(_tileGO.GetComponent<RigidBodyComponent>());
                    }

                    if (_tileData.OldType == TileType.Platform)
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

                    // Give platforms a box collider and rigidbody.
                    if(_tileGO.GetComponent<RigidBodyComponent>() == null)
                    {
                        _tileGO.AddComponent<RigidBodyComponent>();
                    }

                    var rb = _tileGO.GetComponent<RigidBodyComponent>();
                    rb.SetMass(0.0f);
                    rb.SetIgnoreGravity(true);
                    rb.Init();

                    if(_tileGO.GetComponent<BoxColliderComponent>() == null)
                    {
                        // Adds box collider to the tile with a default size of 1,1
                        var coll = _tileGO.AddComponent<BoxColliderComponent>();
                        coll.Create(Vector2.one);
                    }

                    World.Current.PlatformCount++;
                    break;
            }
        }

        /// <summary>
        /// Event called when all changes to a world are finished. (E.g. a selected area of tiles cleared/built are finished)
        /// </summary>
        public void OnWorldChangeFinish()
        {
            // Cull every body/collider that isn't connected to an empty tile on at least 1 of its faces to save performance.
            for (var x = 0; x < worldWidth; x++)
            {
                for(var y = 0; y < worldHeight; y++)
                {
                    var tile = World.Current.GetTileAt(x, y);

                    // If the tile at current x and y is not a platform, then skip to the next tile.
                    if(tile.Type != TileType.Platform)
                    {
                        // skip next tile on the Y axis since we know that the next tile above is guaranteed to not be connected to a platform tile since the
                        // current tile at the current x and y is not a tile itself.
                        y++;
                        continue;
                    }

                    

                    var tileLeft = World.Current.GetTileAt(x - 1, y);
                    var tileRight = World.Current.GetTileAt(x + 1, y);
                    var tileUp = World.Current.GetTileAt(x, y + 1);
                    var tileDown = World.Current.GetTileAt(x, y - 1);

                    if (tileLeft != null && tileLeft.Type != TileType.Platform) continue;
                    if (tileRight != null && tileRight.Type != TileType.Platform) continue;
                    if (tileUp != null && tileUp.Type != TileType.Platform) continue;
                    if (tileDown != null && tileDown.Type != TileType.Platform) continue;

                    //Tile is surrounded by platforms so remove its rigid body and collider components.
                    Destroy(tileGameObjects[x, y].GetComponent<BoxColliderComponent>());
                    Destroy(tileGameObjects[x, y].GetComponent<RigidBodyComponent>());
                }
            }
        }

        /// <summary>
        /// Use Unity fixed update for stepping physics world. By default Unity calls FixedUpdate 50 times per second (Time.fixedDeltaTime)
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
