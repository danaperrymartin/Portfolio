using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TCPServerFTP
{
    public partial class ucIPAddress : UserControl
    {
        //-------------------------------------------------------------------------------------------------
        //  CONSTANTS
        //-------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------
        //  PUBLIC
        //-------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------
        //  PRIVATE
        //-------------------------------------------------------------------------------------------------

        //*************************************************************************************************
        //
        //  CONSTRUCTORS/DESTRUCTORS
        //
        //*************************************************************************************************

        /// <summary>
        /// Default constructor
        /// </summary>
        public ucIPAddress()
        {
            InitializeComponent();
        }

        //*************************************************************************************************
        //
        //  PUBLIC MEMBERS
        //
        //*************************************************************************************************

        /// <summary>
        /// Gets/sets the IP Address string
        /// </summary>
        public string IPAddress
        { 
            set
            {
                //------------------------------------------------------
                //  Remove any unnecessary whitespace and split into parts
                //------------------------------------------------------
                string szCorrected = value.Replace(" ", "");
                string[] szVals = szCorrected.Split( new char[]{'.', ':'} );

                if (szVals.Length > 3)
                {
                    tbxPart1.Text = szVals[0];
                    tbxPart2.Text = szVals[1];
                    tbxPart3.Text = szVals[2];
                    tbxPart4.Text = szVals[3];
                }

                if (szVals.Length > 4)
                {
                    tbxPart5.Text = szVals[4];
                }
                else
                {
                    tbxPart5.Text = string.Empty;
                }
            }

            get
            {
                //------------------------------------------------------
                //  Remove any unnecessary characters and form into dotted 
                //  address.
                //------------------------------------------------------
                string szPart1 = tbxPart1.Text.Replace("\t", "");
                string szPart2 = tbxPart2.Text.Replace("\t", "");
                string szPart3 = tbxPart3.Text.Replace("\t", "");
                string szPart4 = tbxPart4.Text.Replace("\t", "");
                string szPart5 = tbxPart5.Text.Replace("\t", "");

                string szAddress = szPart1 + "." + szPart2 + "." + szPart3 + "." + szPart4 + ":" + szPart5;

                return szAddress;
            }
        }


        /// <summary>
        /// Clears all text boxes, clears the IP Address
        /// </summary>
        public void Clear()
        {
            tbxPart1.Text = string.Empty;
            tbxPart2.Text = string.Empty;
            tbxPart3.Text = string.Empty;
            tbxPart4.Text = string.Empty;
            tbxPart5.Text = string.Empty;
        }

        /// <summary>
        /// Filters the user keypresses to those only acceptable 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = (TextBox)sender;


            if (e.KeyCode == Keys.Decimal)
            {
                switch (tb.Name)
                {
                    case "tbxPart1":
                        tbxPart2.Focus();
                        break;

                    case "tbxPart2":
                        tbxPart3.Focus();
                        break;

                    case "tbxPart3":
                        tbxPart4.Focus();
                        break;

                    case "tbxPart4":
                        tbxPart5.Focus();
                        break;

                    default:
                        break;
                }

                e.SuppressKeyPress = true;
            }
            else
            {
                //------------------------------------------------------
                //  Restrict key presses to valid values
                //------------------------------------------------------
                if (e.KeyCode == Keys.D0 ||
                    e.KeyCode == Keys.D1 ||
                    e.KeyCode == Keys.D2 ||
                    e.KeyCode == Keys.D3 ||
                    e.KeyCode == Keys.D4 ||
                    e.KeyCode == Keys.D5 ||
                    e.KeyCode == Keys.D6 ||
                    e.KeyCode == Keys.D7 ||
                    e.KeyCode == Keys.D8 ||
                    e.KeyCode == Keys.D9 ||
                    e.KeyCode == Keys.NumPad0 ||
                    e.KeyCode == Keys.NumPad1 ||
                    e.KeyCode == Keys.NumPad2 ||
                    e.KeyCode == Keys.NumPad3 ||
                    e.KeyCode == Keys.NumPad4 ||
                    e.KeyCode == Keys.NumPad5 ||
                    e.KeyCode == Keys.NumPad6 ||
                    e.KeyCode == Keys.NumPad7 ||
                    e.KeyCode == Keys.NumPad8 ||
                    e.KeyCode == Keys.NumPad9 ||
                    e.KeyCode == Keys.Delete ||
                    e.KeyCode == Keys.Back ||
                    e.KeyCode == Keys.Right ||
                    e.KeyCode == Keys.Left ||
                    e.KeyCode == Keys.ShiftKey ||
                    e.KeyCode == Keys.Home ||
                    e.KeyCode == Keys.End)
                {
                    e.SuppressKeyPress = false;
                }
                else
                {
                    e.SuppressKeyPress = true;
                    System.Media.SystemSounds.Beep.Play();
                }
            }
        }

        /// <summary>
        /// Make sure user entered a valid IP address value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            try
            {
                errorProvider.SetError(tb, string.Empty);

                int iVal = int.Parse(tb.Text);

                if (iVal < 0)
                {
                    tb.Text = "0";
                    errorProvider.SetError(tb, "Value cannot be < 0, set to zero");
                }
                else if (iVal > 255 && tb.Name != "tbxPart5")
                {
                    tb.Text = "255";
                    errorProvider.SetError(tb, "Value cannot be > 255, set to 255");
                }
                else if (iVal > 9999 && tb.Name == "tbxPart5")
                {
                    tb.Text = "9999";
                    errorProvider.SetError(tb, "Value cannot be > 9999, set to 9999");
                }
            }
            catch (Exception ex)
            {
                errorProvider.SetError(tb, ex.Message);
                tb.Focus();
            }
        }
        /// <summary>
        /// Highlights text when the textbox is entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;

            tb.SelectAll();
        }

        /// <summary>
        /// Highlights text when the textbox is entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            tb.SelectAll();
        }

        //*************************************************************************************************
        //
        //  PRIVATE MEMBERS
        //
        //*************************************************************************************************
    }
}
