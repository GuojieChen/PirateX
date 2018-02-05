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
using PirateX.Protocol.ResponseConvert;

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
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }

            //var headerQuery = new Name

            var tokenbase64 = string.Empty;
            try
            {
                Convert.FromBase64String(txtToken.Text.Trim());
                tokenbase64 = txtToken.Text.Trim();
            }
            catch (Exception exception)
            {
                var tokenQuery = HttpUtility.ParseQueryString(txtToken.Text.Trim());
                var token = new Token()
                {
                    Did = Convert.ToInt32(tokenQuery["did"]),
                    Rid = Convert.ToInt32(tokenQuery["rid"]),
                    Uid = tokenQuery["uid"]
                };

                var pbCovnert = new ProtoResponseConvert();
                tokenbase64 = Convert.ToBase64String(pbCovnert.SerializeObject(token));
            }

            

            _client = new PirateXClient($"ps://{txtHost.Text.Trim()}:{txtPort.Text.Trim()}", tokenbase64 );
            _client.Headers = HttpUtility.ParseQueryString(txtHeader.Text.Trim());
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

                    var tin = Convert.ToInt64(args.Package.Headers["_tin_"]);
                    var tout = Convert.ToInt64(args.Package.Headers["_tout_"]);

                    if (Equals(args.Package.Headers["c"], "_CommandList_"))
                    {
                        var list = Encoding.UTF8.GetString(args.Package.ContentBytes).Split(new char[] {','});
                        foreach (var item in list)
                        {
                            cbbCMDList.Items.Add(item);
                        }

                        cbbCMDList.Text = list[0];
                    }

                    this.jsonViewer1.ResponseInfo = args.Package;
                    btnSend.Enabled = true;
                    btnDisconn.Enabled = true;
                });

            };

            _client.OnNotified += (o, args) =>
            {
                this.Invoke((EventHandler)delegate
                {
                    this.jsonViewer1.ResponseInfo = args.Package;
                    btnSend.Enabled = true;
                    btnDisconn.Enabled = true;
                });

            };


            _client.OnResponseProcessed += (o, log) =>
            {
                this.Invoke((EventHandler)delegate
                {
                    this.lable_duration.Text = $"{log.Ts} ms"; 
                });
            };

            _client.OnSend += (o, args) =>
            {
                this.Invoke((EventHandler)delegate
                {
                    this.jsonViewer1.NewOut(args.Package.ToRequestPackage());
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
                txtHeader.Enabled = true;
                btnSend.Enabled = false;
                btnDisconn.Enabled = false;
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
                txtHeader.Enabled = false; 
                btnSend.Enabled = true;
                btnDisconn.Enabled = true;

                _client.Send("_CommandList_", "", new NameValueCollection(){{"format","txt"}});
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var exHeaders = new NameValueCollection();
            exHeaders.Add("format", comboBox1.Text.Trim());
            _client.Send(cbbCMDList.Text,txtQuery.Text.Trim(),exHeaders);
        }

        private void btnDisconn_Click(object sender, EventArgs e)
        {
            txtToken.Enabled = true;
            btnSend.Enabled = false;
            btnDisconn.Enabled = false;
            txtHeader.Enabled = true;
            txtHost.Enabled = true;
            txtPort.Enabled = true;
        }
    }
}
