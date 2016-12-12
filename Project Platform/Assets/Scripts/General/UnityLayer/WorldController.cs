using System.IO;
using System.Xml;
using Assets.Scripts.General.UnityLayer.Physics_Components;
using Assets.Scripts.General.UnityLayer.UI.LevelEditor;
using Assets.Scripts.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.General.UnityLayer
{
    public class WorldController : MonoBehaviour
    {
        /// <summary>
        /// Prefab for the coin pickup object.
        /// </summary>
        public GameObject coinPrefab;

        /// <summary>
        /// Prefab for the bounce pad object.
        /// </summary>
        public GameObject bouncePadPrefab;

        public EditSubMenuBounceBadController bouncePadSubMenu;

        // Store difference types of sprites based on platforms surrounding.
        public Sprite[] platformSprites;

        public Sprite gridSprite;

        public int worldWidth = 60;
        public int worldHeight = 30;

        public Vector2 worldGravity = new Vector2(0f, -9.807f);
        public float timeStep = 0.02f;

        [SerializeField]
        private bool shouldStepPhysics = true;

        // Howmany iterations to resolve collisions. Higher = better collisions at cost of FPS.
        [SerializeField]
        private int solverIterations = 8;

        // Max velocity a body can have on X and Y axis.
        [SerializeField]
        private float maxBodyVelocity = 100.0f;

        // Store private array of tile gameobjects so the unity gameobject for each tile can be modified if needed.
        private GameObject[,] tileGameObjects;

        /// <summary>
        /// Array of coin pickups in the world.
        /// </summary>
        private GameObject[,] coinObjects;

        /// <summary>
        /// Array of bounce pad objects in the world.
        /// </summary>
        private GameObject[,] bouncePadObjects;

        public void Start()
        {
            // Set the unity fixed update timestep. (Used to control the frequency of PhysicsWorld.Step).
            Time.fixedDeltaTime = timeStep;
            World.Create(worldWidth, worldHeight);
            World.Current.InitPhysicsWorld(worldGravity, solverIterations, maxBodyVelocity);
            World.Current.RegisterWorldModifyFinishCallback(OnWorldChangeFinish);
            World.Current.NavGraph.RegisterScanCompleteCallback(OnNavGraphScanComplete);

            tileGameObjects = new GameObject[worldWidth, worldHeight];
            coinObjects = new GameObject[worldWidth, worldHeight];
            bouncePadObjects = new GameObject[worldWidth, worldHeight];

            // Setup the Unity Gameobjects for the Tiles.
            for(var x = 0; x < World.Current.Width; x++)
            {
                for(var y = 0; y < World.Current.Height; y++)
                {
                    var tileGO = new GameObject();
                    // Set each tile as a child to the WorldController object.
                    tileGO.transform.SetParent(transform);
                    tileGO.name = string.Format("Tile:   X: {0}    Y: {1}", x, y);
                    tileGO.tag = "Tile";

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

                    if(coinObjects[x, y] != null)
                    {
                        // remove a coin if the tile at current x and y was changed from an empty tile.
                        RemoveCoinPickup(new Vector2(x, y));
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
                        // make sure the tile has a collider.
                        AddTileCollider(tileGameObjects[x, y]);
                        continue;
                    }

                    //Tile is surrounded by platforms so remove its rigid body and collider components to save performance.
                    RemoveTileCollider(tileGameObjects[x, y]);
                }
            }

            // Updated pathing when world changes.
            World.Current.NavGraph.ScanGraph();
        }

        public void OnNavGraphScanComplete()
        {
            var pathfindingDebug = FindObjectOfType<PathfindingDebugDrawController>();

            if(pathfindingDebug != null)
                pathfindingDebug.RebuildDebugObjects();
        }

        /// <summary>
        /// Adds a collider component to a given tile gameobject.
        /// </summary>
        /// <param name="_tileGO"></param>
        private void AddTileCollider(GameObject _tileGO)
        {
            var rbc = _tileGO.GetComponent<RigidBodyComponent>();

            if (rbc == null)
            {
                rbc = _tileGO.AddComponent<RigidBodyComponent>();
                rbc.SetMass(0.0f);
                rbc.SetIgnoreGravity(true);
                rbc.Create();

                // Adds box collider to the tile with a default size of 1,1
                var coll = _tileGO.AddComponent<BoxColliderComponent>();
                coll.Create(Vector2.one);
            }
        }

        /// <summary>
        /// Removes a collider and rigidbody component from a given tile gameobject.
        /// </summary>
        /// <param name="_tileGO"></param>
        private void RemoveTileCollider(GameObject _tileGO)
        {
            DestroyImmediate(_tileGO.GetComponent<BoxColliderComponent>()); 
            DestroyImmediate(_tileGO.GetComponent<RigidBodyComponent>());
        }

        /// <summary>
        /// Sets the material for the tile at the given x and y to the given material.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_material"></param>
        public void SetTileMaterial(int _x, int _y, PhysicsMaterial _material)
        {
            var tileGO = tileGameObjects[_x, _y];
            var rbc = tileGO.GetComponent<RigidBodyComponent>();

            if(rbc != null)
            {
                rbc.RigidBody.Material = _material;
            }
        }

        /// <summary>
        /// Adds a coin pickup object at the given world coordinate.
        /// </summary>
        /// <param name="_coord"></param>
        public void AddCoinPickup(Vector2 _coord)
        {
            var worldCoord = _coord;
            var gridCoord = World.Current.WorldPointToGridPoint(worldCoord);
            var gridX = (int) gridCoord.x;
            var gridY = (int) gridCoord.y;

            if(World.Current.GetTileAt(gridX, gridY).Type != TileType.Empty)
            {
                // dont allow coins to be placed ontop of none empty tiles.
                return;
            }

            if(coinObjects[gridX, gridY] != null)
            {
                // if a coin already exists then remove it from the current position. (easy cheat for removing coins in editor)
                RemoveCoinPickup(_coord);
                return;
            }

            var coinObject = Instantiate(coinPrefab, worldCoord, Quaternion.identity);

            // Parent the coin to the world controller object.
            coinObject.transform.SetParent(transform);

            coinObjects[gridX, gridY] = coinObject;
        }

        /// <summary>
        /// Removes a coin pickup from the given world/grid coordinate.
        /// Coordinates are recalculated regardless of what type is given.
        /// </summary>
        /// <param name="_coord"></param>
        public void RemoveCoinPickup(Vector2 _coord)
        {
            var gridCoord = World.Current.WorldPointToGridPoint(_coord);
            var gridX = (int) gridCoord.x;
            var gridY = (int) gridCoord.y;

            if(coinObjects[gridX, gridY] != null)
            {
                DestroyImmediate(coinObjects[gridX, gridY]);
            }
        }

        /// <summary>
        /// Adds a bounce pad object to the world at the given coordinate (world or grid).
        /// </summary>
        /// <param name="_coord"></param>
        public GameObject AddBouncePad(Vector2 _coord)
        {
            var worldCoord = _coord;
            var gridCoord = World.Current.WorldPointToGridPoint(worldCoord);
            var gridX = (int) gridCoord.x;
            var gridY = (int) gridCoord.y;

            if (World.Current.GetTileAt(gridX, gridY).Type != TileType.Empty)
            {
                // dont allow bounce pads to be placed ontop of none empty tiles.
                return null;
            }

            if (bouncePadObjects[gridX, gridY] != null)
            {
                // if a pad already exists then remove it from the current position. (easy cheat for removing pads in editor)
                RemoveBouncePad(_coord);
                return null;
            }

            var padObject = Instantiate(bouncePadPrefab, worldCoord, Quaternion.identity);

            // Parent pad to world controller object.
            padObject.transform.SetParent(transform);

            bouncePadObjects[gridX, gridY] = padObject;

            return padObject;
        }

        /// <summary>
        /// Removes a bounce pad from the world at the given coordinate (world or grid) if one exists.
        /// </summary>
        /// <param name="_coord"></param>
        public void RemoveBouncePad(Vector2 _coord)
        {
            var gridCoord = World.Current.WorldPointToGridPoint(_coord);
            var gridX = (int) gridCoord.x;
            var gridY = (int) gridCoord.y;

            if(bouncePadObjects[gridX, gridY] != null)
            {
                DestroyImmediate(bouncePadObjects[gridX, gridY]);
            }
        }

        /// <summary>
        /// Use Unity fixed update for stepping physics world. Timestep is determined by Time.fixedDeltaTime. (0.02 by default)
        /// </summary>
        public void FixedUpdate()
        {
            // Dont step physics when inside the level editor scene.
            if(shouldStepPhysics && World.Current != null && World.Current.PhysicsWorld != null)
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
        /// Clears the world of all objects, and creates an empty bordered level.
        /// </summary>
        public void Clear()
        {
            World.Current.Clear();

            for(var x = 0; x < worldWidth; x++)
            {
                for(var y = 0; y < worldHeight; y++)
                {
                    RemoveCoinPickup(new Vector2(x, y));
                    RemoveBouncePad(new Vector2(x, y));
                }
            }

            World.Current.SetBorderAsPlatform();
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

        /// <summary>
        /// Saves the current world to the disk with the given filename.
        /// </summary>
        /// <param name="_saveFile">File name for save file (Level name and not full directory or file extension).</param>
        public void Save(string _saveFile)
        {
            World.Current.Save(_saveFile);
            //Debug.Log("Saving level data for level: " + _saveFile);

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.NewLineOnAttributes = false;

            Directories.CheckDirectories();

            var saveFileLocation = Path.Combine(Directories.Save_Levels_Data_Directory, _saveFile + ".xml");

            using (var xmlWriter = XmlWriter.Create(saveFileLocation, settings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("LevelDataFile");

                xmlWriter.WriteStartElement("LevelPlatformPhysicalProperties");

                for (var x = 0; x < worldWidth; x++)
                {
                    for (var y = 0; y < worldHeight; y++)
                    {
                        var rbc = tileGameObjects[x, y].GetComponent<RigidBodyComponent>();

                        if (rbc == null)
                        {
                            continue;
                        }

                        xmlWriter.WriteStartElement("PlatformProperties");
                        xmlWriter.WriteAttributeString("X", x.ToString());
                        xmlWriter.WriteAttributeString("Y", y.ToString());
                        xmlWriter.WriteAttributeString("Restitution", rbc.RigidBody.Material.Restitution.ToString());
                        xmlWriter.WriteAttributeString("StaticFriction", rbc.RigidBody.Material.StaticFriction.ToString());
                        xmlWriter.WriteAttributeString("DynamicFriction", rbc.RigidBody.Material.DynamicFriction.ToString());
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("LevelCoinPickupsData");

                // Write coin pickup data to file.
                for (var x = 0; x < worldWidth; x++)
                {
                    for (var y = 0; y < worldHeight; y++)
                    {
                        // If there isnt a coin at current x,y then continue.
                        if (coinObjects[x, y] == null) continue;

                        xmlWriter.WriteStartElement("CoinPickup");
                        xmlWriter.WriteAttributeString("X", x.ToString());
                        xmlWriter.WriteAttributeString("Y", y.ToString());
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("LevelBouncePadData");

                // Write bounce pad data to file.
                for (var x = 0; x < worldWidth; x++)
                {
                    for (var y = 0; y < worldHeight; y++)
                    {
                        // If there isnt a pad at current x,y then continue.
                        if (bouncePadObjects[x, y] == null) continue;

                        var springComp = bouncePadObjects[x, y].GetComponent<SpringJointComponent>();

                        xmlWriter.WriteStartElement("BouncePad");
                        xmlWriter.WriteAttributeString("X", x.ToString());
                        xmlWriter.WriteAttributeString("Y", y.ToString());
                        xmlWriter.WriteAttributeString("Stiffness", springComp.Joint.Stiffness.ToString());
                        xmlWriter.WriteAttributeString("RestLength", springComp.Joint.RestLength.ToString());
                        xmlWriter.WriteAttributeString("Dampen", springComp.Joint.Dampen.ToString());
                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }

            //Debug.Log("Finished saving level data for level: " + _saveFile);
        }

        /// <summary>
        /// Loads the given file (full directory) from disk and returns the name of the loaded level.
        /// </summary>
        /// <param name="_loadFile">Directory for the save level XML file.</param>
        /// <param name="_loadDataFile">Directory for the save level data XML file.</param>
        public string Load(string _loadFile, string _loadDataFile)
        {
            Clear();
            var levelName = World.Current.Load(_loadFile);
            //Debug.Log("Loading level data for level: " + levelName + " from: " + _loadDataFile);

            using (var xmlReader = XmlReader.Create(_loadDataFile))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.IsStartElement())
                    {
                        switch (xmlReader.Name)
                        {
                            case "PlatformProperties":
                                var x = int.Parse(xmlReader["X"]);
                                var y = int.Parse(xmlReader["Y"]);
                                var material = new PhysicsMaterial();
                                material.Restitution = float.Parse(xmlReader["Restitution"]);
                                material.StaticFriction = float.Parse(xmlReader["StaticFriction"]);
                                material.DynamicFriction = float.Parse(xmlReader["DynamicFriction"]);
                                SetTileMaterial(x, y, material);
                                break;

                            case "CoinPickup":
                                var cX = int.Parse(xmlReader["X"]);
                                var cY = int.Parse(xmlReader["Y"]);
                                AddCoinPickup(new Vector2(cX, cY));
                                break;

                            case "BouncePad":
                                var pX = int.Parse(xmlReader["X"]);
                                var pY = int.Parse(xmlReader["Y"]);
                                var stiff = float.Parse(xmlReader["Stiffness"]);
                                var rest = float.Parse(xmlReader["RestLength"]);
                                var dampen = float.Parse(xmlReader["Dampen"]);
                                var pad = AddBouncePad(new Vector2(pX, pY));
                                var springComp = pad.GetComponent<SpringJointComponent>();
                                springComp.stiffness = stiff;
                                springComp.restLength = rest;
                                springComp.dampen = dampen;
                                break;
                        }
                    }
                }
            }

            //Debug.Log("Finished loading level data for level: " + levelName);
            return levelName;
        }
    }
}