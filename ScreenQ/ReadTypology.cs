using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace ScreenQ
{
    class ReadTypology
    {
        Edit EditClass;

        public ContextMenu ReadTypologyFromXml(string XmlFilePath, Edit EditClass)
        {

            ContextMenu CM = new ContextMenu();

            XDocument document = XDocument.Load(XmlFilePath);

            XElement root = document.Root;
            XElement[] issueList = root.Descendants("category").ToArray();

            XElement[] ErrorCategoriesNames = issueList.Elements("categoryName").ToArray();
            XElement[] ErrorCategoriesTooltips = issueList.Elements("categoryDesc").ToArray();

            int i = 0;
            int j = 0;
            int k = 0;
            int l = 0;

            foreach(XElement ErrorCategoryName in ErrorCategoriesNames)
            {
                MenuItem mi = new MenuItem()
                {
                    Header = ErrorCategoryName.Value.ToString(),
                    ToolTip = ErrorCategoriesTooltips[i].Value.ToString(),
                    
                };
                mi.Click += EditClass.Item_Click;
                CM.Items.Add(mi);

                XElement[] Issues = issueList[i].Elements("issue").ToArray();

                foreach (XElement IssueName in Issues)
                {
                    XElement[] IssuesNames = Issues.Elements("issueName").ToArray();
                    XElement[] IssuesTooltips = Issues.Elements("issueDesc").ToArray();

                    MenuItem mi2 = new MenuItem()
                    {
                        Header = IssuesNames[j].Value.ToString(),
                        ToolTip = IssuesTooltips[j].Value.ToString()
                    };
                    mi2.Click += EditClass.Item_Click;
                    mi.Items.Add(mi2);
                    
                    XElement[] SubIssues = Issues[j].Elements("subIssue").ToArray();

                    j++;

                    foreach (XElement SubIssue in SubIssues)
                    {
                        XElement[] SubIssuesNames = SubIssues.Elements("subIssueName").ToArray();
                        XElement[] SubIssuesTooltips = SubIssues.Elements("subIssueDesc").ToArray();

                        MenuItem mi3 = new MenuItem()
                        {
                            Header = SubIssuesNames[k].Value.ToString(),
                            ToolTip = SubIssuesTooltips[k].Value.ToString()  
                        };
                        mi3.Click += EditClass.Item_Click;
                        mi2.Items.Add(mi3);

                        XElement[] SubIssues2 = SubIssues[k].Elements("subIssue2").ToArray();

                        k++;

                        foreach(XElement SubIssue2 in SubIssues2)
                        {
                            XElement[] SubIssues2Names = SubIssues2.Elements("subIssue2Name").ToArray();
                            XElement[] SubIssues2Tooltips = SubIssues2.Elements("subIssue2Desc").ToArray();

                            MenuItem mi4 = new MenuItem()
                            {
                                Header = SubIssues2Names[l].Value.ToString(),
                                ToolTip = SubIssues2Tooltips[l].Value.ToString()
                            };
                            mi4.Click += EditClass.Item_Click;
                            mi3.Items.Add(mi4);
                            l++;
                        }
                        l = 0;
                    }
                    k = 0;
                }
                j = 0;
                i++;
            }
            return CM;
        }

    }
}
