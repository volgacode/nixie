namespace Nixie.Lexer;

/// <summary>
/// Представляет все возможные типы токенов, распознаваемых лексером языка Nixie.
/// </summary>
public enum TokenType
{
    /// <summary>Ключевое слово для логического типа данных (bool).</summary>
    Bool,

    /// <summary>Ключевое слово для целочисленного типа данных (int).</summary>
    Int,

    /// <summary>Ключевое слово для строкового типа данных (string).</summary>
    String,

    /// <summary>Ключевое слово для типа данных массива (array).</summary>
    Array,

    /// <summary>Ключевое слово условного оператора (if).</summary>
    If,

    /// <summary>Ключевое слово альтернативной ветки условного оператора (else).</summary>
    Else,

    /// <summary>Ключевое слово цикла с предусловием (while).</summary>
    While,

    /// <summary>Ключевое слово цикла со счетчиком (for).</summary>
    For,

    /// <summary>Ключевое слово для обозначения отсутствия возвращаемого значения функции (void).</summary>
    Void,

    /// <summary>Ключевое слово возврата значения из функции (return).</summary>
    Return,

    /// <summary>Ключевое слово для определения структуры (struct).</summary>
    Struct,

    /// <summary>Логическая константа истинности (true).</summary>
    True,

    /// <summary>Логическая константа ложности (false).</summary>
    False,

    /// <summary>Константа нулевой ссылки (null).</summary>
    Null,

    /// <summary>Целочисленный числовой литерал.</summary>
    Number,

    /// <summary>Строковый литерал в кавычках.</summary>
    StringValue,

    /// <summary>Идентификатор (имя переменной, функции или пользовательского типа).</summary>
    Identifier,

    /// <summary>Оператор присваивания (=).</summary>
    Assign,

    /// <summary>Оператор сравнения на равенство (==).</summary>
    Equal,

    /// <summary>Оператор сравнения на неравенство (!=).</summary>
    NotEqual,

    /// <summary>Оператор "меньше" (<).</summary>
    Less,

    /// <summary>Оператор "меньше или равно" (<=).</summary>
    LessOrEqual,

    /// <summary>Оператор "больше" (>).</summary>
    Greater,

    /// <summary>Оператор "больше или равно" (>=).</summary>
    GreaterOrEqual,

    /// <summary>Логическое "И" (&&).</summary>
    And,

    /// <summary>Логическое "ИЛИ" (||).</summary>
    Or,

    /// <summary>Логическое "НЕ" (!).</summary>
    Not,

    /// <summary>Арифметический оператор сложения (+).</summary>
    Plus,

    /// <summary>Арифметический оператор вычитания (-).</summary>
    Minus,

    /// <summary>Арифметический оператор умножения (*).</summary>
    Star,

    /// <summary>Арифметический оператор деления (/).</summary>
    Slash,

    /// <summary>Оператор доступа к члену через точку (.).</summary>
    Dot,

    /// <summary>Левая квадратная скобка ([) для индексации или массивов.</summary>
    LBracket,

    /// <summary>Правая квадратная скобка (]).</summary>
    RBracket,

    /// <summary>Левая круглая скобка (() для группировки или вызова функций.</summary>
    LParen,

    /// <summary>Правая круглая скобка ()).</summary>
    RParen,

    /// <summary>Левая фигурная скобка ({) для блоков кода.</summary>
    LBrace,

    /// <summary>Правая фигурная скобка (}).</summary>
    RBrace,

    /// <summary>Разделитель элементов или параметров (,).</summary>
    Comma,

    /// <summary>Разделитель инструкций (;).</summary>
    Semicolon,

    /// <summary>Служебный токен конца файла или потока (EOF).</summary>
    Eof,

    /// <summary>Служебный токен, обозначающий лексическую ошибку.</summary>
    Error,
}