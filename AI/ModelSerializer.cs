using System;
using System.IO;
using System.Text.Json;

namespace ARSignTranslator.AI
{
    public static class ModelSerializer
    {
        public static void Save(string path, ModelState state)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            File.WriteAllText(path, json);
        }

        public static ModelState Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Model file not found: " + path);

            var json = File.ReadAllText(path);
            var state = JsonSerializer.Deserialize<ModelState>(json);

            if (state == null)
                throw new Exception("Failed to load model.");

            return state;
        }
    }
}
