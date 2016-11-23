using Assets.Scripts.General.UnityLayer.UI.LevelEditor;
using Assets.Scripts.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.General.UnityLayer
{
    public class WorldController : MonoBehaviour
    {

        public EditSubMenuPlatformController platformSubMenu;

        // Store difference types of sprites based on platforms surrounding.
        public Sprite[] platformSprites;

        public Sprite gridSprite;

        public int worldWidth = 128;
        public int worldHeight = 128;

        public Vector2 worldGravity = new Vector2(0f, -9.807f);
        public float timeStep = 0.02f;

        // Howmany iterations to resolve collisions. Higher = better collisions at cost of FPS.
        [SerializeField]
        private int solverIterations = 8;

        // Max velocity a body can have on X and Y axis.
        [SerializeField]
        private float maxBodyVelocity = 100.0f;


        // Store private array of tile gameobjects so the unity gameobject for each tile can be modified if needed.
        private GameObject[,] tileGameObjects;

        public void Start()
        {
            // Set the unity fixed update timestep. (Used to control the frequency of PhysicsWorld.Step).
            Time.fixedDeltaTime = timeStep;
            World.Create(worldWidth, worldHeight);
            World.Current.InitPhysicsWorld(worldGravity, solverIterations, maxBodyVelocity);
            World.Current.RegisterWorldModifyFinishCallback(OnWorldChangeFinish);

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

            World.Current.SetBorderAsPlatform();
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
                    _tileGO.GetComponent<SpriteRenderer>().sprite = platformSprites[0];

                    AddTileCollider(_tileGO);

                    World.Current.PlatformCount++;
                    break;
            }
        }

        /// <summary>
        /// Callback function called when all changes to a world are finished. (E.g. a selected area of tiles cleared/built are finished)
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
                        if(tile.Type == TileType.Empty)
                        {
                            RemoveTileCollider(tileGameObjects[x, y]);
                        }
                        continue;
                    }

                    var tileLeft = World.Current.GetTileAt(x - 1, y);
                    var tileRight = World.Current.GetTileAt(x + 1, y);
                    var tileUp = World.Current.GetTileAt(x, y + 1);
                    var tileDown = World.Current.GetTileAt(x, y - 1);

                    // Set sprite based on surrounding tile types.
                    SetTileSpriteFromAdjacent(tile, tileRight, tileLeft, tileUp, tileDown);

                    if ((tileLeft != null && tileLeft.Type != TileType.Platform)
                        || (tileRight != null && tileRight.Type != TileType.Platform)
                        || (tileUp != null && tileUp.Type != TileType.Platform)
                        || (tileDown != null && tileDown.Type != TileType.Platform))
                    {
                        //AddTileCollider(tileGameObjects[x, y]);
                        continue;
                    }

                    //Tile is surrounded by platforms so remove its rigid body and collider components to save performance.
                    RemoveTileCollider(tileGameObjects[x, y]);
                }
            }
        }

        private void AddTileCollider(GameObject _tileGO)
        {
            var rbc = _tileGO.GetComponent<RigidBodyComponent>();

            if (rbc == null)
            {
                rbc = _tileGO.AddComponent<RigidBodyComponent>();
                rbc.SetMass(0.0f);
                rbc.SetIgnoreGravity(true);
                rbc.Init();

                // Adds box collider to the tile with a default size of 1,1
                var coll = _tileGO.AddComponent<BoxColliderComponent>();
                coll.Create(Vector2.one);
            }
        }

        private void RemoveTileCollider(GameObject _tileGO)
        {
            DestroyImmediate(_tileGO.GetComponent<BoxColliderComponent>()); 
            DestroyImmediate(_tileGO.GetComponent<RigidBodyComponent>());
        }

        /// <summary>
        /// Sets the tile at the given X and Y material to the current material settings set by the platforms UI menu.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        public void SetTileMaterial(int _x, int _y)
        {
            var tileGO = tileGameObjects[_x, _y];
            var rbc = tileGO.GetComponent<RigidBodyComponent>();

            if(rbc != null)
            {
                rbc.RigidBody.Material = new PhysicsMaterial(platformSubMenu.Material);
            }
        }

        /// <summary>
        /// Use Unity fixed update for stepping physics world. Timestep is determined by Time.fixedDeltaTime. (0.02 by default)
        /// </summary>
        public void FixedUpdate()
        {
            if(World.Current != null && World.Current.PhysicsWorld != null)
            {
                World.Current.PhysicsWorld.Step();
            }
        }

        public void OnDestroy()
        {
            // Clear any nodes generated.
            World.Current.NavGraph.Clear();
        }

        /// <summary>
        /// Sets the sprite for given tile based on its surrounding tiles.
        /// </summary>
        /// <param name="_tile">The tile being set.</param>
        /// <param name="_tileRight">The tile to the right.</param>
        /// <param name="_tileLeft">The tile to the left.</param>
        /// <param name="_tileUp">The tile above.</param>
        /// <param name="_tileDown">The tile below.</param>
        private void SetTileSpriteFromAdjacent(Tile _tile, Tile _tileRight, Tile _tileLeft, Tile _tileUp, Tile _tileDown)
        {
            var spriteRenderer = tileGameObjects[_tile.X, _tile.Y].GetComponent<SpriteRenderer>();

            // If for some reason the sprire renderer is null then break out.
            if (spriteRenderer == null)
                return;

            _tile.Adjacent = AdjacentFlag.None;

            // Compute the adjacent flags.
            _tile.CheckAdjacent(_tileLeft, AdjacentFlag.Left);
            _tile.CheckAdjacent(_tileRight, AdjacentFlag.Right);
            _tile.CheckAdjacent(_tileUp, AdjacentFlag.Up);
            _tile.CheckAdjacent(_tileDown, AdjacentFlag.Down);

            var spriteIndex = 0;

            // Bitmasking method as found here: http://www.angryfishstudios.com/2011/04/adventures-in-bitmasking/ 
            if (!_tile.HasAdjacentFlags(AdjacentFlag.Left))
            {
                spriteIndex += 8;
            }

            if(!_tile.HasAdjacentFlags(AdjacentFlag.Right))
            {
                spriteIndex += 2;
            }

            if(!_tile.HasAdjacentFlags(AdjacentFlag.Up))
            {
                spriteIndex += 1;
            }

            if(!_tile.HasAdjacentFlags(AdjacentFlag.Down))
            {
                spriteIndex += 4;
            }

            spriteRenderer.sprite = platformSprites[spriteIndex];
        }
    }
}
