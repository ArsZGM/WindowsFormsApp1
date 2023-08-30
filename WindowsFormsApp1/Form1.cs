using System;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private MyTreeNode rootNode;
        private TreeView treeView;
        private TextBox inputBox;
        private Button addButton;
        private Button generateReportButton;
        private Button showPathButton;
        private Button showParentsButton;
        private Button showChildrenButton;
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

            showPathButton = new Button() { Left = 220, Top = 70, Width = 150, Text = "Показать путь" };
            showPathButton.Click += ShowPath;
            this.Controls.Add(showPathButton);

            showParentsButton = new Button() { Left = 380, Top = 70, Width = 150, Text = "Показать родителей" };
            showParentsButton.Click += ShowParents;
            this.Controls.Add(showParentsButton);

            showChildrenButton = new Button() { Left = 540, Top = 70, Width = 150, Text = "Показать детей" };
            showChildrenButton.Click += ShowChildren;
            this.Controls.Add(showChildrenButton);

            reportBox = new RichTextBox() { Left = 220, Top = 100, Width = 550, Height = 190 };
            this.Controls.Add(reportBox);
        }

        private void UpdateTreeView()
        {
            treeView.Nodes.Clear();
            if (rootNode != null)
            {
                treeView.Nodes.Add(ConvertToTreeNode(rootNode));
            }
        }

        private TreeNode ConvertToTreeNode(MyTreeNode myNode)
        {
            var treeNode = new TreeNode(myNode.Data);
            foreach (var child in myNode.Children)
            {
                treeNode.Nodes.Add(ConvertToTreeNode(child));
            }
            return treeNode;
        }

        private MyTreeNode FindMyTreeNode(MyTreeNode current, string data)
        {
            if (current.Data == data) return current;
            foreach (var child in current.Children)
            {
                var result = FindMyTreeNode(child, data);
                if (result != null) return result;
            }
            return null;
        }

        private void AddNode(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputBox.Text)) return;

            var newNode = new MyTreeNode(inputBox.Text);
            if (treeView.SelectedNode == null)
            {
                if (rootNode == null)
                {
                    rootNode = newNode;
                }
                else
                {
                    rootNode.AddChild(newNode);
                }
            }
            else
            {
                // Найдем соответствующий узел в структуре MyTreeNode
                var selectedMyNode = FindMyTreeNode(rootNode, treeView.SelectedNode.Text);
                selectedMyNode.AddChild(newNode);
            }
            inputBox.Clear();
            UpdateTreeView();
        }

        private string GenerateHTMLReport(MyTreeNode node, bool isRoot = false)
        {
            if (node == null) return "";

            var css = isRoot ? @"
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
    </style>" : "";

            var html = css + "<table>";
            html += "<tr><td rowspan='" + (node.Children.Count + 1) + "'>" + node.Data + "</td>";
            if (node.Children.Count > 0)
            {
                html += "<td>" + GenerateHTMLReport(node.Children[0]) + "</td></tr>";
                for (int i = 1; i < node.Children.Count; i++)
                {
                    html += "<tr><td>" + GenerateHTMLReport(node.Children[i]) + "</td></tr>";
                }
            }
            html += "</tr></table>";

            return html;
        }

        private void GenerateReport(object sender, EventArgs e)
        {
            if (rootNode == null) return;

            string html = GenerateHTMLReport(rootNode, true);

            string filePath = "report.html";
            File.WriteAllText(filePath, html);

            reportBox.Text = "HTML report saved to: " + filePath;
        }

        private void ShowPath(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;

            var selectedMyNode = FindMyTreeNode(rootNode, treeView.SelectedNode.Text);
            reportBox.Text = selectedMyNode.GetPath();
        }

        private void ShowParents(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;

            var selectedMyNode = FindMyTreeNode(rootNode, treeView.SelectedNode.Text);
            reportBox.Text = string.Join(", ", selectedMyNode.GetParents());
        }

        private void ShowChildren(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null) return;

            var selectedMyNode = FindMyTreeNode(rootNode, treeView.SelectedNode.Text);
            reportBox.Text = string.Join(", ", selectedMyNode.GetChildren());
        }
    }
}
