using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BandwidthCalculator.Models;
using BandwidthCalculator.Services;

namespace BandwidthCalculator.Presenters
{
    /// <summary>
    /// Презентер для управления логикой главного окна
    /// </summary>
    public class MainPresenter
    {
        // Сервисы
        private readonly UnitConverter _converter;

        // Коллекции UI элементов
        private readonly Dictionary<SpeedUnit, TextBox> _textBoxes;

        // Состояние
        private SpeedUnit _lastEditedUnit;
        private int _decimalPlaces = 1;

        /// <summary>
        /// Конструктор презентера
        /// </summary>
        public MainPresenter()
        {
            _converter = new UnitConverter();
            _textBoxes = new Dictionary<SpeedUnit, TextBox>();
            _lastEditedUnit = SpeedUnit.MegabitPerSecond; // Значение по умолчанию
        }

        /// <summary>
        /// Инициализирует поля ввода в интерфейсе
        /// </summary>
        public void InitializeInputFields(StackPanel bitsPanel, StackPanel bytesPanel)
        {
            if (bitsPanel == null) throw new ArgumentNullException(nameof(bitsPanel));
            if (bytesPanel == null) throw new ArgumentNullException(nameof(bytesPanel));

            bitsPanel.Children.Clear();
            bytesPanel.Children.Clear();

            // Создаем поля для битов
            CreateUnitFields(bitsPanel, new[]
            {
                SpeedUnit.BitPerSecond,
                SpeedUnit.KilobitPerSecond,
                SpeedUnit.MegabitPerSecond,
                SpeedUnit.GigabitPerSecond,
                SpeedUnit.TerabitPerSecond,
                SpeedUnit.PetabitPerSecond
            }, "bit/s", "Kbit/s", "Mbit/s", "Gbit/s", "Tbit/s", "Pbit/s");

            // Создаем поля для байтов
            CreateUnitFields(bytesPanel, new[]
            {
                SpeedUnit.BytePerSecond,
                SpeedUnit.KilobytePerSecond,
                SpeedUnit.MegabytePerSecond,
                SpeedUnit.GigabytePerSecond,
                SpeedUnit.TerabytePerSecond,
                SpeedUnit.PetabytePerSecond
            }, "B/s", "KB/s", "MB/s", "GB/s", "TB/s", "PB/s");
        }

        /// <summary>
        /// Создает поля ввода для указанных единиц измерения
        /// </summary>
        private void CreateUnitFields(StackPanel panel, SpeedUnit[] units, params string[] labels)
        {
            if (panel == null) throw new ArgumentNullException(nameof(panel));
            if (units == null) throw new ArgumentNullException(nameof(units));
            if (labels == null) throw new ArgumentNullException(nameof(labels));
            if (units.Length != labels.Length) throw new ArgumentException("Количество единиц и меток должно совпадать");

            for (int i = 0; i < units.Length; i++)
            {
                var unit = units[i];
                var label = labels[i];

                // Создаем контейнер для строки
                var rowPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5)
                };

                // Создаем метку единицы измерения
                var textBlock = new TextBlock
                {
                    Text = label,
                    Width = 120,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 5, 0),
                    Foreground = Brushes.Gray
                };

                // Создаем поле ввода
                var textBox = new TextBox
                {
                    Width = 150,
                    Height = 30,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(5, 0, 5, 0),
                    Padding = new Thickness(5),
                    Tag = unit,
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    Text = string.Empty
                };

                // Подписываемся на события
                textBox.GotFocus += (s, e) =>
                {
                    _lastEditedUnit = unit;
                    ResetFieldError(textBox);
                };

                textBox.PreviewTextInput += (s, e) =>
                {
                    // Фильтрация вводимых символов
                    foreach (char c in e.Text)
                    {
                        if (!InputValidator.IsValidNumberChar(c, unit))
                        {
                            e.Handled = true;
                            return;
                        }
                    }

                    // Проверяем, чтобы не было более одной точки/запятой
                    if ((e.Text.Contains(".") || e.Text.Contains(",")) &&
                        (textBox.Text.Contains(".") || textBox.Text.Contains(",")))
                    {
                        e.Handled = true;
                    }
                };

                textBox.KeyDown += (s, e) =>
                {
                    // Разрешаем служебные клавиши
                    if (e.Key == Key.Delete || e.Key == Key.Back ||
                        e.Key == Key.Left || e.Key == Key.Right ||
                        e.Key == Key.Home || e.Key == Key.End ||
                        e.Key == Key.Tab || e.Key == Key.Enter)
                    {
                        return;
                    }

                    // Для bits не разрешаем точку/запятую
                    if (unit == SpeedUnit.BitPerSecond &&
                       (e.Key == Key.OemPeriod || e.Key == Key.OemComma || e.Key == Key.Decimal))
                    {
                        e.Handled = true;
                    }
                };

