# Интерпретируемый язык программирования Nixie

Проект предназначен для .NET 10 (C# 14): https://dotnet.microsoft.com/en-us/download/dotnet/10.0

## Клонирование проекта

Клонирование без авторизации в GitHub:

```bash
# Клонируем репозиторий в каталог nixie/
git clone https://github.com/volgacode/nixie nixie
```

## Сборка

Сборка консольной утилитой dotnet из .NET SDK:

```bash
# Сборка
dotnet build

# Запуск тестов
dotnet test
```

## Анализ покрытия кода тестами

Запустите скрипт и HTML-отчёт о покрытии появится в tests/coverage-report/index.html

```bash
scripts/run-tests-with-coverage
```

## Структура проекта на C#

Ликбез по типам файлов:

1. Файл `*.slnx` (Solution) — основной файл «решения» (то есть проекта), обычно один на репозиторий с кодом
2. Файл `*.csproj` (C# Project) — файл с описанием одного Assembly (_рус._ Сборка), компилируется в один DLL
    - Расширение `*.dll` также используется для разделяемых библиотек в ОС Windows; в Linux аналогом являются файлы `*.so` (shared library)

Код на C# компилируется в MSIL — код виртуальной машины платформы .NET. Такой код сохраняется в файлах `.dll` для всех платформ, где работает .NET.

Структура проекта была создана такими командами:

```bash
# Создаём Solution (основной проект).
dotnet new sln

# Создаём библиотеку Nexie.Lexer в каталоге src/ и добавляем её в Solution
dotnet new classlib -o src/Nexie.Lexer
dotnet sln add src/Nexie.Lexer/

# Создаём проект модульных тестов на тестовом фреймворке XUnit
dotnet new xunit -o tests/Nexie.Lexer.UnitTests
dotnet sln add tests/ExampleLib.UnitTests/

# Добавляем проекту тестов ссылку на проект
dotnet add tests/Nexie.Lexer.UnitTests/ reference src/Nexie.Lexer/
```

## Статический анализ

Шаблон содержит подключённые статические анализаторы:

* Статические анализаторы подключены в файле `Directory.Build.props` — система сборки MSBuild воспринимает этот файл как общие параметры сборки всего проекта;
* В корне проекта есть файл `.editorconfig` с настройками статических анализаторов

Вы можете менять файл `.editorconfig` — но только при условии, что это не снизит качество кода.
