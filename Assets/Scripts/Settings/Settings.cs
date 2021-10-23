using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Settings
{
    public static string namePlayer = "Player";
    public static int rating = 1000;
    public static float volume = 1f;
    public static bool firstStart = true;

    public static float gameLaunch = 0;
    public static ResultGame result = ResultGame.Draw;//0-ничья, 1 - победа, 2 - поражение


    public static void SaveSettings()
    {
        SaveSettings save = new SaveSettings();

        save.namePlayer = namePlayer;
        save.rating = rating;
        save.volume = volume;
        save.firstStart = firstStart;

        if (!Directory.Exists(Application.persistentDataPath + "/lc"))
            Directory.CreateDirectory(Application.persistentDataPath + "/lc");
        FileStream stream = new FileStream(Application.persistentDataPath + "/lc/rg.sv", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, save);
        stream.Close();
    }

    public static void LoadSettings()
    {
        if (File.Exists(Application.persistentDataPath + "/lc/rg.sv"))
        {
            FileStream stream = new FileStream(Application.persistentDataPath + "/lc/rg.sv", FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            SaveSettings save = (SaveSettings)formatter.Deserialize(stream);
            try
            {
                namePlayer = save.namePlayer;
                rating = save.rating;
                volume = save.volume;
                firstStart = save.firstStart;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
            finally
            {
                stream.Close();
            }
        }
        else
            SaveSettings();
    }
}

[System.Serializable]
public class SaveSettings
{
    public string namePlayer = "";
    public int rating = 1000;
    public float volume = 1f;
    public bool firstStart = true;
}

public enum ResultGame
{
    Draw,
    Victory,
    Defeat
}
