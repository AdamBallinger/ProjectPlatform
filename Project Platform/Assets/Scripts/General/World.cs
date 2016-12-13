using System;
using System.Xml;
using Assets.Scripts.AI.Pathfinding;
using Assets.Scripts.Physics;
using UnityEngine;
using Path = System.IO.Path;

namespace Assets.Scripts.General
{
    public class World
    {
        public static World Current { get; private set; }

        public PhysicsWorld PhysicsWorld { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private Tile[,] Tiles { get; set; }

        public NavGraph NavGraph { get; set; }

        public int PlatformCount { get; set; }

        public Vector2 PlayerSpawnPosition
        {
            get { return GameObject.FindGameObjectWithTag("PlayerSpawn").transform.position; }
        }

        public Vector2 AISpawnPosition
        {
            get { return GameObject.FindGameObjectWithTag("AISpawn").transform.position; }
        }

        public Action OnWorldModifyFinishCallback;

        /// <summary>
        /// Creates a new world instance.
        /// </summary>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        public static void Create(int _width = 128, int _height = 128)
        {
            Current = new World();
            Current.Width = _width;
            Current.Height = _height;
            Current.Tiles = new Tile[Current.Width, Current.Height];
            Current.NavGraph = new NavGraph(Current.Width, Current.Height);

            Current.InitWorld();
        }

        /// <summary>
        /// Initialises world with empty tiles.
        /// </summary>
        private void InitWorld()
        {
            for (var x = 0; x < Current.Width; x++)
            {
                for(var y = 0; y < Current.Height; y++)
                {
                    Current.Tiles[x, y] = new Tile(x, y);
                }
            }
        }

        /// <summary>
        /// Initialises the physics world with given gravity vector.
        /// </summary>
        /// <param name="_gravity"></param>
        /// <param name="_solverIterations"></param>
        /// <param name="_maxVelocity"></param>
        public void InitPhysicsWorld(Vector2 _gravity, int _solverIterations, float _maxVelocity)
        {
            Current.PhysicsWorld = new PhysicsWorld();
            Current.PhysicsWorld.Initialize(_gravity, _solverIterations, _maxVelocity);
        }

        /// <summary>
        /// Register a callback function to be called when the world has finished being modified.
        /// </summary>
        /// <param name="_callback"></param>
        public void RegisterWorldModifyFinishCallback(Action _callback)
        {
            Current.OnWorldModifyFinishCallback += _callback;
        }

        /// <summary>
        /// Sets all outer edge tiles of the world to platforms.
        /// </summary>
        public void SetBorderAsPlatform()
        {
            for(var x = 0; x < Current.Width; x++)
            {
                for(var y = 0; y < Current.Height; y++)
                {
                    if(x == 0 || y == 0 || x == Current.Width - 1 || y == Current.Height - 1)
                    {
                        Current.Tiles[x, y].Type = TileType.Platform;
                    }
                }
            }

            if(Current.OnWorldModifyFinishCallback != null)
            {
                Current.OnWorldModifyFinishCallback();
            }
        }

        /// <summary>
        /// Gets the number of tiles in the world.
        /// </summary>
        /// <returns></returns>
        public int GetTileCount()
        {
            return Current.Width * Current.Height;
        }

        /// <summary>
        /// Returns the tile at the given X and Y grid coordinates.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        public Tile GetTileAt(int _x, int _y)
        {
            if(_x < 0 || _x >= Current.Width || _y < 0 || _y >= Current.Height)
            {
                return null;
            }

            return Current.Tiles[_x, _y];
        }

        /// <summary>
        /// Gets the tile at a given world coordinate.
        /// </summary>
        /// <param name="_coord"></param>
        /// <returns></returns>
        public Tile GetTileAtWorldCoord(Vector2 _coord)
        {
            var coord = WorldPointToGridPoint(_coord);
            return GetTileAt((int)coord.x, (int)coord.y);
        }

        /// <summary>
        /// Converts a given world coordinate (unity) to a coordinate in the tile grid.
        /// </summary>
        /// <param name="_world"></param>
        /// <returns></returns>
        public Vector2 WorldPointToGridPoint(Vector2 _world)
        {
            // Add 0.5f offset to account for object pivot points in the center.
            var x = Mathf.FloorToInt(_world.x + 0.5f);
            var y = Mathf.FloorToInt(_world.y + 0.5f);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Returns the amount of tiles matching the given type.
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public int GetTileCountOfType(TileType _type)
        {
            var result = 0;

            for (var x = 0; x < Current.Width; x++)
            {
                for (var y = 0; y < Current.Height; y++)
                {
                    if(Current.Tiles[x, y].Type == _type)
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Clears the current world and resets the world to empty tiles.
        /// </summary>
        public void Clear()
        {
            for (var x = 0; x < Current.Width; x++)
            {
                for (var y = 0; y < Current.Height; y++)
                {
                    Current.Tiles[x, y].Type = TileType.Empty;
                }
            }

            NavGraph.Clear();

            Current.OnWorldModifyFinishCallback();
            Current.PlatformCount = 0;
        }

        /// <summary>
        /// Save world to given save file name.
        /// </summary>
        /// <param name="_saveFile">File name only. E.g. Level1</param>
        /// <returns>Returns if the world was saved to file successfuly or not.</returns>
        public void Save(string _saveFile)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.NewLineOnAttributes = false;

            Directories.CheckDirectories();

            var saveFileLocation = Path.Combine(Directories.Save_Levels_Directory, _saveFile + ".xml");

            //Debug.Log("Saving level to: " + saveFileLocation);

            using (var xmlWriter = XmlWriter.Create(saveFileLocation, settings))
            {
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement("LevelSaveFile");

                xmlWriter.WriteStartElement("LevelData");
                xmlWriter.WriteElementString("LevelName", _saveFile);

                xmlWriter.WriteStartElement("LevelPlayerSpawn");
                xmlWriter.WriteAttributeString("X", PlayerSpawnPosition.x.ToString());
                xmlWriter.WriteAttributeString("Y", PlayerSpawnPosition.y.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("LevelAISpawn");
                xmlWriter.WriteAttributeString("X", AISpawnPosition.x.ToString());
                xmlWriter.WriteAttributeString("Y", AISpawnPosition.y.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("PathfindingSettings");
                xmlWriter.WriteAttributeString("Heuristic", ((int)PathfindingSettings.HeuristicFunction).ToString());
                xmlWriter.WriteAttributeString("FallJumpLinkDistance", PathfindingSettings.FallJumpLinkMaxDist.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("LevelTileData");
                for (var x = 0; x < Current.Width; x++)
                {
                    for (var y = 0; y < Current.Height; y++)
                    {
                        var tile = Current.Tiles[x, y];

                        // Don't waste time writing empty tiles to the file.
                        if (tile.Type == TileType.Empty)
                        {
                            continue;
                        }

                        // Save tile x, y and type to file a long with any other properties unique to the tile.
                        xmlWriter.WriteStartElement("Tile");
                        xmlWriter.WriteAttributeString("TileX", tile.X.ToString());
                        xmlWriter.WriteAttributeString("TileY", tile.Y.ToString());
                        xmlWriter.WriteAttributeString("TileType", tile.Type.ToString());
                        xmlWriter.WriteEndElement();
                    }
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }

            //Debug.Log("Finished saving.");
        }

        /// <summary>
        /// Loads a level from file and initializes the world from its data then returns the level name.
        /// </summary>
        /// <param name="_loadFile">Full file path to level xml save file.</param>
        public string Load(string _loadFile)
        {
            //Debug.Log("Loading level from file: " + _loadFile);
            var levelName = "unknown";
            Current.Clear();

            using (var xmlReader = XmlReader.Create(_loadFile))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.IsStartElement())
                    {
                        switch (xmlReader.Name)
                        {
                            case "LevelName":
                                xmlReader.Read();
                                levelName = xmlReader.Value;
                                break;

                            case "LevelPlayerSpawn":
                                var px = int.Parse(xmlReader["X"]);
                                var py = int.Parse(xmlReader["Y"]);
                                GameObject.FindGameObjectWithTag("PlayerSpawn").transform.position = new Vector2(px, py);
                                break;

                            case "LevelAISpawn":
                                var aiX = int.Parse(xmlReader["X"]);
                                var aiY = int.Parse(xmlReader["Y"]);
                                GameObject.FindGameObjectWithTag("AISpawn").transform.position = new Vector2(aiX, aiY);
                                break;

                            case "PathfindingSettings":
                                PathfindingSettings.SetHeuristicFromID(int.Parse(xmlReader["Heuristic"]));
                                PathfindingSettings.FallJumpLinkMaxDist = int.Parse(xmlReader["FallJumpLinkDistance"]);
                                break;

                            case "Tile":
                                var x = int.Parse(xmlReader["TileX"]);
                                var y = int.Parse(xmlReader["TileY"]);
                                var type = Tile.GetTypeFromString(xmlReader["TileType"]);
                                Current.Tiles[x, y].Type = type;
                                break;
                        }
                    }
                }
            }

            PlatformCount = GetTileCountOfType(TileType.Platform);
            OnWorldModifyFinishCallback();
            //Debug.Log("Finished loading level: " + levelName);
            return levelName;
        }
    }
}
