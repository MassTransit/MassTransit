using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MassTransit.Visualizers
{
    public partial class ServiceBusForm : Form
    {
        public ServiceBusForm()
        {
            InitializeComponent();
        }

        public void SetContent(string input)
        {
            this.InputContent.Text = input;
        }
    }
}
