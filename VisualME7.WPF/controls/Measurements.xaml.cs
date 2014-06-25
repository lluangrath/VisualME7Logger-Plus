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
using VisualME7.WPF.classes;
using VisualME7Logger.Configuration;
using Microsoft.Win32;

namespace VisualME7.WPF.controls
{
    /// <summary>
    /// Interaction logic for Measurements.xaml
    /// </summary>
    public partial class Measurements : UserControl
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
        MainWindow mw;
        VisualME7Logger.Session.ExpressionVariable SelectedExpression;

        public Measurements(MainWindow mw)
        {
            this.mw = mw;
            VisualME7Logger.Session.ME7LoggerSession.Debug = App.Debug;

            InitializeComponent();

            //SetupGrid();

            //this.LoadSettings();
            //this.cmbGraphVariableStyle.DataSource = Enum.GetValues(typeof(ChartDashStyle));
        }

        //void SetupGrid()
        //{
        //    this.dgMeasurements.AutoGenerateColumns = false;

        //    DataGridCheckBoxColumn cbColumn = new DataGridCheckBoxColumn();
        //    cbColumn.Width = 30;
        //    //cbColumn.DataPropertyName = "Selected";
        //    dgMeasurements.Columns.Add(cbColumn);

        //    DataGridTextColumn column = new DataGridTextColumn();
        //    //column.DataPropertyName = "Name";
        //    //column.Name = "Name";
        //    //column.AutoSizeMode = DataGridAutoSizeColumnMode.AllCells;
        //    dgMeasurements.Columns.Add(column);

        //    column = new DataGridTextColumn();
        //    //column.DataPropertyName = "Alias";
        //    //column.Name = "Alias";
        //    //column.AutoSizeMode = DataGridAutoSizeColumnMode.AllCells;
        //    dgMeasurements.Columns.Add(column);

        //    column = new DataGridTextColumn();
        //    //column.DataPropertyName = "Unit";
        //    //column.Name = "Unit";
        //    //column.AutoSizeMode = DataGridAutoSizeColumnMode.AllCells;
        //    dgMeasurements.Columns.Add(column);

        //    column = new DataGridTextColumn();
        //    //column.DataPropertyName = "Comment";
        //    //column.Name = "Comment";
        //    //column.AutoSizeMode = DataGridAutoSizeColumnMode.AllCells;
        //    dgMeasurements.Columns.Add(column);


        //    CheckBox ckBox = new CheckBox();
        //    //Get the column header cell bounds
        //    Rectangle rect = this.dgMeasurements.GetCellDisplayRectangle(0, -1, true);
        //    ckBox.Size = new Size(13, 13);
        //    //Change the location of the CheckBox to make it stay on the header
        //    ckBox.Location = new Point(10, 5);
        //    ckBox.CheckedChanged += ckBox_CheckedChanged;
        //    //Add the CheckBox into the DataGrid
        //    this.dgMeasurements.Controls.Add(ckBox);
        //}

        //private void LoadECUFile()
        //{
        //    this.txtECUFile.Text = this.CurrentProfile.ECUFile.FilePath;
        //    this.CurrentProfile.ECUFile.Open();
        //    this.loadConfigFileToolStripMenuItem.Enabled =
        //    this.saveConfigFileToolStripMenuItem.Enabled =
        //    this.saveConfigFileAsToolStripMenuItem.Enabled = true;
        //    this.btnStartLog.Enabled = true;

        //    this.cmbGraphVariableVariable.DataSource =
        //        this.CurrentProfile.ECUFile.Measurements.Values.Select(m => m.Name).ToList();

        //    LoadConfigFile();
        //}

        //private void ApplyFilter()
        //{
        //    lblMeasurementCount.Text = string.Empty;

        //    IEnumerable<Measurement> measurements =
        //        this.CurrentProfile.ECUFile.Measurements.Values.Where(m => !string.IsNullOrEmpty(m.Alias)).OrderBy(m => m.Alias).ThenBy(m => m.Name).Union(
        //        this.CurrentProfile.ECUFile.Measurements.Values.Where(m => string.IsNullOrEmpty(m.Alias)).OrderBy(m => m.Alias).ThenBy(m => m.Name));

        //    if (!string.IsNullOrEmpty(this.txtFilter.Text))
        //    {
        //        string lookup = this.txtFilter.Text;
        //        measurements = measurements.Where(m =>
        //            m.Name.IndexOf(lookup, StringComparison.InvariantCultureIgnoreCase) > -1 ||
        //            m.Alias.IndexOf(lookup, StringComparison.InvariantCultureIgnoreCase) > -1 ||
        //            m.Comment.IndexOf(lookup, StringComparison.InvariantCultureIgnoreCase) > -1);
        //    }

        //    List<Measurement> filtered = null;
        //    if (radFilterSelected.Checked)
        //    {
        //        filtered = measurements.Where(m => m.Selected).ToList();
        //    }
        //    else if (radFilterUnselected.Checked)
        //    {
        //        filtered = measurements.Where(m => !m.Selected).ToList();
        //    }
        //    else
        //    {
        //        filtered = measurements.ToList();
        //    }
        //    dataGridView1.DataSource = filtered;
        //    lblMeasurementCount.Text = string.Format("Showing {0} of {1}",
        //        filtered.Count,
        //        this.CurrentProfile.ECUFile.Measurements.Values.Count());
        //}

        //private ConfigFile SaveConfigFile(bool saveNew = false)
        //{
        //    if (this.CurrentProfile.ECUFile != null)
        //    {
        //        Measurements ms = new Measurements();
        //        foreach (Measurement m in this.CurrentProfile.ECUFile.Measurements.Values.Where(m => m.Selected))
        //        {
        //            ms.AddMeasurement(m);
        //        }

