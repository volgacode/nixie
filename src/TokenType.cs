namespace Nixie.Lexer;

public enum TokenType
{
    // Ключевые слова: Типы данных
    Bool,
    Int,
    String,
    Array,

    // Ключевые слова: Управление потоком
    If,
    Else,
    While,
    For,

    // Ключевые слова: Функции и структуры
    Void,
    Return,
    Struct,

    // Ключевые слова: Литералы и константы
    True,
    False,
    Null,

    // Динамические категории (пользовательские значения и имена)
    Number,      // Целочисленные литералы (0-9)
    StringValue, // Строковые литералы ("...")
    Identifier,  // Идентификаторы переменная/функция (a-z, A-Z, 0-9, _)

    // Операторы: Присваивание
    Assign,          // =

    // Операторы: Сравнения
    Equal,           // ==
    NotEqual,        // !=
    Less,            // <
    LessOrEqual,     // <=
    Greater,         // >
    GreaterOrEqual,  // >=

    // Операторы: Логические
    And,             // &&
    Or,              // ||
    Not,             // !

    // Операторы: Арифметические
    Plus,            // +
    Minus,           // -
    Star,            // *
    Slash,           // /

    // Операторы: Доступ
    Dot,             // .

    // Индексация, группировка, разделители
    LBracket,        // [
    RBracket,        // ]
    LParen,          // (
    RParen,          // )
    LBrace,          // {
    RBrace,          // }
    Comma,           // ,
    Semicolon,       // ;

    // Служебные токены
    Eof,
    Error
}