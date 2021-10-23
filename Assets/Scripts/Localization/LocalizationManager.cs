using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using UnityEngine;

public static class LocalizationManager
{
    public static event Action LocalizationChanged = () => { };//событие смены языка

    private static Dictionary<string, Dictionary<string, string>> Dictionary = new Dictionary<string, Dictionary<string, string>>();
    private static string _language = "Russia";

    public static SaveLocalizationText LocalizationText = new SaveLocalizationText();
    public static List<NameDictionary> LocalText = new List<NameDictionary>();
    public static bool loadLocalization;//если true - файлы загружены с интернета

    public static string Language //наименование языка
    {
        get
        {
            return _language;
        }
        set
        {
            _language = Application.platform == RuntimePlatform.WebGLPlayer ? "English" : value;
            LocalizationChanged();
            SaveDictionary();
        }
    }

    public static IEnumerator ReadResourseBackGround(string path = "Localization") //обработка перевода при первом запуске игры
    {
        Debug.Log("Read Resourse Background Start");

        ProgressLoading progress = new ProgressLoading();
        LoadScene.loadScene.ExpectationLoading.Add(progress);

        //Dictionary = new Dictionary<string, Dictionary<string, string>>();

        float deltaTimes = 0.01f;

        float times = Time.realtimeSinceStartup;
        yield return null;

        var textAssets = Resources.LoadAll<TextAsset>(path);

        //LocalText = new List<NameDictionary>();

        float countAssets = textAssets.Length;

        foreach (var textAsset in textAssets)
        {
            NameDictionary dictionary = new NameDictionary();
            dictionary.name = textAsset.name;

            var text = textAsset.text.Replace("\"\"", "[quotes]");//ReplaceMarkers(textAsset.text).Replace("\"\"", "[quotes]");//меняет "" на [quotes]
            var matches = Regex.Matches(text, "\"[\\s\\S]+?\""); //находит области, находящиеся в ковычках для их обработки

            foreach (Match match in matches)//то, что находится в ковычках, меняет
            {
                text = text.Replace(match.Value, match.Value.Replace("\"", null).Replace(",", "[comma]").Replace("\n", "||"));

                if (Time.realtimeSinceStartup - times > deltaTimes)
                {
                    times = Time.realtimeSinceStartup;
                    yield return null;
                }
            }

            progress.progress += 0.2f / countAssets;

            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);//разделяет на строки 

            var languages = lines[0].Split(',').Select(i => i.Trim()).ToList();//разделяет 0 строку на части (названия языков)

            for (var i = 1; i < languages.Count; i++) //сохранение названия языка в массив 
            {

                LangDictionary lang = new LangDictionary();
                lang.lang = languages[i];
                dictionary.lang.Add(lang);
                if (!Dictionary.ContainsKey(languages[i]))
                    Dictionary.Add(languages[i], new Dictionary<string, string>());
            }

            progress.progress += 0.1f / countAssets;

            if (Time.realtimeSinceStartup - times > deltaTimes)
            {
                times = Time.realtimeSinceStartup;
                yield return null;
            }

            for (var i = 1; i < lines.Length; i++)//разделяем строку на части по переводам и сохраняем  в массив
            {
                string line = lines[i];
                var columns = line.Split(',').Select(j => j.Trim()).Select(k => k.Replace("[comma]", ",").Replace("||", "\n").Replace("[quotes]", "\"")).ToList();//разделяет i строку на части (по переводам)
                var key = columns[0];//название ключа фразы

                if (key == "") continue;

                if (columns.Count >= languages.Count)// && !Dictionary[languages[j]].ContainsKey(key))
                {
                    for (var j = 1; j < languages.Count; j++)
                    {
                        KeyDictionary textDictionary = new KeyDictionary();
                        textDictionary.key = key;
                        textDictionary.text = columns[j];
                        dictionary.lang[j - 1].key.Add(textDictionary);

                        try
                        {
                            Dictionary[languages[j]].Add(key, columns[j]);
                        }
                        catch (System.Exception e)
                        {
                            Debug.Log(e.Message);
                        }
                    }
                }
                else
                {
                    if (i + 1 < lines.Length)
                    {
                        lines[i + 1] = lines[i] + lines[i + 1];
                    }
                    Debug.Log("Error " + key);
                }

                progress.progress += (0.7f / countAssets) / (lines.Length - 1);

                if (Time.realtimeSinceStartup - times > deltaTimes)
                {
                    times = Time.realtimeSinceStartup;
                    yield return null;
                }
            }
            LocalText.Add(dictionary);
        }

