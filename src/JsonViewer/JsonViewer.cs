using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics.Eventing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Threading;
using EPocalipse.Json.Viewer.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PirateX.Protocol;

namespace EPocalipse.Json.Viewer
{
    public partial class JsonViewer : UserControl
    {
        private string _json { get; set; }
        private int _maxErrorCount = 25;
        private ErrorDetails _errorDetails;
        private PluginsManager _pluginsManager = new PluginsManager();
        bool _updating;
        Control _lastVisualizerControl;

        public JsonViewer()
        {
            InitializeComponent();

            try
            {
                _pluginsManager.Initialize();
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(Resources.ConfigMessage, e.Message), "Json Viewer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]

        private IPirateXResponseInfo _pirateXPackage;

        public IPirateXResponseInfo ResponseInfo {
            get { return _pirateXPackage; }
            set
            {
                _pirateXPackage = value;

                Redraw();
            }
        }

        [DefaultValue(25)]
        public int MaxErrorCount
        {
            get
            {
                return _maxErrorCount;
            }
            set
            {
                _maxErrorCount = value;
            }
        }

        private void Redraw()
        {
            if (_pirateXPackage == null)
                return;

            var response = _pirateXPackage;
            var text = _pirateXPackage.ContentBytes== null?string.Empty: Encoding.UTF8.GetString(_pirateXPackage.ContentBytes);

            
            var viewitem = listViewIn.Items.Add(DateTime.Now.ToString());
            viewitem.SubItems.Add($"{string.Join("&", response.Headers.AllKeys.Select(a=>a+"="+ response.Headers[a]))}");

            if (string.IsNullOrEmpty(text))
            {
                if (response.Headers.AllKeys.Contains("errorCode") && response.Headers.AllKeys.Contains("errorMsg"))
                {
                    text = $@"
                        {"{"}""errorCode"":""{response.Headers["errorCode"]}"",
                             ""errorMsg"":""{response.Headers["errorMsg"]}""{"}"}";
                }
            }
            viewitem.SubItems.Add(text);
            _json = text;
            txtJson.Text = text;

            try
            {
                tvJson.BeginUpdate();
                try
                {
                    Reset();
                    if (!String.IsNullOrEmpty(_json))
                    {
                        JsonObjectTree tree = JsonObjectTree.Parse(_json);
                        VisualizeJsonTree(tree);
                    }
                }
                finally
                {
                    tvJson.EndUpdate();
                }
            }
            catch (JsonParseError e)
            {
                GetParseErrorDetails(e);
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        private void Reset()
        {
            ClearInfo();
            tvJson.Nodes.Clear();
            //pnlVisualizer.Controls.Clear();
            _lastVisualizerControl = null;
            //cbVisualizers.Items.Clear();
        }

        private void GetParseErrorDetails(Exception parserError)
        {
            var strReader = new UnbufferedStringReader(_json);

            /*
            using (var reader = new JsonReader(strReader))
            {
                try
                {
                    while (reader.Read()) { };
                }
                catch (Exception e)
                {
                    _errorDetails._err = e.Message;
                    _errorDetails._pos = strReader.Position;
                }
            }
            */
            if (_errorDetails.Error == null)
                _errorDetails._err = parserError.Message;
            if (_errorDetails.Position == 0)
                _errorDetails._pos = _json.Length;
            if (!txtJson.ContainsFocus)
                MarkError(_errorDetails);
            ShowInfo(_errorDetails);
        }


        public void NewOut(IPirateXPackage package)
        {
            var request = new PirateXRequestInfo(package);
            var text = package.ContentBytes == null ? string.Empty : Encoding.UTF8.GetString(package.ContentBytes);

            _json = text;
            var viewitem = listViewOut.Items.Add(DateTime.Now.ToString());
            viewitem.SubItems.Add($"{string.Join("&", request.Headers.AllKeys.Select(a => a + "=" + request.Headers[a]))}");
            viewitem.SubItems.Add($"{string.Join("&", request.QueryString.AllKeys.Select(a => a + "=" + request.QueryString[a]))}");
        }

        private void MarkError(ErrorDetails _errorDetails)
        {
            txtJson.Select(Math.Max(0, _errorDetails.Position - 1), 10);
            txtJson.ScrollToCaret();
        }

        private void VisualizeJsonTree(JsonObjectTree tree)
        {
            AddNode(tvJson.Nodes, tree.Root);
            JsonViewerTreeNode node = GetRootNode();
            InitVisualizers(node);
            node.Expand();
            tvJson.SelectedNode = node;
        }

        private void AddNode(TreeNodeCollection nodes, JsonObject jsonObject)
        {
            JsonViewerTreeNode newNode = new JsonViewerTreeNode(jsonObject);
            nodes.Add(newNode);
            newNode.Text = jsonObject.Text;
            newNode.Tag = jsonObject;
            newNode.ImageIndex = (int)jsonObject.JsonType;
            newNode.SelectedImageIndex = newNode.ImageIndex;

            foreach (JsonObject field in jsonObject.Fields)
            {
                AddNode(newNode.Nodes, field);
            }
        }

        public ErrorDetails ErrorDetails
        {
            get
            {
                return _errorDetails;
            }
        }

        public void Clear()
        {
            //Json = String.Empty;
        }

        public void ShowInfo(string info)
        {
            lblError.Text = info;
            lblError.Tag = null;
            lblError.Enabled = false;
            //tabControl.SelectedTab = pageTextView;
        }

        public void ShowInfo(ErrorDetails error)
        {
            ShowInfo(error.Error);
            lblError.Text = error.Error;
            lblError.Tag = error;
            lblError.Enabled = true;
            //tabControl.SelectedTab = pageTextView;
        }

        public void ClearInfo()
        {
            lblError.Text = String.Empty;
        }

        public bool HasErrors
        {
            get
            {
                return _errorDetails._err != null;
            }
        }

        private void txtJson_TextChanged(object sender, EventArgs e)
        {
            //Json = txtJson.Text;
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            txtFind.BackColor = SystemColors.Window;
            FindNext(true, true);
        }

        public bool FindNext(bool includeSelected)
        {
            return FindNext(txtFind.Text, includeSelected);
        }

        public void FindNext(bool includeSelected, bool fromUI)
        {
            if (!FindNext(includeSelected) && fromUI)
                txtFind.BackColor = Color.LightCoral;
        }

        public bool FindNext(string text, bool includeSelected)
        {
            TreeNode startNode = tvJson.SelectedNode;
            if (startNode == null && HasNodes())
                startNode = GetRootNode();
            if (startNode != null)
            {
                startNode = FindNext(startNode, text, includeSelected);
                if (startNode != null)
                {
                    tvJson.SelectedNode = startNode;
                    return true;
                }
            }
            return false;
        }

        public TreeNode FindNext(TreeNode startNode, string text, bool includeSelected)
        {
            if (text == String.Empty)
                return startNode;

            if (includeSelected && IsMatchingNode(startNode, text))
                return startNode;

            TreeNode originalStartNode = startNode;
            startNode = GetNextNode(startNode);
            text = text.ToLower();
            while (startNode != originalStartNode)
            {
                if (IsMatchingNode(startNode, text))
                    return startNode;
                startNode = GetNextNode(startNode);
            }

            return null;
        }

        private TreeNode GetNextNode(TreeNode startNode)
        {
            TreeNode next = startNode.FirstNode ?? startNode.NextNode;
            if (next == null)
            {
                while (startNode != null && next == null)
                {
                    startNode = startNode.Parent;
                    if (startNode != null)
                        next = startNode.NextNode;
                }
                if (next == null)
                {
                    next = GetRootNode();
                    FlashControl(txtFind, Color.Cyan);
                }
            }
            return next;
        }

        private bool IsMatchingNode(TreeNode startNode, string text)
        {
            return (startNode.Text.ToLower().Contains(text));
        }

        private JsonViewerTreeNode GetRootNode()
        {
            if (tvJson.Nodes.Count > 0)
                return (JsonViewerTreeNode)tvJson.Nodes[0];
            return null;
        }

        private bool HasNodes()
        {
            return (tvJson.Nodes.Count > 0);
        }

        private void txtFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FindNext(false, true);
            }
            if (e.KeyCode == Keys.Escape)
            {
                HideFind();
            }
        }

        private void FlashControl(Control control, Color color)
        {
            Color prevColor = control.BackColor;
            try
            {
                control.BackColor = color;
                control.Refresh();
                Thread.Sleep(25);
            }
            finally
            {
                control.BackColor = prevColor;
                control.Refresh();
            }
        }

        public void ShowTab(Tabs tab)
        {
            tabControl.SelectedIndex = (int)tab;
        }

        private void btnFormat_Click(object sender, EventArgs e)
        {
            var json = txtJson.Text; 
            if(!string.IsNullOrEmpty(json))
            {
                try
                {
                    txtJson.Text = JObject.Parse(json).ToString();
                }
                catch (Exception)
                {
                    MessageBox.Show(Resources.JsonFormatError);
                }
            }

        }

        public delegate string FormatJsonHandler(string json);

        //public FormatJsonHandler FormatJsonEvent; 

        private void ShowException(Exception e)
        {
            MessageBox.Show(this, e.Message, "Json Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnStripToSqr_Click(object sender, EventArgs e)
        {
            StripTextTo('[', ']');
        }

        private void btnStripToCurly_Click(object sender, EventArgs e)
        {
            StripTextTo('{', '}');
        }

        private void StripTextTo(char sChr, char eChr)
        {
            string text = txtJson.Text;
            int start = text.IndexOf(sChr);
            int end = text.LastIndexOf(eChr);
            int newLen = end - start + 1;
            if (newLen > 1)
            {
                txtJson.Text = text.Substring(start, newLen);
            }
        }

        private void tvJson_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_pluginsManager.DefaultVisualizer == null)
                return;

            cbVisualizers.BeginUpdate();
            _updating = true;
            try
            {
                JsonViewerTreeNode node = (JsonViewerTreeNode)e.Node;
                IJsonVisualizer lastActive = node.LastVisualizer;
                if (lastActive == null)
                    lastActive = (IJsonVisualizer)cbVisualizers.SelectedItem;
                if (lastActive == null)
                    lastActive = _pluginsManager.DefaultVisualizer;

                cbVisualizers.Items.Clear();
                cbVisualizers.Items.AddRange(node.Visualizers.ToArray());
                int index = cbVisualizers.Items.IndexOf(lastActive);
                if (index != -1)
                {
                    cbVisualizers.SelectedIndex = index;
                }
                else
                {
                    cbVisualizers.SelectedIndex = cbVisualizers.Items.IndexOf(_pluginsManager.DefaultVisualizer);
                }
            }
            finally
            {
                cbVisualizers.EndUpdate();
                _updating = false;
            }
            ActivateVisualizer();
        }

        private void ActivateVisualizer()
        {
            IJsonVisualizer visualizer = (IJsonVisualizer)cbVisualizers.SelectedItem;
            if (visualizer != null)
            {
                JsonObject jsonObject = GetSelectedTreeNode().JsonObject;
                Control visualizerCtrl = visualizer.GetControl(jsonObject);
                if (_lastVisualizerControl != visualizerCtrl)
                {
                    pnlVisualizer.Controls.Remove(_lastVisualizerControl);
                    pnlVisualizer.Controls.Add(visualizerCtrl);
                    visualizerCtrl.Dock = DockStyle.Fill;
                    _lastVisualizerControl = visualizerCtrl;
                }
                visualizer.Visualize(jsonObject);
            }
        }


        private void cbVisualizers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating && GetSelectedTreeNode() != null)
            {
                ActivateVisualizer();
                GetSelectedTreeNode().LastVisualizer = (IJsonVisualizer)cbVisualizers.SelectedItem;
            }
        }

