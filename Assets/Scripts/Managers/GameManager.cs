using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager
{
    private string jsonFilePath;

    // Constructeur : tu passes le chemin depuis DirectoryManager
    public GameManager(string filePath)
    {
        jsonFilePath = filePath;
    }

    // Charger la liste des jeux depuis le fichier JSON
    public GameList LoadGameList()
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            return JsonUtility.FromJson<GameList>(json);
        }
        else
        {
            Debug.LogWarning("Aucun fichier games.json trouvé.");
            return new GameList(); // Retourne une liste vide
        }
    }

    // Détecter les jeux dans les répertoires
    public void DetectGames(List<string> gameDirectories)
    {
        GameList gameList = new GameList();

        foreach (string dir in gameDirectories)
        {
            // Si le répertoire contient "steam", vérifier et chercher dans "steamapps\common"
            if (dir.ToLower().EndsWith("steam"))
            {
                string steamCommonPath = Path.Combine(dir, "steamapps", "common");
                if (Directory.Exists(steamCommonPath)) // Vérifier si le dossier "common" existe
                {
                    string[] steamGames = Directory.GetDirectories(steamCommonPath); // Récupérer tous les jeux dans "common"
                    
                    foreach (string gameDir in steamGames)
                    {
                        string[] exeFiles = Directory.GetFiles(gameDir, "*.exe", SearchOption.TopDirectoryOnly);

                        if (exeFiles.Length > 0)
                        {
                            string gameName = Path.GetFileName(gameDir);
                            Debug.Log("Jeu Steam détecté : " + gameName);

                            gameList.games.Add(new GameInfo { name = gameName, exePath = exeFiles[0] });
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Le répertoire Steam 'common' n'existe pas : " + steamCommonPath);
                }
            }
            else
            {
                // Pour les autres répertoires de jeux, suivre la logique actuelle
                string[] subDirs = Directory.GetDirectories(dir);

                foreach (string subDir in subDirs)
                {
                    string[] exeFiles = Directory.GetFiles(subDir, "*.exe", SearchOption.TopDirectoryOnly);

                    if (exeFiles.Length > 0)
                    {
                        string gameName = Path.GetFileName(subDir);
                        Debug.Log("Jeu détecté : " + gameName);

                        gameList.games.Add(new GameInfo { name = gameName, exePath = exeFiles[0] });
                    }
                }
            }
        }

        // Trier les jeux par nom avant sauvegarde
        gameList.games.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.OrdinalIgnoreCase));

        // Sauvegarde la liste des jeux dans un fichier JSON
        SaveGameList(gameList);
    }

    // Sauvegarder la liste des jeux dans un fichier JSON
    public void SaveGameList(GameList gameList)
    {
        if (string.IsNullOrEmpty(jsonFilePath))
        {
            Debug.LogError("Chemin du fichier JSON invalide !");
            return;
        }

        string json = JsonUtility.ToJson(gameList, true);
        File.WriteAllText(jsonFilePath, json);
        Debug.Log("Fichier games.json généré !");
    }
}
