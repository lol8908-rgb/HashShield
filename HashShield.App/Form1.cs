namespace HashShield.App;

public partial class Form1 : Form
{
    private readonly TextBox _targetTextBox = null!;
    private readonly TextBox _apiKeyTextBox = null!;
    private readonly TextBox _resultTextBox = null!;
    private readonly Button _chooseFolderButton = null!;
    private readonly Button _quickScanButton = null!;
    private readonly Button _fullScanButton = null!;

    public Form1()
    {
        InitializeComponent();

        _targetTextBox = Controls.Find("targetTextBox", true).FirstOrDefault() as TextBox ?? throw new InvalidOperationException();
        _apiKeyTextBox = Controls.Find("apiKeyTextBox", true).FirstOrDefault() as TextBox ?? throw new InvalidOperationException();
        _resultTextBox = Controls.Find("resultTextBox", true).FirstOrDefault() as TextBox ?? throw new InvalidOperationException();
        _chooseFolderButton = Controls.Find("chooseFolderButton", true).FirstOrDefault() as Button ?? throw new InvalidOperationException();
        _quickScanButton = Controls.Find("quickScanButton", true).FirstOrDefault() as Button ?? throw new InvalidOperationException();
        _fullScanButton = Controls.Find("fullScanButton", true).FirstOrDefault() as Button ?? throw new InvalidOperationException();

        _chooseFolderButton.Click += ChooseFolderButton_Click;
        _quickScanButton.Click += QuickScanButton_Click;
        _fullScanButton.Click += FullScanButton_Click;
        DragEnter += Form1_DragEnter;
        DragDrop += Form1_DragDrop;
    }

    private void ChooseFolderButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Zielordner für den HashShield-Scan auswählen"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _targetTextBox.Text = dialog.SelectedPath;
            _resultTextBox.Text = $"Ziel ausgewählt: {dialog.SelectedPath}{Environment.NewLine}";
        }
    }

    private async void QuickScanButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_targetTextBox.Text))
        {
            _targetTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }

        await RunScanAsync(isFullScan: false);
    }

    private async void FullScanButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_targetTextBox.Text))
        {
            _targetTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        await RunScanAsync(isFullScan: true);
    }

    private async Task RunScanAsync(bool isFullScan)
    {
        _quickScanButton.Enabled = false;
        _fullScanButton.Enabled = false;
        _resultTextBox.Text = "Scan läuft...";

        try
        {
            var summary = isFullScan
                ? await ScannerEngine.FullScanAsync(_targetTextBox.Text, _apiKeyTextBox.Text)
                : await ScannerEngine.QuickScanAsync(_targetTextBox.Text, _apiKeyTextBox.Text);

            _resultTextBox.Text = summary.ResultText;
        }
        catch (Exception ex)
        {
            _resultTextBox.Text = $"Fehler: {ex.Message}";
        }
        finally
        {
            _quickScanButton.Enabled = true;
            _fullScanButton.Enabled = true;
        }
    }

    private void Form1_DragEnter(object? sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void Form1_DragDrop(object? sender, DragEventArgs e)
    {
        var files = e.Data?.GetData(DataFormats.FileDrop) as string[];
        if (files is null || files.Length == 0)
        {
            return;
        }

        var first = files[0];
        _targetTextBox.Text = Directory.Exists(first) ? first : Path.GetDirectoryName(first) ?? first;
        _resultTextBox.Text = $"Per Drag & Drop geladen: {_targetTextBox.Text}{Environment.NewLine}";
    }
}
