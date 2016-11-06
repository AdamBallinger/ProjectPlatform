using System.IO;
using System.Xml;
using UnityEngine;

namespace Assets.Scripts.General
{
    public class World
    {

        public static World Current { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private Tile[,] Tiles { get; set; }

        public int PlatformCount { get; set; }

        public static void Create(int _width = 128, int _height = 128)
        {
            Current = new World();
            Current.Width = _width;
            Current.Height = _height;
            Current.Tiles = new Tile[Current.Width, Current.Height];

            Current.InitWorld();
        }

        private void InitWorld()
        {
            for(var x = 0; x < Current.Width; x++)
            {
                for(var y = 0; y < Current.Height; y++)
                {
                    Current.Tiles[x, y] = new Tile(x, y);
                }
            }
        }

        public int GetTileCount()
        {
            return Current.Width * Current.Height;
        }

        public Tile GetTileAt(int _x, int _y)
        {
            if(_x < 0 || _x > Current.Width || _y < 0 || _y > Current.Height)
            {
                return null;
            }

            return Tiles[_x, _y];
        }

        public Tile GetTileAtWorldCoord(Vector2 _coord)
        {
            var x = Mathf.FloorToInt(_coord.x + 0.5f);
            var y = Mathf.FloorToInt(_coord.y + 0.5f);
            return GetTileAt(x, y);
        }

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

        public void Clear()
        {
            for (var x = 0; x < Current.Width; x++)
            {
                for (var y = 0; y < Current.Height; y++)
                {
                    Current.Tiles[x, y].Type = TileType.Empty;
                }
            }
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

            var saveFileLocation = Path.Combine(Application.persistentDataPath, "Save_Levels");

            if (!Directory.Exists(saveFileLocation))
            {
                Directory.CreateDirectory(saveFileLocation);
            }

            saveFileLocation = Path.Combine(saveFileLocation, _saveFile + ".xml");

            Debug.Log("Saving level to: " + saveFileLocation);

            using (var xmlWriter = XmlWriter.Create(saveFileLocation, settings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("LevelSaveFile");

                xmlWriter.WriteStartElement("LevelData");
                xmlWriter.WriteElementString("LevelName", _saveFile);
                xmlWriter.WriteElementString("LevelWidth", Current.Width.ToString());
                xmlWriter.WriteElementString("LevelHeight", Current.Height.ToString());
                xmlWriter.WriteElementString("LevelPlatformCount", PlatformCount.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("LevelTileData");
                for (var x = 0; x < Current.Width; x++)
                {
                    for (var y = 0; y < Current.Height; y++)
                    {
                        var tile = Current.Tiles[x, y];
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

            Debug.Log("Finished saving.");
        }

        /// <summary>
        /// Loads a level from file and initializes the world from its data then returns the level name.
        /// </summary>
        /// <param name="_loadFile">Full file path to level xml save file.</param>
        public string Load(string _loadFile)
        {
            Debug.Log("Loading level from file: " + _loadFile);
            var levelName = "unknown";
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

                            case "LevelWidth":
                                xmlReader.Read();
                                Current.Width = int.Parse(xmlReader.Value);
                                break;

                            case "LevelHeight":
                                xmlReader.Read();
                                Current.Height = int.Parse(xmlReader.Value);
                                break;

                            case "LevelPlatformCount":
                                xmlReader.Read();
                                Current.PlatformCount = int.Parse(xmlReader.Value);
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

            Debug.Log("Finished loading level: " + levelName);
            return levelName;
        }
    }
}
