using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class GameManager:MonoBehaviour
{
    private string jsonFilePath=Path.Combine(Application.dataPath, "Art/Data", "games.json");
    public GameObject gamePrefab;
    public GameObject emptyGamePrefab;
    public Transform gameListContainer;

    private GameList gameList=new GameList();
    public void Start()
    {
        gameList = new GameList();
        Debug.Log("la gamelist est ici "+gameList);
        gameList=LoadGameList();
        CreateGameList();
    }
    // // Constructeur :  passes le chemin depuis DirectoryManager
    public GameManager()
    {
    }

    // Charger la liste des jeux depuis le fichier JSON
    public GameList LoadGameList()
    {
        Debug.Log("dans loadgamelist "+jsonFilePath);
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
        void CreateGameList()
    {
        gameListContainer.name = "GameList"; // Assurer que le nom du conteneur est correct

        for (int i = 0; i < gameList.games.Count; i++)
        {
            if(gameList.games[i].appId != 0){
                // Instancier le jeu à partir du prefab
                Instantiate(gamePrefab, gameListContainer);
            }
            else{
                GameObject game = Instantiate(emptyGamePrefab, gameListContainer);
                game.name = gameList.games[i].name;
                game.GetComponentInChildren<TextMeshProUGUI>().text = game.name;

            }

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
    public void SaveGameList(GameList nouveauxJeux)
    {
        if (string.IsNullOrEmpty(jsonFilePath))
        {
            Debug.LogError("Chemin du fichier JSON invalide !");
            return;
        }

        // Charger la liste existante s’il y en a une
        GameList jeuxExistants = new GameList { games = new List<GameInfo>() };

        if (File.Exists(jsonFilePath))
        {
            string jsonExistant = File.ReadAllText(jsonFilePath);
            jeuxExistants = JsonUtility.FromJson<GameList>(jsonExistant);
        }

        // Vérifie les noms déjà existants
        HashSet<string> nomsExistants = new HashSet<string>();
        foreach (var jeu in jeuxExistants.games)
        {
            nomsExistants.Add(jeu.name.ToLowerInvariant()); // insensible à la casse
        }

        // Ajouter uniquement les jeux nouveaux
        foreach (var jeu in nouveauxJeux.games)
        {
            if (!nomsExistants.Contains(jeu.name.ToLowerInvariant()))
            {
                jeuxExistants.games.Add(jeu);
                Debug.Log($"Ajouté : {jeu.name}");
            }
            else
            {
                Debug.Log($"Ignoré (déjà présent) : {jeu.name}");
            }
        }

        // Réécriture du JSON avec les jeux fusionnés
        string jsonFinal = JsonUtility.ToJson(jeuxExistants, true);
        File.WriteAllText(jsonFilePath, jsonFinal);
        Debug.Log("Fichier games.json mis à jour sans doublons.");
    }


}
