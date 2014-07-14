using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Xml.Linq;
using Elysium;
using VisualME7.WPF.classes;
using VisualME7Logger.Configuration;
using VisualME7.WPF.viewmodel;

namespace VisualME7.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Elysium.Controls.Window
    {
        enum EditModes
        {
            View,
            Add,
            Edit
        }


        List<Profile> Profiles = new List<Profile>();
        Profile CurrentProfile = new Profile("Default Profile");

        EditModes ProfileEditMode;
        Profile SelectedProfile;
        EditModes GraphVariableEditMode;
        GraphVariable SelectedGraphVariable;
        EditModes ExpressionEditMode;
        VisualME7Logger.Session.ExpressionVariable SelectedExpression;

        public MainWindow()
        {
            //InitializeComponent();
            VisualME7Logger.Session.ME7LoggerSession.Debug = App.Debug;
        }

        void SetupGrid()
        {
            //this.dataGridView1.AutoGenerateColumns = false;

            //DataGridViewCheckBoxColumn cbColumn = new DataGridViewCheckBoxColumn();
            //cbColumn.Width = 30;
            //cbColumn.DataPropertyName = "Selected";
            //dataGridView1.Columns.Add(cbColumn);

            //DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            //column.DataPropertyName = "Name";
            //column.Name = "Name";
            //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dataGridView1.Columns.Add(column);

            //column = new DataGridViewTextBoxColumn();
            //column.DataPropertyName = "Alias";
            //column.Name = "Alias";
            //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dataGridView1.Columns.Add(column);

            //column = new DataGridViewTextBoxColumn();
            //column.DataPropertyName = "Unit";
            //column.Name = "Unit";
            //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dataGridView1.Columns.Add(column);

            //column = new DataGridViewTextBoxColumn();
            //column.DataPropertyName = "Comment";
            //column.Name = "Comment";
            //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dataGridView1.Columns.Add(column);


            //CheckBox ckBox = new CheckBox();
            ////Get the column header cell bounds
            //Rectangle rect = this.dataGridView1.GetCellDisplayRectangle(0, -1, true);
            //ckBox.Size = new Size(13, 13);
            ////Change the location of the CheckBox to make it stay on the header
            //ckBox.Location = new Point(10, 5);
            //ckBox.CheckedChanged += ckBox_CheckedChanged;
            ////Add the CheckBox into the DataGridView
            //this.dataGridView1.Controls.Add(ckBox);
        }

        void LoadSettings()
        {
            if (!Profiles.Contains(CurrentProfile))
            {
                Profiles.Add(CurrentProfile);
            }

            try
            {
                string filePath = System.IO.Path.Combine(App.ME7LoggerDirectory, "VisualME7Logger.cfg.xml");
                if (System.IO.File.Exists(filePath))
                {
                    string currentProfile = null;
                    XElement root = XElement.Load(filePath);
                    foreach (XAttribute att in root.Attributes())
                    {
                        switch (att.Name.LocalName)
                        {
                            case "CurrentProfile":
                                currentProfile = att.Value;
                                break;
                        }
                    }

                    foreach (XElement ele in root.Elements())
                    {
                        switch (ele.Name.LocalName)
                        {
                            case "Profiles":
                                this.Profiles = new List<Profile>();
                                foreach (XElement child in ele.Elements())
                                {
                                    Profile p = new Profile(string.Empty);
                                    p.Read(child);
                                    Profiles.Add(p);
                                    if (p.Name == currentProfile)
                                    {
                                        CurrentProfile = p;
                                    }
                                }
                                break;
                        }
                    }

                    if (currentProfile == null)
                    {
                        this.Profiles = new List<Profile>();
                        CurrentProfile = new Profile("Default Profile");
                        CurrentProfile.Read(root);
                        this.Profiles.Add(CurrentProfile);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("An error occurred while loading settings.\r\n\r\n{0}", e));
            }

            //this.lstProfiles.Items.Clear();
            //this.lstProfiles.Items.AddRange(this.Profiles.ToArray());
            this.LoadProfile(CurrentProfile);
        }

        private void LoadProfile(Profile profile)
        {
            CurrentProfile = profile;
            //this.Text = string.Format("VisualME7Logger - {0}", CurrentProfile.Name);

            //txtECUFile.Text = CurrentProfile.ECUFile.FilePath;
            LoadECUFile();
            //txtConfigFile.Text = CurrentProfile.ConfigFile.FilePath;
            LoadConfigFile();

            //lstGraphVariables.DataSource = CurrentProfile.DisplayOptions.GraphVariables;
            //lstExpressions.DataSource = CurrentProfile.DisplayOptions.Expressions;
            //nudResfreshRate.Value = CurrentProfile.DisplayOptions.RefreshInterval;
            //nudGraphResH.Value = CurrentProfile.DisplayOptions.GraphHRes;
            //nudGraphResV.Value = CurrentProfile.DisplayOptions.GraphVRes;
        }

        private void LoadECUFile()
        {
            //this.txtECUFile.Text = this.CurrentProfile.ECUFile.FilePath;
            this.CurrentProfile.ECUFile.Open();
            //this.loadConfigFileToolStripMenuItem.Enabled =
            //this.saveConfigFileToolStripMenuItem.Enabled =
            //this.saveConfigFileAsToolStripMenuItem.Enabled = true;
            //this.btnStartLog.Enabled = true;

            //this.cmbGraphVariableVariable.DataSource =
            //    this.CurrentProfile.ECUFile.Measurements.Values.Select(m => m.Name).ToList();

            LoadConfigFile();
        }

        private void LoadConfigFile()
        {
            //txtConfigFile.Text = this.CurrentProfile.ConfigFile.FilePath;
            this.CurrentProfile.ConfigFile.Read();

            foreach (Measurement m in this.CurrentProfile.ECUFile.Measurements.Values)
            {
                m.Selected = false;
                if (this.CurrentProfile.ConfigFile.Measurements[m.Name] != null)
                {
                    m.Selected = true;
                }
            }
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            //lblMeasurementCount.Text = string.Empty;

            IEnumerable<Measurement> measurements =
                this.CurrentProfile.ECUFile.Measurements.Values.Where(m => !string.IsNullOrEmpty(m.Alias)).OrderBy(m => m.Alias).ThenBy(m => m.Name).Union(
                this.CurrentProfile.ECUFile.Measurements.Values.Where(m => string.IsNullOrEmpty(m.Alias)).OrderBy(m => m.Alias).ThenBy(m => m.Name));

            //if (!string.IsNullOrEmpty(this.txtFilter.Text))
            //{
            //    string lookup = this.txtFilter.Text;
            //    measurements = measurements.Where(m =>
            //        m.Name.IndexOf(lookup, StringComparison.InvariantCultureIgnoreCase) > -1 ||
            //        m.Alias.IndexOf(lookup, StringComparison.InvariantCultureIgnoreCase) > -1 ||
            //        m.Comment.IndexOf(lookup, StringComparison.InvariantCultureIgnoreCase) > -1);
            //}

            List<Measurement> filtered = null;
            //if (radFilterSelected.Checked)
            //{
            //    filtered = measurements.Where(m => m.Selected).ToList();
            //}
            //else if (radFilterUnselected.Checked)
            //{
            //    filtered = measurements.Where(m => !m.Selected).ToList();
            //}
            //else
            //{
            //    filtered = measurements.ToList();
            //}
            //dataGridView1.DataSource = filtered;
            //lblMeasurementCount.Text = string.Format("Showing {0} of {1}",
            //    filtered.Count,
            //    this.CurrentProfile.ECUFile.Measurements.Values.Count());
        }

        public void btnDash_click(object sender, RoutedEventArgs e)
        {
            var wd = new wdwDash();
            wd.ShowDialog();
        }

        private void btnMeasurements_click(object sender, RoutedEventArgs e)
        {

        }
    }
}
