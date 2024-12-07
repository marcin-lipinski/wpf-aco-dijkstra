using Microsoft.Win32;
using SPA.App.Models;
using SPA.Core.Algorithms.Aco;
using SPA.Core.Configuration;
using SPA.Core.FileHandler;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SPA.App;

public partial class MainWindow : Window
{
    public ObservableCollection<ListOption> ListOptions { get; set; } = null!;

    public MainWindow()
    {
        InitializeComponent();
        InitializeConfig();
        SelectFileRadioButton.IsChecked = true;
    }

    private void InitializeConfig()
    {
        OptionsList.ItemsSource = ListOptions =
        [
            new("Ants number ratio (ants / nodes)", "0,5"),
            new("ACO iterations Number", "1000"),
            new("Alpha (α)", "2"),
            new("Beta (β)", "4"),
            new("Random node factor", "0,2"),
            new("Evaporation rate (ρ)", "0,3"),
            new("Number of shortest paths to be found", "3"),
            new("No edge chances during generation", "0,2"),
            new("Number of nodes generated", "10"),
            new("Generated files target path", AppContext.BaseDirectory)
        ];
    }

    private void SelectFileRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        ToggleFileGenerationControls(true);
    }

    private void GenerateFileRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        ToggleFileGenerationControls(false);
    }

    private void ToggleFileGenerationControls(bool selectFile)
    {
        SelectFileButton.Visibility = selectFile ? Visibility.Visible : Visibility.Hidden;
        GenerateFileButton.Visibility = selectFile ? Visibility.Hidden : Visibility.Visible;
    }

    private void SelectFileButton_Click(object sender, RoutedEventArgs e)
    {
        var fileDialog = new OpenFileDialog
        {
            Title = "Select a File",
            Filter = "All Files|*.*"
        };

        if (fileDialog.ShowDialog() == true)
        {
            SelectedFileName.Text = fileDialog.FileName;
            LoadFile();
        }
    }

    private void GenerateFileButton_Click(object sender, RoutedEventArgs e)
    {
        var targetPath = ListOptions[^1].Value;
        var nodesNumber = Convert.ToInt32(ListOptions[^2].Value);
        var noEdgeChances = Convert.ToDouble(ListOptions[^3].Value);
        var fileName = FileHandler.GenerateFileWithRandomGraph(nodesNumber, noEdgeChances, targetPath);
        SelectedFileName.Text = fileName;
        LoadFile();
    }

    private void LoadFile()
    {
        var nodes = FileHandler.ReadNodesName(SelectedFileName.Text);

        if (nodes != null)
        {
            StartNodeComboBox.ItemsSource = nodes;
            EndNodeComboBox.ItemsSource = nodes;
            EnableComboBoxes();
        }
    }

    private void ClearComboBoxes()
    {
        StartNodeComboBox.SelectedItem = null;
        EndNodeComboBox.SelectedItem = null;
    }

    private void EnableComboBoxes()
    {
        StartNodeComboBox.IsEnabled = true;
        EndNodeComboBox.IsEnabled = true;
    }

    private void RunButton_Click(object sender, RoutedEventArgs e)
    {
        var config = CreateProgramConfig();
        var resultWindow = new ResultWindow(this, config);
        Hide();
        resultWindow.Show();
    }

    private AlgorithmsConfig CreateProgramConfig()
    {
        return new AlgorithmsConfig
        {
            EndNodeName = EndNodeComboBox.SelectedValue?.ToString()!,
            StartNodeName = StartNodeComboBox.SelectedValue?.ToString()!,
            ShortestPathsNumber = Convert.ToInt32(ListOptions[6].Value.ToString()),
            GraphFilePath = SelectedFileName.Text,
            AcoOptions = new AcoOptions
            {
                AntsNumberRatio = Convert.ToDouble(ListOptions[0].Value.ToString()),
                IterationsNumber = Convert.ToInt32(ListOptions[1].Value),
                Alpha = Convert.ToDouble(ListOptions[2].Value),
                Beta = Convert.ToDouble(ListOptions[3].Value),
                RandomNodeFactor = Convert.ToDouble(ListOptions[4].Value),
                EvaporationRate = Convert.ToDouble(ListOptions[5].Value),
            }
        };
    }

    private void NodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RunButton.IsEnabled = StartNodeComboBox.SelectedItem != null && EndNodeComboBox.SelectedItem != null;
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !double.TryParse(e.Text, out _) && !e.Text.EndsWith(',');
    }

    protected override void OnClosed(EventArgs e)
    {
        Application.Current.Shutdown();
        base.OnClosed(e);
    }
}
