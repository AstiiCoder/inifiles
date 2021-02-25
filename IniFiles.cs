using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Reflection;

/// <summary>
/// Класс, для работы с ini-файлами. Для использования, нужно скопировать и подключить в проект
/// Названия методов максимально унифицированы с соответствующими процедурами в Delphi
/// ini-файлы с одинаковыми ключами внутри секции не поддерживаются
/// A.Tsvetkov, 05.01.2021 
/// </summary>
internal class IniFiles
{
    // Класс для работы с ini-файлами

    // Примеры использования:
    // IniFiles ini = new IniFiles(); // файл в директории программы будет создан, если его нет
    // ini.WriteString("Section7", "var_str", "One Dich22"); // запись строки в секцию по ключу
    // IniFiles ini = new IniFiles(@"D:\1.ini");  // используется указанный файл; при повторном чтении из другой части программы нужно создать новый экземпляр класса 
    // TextBox_Content.Text = Convert.ToString(ini.ReadFloat("Section3", "var4", 600)); // чтение строки

    private bool _CreateFile;
    private string _FileName;
    private List<string> _FileContent;
    private static bool _IsWaiting = false;
    private Encoding _Coding = Encoding.Unicode;
    public List<string> _FileAllContent;
    private bool _SectionExists = false;
    private const string CommonSection = "Common";

    /// <summary>
    /// Будет создан файл в директории программы с именем "config.ini".
    /// </summary>
    public IniFiles() : this(Environment.CurrentDirectory + @"\config.ini")
    {

    }

    /// <summary>
    /// Если имя файла не указать, то файл создастся в директории программы с именем "config.ini".
    /// ВНИМАНИЕ: если чтение происходит в разных местах программы, необходимо создавать новые экземпляры объекта, поскольку повторное чтение идёт только из памяти (не из файла)
    /// </summary>
    /// <param name="FileName"></param>
    /// <returns>Требуется создать экземплят класса для чтения из файла или сохранения в файл.</returns>
    public IniFiles(string FileName) : this(FileName, Encoding.Unicode)
    {

    }
    /// <summary>
    /// Если имя файла не указать, то файл создастся в директории программы с именем "config.ini".
    /// ВНИМАНИЕ: если чтение происходит в разных местах программы, необходимо создавать новые экземпляры объекта, поскольку повторное чтение идёт только из памяти (не из файла)
    /// </summary>
    /// <param name="FileName"></param>
    /// <param name="Coding"></param>
    /// <returns>Требуется создать экземплят класса для чтения из файла или сохранения в файл.</returns>
    public IniFiles(string FileName, Encoding Coding)
    {
        _FileName = FileName;
        _Coding = Coding;
        if (System.IO.File.Exists(FileName) == false)
        {
            try
            {
                StreamWriter sw = new StreamWriter(FileName, true, Coding);
                sw.Close();
                _CreateFile = true;
            }
            catch (Exception)
            {
                _CreateFile = false;
            }
        }
        //Список для чтения, без лишних строк
        _FileContent = new List<string>();
        LoadAllFileContent();
    }

    /// <summary>
    /// Инициализация накоплений изменений, то есть сразу в файл ничего сохраняться не будет, пока не будет вызвано Save() 
    /// Всё, что от строки WaitAndNotSave() и до строки Save() будет сохранено за один раз
    /// </summary>
    public void WaitAndNotSave()
    {
        _IsWaiting = true;
        //Список для записи с полным содержимым
        _FileAllContent = new List<string>();
        using (StreamReader sr = File.OpenText(_FileName))
        {
            while (!sr.EndOfStream) _FileAllContent.Add(sr.ReadLine());
        }
    }

