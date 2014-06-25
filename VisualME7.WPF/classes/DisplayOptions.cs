using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace VisualME7.WPF.classes
{
    public class DisplayOptions
    {
        public int RefreshInterval = 35;
        public int GraphVRes = 1000;
        public int GraphHRes = 1200;
        public List<GraphVariable> GraphVariables = new List<GraphVariable>();
        public List<VisualME7Logger.Session.ExpressionVariable> Expressions = new List<VisualME7Logger.Session.ExpressionVariable>();

        public XElement Write()
        {
            XElement retval = new XElement("DisplayOptions");

            retval.Add(new XAttribute("RefreshInterval", this.RefreshInterval));
            retval.Add(new XAttribute("GraphVRes", this.GraphVRes));
            retval.Add(new XAttribute("GraphHRes", this.GraphHRes));

            XElement expressionsEle = new XElement("Expressions");
            foreach (var exp in Expressions)
            {
                expressionsEle.Add(exp.Write());
            }
            retval.Add(expressionsEle);

            XElement graphVarsEle = new XElement("GraphVariables");
            foreach (GraphVariable gv in this.GraphVariables)
            {
                graphVarsEle.Add(gv.Write());
            }
            retval.Add(graphVarsEle);

            return retval;
        }

        public void Read(XElement ele)
        {
            foreach (XAttribute att in ele.Attributes())
            {
                switch (att.Name.LocalName)
                {
                    case "RefreshInterval":
                        this.RefreshInterval = int.Parse(att.Value);
                        break;
                    case "GraphVRes":
                        this.GraphVRes = int.Parse(att.Value);
                        break;
                    case "GraphHRes":
                        this.GraphHRes = int.Parse(att.Value);
                        break;
                }
            }

            foreach (XElement child in ele.Elements())
            {
                switch (child.Name.LocalName)
                {
                    case "Expressions":
                        this.ReadExpressions(child);
                        break;
                    case "GraphVariables":
                        this.ReadGraphVariables(child);
                        break;
                }
            }
        }

        public DisplayOptions Clone()
        {
            DisplayOptions clone = new DisplayOptions();
            clone.RefreshInterval = this.RefreshInterval;
            clone.GraphVRes = this.GraphVRes;
            clone.GraphHRes = this.GraphHRes;

            clone.Expressions = new List<VisualME7Logger.Session.ExpressionVariable>();
            foreach (var ev in this.Expressions)
            {
                clone.Expressions.Add(ev.Clone());
            }

            clone.GraphVariables = new List<GraphVariable>();
            foreach (GraphVariable gv in this.GraphVariables)
            {
                clone.GraphVariables.Add(gv.Clone());
            }
            return clone;
        }

        private void ReadGraphVariables(XElement ele)
        {
            foreach (XElement e in ele.Elements())
            {
                GraphVariable v = new GraphVariable();
                v.Read(e);
                GraphVariables.Add(v);
            }
        }

        private void ReadExpressions(XElement ele)
        {
            foreach (XElement e in ele.Elements())
            {
                VisualME7Logger.Session.ExpressionVariable v = new VisualME7Logger.Session.ExpressionVariable();
                v.Read(e);
                Expressions.Add(v);
            }
        }
    }
}
