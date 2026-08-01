namespace HashShield.App;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        var titleLabel = new Label();
        var subtitleLabel = new Label();
        var filePathLabel = new Label();
        var filePathTextBox = new TextBox();
        var chooseFileButton = new Button();
        var scanButton = new Button();
        var apiKeyLabel = new Label();
        var apiKeyTextBox = new TextBox();
        var resultTextBox = new TextBox();

        SuspendLayout();

        Text = "HashShield";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(900, 560);
        MinimumSize = new Size(760, 480);
        AllowDrop = true;

        titleLabel.Text = "HashShield – Multi-Engine-Dateiscanner";
        titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        titleLabel.Location = new Point(20, 20);
        titleLabel.Size = new Size(580, 40);

        subtitleLabel.Text = "Dateien per Drag & Drop ablegen oder manuell auswählen. SHA-256 wird berechnet und mit lokalen Signaturen sowie VirusTotal abgeglichen.";
        subtitleLabel.Font = new Font("Segoe UI", 10F);
        subtitleLabel.Location = new Point(20, 58);
        subtitleLabel.Size = new Size(840, 38);

        filePathLabel.Text = "Dateipfad";
        filePathLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        filePathLabel.Location = new Point(20, 112);
        filePathLabel.Size = new Size(120, 24);

        filePathTextBox.Location = new Point(20, 138);
        filePathTextBox.Size = new Size(650, 30);
        filePathTextBox.ReadOnly = true;
        filePathTextBox.Name = "filePathTextBox";

        chooseFileButton.Text = "Datei wählen";
        chooseFileButton.Location = new Point(690, 136);
        chooseFileButton.Size = new Size(150, 32);
        chooseFileButton.Name = "chooseFileButton";

        scanButton.Text = "Scannen";
        scanButton.Location = new Point(690, 178);
        scanButton.Size = new Size(150, 32);
        scanButton.Name = "scanButton";

        apiKeyLabel.Text = "VirusTotal API-Key";
        apiKeyLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        apiKeyLabel.Location = new Point(20, 182);
        apiKeyLabel.Size = new Size(160, 24);

        apiKeyTextBox.Location = new Point(20, 208);
        apiKeyTextBox.Size = new Size(650, 30);
        apiKeyTextBox.PlaceholderText = "Optional: API-Key für VirusTotal";
        apiKeyTextBox.Name = "apiKeyTextBox";

        resultTextBox.Location = new Point(20, 252);
        resultTextBox.Size = new Size(820, 250);
        resultTextBox.Multiline = true;
        resultTextBox.ScrollBars = ScrollBars.Vertical;
        resultTextBox.ReadOnly = true;
        resultTextBox.Font = new Font("Consolas", 10F);
        resultTextBox.Name = "resultTextBox";
        resultTextBox.Text = "Datei hierher ziehen oder über \"Datei wählen\" auswählen.\r\n";

        Controls.Add(titleLabel);
        Controls.Add(subtitleLabel);
        Controls.Add(filePathLabel);
        Controls.Add(filePathTextBox);
        Controls.Add(chooseFileButton);
        Controls.Add(scanButton);
        Controls.Add(apiKeyLabel);
        Controls.Add(apiKeyTextBox);
        Controls.Add(resultTextBox);

        ResumeLayout(false);
    }

    #endregion
}
