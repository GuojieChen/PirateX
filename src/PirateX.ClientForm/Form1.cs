using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using PirateX.Client;

namespace PirateX.ClientForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitRequestHistory();
            InitResponseHistory();

            var request = new
            {
                C = "RoleLoginV2",
                D = new
                {
                    Rid = 1,
                    ServerId = 1
                },
                R = false
            };

            var jo = JObject.FromObject(request);
            jsonViewerRequest.Json = jo.ToString();

            this.Text = "Mr.Glee - Client "+ "(" + Application.ProductVersion + ")";
        }

        private readonly HistoryUserControl _requestHistory = new HistoryUserControl();
        private readonly HistoryUserControl _responseHistory = new HistoryUserControl();

        private void InitRequestHistory()
        {
            _requestHistory.Dock = DockStyle.Fill;

            //requestHistory.listView1.Columns.Add("Method", 150, HorizontalAlignment.Left);
            _requestHistory.listView1.DoubleClick += ListView1_DoubleClick;
            var tabPage = new TabPage()
            {
                Text = "History"
            };
            tabPage.Controls.Add(_requestHistory);

            jsonViewerRequest.tabControl.TabPages.Add(tabPage);
        }

        private void ListView1_DoubleClick(object sender, EventArgs e)
        {
            var jsonViewForm = new FormJsonView();
            jsonViewForm.jsonViewer1.Json = (sender as ListView).FocusedItem.SubItems[2].Text;

            jsonViewForm.Show(this);
        }

        private void InitResponseHistory()
        {
            _responseHistory.Dock = DockStyle.Fill;

            //requestHistory.listView1.Columns.Add("Method", 150, HorizontalAlignment.Left);
            _responseHistory.listView1.DoubleClick += ListView1_DoubleClick;
            var tabPage = new TabPage()
            {
                Text = "History"
            };
            tabPage.Controls.Add(_responseHistory);

            jsonViewerResponse.tabControl.TabPages.Add(tabPage);
        }


        private PSocketClient _client = null;
        private readonly Stopwatch _sw = new Stopwatch();

        private void btnConn_Click(object sender, EventArgs e)
        {
            var uri = $"ps://{txtHost.Text.Trim()}:{txtPort.Text.Trim()}/";

            _client = new PSocketClient(uri, 204800);

            _client.OnClosed += (o, args) => this.Invoke((EventHandler)delegate
            {
                btnConn.Enabled = !btnConn.Enabled;
                btnDisConn.Enabled = !btnDisConn.Enabled;
                btnSend.Enabled = !btnSend.Enabled;

                _client = null;
            });

            _client.OnReceiveMessage += DataReceived;
            _client.OnOpen += Connected;
            _client.Open();
        }
        private void Connected(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                btnConn.Enabled = false;
                btnDisConn.Enabled = true;
                btnSend.Enabled = true;
                //lableStatus.Text = "已连接";
            });
        }

        private void DataReceived(object sender, MsgEventArgs e)
        {
            _sw.Stop();
            //var tree = JsonObjectTree.Parse(oData);
            this.Invoke((EventHandler)delegate
            {
                jsonViewerResponse.Json = e.Msg;

                if (e.Msg.Contains("NewSession"))
                {
                    _client.Close();
                }
                else
                {
                    btnSend.Enabled = true;
                }

                var jObject = JObject.Parse(e.Msg);

                var viewitem = _responseHistory.listView1.Items.Add((jObject["C"] ?? jObject["B"]).ToString());
                viewitem.SubItems.Add(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss ffff"));
                viewitem.SubItems.Add(jObject["D"].ToString());
            });
        }

        private void btnDisConn_Click(object sender, EventArgs e)
        {
            _client.Close();
            btnConn.Enabled = true;
            btnSend.Enabled = false;
            btnDisConn.Enabled = false;
            //lableStatus.Text = "未连接";
            //lableE.Text = "0";
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            _sw.Reset();
            _sw.Start();

            var currentMessage = jsonViewerRequest.Json;

            var jObject = JObject.Parse(currentMessage);

            _client.Send(jObject["C"].ToString(), jObject["D"], jObject["Ex"], Convert.ToBoolean(jObject["R"]));

            this.Invoke((EventHandler)delegate
            {
                jsonViewerResponse.Clear();

                btnSend.Enabled = false;
                //btnSend.Text = "等待中";

                //_requestHistory.listView1.
                var viewitem = _requestHistory.listView1.Items.Add(jObject["C"].ToString());
                viewitem.SubItems.Add(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss ffff"));
                viewitem.SubItems.Add(jObject["D"].ToString());
            });
        }
    }
}
