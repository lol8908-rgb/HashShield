namespace HashShield.App;

public partial class Form1 : Form
{
    private readonly TextBox _filePathTextBox = null!;
    private readonly TextBox _apiKeyTextBox = null!;
    private readonly TextBox _resultTextBox = null!;
    private readonly Button _chooseFileButton = null!;
    private readonly Button _scanButton = null!;

    public Form1()
    {
        InitializeComponent();

        _filePathTextBox = Controls.Find("filePathTextBox", true).FirstOrDefault() as TextBox ?? throw new InvalidOperationException();
        _apiKeyTextBox = Controls.Find("apiKeyTextBox", true).FirstOrDefault() as TextBox ?? throw new InvalidOperationException();
        _resultTextBox = Controls.Find("resultTextBox", true).FirstOrDefault() as TextBox ?? throw new InvalidOperationException();
        _chooseFileButton = Controls.Find("chooseFileButton", true).FirstOrDefault() as Button ?? throw new InvalidOperationException();
        _scanButton = Controls.Find("scanButton", true).FirstOrDefault() as Button ?? throw new InvalidOperationException();

        _chooseFileButton.Click += ChooseFileButton_Click;
        _scanButton.Click += ScanButton_Click;
        DragEnter += Form1_DragEnter;
        DragDrop += Form1_DragDrop;
    }

    private void ChooseFileButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Alle Dateien|*.*",
            Title = "HashShield-Datei auswählen"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _filePathTextBox.Text = dialog.FileName;
            _resultTextBox.Text = $"Ausgewählt: {dialog.FileName}{Environment.NewLine}";
        }
    }

    private async void ScanButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_filePathTextBox.Text))
        {
            MessageBox.Show(this, "Bitte zuerst eine Datei auswählen oder per Drag & Drop ablegen.", "HashShield", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _scanButton.Enabled = false;
        _resultTextBox.Text = "Scan läuft...";

        try
        {
            var result = await ScannerEngine.ScanAsync(_filePathTextBox.Text, _apiKeyTextBox.Text);
            _resultTextBox.Text = $"Datei: {result.FileName}{Environment.NewLine}" +
                                  $"SHA-256: {result.Hash}{Environment.NewLine}" +
                                  $"Größe: {result.Size} Bytes{Environment.NewLine}" +
                                  $"Lokale Regeln: {result.LocalRuleSummary}{Environment.NewLine}" +
                                  $"VirusTotal: {result.VirusTotalSummary}{Environment.NewLine}";
        }
        catch (Exception ex)
        {
            _resultTextBox.Text = $"Fehler: {ex.Message}";
        }
        finally
        {
            _scanButton.Enabled = true;
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

        _filePathTextBox.Text = files[0];
        _resultTextBox.Text = $"Per Drag & Drop geladen: {files[0]}{Environment.NewLine}";
    }
}
