using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace tugasakhir.Control
{
    public partial class FilterPreview : Component
    {
        public FilterPreview()
        {
            InitializeComponent();
        }

        public FilterPreview(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