                // Создаем кнопку копирования
                var copyButton = new Button
                {
                    Content = "Копировать",
                    Width = 80,
                    Height = 30,
                    Margin = new Thickness(5, 0, 0, 0),
                    Tag = textBox,
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Cursor = Cursors.Hand
                };
                copyButton.Click += OnCopyButtonClick;

                // Добавляем элементы в контейнер
                rowPanel.Children.Add(textBlock);
                rowPanel.Children.Add(textBox);
                rowPanel.Children.Add(copyButton);

                // Добавляем контейнер в панель
                panel.Children.Add(rowPanel);

                // Сохраняем ссылку на TextBox
                _textBoxes[unit] = textBox;
            }
        }

        /// <summary>
        /// Сбрасывает подсветку ошибки поля
        /// </summary>
        private void ResetFieldError(TextBox textBox)
        {
            if (textBox == null) return;

            textBox.BorderBrush = Brushes.LightGray;
            textBox.BorderThickness = new Thickness(1);
            textBox.ToolTip = null;
        }

        /// <summary>
        /// Подсвечивает поле с ошибкой
        /// </summary>
        private void MarkFieldError(TextBox textBox, string error)
        {
            if (textBox == null) return;

            textBox.BorderBrush = Brushes.Red;
            textBox.BorderThickness = new Thickness(2);
            textBox.ToolTip = error;
        }

        /// <summary>
        /// Обработчик нажатия кнопки копирования
        /// </summary>
        private void OnCopyButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
            {
                try
                {
                    Clipboard.SetText(textBox.Text);
                }
                catch
                {
                    // Игнорируем ошибки копирования в буфер обмена
                }
            }
        }

        /// <summary>
        /// Выполняет конвертацию значений
        /// </summary>
        public void PerformConversion()
        {
            if (!_textBoxes.ContainsKey(_lastEditedUnit))
                return;

            var textBox = _textBoxes[_lastEditedUnit];
            var text = textBox.Text;
            var unit = (SpeedUnit)textBox.Tag;

            if (string.IsNullOrWhiteSpace(text))
            {
                ResetAll();
                return;
            }

            if (InputValidator.TryParseInput(text, unit, out double value, out string error))
            {
                ResetFieldError(textBox);
                UpdateAllFields(value, unit);
            }
            else
            {
                MarkFieldError(textBox, error);
            }
        }

        /// <summary>
        /// Обновляет все поля с результатами конвертации
        /// </summary>
        private void UpdateAllFields(double value, SpeedUnit fromUnit)
        {
            try
            {
                var results = _converter.ConvertFormatted(value, fromUnit, _decimalPlaces);

                // Обновляем все поля, кроме источника
                foreach (var kvp in results)
                {
                    if (kvp.Key == fromUnit)
                        continue;

                    if (_textBoxes.ContainsKey(kvp.Key))
                    {
                        _textBoxes[kvp.Key].Text = kvp.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки (в реальном приложении)
                Console.WriteLine($"Ошибка при обновлении полей: {ex.Message}");
            }
        }

        /// <summary>
        /// Сбрасывает все поля ввода
        /// </summary>
        public void ResetAll()
        {
            try
            {
                foreach (var textBox in _textBoxes.Values)
                {
                    textBox.Text = string.Empty;
                    ResetFieldError(textBox);
                }
            }
            catch
            {
                // Игнорируем ошибки при сбросе
            }
        }

        /// <summary>
        /// Устанавливает точность отображения чисел
        /// </summary>
        public void SetPrecision(int decimalPlaces)
        {
            _decimalPlaces = Math.Clamp(decimalPlaces, 1, 10);

            // Если есть текущее значение, пересчитываем с новой точностью
            if (_textBoxes.TryGetValue(_lastEditedUnit, out TextBox currentTextBox))
            {
                var unit = (SpeedUnit)currentTextBox.Tag;
                if (InputValidator.TryParseInput(currentTextBox.Text, unit, out double currentValue, out _))
                {
                    UpdateAllFields(currentValue, unit);
                }
            }
        }

        /// <summary>
        /// Копирует все результаты в буфер обмена
        /// </summary>
        public void CopyAllToClipboard()
        {
            try
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("=== Bandwidth Calculator Results ===");
                sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"Precision: {_decimalPlaces} decimal places");
                sb.AppendLine();

                foreach (var unit in _textBoxes.Keys)
                {
                    string text = _textBoxes[unit].Text;
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        sb.AppendLine($"{unit}: {text}");
                    }
                }

                string result = sb.ToString();
                if (!string.IsNullOrWhiteSpace(result))
                {
                    Clipboard.SetText(result);
                }
            }
            catch
            {
                // Игнорируем ошибки копирования в буфер обмена
            }
        }
    }
}