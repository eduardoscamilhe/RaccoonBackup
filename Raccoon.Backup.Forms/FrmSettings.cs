using System.Text.Json;

namespace Raccoon.Backup.Forms
{
    public partial class FrmSettings : Form
    {
        public List<string> ListPaths = new List<string>();
        public FrmSettings()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var file = txtFile.Text;
            var origin = txtOrigin.Text;
            var destination = txtDestination.Text;
            var minutes = txtMinutes.Text;
            var exactHour = txtExactHour.Text;

            string json = JsonSerializer.Serialize(new AppSettings()
            {
                DestinationFolder = destination,
                OriginFolder = origin,
                MinutePeriod = minutes != null ? int.Parse(minutes) : 0,
                ExactHour = exactHour != null ? int.Parse(exactHour) : 0
            });
            File.WriteAllText(file, json);
            lblSuccess.Text = "File saved successfully";
            lblSuccess.ForeColor = Color.Green;
        }

        private void FrmSettings_Load(object sender, EventArgs e)
        {

        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            // Always default to Folder Selection.
            folderBrowser.FileName = "Folder Selection.";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = folderBrowser.FileName;
            }
        }

        private void btnOrigin_Click(object sender, EventArgs e)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            // Always default to Folder Selection.
            folderBrowser.FileName = "Folder Selection.";

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                ListPaths.Add(Path.GetDirectoryName(folderBrowser.FileName));
                txtOrigin.Text = string.Join(';', ListPaths);
            }
        }

        private void btnDestination_Click(object sender, EventArgs e)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            // Always default to Folder Selection.
            folderBrowser.FileName = "Folder Selection.";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtDestination.Text = Path.GetDirectoryName(folderBrowser.FileName);
            }
        }
    }
}
