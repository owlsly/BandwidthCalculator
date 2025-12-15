using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BandwidthCalculator.Presenters;

namespace BandwidthCalculator
{
    public partial class MainWindow : Window
    {
        private readonly MainPresenter _presenter;

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация презентера
            _presenter = new MainPresenter();
            _presenter.InitializeInputFields(BitsPanel, BytesPanel);

            // Подписываемся на события кнопок
            ResetButton.Click += (s, e) => _presenter.ResetAll();
            ConvertButton.Click += (s, e) => _presenter.PerformConversion();
            CopyAllButton.Click += (s, e) => _presenter.CopyAllToClipboard();

            // Подписываемся на события RadioButton для точности
            Precision1.Checked += (s, e) => _presenter.SetPrecision(1);
            Precision2.Checked += (s, e) => _presenter.SetPrecision(2);
            Precision3.Checked += (s, e) => _presenter.SetPrecision(3);
            Precision4.Checked += (s, e) => _presenter.SetPrecision(4);
            Precision5.Checked += (s, e) => _presenter.SetPrecision(5);
            Precision6.Checked += (s, e) => _presenter.SetPrecision(6);
            Precision7.Checked += (s, e) => _presenter.SetPrecision(7);
            Precision8.Checked += (s, e) => _presenter.SetPrecision(8);
            Precision9.Checked += (s, e) => _presenter.SetPrecision(9);
            Precision10.Checked += (s, e) => _presenter.SetPrecision(10);

            // Обработка Enter для конвертации
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    _presenter.PerformConversion();
                }
            };
        }
    }
}