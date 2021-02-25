# INIFiles

Класс для работы с ini-файлами в C# <br>
Изначально создавался по типу реализации подобного механизма в Delphi, но в последствии стал наиудобнейшим механизмом для каждого проекта.
<p align="center"> 
  <img align="center" src="https://github.com/AstiiCoder/inifiles/blob/main/IniFileIcon.png?raw=true" width="256"/> 
</p>

<h1><div align="center">IniFiles</h1>
<p align="center">
  <img src="https://img.shields.io/badge/PRICE-free-yellow"/>
  <img src="https://img.shields.io/badge/SUPPORT-yes-yellow"/>
</p>

<p align="center">
  <img src="https://img.shields.io/github/downloads/AstiiCoder/inifiles/total?color=yellow&label=DOWNLOADS&logo=GitHub&logoColor=yellow&style=flat"/>
  <img src="https://img.shields.io/github/last-commit/AstiiCoder/inifiles?color=yellow&label=LAST%20COMMIT&style=flat"/>
  <img src="https://img.shields.io/github/release-date/AstiiCoder/inifiles?color=yellow&label=RELEASE%20DATE&style=flat"/>
</p>

This class is designed to work with <b>INI files</b>. That is, with text files that have a specific structure. <br>
<b> Files must be in Unicode encoding. </b> If the INI file was created in a different encoding, then you can either translate, for example, in `Notepad ++` into encoding (UTF-8)
either initialize the class with the required encoding (see example below), or use a file that will automatically create this class.
  
The main difference from similar classes: ease of use, syntactic minimalism, the presence of separate and batch writing of several values,
source code with comments, which can be easily modified according to your needs and specific implementation, compatibility with .NET Standard, .Net Framework and .Net Core,
 no use of additional libraries and other external code, the ability to work with files in different encoding. <br>

Generally, using INI files is common for reading and storing application settings. <br>
One of the most popular solutions is to save the position and size of the window (form) and other values ​​when the application is closed,
and the next time the application is launched, these parameters are loaded from the INI file and applied. <br>

  Этот класс предназначен для работы с <b>INI-файлами</b>. То есть с текстовыми файлами, имеющими определённую структуру.<br>
  <b>Файлы должны быть в кодировке Unicode.</b> Если INI-файл был создан в другой кодировке, то можно либо перевести, например в `Notepad++` в кодировку (UTF-8)
  либо инициализировать класс с указанием нужной кодировки (см. пример ниже), либо использовать файл, который автоматически создаст этот класс.
  
  Основное отличие от подобных классов: простота использования, синтаксический минимализм, наличие раздельной и пакетной записи нескольких значений,
  исходный код с комментариями, который легко изменить под свои нужды и конкретную реализацию, совместимость с .NET Standard, .Net Framework и .Net Core,
  отсутствие использования дополнительных библиотек и другого внешнего кода, возможность работать с файлами в разной кодировке. <br>

  Как правило, примениение INI-файлов распространено для чтения и хранения параметров приложения.<br>
  Одно из наиболее популярных решений - сохранение позиции и величины окна (формы) и других значений при закрытии приложения,
  а при последующем запуске приложения - загрузка этих параметров из INI-файла и их применение.<br> 


## 🗂 Зависимости

- System
- System.Collections.Generic
- System.Linq
- System.Text
- System.Diagnostics
- System.Text.RegularExpressions
- System.Reflection

## ✅ Основные возможности

- ### Чтение из файла
- **ReadString** загрузить строковое значение.
- **ReadInt** (ReadInteger) загрузить целое числовое значение типа (int).
- **ReadLong** загрузить целое числовое значение типа (long).
- **ReadBool** загрузить значение типа bool (true/false).
- **ReadSections** загрузить наименования всех секций в список.
- **ReadSection** загрузить всю секцию в список.
- **ReadDate** загрузить значение типа (DateTime).
- **ReadDateAsString** загрузить строковое значение, которое имеет формат даты в виде (DD.MM.YYYY).
- **ReadDateTime** загрузить дату и время.
- **ReadFloat** загрузить вещественное число типа (float).
- **ReadStream** загрузить поток байтов (хранится в виде строки, необходимо преобразовать поток в нужный объект).
- **ReadRect** загрузить параметры прямоугольника (Rect). Можно сразу получить координаты и размеры окна.
- **Read** сокращённая форма чтения значения; возвращает значение того типа, который удалось определить по значению в файле. 
- **ReadFromIni** загрузка нескольких переменных (в том числе и разного типа) одной командой. Самый быстрый способ загрузки значений.