        //        if (ms.Values.Count() > 0)
        //        {
        //            if (saveNew || string.IsNullOrEmpty(this.txtConfigFile.Text))
        //            {
        //                SaveFileDialog d = new SaveFileDialog();
        //                d.Title = "Save Config File As...";
        //                d.InitialDirectory =
        //                    string.IsNullOrWhiteSpace(this.txtConfigFile.Text) ?
        //                    System.IO.Path.Combine(Program.ME7LoggerDirectory, "logs") :
        //                    System.IO.Path.GetDirectoryName(this.txtConfigFile.Text);
        //                d.FileName = System.IO.Path.GetFileName(this.txtConfigFile.Text);
        //                if (d.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        //                {
        //                    return null;
        //                }
        //                this.txtConfigFile.Text = d.FileName;
        //            }

        //            ConfigFile configFile = new ConfigFile(txtConfigFile.Text, this.CurrentProfile.ECUFile.FileName, ms);
        //            configFile.Write();
        //            return configFile;
        //        }
        //        else
        //        {
        //            MessageBox.Show("No Measurements Selected");
        //        }
        //    }
        //    return null;
        //}

        //private void LoadConfigFile()
        //{
        //    txtConfigFile.Text = this.CurrentProfile.ConfigFile.FilePath;
        //    this.CurrentProfile.ConfigFile.Read();

        //    foreach (Measurement m in this.CurrentProfile.ECUFile.Measurements.Values)
        //    {
        //        m.Selected = false;
        //        if (this.CurrentProfile.ConfigFile.Measurements[m.Name] != null)
        //        {
        //            m.Selected = true;
        //        }
        //    }
        //    ApplyFilter();
        //}

        //public void SaveSettings()
        //{
        //    this.CurrentProfile.DisplayOptions.RefreshInterval = (int)this.nudResfreshRate.Value;
        //    this.CurrentProfile.DisplayOptions.GraphHRes = (int)this.nudGraphResH.Value;
        //    this.CurrentProfile.DisplayOptions.GraphVRes = (int)this.nudGraphResV.Value;

        //    try
        //    {
        //        XElement root = new XElement("VisualME7LoggerSettings");
        //        root.Add(new XAttribute("CurrentProfile", this.CurrentProfile.Name));
        //        XElement profiles = new XElement("Profiles");
        //        foreach (Profile p in this.Profiles)
        //        {
        //            profiles.Add(p.Write());
        //        }
        //        root.Add(profiles);
        //        root.Save(System.IO.Path.Combine(App.ME7LoggerDirectory, "VisualME7Logger.cfg.xml"));
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(string.Format("An error occurred while saving settings file\r\n{0}", e.ToString()));
        //    }
        //}

        //void LoadSettings()
        //{
        //    if (!Profiles.Contains(CurrentProfile))
        //    {
        //        Profiles.Add(CurrentProfile);
        //    }

        //    try
        //    {
        //        string filePath = System.IO.Path.Combine(App.ME7LoggerDirectory, "VisualME7Logger.cfg.xml");
        //        if (System.IO.File.Exists(filePath))
        //        {
        //            string currentProfile = null;
        //            XElement root = XElement.Load(filePath);
        //            foreach (XAttribute att in root.Attributes())
        //            {
        //                switch (att.Name.LocalName)
        //                {
        //                    case "CurrentProfile":
        //                        currentProfile = att.Value;
        //                        break;
        //                }
        //            }

        //            foreach (XElement ele in root.Elements())
        //            {
        //                switch (ele.Name.LocalName)
        //                {
        //                    case "Profiles":
        //                        this.Profiles = new List<Profile>();
        //                        foreach (XElement child in ele.Elements())
        //                        {
        //                            Profile p = new Profile(string.Empty);
        //                            p.Read(child);
        //                            Profiles.Add(p);
        //                            if (p.Name == currentProfile)
        //                            {
        //                                CurrentProfile = p;
        //                            }
        //                        }
        //                        break;
        //                }
        //            }

        //            if (currentProfile == null)
        //            {
        //                this.Profiles = new List<Profile>();
        //                CurrentProfile = new Profile("Default Profile");
        //                CurrentProfile.Read(root);
        //                this.Profiles.Add(CurrentProfile);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(string.Format("An error occurred while loading settings.\r\n\r\n{0}", e));
        //    }

        //    this.lstProfiles.Items.Clear();
        //    this.lstProfiles.Items.AddRange(this.Profiles.ToArray());
        //    this.LoadProfile(CurrentProfile);
        //}

        //private void LoadProfile(Profile profile)
        //{
        //    CurrentProfile = profile;
        //    this.Text = string.Format("VisualME7Logger - {0}", CurrentProfile.Name);

        //    txtECUFile.Text = CurrentProfile.ECUFile.FilePath;
        //    LoadECUFile();
        //    txtConfigFile.Text = CurrentProfile.ConfigFile.FilePath;
        //    LoadConfigFile();

        //    lstGraphVariables.DataSource = CurrentProfile.DisplayOptions.GraphVariables;
        //    lstExpressions.DataSource = CurrentProfile.DisplayOptions.Expressions;
        //    nudResfreshRate.Value = CurrentProfile.DisplayOptions.RefreshInterval;
        //    nudGraphResH.Value = CurrentProfile.DisplayOptions.GraphHRes;
        //    nudGraphResV.Value = CurrentProfile.DisplayOptions.GraphVRes;
        //}
    }
}
