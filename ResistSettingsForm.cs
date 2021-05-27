using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResistAssist
{
    public partial class ResistSettingsForm : Form
    {
        public ResistSettingsForm()
        {
            InitializeComponent();
        }
        
        private void ResistSettingsForm_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = ResistSettings.Instance;
        }    
    }


}