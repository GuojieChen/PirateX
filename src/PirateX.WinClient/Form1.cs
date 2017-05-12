using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using PirateX.Client;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using PirateX.Protocol.Package.ResponseConvert;

namespace PirateX.WinClient
{
    public partial class Form1 : Form
    {
        private PirateXClient _client;

        public Form1()
        {
            InitializeComponent();

            foreach (var responseConvert in typeof(IResponseConvert).Assembly.GetTypes().Where(item => typeof(IResponseConvert).IsAssignableFrom(item)))
            {
                if (responseConvert.IsInterface)
                    continue;

                var attrs = responseConvert.GetCustomAttributes(typeof(DisplayColumnAttribute), false);
                if (attrs.Any())
                {
                    var convertName = ((DisplayColumnAttribute)attrs[0]).DisplayColumn;
                    if (!string.IsNullOrEmpty(convertName))
                    {
                        comboBox1.Items.Add(convertName.ToLower());
                    }
                }
            }

            comboBox1.SelectedIndex = 0;
        }

        private void btnConn_Click(object sender, EventArgs e)
        {
            //var headerQuery = new Name
            var tokenQuery = HttpUtility.ParseQueryString(txtToken.Text.Trim());
            var token = new Token()
            {
                Did = Convert.ToInt32(tokenQuery["did"]),
                Rid = Convert.ToInt32(tokenQuery["rid"]),
                Uid = tokenQuery["uid"]
            };

            var pbCovnert = new PirateX.Protocol.Package.ResponseConvert.ProtoResponseConvert();

            _client = new PirateXClient($"ps://{txtHost.Text.Trim()}:{txtPort.Text.Trim()}", Convert.ToBase64String(pbCovnert.SerializeObject(token)));
            if (!string.IsNullOrEmpty(comboBox1.Text))
                _client.DefaultFormat = comboBox1.Text;
            //_client.ExHeaders

            //_client.ExHeaders.Add("deviceid",HttpUtility.UrlEncode(Guid.NewGuid().ToString()));
            //_client.ExHeaders.Add("phone","windows");
            //_client.ExHeaders.Add("os","1.0");
            //_client.ExHeaders.Add("net","wlan");
            _client.OnOpen += OnOpen;
            _client.OnError += OnError;
            _client.OnClosed += OnClosed;
            _client.OnReceiveMessage += (o, args) =>
            {
                this.Invoke((EventHandler)delegate
                {

                    this.jsonViewer1.ResponseInfo = new PirateXResponseInfo(args.Package);
                    btnSend.Enabled = true;
                });

            };
            _client.OnSend += (o, args) =>
            {
                this.Invoke((EventHandler)delegate
                {
                    this.jsonViewer1.NewOut(args.Package);
                });
            };
            _client.Open();

        }

        private void OnClosed(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                txtHost.Enabled = true;
                txtPort.Enabled = true;
                btnConn.Enabled = true;
                txtToken.Enabled = true;
                btnSend.Enabled = false;
            });

            _client = null; 
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        private void OnOpen(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                txtHost.Enabled = false;
                txtPort.Enabled = false;
                txtToken.Enabled = false;

                btnConn.Text = "断开";
                btnConn.Enabled = false; 
                btnSend.Enabled = true;
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            var exHeaders = HttpUtility.ParseQueryString(txtHeader.Text.Trim());
            exHeaders.Add("format", comboBox1.Text.Trim());
            _client.Send("RoleInfo",txtQuery.Text.Trim(), exHeaders);
        }
    }
}