    private void LoadAllFileContent()
    {
        using (StreamReader sr = new StreamReader(_FileName, _Coding))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if ((line.Length > 0) && (line[0] != ';')) _FileContent.Add(line);
            }
            // Добавим пустую строку в файл, если он совсем пустой
            if ((!_CreateFile) && (_FileContent.Count == 0)) _FileContent.Add(Environment.NewLine);
        }
    }

    public string ReadAll()
    {
        string combindedString = string.Join(Environment.NewLine, _FileContent);
        return combindedString;
    }

    #region ReadByTypes

    public List<string> ReadSection(string Section)
    {
        bool SectionExists = false;
        List<string> SectionContent = new List<string>();
        for (int i = 0; i < _FileContent.Count; i++)
        {
            // Ищем секцию
            if (_FileContent[i] == '[' + Section + ']') SectionExists = true;
            // Если секция существует
            if ((SectionExists) && (_FileContent[i] != '[' + Section + ']'))
            {
                // Ишем ключи=значения в секции
                if ((_FileContent[i].Length != 0) && (_FileContent[i].Substring(0, 1) != "[") && (i != _FileContent.Count()))
                    SectionContent.Add(_FileContent[i]);
            }
        }
        return SectionContent;
    }

    public List<string> ReadSections()
    {
        List<string> SectionContent = new List<string>();
        string item;
        for (int i = 0; i < _FileContent.Count; i++)
        {
            item = _FileContent[i];
            // Ищем секции
            if ((item.Substring(0, 1) == "[") && (item.Substring(item.Length - 1, 1) == "]")) SectionContent.Add(_FileContent[i]);
        }
        return SectionContent;
    }

    public string ReadString(string Section, string Key, string Default = "")
    {
        bool SectionExists = false;
        string KeyValue = "";
        string[] SecParts;
        for (int i = 0; i < _FileContent.Count; i++)
        {
            // Ищем секцию
            if (_FileContent[i] == '[' + Section + ']') SectionExists = true;
            // Если секция существует и если это не её заголовок, разобьём её каждую строку на массив: один элемент ключ, другой значение
            if ((SectionExists) && (_FileContent[i] != '[' + Section + ']'))
            {
                SecParts = _FileContent[i].Split('=');
                // Должно быть два элемента массива, иначе нужно обрабатывать по-другому
                if (SecParts.Count() < 2)
                {
                    continue;
                }
                else if (SecParts.Count() > 2)
                {
                    string s = _FileContent[i].Replace(SecParts[0] + '=', "");
                    SecParts[1] = s;
                }
                // Ишем значение в секции
                if ((SecParts[0] == Key) && (_FileContent[i].Length > 0) && (_FileContent[i][0] != '['))
                {
                    KeyValue = SecParts[1];
                    break;
                }
            }
        }

        if ((KeyValue == "") || (!SectionExists)) KeyValue = Default;
        return KeyValue;
    }

    /// <summary>
    /// Сокращённая форма чтения значения. Читать можно прямо в тот тип, которому соответствует значение. Для определения типа идёт перебор методов преобразования.
    /// Если тип значения может быть определен неоднозначно, необходимо использовать методы (ReadInteger, ReadBool и т.д.)
    ///   int X = ini.Read("X");
    ///   bool B = ini.Read("IsIamBest");
    /// </summary>
    public dynamic Read(string Key, string Default = "")
    {
        string s = ReadString(CommonSection, Key, Default);
        if (int.TryParse(s, out int IntValue)) return IntValue;
        else if (long.TryParse(s, out long LongValue)) return LongValue;
        else if (bool.TryParse(s, out bool BoolValue)) return BoolValue;
        else if (DateTime.TryParse(s, out DateTime DateTimeValue)) return DateTimeValue;
        else if (float.TryParse(s, out float FloatValue)) return FloatValue;
        else return s;
    }

    public int ReadInteger(string Section, string Key, int Default = 0)
    {
        string IntStr = ReadString(Section, Key);
        if (int.TryParse(IntStr, out int KeyValue))
            return KeyValue;
        else
            return Default;
    }

    public int ReadInt(string Section, string Key, int Default = 0)
    {
        return ReadInteger(Section, Key, Default);
    }

    public long ReadLong(string Section, string Key, long Default = 0)
    {
        string IntStr = ReadString(Section, Key);
        if (long.TryParse(IntStr, out long KeyValue))
            return KeyValue;
        else
            return Default;
    }

    public bool ReadBool(string Section, string Key, bool Default = false)
    {
        string IntStr = ReadString(Section, Key);
        if (bool.TryParse(IntStr, out bool KeyValue))
            return KeyValue;
        else
            return Default;
    }

    public DateTime ReadDate(string Section, string Key, DateTime Default = default(DateTime))
    {
        string IntStr = ReadString(Section, Key);
        if (DateTime.TryParse(IntStr, out DateTime KeyValue))
            return KeyValue;
        else
            return Default;
    }

    public string ReadDateAsString(string Section, string Key, DateTime Default = default(DateTime))
    {
        string Str = ReadString(Section, Key);
        if (Str.Length >= 10)
            return Str.Substring(0, 10);
        else if ((Default.ToShortDateString()).Length >= 10)
            return (Default.ToShortDateString()).Substring(0, 10);
        else
            return (default(DateTime).ToShortDateString()).Substring(0, 10);
    }

    public DateTime ReadDateTime(string Section, string Key, DateTime Default = default(DateTime))
    {
        string IntStr = ReadString(Section, Key);
        if (DateTime.TryParse(IntStr, out DateTime KeyValue))
            return KeyValue;
        else
            return Default;
    }

    public float ReadFloat(string Section, string Key, float Default = 0)
    {
        string IntStr = ReadString(Section, Key);
        if (float.TryParse(IntStr, out float KeyValue))
            return KeyValue;
        else
            return Default;
    }

    public MemoryStream ReadBinaryStream(string Section, string Key, MemoryStream Value)
    {
        string StringFromFile = ReadString(Section, Key);
        byte[] byteArray = _Coding.GetBytes(StringFromFile);
        MemoryStream stream = new MemoryStream(byteArray);
        return stream;
    }

    public System.Windows.Rect ReadRect(string Section, string Key)
    {
        string IntStr = ReadString(Section, Key);
        try
        {
            return System.Windows.Rect.Parse(IntStr);
        }
        catch
        {
            return System.Windows.Rect.Parse("0,0,0,0"); 
        }
    }

    #endregion ReadByTypes

    #region WriteByTypes

    /// <summary>
    /// Сохранение строки в файл в указанную секцию с указанным ключом. Если указанного ключа в секции нет, то он будет создан, если есть - будет изменено значение
    /// </summary>
    /// <param name="Section">Секция (имя секции), содержащая ключ и его значение</param>
    /// <param name="Key">Ключ (имя ключа), в данной секции, значение которого нужно сохранить</param>
    /// <param name="Value">Значение, соответствующее ключу в виде строки</param>
    public void WriteString(string Section, string Key, string Value)
    {
        int KeyLength = Key.Length;
        if (_FileAllContent == null)
        {
            _FileAllContent = new List<string>();
            // Загружаем всё содержимое файла, ради одного исправления
            using (StreamReader sr = File.OpenText(_FileName))
            {
                while (!sr.EndOfStream) _FileAllContent.Add(sr.ReadLine());
            }
        }
        else if (!_IsWaiting)
        {
            _FileAllContent.Clear();
            // Загружаем всё содержимое файла, ради одного исправления
            using (StreamReader sr = File.OpenText(_FileName))
            {
                while (!sr.EndOfStream) _FileAllContent.Add(sr.ReadLine());
            }
        }

        if (_IsWaiting) _SectionExists = false;
        for (int i = 0; i < _FileAllContent.Count; i++)
        {
            // Ищем секцию
            if (_FileAllContent[i] == '[' + Section + ']')
            {
                _SectionExists = true;
                break;
            }
        }
        // Если секции не было никогда, то просто дописываем в конец.
        if ((!_SectionExists) && (!_IsWaiting))
        {
            string s = Environment.NewLine + Environment.NewLine + '[' + Section + ']' + Environment.NewLine + Key + '=' + Value;
            System.IO.StreamWriter writer = new System.IO.StreamWriter(_FileName, true, _Coding);
            writer.WriteLine(s);
            writer.Close();
            _FileAllContent.Add(s);
            _SectionExists = true; // Теперь секция есть
        }
        // Если секции не было, то добавляем в список строк в конец.
        else if ((!_SectionExists) && (_IsWaiting))
        {
            _FileAllContent.Add(String.Empty);
            _FileAllContent.Add('[' + Section + ']');
            _FileAllContent.Add(Key + '=' + Value);
            _SectionExists = true; // Теперь секция есть
        }
        // Если секция есть, то нужно поменять значение по ключу
        else if (_SectionExists)
        {
            // Ищем секцию, так как проверили, что она есть
            bool WeInSection = false;
            bool NeedSave = false;
            int NeedInsertIndex = 0;
            for (int i = 0; i < _FileAllContent.Count(); i++)
            {
                if (_FileAllContent[i] == '[' + Section + ']')
                {
                    WeInSection = true;
                    continue;
                }
                if (WeInSection)
                {
                    if ((_FileAllContent[i].Length > KeyLength) && (_FileAllContent[i].Substring(0, KeyLength) == Key))
                    {
                        NeedSave = true;
                        _FileAllContent[i] = Key + '=' + Value;
                        break;
                    }
                    if ((_FileAllContent[i].Length == 0) || (_FileAllContent[i].Substring(0, 1) == "[") || (i == _FileAllContent.Count() - 1))  // не нашли в секции этого ключа
                    {
                        NeedInsertIndex = _FileAllContent.IndexOf('[' + Section + ']') + 1;
                        NeedSave = true;
                        break;
                    }
                }

            }
            if (NeedInsertIndex != 0) _FileAllContent.Insert(NeedInsertIndex, Key + '=' + Value);
            // Сохраняем всё содержимое с изменением ключа или значения по нему
            if ((NeedSave) && (!_IsWaiting)) Save();
        }
    }

    /// <summary>
    /// Сохранение всех изменений от момента инициализации (всё, что после строки WaitAndNotSave() и до строки Save())
    /// </summary>
    public void Save()
    {
        using (StreamWriter sr = new StreamWriter(_FileName, false, _Coding))
        {
            foreach (string item in _FileAllContent)
            {
                sr.WriteLine(item);
            }
        }
        _IsWaiting = false;
    }

    /// <summary>
    /// Сокращённая форма сохранения значения типа "строка". Все остальные типы данных необходимо конвертировать в строку 
    /// для использования данного метода или использовать соответствующий метод для работы с данным типом (WriteInteger, WriteBool и т.д.)
    /// </summary>
    public void Write(string Key, string Value)
    {
        WriteString(CommonSection, Key, Value);
    }

    public void WriteInteger(string Section, string Key, int Value)
    {
        WriteString(Section, Key, Convert.ToString(Value));
    }

    public void WriteInt(string Section, string Key, int Value)
    {
        WriteString(Section, Key, Convert.ToString(Value));
    }

    public void WriteLong(string Section, string Key, long Value)
    {
        WriteString(Section, Key, Convert.ToString(Value));
    }

    public void WriteBool(string Section, string Key, bool Value)
    {
        WriteString(Section, Key, Convert.ToString(Value));
    }

    public void WriteDate(string Section, string Key, DateTime Value)
    {
        WriteString(Section, Key, Value.ToShortDateString());
    }

    public void WriteDateTime(string Section, string Key, DateTime Value)
    {
        WriteString(Section, Key, Convert.ToString(Value));
    }

    public void WriteFloat(string Section, string Key, float Value)
    {
        WriteString(Section, Key, Convert.ToString(Value));
    }

    public void WriteBinaryStream(string Section, string Key, MemoryStream Value)
    {
        string StringFromStream = "";
        using (var sr = new StreamReader(Value))
        {
            StringFromStream = sr.ReadToEnd();
        }
        WriteString(Section, Key, Convert.ToString(StringFromStream));
    }

    public void WriteRect(string Section, string Key, System.Windows.Rect Value)
    {
        WriteString(Section, Key, Convert.ToString(Value));
    }

    #endregion WriteByTypes

    public bool SectionExists(string Section)
    {
        bool _SectionExists = false;
        for (int i = 0; i < _FileContent.Count; i++)
        {
            // Ищем секцию
            if (_FileContent[i] == '[' + Section + ']')
            {
                _SectionExists = true;
                break;
            }
        }
        return _SectionExists;
    }

    public bool KeyExists(string Section, string Key)
    {
        bool _SectionExists = false;
        bool _KeyExists = false;
        int KeyLength = Key.Length;
        for (int i = 0; i < _FileContent.Count; i++)
        {

            // Ищем секцию
            if (_FileContent[i] == '[' + Section + ']')
            {
                _SectionExists = true;  
                continue;
            }
            
            // Ищем ключ
            if (_SectionExists)
            {
                if ((_FileContent[i].Length == 0) || (_FileContent[i].Substring(0, 1) == "[") || (i == _FileContent.Count() - 1))  // не нашли в секции этого ключа
                {
                    break;
                }
                else
                {
                    if ((_FileContent[i].Length > KeyLength) && (_FileContent[i].Substring(0, KeyLength) == Key))
                    {
                        _KeyExists = true;
                        break;
                    }

                }
            }
        }
        return _KeyExists;
    }

    public void EraseSection(string Section)
    {
        if (_IsWaiting) return;
        bool _SectionExists = false;
        bool NeedSave = false;

        if (_FileAllContent == null) _FileAllContent = new List<string>();
        else _FileAllContent.Clear();
        // Загружаем всё содержимое файла
        using (StreamReader sr = File.OpenText(_FileName))
        {
            while (!sr.EndOfStream) _FileAllContent.Add(sr.ReadLine());
        }
        // Ищем секцию: с какой строки она начинается и какой заканчивается
        for (int i = 0; i < _FileAllContent.Count; i++)
        {
            
            if (_FileAllContent[i] == '[' + Section + ']')
            {
                _SectionExists = true;
                _FileAllContent[i] = "-";
            }
            if (_SectionExists)
            {
                if ((_FileAllContent[i].Length == 0) || (_FileAllContent[i].Substring(0, 1) == "[") || (i == _FileAllContent.Count() - 1))  // не нашли в секции этого ключа
                {
                    NeedSave = true;
                    break;
                }
                else
                {
                    _FileAllContent[i] = "-";
                }
            }
        }
        if (!_SectionExists) return;
        _FileAllContent = _FileAllContent.Where(x => x != "-").ToList();

        if (NeedSave) Save();
    }

}

