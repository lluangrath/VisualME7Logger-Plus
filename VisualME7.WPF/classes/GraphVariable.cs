using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using System.Drawing;

namespace VisualME7.WPF.classes
{
    public class GraphVariable
    {
        public bool Active { get; set; }
        public string Variable { get; set; }
        public string Name { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public Color LineColor { get; set; }
        public int LineThickness { get; set; }
        public ChartDashStyle LineStyle { get; set; }

        public GraphVariable()
        {
            LineColor = Color.Red;
            Min = 0;
            Max = 100;
            Name = "";
            Variable = "";
            LineThickness = 1;
            LineStyle = ChartDashStyle.Solid;
            Active = true;
        }

        public XElement Write()
        {
            XElement retval = new XElement("GraphVariable");
            retval.Add(new XAttribute("Active", this.Active));
            retval.Add(new XAttribute("Variable", this.Variable));
            retval.Add(new XAttribute("Name", this.Name));
            retval.Add(new XAttribute("Min", this.Min));
            retval.Add(new XAttribute("Max", this.Max));
            retval.Add(new XAttribute("LineColor", this.LineColor.ToArgb()));
            retval.Add(new XAttribute("LineThickness", this.LineThickness));
            retval.Add(new XAttribute("LineStyle", (int)this.LineStyle));
            return retval;
        }

        public void Read(XElement ele)
        {
            foreach (XAttribute att in ele.Attributes())
            {
                switch (att.Name.LocalName)
                {
                    case "Active":
                        Active = bool.Parse(att.Value);
                        break;
                    case "Variable":
                        Variable = att.Value;
                        break;
                    case "Name":
                        Name = att.Value;
                        break;
                    case "Min":
                        Min = decimal.Parse(att.Value);
                        break;
                    case "Max":
                        Max = decimal.Parse(att.Value);
                        break;
                    case "LineColor":
                        LineColor = Color.FromArgb(int.Parse(att.Value));
                        break;
                    case "LineThickness":
                        LineThickness = int.Parse(att.Value);
                        break;
                    case "LineStyle":
                        LineStyle = (ChartDashStyle)int.Parse(att.Value);
                        break;
                }
            }
        }

        public GraphVariable Clone()
        {
            GraphVariable clone = new GraphVariable();
            clone.Active = this.Active;
            clone.Variable = this.Variable;
            clone.Name = this.Name;
            clone.Min = this.Min;
            clone.Max = this.Max;
            clone.LineColor = this.LineColor;
            clone.LineThickness = this.LineThickness;
            clone.LineStyle = this.LineStyle;
            return clone;
        }

        public override string ToString()
        {
            return string.Format("{0}, Name: {1}", this.Variable, this.Name);
        }
    }
}
