﻿using Cellphone_Service.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Cellphone_Service
{
    public partial class MainForm : Form
    {
        #region Form Loading

        public List<Client> Clients;

        public MainForm()
        {
            InitializeComponent();
            Clients = new List<Client>();
            Deserialize(true);
        }

        #endregion

        #region Client Buttons

        private void AddClientBtn_Click(object sender, EventArgs e)
        {
            Client client = new Client();
            AddEditClientForm form = new AddEditClientForm(client);
            if (DialogResult.OK == form.ShowDialog())
            {
                Clients.Add(client);
                DisplayClients();
                setAllButtons(false);
            }
        }

        private void EditClientBtn_Click(object sender, EventArgs e)
        {
            if (ClientsTreeView.SelectedNode.Tag == null)
                return;
            TreeNode node = ClientsTreeView.SelectedNode;
            if (node != null && node.Tag.GetType() == typeof(Client))
            {
                Client client = (Client)node.Tag;
                AddEditClientForm form = new AddEditClientForm(client);
                var dialogResult = form.ShowDialog();
                if (DialogResult.OK == dialogResult)
                {
                    DisplayClients();
                }
                if (DialogResult.Cancel == dialogResult)
                {
                    ClientsTreeView.SelectedNode = null;
                }
                setAllButtons(false);
            }
        }

        private void DeleteClientBtn_Click(object sender, EventArgs e)
        {
            if (ClientsTreeView.SelectedNode.Tag == null)
                return;
            TreeNode node = ClientsTreeView.SelectedNode;
            if (node != null && node.Tag.GetType() == typeof(Client))
            {
                Client client = (Client)node.Tag;
                Clients.Remove(client);
                DisplayClients();
                setAllButtons(false);
            }
        }

        private void setClientButtons(bool dec)
        {
            if(dec == false)
            {
                EditClientBtn.Enabled = false;
                EditClientBtn.BackColor = Color.White;
                DeleteClientBtn.Enabled = false;
                DeleteClientBtn.BackColor = Color.White;
            }
            else
            {
                EditClientBtn.Enabled = true;
                EditClientBtn.BackColor = Color.Yellow;
                DeleteClientBtn.Enabled = true;
                DeleteClientBtn.BackColor = Color.Red;
            }
        }

        #endregion

        #region Extra Options Buttons

        private void AddExtraOptionBtn_Click(object sender, EventArgs e)
        {
            if (ClientsTreeView.SelectedNode.Tag == null)
                return;
            TreeNode node = ClientsTreeView.SelectedNode;
            if (node != null && node.Tag.GetType() == typeof(Client))
            {
                Client client = (Client)node.Tag;
                ExtraOption extraOption = new ExtraOption();
                AddEditExtraOptionForm form = new AddEditExtraOptionForm(extraOption);
                if (DialogResult.OK == form.ShowDialog())
                {
                    client.AddExtraOption(extraOption);
                    DisplayClients();
                }
                setAllButtons(false);
            }
        }

        private void EditExtraOptionBtn_Click(object sender, EventArgs e)
        {
            if (ClientsTreeView.SelectedNode.Tag == null)
                return;
            TreeNode node = ClientsTreeView.SelectedNode;
            if (node != null && node.Tag.GetType() == typeof(ExtraOption))
            {
                ExtraOption extraOption = (ExtraOption)node.Tag;
                AddEditExtraOptionForm form = new AddEditExtraOptionForm(extraOption);
                if (DialogResult.OK == form.ShowDialog())
                {
                    DisplayClients();
                }
                setAllButtons(false);
            }
        }

        private void DeleteExtraOptionBtn_Click(object sender, EventArgs e)
        {
            if (ClientsTreeView.SelectedNode.Tag == null)
                return;
            TreeNode node = ClientsTreeView.SelectedNode;
            if (node != null && node.Tag.GetType() == typeof(ExtraOption))
            {
                TreeNode parent = ClientsTreeView.SelectedNode.Parent.Parent;
                if (node != null && parent.GetType() == typeof(Client))
                {
                    ExtraOption extraOption = (ExtraOption)node.Tag;
                    Client client = (Client)parent.Tag;
                    client.ExtraOptions.Remove(extraOption);
                    DisplayClients();
                    setAllButtons(false);
                }
            }
        }

        private void setExtraOptionButtons(bool dec)
        {
            if(dec == false)
            {
                AddExtraOptionBtn.Enabled = false;
                AddExtraOptionBtn.BackColor = Color.White;
                EditExtraOptionBtn.Enabled = false;
                EditExtraOptionBtn.BackColor = Color.White;
                DeleteExtraOptionBtn.Enabled = false;
                DeleteExtraOptionBtn.BackColor = Color.White;
            }
            else
            {
                AddExtraOptionBtn.Enabled = true;
                AddExtraOptionBtn.BackColor = Color.Lime;
                EditExtraOptionBtn.Enabled = true;
                EditExtraOptionBtn.BackColor = Color.Yellow;
                DeleteExtraOptionBtn.Enabled = true;
                DeleteExtraOptionBtn.BackColor = Color.Red;
            }
        }

        #endregion

        #region Client Tree View

        private void DisplayClients()
        {
            var savedExpansionState = ClientsTreeView.Nodes.GetExpansionState();
            ClientsTreeView.Nodes.Clear();
            Clients.Sort();
            int totalOptions = 0;

            for (int i = 0; i < Clients.Count; i++)
            {
                Clients[i].Update();
                TreeNode node = new TreeNode(Clients[i].Name);
                node.Tag = Clients[i];

                TreeNode totalBillNode = new TreeNode("Total Bill: " + Clients[i].TotalBill.ToString());
                node.Nodes.Add(totalBillNode);
                TreeNode subscriptionName = new TreeNode("Package: " + Clients[i].Subscription.Name);
                node.Nodes.Add(subscriptionName);
                TreeNode subscriptionPrice = new TreeNode("Subscription Price: " + Clients[i].Subscription.Price.ToString());
                node.Nodes.Add(subscriptionPrice);
                TreeNode _clientType = new TreeNode("Type: " + Clients[i].Subscription.ClientType.ToString());
                node.Nodes.Add(_clientType);
                if (Clients[i].ExtraOptions.Count > 0)
                {
                    TreeNode _extraOptions = new TreeNode("Extra Options");
                    _extraOptions.Expand();
                    for (int j = 0; j < Clients[i].ExtraOptions.Count; j++)
                    {
                        TreeNode nawd = new TreeNode("Option - " + Clients[i].ExtraOptions[j].Name);
                        TreeNode _description = new TreeNode("Description: " + Clients[i].ExtraOptions[j].Description);
                        nawd.Nodes.Add(_description);
                        TreeNode _price = new TreeNode("Price: " + Clients[i].ExtraOptions[j].Price.ToString());
                        nawd.Nodes.Add(_price);
                        nawd.Tag = Clients[i].ExtraOptions[j];
                        _extraOptions.Nodes.Add(nawd);
                        totalOptions++;
                    }
                    node.Nodes.Add(_extraOptions);
                }
                ClientsTreeView.Nodes.Add(node);
            }
            ClientsTreeView.Nodes.SetExpansionState(savedExpansionState);

            TotalClientsStrip.Text = "Total Clients: " + Clients.Count;
            TotalOptionsStrip.Text = "Total Options: " + totalOptions;
        }

        private void ClientsTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // this is retarded lmao
            ClientsTreeView.SelectedNode = e.Node;

            if (ClientsTreeView.SelectedNode != null)
            {
                if (ClientsTreeView.SelectedNode.Tag != null)
                {
                    if (ClientsTreeView.SelectedNode.Tag.GetType() == typeof(Client))
                    {
                        setClientButtons(true);
                        setExtraOptionButtons(false);
                        AddExtraOptionBtn.Enabled = true;
                        AddExtraOptionBtn.BackColor = Color.Lime;
                    }

                    if (ClientsTreeView.SelectedNode.Tag.GetType() == typeof(ExtraOption))
                    {
                        setClientButtons(false);
                        setExtraOptionButtons(true);
                        AddExtraOptionBtn.Enabled = false;
                        AddExtraOptionBtn.BackColor = Color.White;
                    }
                }
                else
                {
                    setAllButtons(false);
                }
            }
        }

        private void ClientsTreeView_Leave(object sender, EventArgs e)
        {
            if (ClientsTreeView.SelectedNode == null)
            {
                setAllButtons(false);
            }
        }

        private void setAllButtons(bool dec)
        {
            setClientButtons(dec);
            setExtraOptionButtons(dec);
        }

        #endregion

        #region Strip Menu

        private void serializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream f = File.OpenWrite("data.bin"))
            {
                formatter.Serialize(f, Clients);
            }
        }

        private void Deserialize(bool start)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream f = null;
            try
            {
                f = File.OpenRead("data.bin");
                Clients = (List<Client>)formatter.Deserialize(f);
                DisplayClients();
            }
            catch (IOException ioe)
            {
                if(!start)
                {
                    MessageBox.Show(ioe.Message);
                }
            }
            finally
            {
                if (f != null)
                    f.Close();
            }
        }

        private void deserializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Deserialize(false);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new HelpForm().ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Status Strip Menu

        private void ExportReportBtn_Click(object sender, EventArgs e)
        {
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sr = new StreamWriter(saveFileDialog.FileName);
                foreach (var client in Clients)
                {
                    sr.WriteLine(client.ToString());
                    sr.Write("\n");
                }
                if(Clients.Count == 0)
                {
                    sr.WriteLine("No clients found");
                }
                sr.Close();
            }
        }

        private void ViewStatisticsBtn_Click(object sender, EventArgs e)
        {
            ChartForm form = new ChartForm(Clients);
            form.ShowDialog();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            PrintForm printForm = new PrintForm(Clients);
            printForm.ShowDialog();
        }

        #endregion

        #region Context Strip Menu

        private void ContextMenuStrip_Opened(object sender, EventArgs e)
        {
            editClientToolStripMenuItem.Enabled = false;
            deleteClientToolStripMenuItem.Enabled = false;
            addExtraOptionToolStripMenuItem.Enabled = false;
            editExtraOptionToolStripMenuItem.Enabled = false;
            deleteExtraOptionToolStripMenuItem.Enabled = false;

            if (ClientsTreeView.SelectedNode != null)
            {
                if (ClientsTreeView.SelectedNode.Tag != null)
                {
                    if (ClientsTreeView.SelectedNode.Tag.GetType() == typeof(Client))
                    {
                        editClientToolStripMenuItem.Enabled = true;
                        deleteClientToolStripMenuItem.Enabled = true;
                        addExtraOptionToolStripMenuItem.Enabled = true;
                    }

                    if (ClientsTreeView.SelectedNode.Tag.GetType() == typeof(ExtraOption))
                    {
                        editExtraOptionToolStripMenuItem.Enabled = true;
                        deleteExtraOptionToolStripMenuItem.Enabled = true;
                    }
                }
            }
        }

        private void reloadViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayClients();
        }

        private void addClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddClientBtn_Click(sender, e);
        }

        private void editClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditClientBtn_Click(sender, e);
        }

        private void deleteClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteClientBtn_Click(sender, e);
        }

        private void addExtraOptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddExtraOptionBtn_Click(sender, e);
        }

        private void editExtraOptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditExtraOptionBtn_Click(sender, e);
        }

        private void deleteExtraOptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteExtraOptionBtn_Click(sender, e);
        }

        #endregion

        #region Alt Shortcuts

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.Control && e.KeyCode == Keys.A)
            {
                AddExtraOptionBtn_Click(sender, e);
                return;
            }

            if (e.Alt && e.Control && e.KeyCode == Keys.E)
            {
                EditExtraOptionBtn_Click(sender, e);
                return;
            }

            if (e.Alt && e.Control && e.KeyCode == Keys.D)
            {
                DeleteExtraOptionBtn_Click(sender, e);
                return;
            }

            if (e.Alt && e.KeyCode == Keys.A)
            {
                AddClientBtn_Click(sender, e);
                return;
            }

            if (e.Alt && e.KeyCode == Keys.E)
            {
                EditClientBtn_Click(sender, e);
                return;
            }

            if (e.Alt && e.KeyCode == Keys.D)
            {
                DeleteClientBtn_Click(sender, e);
                return;
            }
        }

        #endregion
    }

    #region Tree View Extension

    // got this from https://stackoverflow.com/questions/8308258/expand-selected-node-after-refresh-treeview-in-c-sharp
    public static class TreeViewExtensions
    {
        public static List<string> GetExpansionState(this TreeNodeCollection nodes)
        {
            return nodes.Descendants()
                        .Where(n => n.IsExpanded)
                        .Select(n => n.FullPath)
                        .ToList();
        }

        public static void SetExpansionState(this TreeNodeCollection nodes, List<string> savedExpansionState)
        {
            foreach (var node in nodes.Descendants()
                                      .Where(n => savedExpansionState.Contains(n.FullPath)))
            {
                node.Expand();
            }
        }

        public static IEnumerable<TreeNode> Descendants(this TreeNodeCollection c)
        {
            foreach (var node in c.OfType<TreeNode>())
            {
                yield return node;

                foreach (var child in node.Nodes.Descendants())
                {
                    yield return child;
                }
            }
        }
    }

    #endregion
}