public class IniFilesExt
{
    /// <summary>
    /// Сохранение нескольких значений переменных (в том числе разного типа) в файл (с именем по умолчанию) одной строкой, без создания экземпляра класса
    /// Пример: int i = 1; string j = "hello"; bool X = false;
    ///         IniFilesExt.WriteToIni(i, j, X);
    /// </summary>
    /// <param name="parameters">Набор переменных через запятую</param>
    public static void WriteToIni(params object[] parameters)
    {
        // Обращаемся к первому фрейму текущего стека вызовов.
        StackFrame frame = new StackTrace(true).GetFrame(1);
        // Именно в этой строке в оригинальном файле есть имена переменных
        string LineOfCode = File.ReadAllLines(frame.GetFileName())[frame.GetFileLineNumber() - 1].Trim();
        // Непосредственно, получаем массив имён переменных
        string[] VarNames = Regex.Match(LineOfCode, @"\((.+?)\)").Groups[1].Value.Split(',').Select(x => x.Trim()).ToArray();
        // Сохранение всех переменных в файл по именам и соответствующим значениям.
        IniFiles ini = new IniFiles();
        ini.WaitAndNotSave();
        for (int i = 0; i < parameters.Length; i++)
        {
            ini.Write(VarNames[i], parameters[i].ToString());
        }
        ini.Save();
    }

