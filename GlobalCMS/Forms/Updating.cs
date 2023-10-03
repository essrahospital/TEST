using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class Updating : Form
    {
        public Updating()
        {
            InitializeComponent();
        }

        private void Updating_Load(object sender, EventArgs e)
        {
            frmObj = this;
        }

        static Updating _frmObj;
        public static Updating frmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }
    }
}
