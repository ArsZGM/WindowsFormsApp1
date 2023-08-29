using System;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private TreeView treeView;
        private TextBox inputBox;
        private Button addButton;
        private Button generateReportButton;
        private RichTextBox reportBox;

        public Form1()
        {
            InitializeComponent();
            InitializeApp();
        }

        private void InitializeApp()
        {
            treeView = new TreeView() { Left = 10, Top = 10, Width = 200, Height = 280 };
            this.Controls.Add(treeView);

            inputBox = new TextBox() { Left = 220, Top = 10, Width = 190 };
            this.Controls.Add(inputBox);

            addButton = new Button() { Left = 420, Top = 10, Text = "Добавить" };
            addButton.Click += AddNode;
            this.Controls.Add(addButton);

            generateReportButton = new Button() { Left = 220, Top = 40, Text = "Отчёт" };
            generateReportButton.Click += GenerateReport;
            this.Controls.Add(generateReportButton);

            reportBox = new RichTextBox() { Left = 220, Top = 70, Width = 350, Height = 220 };
            this.Controls.Add(reportBox);
        }

        private void AddNode(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputBox.Text)) return;

            TreeNode newNode = new TreeNode(inputBox.Text);
            if (treeView.SelectedNode == null)
            {
                treeView.Nodes.Add(newNode);
            }
            else
            {
                treeView.SelectedNode.Nodes.Add(newNode);
            }
            inputBox.Clear();
        }

        private string GenerateHTMLReport(TreeNode node)
        {
            if (node == null) return "";

            var css = @"
    <style>
        table {
            width: 100%;
            border-collapse: collapse;
        }
        td {
            border: 1px solid black;
            vertical-align: top;
            text-align: center;
        }
        .connector {
            border-left: 1px solid black;
            height: 100%;
            width: 20px;
        }
    </style>";

            var html = node == treeView.Nodes[0] ? css : "";
            html += "<table>";
            html += "<tr><td rowspan='" + (node.Nodes.Count + 1) + "'>" + node.Text + "</td>";
            if (node.Nodes.Count > 0)
            {
                html += "<td>" + GenerateHTMLReport(node.Nodes[0]) + "</td></tr>";
                for (int i = 1; i < node.Nodes.Count; i++)
                {
                    html += "<tr><td>" + GenerateHTMLReport(node.Nodes[i]) + "</td></tr>";
                }
            }
            html += "</tr></table>";
            //не совсем понимаю, почему границы первых детей съезжают, надеюсь, не критично
            return html;
        }

        private void GenerateReport(object sender, EventArgs e)
        {
            if (treeView.Nodes.Count == 0) return;

            string html = "";
            foreach (TreeNode node in treeView.Nodes)
            {
                html += GenerateHTMLReport(node);
            }

            
            string filePath = "report.html";
            File.WriteAllText(filePath, html);

            
            reportBox.Text = "HTML report saved to: " + filePath;
        }
    }
}