    /// <summary>
    /// Получение нескольких значений переменных (в том числе разного типа) из файла (с именем по умолчанию) одной строкой
    /// Можно использовать этот метод без создания экземпляра класса, если скопировать его в нужные cs-файлики.
    /// Пример: int i; string j; bool X;
    ///         ReadFromIni(i, j, X);
    /// </summary>
    /// <param name="parameters">Набор переменных через запятую</param>
    public void ReadFromIni(params object[] parameters)
    {
        string v = "";
        // Обращаемся к первому фрейму текущего стека вызовов.
        StackFrame frame = new StackTrace(true).GetFrame(1);
        // Именно в этой строке в оригинальном файле есть имена переменных
        string LineOfCode = File.ReadAllLines(frame.GetFileName())[frame.GetFileLineNumber() - 1].Trim();
        // Непосредственно, получаем массив имён переменных
        string[] VarNames = Regex.Match(LineOfCode, @"\((.+?)\)").Groups[1].Value.Split(',').Select(x => x.Trim()).ToArray();
        // Заполним список значениями из ini-файла.
        List<string> VarValues = new List<string>();
        IniFiles ini = new IniFiles();
        for (int i = 0; i < parameters.Length; i++)
        {
            // Присвоение значений переменной.
            v = VarNames[i];
            VarValues.Add(ini.ReadString("Common", VarNames[i].Trim('"'), ""));

            FieldInfo field_info =
                //this.GetType().GetField(v,
                GetType().GetField(v,
                    BindingFlags.Instance |
                    BindingFlags.NonPublic |
                    BindingFlags.Public);

            if (field_info == null)
            {
                // Нет значения
            }
            else
            {
                switch (parameters[i].GetType().ToString())
                {
                    case "System.String":
                        field_info.SetValue(this, VarValues[i]);
                        break;
                    case "System.Int32":
                        field_info.SetValue(this, Convert.ToInt32(VarValues[i]));
                        break;
                    case "System.Single":
                        field_info.SetValue(this, Convert.ToSingle(VarValues[i]));
                        break;
                    case "System.Boolean":
                        field_info.SetValue(this, Convert.ToBoolean(VarValues[i]));
                        break;
                    case "System.DateTime":
                        field_info.SetValue(this, Convert.ToDateTime(VarValues[i]));
                        break;
                    case "System.Long":
                        field_info.SetValue(this, Convert.ToInt64(VarValues[i]));
                        break;
                    default:
                        field_info.SetValue(this, VarValues[i]);
                        break;
                }
            }

        }
    }

}