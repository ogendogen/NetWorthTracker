using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetWorthTracker.Definitions
{
    /// <summary>
    /// Interaction logic for DefinitionsWindow.xaml
    /// </summary>
    public partial class DefinitionsWindow : Window
    {
        public DefinitionsWindow() // todo: inject user and parameter which definitions collection to fetch
        {
            InitializeComponent();
        }
    }
}
