using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Numerisation_GIST
{
    public partial class CustomMessageBox : Form
    {
        public CustomMessageBox(String title, String Message, String yesText, String noText)
        {
            InitializeComponent();
            this.Text = title;
            lbl_message.Text = Message;
            btn_left.Text = yesText;
            btn_right.Text = noText;
        }
    }
}
