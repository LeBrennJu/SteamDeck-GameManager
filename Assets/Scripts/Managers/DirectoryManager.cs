using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DirectoryManager : MonoBehaviour
{
    private GameManager GameManager;
    private List<string> gameDirectories = new List<string>();

    private string gameListPath;

    private void Start()
    {
        ///////////////
        string gameDirectoriesFilePath = Path.Combine(Application.dataPath, "Art/Data", "directoryRoad.json"); // Construit le chemin vers le fichier JSON lié au repertoire CODE EN DUR POUR LE MOMENT :::::: //////
        //////////////
        // gameListPath = Path.Combine(Application.dataPath, "Art/Data", "games.json");// Construit le chemin vers le fichier JSON lié au jeux

        string json = File.ReadAllText(gameDirectoriesFilePath); // Lit le contenu du fichier JSON

        GameDirectories data = JsonUtility.FromJson<GameDirectories>(json); // Désérialise dans ton modèle existant

        GameManager = new GameManager();
// Debug.Log("dans le start di directoryManager "+gameListPath);
        LoadDirectories(data.gameDirectories, "gameDirectories"); // Lance le chargement au démarrage/prend en argument le/les chemins des repertoires de jeux
        GameManager.DetectGames(gameDirectories); 
    }

    // Charger les répertoires depuis la liste donnée
    public void LoadDirectories(List<string> directories, string directoryType)
    {
        gameDirectories.Clear();

        foreach (string directory in directories)
        {
            if (Directory.Exists(directory))
            {
                if (!gameDirectories.Contains(directory))
                {
                    gameDirectories.Add(directory);
                    Debug.Log("Répertoire ajouté : " + directory);
                }
            }
            else
            {
                Debug.LogWarning("Répertoire introuvable : " + directory);
            }
        }

        if (gameDirectories.Count > 0)
            SaveDirectories(gameDirectories, directoryType);
    }
    // Sauvegarder les répertoires dans un fichier JSON selon le type

    public void SaveDirectories(List<string> directories, string directoryType)
    {
        string typePath = Path.Combine(Application.dataPath, "Art/Data", directoryType.ToLower() + ".json");

        GameDirectories newData = new GameDirectories();

        foreach (string dir in directories)
        {
            string folderName = Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            if (folderName.ToLower() == "steam")
            {
                newData.steamDirectories = dir;
            }
            else
            {
                if (!newData.gameDirectories.Contains(dir))
                    newData.gameDirectories.Add(dir);
            }
        }
        if(directoryType!="gameDirectories")
        // Sauvegarde fichier spécifique
        File.WriteAllText(typePath, JsonUtility.ToJson(newData, true));

        // Mise à jour du fichier global
        string globalPath = Path.Combine(Application.dataPath, "Art/Data", "gameDirectories.json");
        GameDirectories allDirs = new GameDirectories();

        if (File.Exists(globalPath))
            allDirs = JsonUtility.FromJson<GameDirectories>(File.ReadAllText(globalPath));

        foreach (string dir in newData.gameDirectories)
        {
            if (!allDirs.gameDirectories.Contains(dir))
                allDirs.gameDirectories.Add(dir);
        }

        if (!string.IsNullOrEmpty(newData.steamDirectories))
            allDirs.steamDirectories = newData.steamDirectories;

        File.WriteAllText(globalPath, JsonUtility.ToJson(allDirs, true));
    }

    public static List<string> GetAllGridDirectories(string steamPath)
    {
        List<string> gridDirs = new List<string>();
        string userdataPath = Path.Combine(steamPath, "userdata");

        if (Directory.Exists(userdataPath))
        {
            string[] userDirs = Directory.GetDirectories(userdataPath);

            foreach (string userDir in userDirs)
            {
                string gridPath = Path.Combine(userDir, "config", "grid");

                if (Directory.Exists(gridPath))
                {
                    gridDirs.Add(gridPath);
                    Debug.Log("Dossier grid trouvé : " + gridPath);
                }
            }
        }
        else
        {
            Debug.LogWarning("Dossier userdata introuvable dans : " + userdataPath);
        }

        return gridDirs;
    }



}
