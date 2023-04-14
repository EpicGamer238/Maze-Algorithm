using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TEST
{
    public class MouseDownFilter : IMessageFilter
    {
        public event EventHandler FormClicked;
        public event EventHandler RFormClicked;
        private int WM_LBUTTONDOWN = 0x201;
        private int WM_RBUTTONDOWN = 0x0204;
        private Form form = null;

        [DllImport("user32.dll")]
        public static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

        public MouseDownFilter(Form f)
        {
            form = f;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN)
            {
                if (Form.ActiveForm != null && Form.ActiveForm.Equals(form))
                {
                    OnFormClicked();
                }
            }
            else if (m.Msg == WM_RBUTTONDOWN)
            {
                if (Form.ActiveForm != null && Form.ActiveForm.Equals(form))
                {
                    ROnFormClicked();
                }
            }
            return false;
        }

        protected void OnFormClicked()
        {
            if (FormClicked != null)
            {
                FormClicked(form, EventArgs.Empty);
            }
        }
        protected void ROnFormClicked()
        {
            if (FormClicked != null)
            {
                RFormClicked(form, EventArgs.Empty);
            }
        }
    }
}
