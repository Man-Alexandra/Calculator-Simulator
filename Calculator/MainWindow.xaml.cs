using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Text.RegularExpressions;


namespace Calculator
{
    public partial class MainWindow : Window
    {
        //Objects
        private AritmeticOperations arithmetic = new AritmeticOperations();
        private MemoryOperations memory;
        public MainWindow()
        {
            InitializeComponent();
            memory = new MemoryOperations(MemoryListBox, DisplayTextBox);
        }

        // Handle button clicks
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string buttonContent = button.Content.ToString();
                DisplayTextBox.Text = arithmetic.ProcessInput(DisplayTextBox.Text, buttonContent);
            }
        }

        // Memory operations
        private void MMinusButton(object sender, RoutedEventArgs e)
        {
            memory.SubtractFromLastMemoryValue();
        }

        private void MPlusButton(object sender, RoutedEventArgs e)
        {
            memory.AddToLastMemoryValue();
        }

        private void MSButton_Click(object sender, RoutedEventArgs e)
        {
            memory.AddToMemory(DisplayTextBox.Text);
        }

        private void MRButton_Click(object sender, RoutedEventArgs e)
        {
            memory.PrintLastFromMemory();

        }

        private string selectedText = ""; 

        // Copy Paste and Cut operations
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DisplayTextBox.SelectedText))
            {
                selectedText = DisplayTextBox.SelectedText;
                Clipboard.SetText(selectedText); 
            }
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DisplayTextBox.SelectedText))
            {
                selectedText = DisplayTextBox.SelectedText;
                Clipboard.SetText(selectedText); 

                int startIndex = DisplayTextBox.SelectionStart;
                DisplayTextBox.Text = DisplayTextBox.Text.Remove(startIndex, DisplayTextBox.SelectionLength);
            }
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            string clipboardText = Clipboard.GetText(); 

            if (!string.IsNullOrEmpty(clipboardText))
            {
                int startIndex = DisplayTextBox.SelectionStart;
                DisplayTextBox.Text = DisplayTextBox.Text.Insert(startIndex, clipboardText);
            }
        }

        //DG
        private void DGButtonClick(object sender, RoutedEventArgs e)
        {
            DisplayTextBox.Text = arithmetic.ApplyDigitGrouping(DisplayTextBox.Text);
        }

        //Delete
        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayTextBox.Text.Length > 0)
            {
                DisplayTextBox.Text = DisplayTextBox.Text.Substring(0, DisplayTextBox.Text.Length - 1);
            }
        }

        //Menu Button
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MenuBar.Visibility = (MenuBar.Visibility == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
        }

        //About section
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aceasta aplicatie a fost realizata de:\n" +
                   "Man Alexandra din grupa 10LF332.");
        }

        //Memory Button
        private void MemoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemoryListBox.Visibility == Visibility.Collapsed)
            {
                MemoryListBox.Visibility = Visibility.Visible;
            }
            else
            {
                MemoryListBox.Visibility = Visibility.Collapsed;
            }
        }

    }
}