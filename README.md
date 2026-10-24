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