        private JsonViewerTreeNode GetSelectedTreeNode()
        {
            return (JsonViewerTreeNode)tvJson.SelectedNode;
        }

        private void tvJson_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            foreach (JsonViewerTreeNode node in e.Node.Nodes)
            {
                InitVisualizers(node);
            }
        }

        private void InitVisualizers(JsonViewerTreeNode node)
        {
            if (!node.Initialized)
            {
                node.Initialized = true;
                JsonObject jsonObject = node.JsonObject;
                foreach (ICustomTextProvider textVis in _pluginsManager.TextVisualizers)
                {
                    if (textVis.CanVisualize(jsonObject))
                        node.TextVisualizers.Add(textVis);
                }

                node.RefreshText();

                foreach (IJsonVisualizer visualizer in _pluginsManager.Visualizers)
                {
                    if (visualizer.CanVisualize(jsonObject))
                        node.Visualizers.Add(visualizer);
                }
            }
        }

        private void btnCloseFind_Click(object sender, EventArgs e)
        {
            HideFind();
        }

        private void JsonViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && e.Control)
            {
                ShowFind();
            }
        }

        private void HideFind()
        {
            pnlFind.Visible = false;
            tvJson.Focus();
        }

        private void ShowFind()
        {
            pnlFind.Visible = true;
            txtFind.Focus();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFind();
        }

        private void expandallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvJson.BeginUpdate();
            try
            {
                if (tvJson.SelectedNode != null)
                {
                    TreeNode topNode = tvJson.TopNode;
                    tvJson.SelectedNode.ExpandAll();
                    tvJson.TopNode = topNode;
                }
            }
            finally
            {
                tvJson.EndUpdate();
            }
        }

        private void tvJson_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = tvJson.GetNodeAt(e.Location);
                if (node != null)
                {
                    tvJson.SelectedNode = node;
                }
            }
        }

        private void rightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender == mnuShowOnBottom)
            {
                spcViewer.Orientation = Orientation.Horizontal;
                mnuShowOnRight.Checked = false;
            }
            else
            {
                spcViewer.Orientation = Orientation.Vertical;
                mnuShowOnBottom.Checked = false;
            }
        }

        private void cbVisualizers_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = ((IJsonViewerPlugin)e.ListItem).DisplayName;
        }

        private void mnuTree_Opening(object sender, CancelEventArgs e)
        {
            mnuFind.Enabled = (GetRootNode() != null);
            mnuExpandAll.Enabled = (GetSelectedTreeNode() != null);

            mnuCopy.Enabled = mnuExpandAll.Enabled;
            mnuCopyValue.Enabled = mnuExpandAll.Enabled;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            string text;
            if (txtJson.SelectionLength > 0)
                text = txtJson.SelectedText;
            else
                text = txtJson.Text;
            Clipboard.SetText(text);
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            txtJson.Text = Clipboard.GetText();
        }

        private void mnuCopy_Click(object sender, EventArgs e)
        {
            JsonViewerTreeNode node = GetSelectedTreeNode();
            if (node != null)
            {
                Clipboard.SetText(node.Text);
            }
        }

        private void mnuCopyValue_Click(object sender, EventArgs e)
        {
            JsonViewerTreeNode node = GetSelectedTreeNode();
            if (node != null && node.JsonObject.Value != null)
            {
                Clipboard.SetText(node.JsonObject.Value.ToString());
            }
        }

        private void lblError_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lblError.Enabled && lblError.Tag != null)
            {
                ErrorDetails err = (ErrorDetails)lblError.Tag;
                MarkError(err);
            }
        }

        private void removeNewLineMenuItem_Click(object sender, EventArgs e)
        {
            StripFromText('\n', '\r');
        }

        private void removeSpecialCharsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = txtJson.Text;
            text = text.Replace(@"\""", @"""");
            txtJson.Text = text;
        }

        private void StripFromText(params char[] chars)
        {
            string text = txtJson.Text;
            foreach (char ch in chars)
            {
                text = text.Replace(ch.ToString(), "");
            }
            txtJson.Text = text;
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var jsonForm = new JsonFieldForm();
            //jsonForm.SaveEvent += SaveEvent;
            //jsonForm.ShowDialog(); 
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void SaveEvent(string key, string value)
        {

            
        }
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var v = sender as ListView;
            

        }

        private void listViewIn_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listview = (ListView)sender;
            ListViewItem lstrow = listview.GetItemAt(e.X, e.Y);
            System.Windows.Forms.ListViewItem.ListViewSubItem lstcol = lstrow.GetSubItemAt(e.X, e.Y);
            string strText = lstcol.Text;
            try
            {
                Clipboard.SetText(strText);
                notifyIcon1.Visible = true;
                string info = string.Format("内容【{0}】已经复制到剪贴板", strText);
                notifyIcon1.ShowBalloonTip(1500, "提示", info, ToolTipIcon.Info);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public struct ErrorDetails
    {
        internal string _err;
        internal int _pos;

        public string Error
        {
            get
            {
                return _err;
            }
        }

        public int Position
        {
            get
            {
                return _pos;
            }
        }

        public void Clear()
        {
            _err = null;
            _pos = 0;
        }
    }

    public class JsonViewerTreeNode : TreeNode
    {
        JsonObject _jsonObject;
        List<ICustomTextProvider> _textVisualizers = new List<ICustomTextProvider>();
        List<IJsonVisualizer> _visualizers = new List<IJsonVisualizer>();
        private bool _init;
        private IJsonVisualizer _lastVisualizer;

        public JsonViewerTreeNode(JsonObject jsonObject)
        {
            _jsonObject = jsonObject;
        }

        public List<ICustomTextProvider> TextVisualizers
        {
            get
            {
                return _textVisualizers;
            }
        }

        public List<IJsonVisualizer> Visualizers
        {
            get
            {
                return _visualizers;
            }
        }

        public JsonObject JsonObject
        {
            get
            {
                return _jsonObject;
            }
        }

        internal bool Initialized
        {
            get
            {
                return _init;
            }
            set
            {
                _init = value;
            }
        }

        internal void RefreshText()
        {
            StringBuilder sb = new StringBuilder(_jsonObject.Text);
            foreach (ICustomTextProvider textVisualizer in _textVisualizers)
            {
                try
                {
                    string customText = textVisualizer.GetText(_jsonObject);
                    sb.Append(" (" + customText + ")");
                }
                catch
                {
                    //silently ignore
                }
            }
            string text = sb.ToString();
            if (text != this.Text)
                this.Text = text;
        }

        public IJsonVisualizer LastVisualizer
        {
            get
            {
                return _lastVisualizer;
            }
            set
            {
                _lastVisualizer = value;
            }
        }
    }

    public enum Tabs { Viewer, Text };
}