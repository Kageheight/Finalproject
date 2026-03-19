using System.Text.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class WhiteListService
{
    private static readonly string FilePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "whitelist.json");

    private List<WhiteItem> _items = new();

    public WhiteListService()
    {
        Load();
    }

    public bool IsWhitelisted(string sha256)
    {
        return _items.Any(x => x.Sha256 == sha256);
    }

    public void Add(string fileName, string sha256)
    {
        if (_items.Any(x => x.Sha256 == sha256))
            return;

        _items.Add(new WhiteItem
        {
            FileName = fileName,
            Sha256 = sha256,
            AddedAt = DateTime.Now,
            Source = "manual"
        });

        Save();
    }

    private void Load()
    {
        if (!File.Exists(FilePath)) return;

        var json = File.ReadAllText(FilePath);
        _items = JsonSerializer.Deserialize<List<WhiteItem>>(json) ?? new();
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }
}

public class WhiteItem
{
    public string FileName { get; set; } = "";
    public string Sha256 { get; set; } = "";
    public DateTime AddedAt { get; set; }
    public string Source { get; set; } = "";
}