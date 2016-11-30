using UnityEngine;
using System.IO;

namespace Assets.Scripts.General
{
    public class Directories
    {
        /// <summary>
        /// Directory for saved levels XML files to be written to. (Normal save files only contain basic level information, tile positions and tile type data.
        /// </summary>
        public static readonly string Save_Levels_Directory = Path.Combine(Application.persistentDataPath, "Save_Levels");

        /// <summary>
        /// Directory for saved levels data XML files to be written to. (Data files contains tiles physics properties, pickup positions etc.)
        /// </summary>
        public static readonly string Save_Levels_Data_Directory = Path.Combine(Save_Levels_Directory, "Level_Data");

        /// <summary>
        /// Directory for the built-in levels XML files for the game.
        /// </summary>
        public static readonly string Stock_Levels_Directory = Path.Combine(Application.streamingAssetsPath, "Stock_Levels");

        /// <summary>
        /// Directory for the built-in levels data XML files for the game.
        /// </summary>
        public static readonly string Stock_Levels_Data_Directory = Path.Combine(Stock_Levels_Directory, "Level_Data");


        /// <summary>
        /// Checks that the required directories for saveing and loading levels exist, and creates them if they don't.
        /// </summary>
        public static void CheckDirectories()
        {
            if (!Directory.Exists(Save_Levels_Directory))
            {
                Directory.CreateDirectory(Save_Levels_Directory);
            }

            if (!Directory.Exists(Save_Levels_Data_Directory))
            {
                Directory.CreateDirectory(Save_Levels_Data_Directory);
            }
        }
    }
}