        LoadScene.loadScene.ExpectationLoading.Remove(progress);
        SaveDictionary();

        Debug.Log("Read Resourse Background End");
    }

    public static IEnumerator ReedBackground() //обработка перевода текста с интернета
    {
        Debug.Log("Reed Background Start");

        float deltaTimes = 0.02f;

        float times = Time.realtimeSinceStartup;
        yield return null;

        List<NameDictionary> localText = new List<NameDictionary>();
        Dictionary<string, Dictionary<string, string>> Dic = new Dictionary<string, Dictionary<string, string>>();

        for (int d = 0; d < LocalizationText.name.Count; d++)
        {
            NameDictionary dictionary = new NameDictionary();
            string textAsset = LocalizationText.text[d];
            dictionary.name = LocalizationText.name[d];

            var text = textAsset.Replace("\"\"", "[quotes]");//меняет "" на [quotes]
            var matches = Regex.Matches(text, "\"[\\s\\S]+?\""); //поделить области, находящиеся в ковычках для их обработки

            foreach (Match match in matches)//то, что находится в ковычках, меняет
            {
                text = text.Replace(match.Value, match.Value.Replace("\"", null).Replace(",", "[comma]").Replace("\n", "||"));

                if (Time.realtimeSinceStartup - times > deltaTimes)
                {
                    times = Time.realtimeSinceStartup;
                    yield return null;
                }
            }

            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);//разделяет на строки 
            var languages = lines[0].Split(',').Select(i => i.Trim()).ToList();//разделяет 0 строку на части (названия языков)

            for (var i = 1; i < languages.Count; i++) //сохранение названия языка в массив 
            {
                LangDictionary lang = new LangDictionary();
                lang.lang = languages[i];
                dictionary.lang.Add(lang);

                if (Dictionary.ContainsKey(languages[i]))
                    if (!Dic.ContainsKey(languages[i]))
                        Dic.Add(languages[i], new Dictionary<string, string>());
            }

            if (Time.realtimeSinceStartup - times > deltaTimes)
            {
                times = Time.realtimeSinceStartup;
                yield return null;
            }

            for (var i = 1; i < lines.Length; i++)//разделяем строку на части по переводам и сохраняем  в массив
            {
                string line = lines[i];
                var columns = line.Split(',').Select(j => j.Trim()).Select(j => j.Replace("[comma]", ",").Replace("||", "\n").Replace("[quotes]", "\"")).ToList();//разделяет i строку на части (по переводам
                var key = columns[0];//название ключа фразы

                if (key == "") continue;

                if (columns.Count >= languages.Count)//проверка на наличие одинаковых ключей !Dictionary[languages[j]].ContainsKey(key)
                {
                    for (var j = 1; j < languages.Count; j++)
                    {

                        KeyDictionary textDictionary = new KeyDictionary();
                        textDictionary.key = columns[0];
                        textDictionary.text = columns[j];
                        dictionary.lang[j - 1].key.Add(textDictionary);

                        if (Dictionary.ContainsKey(languages[j]))
                            if (Dic.ContainsKey(languages[j]))
                                try
                                {
                                    Dic[languages[j]].Add(key, columns[j]);
                                }
                                catch (System.Exception e)
                                {
                                    Debug.Log(e.Message);
                                }
                    }
                }
                else
                {
                    if (i + 1 < lines.Length)
                    {
                        lines[i + 1] = lines[i] + lines[i + 1];
                    }
                    Debug.Log("Error " + key);
                }


                if (Time.realtimeSinceStartup - times > deltaTimes)
                {
                    times = Time.realtimeSinceStartup;
                    yield return null;
                }
            }
            localText.Add(dictionary);
        }

        for (int i = 0; i < localText.Count; i++)
        {
            for (int j = 0; j < LocalText.Count; j++)
            {
                if (localText[i].name == LocalText[j].name)
                {
                    LocalText[j] = localText[i];
                }
            }
        }

        //при окончании загрузки производится сохранение текста и его обновление с помощью смены языка
        SaveDictionary();
        Dictionary = Dic;
        Language = Language;

        Debug.Log("Reed Background End");
    }

    public static void ReadResourse(string path = "Localization") //обработка перевода при отсутствии Dictionary (крайний случай, если не был загружен язык с сохранения)
    {
        Debug.Log("Read Resourse");

        Dictionary = new Dictionary<string, Dictionary<string, string>>();

        LocalText = new List<NameDictionary>();
        var textAssets = Resources.LoadAll<TextAsset>(path);

        foreach (var textAsset in textAssets)
        {
            MonoBehaviour.print(textAsset.name);
            NameDictionary dictionary = new NameDictionary();
            dictionary.name = textAsset.name;

            var text = textAsset.text.Replace("\"\"", "[quotes]");//меняет "" на [quotes]
            var matches = Regex.Matches(text, "\"[\\s\\S]+?\""); //поделить области, находящиеся в ковычках для их обработки
            foreach (Match match in matches)//то, что находится в ковычках, меняет
            {
                text = text.Replace(match.Value, match.Value.Replace("\"", null).Replace(",", "[comma]").Replace("\n", "||"));
            }

            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);//разделяет на строки 

            var languages = lines[0].Split(',').Select(i => i.Trim()).ToList();//разделяет 0 строку на части (названия языков)


            for (var i = 1; i < languages.Count; i++) //сохранение названия языка в массив 
            {

                LangDictionary lang = new LangDictionary();
                lang.lang = languages[i];
                dictionary.lang.Add(lang);
                if (!Dictionary.ContainsKey(languages[i]))
                    Dictionary.Add(languages[i], new Dictionary<string, string>());
            }

            for (var i = 1; i < lines.Length; i++)//разделяем строку на части по переводам и сохраняем  в массив
            {
                string line = lines[i];
                var columns = line.Split(',').Select(j => j.Trim()).Select(k => k.Replace("[comma]", ",").Replace("||", "\n").Replace("[quotes]", "\"")).ToList();//разделяет i строку на части (по переводам
                var key = columns[0];//название ключа фразы


                if (key == "") continue;
                if (columns.Count >= languages.Count)// && !Dictionary[languages[j]].ContainsKey(key))
                {
                    for (var j = 1; j < languages.Count; j++)
                    {

                        KeyDictionary textDictionary = new KeyDictionary();
                        textDictionary.key = key;
                        textDictionary.text = columns[j];
                        dictionary.lang[j - 1].key.Add(textDictionary);

                        try
                        {
                            Dictionary[languages[j]].Add(key, columns[j]);
                        }
                        catch (System.Exception e)
                        {
                            Debug.Log(e.Message);
                        }
                    }
                }
                else
                {
                    if (i + 1 < lines.Length)
                    {
                        lines[i + 1] = lines[i] + lines[i + 1];
                    }
                    Debug.Log("Error " + key);
                }
            }
            LocalText.Add(dictionary);
        }

        SaveDictionary();
    }

    public static void ReadDictionary(string langLoad = "All") //Чтение перевода указанного языка c сохранения 
    {
        Debug.Log("Read Dictionary " + langLoad);

        foreach (var textAsset in LocalText)
        {
            var languages = textAsset.lang;

            for (int j = 0; j < languages.Count; j++) //сохранение названия языка в массив 
            {
                if (langLoad == languages[j].lang || langLoad == "All")
                {
                    if (!Dictionary.ContainsKey(languages[j].lang))
                    {
                        Dictionary.Add(languages[j].lang, new Dictionary<string, string>());
                    }

                    for (int i = 0; i < languages[j].key.Count; i++)
                    {
                        try
                        {
                            Dictionary[languages[j].lang].Add(languages[j].key[i].key, languages[j].key[i].text);
                        }
                        catch (System.Exception e)
                        {
                            Debug.Log(e.Message);
                        }
                    }
                }
            }
        }
    }

    public static string Localize(string localizationKey) //возврат текста
    {
        try
        {
            string lang = Language;
            if (Dictionary.Count == 0)
            {
                LoadDictionary();
            }

            if (!Dictionary.ContainsKey(lang)) ReadDictionary(lang);
            if (!Dictionary.ContainsKey(lang))
            {
                lang = "English";
                ReadDictionary(lang);
            }

            if (!Dictionary[lang].ContainsKey(localizationKey)) throw new KeyNotFoundException("Translation not found: " + localizationKey);

            var missed = !Dictionary[lang].ContainsKey(localizationKey) || string.IsNullOrEmpty(Dictionary[lang][localizationKey]);

            if (missed)
            {
                Debug.LogFormat("Translation not found: {localizationKey} ({0}).", lang);

                return Dictionary["English"].ContainsKey(localizationKey) ? Dictionary["English"][localizationKey] : "";
            }
            return Dictionary[lang][localizationKey];
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return "";
        }
    }

    public static string NumberString(string localizationKey) //возврат текста для чисел
    {
        try
        {
            if (Dictionary.Count == 0)
            {
                LoadDictionary();
            }

            if (!Dictionary.ContainsKey("Numbers")) ReadDictionary("Numbers");
            if (!Dictionary.ContainsKey("Numbers")) throw new KeyNotFoundException("Language not found: " + "Nombers");

            if (!Dictionary["Numbers"].ContainsKey(localizationKey)) throw new KeyNotFoundException("Translation not found: " + localizationKey);

            var missed = !Dictionary["Numbers"].ContainsKey(localizationKey) || string.IsNullOrEmpty(Dictionary["Numbers"][localizationKey]);

            if (missed)
            {
                Debug.LogFormat("Translation not found: {localizationKey} ({0}).", "Numbers");

                return "0";
            }

            return Dictionary["Numbers"][localizationKey];
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return "0";
        }
    }

    public static float NumberFloat(string LocalizationKey)
    {
        try
        {
            return float.Parse(NumberString(LocalizationKey));
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return 0;
        }
    }

    public static int NumberInt(string LocalizationKey)
    {
        try
        {
            return int.Parse(NumberString(LocalizationKey));
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return 0;
        }
    }

    public static string Localize(string localizationKey, params object[] args)//возврат текста с параметрами
    {
        var pattern = Localize(localizationKey);

        return string.Format(pattern, args);
    }

    public static void SaveDictionary()//сохранение перевода
    {
        SaveDictionary save = new SaveDictionary();

        save.versionText = Application.version;
        save.Dictionary = LocalText;

        if (!Directory.Exists(Application.persistentDataPath + "/lc"))
            Directory.CreateDirectory(Application.persistentDataPath + "/lc");
        FileStream stream = new FileStream(Application.persistentDataPath + "/lc/lctd.sv", FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, save);
        stream.Close();
    }

    public static void LoadDictionary()//загрузка перевода при отсутсвии Dictionary (крайний случай)
    {
        if (File.Exists(Application.persistentDataPath + "/lc/lctd.sv"))
        {
            FileStream stream = new FileStream(Application.persistentDataPath + "/lc/lctd.sv", FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            SaveDictionary save = (SaveDictionary)formatter.Deserialize(stream);
            try
            {
                if (LocalizationSync.instance.Sheets.Length != save.Dictionary.Count || Application.version != save.versionText)
                {
                    ReadResourse();
                    Debug.Log("<color=red>Error. Read Resourse</color>");
                }
                else
                {
                    LocalText = save.Dictionary;
                }
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
        {
            ReadResourse();
            Debug.Log("<color=red>Error. Read Resourse</color>");
        }
    }

    public static bool LoadStartDictionary()//возвращает информацию о наличии актуального сохранения
    {
        bool load = true;
        if (File.Exists(Application.persistentDataPath + "/lc/lctd.sv"))
        {
            FileStream stream = new FileStream(Application.persistentDataPath + "/lc/lctd.sv", FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            SaveDictionary save = (SaveDictionary)formatter.Deserialize(stream);
            try
            {
                if (LocalizationSync.instance.Sheets.Length != save.Dictionary.Count || Application.version != save.versionText)
                {
                    load = false;
                }
                else
                {
                    Debug.Log("Read Save");
                    LocalText = save.Dictionary;
                    load = true;
                }
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
        {
            load = false;
        }

        return load;
    }
}

[System.Serializable]
public class SaveLocalizationText
{
    public string versionText = "";
    public List<string> name = new List<string>();
    public List<string> text = new List<string>();
}


[System.Serializable]
public class SaveDictionary
{
    public string versionText = "";
    public List<NameDictionary> Dictionary = new List<NameDictionary>();
}
[System.Serializable]
public class NameDictionary //файл 
{
    public string name;//имя файла
    public List<LangDictionary> lang = new List<LangDictionary>();//массив языков
}
[System.Serializable]
public class LangDictionary
{
    public string lang;//название языка
    public List<KeyDictionary> key = new List<KeyDictionary>();
}
[System.Serializable]
public class KeyDictionary
{
    public string key;//название ключа
    public string text;//текст ключа
}


