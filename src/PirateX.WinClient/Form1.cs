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
            _client.OnOpen += OnOpen;
            _client.OnError += OnError;
            _client.OnReceiveMessage += (o, args) =>
            {
                this.Invoke((EventHandler)delegate
                {
                    this.jsonViewer1.Json = Encoding.UTF8.GetString(args.OriginalBytes);
                });
            };
            _client.Open();

            this.Invoke((EventHandler)delegate
            {
                txtHost.Enabled = false;
                txtPort.Enabled = false;
                btnConn.Enabled = false;
                txtToken.Enabled = false;
            });
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        private void OnOpen(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                btnSend.Enabled = true;
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            

            _client.Send("RoleInfo",txtQuery.Text.Trim(),new NameValueCollection(){ { "format", comboBox1.Text.Trim() } });
        }
    }
}