- ### Запись в файл
- **WriteString** cохранить строковое значение.
- **WriteInt** (WriteInteger) сохранить целое числовое значение типа (int).
- **WriteLong** сохранить целое числовое значение типа (long).
- **WriteBool** сохранить значение типа bool (true/false).
- **WriteDate** сохранить значение типа (DateTime) когда время не нужно в формате вида (DD.MM.YYYY).
- **WriteDateTime** сохранить значение типа (DateTime), то есть дата и время.
- **WriteFloat** сохранить вещественное число типа (float).
- **WriteStream** сохранить поток байтов.
- **WriteRect** сохранить прямоугольник (Rect). Позволяет сразу сохранить координаты и размеры окна.
- **Write** сокращённая форма сохранения строкового значения, но при необходимости, можно использовать для любого типа, конвертируемого в строку.
- **WriteToIni** сохранение нескольких переменных (в том числе и разного типа) одной командой. Самый быстрый способ сохранения значений.

- ### Проверки и удаления (не поддерживается в режиме Пакетного сохранения)
- **KeyExists** проверка, существует ли ключ в заданной секции
- **SectionExists** проверка, существует ли данная секция
- **EraseSection** удаление секции

- ### Пакетное сохранение
- **WaitAndNotSave** инициализация режима сохранения нескольких значений одновременно, то есть, сразу в файл значения сохраняться не будут.
- **Save** сохранение в файл нескольких значений за один раз в режиме WaitAndNotSave.


## 🔨 Как пользоваться

- ### Подключение
1. Скачайте файл `IniFiles.cs` из данного репозитория.
2. Добавьте его в свой проект.
3. Используйте код из примеров ниже в нужном месте вашей программы.

- ### Вырианты работы
1. Чтение по ключу
2. Чтение секций
3. Удаление по ключу и секций целиком
4. Сохранение по ключу
5. Сохранение нескольких значениий за раз
6. Сохранение и чтение нескольких значений переменных одной строкой кода

- ### Примеры использования

Для использования класса, необходима его инициализация. Например, таким кодом: IniFiles ini = new IniFiles(@"D:\1.ini");
В момент инициализации, данные их INI-файла загружаются в память. После инициализации можно читать данные из файла.
Если необходимо загрузить данные в другом месте программы и в этот момент они могли измениться в файле, то необходимо создать новый экземпляр класса
и работать уже с ним. При записи в файл можно использовать режим непосредственного сохранения значения или же режим пакетного сохранения.

1. Чтение строкового значения из секции `Section7` по ключу `song` из файла `D:\1.ini`:
```csharp
IniFiles ini = new IniFiles(@"D:\1.ini");
string MySong = ini.ReadString("Section7", "song", "ohne dich");
```
2. Чтение числовых значений из секции `Section1` по ключу `Left` из файла `config.ini`:<br>
Здесь не указывается имя файла, поэтому программа будет обращаться к файлу `config.ini`, размещённому в текущей директории.
```csharp
IniFiles ini = new IniFiles();
int Left = ini.ReadInteger("Section1", "Left", 0);
int Top = ini.ReadInteger("Section1", "Top", 1);
```
3. Чтение значений разных типов и заполнение элемента:
```csharp
IniFiles ini = new IniFiles(@"D:\1.ini");
TextBox_Content.Text += "\n" + Convert.ToString(ini.ReadBool("Section7", "var_bool", true)); // загрузка логического значения
TextBox_Content.Text += "\n" + Convert.ToString(ini.ReadDate("Section7", "var_date", DateTime.Now)); // загрузка даты без времени
TextBox_Content.Text += "\n" + ini.ReadDateAsString("Section7", "var_date", DateTime.Now); // загрузка даты как строки в сокращённом формате
TextBox_Content.Text += "\n" + Convert.ToString(ini.ReadDateTime("Section7", "var_date_time", DateTime.Now)); //загрузка даты и времени
```
результат на экране:<br>
True<br>
12.01.2021 0:00:00<br>
12.01.2021<br>
12.01.2021 13:31:19<br>

4. Сохранение значений разных типов в файл в текущей директории:
```csharp
IniFiles ini = new IniFiles();
ini.WriteString("Section7", "var_str", "Ohne Dich");
ini.WriteInteger("Section7", "var_int", 700);
ini.WriteBool("Section7", "var_bool", true);
ini.WriteDate("Section7", "var_date", DateTime.Now);
ini.WriteDateTime("Section7", "var_date_time", DateTime.Now);
```

