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
        var targetLabel = new Label();
        var targetTextBox = new TextBox();
        var chooseFolderButton = new Button();
        var quickScanButton = new Button();
        var fullScanButton = new Button();
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

        subtitleLabel.Text = "Ordner, Laufwerke oder Desktopbereiche für einen schnellen Scan auswählen. Dabei werden Hashes, lokale Signaturen und optional VirusTotal-Abfragen verwendet.";
        subtitleLabel.Font = new Font("Segoe UI", 10F);
        subtitleLabel.Location = new Point(20, 58);
        subtitleLabel.Size = new Size(840, 38);

        targetLabel.Text = "Zielpfad";
        targetLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        targetLabel.Location = new Point(20, 112);
        targetLabel.Size = new Size(120, 24);

        targetTextBox.Location = new Point(20, 138);
        targetTextBox.Size = new Size(650, 30);
        targetTextBox.ReadOnly = true;
        targetTextBox.Name = "targetTextBox";

        chooseFolderButton.Text = "Ordner wählen";
        chooseFolderButton.Location = new Point(690, 136);
        chooseFolderButton.Size = new Size(150, 32);
        chooseFolderButton.Name = "chooseFolderButton";

        quickScanButton.Text = "Quick Scan";
        quickScanButton.Location = new Point(690, 178);
        quickScanButton.Size = new Size(150, 32);
        quickScanButton.Name = "quickScanButton";

        fullScanButton.Text = "Full Scan";
        fullScanButton.Location = new Point(690, 218);
        fullScanButton.Size = new Size(150, 32);
        fullScanButton.Name = "fullScanButton";

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
        resultTextBox.Text = "Dateien oder Ordner hierher ziehen oder über \"Ordner wählen\" auswählen.\r\n";

        Controls.Add(titleLabel);
        Controls.Add(subtitleLabel);
        Controls.Add(targetLabel);
        Controls.Add(targetTextBox);
        Controls.Add(chooseFolderButton);
        Controls.Add(quickScanButton);
        Controls.Add(fullScanButton);
        Controls.Add(apiKeyLabel);
        Controls.Add(apiKeyTextBox);
        Controls.Add(resultTextBox);

        ResumeLayout(false);
    }

    #endregion
}
