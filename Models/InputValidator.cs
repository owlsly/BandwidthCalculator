using System;
using System.Globalization;

namespace BandwidthCalculator.Models
{
    /// <summary>
    /// Валидатор и форматтер для ввода чисел
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// Пытается преобразовать строку в число с учетом единицы измерения
        /// </summary>
        public static bool TryParseInput(string input, SpeedUnit unit, out double value, out string error)
        {
            value = 0;
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                error = "Пустое значение";
                return false;
            }

            // Заменяем запятую на точку для поддержки обоих форматов
            string normalizedInput = input.Replace(',', '.');

            // Пробуем распарсить число
            if (!double.TryParse(normalizedInput,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double result))
            {
                error = "Некорректный формат числа";
                return false;
            }

            // Проверяем, что число не отрицательное
            if (result < 0)
            {
                error = "Число не может быть отрицательным";
                return false;
            }

            // Для bits (наименьшая единица) проверяем, что число целое
            if (unit == SpeedUnit.BitPerSecond)
            {
                if (Math.Abs(result - Math.Round(result)) > 0.000000001)
                {
                    error = "Биты должны быть целым числом";
                    return false;
                }

                // Округляем до целого
                result = Math.Round(result);
            }

            value = result;
            return true;
        }

        /// <summary>
        /// Форматирует число с указанной точностью
        /// </summary>
        public static string FormatNumber(double number, int decimalPlaces = 1)
        {
            // Определяем, нужно ли показывать десятичную часть
            bool hasFraction = Math.Abs(number - Math.Round(number)) > 0.000000001;

            // Если дробной части нет и точность = 1, показываем без десятичной части
            if (!hasFraction && decimalPlaces == 1)
            {
                return number.ToString("F0", CultureInfo.InvariantCulture);
            }

            // Применяем стандартное математическое округление
            double rounded = Math.Round(number, decimalPlaces, MidpointRounding.AwayFromZero);

            // Форматируем с указанным количеством знаков
            string format = $"F{decimalPlaces}";
            return rounded.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Проверяет, является ли символ допустимым для ввода
        /// </summary>
        public static bool IsValidNumberChar(char c, SpeedUnit unit)
        {
            // Для bits не разрешаем точку или запятую
            if (unit == SpeedUnit.BitPerSecond && (c == '.' || c == ','))
            {
                return false;
            }

            // Разрешаем цифры, точку, запятую и Backspace
            return char.IsDigit(c) || c == '.' || c == ',' || c == (char)8;
        }
    }
}