результат в файле `config.ini`:<br>
[Section7]<br>
var_date_time=12.01.2021 13:31:19<br>
var_date=12.01.2021<br>
var_bool=True<br>
var_int=700<br>
var_str=Ohne Dich<br>

5. Сохранение нескольких значений сразу (пакетная запись):<br>
В этом режиме от строчки WaitAndNotSave до строчки Save не происходит мгновенное сохранение в файл. Непосредственно сохранение происходит командой Save.
```csharp
IniFiles ini = new IniFiles(@"D:\1.ini");
ini.WaitAndNotSave(); // инициализация режима пакетного сохранения
ini.WriteString("Common", nameof(X), X); // здесь в файл пока ничего не записывается
ini.WriteInteger("Common", nameof(Y), Y); // здесь в файл пока ничего не записывается
ini.WriteBool("Common", nameof(Z), Z); // здесь в файл пока ничего не записывается
ini.Save(); // здесь происходит сохранение трёх значений
```

6. Краткая форма сохранения:<br>
В данном примере в файл `config.ini`, размещённому в текущей директории, в секцию `Common` (имя по умолчанию) сохраняется значение из переменных X и Y
```csharp
IniFiles ini = new IniFiles();
ini.Write(nameof(X), X);  // если переменная будет переименована через IDE, то этот код будет работать без ошибок
ini.Write("Y", Y.ToString());
```
7. Чтение названий секций:
```csharp
IniFiles ini = new IniFiles(@"D:\1.ini");
List<string> l = new List<string>(ini.ReadSections());
foreach (string item in l)
{
     TextBox_Content.Text += Environment.NewLine + item;
}

```

8. Есть ли ключ в данной секции:
```csharp
IniFiles ini = new IniFiles(@"D:\1.ini");
if (ini.KeyExists("Section7", "var_int")) MessageBox.Show("есть"); 
else MessageBox.Show("нет");
```

9. Удаление секции `Section4`:
```csharp
IniFiles ini = new IniFiles(@"D:\1.ini");
ini.EraseSection("Section4");
```

10. Чтение и запись потоков в файл в указанной кодировке:<br>
```csharp
IniFiles ini = new IniFiles(@"D:\1.ini", Encoding.UTF8);
MemoryStream Value = ini.ReadBinaryStream("Section5", "BS", stream);
...
ini.WriteBinaryStream("Section5", "BS", stream);
```

11. Чтение с попыткой определения типа:<br>
Не используйте для значений, которые могут быть неоднозначно интерпретированы.
```csharp
IniFiles ini = new IniFiles();
int X = ini.Read("X");
bool B = ini.Read("IsIamBest");
```

12. Сохранение координат и размеров окна при его закрытии:<br>
```csharp
private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IniFiles ini = new IniFiles();
            ini.WriteRect("Main", "WindowPosition", RestoreBounds);
        }
```

13. Восстановление координат и размеров окна при его инициализации:<br>
```csharp
private void Window_Initialized(object sender, EventArgs e)
        {
            IniFiles ini = new IniFiles();
            Rect bounds = ini.ReadRect("Main", "WindowPosition");
            if (bounds == Rect.Parse("0,0,0,0")) return;
            Top = bounds.Top;
            Left = bounds.Left;
            // восстановить размеры окна, но не в полноэкранном режиме. Если полностью, то this.Location = ... this.WindowState = ...
            if (SizeToContent == SizeToContent.Manual)
            {
                Width = bounds.Width;
                Height = bounds.Height;
            }
        }
```

14. Чтение нескольких переменных одной командой:<br>
Переменные могут быть разных типов. Чтение происходит исходя из имён переменных. Самый минимум написания кода!
```csharp
   public int X = 1;
   public float k = 3.5f;
            ...
			string j = "Hello";
            IniFilesExt ini = new IniFilesExt();
            ini.ReadFromIni(X, j, k);
```

15. Сохранение нескольких переменных одной строчкой кода:<br>
Переменные могут быть разных типов. Без создания экземпляра класса. Сохранение происходит исходя из имён переменных. Предельный минимум написания кода!
```csharp
   public int X = 1;
   public float k = 3.5f;
            ...
			string j = "Hello";
			...
	IniFilesExt.WriteToIni(X, k, j);
```