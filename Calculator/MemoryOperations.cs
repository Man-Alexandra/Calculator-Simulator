using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Calculator
{
    internal class MemoryOperations
    {
        // Attributes
        private List<double> memoryList = new List<double>();
        private ListBox memoryListBox;
        private TextBox displayTextBox;

        //Constructor
        public MemoryOperations(ListBox listBox, TextBox displayText)
        {
            memoryListBox = listBox;
            displayTextBox = displayText;
        }

        //MS Button function
        public void AddToMemory(string expression)
        {
            if (double.TryParse(expression, out double value))
            {
                memoryList.Add(value);
                memoryListBox.Items.Add(value.ToString()); 
            }
        }

        // MR Button
        public void PrintLastFromMemory()
        {
            if (memoryList.Count > 0)
            {
                double lastMemoryValue = memoryList.Last();
                displayTextBox.Text = lastMemoryValue.ToString();
                memoryListBox.SelectedItem = lastMemoryValue.ToString();
            }
        }

        //M+
        public void AddToLastMemoryValue()
        {
            if (memoryListBox.SelectedItem != null && double.TryParse(memoryListBox.SelectedItem.ToString(), out double selectedValue) &&
        double.TryParse(displayTextBox.Text, out double currentValue))
            {
                double newValue = selectedValue + currentValue;
                int selectedIndex = memoryListBox.SelectedIndex;

                memoryList[selectedIndex] = newValue;
                UpdateMemoryListBox();
            }
        }

        // M- 
        public void SubtractFromLastMemoryValue()
        {
            if (memoryListBox.SelectedItem != null && double.TryParse(memoryListBox.SelectedItem.ToString(), out double selectedValue) &&
        double.TryParse(displayTextBox.Text, out double currentValue))
            {
                double newValue = selectedValue - currentValue;
                int selectedIndex = memoryListBox.SelectedIndex;

                memoryList[selectedIndex] = newValue;
                UpdateMemoryListBox();
            }
        }
        private void UpdateMemoryListBox()
        {
            memoryListBox.Items.Clear();
            foreach (double value in memoryList)
            {
                memoryListBox.Items.Add(value.ToString());
            }
        }
    }
}
