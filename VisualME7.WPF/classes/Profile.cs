using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualME7Logger.Configuration;
using System.Xml.Linq;

namespace VisualME7.WPF.classes
{
    public class Profile
    {
        public string Name { get; set; }
        public ECUFile ECUFile { get; set; }
        public ConfigFile ConfigFile { get; set; }
        public VisualME7Logger.Session.LoggerOptions LoggerOptions { get; set; }
        public VisualME7Logger.Output.ChecksumInfo ChecksumInfo { get; set; }
        public DisplayOptions DisplayOptions { get; set; }

        public Profile()
        {
            this.LoggerOptions = new VisualME7Logger.Session.LoggerOptions(App.ME7LoggerDirectory);
            this.ChecksumInfo = new VisualME7Logger.Output.ChecksumInfo();
            this.DisplayOptions = new DisplayOptions();
            this.ECUFile = new ECUFile(string.Empty);
            this.ConfigFile = new ConfigFile(string.Empty);
        }

        public Profile(string name)
            : this()
        {
            this.Name = name;
        }

        public void Read(XElement ele)
        {
            foreach (XAttribute att in ele.Attributes())
            {
                switch (att.Name.LocalName)
                {
                    case "Name":
                        this.Name = att.Value;
                        break;
                    case "ECUFile":
                        this.ECUFile = new ECUFile(att.Value);
                        break;
                    case "ConfigFile":
                        this.ConfigFile = new ConfigFile(att.Value);
                        break;
                }
            }

            foreach (XElement childEle in ele.Elements())
            {
                switch (childEle.Name.LocalName)
                {
                    case "Options":
                        this.LoggerOptions.Read(childEle);
                        break;
                    case "ChecksumInfo":
                        this.ChecksumInfo.Read(childEle);
                        break;
                    case "DisplayOptions":
                        this.DisplayOptions.Read(childEle);
                        break;
                }
            }
        }

        public XElement Write()
        {
            XElement retval = new XElement("Profile");
            retval.Add(new XAttribute("Name", this.Name));
            retval.Add(new XAttribute("ECUFile", this.ECUFile.FilePath));
            retval.Add(new XAttribute("ConfigFile", this.ConfigFile.FilePath));
            retval.Add(this.LoggerOptions.Write());
            retval.Add(this.ChecksumInfo.Write());
            retval.Add(this.DisplayOptions.Write());
            return retval;
        }

        public Profile Clone()
        {
            Profile clone = new Profile();
            clone.Name = this.Name;
            clone.ECUFile = new ECUFile(this.ECUFile.FilePath);
            clone.ConfigFile = new ConfigFile(this.ConfigFile.FilePath);
            clone.LoggerOptions = this.LoggerOptions.Clone();
            clone.ChecksumInfo = this.ChecksumInfo.Clone();
            clone.DisplayOptions = this.DisplayOptions.Clone();
            return clone;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
