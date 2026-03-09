using System;
using System.IO;
using System.Collections.Generic; // Важно для списка
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace MenuTemplate
{
    public static class SaveSystem
    {
        private static string _folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourGameName");
        private static string _path = Path.Combine(_folder, "YourSaveName.yourfileextensionname");

        public static int Width = 1280;
        public static int Height = 720;

        public static Keys[] Keys = {
            Microsoft.Xna.Framework.Input.Keys.W,
            Microsoft.Xna.Framework.Input.Keys.S,
            Microsoft.Xna.Framework.Input.Keys.A,
            Microsoft.Xna.Framework.Input.Keys.D,
            Microsoft.Xna.Framework.Input.Keys.Space,
            Microsoft.Xna.Framework.Input.Keys.LeftShift,
            Microsoft.Xna.Framework.Input.Keys.X
        };

        public static List<string> UnlockedEndings = new List<string>();

        public static int RegisterEnding(string endingId)
        {
            if (!UnlockedEndings.Contains(endingId))
            {
                UnlockedEndings.Add(endingId);
                Save(Width, Height);
            }
            return UnlockedEndings.Count;
        }

        public static void Save(int width, int height, Keys[] keys = null)
        {
            try
            {
                if (!Directory.Exists(_folder)) Directory.CreateDirectory(_folder);

                Width = width;
                Height = height;
                if (keys != null) Keys = keys;

                string data = $"{Width}x{Height}";
                foreach (var k in Keys) data += "x" + (int)k;

                if (UnlockedEndings.Count > 0)
                {
                    data += "|" + string.Join(",", UnlockedEndings);
                }

                File.WriteAllText(_path, data);
            }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine("Save Error: " + e.Message); }
        }

        public static (int w, int h) Load()
        {
            try
            {
                if (File.Exists(_path))
                {
                    string content = File.ReadAllText(_path);

                    string[] mainParts = content.Split('|');

                    string[] settingsParts = mainParts[0].Split('x');

                    if (settingsParts.Length >= 2)
                    {
                        Width = int.Parse(settingsParts[0]);
                        Height = int.Parse(settingsParts[1]);
                    }

                    if (settingsParts.Length >= 3)
                    {
                        int keysInFile = settingsParts.Length - 2;
                        for (int i = 0; i < keysInFile; i++)
                        {
                            if (i < Keys.Length)
                                Keys[i] = (Keys)int.Parse(settingsParts[i + 2]);
                        }
                    }

                    if (mainParts.Length > 1)
                    {
                        string[] endings = mainParts[1].Split(',');
                        UnlockedEndings = endings.ToList();
                    }

                    return (Width, Height);
                }
            }
            catch { }
            return (1280, 720);
        }
    }
}