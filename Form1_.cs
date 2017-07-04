using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;
using Test1;
using System.Globalization;
using System.Xml;
using System.Windows.Forms.VisualStyles;
using CrystalDecisions.CrystalReports.Engine;
using System.Threading;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.IO.Ports;
namespace Circulation
{
    public delegate void ScannedEventHandler(object sender, EventArgs ev);
    public delegate void HeaderClick(object sender, DataGridViewCellMouseEventArgs ev);
    public delegate void AbonChangedEventHandler(object sender, EventArgs ev);

    public partial class Form1 : Form
    {
        // System.Collections.Generic.Dictionary<int, string> FFFF = new Dictionary<int, string>(8);
        public static event AbonChangedEventHandler AbonChanged;

        public DBWork dbw;
        _BarcScan sc;
        //private string WasFirstScan = "";
        public string EmpID;
        private Form2 f2;
        private Form4 f4;
        private Form5 f5;
        SerialPort port;

        public DBWork.dbReader ReaderRecord, ReaderRecordWork, ReaderRecordFormular;
        public DBWork.dbBook BookRecord, BookRecordWork;
        public DBWork.dbReader ReaderSetBarcode;
        private System.Windows.Forms.Label label16;
        public ExtGui.RoundProgress RndPrg;
        public Form1()
        {
            //System.Collections.Generic.List<int> f = new List<int>(3);

            f2 = new Form2(this);
            InitializeComponent();
            dbw = new DBWork(this);
            dbw.SetPenaltyAll();
            dbw.DeleteExceededOrders();
            //ReaderRecord = new DBWork.dbReader("9643907728022200024 " + "07020077");
            //sc = new _BarcScan(this);
            //this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            //this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.label16 = new System.Windows.Forms.Label();
            //this.Controls.Add(RndPrg);
            // 
            // label16
            // 
            this.Controls.Add(this.label16);
            f2.ShowDialog();

            /*if (EmpID == null)
            {
                MessageBox.Show("�� �� ������������! ��������� ��������� ���� ������.", "������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }*/
            Form1.Scanned += new ScannedEventHandler(Form1_Scanned);
            Form1.AbonChanged += new AbonChangedEventHandler(Form1_AbonChanged);
            //dataGridView1.Rows.RemoveAt(0);// .Remove(dataGridView1.Rows[0].);
            //ReaderRecord = new DBWork.dbReader();
            //BookRecord = new DBWork.dbBook();    
            this.button2.Enabled = false;
            this.button4.Enabled = false;
            label4.Text = "������ ������� " + DateTime.Now.ToShortDateString() + ":";
            //this.Controls.Add(this.panel1);
            //this.tabPage3.Controls.Remove(this.panel1);
            //MessageBox.Show(tabControl1.SelectedTab.ToString());// = tabControl1.TabPages[1];
            this.label16.AutoSize = true;
            this.label16.BringToFront();
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label16.Location = new System.Drawing.Point(328, 458);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(355, 39);
            this.label16.TabIndex = 0;
            this.label16.Text = "�������� ��������";
            this.label16.Visible = false;
            this.label16.ForeColor = Color.Green;
            Formular.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Formular.Columns.Clear();

            port = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            port.Handshake = Handshake.RequestToSend;
            port.NewLine = Convert.ToChar(13).ToString();

            try
            {
                port.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public delegate void ScanFuncDelegate(object sender, EventArgs ev);
        string FromPort = "";

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            FromPort = port.ReadLine();
            FromPort = FromPort.Trim();
            port.DiscardInBuffer();
            ScanFuncDelegate ScanDelegate;
            ScanDelegate = new ScanFuncDelegate(Form1_Scanned);
            this.Invoke(ScanDelegate, new object[] { sender, e });
        }
        public static void FireAbon(object sender, EventArgs ev)
        {
            if (AbonChanged != null)
            {
                AbonChanged(sender, ev);
            }
        }


        void Form1_AbonChanged(object sender, EventArgs ev)
        {
            if (this.ReaderRecord != null)
            {
                this.ReaderRecord.AbonType = ((Form5)sender).abon;
            }
        }
        //public enum Regim {Knigi,vidachaShtrihkodov,} 
        public class DataGridViewDisableButtonColumn : DataGridViewButtonColumn
        {
            public DataGridViewDisableButtonColumn()
            {
                this.CellTemplate = new DataGridViewDisableButtonCell();
            }
        }
        public class DataGridViewDisableButtonCell : DataGridViewButtonCell
        {
            private bool enabledValue;
            public bool Enabled
            {
                get
                {
                    return enabledValue;
                }
                set
                {
                    enabledValue = value;
                }
            }

            // Override the Clone method so that the Enabled property is copied.
            public override object Clone()
            {
                DataGridViewDisableButtonCell cell =
                    (DataGridViewDisableButtonCell)base.Clone();
                cell.Enabled = this.Enabled;
                return cell;
            }

            // By default, enable the button cell.
            public DataGridViewDisableButtonCell()
            {
                this.enabledValue = true;
            }

            protected override void Paint(Graphics graphics,
                Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                DataGridViewElementStates elementState, object value,
                object formattedValue, string errorText,
                DataGridViewCellStyle cellStyle,
                DataGridViewAdvancedBorderStyle advancedBorderStyle,
                DataGridViewPaintParts paintParts)
            {
                // The button cell is disabled, so paint the border,  
                // background, and disabled button for the cell.
                if (!this.enabledValue)
                {
                    // Draw the cell background, if specified.
                    if ((paintParts & DataGridViewPaintParts.Background) ==
                        DataGridViewPaintParts.Background)
                    {
                        SolidBrush cellBackground =
                            new SolidBrush(cellStyle.BackColor);
                        graphics.FillRectangle(cellBackground, cellBounds);
                        cellBackground.Dispose();
                    }

                    // Draw the cell borders, if specified.
                    if ((paintParts & DataGridViewPaintParts.Border) ==
                        DataGridViewPaintParts.Border)
                    {
                        PaintBorder(graphics, clipBounds, cellBounds, cellStyle,
                            advancedBorderStyle);
                    }

                    // Calculate the area in which to draw the button.
                    Rectangle buttonArea = cellBounds;
                    Rectangle buttonAdjustment =
                        this.BorderWidths(advancedBorderStyle);
                    buttonArea.X += buttonAdjustment.X;
                    buttonArea.Y += buttonAdjustment.Y;
                    buttonArea.Height -= buttonAdjustment.Height;
                    buttonArea.Width -= buttonAdjustment.Width;

                    // Draw the disabled button.                
                    ButtonRenderer.DrawButton(graphics, buttonArea,
                        PushButtonState.Disabled);

                    // Draw the disabled button text. 
                    if (this.FormattedValue is String)
                    {
                        TextRenderer.DrawText(graphics,
                            (string)this.FormattedValue,
                            this.DataGridView.Font,
                            buttonArea, SystemColors.GrayText);
                    }
                }
                else
                {
                    // The button cell is enabled, so let the base class 
                    // handle the painting.
                    base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                        elementState, value, formattedValue, errorText,
                        cellStyle, advancedBorderStyle, paintParts);
                }
            }
        }
        void FormularColumnsForming(string ReaderID)
        {
            Formular.Columns.Clear();
            Formular.AutoGenerateColumns = false;
            Formular.Columns.Add("NN", "��");
            Formular.Columns[0].Width = 35;
            dbw.SetPenalty(ReaderID);
            try
            {
                Formular.DataSource = dbw.GetFormular(ReaderID);
                //Formular.DataMember = "tmp";
            }
            catch (IndexOutOfRangeException evv)
            {
                string d = evv.Message;
                MessageBox.Show("�������� �� �������� �����������!", "����������.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //Formular.Columns[1].HeaderText = "��";
            if (Formular.Rows.Count == 0)
            {
                MessageBox.Show("�� ��������� �� �������� ���� � ���������!", "����������.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Formular.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.HeaderText = "��������";
            col.Width = 225;
            col.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            col.ReadOnly = true;
            Formular.Columns.Add(col);
            col.DataPropertyName = "Zag";

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "zagsort";
            col.Visible = false;
            Formular.Columns.Add(col);
            col.DataPropertyName = "��������_sort";

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "�����";
            col.Width = 130;
            col.ReadOnly = true;
            col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            Formular.Columns.Add(col);
            col.DataPropertyName = "�����";

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "";
            col.Visible = false;
            Formular.Columns.Add(col);
            col.DataPropertyName = "�����_Sort";

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "";
            col.Visible = false;
            col.Name = "idmain";
            col.DataPropertyName = "idmain";
            Formular.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "";
            col.Visible = false;
            col.Name = "zkid";
            col.DataPropertyName = "zkid";
            this.Formular.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "";
            col.Visible = false;
            col.Name = "zi";
            col.DataPropertyName = "zi";
            this.Formular.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "";
            col.Visible = false;
            col.Name = "idmain";
            col.DataPropertyName = "zakid";
            this.Formular.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "����� ������ �����";
            col.ReadOnly = true;
            col.Width = 80;
            Formular.Columns.Add(col);
            col.DataPropertyName = "inv";
            col.Name = "inv";

            /*col = new DataGridViewTextBoxColumn();
            col.HeaderText = "����� �������";
                    
            Formular.Columns.Add(col);
            col.DataPropertyName = "�����_�������";*/

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "���� ������";
            col.ReadOnly = true;
            col.Width = 80;
            Formular.Columns.Add(col);
            col.DataPropertyName = "issue";

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "������ ����� ��� ���� ��������";
            col.ReadOnly = true;
            col.Width = 80;
            col.DataPropertyName = "vozv";
            col.Name = "vozv";
            Formular.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "����� ������ ���� ���� ����";
            col.ReadOnly = true;
            col.Width = 80;
            col.Name = "fact";
            col.DataPropertyName = "fact";
            Formular.Columns.Add(col);

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "��� �� ���� ��";
            col.ReadOnly = true;
            col.Width = 40;
            col.Name = "peny";
            Formular.Columns.Add(col);

            DataGridViewCheckBoxColumn colch = new DataGridViewCheckBoxColumn();
            colch.HeaderText = "���� �����";
            colch.Name = "pen";
            colch.Width = 50;
            colch.ReadOnly = false;
            Formular.Columns.Add(colch);
            colch.DataPropertyName = "penalty";

            colch = new DataGridViewCheckBoxColumn();
            colch.Name = "rempen";
            colch.Visible = false;
            Formular.Columns.Add(colch);
            colch.DataPropertyName = "rempenalty";

            DataGridViewDisableButtonColumn ButCol = new DataGridViewDisableButtonColumn();
            Formular.Columns.Add(ButCol);
            ButCol.Name = "but";
            ButCol.Width = 120;
            ButCol.HeaderText = "��������� ����� ������ �����";

            ButCol.DefaultCellStyle.BackColor = Form1.DefaultBackColor;
            Padding myPadd = ButCol.DefaultCellStyle.Padding;
            ButCol.DefaultCellStyle.SelectionBackColor = Form1.DefaultBackColor;
            ButCol.DefaultCellStyle.SelectionForeColor = Color.Black;
            Font myF = new Font(FontFamily.GenericSansSerif, 8);

            ButCol.DefaultCellStyle.Font = myF;
            myPadd.All = 1;
            ButCol.DefaultCellStyle.Padding = myPadd;
            Formular.Columns["pen"].ReadOnly = true;

            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "bar";
            col.Visible = false;
            col.Name = "bar";
            col.DataPropertyName = "bar";
            Formular.Columns.Add(col);

            foreach (DataGridViewRow row in Formular.Rows)
            {
                row.Cells["peny"].Value = CalculatePeny(row).ToString() + " �.";
                //row.Cells["pen"].ReadOnly = true;
                DataGridViewDisableButtonCell bc = (DataGridViewDisableButtonCell)row.Cells["but"];
                
                
                
                if ((row.Cells["pen"].Value.ToString().ToLower() == "false") && (row.Cells["zkid"].Value.ToString() != "0") && (bool.Parse(row.Cells["rempen"].Value.ToString()) == true))
                {
                    bc.Value = "��� ���������";//����� �����
                    bc.Enabled = false;
                }
                else
                    if ((row.Cells["pen"].Value.ToString().ToLower() == "false") && (row.Cells["rempen"].Value.ToString().ToLower() == "false"))
                    {
                        bc.Value = "��������";
                        bc.Enabled = true;
                        //row.Cells["pen"].ReadOnly = true;
                    }
                    else
                        if ((row.Cells["pen"].Value.ToString().ToLower() == "true") && (row.Cells["rempen"].Value.ToString().ToLower() == "false") && (row.Cells["zkid"].Value.ToString() != "0"))
                        {
                            bc.Value = "��������";//����� ��� �� ����������
                            bc.Enabled = true;

                        }
                        else
                            if ((row.Cells["pen"].Value.ToString().ToLower() == "true") && (row.Cells["rempen"].Value.ToString().ToLower() == "false") && (row.Cells["zkid"].Value.ToString() == "0"))
                            {
                                bc.Value = "����� ���������";//����� ����������, �� � ���������� �����
                                bc.Enabled = true;
                                
                            }
                            else
                                if ((row.Cells["pen"].Value.ToString().ToLower() == "true") && (row.Cells["rempen"].Value.ToString().ToLower() == "true"))
                                {
                                    bc.Value = "��� ���������";//������ �� ���� �� ������ ����, ���� ���� ��������� ���������� ��������� � �������� � ��.
                                    bc.Enabled = false;
                                    MessageBox.Show("��������� ��������� ������������ ��������.������ ���� �� ������. ���������� � ������������.");
                                }

            }
            autoinc(Formular);

        }
        int CalculatePeny(DataGridViewRow r)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select * from Reservation_R..PENY";
            DataSet DS = new DataSet();
            Conn.SQLDA.Fill(DS, "peny");
            int sum = 0;
            int LastPeny = 0;

            DateTime DateOfFineVozv = DateTime.Parse(r.Cells["vozv"].Value.ToString());
            DateTime DateOfFineFact = (r.Cells["fact"].Value.ToString() == string.Empty) ?
                                      DateTime.Today :
                                      DateTime.Parse(r.Cells["fact"].Value.ToString());
            if (DateOfFineFact <= DateOfFineVozv) return 0;
            List<int> fine = new List<int>();
            List<DateTime> dfine = new List<DateTime>();

            int i = -1;

            foreach (DataRow row in DS.Tables["peny"].Rows)
            {
                dfine.Add(DateTime.Parse(row["DATEOFSETUP"].ToString()));
                fine.Add(int.Parse(row["PENYPERDAY"].ToString()));
            }
            foreach (DateTime df in dfine)
            {
                i++;
                if (DateOfFineVozv > df)
                {
                    break;
                }
            }
            dfine[i] = DateOfFineVozv;
            for (int j = 0; j < i; j++)
            {
                dfine.RemoveAt(0);
                fine.RemoveAt(0);
            }
            if (dfine[dfine.Count - 1] >= DateOfFineFact)
            {
                dfine[dfine.Count - 1] = DateOfFineFact;
            }
            else
            {
                dfine.Add(DateOfFineFact);
            }
            List<TimeSpan> span = new List<TimeSpan>();
            DateTime tmp = dfine[0];
            for (int j = 1; j < dfine.Count; j++)
            {
                span.Add(dfine[j] - tmp);
                if (span[span.Count - 1].Days < 0)
                {
                    span[span.Count - 1] = new TimeSpan(0, 0, 0, 0);
                }
                tmp = dfine[j];
            }
            for (int j = 0; j < span.Count; j++)
            {
                sum += span[j].Days * fine[j];
            }
            return sum;

        }
        int CalculatePeny(DateTime datevozv_, DateTime datefactvozv_)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select * from Reservation_R..PENY";
            DataSet DS = new DataSet();
            Conn.SQLDA.Fill(DS, "peny");
            int sum = 0;
            int LastPeny = 0;

            DateTime DateOfFineVozv = datevozv_;
            DateTime DateOfFineFact = datefactvozv_;
            if (DateOfFineFact <= DateOfFineVozv) return 0;
            List<int> fine = new List<int>();
            List<DateTime> dfine = new List<DateTime>();

            int i = -1;

            foreach (DataRow row in DS.Tables["peny"].Rows)
            {
                dfine.Add(DateTime.Parse(row["DATEOFSETUP"].ToString()));
                fine.Add(int.Parse(row["PENYPERDAY"].ToString()));
            }
            foreach (DateTime df in dfine)
            {
                i++;
                if (DateOfFineVozv > df)
                {
                    break;
                }
            }
            dfine[i] = DateOfFineVozv;
            for (int j = 0; j < i; j++)
            {
                dfine.RemoveAt(0);
                fine.RemoveAt(0);
            }
            if (dfine[dfine.Count - 1] >= DateOfFineFact)
            {
                dfine[dfine.Count - 1] = DateOfFineFact;
            }
            else
            {
                dfine.Add(DateOfFineFact);
            }
            List<TimeSpan> span = new List<TimeSpan>();
            DateTime tmp = dfine[0];
            for (int j = 1; j < dfine.Count; j++)
            {
                span.Add(dfine[j] - tmp);
                if (span[span.Count - 1].Days < 0)
                {
                    span[span.Count - 1] = new TimeSpan(0, 0, 0, 0);
                }
                tmp = dfine[j];
            }
            for (int j = 0; j < span.Count; j++)
            {
                sum += span[j].Days * fine[j];
            }
            return sum;

        }

        void Form1_Scanned(object sender, EventArgs ev)
        {
            #region cash
            /*Singletone  - ���� �������� ��� ���
typedef std::pair<BookId, bool> BookFreePair;
typedef std::vector<BookFreePair> BooksFreeType;

� ������ ������ ���� ��������� BooksFreeType

BooksFreeType cache_;
...
bool isBookBusy(BookId bookId)
{
   if(iterator find = cashe_.find(bookId))
      return find->second;

   bool busy = ReadBookBusyFromBD();
   cache.push_back(std::make_pair(bookId, busy));
   return busy;
}
              */
            #endregion
            //MessageBox.Show(((IOPOSScanner_1_10)sender).ScanData.ToString());
            string g = tabControl1.SelectedTab.ToString();
            switch (tabControl1.SelectedTab.Text)
            {
                case "�������� ��������":
                    #region formular
                    //string _data = ((IOPOSScanner_1_10)sender).ScanData.ToString();
                    string _data = FromPort;
                    if (!dbw.isReader(_data))
                    {
                        MessageBox.Show("�������� �������� ��������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    /*if (_data.Length < 20)
                        _data = _data.Remove(0, 1);*/
                    //_data = _data.Remove(_data.Length - 1, 1);
                    ReaderRecordFormular = new DBWork.dbReader(_data);

                    if (ReaderRecordFormular.barcode == "notfoundbynumber")
                    {
                        MessageBox.Show("�������� �� ������, ���� �������� ��������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (ReaderRecordFormular.barcode == "numsoc")
                    {
                        MessageBox.Show("�������� �� ������, ���� �������� �������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (ReaderRecordFormular.barcode == "sersoc")
                    {
                        MessageBox.Show("�� ������������� ����� ���������� �����!�������� ������� ���������� �����!����� ���������� ����� ������� �������, �� ��������� �����! ����� ���������� ����� ���������� ���������������� � ������������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    OldBase(ReaderRecordFormular);
                    label20.Text = ReaderRecordFormular.Surname + " " + ReaderRecordFormular.Name + " " + ReaderRecordFormular.SecondName;
                    textBox6.Text = ReaderRecordFormular.AbonType;
                    label25.Text = ReaderRecordFormular.id;

                    //dbw.SetPenalty(ReaderRecordFormular.id);
                    this.FormularColumnsForming(ReaderRecordFormular.id);
                
                    /*Formular.Columns[1].Width = 100;
                    Formular.Columns[2].Visible = false;
                    Formular.Columns[4].Visible = false;
                    Formular.Columns[3].HeaderText = "�����";
                    Formular.Columns[3].Width = 90;
                    Formular.Columns[5].HeaderText = "��� �������";
                    Formular.Columns[5].Width = 110;
                    Formular.Columns[7].Visible = false;
                    Formular.Columns[6].HeaderText = "����� �������";
                    Formular.Columns[6].Width = 170;
                    Formular.Columns[8].HeaderText = "���� ������";
                    Formular.Columns[8].Width = 130;
                    Formular.Columns[9].HeaderText = "�������������� ���� ��������";
                    Formular.Columns[9].Width = 130;
                    Formular.Columns[10].HeaderText = "����������� ���� ��������";
                    Formular.Columns[10].Width = 130;
                    Formular.Columns[11].HeaderText = "���������";
                    Formular.Columns[11].Width = 130;*/
                    
                    
                    //Formular.Columns[8].Visible = false;
                    //Formular.Columns[9].Visible = false;
                    Sorting.WhatStat = Stats.Formular;
                    Sorting.AuthorSort = SortDir.None;
                    Sorting.ZagSort = SortDir.None;
                    break;
                    #endregion
                case "������ ��������� ��������":
                    #region vidachashtrihcoda

                    if (label16.Visible)
                    {
                        //string BarCode = ((IOPOSScanner_1_10)sender).ScanData.ToString().Remove(((IOPOSScanner_1_10)sender).ScanData.ToString().Length - 1, 1);
                        string BarCode = FromPort;
                        switch (dbw.SetReaderBarCode(ReaderSetBarcode.id, BarCode))
                        {
                            case -5:
                                MessageBox.Show("������ �������� ��������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            case -1:
                                MessageBox.Show("������ ���� ������.", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            case -2:
                                MessageBox.Show("�������� �� ������", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            case -4:
                                MessageBox.Show("����� �������� ��� ���������� � ����! �������� ������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            case -3:
                                MessageBox.Show("� �������� ���� �������� �� ���������� �����! ������ ��������� ����� �������� �� ���������.", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                groupBox4.Enabled = true;
                                break;
                            case 1:
                                textBox5.Text = BarCode;
                                //timer2.Enabled = false;
                                label16.Visible = false;
                                //tabControl1.Enabled = true;
                                groupBox4.Enabled = true;
                                label16.Visible = false;
                                MessageBox.Show("�������� ������� ������ � ��������!", "�������!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;

                        }
                        return;
                    }
                    else
                        MessageBox.Show("������� ������� ��������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                    #endregion
                case "����/������ �������":
                    #region priemVidacha
                    //timer1.Enabled = true;
                    label1.Enabled = true;
                    //button2.Enabled = false;
                    //button4.Enabled = false;
                    
                    //_data = "9643909027074162622 1107009";

                    if ((this.emul == "") || (this.emul == null))
                    {
                        _data = FromPort;
                    }
                    else
                    {
                        _data = this.emul;
                    }
                    if ((this.ReaderRecord != null) && (this.BookRecord != null))
                    {
                        MessageBox.Show("����������� ���������� ��������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.emul = "";
                        return;
                    }
                    //MessageBox.Show(_data);
                    ReaderRecordWork = null;
                    BookRecordWork = null;
                    if (dbw.isReader(_data))
                    {
                        /*if (_data.Length < 20)
                            _data = _data.Remove(0, 1);*/
                        //_data = _data.Remove(_data.Length - 1, 1);
                        ReaderRecordWork = new DBWork.dbReader(_data);
                        if (ReaderRecordWork.barcode == "notfoundbynumber")
                        {
                            MessageBox.Show("�������� �� ������, ���� �������� ��������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.emul = "";
                            return;
                        }
                        if (ReaderRecordWork.barcode == "numsoc")
                        {
                            MessageBox.Show("�������� �� ������, ���� �������� �������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.emul = "";
                            return;
                        }
                        if (ReaderRecordWork.barcode == "sersoc")
                        {
                            MessageBox.Show("�� ������������� ����� ���������� �����!�������� ������� ���������� �����!����� ���������� ����� ������� �������, �� ��������� �����! ����� ���������� ����� ���������� ���������������� � ������������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.emul = "";
                            return;
                        }

                        if (OldBase())
                        {
                            this.emul = "";
                            return;
                        }
                        if (this.ReaderRecord != null)
                        {
                            this.emul = "";
                            return;
                        }
                        if (this.ReaderRecord != null)
                        {
                            MessageBox.Show("����������� ���������� ��������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.emul = "";
                            return;
                        }
                        if (ReaderRecordWork.barcode == "error")
                        {
                            MessageBox.Show("�������� �� ��������������� ���� �� ������������� ����� ���������� �����!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.emul = "";
                            return;
                        }
                        /*if (ReaderRecordWork.RegInMos != DateTime.MinValue)
                        {
                            if ((ReaderRecordWork.RegInMos - DateTime.Today).Days < 60)
                            {
                                MessageBox.Show("� �������� ������������� ���� ����������� � ������! �������� ����� 60 ����!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.emul = "";
                            }
                        }*/
                        
                    }
                    else
                    {
                        //_data = _data.Remove(_data.Length - 1, 1);
                        BookRecordWork = new DBWork.dbBook(_data);
                        if (BookRecordWork.id == "�������� ��������")
                        {
                            MessageBox.Show("������ �������� �������� ���� ������� �� �������� (�����) ��������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.emul = "";
                            return;
                         }
                        
                    }
                    if ((BookRecord != null) && (BookRecordWork != null))
                    {
                        MessageBox.Show("������� ��������� 2-� ������� ������! �������� �������� ��������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.emul = "";
                        return;
                    }

                    if (ReaderRecord == null)
                    {
                        if (BookRecord != null)
                        {
                            if (ReaderRecordWork != null)
                            {
                                ReaderRecord = ReaderRecordWork.Clone();
                                this.label5.Text = ReaderRecord.FIO;
                                //��������� ����� � �������� ����� ������ � ����������� ��� ������
                                button2.Enabled = true;
                                button4.Enabled = true;
                                button2.Focus();
                                this.AcceptButton = button2;
                                label1.Text = "����������� ������";
                            }
                            else
                            {
                                MessageBox.Show("������� ��������� 2-� ������� ������! �������� �������� ��������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.emul = "";
                                return;
                            }
                        }
                        else
                        {
                            if (ReaderRecordWork != null)
                            {
                                MessageBox.Show("������ �������� ��������! �������� �������� �������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.emul = "";
                                return;
                            }
                            else
                            {
                                if (dbw.isBookBusy(_data))
                                {
                                    string expireddays = GetExpiredDays(BookRecordWork.inv);
                                    dbw.setBookReturned(BookRecordWork.id, BookRecordWork);
                                    
                                    if (int.Parse(expireddays) > 0)
                                    {
                                        MessageBox.Show("������� ������ ����� ��������� �� " + expireddays + " ����. ����� ���������� " + CalculatePeny(BookRecordWork.vzv, DateTime.Today).ToString() + " ���.", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }

                                    dataGridView1.Rows.Insert(0, 1);
                                    dataGridView1.Rows[0].Cells[0].Value = BookRecordWork.inv;
                                    dataGridView1.Rows[0].Cells[1].Value = BookRecordWork.name;
                                    if (BookRecordWork.rname == "-1")
                                    {
                                        BookRecordWork.rname = "����������";
                                    }
                                    dataGridView1.Rows[0].Cells[2].Value = BookRecordWork.rname;
                                    dataGridView1.Rows[0].Cells[3].Value = "����������";
                                    dbw.InsertActionRETURNED(new DBWork.dbReader(BookRecordWork.rbar), BookRecordWork);
                                    BookRecord = null;
                                    ReaderRecord = null;
                                    button2.Enabled = false;
                                    button4.Enabled = false;
                                    label1.Text = "�������� �������� �������";
                                    //MessageBox.Show("����� ����������!", "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    this.emul = "";
                                    return;
                                }
                                else
                                {
                                    this.label8.Text = BookRecordWork.author;
                                    this.label9.Text = BookRecordWork.name;
                                    this.label1.Text = "�������� ����� ��� ��������";
                                    this.button4.Enabled = true;
                                    BookRecord = BookRecordWork.Clone();
                                    this.emul = "";
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (BookRecord != null)
                        {
                            MessageBox.Show("����������� ���������� ��������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            button2.Enabled = true;
                            button4.Enabled = true;
                            this.emul = "";
                            return;
                        }
                        else
                        {
                            this.label1.Text = "�������� ����� ��� �������!";
                            BookRecord = null;
                            ReaderRecord = null;
                        }
                    }

                    /*if (dbw.isReader(_data))
                        ReaderRecord = dbw.getDbReader(_data);
                    else
                        BookRecord = dbw.getDbBook(_data);*/
                    this.emul = "";
                    break;
                    #endregion
                case "�������":
                    //timer1.Enabled = false;
                    label1.Enabled = false;
                    //timer2.Enabled = false;
                    label16.Visible = false;
                    break;

            }
        }

        private string GetExpiredDays(string inv)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select * from Reservation_R..ISSUED where INV = '"+inv+"' and IDMAIN <> 0" ;
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet DS = new DataSet();
            int i  = Conn.SQLDA.Fill(DS, "tmp");
            if (i == 0) return "0";
            DateTime vzv = (DateTime)DS.Tables["tmp"].Rows[0]["DATE_VOZV"];
            TimeSpan rtr = DateTime.Today - vzv;
            return rtr.Days.ToString();
        }

        public static event ScannedEventHandler Scanned;
        public void FireScan(object sender, EventArgs ev)
        {
            if (Form1.Scanned != null)
                Form1.Scanned(sender, ev);
        }
        private void button1_Click_1(object sender, EventArgs e)
        {

            //dbw.setReaderRight("1000002");
            //dbw.setBookReturned("503");
            //if (dataGridView1.Rows[0].Cells[0].Value != null)
            /*if ((dataGridView1.Rows.Count == 1) && (dataGridView1.Rows[0].Cells[0].Value == null))
                dataGridView1.Rows[0].Cells[0].Value = DateTime.Now.ToLongTimeString();
            else
            {
                dataGridView1.Rows.Insert(0, 1);
                dataGridView1.Rows[0].Cells[0].Value = DateTime.Now.ToLongTimeString();
            }*/
            //string d = dbw.getBookFromZAKAZ("R00063Y0803").id;
            //bool f = dbw.isReaderHaveRights("R00063Y0803", "R1000004g");
            //string f = dbw.getBookFromZAKAZ("R00063Y0803").name;
            //dbw.setBookForReader("R00063Y0803", "1234", (int)numericUpDown1.Value);

            //dataGridView1.Rows.Add(1);
            //dataGridView1.Rows[dataGridView1.Rows.Count-1].Cells[0].Value = DateTime.Now.ToShortTimeString().ToString();
            //dataGridView1.Rows[dataGridView1.Rows.Count-1].Cells[1].Value = "�������� " + dbw.getDbReader("1234").FIO + " ������ �����.";

            //            dbw.setBookReturned("1");
            //MessageBox.Show(dbw.getDbReader("1234").barcode.ToString() + dbw.getDbReader("1234").id.ToString());
            //MessageBox.Show(dbw.getDbBook("R00063Y0803").barcode);
            //string f = dbw.isReader("R1000001");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //BookRecordWork = new DBWork.dbBook("R00063Y0803");

            f2.textBox2.Text = "";
            f2.textBox3.Text = "";
            f2.ShowDialog();
            if ((this.EmpID == "") || (this.EmpID == null))
            {
                MessageBox.Show("�� �� ������������! ��������� ����������� ���� ������", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Visible = !label1.Visible;
        }
        public bool OldBase()
        {
            Conn.SQLDA.SelectCommand.CommandText = "select IDOldAbonement from Readers..Main where NumberReader = " + ReaderRecordWork.id;
            DataSet DS = new DataSet();
            Conn.SQLDA.Fill(DS, "iswas");
            if (!(bool)DS.Tables["iswas"].Rows[0][0])
            {
                Form10 f10 = new Form10(ReaderRecordWork);
                if (f10.canShow)
                {
                    f10.ShowDialog();
                    button4_Click(button4, new EventArgs());
                }
                else
                {
                    button4_Click(button4, new EventArgs());
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool OldBase(DBWork.dbReader rdr)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select IDOldAbonement from Readers..Main where NumberReader = " + rdr.id;
            DataSet DS = new DataSet();
            Conn.SQLDA.Fill(DS, "iswas");
            bool d = (bool)DS.Tables["iswas"].Rows[0][0];
            if (!d)
            {
                Form10 f10 = new Form10(rdr);
                if (f10.canShow)
                {
                    f10.ShowDialog();
                    button4_Click(button4, new EventArgs());
                }
                else
                {
                    button4_Click(button4, new EventArgs());
                }
                return true;
            }
            else
            {
                return false;
            }


        }
        private void button18_Click(object sender, EventArgs e)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select IDOldAbonement from Readers..Main where NumberReader = "+ ReaderRecord.id;
            DataSet DS = new DataSet();
            Conn.SQLDA.Fill(DS, "iswas");
            if (!(bool)DS.Tables["iswas"].Rows[0][0])
            {
                Form10 f10 = new Form10(ReaderRecord);
                f10.ShowDialog();
            }

        }
        public void button2_Click_1(object sender, EventArgs e)
        {
            //FindReaderInOldBase(ReaderRecord);
            if (ReaderRecord.RegInMos != DateTime.MinValue)
            {
                if ((ReaderRecord.RegInMos - DateTime.Today).Days < 60)
                {
                    MessageBox.Show("� �������� ������������� ���� ����������� � ������! �������� ����� 60 ����!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.emul = "";
                }
            }

            bool set = false;
            //long copy_tStatus;
            //copy_tStatus = 0;
            if (dbw.isBookBusy(BookRecord.barcode))
            {
                MessageBox.Show("����� � ������� ��������! ���� ��������: " + dbw.GetDateRet(BookRecord.barcode) + ".", "��������!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                this.label8.Text = "";
                this.label9.Text = "";
                this.label5.Text = "";
                BookRecord = null;
                ReaderRecord = null;
            }
            else
            {
                if (dbw.isReaderHaveRights(ReaderRecord))
                {
                    if (!dbw.isRightsExpired(ReaderRecord.id))
                    {
                        set = true;
                    }
                    else
                    {
                        switch (MessageBox.Show("� ������� �������� ���������� ���� ���� ����������� ������������ �����������! ������ �������� ����� ������������ ����� �� ��������� ���� ������������� ���������� � ������ ��� �����?", "��������!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                        {
                            case DialogResult.Yes:
                                set = true;
                                dbw.ProlongRights(ReaderRecord.id);
                                break;
                            case DialogResult.No:
                                set = false;
                                this.label8.Text = "";
                                this.label9.Text = "";
                                this.label5.Text = "";
                                BookRecord = null;
                                ReaderRecord = null;
                                button2.Enabled = false;
                                button4.Enabled = false;
                                label1.Text = "�������� �������� �������";
                                break;
                            case DialogResult.Cancel:
                                set = false;
                                break;
                        }

                    }

                }
                else
                {
                    switch (MessageBox.Show("� ������� �������� ��� ���� ��� ��������� ���� ������������� ����������! ������ ������ ����� ������������ ����� �� ��������� ���� ������������� ���������� � ������ ��� �����?", "��������!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.Yes:
                            set = true;
                            dbw.setReaderRight(ReaderRecord.id);
                            break;
                        case DialogResult.No:
                            set = false;
                            this.label8.Text = "";
                            this.label9.Text = "";
                            this.label5.Text = "";
                            BookRecord = null;
                            ReaderRecord = null;
                            button2.Enabled = false;
                            button4.Enabled = false;
                            label1.Text = "�������� �������� �������";
                            break;
                        case DialogResult.Cancel:
                            set = false;
                            break;
                    }

                }
                if (set)
                {
                    /*if (ReaderRecord.AbonType == "��� ��������")
                    {
                        MessageBox.Show("� ������� �������� �� ��������� �������� ���� ����������. ������ ����������. ������� ��������� �������� ��� ���������� �� ������� \"�������� ��������\"�", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }*/
                    if (dbw.GetBookCountForReader(ReaderRecord.id) >= 5)
                    {
                        switch (MessageBox.Show("������ �������� �������� ����� ����� 5 ����. ������ ���������� ������?", "��������!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                        {
                            case DialogResult.No:
                                this.label8.Text = "";
                                this.label9.Text = "";
                                this.label5.Text = "";
                                BookRecord = null;
                                ReaderRecord = null;
                                button2.Enabled = false;
                                button4.Enabled = false;
                                label1.Text = "�������� �������� �������";
                                return;
                            case DialogResult.Yes:
                                dbw.setBookForReader(BookRecord, ReaderRecord, (int)numericUpDown1.Value);
                                dataGridView1.Rows.Insert(0, 1);
                                dataGridView1.Rows[0].Cells[0].Value = BookRecord.inv;
                                dataGridView1.Rows[0].Cells[1].Value = BookRecord.name;
                                BookRecord = new DBWork.dbBook(BookRecord.barcode);
                                dataGridView1.Rows[0].Cells[2].Value = ReaderRecord.FIO;
                                dataGridView1.Rows[0].Cells[3].Value = "������";
                                this.label8.Text = "";
                                this.label9.Text = "";
                                this.label5.Text = "";
                                dbw.InsertActionISSUED(ReaderRecord,BookRecord);
                                BookRecord = null;
                                ReaderRecord = null;
                                button2.Enabled = false;
                                button4.Enabled = false;
                                label1.Text = "�������� �������� �������";
                                break;
                        }

                    }
                    else
                    {
                        dbw.setBookForReader(BookRecord, ReaderRecord, (int)numericUpDown1.Value);
                        dataGridView1.Rows.Insert(0, 1);
                        dataGridView1.Rows[0].Cells[0].Value = BookRecord.inv;
                        dataGridView1.Rows[0].Cells[1].Value = BookRecord.name;
                        BookRecord = new DBWork.dbBook(BookRecord.barcode);
                        dataGridView1.Rows[0].Cells[2].Value = ReaderRecord.FIO;
                        dataGridView1.Rows[0].Cells[3].Value = "������";
                        this.label8.Text = "";
                        this.label9.Text = "";
                        this.label5.Text = "";
                        dbw.InsertActionISSUED(ReaderRecord,BookRecord);
                        BookRecord = null;
                        ReaderRecord = null;
                        button2.Enabled = false;
                        button4.Enabled = false;
                        label1.Text = "�������� �������� �������";
                    }

                }
            }
            BookRecord = null;
            ReaderRecord = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //dbw.isBookBusy("");
            this.label8.Text = "";
            this.label9.Text = "";
            this.label5.Text = "";
            BookRecord = null;
            ReaderRecord = null;
            label1.Text = "�������� �������� �������";
            button2.Enabled = false;
            button4.Enabled = false;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            ReaderSetBarcode = new DBWork.dbReader((int)numericUpDown2.Value);
            if (ReaderSetBarcode.barcode == "error")
            {
                MessageBox.Show("�������� �� ������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                button6.Enabled = false;
                return;
            }
            textBox2.Text = ReaderSetBarcode.Surname;
            textBox3.Text = ReaderSetBarcode.Name;
            textBox4.Text = ReaderSetBarcode.SecondName;
            textBox5.Text = "R" + ReaderSetBarcode.barcode;
            button8.Visible = true;
            //timer2.Enabled = true;
            label16.Visible = true;
            //tabControl1.Enabled = false;
            groupBox4.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //timer2.Enabled = true;
            label16.Visible = true;
            //tabControl1.Enabled = false;
            groupBox4.Enabled = false;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedTab.Text)
            {
                case "������ ��������� ��������":
                    //timer2.Enabled = false;
                    label16.Visible = false;
                    //timer1.Enabled = false;
                    label1.Enabled = false;
                    numericUpDown2.Value = 0;
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                    this.label16.Text = "�������� ��������";
                    button6.Enabled = false;
                    button8.Visible = false;
                    break;
                case "����/������ �������":
                    //timer1.Enabled = true;
                    label1.Enabled = true;
                    //timer2.Enabled = false;
                    label16.Visible = false;
                    
                    //this.label8.Text = "";
                    //this.label9.Text = "";
                    //this.label5.Text = "";
                    //this.label16.Text = "";
                    //this.label16.Visible = false;
                    //BookRecord = null;
                    //ReaderRecord = null;
                    label1.Text = "�������� �������� �������";
                    //button2.Enabled = false;
                    //button4.Enabled = false;
                    this.AcceptButton = button5;
                    break;
                case "�������":
                    //timer1.Enabled = false;
                    label1.Enabled = false;
                    //timer2.Enabled = false;
                    label16.Visible = false;
                    this.label16.Visible = false;
                    this.label16.Text = "";
                    break;
                case "�������� ��������":
                    label25.Text = "";
                    label20.Text = "";
                    textBox6.Text = "";
                    Formular.Columns.Clear();
                    AcceptButton = this.button10;

                    break;

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: ������ ������ ���� ��������� ��������� ������ � ������� "bRIT_SOVETDataSet.ZAKAZ". ��� ������������� ��� ����� ���� ���������� ��� �������.
            //this.zAKAZTableAdapter.Fill(this.bRIT_SOVETDataSet.ZAKAZ);
            //this.EmpID = "1";
            if ((this.EmpID == "") || (this.EmpID == null))
            {
                MessageBox.Show("�� �� ������������! ��������� ����������� ���� ������", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            //this.reportViewer1.RefreshReport();
            //this.reportViewer2.RefreshReport();
        }


        private void button7_Click(object sender, EventArgs e)
        {
            //int Y = button7.Location.Y-20;
            button12.Enabled = false;
            int x = this.Left + button7.Left;
            int y = this.Top + button7.Top + tabControl1.Top + 60;
            //Point p = button7.PointToScreen(button7.Location);
            contextMenuStrip1.Show(x, y);
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            label16.Visible = !label16.Visible;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Statistics.Columns.Clear();
            Form3 f3 = new Form3();
            f3.ShowDialog();
            label19.Text = "���������� ���������, �� ������ �" + f3.StartDate.ToString("yyyyMMdd") + " �� " + f3.EndDate.ToString("yyyyMMdd") + ": ";
            label18.Text = dbw.GetReaderCount(f3.StartDate, f3.EndDate);
        }


        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Statistics.Columns.Clear();
            Form3 f3 = new Form3();
            f3.ShowDialog();
            //label17.Text = "���������� �������� ����������, �� ������ �" + f3.StartDate.ToString("dd.MM.yyyy") + " �� " + f3.EndDate.ToString("dd.MM.yyyy") + ": " + dbw.GetBooksCount(f3.StartDate, f3.EndDate);
            label19.Text = "���-�� �������� ����������, �� ������ � " + f3.StartDate.ToString("dd.MM.yyyy") + " �� " + f3.EndDate.ToString("dd.MM.yyyy") + ": ";
            label18.Text = dbw.GetBooksCount(f3.StartDate, f3.EndDate);
        }
        public void autoinc(DataGridView dgv)
        {
            //listBox1.end
            int i = 0;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                row.Cells[0].Value = ++i;
            }
            //Statistics.Rows[Statistics.Rows.Count - 1].Cells[0].Value = "";
        }
        

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Statistics.Columns.Clear();
            Statistics.Columns.Add("NN", "� �/�");
            label19.Text = "������ ���� ����������, ����������� � ������� � ����� �� " + DateTime.Now.ToShortDateString() + " :";
            label18.Text = "";
            Statistics.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            backgroundWorker1.RunWorkerAsync();
            RndPrg = new ExtGui.RoundProgress();
            RndPrg.Visible = true;
            RndPrg.Name = "progress";
            tabControl1.TabPages[1].Controls.Add(RndPrg);
            RndPrg.BringToFront();
            RndPrg.Size = new Size(40, 60);
            RndPrg.Location = new Point(450, 200);
            RndPrg.BackColor = SystemColors.AppWorkspace;
            //int p1 = 0;
            //int p2 = 0;
            //Action<int>
            //backgroundWorker2.RunWorkerAsync();
            

            /*progressBar1.Invoke(delegate()
            {
                progressBar1.Value = p1;
            });
            /*delegate()
            {
                while (p1 != 100)
                {
                    p1++;
                    Thread.CurrentThread.Join(1000);

                    progressBar1.Invoke((ThreadStart)delegate()
                    {
                        progressBar1.Value = p1;
                    });
                }
            };*/

            //------------------------------------------------------

            /*new Thread(delegate()
            {
                while (p1 != 100)
                {
                    p1++;
                    Thread.CurrentThread.Join(1000);

                    progressBar1.Invoke((ThreadStart)delegate()
                    {
                        progressBar1.Value = p1;
                    });
                }
            }).Start();*/

            
            //backgroundWorker2.RunWorkerAsync(backgroundWorker1.IsBusy);
            //Statistics.DataSource = dbw.GetAllBooks();
            /*autoinc(Statistics);
            Statistics.Columns[0].Width = 50;
            Statistics.Columns[1].HeaderText = "����� �����";
            Statistics.Columns[1].Width = 140;
            Statistics.Columns[2].HeaderText = "��������";
            Statistics.Columns[2].Visible = false;
            Statistics.Columns[3].HeaderText = "��������";
            Statistics.Columns[3].Width = 330;
            Statistics.Columns[4].HeaderText = "�����";
            Statistics.Columns[4].Width = 150;
            Statistics.Columns[5].HeaderText = "��� �������";
            Statistics.Columns[5].Width = 70;
            Statistics.Columns[6].HeaderText = "�������� ������";
            Statistics.Columns[6].Width = 80;
            Statistics.Columns[7].Visible = false;
            Statistics.Columns[8].Visible = false;
            Statistics.Columns[8].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            Statistics.Columns[9].HeaderText = "������";
            Statistics.Columns[9].Width = 100;
            Sorting.WhatStat = Stats.AllBooks;
            Sorting.AuthorSort = SortDir.None;
            Sorting.ZagSort = SortDir.None;
            //Statistics.
            //Statistics.Columns[0].SortMode = DataGridViewColumnSortMode.Programmatic;
            //Statistics.Columns[2].SortMode = DataGridViewColumnSortMode.;
            button12.Enabled = true;*/
        }
        //public static event HeaderClick eHeaderClick;

        public void FireHeaderClick(object sender, DataGridViewCellMouseEventArgs ev)
        {
            autoinc(Statistics);

        }
        public enum Stats { Debtors, AllBooks, IssuedBooks, Formular };
        public enum SortDir { Asc, Desc, None };
        class Sorting
        {
            private static SortDir authorSort;
            public static SortDir AuthorSort
            {
                get { return authorSort; }
                set { authorSort = value; }
            }
            private static SortDir zagSort;
            public static SortDir ZagSort
            {
                get { return zagSort; }
                set { zagSort = value; }
            }
            private static Stats whatStat;
            public static Stats WhatStat
            {
                get { return whatStat; }
                set { whatStat = value; }
            }
            private static bool sortOrd;
            public static bool SortOrd
            {
                get { return sortOrd; }
                set { sortOrd = value; }
            }


        }

        private void Statistics_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //MouseEventArgs m = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
            //DataGridViewCellMouseEventArgs ev = new DataGridViewCellMouseEventArgs(1, 0, 0, 0, m);
            //this.Statistics_ColumnHeaderMouseClick(Statistics, ev);// .FireHeaderClick(Statistics, ev);
            //Statistics.Columns[5].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
            /*DataGridView C1 = (DataGridView)sender;
            if (C1.Name == "Statistics")
            {
                autoinc(Statistics);
                return;
            }
            switch (Sorting.WhatStat)
            {
                case Stats.IssuedBooks:
                    if ((e.ColumnIndex == 1) && ((Sorting.ZagSort == SortDir.Asc)))
                    {
                        Statistics.Sort(Statistics.Columns[5], ListSortDirection.Descending);
                        Statistics.Columns[1].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                        Sorting.ZagSort = SortDir.Desc;
                        //                        Statistics.SortOrder;
                    }
                    else
                    {
                        if ((e.ColumnIndex == 1) && ((Sorting.ZagSort == SortDir.Desc) || (Sorting.ZagSort == SortDir.None)))
                        {
                            Statistics.Sort(Statistics.Columns[5], ListSortDirection.Ascending);
                            Statistics.Columns[1].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                            Sorting.ZagSort = SortDir.Asc;
                            if ((e.ColumnIndex == 2) && ((Sorting.AuthorSort == SortDir.Asc)))
                            {
                                Statistics.Sort(Statistics.Columns[6], ListSortDirection.Descending);
                                Statistics.Columns[2].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                                Sorting.AuthorSort = SortDir.Desc;
                            }
                            else
                                if ((e.ColumnIndex == 2) && ((Sorting.AuthorSort == SortDir.Desc) || (Sorting.AuthorSort == SortDir.None)))
                                {
                                    Statistics.Sort(Statistics.Columns[6], ListSortDirection.Ascending);
                                    Statistics.Columns[2].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                                    Sorting.AuthorSort = SortDir.Asc;
                                }
                                else
                                {
                                    Statistics.Sort(Statistics.Columns[e.ColumnIndex], ListSortDirection.Ascending);
                                }
                        }
                        else
                        {
                            if (e.ColumnIndex != 0)
                            {
                                Statistics.Sort(Statistics.Columns[e.ColumnIndex], ListSortDirection.Ascending);
                            }
                            //Statistics.Columns[1].ValueType = typeof(DateTime);
                        }
                    }
                    break;
                case Stats.AllBooks:
                    if ((e.ColumnIndex == 2) && ((Sorting.ZagSort == SortDir.Asc)))
                    {
                        Statistics.Sort(Statistics.Columns[6], ListSortDirection.Descending);
                        Statistics.Columns[2].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                        Sorting.ZagSort = SortDir.Desc;
                        //                        Statistics.SortOrder;
                    }
                    else
                        if ((e.ColumnIndex == 2) && ((Sorting.ZagSort == SortDir.Desc) || (Sorting.ZagSort == SortDir.None)))
                        {
                            Statistics.Sort(Statistics.Columns[6], ListSortDirection.Ascending);
                            Statistics.Columns[2].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                            Sorting.ZagSort = SortDir.Asc;
                            if ((e.ColumnIndex == 3) && ((Sorting.AuthorSort == SortDir.Asc)))
                            {
                                Statistics.Sort(Statistics.Columns[7], ListSortDirection.Descending);
                                Statistics.Columns[3].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                                Sorting.AuthorSort = SortDir.Desc;
                            }
                            else
                                if ((e.ColumnIndex == 3) && ((Sorting.AuthorSort == SortDir.Desc) || (Sorting.AuthorSort == SortDir.None)))
                                {
                                    Statistics.Sort(Statistics.Columns[7], ListSortDirection.Ascending);
                                    Statistics.Columns[3].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                                    Sorting.AuthorSort = SortDir.Asc;
                                }
                                else
                                {
                                    Statistics.Sort(Statistics.Columns[e.ColumnIndex], ListSortDirection.Ascending);
                                }
                        }
                        else
                        {
                            Statistics.Sort(Statistics.Columns[e.ColumnIndex], ListSortDirection.Ascending);
                        }
                    break;
                case Stats.Debtors:
                    break;
                    if ((e.ColumnIndex == 1) && ((Sorting.ZagSort == SortDir.Asc)))
                    {
                        Statistics.Sort(Statistics.Columns[5], ListSortDirection.Descending);
                        Statistics.Columns[1].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                        Sorting.ZagSort = SortDir.Desc;
                    }
                    else
                    {
                        if ((e.ColumnIndex == 1) && ((Sorting.ZagSort == SortDir.Desc) || (Sorting.ZagSort == SortDir.None)))
                        {
                            Statistics.Sort(Statistics.Columns[5], ListSortDirection.Ascending);
                            Statistics.Columns[1].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                            Sorting.ZagSort = SortDir.Asc;
                            if ((e.ColumnIndex == 2) && ((Sorting.AuthorSort == SortDir.Asc)))
                            {
                                Statistics.Sort(Statistics.Columns[6], ListSortDirection.Descending);
                                Statistics.Columns[2].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                                Sorting.AuthorSort = SortDir.Desc;
                            }
                            else
                                if ((e.ColumnIndex == 2) && ((Sorting.AuthorSort == SortDir.Desc) || (Sorting.AuthorSort == SortDir.None)))
                                {
                                    Statistics.Sort(Statistics.Columns[6], ListSortDirection.Ascending);
                                    Statistics.Columns[2].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                                    Sorting.AuthorSort = SortDir.Asc;
                                }
                                else
                                {
                                    Statistics.Sort(Statistics.Columns[e.ColumnIndex], ListSortDirection.Ascending);
                                }
                        }
                        else
                        {
                            Statistics.Sort(Statistics.Columns[e.ColumnIndex], ListSortDirection.Ascending);
                        }
                    }

                    break;
                case Stats.Formular:
                    if ((e.ColumnIndex == 6) && ((Sorting.ZagSort == SortDir.Asc)))
                    {
                        Statistics.Sort(Statistics.Columns[8], ListSortDirection.Descending);
                        Statistics.Columns[6].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                        Sorting.ZagSort = SortDir.Desc;
                    }
                    else
                        if ((e.ColumnIndex == 6) && ((Sorting.ZagSort == SortDir.Desc) || (Sorting.ZagSort == SortDir.None)))
                        {
                            Statistics.Sort(Statistics.Columns[8], ListSortDirection.Ascending);
                            Statistics.Columns[6].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                            Sorting.ZagSort = SortDir.Asc;
                            if ((e.ColumnIndex == 7) && ((Sorting.AuthorSort == SortDir.Asc)))
                            {
                                Statistics.Sort(Statistics.Columns[9], ListSortDirection.Descending);
                                Statistics.Columns[7].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                                Sorting.AuthorSort = SortDir.Desc;
                            }
                            else
                                if ((e.ColumnIndex == 7) && ((Sorting.AuthorSort == SortDir.Desc) || (Sorting.AuthorSort == SortDir.None)))
                                {
                                    Statistics.Sort(Statistics.Columns[9], ListSortDirection.Ascending);
                                    Statistics.Columns[7].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                                    Sorting.AuthorSort = SortDir.Asc;
                                }
                                else
                                {
                                    Statistics.Sort(Statistics.Columns[e.ColumnIndex], ListSortDirection.Ascending);
                                }
                        }
                        else
                        {
                            Statistics.Sort(Statistics.Columns[e.ColumnIndex], ListSortDirection.Ascending);
                        }

                    break;
            }
            /*if (e.ColumnIndex == 1)
            {
                AscDescZag = !AscDescZag;
                Statistics.Sort(Statistics.Columns[5], AscDescZag ? ListSortDirection.Ascending : ListSortDirection.Descending);
                //Statistics.SortOrder = !!!!!!!
            }
            if (e.ColumnIndex == 2)
            {
                AscDescAvt = !AscDescAvt;
                Statistics.Sort(Statistics.Columns[6], AscDescAvt ? ListSortDirection.Ascending : ListSortDirection.Descending);
            }*/
            if (label19.Text.Contains("�������"))
            foreach (DataGridViewRow r in Statistics.Rows)
            {
                if (r.Cells[10].Value.ToString() == "true")
                {
                    r.DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }
            autoinc(Statistics);
            
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            button8.Visible = false;
            //timer2.Enabled = false;
            label16.Visible = false;
            //tabControl1.Enabled = true;
            groupBox4.Enabled = true;
        }

        private void toolTip1_Draw(object sender, DrawToolTipEventArgs e)
        {

            //e.Graphics.FillRectangle(new SolidBrush(Color.SteelBlue), e.Bounds);
            e.DrawBackground();
            e.DrawBorder();
            TextRenderer.DrawText(e.Graphics, "red", e.Font, e.Bounds, Color.Red);
            //            e.DrawText();
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {


        }

        private void button10_Click(object sender, EventArgs e)
        {
            //dbw.GetFormular("1000001");

            if (this.numericUpDown3.Value.ToString() == "")
            {
                MessageBox.Show("������� ����� ��������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (this.numericUpDown3.Value <= 0)
            {
                MessageBox.Show("������� ����� ������ ����", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            ReaderSetBarcode = new DBWork.dbReader((int)numericUpDown3.Value);
            if (ReaderSetBarcode.barcode == "error")
            {
                MessageBox.Show("�������� �� ������!", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            OldBase(ReaderSetBarcode);
            dbw.GetFormular(ReaderSetBarcode.id);
            label20.Text = ReaderSetBarcode.Surname + " " + ReaderSetBarcode.Name + " " + ReaderSetBarcode.SecondName;
            textBox6.Text = ReaderSetBarcode.AbonType;
            label25.Text = ReaderSetBarcode.id;
            ReaderRecordFormular = new DBWork.dbReader(ReaderSetBarcode);

            this.FormularColumnsForming(ReaderSetBarcode.id);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (label25.Text == "")
            {
                MessageBox.Show("������� ������� ��������", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            f5 = new Form5(label25.Text);

            f5.ShowDialog();
            if (f5.abon != null)
                textBox6.Text = f5.abon;
            /*ReaderRecordFormular = new DBWork.dbReader("1000001");
            dbw.SetPenalty(ReaderRecordFormular.id);
            Formular.DataSource = dbw.GetFormular(ReaderRecordFormular.id);
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.HeaderText = "��������";
            Formular.Columns.Add(col);
            col.DataPropertyName = "Zag";
            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "��������";
            Formular.Columns.Add(col);
            col.DataPropertyName = "issue";
            DataGridViewCheckBoxColumn colch = new DataGridViewCheckBoxColumn();
            colch.HeaderText = "���������";
            colch.Name = "pen";
            Formular.Columns.Add(colch);
            Formular.Columns["pen"].ReadOnly = false;
            colch.DataPropertyName = "penalty";

            DataGridViewDisableButtonColumn ButCol = new DataGridViewDisableButtonColumn();
            Formular.Columns.Add(ButCol);
            ButCol.Name = "but";
            ButCol.HeaderText = "������ ���������";
            ButCol.DefaultCellStyle.BackColor = Form1.DefaultBackColor;
            Padding myPadd = ButCol.DefaultCellStyle.Padding;
            myPadd.All = 2;
            ButCol.DefaultCellStyle.Padding = myPadd;
            foreach (DataGridViewRow row in Formular.Rows)
            {
                DataGridViewDisableButtonCell bc = (DataGridViewDisableButtonCell)row.Cells["but"]; ;
                if ((row.Cells["pen"].Value.ToString().ToLower() == "false") && (row.Cells["rempen"].Value.ToString().ToLower() == "true"))
                {
                    bc.Value = "��� ���������";//����� �����
                    bc.Enabled = false;
                }


            }

           // Formular.DataMember = "form";*/
        }

        private void Formular_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            switch (Formular.Columns[e.ColumnIndex].Name)
            {
                case "but":
                    if (e.RowIndex == -1) break;
                    
                    if (((DataGridViewDisableButtonCell)Formular.Rows[e.RowIndex].Cells["but"]).Value.ToString() == "����� ���������")
                    {
                        switch (MessageBox.Show("�� ������� ��� ������ ����� ���������? ����� ������������� ����� �������� �� ����� ������, �.�. ��� ���������� � ������ ��������� ���������.", "��������!", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            case DialogResult.Yes:
                                this.dbw.RemPenalty(this.Formular.Rows[e.RowIndex].Cells["zi"].Value.ToString());
                                Conn.SQLDA.InsertCommand = new SqlCommand();
                                Conn.SQLDA.InsertCommand.Connection = Conn.ZakazCon;
                                Conn.SQLDA.InsertCommand.CommandText = "insert into Reservation_R..PENY_HIST (SUM, PDATE, INV, IDREADER, IDMAIN) values "+
                                                                       " ('"+Formular.Rows[e.RowIndex].Cells["peny"].Value.ToString()+
                                                                       "' , '"+DateTime.Now.ToString("yyyyMMdd")+
                                                                       "' ,  '" + Formular.Rows[e.RowIndex].Cells["inv"].Value.ToString() +
                                                                       "' , " + label25.Text + ", " + Formular.Rows[e.RowIndex].Cells["idmain"].Value.ToString() + ")";
                                if (Conn.SQLDA.InsertCommand.Connection.State == ConnectionState.Closed)
                                {
                                    Conn.SQLDA.InsertCommand.Connection.Open();
                                }
                                Conn.SQLDA.InsertCommand.ExecuteNonQuery();
                                Conn.SQLDA.InsertCommand.Connection.Close();
                                this.Formular.Rows.RemoveAt(e.RowIndex);
                                this.autoinc(Formular);
                                return;
                                //break;
                            case DialogResult.No:
                                return;
                                //break;
                        }
                    }
                        
                    f4 = new Form4();
                    f4.ShowDialog();
                    if (f4.Days == -99)
                        return;
                    if (!dbw.Prolong(f4.Days, Formular.Rows[e.RowIndex].Cells["idmain"].Value.ToString(), Formular.Rows[e.RowIndex].Cells["inv"].Value.ToString()))
                    {
                        Formular.Rows[e.RowIndex].Cells["pen"].Value = false;
                        //Formular.Rows[e.RowIndex].Cells["pen"].ReadOnly = true;
                        ((DataGridViewDisableButtonCell)Formular.Rows[e.RowIndex].Cells["but"]).Enabled = true;
                        ((DataGridViewDisableButtonCell)Formular.Rows[e.RowIndex].Cells["but"]).Value = "��������";
                    }
                    dbw.InsertActionProlong(new DBWork.dbReader(int.Parse(label25.Text)), new DBWork.dbBook(Formular.Rows[e.RowIndex].Cells["bar"].Value.ToString()));
                    Formular.Rows[e.RowIndex].Cells["vozv"].Value = DateTime.Parse(Formular.Rows[e.RowIndex].Cells["vozv"].Value.ToString()).AddDays(f4.Days);
                    Formular.Rows[e.RowIndex].Cells["peny"].Value = CalculatePeny(Formular.Rows[e.RowIndex]).ToString() + " �.";
                    return;
                    //break;
                case "pen":
                    if (e.RowIndex == -1) break;
                    if (Formular.Rows[e.RowIndex].Cells["pen"].Value.ToString().ToLower() == "true")
                    {
                        if (Formular.Rows[e.RowIndex].Cells["fact"].Value.ToString() == "")
                        {
                            MessageBox.Show("�� �� ������ ����� ��������� �������, �.�. ����� ��� �� ����������! ��������� ��������� ��� ��������� �����.", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            //switch (MessageBox.Show("����� ��� �� ����������. �� ������������� ������ ����� ���������? ", "��������!", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            //{
                            //    case DialogResult.Yes:
                            //        dbw.RemPenalty(Formular.Rows[e.RowIndex].Cells["idmain"].Value.ToString());
                            //        Formular.Rows[e.RowIndex].Cells["pen"].Value = false;
                            //        ((DataGridViewDisableButtonCell)Formular.Rows[e.RowIndex].Cells["but"]).Enabled = false;
                            //        ((DataGridViewDisableButtonCell)Formular.Rows[e.RowIndex].Cells["but"]).Value = "��� ���������";
                            //        break;
                            //    case DialogResult.No:
                            //        return;
                            //        //break;
                            //}
                        }
                        else
                        {
                            MessageBox.Show("����� ����� ��������� ������� �� ������ \"����� ���������\"", "��������!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    else
                    {
                        MessageBox.Show("������ ���������� ��������� �������: ��� ��������������� �������������.");
                        //Formular.Rows[e.RowIndex].Cells["pen"].Value = false;
                    }
                    break;
                
            }
        }

        private void Formular_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            /*switch (Formular.Columns[e.ColumnIndex].Name)
            {
                
            }*/
        }

        private void Formular_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ((e.ColumnIndex == 1) && ((Sorting.ZagSort == SortDir.Asc)))
            {
                Formular.Sort(Formular.Columns[2], ListSortDirection.Descending);
                Formular.Columns[1].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                Sorting.ZagSort = SortDir.Desc;
                //                        Statistics.SortOrder;
            }
            else
                if ((e.ColumnIndex == 1) && ((Sorting.ZagSort == SortDir.Desc) || (Sorting.ZagSort == SortDir.None)))
                {
                    Formular.Sort(Formular.Columns[2], ListSortDirection.Ascending);
                    Formular.Columns[1].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                    Sorting.ZagSort = SortDir.Asc;
                    if ((e.ColumnIndex == 3) && ((Sorting.AuthorSort == SortDir.Asc)))
                    {
                        Formular.Sort(Formular.Columns[4], ListSortDirection.Descending);
                        Formular.Columns[3].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;
                        Sorting.AuthorSort = SortDir.Desc;
                    }
                    else
                        if ((e.ColumnIndex == 3) && ((Sorting.AuthorSort == SortDir.Desc) || (Sorting.AuthorSort == SortDir.None)))
                        {
                            Formular.Sort(Formular.Columns[4], ListSortDirection.Ascending);
                            Formular.Columns[3].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
                            Sorting.AuthorSort = SortDir.Asc;
                        }
                }
            autoinc(Formular);
            foreach (DataGridViewRow row in Formular.Rows)
            {
                row.Cells["peny"].Value = CalculatePeny(row).ToString() + " �.";
                //row.Cells["pen"].ReadOnly = true;
                DataGridViewDisableButtonCell bc = (DataGridViewDisableButtonCell)row.Cells["but"];



                if ((row.Cells["pen"].Value.ToString().ToLower() == "false") && (row.Cells["zkid"].Value.ToString() != "0") && (bool.Parse(row.Cells["rempen"].Value.ToString()) == true))
                {
                    bc.Value = "��� ���������";//����� �����
                    bc.Enabled = false;
                }
                else
                    if ((row.Cells["pen"].Value.ToString().ToLower() == "false") && (row.Cells["rempen"].Value.ToString().ToLower() == "false"))
                    {
                        bc.Value = "��������";
                        bc.Enabled = true;
                        //row.Cells["pen"].ReadOnly = true;
                    }
                    else
                        if ((row.Cells["pen"].Value.ToString().ToLower() == "true") && (row.Cells["rempen"].Value.ToString().ToLower() == "false") && (row.Cells["zkid"].Value.ToString() != "0"))
                        {
                            bc.Value = "��������";//����� ��� �� ����������
                            bc.Enabled = true;

                        }
                        else
                            if ((row.Cells["pen"].Value.ToString().ToLower() == "true") && (row.Cells["rempen"].Value.ToString().ToLower() == "false") && (row.Cells["zkid"].Value.ToString() == "0"))
                            {
                                bc.Value = "����� ���������";//����� ����������, �� � ���������� �����
                                bc.Enabled = true;

                            }
                            else
                                if ((row.Cells["pen"].Value.ToString().ToLower() == "true") && (row.Cells["rempen"].Value.ToString().ToLower() == "true"))
                                {
                                    bc.Value = "��� ���������";//������ �� ���� �� ������ ����, ���� ���� ��������� ���������� ��������� � �������� � ��.
                                    bc.Enabled = false;
                                    MessageBox.Show("��������� ��������� ������������ ��������.������ ���� �� ������. ���������� � ������������.");
                                }

            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (label25.Text == string.Empty)
            {
                MessageBox.Show("�������� �� ������!");
                return;
            }
            if (Formular.Rows.Count == 0)
            {
                MessageBox.Show("�� ��������� �� �������� �� ���� �� ���������!");
                return;
            }
            LostBook lb = new LostBook(label25.Text, this, Formular);
            lb.ShowDialog();
            FormularColumnsForming(ReaderRecordFormular.id);
            
        }
        System.Drawing.Printing.PrintDocument pd;
        DataGridViewPrinter prin;
        DataGridView dgw2;
        private bool SetupThePrinting()
        {
            PrintDialog MyPrintDialog = new PrintDialog();
            MyPrintDialog.AllowCurrentPage = false;
            MyPrintDialog.AllowPrintToFile = false;
            MyPrintDialog.AllowSelection = false;
            MyPrintDialog.AllowSomePages = false;
            MyPrintDialog.PrintToFile = false;
            MyPrintDialog.ShowHelp = false;
            MyPrintDialog.ShowNetwork = false;

            if (MyPrintDialog.ShowDialog() != DialogResult.OK)
                return false;
            pd = new System.Drawing.Printing.PrintDocument();
            pd.DocumentName = "������ �����";
            //pd.PrinterSettings = MyPrintDialog.PrinterSettings;
            pd.DefaultPageSettings = pd.PrinterSettings.DefaultPageSettings;
            pd.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(20, 20, 20, 20);
            pd.DefaultPageSettings.Landscape = true;
            pd.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(pd_PrintPage);
            prin = new DataGridViewPrinter(dgw2, pd, true, false, string.Empty, new Font("Tahoma", 18), Color.Black, false);
            

            return true;
        }

        void pd_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            bool more = prin.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }
        class Span
        {
            public DateTime start;
            public DateTime end;
        }
        Span MyDateSpan;
        private void button12_Click(object sender, EventArgs e)
        {
            if (Statistics.Columns.Count == 3)
            {
                PrintDialog MyPrintDialog = new PrintDialog();
                MyPrintDialog.AllowCurrentPage = false;
                MyPrintDialog.AllowPrintToFile = false;
                MyPrintDialog.AllowSelection = false;
                MyPrintDialog.AllowSomePages = false;
                MyPrintDialog.PrintToFile = false;
                MyPrintDialog.ShowHelp = false;
                MyPrintDialog.ShowNetwork = false;
                if (MyPrintDialog.ShowDialog() != DialogResult.OK)
                    return;
                pd = new System.Drawing.Printing.PrintDocument();
                pd.DocumentName = "������ �����";
                //pd.PrinterSettings = MyPrintDialog.PrinterSettings;
                pd.DefaultPageSettings = pd.PrinterSettings.DefaultPageSettings;
                pd.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(20, 20, 20, 20);
                pd.DefaultPageSettings.Landscape = false;
                pd.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(pd_PrintPage);
                string tit =  "���������� ����������/��������� \n�� ������ � "+MyDateSpan.start.ToShortDateString() + " �� " + MyDateSpan.end.ToShortDateString() + ".";
                prin = new DataGridViewPrinter(Statistics, pd, true, true, tit, new Font("Tahoma", 18), Color.Black, false);
                pd.Print();
            }
            else
            {
                //����������� ����������
                ListSortDirection SO = ListSortDirection.Ascending;
                dgw2 = new DataGridView();
                //DataGridViewRow[] arr = new DataGridViewRow[Statistics.SelectedRows.Count];
                DataGridViewColumn[] arr1 = new DataGridViewColumn[Statistics.Columns.Count];
                //Statistics.SelectedRows.CopyTo(arr, 0);
                Statistics.Columns.CopyTo(arr1, 0);
                dgw2.AutoGenerateColumns = false;
                dgw2.Columns.Clear();
                dgw2.Font = new Font("Times New Roman", 10);
                //dgw2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                //dgw2.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
                //dgw2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dgw2.AllowUserToAddRows = false;
                //dgw2.Columns.AddRange(arr1);
                foreach (DataGridViewColumn col in arr1)
                {
                    //dgw2.Columns.Add((DataGridViewColumn)col.Clone());
                    dgw2.Columns.Add("","");
                }
                int i = 0;
                for (int ri = 0; ri < Statistics.Rows.Count; ri++)
                {
                    
                    dgw2.Rows.Add();// (DataGridViewRow)Statistics.SelectedRows[ri].Clone();
                    DataGridViewRow clonedRow = dgw2.Rows[i];
                    for (Int32 index = 0; index < Statistics.Rows[ri].Cells.Count; index++)
                    {
                        dgw2.Rows[i].Cells[index].Value = Statistics.Rows[ri].Cells[index].Value;
                    }
                    //dgw2.Rows.Add(clonedRow);
                    i++;
                }
                dgw2.AutoSize = false;
                dgw2.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
                dgw2.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgw2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                dgw2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                //Statistics.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dgw2.Columns[0].Width = 35;
                dgw2.Columns[1].Width = 470;
                dgw2.Columns[2].Width = 215;
                dgw2.Columns[2].Visible = true;

                dgw2.Columns[3].Width = 70;
                dgw2.Columns[4].Visible = false;
                dgw2.Columns[5].Visible = false;
                dgw2.Columns[6].Visible = false;
                dgw2.Columns[7].Width = 70;
                dgw2.Columns[8].Width = 100;
                dgw2.Columns[9].Visible = false;
                dgw2.Columns[10].Width = 70;
                dgw2.Columns[11].Width = 70;
                dgw2.Columns[1].HeaderText = "��������";
                dgw2.Columns[2].HeaderText = "�����";
                dgw2.Columns[3].HeaderText = "���. �����";
                dgw2.Columns[4].HeaderText = "��������������";
                dgw2.Columns[7].HeaderText = "����� ������";
                dgw2.Columns[8].HeaderText = "���";
                dgw2.Columns[10].HeaderText = "���� ������";
                dgw2.Columns[10].ValueType = typeof(DateTime);
                dgw2.Columns[11].HeaderText = "���� ��������";
                if (Statistics.SortedColumn != null)
                {
                    if (Statistics.Columns[Statistics.SortedColumn.Index].HeaderCell.SortGlyphDirection == System.Windows.Forms.SortOrder.Ascending)
                    {
                        SO = ListSortDirection.Ascending;
                    }
                    if (Statistics.Columns[Statistics.SortedColumn.Index].HeaderCell.SortGlyphDirection == System.Windows.Forms.SortOrder.Descending)
                    {
                        SO = ListSortDirection.Descending;
                    }
                    dgw2.Sort(dgw2.Columns[Statistics.SortedColumn.Index], SO);
                }
                //dgw2
                autoinc(dgw2);
                if (SetupThePrinting())
                    pd.Print();
            }
            
        
        }

        private void button13_Click(object sender, EventArgs e)
        {
#region old
            /*if (label25.Text == "")
            {
                MessageBox.Show("�������� �� ������! ������� �������� ��������!");
                return;
            }
            DBWork.dbReader reader = new DBWork.dbReader(int.Parse(label25.Text));
            
            Conn.ReaderDA.SelectCommand.CommandText = "select * from Main where";

            //DataSet ds = dbw.GetFormular("149921");
            //int i = ds.Tables.Count;
            //CrystalReport11.SetDataSource(dbw.GetFormular("149921"));
            Conn.SQLDA.SelectCommand.Parameters["@IDR"].Value = reader.id;
            Conn.SQLDA.SelectCommand.CommandText = " select " +
            "max(case when tmp.mnf = 200 then pl end) as Zag, " +
            "max(case when tmp.mnf = 200 then srt end) as zag_sort, " +
            "max(case when tmp.mnf = 700 then pl end) as avt, " +
            "max(case when tmp.mnf = 700 then srt end) as avt_sort, " +
            "max(case when tmp.mnf = 2100 then pl end) as god, " +
            "max(case when tmp.mnf = 200 then pl end) as mesto, " +
            "max(case when tmp.mnf = 200 then idm end) as idmain, " +
            "max(case when tmp.mnf = 200 then iss end) as issue, " +
            "max(case when tmp.mnf = 200 then vozv end) as vozv, " +
            "max(case when tmp.mnf = 200 then fct end) as fact, " +
            "max(case when tmp.mnf = 200 then zakid end) as zkid, " +
            "max(case when tmp.mnf = 200 then zi end) as zid, " +
            "((case when (tmp.pnlt = 'false' or tmp.pnlt is NULL) then 'false' else 'true' end)) as penalty, " +
            "((case when (tmp.rempnlt = 'false' or tmp.rempnlt is NULL) then 'false' else 'true' end)) as rempenalty " +
            "from " +
            "(select Z.ID as zi,Z.IDMAIN as zakid, Z.DATE_ISSUE as iss, Z.DATE_VOZV as vozv, Z.DATE_FACT_VOZV as fct, Z.PENALTY as pnlt,Z.REMPENALTY as rempnlt, X.IDMAIN as idm, X.PLAIN as pl, Y.SORT as srt, Y.MNFIELD as mnf " +
            "from BJVVV..DATAEXTPLAIN X " +
            "join BJVVV..DATAEXT Y on Y.ID=X.IDDATAEXT " +
            "join Reservation_R..ISSUED Z on ((Z.IDMAIN = Y.IDMAIN) or (Z.IDMAIN_CONST=Y.IDMAIN and Z.PENALTY='true')) " +
                //"--join Reservation_R..ISSUED ZZ on Z.IDMAIN = ZZ.IDMAIN_CONST "+
            "where (((Y.MNFIELD = 200 and Y.MSFIELD = '$a') " +
            "or (Y.MSFIELD = '$a' and Y.MNFIELD = 700) " +
            "or (Y.MSFIELD = '$d' and Y.MNFIELD = 2100) " +
            "or (Y.MSFIELD = '$a' and Y.MNFIELD = 210)) and (Z.IDREADER = @IDR) and (( ((Z.IDMAIN!=0)and(Z.REMPENALTY = 'false')and (Z.PENALTY='true')) or ((Z.IDMAIN=0)and(Z.PENALTY='true')) or ((Z.IDMAIN!=0)and(Z.REMPENALTY = 'false')and (Z.PENALTY='false'))  ) )) " +
            "group by Z.ID, Z.IDMAIN, X.PLAIN, Y.SORT, Y.MNFIELD, X.IDMAIN,Z.DATE_ISSUE,Z.DATE_VOZV, Z.DATE_FACT_VOZV,Z.PENALTY, Z.REMPENALTY " +
            ") as tmp " +
            "group by idm,pnlt,rempnlt ";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet R = new DataSet();
            R.Tables.Add("form");

            int i = Conn.SQLDA.Fill(R.Tables["form"]);
            CrystalReport1 cr1 = new CrystalReport1();
            cr1.SetDataSource(R.Tables["form"]);
            crystalReportViewer1.ReportSource = cr1;

            CrystalDecisions.CrystalReports.Engine.TextObject txtReaderName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtReaderNum;
            txtReaderName = cr1.ReportDefinition.ReportObjects["Text19"] as TextObject;
            txtReaderNum = cr1.ReportDefinition.ReportObjects["Text20"] as TextObject;

            txtReaderName.Text = reader.Surname + " " + reader.Name + " " + reader.SecondName;
            txtReaderNum.Text = reader.id;
            //crystalReportViewer1.PrintReport();
            cr1.PrintToPrinter(2, false, 1, 99999);*/
            #endregion
            if (label25.Text == "")
            {
                MessageBox.Show("�������� �� ������! ������� �������� ��������!");
                return;
            }
            DBWork.dbReader reader = new DBWork.dbReader(int.Parse(label25.Text));

            Conn.ReaderDA.SelectCommand.CommandText = "select * from Main where";

            //DataSet ds = dbw.GetFormular("149921");
            //int i = ds.Tables.Count;
            //CrystalReport11.SetDataSource(dbw.GetFormular("149921"));
            Conn.SQLDA.SelectCommand.Parameters["@IDR"].Value = reader.id;
            Conn.SQLDA.SelectCommand.CommandText = "select zagp.PLAIN zag,avtp.PLAIN avt, B.INV inv, B.DATE_ISSUE iss,B.DATE_VOZV vzv  " +
                                                   "  from BJVVV..DATAEXT A  " +
                                                   " inner join Reservation_R..ISSUED B on B.INV collate Cyrillic_General_CI_AI = A.SORT and A.MNFIELD = 899 and A.MSFIELD = '$p' " +
                                                   " left join BJVVV..DATAEXT zag on zag.MNFIELD = 200 and zag.MSFIELD = '$a' and zag.IDMAIN = A.IDMAIN " +
                                                   " left join BJVVV..DATAEXT avt on avt.MNFIELD = 700 and avt.MSFIELD = '$a' and avt.IDMAIN = A.IDMAIN " +
                                                   " left join BJVVV..DATAEXTPLAIN zagp on zagp.IDDATAEXT = zag.ID " +
                                                   " left join BJVVV..DATAEXTPLAIN avtp on avtp.IDDATAEXT = avt.ID " +
                                                   " where B.IDREADER = @IDR and B.IDMAIN != 0";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet R = new DataSet();
            R.Tables.Add("form");

            int i = Conn.SQLDA.Fill(R.Tables["form"]);
            CrystalReport1 cr1 = new CrystalReport1();
            cr1.SetDataSource(R.Tables["form"]);
            crystalReportViewer1.ReportSource = cr1;

            CrystalDecisions.CrystalReports.Engine.TextObject txtReaderName;
            CrystalDecisions.CrystalReports.Engine.TextObject txtReaderNum;
            txtReaderName = cr1.ReportDefinition.ReportObjects["Text19"] as TextObject;
            txtReaderNum = cr1.ReportDefinition.ReportObjects["Text20"] as TextObject;

            txtReaderName.Text = reader.Surname + " " + reader.Name + " " + reader.SecondName;
            txtReaderNum.Text = reader.id;
            //crystalReportViewer1.PrintReport();
            cr1.PrintToPrinter(1, false, 1, 99999);


        }

        private void reportDocument1_InitReport(object sender, EventArgs e)
        {

        }

        private void crystalReport11_InitReport(object sender, EventArgs e)
        {

        }

        private void CrystalReport11_InitReport_1(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = dbw.GetAllBooks();
            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RndPrg.Dispose();
            Statistics.DataSource = e.Result;
            autoinc(Statistics);
            Statistics.Columns[0].Width = 50;
            Statistics.Columns[1].HeaderText = "����� �����";
            Statistics.Columns[1].Width = 140;
            Statistics.Columns[2].HeaderText = "��������";
            Statistics.Columns[2].Visible = false;
            Statistics.Columns[3].HeaderText = "��������";
            Statistics.Columns[3].Width = 330;
            Statistics.Columns[4].HeaderText = "�����";
            Statistics.Columns[4].Width = 150;
            Statistics.Columns[5].HeaderText = "��� �������";
            Statistics.Columns[5].Width = 70;
            Statistics.Columns[6].HeaderText = "�������� ������";
            Statistics.Columns[6].Width = 80;
            Statistics.Columns[7].Visible = false;
            Statistics.Columns[8].Visible = false;
            Statistics.Columns[8].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            Statistics.Columns[9].HeaderText = "������";
            Statistics.Columns[9].Width = 100;
            Sorting.WhatStat = Stats.AllBooks;
            Sorting.AuthorSort = SortDir.None;
            Sorting.ZagSort = SortDir.None;
            //Statistics.
            //Statistics.Columns[0].SortMode = DataGridViewColumnSortMode.Programmatic;
            //Statistics.Columns[2].SortMode = DataGridViewColumnSortMode.;
            button12.Enabled = true;
           // backgroundWorker2.CancelAsync();
        }



        delegate void pbrun();

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            
            /*Action dlg = delegate()
            {
                if (progressBar1.Value == 100)
                    progressBar1.Value = 0;
                progressBar1.Value += 1;
            };
            while (backgroundWorker1.IsBusy)
            {
                //Thread.CurrentThread.Join(500);
                Thread.Sleep(500);
                this.Invoke(dlg);
            }*/
            Action delegProgress = delegate()
            {
                
                RndPrg = new ExtGui.RoundProgress();
                RndPrg.Visible = true;
                RndPrg.Name = "progress";
                tabControl1.TabPages[1].Controls.Add(RndPrg);
                RndPrg.BringToFront();
                RndPrg.Size = new Size(40, 60);
                RndPrg.Location = new Point(450, 200);
                RndPrg.BackColor = SystemColors.AppWorkspace;
            };
            this.Invoke(delegProgress);
            
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           // progressBar1.Value = e.ProgressPercentage;
           // if (progressBar1.Value == 100)
           //     progressBar1.Value = 0;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            tabControl1.TabPages.RemoveAt(1);
        }
        public string emul;
        public string pass;
        private void button14_Click(object sender, EventArgs e)
        {
            Form20 f20 = new Form20(this);
            f20.ShowDialog();
            if (pass == "adminn")
            {
                pass = "";
                Form19 f19 = new Form19(this);
                f19.ShowDialog();
                Form1_Scanned(sc, new EventArgs());
            }

            /*SqlCommand cmd = new SqlCommand("[Reservation_R]..[updbooks]", Conn.ZakazCon);
            Conn.ZakazCon.Open();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@IDR", SqlDbType.Int);
            cmd.Parameters.Add("@IDROLD", SqlDbType.NVarChar);
            cmd.Parameters["@IDR"].Value = 23;// reader.id;
            cmd.Parameters["@IDROLD"].Value = "A/15720";// idrold;

            cmd.ExecuteNonQuery();*/

        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (label25.Text == "")
            {
                MessageBox.Show("������� ����� ��� �������� �������� ��������!");
                return;
            }

            Form7 f7 = new Form7(ReaderRecordFormular);
            f7.ShowDialog();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (label25.Text == "")
            {
                MessageBox.Show("������� ����� ��� �������� �������� ��������!");
                return;
            }


            Form8 f8 = new Form8(ReaderRecordFormular);
            f8.ShowDialog();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (label25.Text == "")
            {
                MessageBox.Show("������� ����� ��� �������� �������� ��������!");
                return;
            }
            Form9 f9 = new Form9(ReaderRecordFormular,this);
            f9.ShowDialog();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Form13 f13 = new Form13();
            f13.ShowDialog();
        }


        private void button21_Click(object sender, EventArgs e)
        {
            //����� �������� �� �������
            Form16 f16 = new Form16(this);
            f16.ShowDialog();
        }
        public void FrmlrFam(string id)
        {
            ReaderRecordFormular = new DBWork.dbReader(int.Parse(id));

            dbw.GetFormular(ReaderRecordFormular.id);
            ReaderSetBarcode = new DBWork.dbReader(ReaderRecordFormular);
            label20.Text = ReaderRecordFormular.Surname + " " + ReaderRecordFormular.Name + " " + ReaderRecordFormular.SecondName;
            textBox6.Text = ReaderRecordFormular.AbonType;
            label25.Text = ReaderRecordFormular.id;
            FormularColumnsForming(ReaderRecordFormular.id);

        }

        private void button22_Click(object sender, EventArgs e)
        {
            //������ ����� �� ������
            if (label5.Text != "")
            {
                MessageBox.Show("�������� ��� ���������������! ����������� ������!", "��������", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (label9.Text == "")
            {
                MessageBox.Show("������� �������� �������� � �����!","��������",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
            Form17 f17 = new Form17(this);
            f17.ShowDialog();

        }

        private void button23_Click(object sender, EventArgs e)
        {
            //����� ���������
            Form18 f18 = new Form18();
            f18.ShowDialog();
            
        }

        private void �������������������������������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //��������������
            Form21 f21 = new Form21();
            f21.ShowDialog();
        }

        private void �ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form22 f22 = new Form22();
            f22.ShowDialog();
        }
        private void SetAllPenalty()
        {
            Conn.SQLDA.UpdateCommand = new SqlCommand();
            Conn.SQLDA.UpdateCommand.Connection = Conn.BJVVVConn;
            if (Conn.SQLDA.UpdateCommand.Connection.State == ConnectionState.Closed)
            {
                Conn.SQLDA.UpdateCommand.Connection.Open();
            }
            Conn.SQLDA.UpdateCommand.CommandText = "update Reservation_R..ISSUED set PENALTY = 'true' where getdate() > DATE_VOZV and IDMAIN != 0 and DATE_FACT_VOZV is null and PENALTY = 'false' and REMPENALTY = 'false' ";
            Conn.SQLDA.UpdateCommand.ExecuteNonQuery();
            Conn.SQLDA.UpdateCommand.Connection.Close();
        }
        private void toolStripMenuItem3_Click_1(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.ShowDialog();
                
            Statistics.Columns.Clear();
            Statistics.Columns.Add("NN", "� �/�");
            label19.Text = "������ ����������� ������ �����������  �� " + DateTime.Now.ToShortDateString() + " :";
            label18.Text = "";
            //SetAllPenalty();
            try
            {
                Statistics.DataSource = dbw.GetDebtors(f3.StartDate, f3.EndDate);//StatDS.Tables[0];
            }
            catch (IndexOutOfRangeException ev)
            {
                string s = ev.Message;
                MessageBox.Show("����������� ���!", "����������.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Statistics.Columns.Clear();
                return;
            }
            autoinc(Statistics);
            Statistics.Columns[1].HeaderText = "���� ��������";
            Statistics.Columns[2].HeaderText = "����� ������";
            //Statistics.Columns[2].ValueType = typeof(int);
            Statistics.Columns[3].HeaderText = "�������";
            Statistics.Columns[3].Width = 100;
            Statistics.Columns[4].HeaderText = "���";
            Statistics.Columns[4].Width = 90;
            Statistics.Columns[5].HeaderText = "��������";
            Statistics.Columns[5].Width = 110;
            Statistics.Columns[6].HeaderText = "��������";
            Statistics.Columns[6].Width = 170;
            Statistics.Columns[7].HeaderText = "�����";
            Statistics.Columns[7].Width = 130;
            Statistics.Columns[8].Visible = false;
            Statistics.Columns[9].Visible = false;
            Statistics.Columns[10].Visible = false;
            Sorting.WhatStat = Stats.Debtors;
            Sorting.AuthorSort = SortDir.None;
            Sorting.ZagSort = SortDir.None;
            foreach (DataGridViewRow r in Statistics.Rows)
            {
                if (r.Cells[10].Value.ToString() == "true")
                {
                    r.DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }
            //Statistics.Columns[10].Visible = false;

            //DataGridViewColumn col = new DataGridViewColumn();
            //col.HeaderText = "�/�";

            //Statistics.Columns

        }

        private void button24_Click(object sender, EventArgs e)
        {
          
        }

        private void yfqnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //����� ����� �� ������������ ������
            Form15 f15 = new Form15();
            f15.ShowDialog();
        }

        private void ������������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Statistics.Columns.Clear();
            Statistics.Columns.Add("NN", "� �/�");
            Statistics.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            Statistics.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            Statistics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            Form3 f3 = new Form3();
            f3.ShowDialog();
            label19.Text = "������ �������� ���������� c " + f3.StartDate.ToShortDateString() + " �� " + f3.EndDate.ToShortDateString() + " :";
            label18.Text = "";
            Statistics.DataSource = dbw.GetIssuedBooks(f3.StartDate, f3.EndDate); //StatDS.Tables[0];
            if (this.Statistics.Rows.Count == 0)
            {
                this.Statistics.Columns.Clear();
                MessageBox.Show("��� �������� ����!");
                return;
            }

            autoinc(Statistics);
            Statistics.Columns[0].Width = 40;
            Statistics.Columns[1].HeaderText = "��������";
            Statistics.Columns[1].Width = 280;
            Statistics.Columns[2].HeaderText = "�����";
            Statistics.Columns[2].Width = 150;
            Statistics.Columns[9].Visible = false;
            Statistics.Columns[4].HeaderText = "��������������";
            Statistics.Columns[4].Width = 150;
            Statistics.Columns[4].Visible = false;
            Statistics.Columns[5].Visible = false;
            Statistics.Columns[6].Visible = false;
            Statistics.Columns[7].HeaderText = "����� ������ ������� ������";
            Statistics.Columns[7].Width = 70;
            Statistics.Columns[8].HeaderText = "���";
            Statistics.Columns[8].Width = 100;
            Statistics.Columns[3].HeaderText = "���. �����";
            Statistics.Columns[3].Width = 100;
            Statistics.Columns[10].HeaderText = "���� ������";
            Statistics.Columns[10].ValueType = typeof(DateTime);
            Statistics.Columns[10].Width = 85;
            Statistics.Columns[11].HeaderText = "������ �������� ���� ��������";
            Statistics.Columns[11].Width = 85;

            Sorting.WhatStat = Stats.IssuedBooks;
            Sorting.AuthorSort = SortDir.None;
            Sorting.ZagSort = SortDir.None;
            button12.Enabled = true;
        }

        private void Statistics_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (label19.Text.IndexOf("������ ����������� ������ ����������� ���������� �� ����� ����������, �������") != -1)
            {
                if (Statistics.Rows[e.RowIndex].Cells[2].Value.ToString().Contains("��������"))
                {
                    MessageBox.Show("���������� ���������� ��������.");
                    return;
                }
                tabControl1.SelectedIndex = 1;
                numericUpDown3.Value = int.Parse(Statistics.Rows[e.RowIndex].Cells[2].Value.ToString());
                button10_Click(sender, new EventArgs());
            }
            else
                if (label19.Text.IndexOf("������ ����������� ������ �����������") != -1)
                {
                    int res;
                    if (Statistics.Rows[e.RowIndex].Cells[2].Value.ToString().Contains("��������"))
                    {
                        MessageBox.Show("���������� ���������� ��������.");
                        return;
                    }
                    if (!int.TryParse(Statistics.Rows[e.RowIndex].Cells[2].Value.ToString(), out res))
                    {
                        MessageBox.Show("�������� �� ������ ���� �� ��������� � ������������ � �����! �������� �� ����� ���� ������ ����� ������������� ������ ��� �����!");
                        return;
                    }
                    tabControl1.SelectedIndex = 1;
                    numericUpDown3.Value = res;//int.Parse(Statistics.Rows[e.RowIndex].Cells[2].Value.ToString());
                    button10_Click(sender, new EventArgs());
                }
                else
                {
                    return;
                }
        }

        private void �������������������������������������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.ShowDialog();
            Conn.SQLDA.SelectCommand.CommandText = "select IDREADER,DATE_FACT_VOZV from Reservation_R..ISSUED " +
                                                   " where DATE_FACT_VOZV between '" + f3.StartDate.ToString("yyyyMMdd") + "' and '" + f3.EndDate.ToString("yyyyMMdd") + "' " +
                                                   " group by DATE_FACT_VOZV,IDREADER";
            DataSet DS = new DataSet();
            int i = Conn.SQLDA.Fill(DS, "t");
            
            MessageBox.Show("���������� ���������, ��������� ���������� �� ��������� ������, ��������� " + i.ToString(),"����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ��������������������������������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.ShowDialog();
            Conn.SQLDA.SelectCommand.CommandText = "select IDREADER,DATE_ISSUE from Reservation_R..ISSUED " +
                                                   " where DATE_ISSUE between '" + f3.StartDate.ToString("yyyyMMdd") + "' and '" + f3.EndDate.ToString("yyyyMMdd") + "' " +
                                                   " group by DATE_ISSUE,IDREADER";
            DataSet DS = new DataSet();
            int i = Conn.SQLDA.Fill(DS, "t");

            MessageBox.Show("���������� ���������, ������� ���������� �� ��������� ������, ��������� " + i.ToString(), "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ����������������������������������������������������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.ShowDialog();
            Conn.SQLDA.SelectCommand.CommandText = "select IDREADER,DATE_PROLONG from Reservation_R..ISSUED " +
                                                   " where DATE_PROLONG between '" + f3.StartDate.ToString("yyyyMMdd") + "' and '" + f3.EndDate.ToString("yyyyMMdd") + "' " +
                                                   " group by DATE_PROLONG,IDREADER";
            DataSet DS = new DataSet();
            int i = Conn.SQLDA.Fill(DS, "t");

            MessageBox.Show("���������� ���������, ���������� ���� ������������� ���������� �� ��������� ������, ��������� " + i.ToString(), "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void ����������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.ShowDialog();
            Conn.SQLDA.SelectCommand.CommandText = "with A as ( " +
                                                    "select IDREADER,DATE_ISSUE  " +
                                                    "from Reservation_R..ISSUED   " +
                                                    "where DATE_ISSUE between '"+f3.StartDate.ToString("yyyyMMdd")+"' and '"+f3.EndDate.ToString("yyyyMMdd")+"' " +
                                                    "group by DATE_ISSUE,IDREADER " +
                                                    "), " +
                                                    "B as ( " +
                                                    "select IDREADER,DATE_FACT_VOZV  " +
                                                    "from Reservation_R..ISSUED   " +
                                                    "where DATE_FACT_VOZV between '" + f3.StartDate.ToString("yyyyMMdd") + "' and '" + f3.EndDate.ToString("yyyyMMdd") + "'   " +
                                                    "group by DATE_FACT_VOZV,IDREADER " +
                                                    "), " +
                                                    "C as ( " +
                                                    "select IDREADER,DATE_PROLONG  " +
                                                    "from Reservation_R..ISSUED   " +
                                                    "where DATE_PROLONG between '" + f3.StartDate.ToString("yyyyMMdd") + "' and '" + f3.EndDate.ToString("yyyyMMdd") + "'   " +
                                                    "group by DATE_PROLONG,IDREADER " +
                                                    ") " +
                                                    "select * from A " +
                                                    "union " +
                                                    "select * from B " +
                                                    "union " +
                                                    "select * from C";

            DataSet DS = new DataSet();
            int i = Conn.SQLDA.Fill(DS, "t");

            MessageBox.Show("���������� ����������� ��������� �� ��������� ������, ��������� " + i.ToString(), "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void ������������������������������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.ShowDialog();

            Statistics.Columns.Clear();
            Statistics.Columns.Add("NN", "� �/�");
            label19.Text = "������ ����������� ������ ����������� ���������� �� ����� ����������, ������� ����������, �� " + DateTime.Now.ToShortDateString() + " :";
            label18.Text = "";
            //DataSet StatDS = dbw.GetDebtors();
            try
            {
                Statistics.DataSource = dbw.GetDebtorsFCT(f3.StartDate, f3.EndDate);//StatDS.Tables[0];
            }
            catch (IndexOutOfRangeException ev)
            {
                string s = ev.Message;
                MessageBox.Show("����������� ���!", "����������.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Statistics.Columns.Clear();
                return;
            }
            autoinc(Statistics);
            Statistics.Columns[1].HeaderText = "���� ��������";
            Statistics.Columns[2].HeaderText = "����� ������";
            Statistics.Columns[3].HeaderText = "�������";
            Statistics.Columns[3].Width = 100;
            Statistics.Columns[4].HeaderText = "���";
            Statistics.Columns[4].Width = 90;
            Statistics.Columns[5].HeaderText = "��������";
            Statistics.Columns[5].Width = 110;
            Statistics.Columns[6].HeaderText = "��������";
            Statistics.Columns[6].Width = 170;
            Statistics.Columns[7].HeaderText = "�����";
            Statistics.Columns[7].Width = 130;
            Statistics.Columns[8].Visible = false;
            Statistics.Columns[9].Visible = false;
            Statistics.Columns[10].Visible = false;
            Sorting.WhatStat = Stats.Debtors;
            Sorting.AuthorSort = SortDir.None;
            Sorting.ZagSort = SortDir.None;
            foreach (DataGridViewRow r in Statistics.Rows)
            {
                if (r.Cells[10].Value.ToString() == "true")
                {
                    r.DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }
            //DataGridViewColumn col = new DataGridViewColumn();
            //col.HeaderText = "�/�";

            //Statistics.Columns

        }

        private void �������������������������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Statistics.Columns != null)
                Statistics.Columns.Clear();
            Form3 f3 = new Form3();
            f3.ShowDialog();
            label18.Text = "";
            label19.Text = "";

            //label19.Text = "���������� �������� ����������, �� ������ �" + f3.StartDate.ToString("yyyyMMdd") + " �� " + f3.EndDate.ToString("yyyyMMdd") + ": ";
            string cntISSBOOK = dbw.GetBooksCount(f3.StartDate, f3.EndDate);
            MyDateSpan = new Span();
            MyDateSpan.start = f3.StartDate;
            MyDateSpan.end = f3.EndDate;
            //���������� ��������� ��������� ����������
            Conn.SQLDA.SelectCommand.CommandText = "select IDREADER,DATE_FACT_VOZV from Reservation_R..ISSUED " +
                                                   " where DATE_FACT_VOZV between '" + f3.StartDate.ToString("yyyyMMdd") + "' and '" + f3.EndDate.ToString("yyyyMMdd") + "' " +
                                                   " group by DATE_FACT_VOZV,IDREADER";
            DataSet DS = new DataSet();
            string cntRETRDR = Conn.SQLDA.Fill(DS, "t").ToString();

            //���������� ��������� ������� ����������(������ ����������)
            Conn.SQLDA.SelectCommand.CommandText = "select IDREADER,DATE_ISSUE from Reservation_R..ISSUED " +
                                                   " where DATE_ISSUE between '" + f3.StartDate.ToString("yyyyMMdd") + "' and '" + f3.EndDate.ToString("yyyyMMdd") + "' " +
                                                   " group by DATE_ISSUE,IDREADER";
            DS = new DataSet();
            string cntISSRDR = Conn.SQLDA.Fill(DS, "t").ToString();



            //���������� ��������� ���������� ���� ����������� ����������
            Conn.SQLDA.SelectCommand.CommandText = "select IDREADER,DATE_PROLONG from Reservation_R..ISSUED " +
                                                   " where DATE_PROLONG between '" + f3.StartDate.ToString("yyyyMMdd") + "' and '" + f3.EndDate.ToString("yyyyMMdd") + "' " +
                                                   " group by DATE_PROLONG,IDREADER";
            DS = new DataSet();
            string cntPROLONGRDR = Conn.SQLDA.Fill(DS, "t").ToString();

            //���������� ����������� ���������
            Conn.SQLDA.SelectCommand.CommandText = "with A as ( " +
                                                    "select IDREADER,DATE_ISSUE  " +
                                                    "from Reservation_R..ISSUED   " +
                                                    "where DATE_ISSUE between '" + f3.StartDate.ToString("yyyyMMdd") + "' and '" + f3.EndDate.ToString("yyyyMMdd") + "' " +
                                                    "group by DATE_ISSUE,IDREADER " +
                                                    "), " +
                                                    "B as ( " +
                                                    "select IDREADER,DATE_FACT_VOZV  " +
                                                    "from Reservation_R..ISSUED   " +
                                                    "where DATE_FACT_VOZV between '" + f3.StartDate.ToString("yyyyMMdd") + "' and '" + f3.EndDate.ToString("yyyyMMdd") + "'   " +
                                                    "group by DATE_FACT_VOZV,IDREADER " +
                                                    "), " +
                                                    "C as ( " +
                                                    "select IDREADER,DATE_PROLONG  " +
                                                    "from Reservation_R..ISSUED   " +
                                                    "where DATE_PROLONG between '" + f3.StartDate.ToString("yyyyMMdd") + "' and '" + f3.EndDate.ToString("yyyyMMdd") + "'   " +
                                                    "group by DATE_PROLONG,IDREADER " +
                                                    ") " +
                                                    "select * from A " +
                                                    "union " +
                                                    "select * from B " +
                                                    "union " +
                                                    "select * from C";

            DS = new DataSet();
            string cntSRVRDR = Conn.SQLDA.Fill(DS, "t").ToString();
            Statistics.Columns.Clear();
            //Statistics.Rows.Clear();
            Statistics.DataSource = null;
            Statistics.Columns.Add("NN", "��");
            Statistics.Columns.Add("spravka", "�������");
            Statistics.Columns[1].Width = 500;
            Statistics.Columns.Add("kolvo", "����������");
            string[] row = { "1", "������ ����������", cntISSBOOK };
            Statistics.Rows.Add(row);
            row = new string[] { "2", "���������� ��������� ��������� ����������", cntRETRDR };
            Statistics.Rows.Add(row);
            row = new string[] { "3", "���������� ��������� ������� ���������� (������ ����������)", cntISSRDR };
            Statistics.Rows.Add(row);
            row = new string[] { "4", "���������� ��������� ���������� ���� ����������� ���������", cntPROLONGRDR };
            Statistics.Rows.Add(row);
            row = new string[] { "5", "���������� ����������� ���������", cntSRVRDR };
            Statistics.Rows.Add(row);
            button12.Enabled = true;

        }

        private void button20_Click(object sender, EventArgs e)
        {
            Form10 f10 = new Form10(new DBWork.dbReader("1004228"));
        }

        private void button20_Click_1(object sender, EventArgs e)
        {

        }

        private void ���������������������������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Statistics.Columns != null)
                Statistics.Columns.Clear();
            Form3Act f3 = new Form3Act(this);
            f3.ShowDialog();
            label18.Text = "";
            label19.Text = "";
            label19.Text = "������ �������� ��������� �� ������ � " + f3.StartDate.ToString("dd.MM.yyyy") + " �� " + f3.EndDate.ToString("dd.MM.yyyy") + ": ";
            try
            {
                Statistics.DataSource = dbw.GetActions(f3.StartDate,f3.EndDate,f3.UserID);//StatDS.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Statistics.Columns.Clear();
                return;
            }
            autoinc(Statistics);
            Statistics.Columns[0].HeaderText = "��";
            Statistics.Columns[1].HeaderText = "��������";
            Statistics.Columns[2].HeaderText = "���";
            Statistics.Columns[2].Width = 400;
            Statistics.Columns[3].HeaderText = "����";
            Statistics.Columns[3].Width = 70;
            Statistics.Columns[4].HeaderText = "����";
            Statistics.Columns[4].Width = 80;
            autoinc(Statistics);
        }


    }
    public class _BarcScan
    {
        //public event EventHandler Scanned;
        public Form1 F1;
        public _BarcScan(Form1 _F1)
        {

            F1 = _F1;
            //SqlConnectionStringBuilder builder =
            //new SqlConnectionStringBuilder(();
            //CrystalReport1 cr = new CrystalReport1();
            //cr.SetDataSource(
            try
            {
                //this.Scanner = new OPOSScannerClass();
                //this.Scanner.ErrorEvent += new _IOPOSScannerEvents_ErrorEventEventHandler(Scanner_ErrorEvent);
                //this.Scanner.DataEvent += new _IOPOSScannerEvents_DataEventEventHandler(Scanner_DataEvent);
                //this.Scanner.Open("STI_SCANNER");
                ////MessageBox.Show("1");
                //ResultCodeH.Check(this.Scanner.ClaimDevice(7000));
                ////MessageBox.Show("2");
                //this.Scanner.DeviceEnabled = true;
                //ResultCodeH.Check(this.Scanner.ResultCode);
                ////MessageBox.Show("3");
                //this.Scanner.AutoDisable = true;

                //ResultCodeH.Check(this.Scanner.ResultCode);
                ////MessageBox.Show("4");
                //this.Scanner.DataEventEnabled = true;
                //ResultCodeH.Check(this.Scanner.ResultCode);
                //MessageBox.Show("5");
            }
            catch (Exception _e)
            {
                MessageBox.Show(_e.Message.ToString());
            }
        }
        void Scanner_DataEvent(int Status)
        {
            //this.Scanner.DeviceEnabled = true;
            //this.Scanner.DataEventEnabled = true;
            //F1.FireScan(this.Scanner, EventArgs.Empty);
        }

        void Scanner_ErrorEvent(int ResultCode, int ResultCodeExtended, int ErrorLocus, ref int pErrorResponse)
        {
            //pErrorResponse = (int)OPOS_Constants.OPOS_ER_CLEAR;
            //MessageBox.Show(ResultCodeH.Message(ResultCode));
            //this.Scanner.DeviceEnabled = true;
            //this.Scanner.DataEventEnabled = true;
        }
    }
    public static class Conn
    {
        public static SqlConnection ReadersCon;
        public static SqlConnection ZakazCon;
        public static SqlConnection BRIT_SOVETCon;
        public static SqlConnection BJVVVConn;
        public static SqlDataAdapter ReaderDA;
        public static SqlDataAdapter SQLDA;
    }
    public class DBWork
    {
        private DataSet ReaderMain;
        private DataSet Book;
        private DataSet Zakaz;
        Form1 F1;
        public DBWork()
        {
            XmlConnections xml = new XmlConnections();
            Conn.ReadersCon = new SqlConnection(xml.GetReaderCon());// ("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Z:\\progs\\Circulation\\Readers.mdb");
            Conn.BRIT_SOVETCon = new SqlConnection(xml.GetBRIT_SOVETCon());// ("Data Source=192.168.3.241;Initial Catalog=BRIT_SOVET;Integrated Security=True");
            Conn.BJVVVConn = new SqlConnection(xml.GetBJVVVCon());
            Conn.ZakazCon = new SqlConnection(xml.GetZakazCon());//("Data Source=192.168.3.241;Initial Catalog=TECHNOLOG;Integrated Security=True");
            Conn.ReaderDA = new SqlDataAdapter();
            Conn.ReaderDA.SelectCommand = new SqlCommand("select * from main where BarCode = 19", Conn.ReadersCon);
            Conn.ReaderDA.SelectCommand.Connection.Open();
            Conn.SQLDA = new SqlDataAdapter();
            Conn.SQLDA.SelectCommand = new SqlCommand("select * from BARCODE_UNITS where ID = 0", Conn.BRIT_SOVETCon);
            Conn.SQLDA.SelectCommand.Connection.Open();
            Conn.SQLDA.SelectCommand.Parameters.Add("@IDR", SqlDbType.NVarChar);
            Conn.SQLDA.SelectCommand.Parameters["@IDR"].Value = "0";

            Book = new DataSet();
            ReaderMain = new DataSet();
            Zakaz = new DataSet();
        }
        public DBWork(Form1 f1)
        {
            F1 = f1;
            XmlConnections xml = new XmlConnections();
            Conn.ReadersCon = new SqlConnection(xml.GetReaderCon());// ("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Z:\\progs\\Circulation\\Readers.mdb");
            Conn.BRIT_SOVETCon = new SqlConnection(xml.GetBRIT_SOVETCon());// ("Data Source=192.168.3.241;Initial Catalog=BRIT_SOVET;Integrated Security=True");
            Conn.ZakazCon = new SqlConnection(xml.GetZakazCon());//("Data Source=192.168.3.241;Initial Catalog=TECHNOLOG;Integrated Security=True");
            Conn.BJVVVConn = new SqlConnection(xml.GetBJVVVCon());
            Conn.ReaderDA = new SqlDataAdapter();
            Conn.ReaderDA.SelectCommand = new SqlCommand("select * from main where BarCode = 19", Conn.ReadersCon);
            Conn.ReaderDA.SelectCommand.Connection.Open();
            Conn.SQLDA = new SqlDataAdapter();
            Conn.SQLDA.SelectCommand = new SqlCommand("select * from BARCODE_UNITS where ID = 0", Conn.BRIT_SOVETCon);
            Conn.SQLDA.SelectCommand.Connection.Open();
            Conn.SQLDA.SelectCommand.Parameters.Add("@IDR", SqlDbType.NVarChar);
            Conn.SQLDA.SelectCommand.Parameters["@IDR"].Value = "0";
            Book = new DataSet();
            ReaderMain = new DataSet();
            Zakaz = new DataSet();
            //DR = new OleDbDataReader();
        }
        public DataSet getBooksForReader(string reader)
        {
            Conn.SQLDA.SelectCommand.CommandText = "WITH FC AS (SELECT dt.ID,dt.SORT, "+
                                                                        "dt.MNFIELD, " +
                                                                        "dt.MSFIELD, " +
                                                                        "dt.IDMAIN, " +
                                                                        "dtp.PLAIN " +

                                                                   "FROM   BJVVV..DATAEXT dt " +
                                                                          "JOIN BJVVV..DATAEXTPLAIN dtp " +
                                                                          "     ON  dt.ID = dtp.IDDATAEXT) "+

                                                    "select COL1.PLAIN zag,dtpa.PLAIN avt,Z.IDREADER,Z.IDMAIN,Z.INV inv from FC COL1 "+
                                                     "left join FC dtpa ON COL1.IDMAIN = dtpa.IDMAIN and dtpa.MNFIELD = 700 and dtpa.MSFIELD = '$a' "+
                                                     "left join Reservation_R..ISSUED Z on Z.IDMAIN = COL1.IDMAIN and Z.INV = COL1.PLAIN and COL1.MNFIELD = 899 and COL1.MSFIELD = '$p'"+
                                                     "where COL1.MNFIELD = 200 and COL1.MSFIELD = '$a' and Z.IDREADER =  " + reader +
                                                    "and Z.IDMAIN != 0";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            Book.Clear();
            int i = Conn.SQLDA.Fill(Book, "booksonreader");
            return (i == 0) ? new DataSet() : Book;
        }
        public dbBook getBookFromZAKAZ(string s)
        {
            //s = s.Remove(s.Length - 1, 1);
            Conn.SQLDA.SelectCommand.CommandText = "select * from Reservation_R..ISSUED where BAR = '" + s + "' and IDMAIN <> 0";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            Book.Clear();
            int i = Conn.SQLDA.Fill(Book, "zakbk");
            if (i != 0)
                return new dbBook(Book.Tables["zakbk"].Rows[0]["IDMAIN"].ToString(), Book.Tables["zakbk"].Rows[0]["BAR"].ToString(), "", Book.Tables["zakbk"].Rows[0]["IDREADER"].ToString(), Book.Tables["zakbk"].Rows[0]["INV"].ToString(), DateTime.Parse(Book.Tables["zakbk"].Rows[0]["DATE_VOZV"].ToString()), DateTime.Parse(Book.Tables["zakbk"].Rows[0]["DATE_FACT_VOZV"].ToString()));
            else
                return new dbBook();
        }

        public void setBookForReader(dbBook book, dbReader reader, int days)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select * from Reservation_R..ADVORDER where INV = '" + book.inv+"'";
            DataSet DS = new DataSet();
            int c = Conn.SQLDA.Fill(DS, "t");
            if (c != 0)
            {
                MessageBox.Show("���� ��������� ����� �� ���������� � �������� � ������� " + DS.Tables["t"].Rows[0]["IDREADER"].ToString() + "! ������� ���������� ����� ���������!");
                return;
            }

            Conn.SQLDA.InsertCommand = new SqlCommand();
            Conn.SQLDA.InsertCommand.Connection = Conn.ZakazCon;
            if (Conn.ZakazCon.State != ConnectionState.Open) Conn.ZakazCon.Open();
            Conn.SQLDA.InsertCommand.CommandText = "insert into Reservation_R..ISSUED (IDMAIN,BAR,DATE_VOZV,IDREADER,IDEMP,DATE_ISSUE,IDMAIN_CONST, " +
                                                    " PENALTY, REMPENALTY, INV, STATUS, IDDATA) values (@IDMAIN,@BAR,@DATE_VOZV,@IDREADER,@IDEMP,@DATE_ISSUE,@IDMAIN_CONST, " +
                                                    "@PENALTY, @REMPENALTY, @INV, @STATUS,@IDDATA)";
            Conn.SQLDA.InsertCommand.Parameters.Add("IDMAIN", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("BAR", SqlDbType.NVarChar);
            Conn.SQLDA.InsertCommand.Parameters.Add("DATE_VOZV", SqlDbType.DateTime);
            Conn.SQLDA.InsertCommand.Parameters.Add("IDREADER", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("IDEMP", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("DATE_ISSUE", SqlDbType.DateTime);
            Conn.SQLDA.InsertCommand.Parameters.Add("IDMAIN_CONST", SqlDbType.NVarChar);
            Conn.SQLDA.InsertCommand.Parameters.Add("PENALTY", SqlDbType.Bit);
            Conn.SQLDA.InsertCommand.Parameters.Add("REMPENALTY", SqlDbType.Bit);
            Conn.SQLDA.InsertCommand.Parameters.Add("INV", SqlDbType.NVarChar);
            Conn.SQLDA.InsertCommand.Parameters.Add("IDDATA", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("STATUS", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters["IDMAIN"].Value = book.id;
            Conn.SQLDA.InsertCommand.Parameters["BAR"].Value = book.barcode;
            Conn.SQLDA.InsertCommand.Parameters["DATE_VOZV"].Value = DateTime.Now.AddDays(days).ToShortDateString();
            Conn.SQLDA.InsertCommand.Parameters["IDREADER"].Value = reader.id;
            Conn.SQLDA.InsertCommand.Parameters["IDEMP"].Value = F1.EmpID;
            Conn.SQLDA.InsertCommand.Parameters["DATE_ISSUE"].Value = DateTime.Now.ToShortDateString();
            Conn.SQLDA.InsertCommand.Parameters["IDMAIN_CONST"].Value = book.id;
            Conn.SQLDA.InsertCommand.Parameters["PENALTY"].Value = false;
            Conn.SQLDA.InsertCommand.Parameters["REMPENALTY"].Value = false;
            Conn.SQLDA.InsertCommand.Parameters["INV"].Value = book.inv;
            Conn.SQLDA.InsertCommand.Parameters["STATUS"].Value = 3;
            Conn.SQLDA.InsertCommand.Parameters["IDDATA"].Value = book.iddata;
            Conn.SQLDA.InsertCommand.ExecuteNonQuery();
            //book = book.Remove(book.Length - 1, 1);
            //reader = reader.Remove(0, 1);
            //reader = reader.Remove(reader.Length - 1, 1);
            /*Conn.SQLDA.SelectCommand.CommandText = "select * from Reservation_R..ISSUED where ID = -1";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(Conn.SQLDA);
            int i = Conn.SQLDA.Fill(Zakaz);

            DataRow row = Zakaz.Tables[0].NewRow();
            row["IDMAIN"] = book.id;
            row["BAR"] = book.barcode;
            row["DATE_VOZV"] = DateTime.Now.AddDays(days).ToShortDateString();
            row["IDREADER"] = reader.id;
            row["IDEMP"] = F1.EmpID;
            row["DATE_ISSUE"] = DateTime.Now.ToShortDateString();
            row["IDMAIN_CONST"] = book.id;
            row["PENALTY"] = false;
            row["REMPENALTY"] = false;
            row["INV"] = book.inv;
            Zakaz.Tables[0].Rows.Add(row);
            //Conn.SQLDA.SelectCommand.CommandText = "select * from ZAKAZ where ID = -1";
            //Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            //SQLDA.InsertCommand = cmdBuilder.GetInsertCommand();
            Conn.SQLDA.Update(Zakaz.Tables[0]);*/
        }
        public bool isRightsExpired(string s)
        {
            Conn.ReaderDA.SelectCommand.CommandText = "select * from ReaderRight where IDReader = " + s + " and IDReaderRight = 4";
            Conn.ReaderDA.SelectCommand.Connection = Conn.ReadersCon;
            SqlCommandBuilder cmd = new SqlCommandBuilder(Conn.ReaderDA);
            ReaderMain = new DataSet();
            Conn.ReaderDA.Fill(ReaderMain, "right");
            bool retval = false;
            DateTime RightsDate = (DateTime)ReaderMain.Tables["right"].Rows[0]["DataEndReaderRight"];
            if ((DateTime.Now.Month == 12) && (RightsDate.Year == DateTime.Now.Year))
            {
                retval = true;
            }

            return retval; 
                //((DateTime)ReaderMain.Tables["right"].Rows[0]["DataEndReaderRight"] < DateTime.Now) ? true : false;
        }
        public void ProlongRights(string s)
        {
            Conn.ReaderDA.SelectCommand.CommandText = "select * from [Readers].[dbo].ReaderRight where IDReader = " + s + " and IDReaderRight = 4";
            Conn.ReaderDA.SelectCommand.Connection = Conn.ReadersCon;
            SqlCommandBuilder cmd = new SqlCommandBuilder(Conn.ReaderDA);
            ReaderMain = new DataSet();
            Conn.ReaderDA.Fill(ReaderMain, "right");
            ReaderMain.Tables[0].Rows[0]["DataEndReaderRight"] = ((DateTime)ReaderMain.Tables[0].Rows[0]["DataEndReaderRight"]).AddYears(1);
            Conn.ReaderDA.Update(ReaderMain.Tables[0]);
        }
/*            class Class1
{
   bool  aviableToTakeABook ();
   bool isExpired ();
}

Class1 cl;

if (cl.aviableToTakeABook ())
   return;

string mes = cl.isExpired () ? "��������?" : "��������� �����?";

 Mor (13:31:57 12/08/2009)
���� � ���, ��������, �����:

 Mor (13:37:05 12/08/2009)
class Rights {}
class TakeResult
{
   bool Expired;
   Rights Rights;
   bool Ok
}

class A
{
  TakeResul       TakeABook ();
}

A a;
TakeResult tr = a.TakeABook ();

if (tr.Ok)
   return;

if (tr.Expired)
  mes = "��������?"

if (tr.Right == null || tr.Rights == None)
  mes = "�����?"*/
        
        public void setReaderRight(string s)
        {
            Conn.ReaderDA.SelectCommand.CommandText = "select * from [Readers].[dbo].ReaderRight where IDReader = -1";
            Conn.ReaderDA.SelectCommand.Connection = Conn.ReadersCon;
            SqlCommandBuilder cmd = new SqlCommandBuilder(Conn.ReaderDA);
            ReaderMain = new DataSet();
            Conn.ReaderDA.Fill(ReaderMain, "right");
            DataRow row = ReaderMain.Tables["right"].NewRow();
            row["IDReader"] = s;
            row["IDReaderRight"] = 4;
            row["DataEndReaderRight"] = new DateTime(DateTime.Now.AddYears(1).Year,12,31);
            ReaderMain.Tables["right"].Rows.Add(row);
            Conn.ReaderDA.Update(ReaderMain, "right");
            Conn.ReaderDA.SelectCommand.CommandText = "select * from [Readers].[dbo].ReaderRight where IDReader = -1";
            Conn.ReaderDA.SelectCommand.Connection = Conn.ReadersCon;

        }
        public void setBookReturned(string s, dbBook book)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select * from [Reservation_R].[dbo].[ISSUED] where IDMAIN = " + s + " and BAR = '"+book.barcode+"' ";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(Conn.SQLDA);
            DataSet B = new DataSet();
            int i = Conn.SQLDA.Fill(B);
            if (i == 0)
            {
                //
            }
            if (B.Tables[0].Rows[0]["IDROLD"].ToString() != "")
            {
                DeleteBarFromBJVVV(B.Tables[0].Rows[0]["BAR"].ToString());
            }
            Conn.SQLDA.UpdateCommand = new SqlCommand();
            Conn.SQLDA.UpdateCommand.Connection = Conn.BJVVVConn;
            Conn.SQLDA.UpdateCommand.CommandText = "update Reservation_R..ISSUED set IDMAIN = 0, IDROLD = '',DATE_FACT_VOZV = '"+DateTime.Today.ToString("yyyyMMdd")+"' where ID = " + B.Tables[0].Rows[0]["ID"].ToString();
            if (Conn.SQLDA.UpdateCommand.Connection.State == ConnectionState.Closed)
            {
                Conn.SQLDA.UpdateCommand.Connection.Open();
            }
            
            int rc = Conn.SQLDA.UpdateCommand.ExecuteNonQuery();
            Conn.SQLDA.UpdateCommand.Connection.Close();
            //B.Tables[0].Rows[0]["IDMAIN"] = "0";
            //B.Tables[0].Rows[0]["DATE_FACT_VOZV"] = DateTime.Now.ToShortDateString();
            //B.Tables[0].Rows[0]["IDROLD"] = "";
            //Conn.SQLDA.UpdateCommand = cmdBuilder.GetUpdateCommand();
            //Conn.SQLDA.Update(B.Tables[0]);



        }
        private void DeleteBarFromBJVVV(string bar)
        {
            //������� �������� �� BJVVV ������ ��� �� ������
            Conn.SQLDA.SelectCommand.CommandText = "select ID from BJVVV.[dbo].DATAEXT where MNFIELD = 899 and MSFIELD = '$w' and SORT = '" + bar + "'";
            DataSet DS = new DataSet();
            Conn.SQLDA.Fill(DS, "t");
            Conn.SQLDA.DeleteCommand = new SqlCommand();
            Conn.SQLDA.DeleteCommand.Connection = Conn.BJVVVConn;
            if (Conn.SQLDA.DeleteCommand.Connection.State == ConnectionState.Closed)
            {
                Conn.SQLDA.DeleteCommand.Connection.Open();
            }
         
            Conn.SQLDA.DeleteCommand.CommandText = "delete from BJVVV.dbo.DATAEXT where ID = " + DS.Tables["t"].Rows[0][0].ToString();
            int rc = Conn.SQLDA.DeleteCommand.ExecuteNonQuery();
            Conn.SQLDA.DeleteCommand.CommandText = "delete from BJVVV.dbo.DATAEXTPLAIN where IDDATAEXT = " + DS.Tables["t"].Rows[0][0].ToString();
            rc = Conn.SQLDA.DeleteCommand.ExecuteNonQuery();
            Conn.SQLDA.DeleteCommand.Connection.Close();
            return;
        }
        public void setBookLost(string s)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select * from [Reservation_R].[dbo].ISSUED where INV = '" + s + "' and IDMAIN <> 0";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(Conn.SQLDA);
            DataSet B = new DataSet();
            int i = Conn.SQLDA.Fill(B);
            B.Tables[0].Rows[0]["IDMAIN"] = "0";
            B.Tables[0].Rows[0]["DATE_FACT_VOZV"] = B.Tables[0].Rows[0]["DATE_ISSUE"];
            Conn.SQLDA.Update(B);

        }
        public bool isBookBusy(string s)
        {
            //s = s.Remove(s.Length - 1, 1);
            Conn.SQLDA.SelectCommand.CommandText = "select * from Reservation_R..ISSUED where  BAR ='" + s + "' and IDMAIN <>0";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            //Book.Tables.Clear();
            DataSet Book = new DataSet();
            int i = Conn.SQLDA.Fill(Book);
            if (i == 0) return false;
            string j = Book.Tables[0].Rows[0]["IDReader"].ToString();
            if (j == "-1") return true;
            else return true;
            //return (i != 0) ? true : false;
        }
        public bool isReaderHaveRights(dbReader r)
        {
            //r = r.Remove(0, 1);
            //r = r.Remove(r.Length - 1, 1);
            CultureInfo ci = new CultureInfo("en-US");
            string date = DateTime.Now.ToString("d", ci);//SELECT ReaderRight.* FROM ReaderRight WHERE (((ReaderRight.DataEndReaderRight)=#10/20/2008#) AND ((ReaderRight.IDReader)=1) AND ((ReaderRight.IDReaderRight)=1));
            //Conn.ReaderDA.SelectCommand.CommandText = "SELECT ReaderRight.* FROM ReaderRight WHERE (((ReaderRight.DataEndReaderRight)>#" + date + "#) AND ((ReaderRight.IDReader)=" + r.id + ") AND ((ReaderRight.IDReaderRight)=1))";
            Conn.ReaderDA.SelectCommand.CommandText = "SELECT ReaderRight.* FROM ReaderRight WHERE ReaderRight.IDReader=" + r.id + " AND ReaderRight.IDReaderRight=4";
            //"select * from ReaderRight where IDReader = " + this.getDbReader(r).id + " and IDReaderRight = 1 and DateEndReaderRight > (#"+date +"#)";//������ �������
            //int i = ReaderDA.Fill(ReaderMain, "dbr");
            DataSet R = new DataSet();
            return (Conn.ReaderDA.Fill(R) == 0) ? false : true;
        }
        public bool isReader(string s)
        {
            if (s.Length > 0)
                s = s.Remove(0, 1);
            //if (s.Length > 0)
            //s = s.Remove(s.Length - 1, 1);
            return ((s.Length > 18) || (s.Length == 7)) ? true : false;
        }
        public dbReader getDbReader(string s)
        {
            //s = s.Remove(0, 1);
            //s = s.Remove(s.Length - 1, 1);
            if (s.Length < 19)
            {
                s = s.Remove(0, 1);
                //s = s.Remove(s.Length - 1, 1);
                Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName from main where BarCode = " + s;
            }
            else
            {
                s = s.Remove(s.IndexOf(' '), s.Length - s.IndexOf(' '));
                Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName from main where  NumberSC = '" + s + "'";
            }
            DataSet R = new DataSet();
            if (Conn.ReaderDA.Fill(R) != 0)
                return new dbReader(R.Tables[0].Rows[0][0].ToString(), R.Tables[0].Rows[0][1].ToString(), R.Tables[0].Rows[0][2].ToString() + " " + R.Tables[0].Rows[0][3].ToString().Remove(1, R.Tables[0].Rows[0][3].ToString().Length - 1) + ". " + R.Tables[0].Rows[0][4].ToString().Remove(1, R.Tables[0].Rows[0][4].ToString().Length - 1) + ".");
            else
                return new dbReader();
        }
        public dbBook getDbBook(string s)
        {
            //s = s.Remove(s.Length - 1, 1);
            Conn.SQLDA.SelectCommand.CommandText = "select  ID, IDMAIN, BARCODE from BARCODE_UNITS where BARCODE = '" + s + "'";
            Conn.SQLDA.SelectCommand.Connection = Conn.BRIT_SOVETCon;
            //Book.Tables.Clear();
            DataSet B = new DataSet();
            int i = Conn.SQLDA.Fill(B);
            Conn.SQLDA.SelectCommand.CommandText = "select SORT from DATAEXT where IDMAIN = '" + B.Tables[0].Rows[0]["IDMAIN"].ToString() + "' and MNFIELD = '200' and MSFIELD = '$a'";
            Conn.SQLDA.SelectCommand.Connection = Conn.BRIT_SOVETCon;
            DataSet Z = new DataSet();
            i = Conn.SQLDA.Fill(Z);
            //string j = Book.Tables[0].Rows[0]["Creator"].ToString();
            return new dbBook(B.Tables[0].Rows[0]["IDMAIN"].ToString(), B.Tables[0].Rows[0]["BARCODE"].ToString(), Z.Tables[0].Rows[0]["SORT"].ToString(), "", "",DateTime.Now,DateTime.Now);
        }
        public string GetDateRet(string s)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select DATE_VOZV from Reservation_R..ISSUED where BARCODE = '" + s + "'";
            Conn.SQLDA.SelectCommand.Connection = Conn.BRIT_SOVETCon;
            //Book.Tables.Clear();
            DataSet B = new DataSet();
            int i = Conn.SQLDA.Fill(B);
            return B.Tables[0].Rows[0]["DATE_VOZV"].ToString();
        }
        public int SetReaderBarCode(string ID, string barCode)
        {

            Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, NumberSC, FamilyName, Name, FatherName from main where BarCode = " + barCode.Remove(0, 1);
            Conn.ReaderDA.SelectCommand.Connection = Conn.ReadersCon;
            DataSet R = new DataSet();
            int i = 0;
            try
            {
                i = Conn.ReaderDA.Fill(R);
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                //MessageBox.Show("������ �������� ��������!");
                return -5;
            }
            if (i != 0)
                return -4;
            Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, NumberSC, FamilyName, Name, FatherName from [Readers].[dbo].Main where NumberReader = " + ID;
            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(Conn.ReaderDA);
            R = new DataSet();
            i = 0;
            try
            {
                i = Conn.ReaderDA.Fill(R);
            }
            catch
            {
                return -1;
            }
            if (i == 0)
                return -2;
            if (R.Tables[0].Rows[0]["NumberSC"].ToString() != "")
                return -3;
            R.Tables[0].Rows[0]["BarCode"] = barCode.Remove(0, 1);
            Conn.ReaderDA.Update(R);
            return 1;
        }
        public class dbReader
        {
            public dbReader()
            {
                this.barcode = "";
                this.FIO = "";
                this.id = "";
                this.IsWasInOldBase = false;
            }
            public dbReader(int numberReader)
            {
                //Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName from main where NumberReader = " + numberReader.ToString();
                Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName,AbonementType,NameAbonType,IDOldAbonement from main inner join AbonementType on main.AbonementType = AbonementType.IDAbonemetType where NumberReader = " + numberReader.ToString();
                Conn.ReaderDA.SelectCommand.Connection = Conn.ReadersCon;
                DataSet R = new DataSet();
                int i;
                try
                {
                    i = Conn.ReaderDA.Fill(R);
                }
                catch
                {
                    this.barcode = "error";
                    return;
                }
                if (i == 0)
                {
                    this.barcode = "error";
                    return;
                }
                this.Surname = R.Tables[0].Rows[0]["FamilyName"].ToString();
                this.Name = R.Tables[0].Rows[0]["Name"].ToString();
                this.SecondName = R.Tables[0].Rows[0]["FatherName"].ToString();
                string name = "";
                string secondName = "";
                try
                {
                    name = R.Tables[0].Rows[0]["Name"].ToString().Remove(1, R.Tables[0].Rows[0]["Name"].ToString().Length - 1) + ". ";
                }
                catch
                {
                    name = "";
                }
                try
                {
                    secondName = R.Tables[0].Rows[0]["FatherName"].ToString().Remove(1, R.Tables[0].Rows[0]["FatherName"].ToString().Length - 1) + ".";
                }
                catch
                {
                    secondName = "";
                }
                this.FIO = R.Tables[0].Rows[0]["FamilyName"].ToString() + " " + name + secondName;
                this.id = R.Tables[0].Rows[0]["NumberReader"].ToString();
                this.barcode = R.Tables[0].Rows[0]["BarCode"].ToString();
                this.AbonType = R.Tables[0].Rows[0]["NameAbonType"].ToString();
                Type t = R.Tables[0].Rows[0]["IDOldAbonement"].GetType();
                if (t == typeof(System.DBNull))
                {
                    this.IsWasInOldBase = false;
                }
                else
                {
                    this.IsWasInOldBase = this.IsWasInOldBase = (bool)R.Tables[0].Rows[0]["IDOldAbonement"];
                }
                Conn.ReaderDA.SelectCommand.CommandText = "select * from Readers..AbonementAdd where IDReader = " + this.id;
                DataSet DS = new DataSet();
                int rr = Conn.ReaderDA.Fill(DS, "t");
                if (rr == 0)
                {
                    this.RegInMos = DateTime.MinValue;
                }
                else
                {
                    if (DS.Tables["t"].Rows[0]["RegInMoscow"] == DBNull.Value)
                    {
                        this.RegInMos = DateTime.MinValue;
                    }
                    else
                    {
                        this.RegInMos = (DateTime)DS.Tables["t"].Rows[0]["RegInMoscow"];
                    }
                }
                
            }
            public dbReader(dbReader Reader)
            {
                this.barcode = Reader.barcode;
                this.FIO = Reader.FIO;
                this.id = Reader.id;
                this.Surname = Reader.Surname;
                this.Name = Reader.Name;
                this.SecondName = Reader.SecondName;
                this.AbonType = Reader.AbonType;
                this.IsWasInOldBase = Reader.IsWasInOldBase;
                this.RegInMos = Reader.RegInMos;
            }
            public dbReader Clone()
            {
                return new dbReader(this);
            }
            public dbReader(string id, string barcode, string FIO)
            {
                this.barcode = barcode;
                this.id = id;
                this.FIO = FIO;
                this.IsWasInOldBase = false;
                Conn.ReaderDA.SelectCommand.CommandText = "select * from Readers..AbonementAdd where IDReader = " + this.id;
                DataSet DS = new DataSet();
                int rr = Conn.ReaderDA.Fill(DS, "t");
                if (rr == 0)
                {
                    this.RegInMos = DateTime.MinValue;
                }
                else
                {
                    if (DS.Tables["t"].Rows[0]["RegInMoscow"] == DBNull.Value)
                    {
                        this.RegInMos = DateTime.MinValue;
                    }
                    else
                    {
                        this.RegInMos = (DateTime)DS.Tables["t"].Rows[0]["RegInMoscow"];
                    }
                }

            }
            public dbReader(string Bar)
            {
                bool SocCard = false;
                bool NumSocCard = false;
                bool SerSocCard = false;
                bool FoundByNumber = false;
                DataSet DS = new DataSet();
                if (Bar.Length > 18)
                {
                    SocCard = true;
                    if (Bar.Contains(" "))
                    {
                        Bar = Bar.Remove(19, 1);
                    } 
                    string Ser = Bar.Substring(19, 8);
                    Bar = Bar.Substring(0, 19);
                    //Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName from main where NumberSC = '" + Bar + "' and SerialSC = '" + Ser + "'";
                    Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName,AbonementType,NameAbonType,IDOldAbonement from main inner join AbonementType on main.AbonementType= AbonementType.IDAbonemetType where NumberSC = '" + Bar + "'";
                    DS = new DataSet(); 
                    int c = Conn.ReaderDA.Fill(DS);
                    if (c == 0)
                        NumSocCard = true;
                    else
                    {
                        NumSocCard = false;
                        Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName,AbonementType,NameAbonType,IDOldAbonement from main inner join AbonementType on main.AbonementType= AbonementType.IDAbonemetType where NumberSC = '" + Bar + "' and SerialSC = '" + Ser + "'";
                        DS = new DataSet();
                        int cnt = Conn.ReaderDA.Fill(DS);
                        if (cnt == 0)
                            SerSocCard = true;
                        else
                            SerSocCard = false;
                    }

                }
                else
                {
                    //Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName from main where BarCode = " + Bar;
                    if (Bar[0].ToString() == "R")
                    {
                        Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName,AbonementType,NameAbonType," +
                            " IDOldAbonement " +
                            " from Readers..Main " +
                            " inner join AbonementType on main.AbonementType = AbonementType.IDAbonemetType " +
                            " where BarCode = '" + Bar.Remove(0, 1) + "'";
                    }
                    else
                    {
                        Conn.ReaderDA.SelectCommand.CommandText =
                             " select A.NumberReader, A.BarCode, A.FamilyName, A.[Name], A.FatherName, " +
                             " A.AbonementType,B.NameAbonType, A.IDOldAbonement  " +
                             " from Readers..Main A " +
                             " inner join Readers..AbonementType B on A.AbonementType = B.IDAbonemetType  " +
                             " left join Readers..Input C on C.IDReaderInput = A.NumberReader " +
                             " where C.BarCodeInput = '" + Bar + "' and DateOutInput is null ";

                    }
                    DS = new DataSet();
                    int ct = Conn.ReaderDA.Fill(DS);
                    if (ct == 0)
                        FoundByNumber = true;
                    else
                        FoundByNumber = false;
                }

                //DataSet R = new DataSet();
                //int i;
                /*try
                {
                    i = Conn.ReaderDA.Fill(R);
                }
                catch
                {
                    this.barcode = "error";
                    return;
                }*/
                if (SocCard)
                {
                    if (!NumSocCard)
                    {
                        if (!SerSocCard)
                        {
                            //� ������. ���� �� ������
                        }
                        else
                        {
                            this.barcode = "sersoc";
                        }
                    }
                    else
                    {
                        this.barcode = "numsoc";
                    }
                }
                else
                {
                    if (!FoundByNumber)
                    {
                        //� ������
                    }
                    else
                    {
                        this.barcode = "notfoundbynumber";
                    }
                }
                if ((this.barcode == "sersoc") || (this.barcode == "numsoc") || (this.barcode == "notfoundbynumber"))
                {
                    this.FIO = "";
                    this.AbonType = "";
                    this.Name = "";
                    this.Surname = "";
                    this.SecondName = "";
                    this.IsWasInOldBase = false;
                    this.id = "";
                }
                else
                {
                    this.barcode = DS.Tables[0].Rows[0]["BarCode"].ToString();
                    this.id = DS.Tables[0].Rows[0]["NumberReader"].ToString();
                    string name = "";
                    string secondName = "";
                    try
                    {
                        name = DS.Tables[0].Rows[0]["Name"].ToString().Remove(1, DS.Tables[0].Rows[0]["Name"].ToString().Length - 1) + ". ";
                    }
                    catch
                    {
                        name = "";
                    }
                    try
                    {
                        secondName = DS.Tables[0].Rows[0]["FatherName"].ToString().Remove(1, DS.Tables[0].Rows[0]["FatherName"].ToString().Length - 1) + ".";
                    }
                    catch
                    {
                        secondName = "";
                    }
                    this.FIO = DS.Tables[0].Rows[0]["FamilyName"].ToString() + " " + name + secondName;
                    this.AbonType = DS.Tables[0].Rows[0]["NameAbonType"].ToString();
                    this.Name = DS.Tables[0].Rows[0]["Name"].ToString();
                    this.Surname = DS.Tables[0].Rows[0]["FamilyName"].ToString();
                    this.SecondName = DS.Tables[0].Rows[0]["FatherName"].ToString();
                    this.IsWasInOldBase = (bool)DS.Tables[0].Rows[0]["IDOldAbonement"];
                }
                if (this.barcode != "notfoundbynumber")
                {
                    Conn.ReaderDA.SelectCommand.CommandText = "select * from Readers..AbonementAdd where IDReader = " + this.id;
                    DS = new DataSet();
                    int rr = Conn.ReaderDA.Fill(DS, "t");
                    if (rr == 0)
                    {
                        this.RegInMos = DateTime.MinValue;
                    }
                    else
                    {
                        if (DS.Tables["t"].Rows[0]["RegInMoscow"] == DBNull.Value)
                        {
                            this.RegInMos = DateTime.MinValue;
                        }
                        else
                        {
                            this.RegInMos = (DateTime)DS.Tables["t"].Rows[0]["RegInMoscow"];
                        }
                    }
                }
            }
            public string barcode;
            public string id;
            public string FIO;
            public string Surname;
            public string Name;
            public string SecondName;
            public string AbonType;
            public bool IsWasInOldBase;
            public DateTime RegInMos;
            public int IntID
            {
                get
                {
                    return int.Parse(this.id);
                }
            }

            internal string GetLiveemail()
            {
                Conn.SQLDA.SelectCommand.CommandText = "select LiveEmail from Readers..Main where NumberReader = " + this.id;
                Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
                DataSet D = new DataSet();
                int i = Conn.SQLDA.Fill(D);
                if (i == 0) return "";
                if (dbReader.IsValidEmail(D.Tables[0].Rows[0][0].ToString()))
                {
                    return D.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    return "";
                }
                
            }
            public static bool IsValidEmail(string strIn)
            {
                // Return true if strIn is in valid e-mail format.
                return Regex.IsMatch(strIn,
                       @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                       @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
            }

            internal string GetWorkEmail()
            {
                Conn.SQLDA.SelectCommand.CommandText = "select WorkEmail from Readers..Main where NumberReader = " + this.id;
                Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
                DataSet D = new DataSet();
                int i = Conn.SQLDA.Fill(D);
                if (i == 0) return "";
                if (dbReader.IsValidEmail(D.Tables[0].Rows[0][0].ToString()))
                {
                    return D.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    return "";
                }
            }

            internal string GetRegEmail()
            {
                Conn.SQLDA.SelectCommand.CommandText = "select RegistrationEmail from Readers..Main where NumberReader = " + this.id;
                Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
                DataSet D = new DataSet();
                int i = Conn.SQLDA.Fill(D);
                if (i == 0) return "";
                if (dbReader.IsValidEmail(D.Tables[0].Rows[0][0].ToString()))
                {
                    return D.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    return "";
                }
            }
        }
        public class dbBook
        {
            public dbBook Clone()
            {
                return new dbBook(this);
            }
            public dbBook()
            {
                this.author = "";
                this.barcode = "";
                this.id = "";
                this.name = "";
                this.rname = "";
                this.inv = "";
                this.vzv = new DateTime();
                this.fctvzv = new DateTime();
                this.iddata = 0;
            }

            public dbBook(dbBook Book)
            {
                this.author = Book.author;
                this.barcode = Book.barcode;
                this.id = Book.id;
                this.name = Book.name;
                this.rname = Book.rname;
                this.inv = Book.inv;
                this.fctvzv = Book.fctvzv;
                this.vzv = Book.vzv;
                this.iddata = Book.iddata;
                this.name = Book.name;
                this.rname = Book.rname;
            }
            public dbBook(string id, string barcode, string name, string rname, string inv_, DateTime vzv_, DateTime fctvzv_)
            {
                this.id = id;
                this.barcode = barcode;
                this.name = name;
                this.rname = rname;
                this.author = "";
                this.inv = inv_;
                this.vzv = vzv_;
                this.fctvzv = fctvzv_;
            }
            public dbBook(string Bar)
            {
                Conn.SQLDA.SelectCommand.CommandText = "select  ID, IDMAIN, SORT, IDDATA from BJVVV..DATAEXT where SORT = '" + Bar + "' and MNFIELD = 899 and MSFIELD = '$w'";
                Conn.SQLDA.SelectCommand.Connection = Conn.BRIT_SOVETCon;
                //Book.Tables.Clear();
                DataSet B = new DataSet();
                int i = Conn.SQLDA.Fill(B);
                string IDDATA = B.Tables[0].Rows[0]["IDDATA"].ToString();
                if (i == 0)
                {
                    this.id = "�������� ��������";
                    return;
                }
                this.id = B.Tables[0].Rows[0]["IDMAIN"].ToString();
                this.barcode = B.Tables[0].Rows[0]["SORT"].ToString();
                this.iddata = (int)B.Tables[0].Rows[0]["IDDATA"];
                Conn.SQLDA.SelectCommand.CommandText = "select  ID, IDMAIN, SORT, IDDATA from BJVVV..DATAEXT where IDDATA = '" + IDDATA + "' and MNFIELD = 899 and MSFIELD = '$p'";
                Conn.SQLDA.SelectCommand.Connection = Conn.BRIT_SOVETCon;
                B = new DataSet();
                i = Conn.SQLDA.Fill(B);
                string INVN = B.Tables[0].Rows[0]["SORT"].ToString();

                Conn.SQLDA.SelectCommand.CommandText = "WITH FC AS (SELECT dt.ID,dt.SORT, "+
                                                          "dt.MNFIELD, "+
                                                          "dt.MSFIELD, "+
                                                          "dt.IDMAIN, "+
                                                          "dtp.PLAIN "+
                                                   "FROM   BJVVV..DATAEXT dt " +
                                                   "       JOIN BJVVV..DATAEXTPLAIN dtp " +
                                                   "            ON  dt.ID = dtp.IDDATAEXT) "+
                                                   "select  COL1.PLAIN zag,dtpa.PLAIN avt from FC COL1 "+
                                                   "left join FC dtpa ON COL1.IDMAIN = dtpa.IDMAIN and dtpa.MNFIELD = 700 and dtpa.MSFIELD = '$a' "+
                                                   "where COL1.MNFIELD = 200 and COL1.MSFIELD = '$a'  and COL1.IDMAIN = " + this.id;
                Conn.SQLDA.SelectCommand.Connection = Conn.BRIT_SOVETCon;
                B = new DataSet();
                i = Conn.SQLDA.Fill(B);
                this.name = B.Tables[0].Rows[0]["zag"].ToString(); ;
                this.author = B.Tables[0].Rows[0]["avt"].ToString();
                Conn.SQLDA.SelectCommand.CommandText = "select B.SORT from BJVVV..DATAEXT A, BJVVV..DATAEXT B " +
                                                       " where A.IDMAIN  = " + this.id + " and A.SORT = '" + this.barcode +
                                                       "' and A.MSFIELD = '$w' and A.MNFIELD = 899  and " +
                                                       " A.IDDATA = B.IDDATA and B.MNFIELD= 899 and B.MSFIELD = '$p' ";
                B = new DataSet();
                i = Conn.SQLDA.Fill(B);
                this.inv = B.Tables[0].Rows[0]["SORT"].ToString();

                Conn.SQLDA.SelectCommand.CommandText = "select * from Reservation_R..ISSUED where IDMAIN = " + this.id + " and IDDATA = " + this.iddata;
                Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
                B = new DataSet();
                this.rname = "";
                try
                {
                    i = Conn.SQLDA.Fill(B);
                    this.rid = B.Tables[0].Rows[0]["IDREADER"].ToString();
                    this.vzv = DateTime.Parse(B.Tables[0].Rows[0]["DATE_VOZV"].ToString());
                    this.fctvzv = DateTime.Parse(B.Tables[0].Rows[0]["DATE_FACT_VOZV"].ToString());
                }
                catch
                {
                    this.rname = "";
                }
                if ((this.rid != "") && (this.rid != "-1") && (this.rid != null))
                {
                    Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName,NumberSC,SerialSC from main where NumberReader = " + this.rid;
                    DataSet R = new DataSet();
                    Conn.ReaderDA.Fill(R);
                    string name = "";
                    string secondName = "";
                    try
                    {
                        name = R.Tables[0].Rows[0]["Name"].ToString().Remove(1, R.Tables[0].Rows[0]["Name"].ToString().Length - 1) + ". ";
                    }
                    catch
                    {
                        name = "";
                    }
                    try
                    {
                        secondName = R.Tables[0].Rows[0]["FatherName"].ToString().Remove(1, R.Tables[0].Rows[0]["FatherName"].ToString().Length - 1) + ".";
                    }
                    catch
                    {
                        secondName = "";
                    }
                    this.rname = R.Tables[0].Rows[0]["FamilyName"].ToString() + " " + name + secondName;
                    this.rbar = R.Tables[0].Rows[0]["BarCode"].ToString();
                    if (this.rbar == "0")
                        this.rbar = R.Tables[0].Rows[0]["NumberSC"].ToString().Trim().Replace("\0", "") + " " + R.Tables[0].Rows[0]["SerialSC"].ToString().Trim().Replace("\0", ""); ;
                    //this.rname = R.Tables[0].Rows[0]["FamilyName"].ToString() + " " + R.Tables[0].Rows[0]["Name"].ToString().Remove(1, R.Tables[0].Rows[0]["Name"].ToString().Length - 1) + ". " + R.Tables[0].Rows[0]["FatherName"].ToString().Remove(1, R.Tables[0].Rows[0]["FatherName"].ToString().Length - 1) + ".";
                }
                /*                finally
                                {
                                    this.rname = "";
                                }*/

                //this.rname = ;
            }
            public string barcode;
            public string id;
            public string name;
            public string rname;
            public string author;
            public string inv;
            public DateTime vzv;
            public DateTime fctvzv;
            public int iddata;
            public string rid;
            public string rbar;
        }



        public bool ChangeEmployee(string login, string pass)
        {//                                    SELECT Employee.* FROM Employee WHERE (((Employee.Login)="1") AND ((Employee.Password)="1"));

            Conn.ReaderDA.SelectCommand.CommandText = "SELECT * FROM BJVVV..USERS WHERE lower(LOGIN)='" + login.ToLower() + "' AND lower(PASSWORD)='" + pass.ToLower() + "'";
            //ReaderMain.Tables.Clear();
            DataSet R = new DataSet();
            if (Conn.ReaderDA.Fill(R) != 0)
            {
                F1.textBox1.Text = R.Tables[0].Rows[0]["LOGIN"].ToString();
                F1.EmpID = R.Tables[0].Rows[0]["ID"].ToString();
                return true;
            }
            else
                return false;
        }

        public class XmlConnections
        {
            public XmlTextReader reader;
            static String filename = Application.StartupPath + "\\DBConnections.xml";
            public XmlDocument doc;
            public string GetReaderCon()
            {
                XmlNode node = this.doc.SelectSingleNode("/Connections/Readers");
                return node.InnerText;
            }
            public string GetZakazCon()
            {
                XmlNode node = this.doc.SelectSingleNode("/Connections/Zakaz");
                return node.InnerText;
            }
            public string GetBRIT_SOVETCon()
            {
                XmlNode node = this.doc.SelectSingleNode("/Connections/Zakaz");
                return node.InnerText;
            }
            internal string GetBJVVVCon()
            {
                XmlNode node = this.doc.SelectSingleNode("/Connections/BJVVV");
                return node.InnerText;                
            }

            public XmlConnections()
            {
                // Create the validating reader and specify DTD validation.
                try
                {
                    //XmlReaderSettings settings = new XmlReaderSettings();
                    //settings.ProhibitDtd = false;
                    //settings.ValidationType = ValidationType.DTD;
                    //settings.ValidationEventHandler += eventHandler;

                    //reader = new XmlTextReader(filename);

                    // Pass the validating reader to the XML document.
                    // Validation fails due to an undefined attribute, but the 
                    // data is still loaded into the document.
                    doc = new XmlDocument();
                    doc.Load(filename);// (reader);
                    //Console.WriteLine(doc.OuterXml);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }


        }

        public DataTable GetDebtors()
        {                                                                                                                                                                                                                                                                                       // "+DateTime.Now.ToString("MM/dd/yyyy")+"                   
            Conn.SQLDA.SelectCommand.CommandText = "select X.IDMAIN, X.PLAIN, Y.SORT, Y.MNFIELD, Z.DATE_VOZV, Z.IDREADER from BJVVV..DATAEXTPLAIN X join BJVVV..DATAEXT Y on Y.ID=X.IDDATAEXT join Reservation_R..ISSUED Z on Z.IDMAIN = Y.IDMAIN where (Z.IDMAIN <> 0) and (Z.DATE_VOZV < '" + DateTime.Now.ToString("yyyyMMdd") + "') and ((Y.MNFIELD = 200 and Y.MSFIELD = '$a') or (Y.MSFIELD = '$a' and Y.MNFIELD = 700)) order by X.IDMAIN";
            //Conn.SQLDA.SelectCommand.CommandText = "select DATE_VOZV, IDREADER from ZAKAZ where IDMAIN <> 0 and DATE_VOZV < '11.11.2008'"; //" + DateTime.Now.ToString("MM/dd/yyyy") + "'";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet R = new DataSet();
            DataSet D = new DataSet();
            R.Tables.Add("vperemeshku");
            R.Tables.Add("distinct");
            int i = Conn.SQLDA.Fill(R.Tables["vperemeshku"]);
            Conn.SQLDA.SelectCommand.CommandText = "select DATE_VOZV, IDREADER from Reservation_R..ISSUED where IDMAIN <> 0 and DATE_VOZV < '" + DateTime.Now.ToString("yyyyMMdd") +"' order by IDMAIN";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            i = Conn.SQLDA.Fill(R.Tables["distinct"]);

            R.Tables.Add("postolbcam");
            R.Tables["postolbcam"].Columns.Add("date");
            R.Tables["postolbcam"].Columns.Add("num");
            R.Tables["postolbcam"].Columns.Add("fam");
            R.Tables["postolbcam"].Columns.Add("name");
            R.Tables["postolbcam"].Columns.Add("secname");
            R.Tables["postolbcam"].Columns.Add("Zagl");
            R.Tables["postolbcam"].Columns.Add("Avtor");
            R.Tables["postolbcam"].Columns.Add("ZagSort");
            R.Tables["postolbcam"].Columns.Add("AvtorSort");

            DataRow ARow = R.Tables["postolbcam"].NewRow();
            string id = R.Tables["vperemeshku"].Rows[0]["IDMAIN"].ToString();
            ARow["date"] = DateTime.Parse(R.Tables["vperemeshku"].Rows[0]["DATE_VOZV"].ToString()).ToString("yyyy-MM-dd"); 
            if (R.Tables["vperemeshku"].Rows[0]["IDREADER"].ToString() == "-1")
            {
                ARow["num"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                ARow["fam"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                ARow["name"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                ARow["secname"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
            }
            else
            {
                Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName from main where NumberReader = " + R.Tables["vperemeshku"].Rows[0]["IDREADER"].ToString();
                i = Conn.ReaderDA.Fill(D);
                ARow["num"] = D.Tables[0].Rows[0]["NumberReader"].ToString();
                ARow["fam"] = D.Tables[0].Rows[0]["FamilyName"].ToString();
                ARow["name"] = D.Tables[0].Rows[0]["Name"].ToString();
                ARow["secname"] = D.Tables[0].Rows[0]["FatherName"].ToString();
            }
            //ARow["sprash"] = R.Tables["vperemeshku"].Rows[0]["sp"].ToString();
            foreach (DataRow row in R.Tables["vperemeshku"].Rows)
            {
                if (id != row["IDMAIN"].ToString())
                {
                    D.Clear();
                    R.Tables["postolbcam"].Rows.Add(ARow);
                    ARow = R.Tables["postolbcam"].NewRow();
                    id = row["IDMAIN"].ToString();
                    ARow["date"] = DateTime.Parse(row["DATE_VOZV"].ToString()).ToString("yyyy-MM-dd");
                    if (row["IDREADER"].ToString() == "-1")
                    {
                        ARow["num"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                        ARow["fam"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                        ARow["name"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                        ARow["secname"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                    }
                    else
                    {
                        Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName from main where NumberReader = " + row["IDREADER"].ToString();
                        i = Conn.ReaderDA.Fill(D);
                        ARow["num"] = D.Tables[0].Rows[0]["NumberReader"].ToString();
                        ARow["fam"] = D.Tables[0].Rows[0]["FamilyName"].ToString();
                        ARow["name"] = D.Tables[0].Rows[0]["Name"].ToString();
                        ARow["secname"] = D.Tables[0].Rows[0]["FatherName"].ToString();
                    }
                }

                switch (row["MNFIELD"].ToString())
                {
                    case "200":
                        ARow["Zagl"] = row["PLAIN"].ToString();
                        ARow["ZagSort"] = row["SORT"].ToString();
                        break;
                    case "700":
                        ARow["Avtor"] = row["PLAIN"].ToString();
                        ARow["AvtorSort"] = row["SORT"].ToString();
                        break;
                }
            }
            R.Tables["postolbcam"].Rows.Add(ARow);

            return R.Tables["postolbcam"];
        }

        public DataTable GetIssuedBooks(DateTime start_, DateTime finish_)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select  X.IDMAIN, X.PLAIN, Y.SORT, Y.MNFIELD,Y.MSFIELD, (count(Z.BAR)) as sp, Z.DATE_VOZV,Z.DATE_ISSUE,Z.IDREADER " +
                                                   " from BJVVV..DATAEXTPLAIN X " +
                                                   "  join BJVVV..DATAEXT Y on Y.ID=X.IDDATAEXT " +
                                                   "  join Reservation_R..ISSUED Z on Z.IDMAIN = Y.IDMAIN " +
                                                   "  join Reservation_R..ISSUED ZZ on Z.IDMAIN = ZZ.IDMAIN_CONST " +
                                                   " where ((Y.MNFIELD = 200 and Y.MSFIELD = '$a') or (Y.MSFIELD = '$a' and Y.MNFIELD = 700) " +
                                                   " or (Y.MSFIELD = '$p' and Y.MNFIELD = 899 and Y.SORT collate Cyrillic_General_CI_AI  =  Z.INV)) and (Z.DATE_ISSUE between '" + start_.ToString("yyyyMMdd") + "' and '" + finish_.ToString("yyyyMMdd") + "') " +
                                                   " group by X.PLAIN, Y.SORT, Y.MNFIELD,Y.MSFIELD, X.IDMAIN,Z.DATE_VOZV,Z.DATE_ISSUE,Z.IDREADER order by X.IDMAIN"; //inner join TECHNOLOG..ZAKAZ Y on Y.BAR=Z.BAR";
            //Conn.SQLDA.SelectCommand.CommandText = "select  X.PREOPS, X.PREOPSAUTHOR,count(Z.BAR) as �������������� from technolog..zakaz Z inner join BRIT_SOVET..MAIN X on Z.IDMAIN_CONST = X.ID  group by X.PREOPS,X.PREOPSAUTHOR";
            //Conn.SQLDA.SelectCommand.CommandText = "select  BRIT_SOVET..MAIN.PREOPS, BRIT_SOVET..MAIN.PREOPSAUTHOR from technolog..zakaz inner join BRIT_SOVET..MAIN on ZAKAZ.IDMAIN = MAIN.ID";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet R = new DataSet();
            DataSet D = new DataSet();
            R.Tables.Add("vperemeshku");
            R.Tables.Add("distinct");
            int i = Conn.SQLDA.Fill(R.Tables["vperemeshku"]);
            Conn.SQLDA.SelectCommand.CommandText = "select distinct Y.IDMAIN from BJVVV..DATAEXT Y inner join Reservation_R..ISSUED Z on Z.IDMAIN = Y.IDMAIN  where Z.IDMAIN != 0 and Z.INV collate Cyrillic_General_CI_AI = Y.SORT and Y.MNFIELD = 899 and Y.MSFIELD = '$p' order by Y.IDMAIN";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon; //
            i = Conn.SQLDA.Fill(R.Tables["distinct"]);
            R.Tables.Add("postolbcam");
            R.Tables["postolbcam"].Columns.Add("Zagl");
            R.Tables["postolbcam"].Columns.Add("Avtor");
            R.Tables["postolbcam"].Columns.Add("Inv");
            R.Tables["postolbcam"].Columns.Add("sprash");
            R.Tables["postolbcam"].Columns.Add("ZagSort");
            R.Tables["postolbcam"].Columns.Add("AvtorSort");

            R.Tables["postolbcam"].Columns.Add("NN");
            R.Tables["postolbcam"].Columns.Add("FIO");
            R.Tables["postolbcam"].Columns.Add("abn");
            R.Tables["postolbcam"].Columns.Add("diss");
            R.Tables["postolbcam"].Columns.Add("dvzv");
            R.Tables["postolbcam"].Columns["diss"].DataType = typeof(DateTime);
            R.Tables["postolbcam"].Columns["dvzv"].DataType = typeof(DateTime);
            DataRow ARow = R.Tables["postolbcam"].NewRow();
            string id = R.Tables["vperemeshku"].Rows[0]["IDMAIN"].ToString();
            ARow["dvzv"] = DateTime.Parse(R.Tables["vperemeshku"].Rows[0]["DATE_VOZV"].ToString()).ToString();
            ARow["diss"] = DateTime.Parse(R.Tables["vperemeshku"].Rows[0]["DATE_ISSUE"].ToString()).ToString();
            dbReader rdr = new dbReader(int.Parse(R.Tables["vperemeshku"].Rows[0]["IDREADER"].ToString()));
            ARow["NN"] = rdr.id;
            ARow["FIO"] = rdr.FIO;
            ARow["abn"] = R.Tables["vperemeshku"].Rows[0]["SORT"].ToString();//rdr.AbonType;
            ARow["sprash"] = R.Tables["vperemeshku"].Rows[0]["sp"].ToString();
            foreach (DataRow row in R.Tables["vperemeshku"].Rows)
            {
                if (id != row["IDMAIN"].ToString())
                {
                    R.Tables["postolbcam"].Rows.Add(ARow);
                    ARow = R.Tables["postolbcam"].NewRow();
                    id = row["IDMAIN"].ToString();
                    ARow["sprash"] = row["sp"].ToString();
                    rdr = new dbReader(int.Parse(row["IDREADER"].ToString()));
                    ARow["NN"] = rdr.id;
                    ARow["FIO"] = rdr.FIO;
                    ARow["abn"] = row["SORT"].ToString();//rdr.AbonType;
                }

                switch (row["MNFIELD"].ToString()+row["MSFIELD"].ToString())
                {
                    case "200$a":
                        ARow["Zagl"] = row["PLAIN"].ToString();
                        ARow["ZagSort"] = row["SORT"].ToString();
                        ARow["dvzv"] = DateTime.Parse(row["DATE_VOZV"].ToString()).ToString();
                        ARow["diss"] = DateTime.Parse(row["DATE_ISSUE"].ToString()).ToString();
                        break;
                    case "700$a":
                        ARow["Avtor"] = row["PLAIN"].ToString();
                        ARow["AvtorSort"] = row["SORT"].ToString();
                        break;
                    case "899$p":
                        ARow["Inv"] = row["SORT"].ToString();
                        break;
                }
            }
            R.Tables["postolbcam"].Rows.Add(ARow);

            return R.Tables["postolbcam"];

            /*R.Tables.Add();
            int i = Conn.SQLDA.Fill(R.Tables[0]);

            return R;*/
        }

        public string GetReaderCount(DateTime Start, DateTime End)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select distinct IDREADER,DATE_ISSUE from Reservation_R..ISSUED where DATE_ISSUE >= '" + Start.ToString("yyyyMMdd") + "' and DATE_ISSUE <= '" + End.ToString("yyyyMMdd") +"'";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet R = new DataSet();

            int i = Conn.SQLDA.Fill(R);
            return i.ToString();
        }

        public string GetBooksCount(DateTime Start, DateTime End)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select BAR from Reservation_R..ISSUED where DATE_ISSUE >= '" + Start.ToString("yyyyMMdd") + "' and DATE_ISSUE <= '" + End.ToString("yyyyMMdd") +"'";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet R = new DataSet();
            int i = Conn.SQLDA.Fill(R);
            //CultureInfo.CurrentCulture = ...
            return i.ToString();
        }

        public DataTable GetAllBooks()
        {
            Conn.SQLDA.SelectCommand.CommandText =
                    " select X.IDMAIN, Y.MNFIELD, X.PLAIN, Y.SORT, ( count(Z.BAR)) as sp, Z.IDMAIN as idm, Z.IDMAIN_CONST as idmc,ZZ.IDMAIN as zid, " +
                    " max(case when ZZ.IDMAIN is null then '��������' else '������' end) as vida,Z.BAR bar, Y.MSFIELD " +
                    " from BJVVV..DATAEXTPLAIN X " +
                    " join BJVVV..DATAEXT Y on Y.ID=X.IDDATAEXT " +
                    " left join Reservation_R..ISSUED Z on Z.IDMAIN_CONST=Y.IDMAIN " +
                    " left join Reservation_R..ISSUED ZZ on ZZ.IDMAIN=X.IDMAIN " +
                    " where ((Y.MNFIELD = 200 and Y.MSFIELD = '$a') or (Y.MSFIELD = '$a' and Y.MNFIELD = 700) or (Y.MSFIELD = '$d' and Y.MNFIELD = 2100) or (Y.MSFIELD = '$c' and Y.MNFIELD = 899) or (Y.MSFIELD = '$w' and Y.MNFIELD = 899)) " +
                    " and ((Z.IDMAIN is null and Z.IDMAIN_CONST is null) or " +
                    " (Z.IDMAIN != Z.IDMAIN_CONST) or " +
                    " not exists (select * from Reservation_R..ISSUED t2 where t2.IDMAIN = 0 and t2.IDMAIN_CONST = Z.IDMAIN_CONST)) " +
                    " group by X.PLAIN, Y.SORT, X.IDMAIN, Y.MNFIELD, Z.IDMAIN, Z.IDMAIN_CONST, ZZ.IDMAIN,Z.BAR,Y.MSFIELD " +
                    " order by X.IDMAIN";
            //Conn.SQLDA.SelectCommand.CommandText = "select X.IDMAIN,X.MNFIELD, X.SORT, (count(Y.BAR)) as sp from BRIT_SOVET..DATAEXT X left join TECHNOLOG..ZAKAZ Y on Y.IDMAIN_CONST=X.IDMAIN where (X.MSFIELD = '$a' and X.MNFIELD = 200) or (X.MSFIELD = '$a' and X.MNFIELD = 700) or (X.MSFIELD = '$d' and X.MNFIELD = 2100) group by X.IDMAIN,X.SORT,X.MNFIELD";
            //Conn.SQLDA.SelectCommand.CommandText = "select IDMAIN, SORT, MNFIELD from BRIT_SOVET..DATAEXT where (MSFIELD = '$a' and MNFIELD = 200) or (MSFIELD = '$a' and MNFIELD = 700) or (MSFIELD = '$d' and MNFIELD = 2100)";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet R = new DataSet();
            R.Tables.Add("vperemeshku");
            R.Tables.Add("distinct");
            int i = Conn.SQLDA.Fill(R.Tables["vperemeshku"]);
            Conn.SQLDA.SelectCommand.CommandText = "select distinct IDMAIN from BJVVV..DATAEXT order by IDMAIN ";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            i = Conn.SQLDA.Fill(R.Tables["distinct"]);
            R.Tables.Add("postolbcam");
            R.Tables["postolbcam"].Columns.Add("Polka");
            R.Tables["postolbcam"].Columns.Add("bar");
            R.Tables["postolbcam"].Columns.Add("Zagl");
            R.Tables["postolbcam"].Columns.Add("Avtor");
            R.Tables["postolbcam"].Columns.Add("God");
            Type t = i.GetType();
            R.Tables["postolbcam"].Columns.Add("sprash",t);
            R.Tables["postolbcam"].Columns.Add("ZagSort");
            R.Tables["postolbcam"].Columns.Add("AvtorSort");
            R.Tables["postolbcam"].Columns.Add("vidacha");

            DataRow ARow = R.Tables["postolbcam"].NewRow();
            string id = R.Tables["vperemeshku"].Rows[0]["IDMAIN"].ToString();
            ARow["sprash"] = R.Tables["vperemeshku"].Rows[0]["sp"];
            //string vida = R.Tables["vperemeshku"].Rows[0]["idm"].ToString();
            ARow["vidacha"] = R.Tables["vperemeshku"].Rows[0]["vida"].ToString();
            ARow["bar"] = R.Tables["vperemeshku"].Rows[0]["bar"].ToString();
            foreach (DataRow row in R.Tables["vperemeshku"].Rows)
            {
                if (id != row["IDMAIN"].ToString())
                {
                    R.Tables["postolbcam"].Rows.Add(ARow);
                    ARow = R.Tables["postolbcam"].NewRow();
                    id = row["IDMAIN"].ToString();
                    ARow["sprash"] = row["sp"];
                    //vida = row["idm"].ToString();
                    //if (vida != "")
                        //MessageBox.Show(vida);
                    ARow["vidacha"] = row["vida"].ToString();
                    ARow["bar"] = row["bar"].ToString();
                }

                switch (row["MNFIELD"].ToString() + row["MSFIELD"].ToString())
                {
                    case "200$a":
                        ARow["Zagl"] = row["PLAIN"].ToString();
                        ARow["ZagSort"] = row["SORT"].ToString();
                        break;
                    case "700$a":
                        ARow["Avtor"] = row["PLAIN"].ToString();
                        ARow["AvtorSort"] = row["SORT"].ToString();
                        break;
                    case "2100$d":
                        ARow["God"] = row["PLAIN"].ToString();
                        break;
                    case "899$c":
                        ARow["Polka"] = row["PLAIN"].ToString();
                        break;
                    case "899$w":
                        ARow["bar"] = row["PLAIN"].ToString();
                        break;
                }
            }
            R.Tables["postolbcam"].Rows.Add(ARow);

			return R.Tables["postolbcam"];
		}

        internal DataTable GetFormular(string p)
        {
            Conn.SQLDA.SelectCommand.Parameters["@IDR"].Value = p;
            Conn.SQLDA.SelectCommand.CommandText = "select zagp.PLAIN zag,zag.SORT ��������_sort,avtp.PLAIN �����,avt.SORT �����_sort, " +
                                                   " B.INV inv,zag.IDMAIN idmain, B.DATE_ISSUE issue,B.DATE_VOZV vozv,B.DATE_FACT_VOZV fact,  " +
                                                   " B.IDMAIN zkid,B.ID zi,B.PENALTY penalty,B.REMPENALTY rempenalty,B.BAR bar " +
                                                   " from Reservation_R..ISSUED B  " +
                                                   " left join BJVVV..DATAEXT A on B.BAR collate Cyrillic_General_CI_AI = A.SORT and A.MNFIELD = 899 and A.MSFIELD = '$w' " +
                                                   " left join BJVVV..DATAEXT zag on " +
                                                                                    " zag.MNFIELD = 200 and " +
                                                                                    " zag.MSFIELD = '$a' and " +
                                                                                    " zag.IDMAIN = B.IDMAIN_CONST " +
                                                   " left join BJVVV..DATAEXT avt on " +
                                                                                    " avt.MNFIELD = 700 and " +
                                                                                    " avt.MSFIELD = '$a' " +
                                                                                    " and avt.IDMAIN = B.IDMAIN_CONST " +
                                                   " left join BJVVV..DATAEXTPLAIN zagp on zagp.IDDATAEXT = zag.ID " +
                                                   " left join BJVVV..DATAEXTPLAIN avtp on avtp.IDDATAEXT = avt.ID " +
                                                   " where B.IDREADER = @IDR " +
                                                   " and (B.IDMAIN != 0 or (B.IDMAIN = 0 and B.PENALTY = 1))";

            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet R = new DataSet();
            R.Tables.Add("form");
            int i = Conn.SQLDA.Fill(R.Tables["form"]);
            
            return R.Tables["form"];
        }

        internal bool Prolong(int x, string idb, string inv)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select * from [Reservation_R].[dbo].ISSUED where IDMAIN = '" + idb + "' and INV = '" + inv + "'";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            if (Conn.SQLDA.SelectCommand.Connection.State == ConnectionState.Closed)
            {
                Conn.SQLDA.SelectCommand.Connection.Open();
            }
            Conn.SQLDA.UpdateCommand = new SqlCommand();
            Conn.SQLDA.UpdateCommand.Connection = Conn.ZakazCon;
            if (Conn.SQLDA.UpdateCommand.Connection.State == ConnectionState.Closed)
            {
                Conn.SQLDA.UpdateCommand.Connection.Open();
            }

            DataSet B = new DataSet();
            int i = Conn.SQLDA.Fill(B, "t");

            
            DateTime dt = DateTime.Parse(B.Tables[0].Rows[0]["DATE_VOZV"].ToString()).AddDays(x);
            bool result = false;
            if (dt >= DateTime.Parse(DateTime.Now.ToShortDateString()))
            {
                result = false;
                Conn.SQLDA.UpdateCommand.CommandText = "update Reservation_R..ISSUED set PENALTY = 'false', DATE_PROLONG = '" + DateTime.Today.ToString("yyyyMMdd") + "', DATE_VOZV =  '" + dt.ToString("yyyyMMdd") + "' where IDMAIN = '" + idb + "' and INV = '" + inv + "'";
            }
            else
            {
                result = true;
                Conn.SQLDA.UpdateCommand.CommandText = "update Reservation_R..ISSUED set PENALTY = 'true', DATE_PROLONG = '" + DateTime.Today.ToString("yyyyMMdd") + "', DATE_VOZV =  '" + dt.ToString("yyyyMMdd") + "' where IDMAIN = '" + idb + "' and INV = '" + inv + "'";
            }
            Conn.SQLDA.UpdateCommand.ExecuteNonQuery();
            Conn.SQLDA.UpdateCommand.Connection.Close();
            Conn.SQLDA.SelectCommand.Connection.Close();
            return result;
        }

        internal void SetPenalty(string idr)
        {
            Conn.SQLDA.SelectCommand.Parameters["@IDR"].Value = idr;
            Conn.SQLDA.SelectCommand.CommandText = "select * from [Reservation_R].[dbo].ISSUED where IDREADER = @IDR";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            if (Conn.SQLDA.SelectCommand.Connection.State == ConnectionState.Closed)
            {
                Conn.SQLDA.SelectCommand.Connection.Open();
            }
            DataSet B = new DataSet();
            int i = Conn.SQLDA.Fill(B, "t");
            Conn.SQLDA.UpdateCommand = null;
            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(Conn.SQLDA);
            foreach (DataRow row in B.Tables["t"].Rows)
            {
                bool isReturned = (int)row["IDMAIN"] == 0;
                bool isFactReturned = (row["DATE_FACT_VOZV"].ToString() != string.Empty);//�� �������� ���� ������ ��� ��������� ��������
                DateTime vozv = (DateTime)row["DATE_VOZV"];//����� �� ������������ � ����� ������ ��� ���� ������ ��� ���� ����� ����� ��������
                
                bool isRetLater = (isFactReturned)? (DateTime)row["DATE_VOZV"] < (DateTime)row["DATE_FACT_VOZV"] : true;
                bool isTimeOver = (DateTime)row["DATE_VOZV"] < DateTime.Now;
                bool wasPenalty = (bool)row["REMPENALTY"] ;
                bool nowPenalty = (bool)row["PENALTY"] ;

                if ( (!isFactReturned || isRetLater) && isTimeOver && !wasPenalty && !nowPenalty)
                //if ((((row["DATE_FACT_VOZV"].ToString() == null) && (DateTime.Parse(row["DATE_VOZV"].ToString()) < DateTime.Now)) || ((DateTime.Parse(row["DATE_VOZV"].ToString()) < DateTime.Parse(row["DATE_FACT_VOZV"].ToString()) && (row["REMPENALTY"].ToString().ToLower() == "false")))))// ����� ��������
                //if ((row["IDMAIN"].ToString() != "0") && ((row["DATE_FACT_VOZV"].ToString() == string.Empty) || (DateTime.Parse(row["DATE_VOZV"].ToString()) < DateTime.Parse(row["DATE_FACT_VOZV"].ToString()))) && (DateTime.Parse(row["DATE_VOZV"].ToString()) < DateTime.Now) && (!(bool)row["REMPENALTY"]) && (!(bool)row["PENALTY"]))
                {
                    row["PENALTY"] = true;
                    row["REMPENALTY"] = false;
                    //row["REMPENALTY"] = true;
                }
            }
            
            Conn.SQLDA.Update(B.Tables["t"]);
            Conn.SQLDA.SelectCommand.Connection.Close();
        }
        internal void SetPenaltyAll()
        {
            //Conn.SQLDA.SelectCommand.Parameters["@IDR"].Value = idr;
            Conn.SQLDA.SelectCommand.CommandText = "select * from [Reservation_R].[dbo].ISSUED";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            if (Conn.SQLDA.SelectCommand.Connection.State == ConnectionState.Closed)
            {
                Conn.SQLDA.SelectCommand.Connection.Open();
            }
            DataSet B = new DataSet();
            int i = Conn.SQLDA.Fill(B, "t");
            Conn.SQLDA.UpdateCommand = null;
            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(Conn.SQLDA);
            foreach (DataRow row in B.Tables["t"].Rows)
            {
                bool isReturned = (int)row["IDMAIN"] == 0;
                bool isFactReturned = (row["DATE_FACT_VOZV"].ToString() != string.Empty);//�� �������� ���� ������ ��� ��������� ��������
                DateTime vozv = (DateTime)row["DATE_VOZV"];//����� �� ������������ � ����� ������ ��� ���� ������ ��� ���� ����� ����� ��������

                bool isRetLater = (isFactReturned) ? (DateTime)row["DATE_VOZV"] < (DateTime)row["DATE_FACT_VOZV"] : true;
                bool isTimeOver = (DateTime)row["DATE_VOZV"] < DateTime.Now;
                bool wasPenalty = (bool)row["REMPENALTY"];
                bool nowPenalty = (bool)row["PENALTY"];

                if ((!isFactReturned || isRetLater) && isTimeOver && !wasPenalty && !nowPenalty)
                //if ((((row["DATE_FACT_VOZV"].ToString() == null) && (DateTime.Parse(row["DATE_VOZV"].ToString()) < DateTime.Now)) || ((DateTime.Parse(row["DATE_VOZV"].ToString()) < DateTime.Parse(row["DATE_FACT_VOZV"].ToString()) && (row["REMPENALTY"].ToString().ToLower() == "false")))))// ����� ��������
                //if ((row["IDMAIN"].ToString() != "0") && ((row["DATE_FACT_VOZV"].ToString() == string.Empty) || (DateTime.Parse(row["DATE_VOZV"].ToString()) < DateTime.Parse(row["DATE_FACT_VOZV"].ToString()))) && (DateTime.Parse(row["DATE_VOZV"].ToString()) < DateTime.Now) && (!(bool)row["REMPENALTY"]) && (!(bool)row["PENALTY"]))
                {
                    row["PENALTY"] = true;
                    row["REMPENALTY"] = false;
                    //row["REMPENALTY"] = true;
                }
            }

            int rn = Conn.SQLDA.Update(B.Tables["t"]);
            Conn.SQLDA.SelectCommand.Connection.Close();
        }

        internal void RemPenalty(string zid)
        {
            /*Conn.SQLDA.SelectCommand.CommandText = "select * from [Reservation_R].[dbo].ISSUED where ID = '" + zid + "'";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            //SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(Conn.SQLDA);
            DataSet B = new DataSet();
            int i = Conn.SQLDA.Fill(B);
            B.Tables[0].Rows[0]["REMPENALTY"] = true;
            B.Tables[0].Rows[0]["PENALTY"] = false;
            Conn.SQLDA.Update(B.Tables[0]);
            */
            Conn.SQLDA.UpdateCommand = new SqlCommand();
            Conn.SQLDA.UpdateCommand.Connection = Conn.ZakazCon;
            if (Conn.SQLDA.UpdateCommand.Connection.State == ConnectionState.Closed)
            {
                Conn.SQLDA.UpdateCommand.Connection.Open();
            }
            Conn.SQLDA.UpdateCommand.CommandText = "update Reservation_R..ISSUED set PENALTY = 'false', REMPENALTY = 'true' where ID = " + zid;
            Conn.SQLDA.UpdateCommand.ExecuteNonQuery();
            Conn.SQLDA.UpdateCommand.Connection.Close();

            //throw new Exception("The method or operation is not implemented.");
        }

        internal int GetBookCountForReader(string idr)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select * from Reservation_R..ISSUED where IDREADER = '" + idr + "' and IDMAIN != 0 and REMPENALTY = 'false'";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(Conn.SQLDA);
            DataSet B = new DataSet();
            return Conn.SQLDA.Fill(B);
        }

        internal void SetReaderAbonement(string idr, string abt)
        {
            Conn.ReaderDA.SelectCommand.CommandText = "select * from [Readers].[dbo].Main where NumberReader = " + idr;
            Conn.ReaderDA.SelectCommand.Connection = Conn.ReadersCon;

            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(Conn.ReaderDA);
            DataSet B = new DataSet();
            int i = Conn.ReaderDA.Fill(B);
            B.Tables[0].Rows[0]["AbonementType"] = abt;
            Conn.ReaderDA.Update(B);
        }

        internal object GetDebtors(DateTime start, DateTime finish)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select A.DATE_VOZV,A.IDREADER,B.FamilyName,B.[Name],B.FatherName," +
                    " C.PLAIN,D.PLAIN,CC.SORT,DD.SORT," +
                    " (case when B.LiveEmail is null and B.RegistrationEmail is null and B.WorkEmail is null then 'false' else 'true' end) email" +
                    " from Reservation_R..ISSUED A" +
                    " left join Readers..Main B on A.IDREADER = B.NumberReader" +
                    " left join BJVVV..DATAEXT CC on A.IDMAIN = CC.IDMAIN and CC.MNFIELD = 200 and CC.MSFIELD = '$a'" +
                    " left join BJVVV..DATAEXT DD on A.IDMAIN = DD.IDMAIN and DD.MNFIELD = 700 and DD.MSFIELD = '$a'" +
                    " left join BJVVV..DATAEXTPLAIN C on C.IDDATAEXT = CC.ID" +
                    " left join BJVVV..DATAEXTPLAIN D on D.IDDATAEXT = DD.ID" +
                    " where " +
                    " A.DATE_VOZV between '" + start.ToString("yyyyMMdd") + "' and '" + finish.ToString("yyyyMMdd") + "'" +
                    " and A.IDMAIN != 0 and A.PENALTY = 1";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet DS = new DataSet();
            int i = Conn.SQLDA.Fill(DS, "t");
            return DS.Tables[0];
            /*Conn.SQLDA.SelectCommand.CommandText = "select X.IDMAIN, X.PLAIN, Y.SORT, Y.MNFIELD, Z.DATE_VOZV, Z.IDREADER " +
                                                   " from BJVVV..DATAEXTPLAIN X join BJVVV..DATAEXT Y on Y.ID=X.IDDATAEXT " +
                                                   " join Reservation_R..ISSUED Z on Z.IDMAIN = Y.IDMAIN " +
                                                   " where (Z.DATE_VOZV between '" + start.ToString("yyyyMMdd") + "' and '"
                                                   + finish.ToString("yyyyMMdd") + "'  and PENALTY = 'true')" +
                                                   "  and ((Y.MNFIELD = 200 and Y.MSFIELD = '$a') or (Y.MSFIELD = '$a' and Y.MNFIELD = 700)) " +
                                                   " order by X.IDMAIN";
            //Conn.SQLDA.SelectCommand.CommandText = "select DATE_VOZV, IDREADER from ZAKAZ where IDMAIN <> 0 and DATE_VOZV < '11.11.2008'"; //" + DateTime.Now.ToString("MM/dd/yyyy") + "'";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet R = new DataSet();
            DataSet D = new DataSet();
            R.Tables.Add("vperemeshku");
            R.Tables.Add("distinct");
            int i = Conn.SQLDA.Fill(R.Tables["vperemeshku"]);
            Conn.SQLDA.SelectCommand.CommandText = "select DATE_VOZV, IDREADER from Reservation_R..ISSUED where IDMAIN <> 0 and DATE_VOZV < '" + DateTime.Now.ToString("yyyyMMdd") + "' order by IDMAIN";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            i = Conn.SQLDA.Fill(R.Tables["distinct"]);

            R.Tables.Add("postolbcam");
            R.Tables["postolbcam"].Columns.Add("date");
            R.Tables["postolbcam"].Columns.Add("num");
            //R.Tables["postolbcam"].Columns["num"].DataType = typeof(int);
            R.Tables["postolbcam"].Columns.Add("fam");
            R.Tables["postolbcam"].Columns.Add("name");
            R.Tables["postolbcam"].Columns.Add("secname");
            R.Tables["postolbcam"].Columns.Add("Zagl");
            R.Tables["postolbcam"].Columns.Add("Avtor");
            R.Tables["postolbcam"].Columns.Add("ZagSort");
            R.Tables["postolbcam"].Columns.Add("AvtorSort");
            R.Tables["postolbcam"].Columns.Add("Email");
            R.Tables["postolbcam"].Columns["date"].DataType = typeof(DateTime);
            R.Tables["postolbcam"].Columns["Email"].DataType = typeof(bool);

            DataRow ARow = R.Tables["postolbcam"].NewRow();
            string id = R.Tables["vperemeshku"].Rows[0]["IDMAIN"].ToString();
            ARow["date"] = DateTime.Parse(R.Tables["vperemeshku"].Rows[0]["DATE_VOZV"].ToString()).ToString();
            if (R.Tables["vperemeshku"].Rows[0]["IDREADER"].ToString() == "-1")
            {
                ARow["num"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                ARow["fam"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                ARow["name"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                ARow["secname"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                ARow["Email"] = false;
            }
            else
            {
                Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName, "+
                                                          " (case when LiveEmail is null and RegistrationEmail is null and WorkEmail is null then 'false' else 'true' end) email"   +
                                                          "  from main where NumberReader = "
                                                          + R.Tables["vperemeshku"].Rows[0]["IDREADER"].ToString();
                i = Conn.ReaderDA.Fill(D);
                ARow["num"] = D.Tables[0].Rows[0]["NumberReader"].ToString();
                ARow["fam"] = D.Tables[0].Rows[0]["FamilyName"].ToString();
                ARow["name"] = D.Tables[0].Rows[0]["Name"].ToString();
                ARow["secname"] = D.Tables[0].Rows[0]["FatherName"].ToString();
                ARow["Email"] = D.Tables[0].Rows[0]["email"];
            }
            //ARow["sprash"] = R.Tables["vperemeshku"].Rows[0]["sp"].ToString();
            foreach (DataRow row in R.Tables["vperemeshku"].Rows)
            {
                if (id != row["IDMAIN"].ToString())
                {
                    D.Clear();
                    R.Tables["postolbcam"].Rows.Add(ARow);
                    ARow = R.Tables["postolbcam"].NewRow();
                    id = row["IDMAIN"].ToString();
                    ARow["date"] = DateTime.Parse(row["DATE_VOZV"].ToString()).ToString("yyyy-MM-dd");
                    if (row["IDREADER"].ToString() == "-1")
                    {
                        ARow["num"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                        ARow["fam"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                        ARow["name"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                        ARow["secname"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                        ARow["Email"] = false;
                    }
                    else
                    {
                        Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName, " +
                                                                  " (case when LiveEmail is null and RegistrationEmail is null and WorkEmail is null then 'false' else 'true' end) email" +
                                                                  "  from main where NumberReader = "
                                                                  + R.Tables["vperemeshku"].Rows[0]["IDREADER"].ToString();
                        i = Conn.ReaderDA.Fill(D);
                        ARow["num"] = D.Tables[0].Rows[0]["NumberReader"].ToString();
                        ARow["fam"] = D.Tables[0].Rows[0]["FamilyName"].ToString();
                        ARow["name"] = D.Tables[0].Rows[0]["Name"].ToString();
                        ARow["secname"] = D.Tables[0].Rows[0]["FatherName"].ToString();
                        ARow["Email"] = D.Tables[0].Rows[0]["email"];
                    }
                }

                switch (row["MNFIELD"].ToString())
                {
                    case "200":
                        ARow["Zagl"] = row["PLAIN"].ToString();
                        ARow["ZagSort"] = row["SORT"].ToString();
                        break;
                    case "700":
                        ARow["Avtor"] = row["PLAIN"].ToString();
                        ARow["AvtorSort"] = row["SORT"].ToString();
                        break;
                }
            }
            R.Tables["postolbcam"].Rows.Add(ARow);

            return R.Tables["postolbcam"];*/

        }
        internal object GetDebtorsFCT(DateTime start, DateTime finish)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select A.DATE_VOZV,A.IDREADER,B.FamilyName,B.[Name],B.FatherName," +
                " C.PLAIN,D.PLAIN,CC.SORT,DD.SORT," +
                " (case when B.LiveEmail is null and B.RegistrationEmail is null and B.WorkEmail is null then 'false' else 'true' end) email" +
                " from Reservation_R..ISSUED A" +
                " left join Readers..Main B on A.IDREADER = B.NumberReader" +
                " left join BJVVV..DATAEXT CC on A.IDMAIN_CONST = CC.IDMAIN and CC.MNFIELD = 200 and CC.MSFIELD = '$a'" +
                " left join BJVVV..DATAEXT DD on A.IDMAIN_CONST = DD.IDMAIN and DD.MNFIELD = 700 and DD.MSFIELD = '$a'" +
                " left join BJVVV..DATAEXTPLAIN C on C.IDDATAEXT = CC.ID" +
                " left join BJVVV..DATAEXTPLAIN D on D.IDDATAEXT = DD.ID" +
                " where " +
                " A.DATE_FACT_VOZV between '" + start.ToString("yyyyMMdd") + "' and '" + finish.ToString("yyyyMMdd") + "'" +
                " and A.IDMAIN = 0 and A.PENALTY = 1";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet DS = new DataSet();
            int i = Conn.SQLDA.Fill(DS, "t");
            return DS.Tables[0];
            /*Conn.SQLDA.SelectCommand.CommandText = "select X.IDMAIN, X.PLAIN, Y.SORT, Y.MNFIELD, Z.DATE_VOZV, Z.IDREADER " +
                                                   " from BJVVV..DATAEXTPLAIN X join BJVVV..DATAEXT Y on Y.ID=X.IDDATAEXT " +
                                                   " join Reservation_R..ISSUED Z on Z.IDMAIN_CONST = Y.IDMAIN " +
                                                   " where (Z.DATE_FACT_VOZV between '" + start.ToString("yyyyMMdd") + "' and '" + finish.ToString("yyyyMMdd") + "'  and PENALTY = 'true') " +
                                                   " and ((Y.MNFIELD = 200 and Y.MSFIELD = '$a') or (Y.MSFIELD = '$a' and Y.MNFIELD = 700)) " +
                                                   " order by X.IDMAIN";
            //Conn.SQLDA.SelectCommand.CommandText = "select DATE_VOZV, IDREADER from ZAKAZ where IDMAIN <> 0 and DATE_VOZV < '11.11.2008'"; //" + DateTime.Now.ToString("MM/dd/yyyy") + "'";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet R = new DataSet();
            DataSet D = new DataSet();
            R.Tables.Add("vperemeshku");
            R.Tables.Add("distinct");
            int i = Conn.SQLDA.Fill(R.Tables["vperemeshku"]);
            Conn.SQLDA.SelectCommand.CommandText = "select DATE_VOZV, IDREADER from Reservation_R..ISSUED where IDMAIN <> 0 and DATE_VOZV < '" + DateTime.Now.ToString("yyyyMMdd") + "' order by IDMAIN";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            i = Conn.SQLDA.Fill(R.Tables["distinct"]);

            R.Tables.Add("postolbcam");
            R.Tables["postolbcam"].Columns.Add("date");
            R.Tables["postolbcam"].Columns.Add("num");
            R.Tables["postolbcam"].Columns.Add("fam");
            R.Tables["postolbcam"].Columns.Add("name");
            R.Tables["postolbcam"].Columns.Add("secname");
            R.Tables["postolbcam"].Columns.Add("Zagl");
            R.Tables["postolbcam"].Columns.Add("Avtor");
            R.Tables["postolbcam"].Columns.Add("ZagSort");
            R.Tables["postolbcam"].Columns.Add("AvtorSort");
            R.Tables["postolbcam"].Columns["date"].DataType = typeof(DateTime);

            DataRow ARow = R.Tables["postolbcam"].NewRow();
            string id = R.Tables["vperemeshku"].Rows[0]["IDMAIN"].ToString();
            ARow["date"] = DateTime.Parse(R.Tables["vperemeshku"].Rows[0]["DATE_VOZV"].ToString()).ToString();
            if (R.Tables["vperemeshku"].Rows[0]["IDREADER"].ToString() == "-1")
            {
                ARow["num"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                ARow["fam"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                ARow["name"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                ARow["secname"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
            }
            else
            {
                Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName from main where NumberReader = " + R.Tables["vperemeshku"].Rows[0]["IDREADER"].ToString();
                i = Conn.ReaderDA.Fill(D);
                ARow["num"] = D.Tables[0].Rows[0]["NumberReader"].ToString();
                ARow["fam"] = D.Tables[0].Rows[0]["FamilyName"].ToString();
                ARow["name"] = D.Tables[0].Rows[0]["Name"].ToString();
                ARow["secname"] = D.Tables[0].Rows[0]["FatherName"].ToString();
            }
            //ARow["sprash"] = R.Tables["vperemeshku"].Rows[0]["sp"].ToString();
            foreach (DataRow row in R.Tables["vperemeshku"].Rows)
            {
                if (id != row["IDMAIN"].ToString())
                {
                    D.Clear();
                    R.Tables["postolbcam"].Rows.Add(ARow);
                    ARow = R.Tables["postolbcam"].NewRow();
                    id = row["IDMAIN"].ToString();
                    ARow["date"] = DateTime.Parse(row["DATE_VOZV"].ToString()).ToString("yyyy-MM-dd");
                    if (row["IDREADER"].ToString() == "-1")
                    {
                        ARow["num"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                        ARow["fam"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                        ARow["name"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                        ARow["secname"] = "�������� �� ������ ���� �� ��������� � ������������ � �����.";
                    }
                    else
                    {
                        Conn.ReaderDA.SelectCommand.CommandText = "select NumberReader, BarCode, FamilyName, Name, FatherName from main where NumberReader = " + row["IDREADER"].ToString();
                        i = Conn.ReaderDA.Fill(D);
                        ARow["num"] = D.Tables[0].Rows[0]["NumberReader"].ToString();
                        ARow["fam"] = D.Tables[0].Rows[0]["FamilyName"].ToString();
                        ARow["name"] = D.Tables[0].Rows[0]["Name"].ToString();
                        ARow["secname"] = D.Tables[0].Rows[0]["FatherName"].ToString();
                    }
                }

                switch (row["MNFIELD"].ToString())
                {
                    case "200":
                        ARow["Zagl"] = row["PLAIN"].ToString();
                        ARow["ZagSort"] = row["SORT"].ToString();
                        break;
                    case "700":
                        ARow["Avtor"] = row["PLAIN"].ToString();
                        ARow["AvtorSort"] = row["SORT"].ToString();
                        break;
                }
            }
            R.Tables["postolbcam"].Rows.Add(ARow);

            return R.Tables["postolbcam"];*/
        }

        internal void InsertActionISSUED(dbReader reader,dbBook book)
        {
            
            Conn.SQLDA.InsertCommand = new SqlCommand();
            Conn.SQLDA.InsertCommand.Connection = Conn.ZakazCon;
            if (Conn.ZakazCon.State != ConnectionState.Open) Conn.ZakazCon.Open();
            Conn.SQLDA.InsertCommand.CommandText = "insert into Reservation_R..ABONEMENTACTIONS (ACTIONTYPE,BAR,IDEMP,IDREADER,DATEACT) " +
                                                    " values (@ACTIONTYPE,@BAR,@IDEMP,@IDREADER,@DATEACT)";
            Conn.SQLDA.InsertCommand.Parameters.Add("ACTIONTYPE", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("BAR", SqlDbType.NVarChar);
            Conn.SQLDA.InsertCommand.Parameters.Add("IDEMP", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("IDREADER", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("DATEACT", SqlDbType.DateTime);
            Conn.SQLDA.InsertCommand.Parameters["ACTIONTYPE"].Value = 1;
            Conn.SQLDA.InsertCommand.Parameters["BAR"].Value = book.barcode;
            Conn.SQLDA.InsertCommand.Parameters["IDEMP"].Value = this.F1.EmpID;
            Conn.SQLDA.InsertCommand.Parameters["IDREADER"].Value = reader.id;
            Conn.SQLDA.InsertCommand.Parameters["DATEACT"].Value = DateTime.Now;
            try
            {
                Conn.SQLDA.InsertCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ". �� ��������� ���������������� �������� - ������. ���������� � ������������.");
            }
        }

        internal void InsertActionRETURNED(dbReader reader, dbBook book)
        {
            Conn.SQLDA.InsertCommand = new SqlCommand();
            Conn.SQLDA.InsertCommand.Connection = Conn.ZakazCon;
            if (Conn.ZakazCon.State != ConnectionState.Open) Conn.ZakazCon.Open();
            Conn.SQLDA.InsertCommand.CommandText = "insert into Reservation_R..ABONEMENTACTIONS (ACTIONTYPE,BAR,IDEMP,IDREADER,DATEACT) " +
                                                    " values (@ACTIONTYPE,@BAR,@IDEMP,@IDREADER,@DATEACT)";
            Conn.SQLDA.InsertCommand.Parameters.Add("ACTIONTYPE", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("BAR", SqlDbType.NVarChar);
            Conn.SQLDA.InsertCommand.Parameters.Add("IDEMP", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("IDREADER", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("DATEACT", SqlDbType.DateTime);
            Conn.SQLDA.InsertCommand.Parameters["ACTIONTYPE"].Value = 2;
            Conn.SQLDA.InsertCommand.Parameters["BAR"].Value = book.barcode;
            Conn.SQLDA.InsertCommand.Parameters["IDEMP"].Value = this.F1.EmpID;
            Conn.SQLDA.InsertCommand.Parameters["IDREADER"].Value = book.rid;
            Conn.SQLDA.InsertCommand.Parameters["DATEACT"].Value = DateTime.Now;
            try
            {
                Conn.SQLDA.InsertCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ". �� ��������� ���������������� �������� - �������. ���������� � ������������.");
            }
        }

        internal void InsertActionProlong(dbReader reader, dbBook book)
        {
            Conn.SQLDA.InsertCommand = new SqlCommand();
            Conn.SQLDA.InsertCommand.Connection = Conn.ZakazCon;
            if (Conn.ZakazCon.State != ConnectionState.Open) Conn.ZakazCon.Open();
            Conn.SQLDA.InsertCommand.CommandText = "insert into Reservation_R..ABONEMENTACTIONS (ACTIONTYPE,BAR,IDEMP,IDREADER,DATEACT) " +
                                                    " values (@ACTIONTYPE,@BAR,@IDEMP,@IDREADER,@DATEACT)";
            Conn.SQLDA.InsertCommand.Parameters.Add("ACTIONTYPE", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("BAR", SqlDbType.NVarChar);
            Conn.SQLDA.InsertCommand.Parameters.Add("IDEMP", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("IDREADER", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("DATEACT", SqlDbType.DateTime);
            Conn.SQLDA.InsertCommand.Parameters["ACTIONTYPE"].Value = 3;
            Conn.SQLDA.InsertCommand.Parameters["BAR"].Value = book.barcode;
            Conn.SQLDA.InsertCommand.Parameters["IDEMP"].Value = this.F1.EmpID;
            Conn.SQLDA.InsertCommand.Parameters["IDREADER"].Value = reader.id;
            Conn.SQLDA.InsertCommand.Parameters["DATEACT"].Value = DateTime.Now;
            try
            {
                Conn.SQLDA.InsertCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ". �� ��������� ���������������� �������� - ���������. ���������� � ������������.");
            }
        }

        internal void InsertActionEMAIL(dbReader reader)
        {
            Conn.SQLDA.InsertCommand = new SqlCommand();
            Conn.SQLDA.InsertCommand.Connection = Conn.ZakazCon;
            if (Conn.ZakazCon.State != ConnectionState.Open) Conn.ZakazCon.Open();
            Conn.SQLDA.InsertCommand.CommandText = "insert into Reservation_R..ABONEMENTACTIONS (ACTIONTYPE,BAR,IDEMP,IDREADER,DATEACT) " +
                                                    " values (@ACTIONTYPE,@BAR,@IDEMP,@IDREADER,@DATEACT)";
            Conn.SQLDA.InsertCommand.Parameters.Add("ACTIONTYPE", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("BAR", SqlDbType.NVarChar);
            Conn.SQLDA.InsertCommand.Parameters.Add("IDEMP", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("IDREADER", SqlDbType.Int);
            Conn.SQLDA.InsertCommand.Parameters.Add("DATEACT", SqlDbType.DateTime);
            Conn.SQLDA.InsertCommand.Parameters["ACTIONTYPE"].Value = 4;
            Conn.SQLDA.InsertCommand.Parameters["BAR"].Value = "email";
            Conn.SQLDA.InsertCommand.Parameters["IDEMP"].Value = this.F1.EmpID;
            Conn.SQLDA.InsertCommand.Parameters["IDREADER"].Value = reader.id;
            Conn.SQLDA.InsertCommand.Parameters["DATEACT"].Value = DateTime.Now;
            try
            {
                Conn.SQLDA.InsertCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ". �� ��������� ���������������� �������� - �������� email. ���������� � ������������.");
            }
        }

        internal object GetActions(DateTime start, DateTime end,int userID)
        {
            //Conn.SQLDA.SelectCommand.Parameters["@IDEMP"].Value = p;
            Conn.SQLDA.SelectCommand.CommandText = "select A.ID, " +
               " (case when ACTIONTYPE = 1 then '�����' else" +
               " case when ACTIONTYPE = 2 then '������' else" +
               " case when ACTIONTYPE = 3 then '�������' else" +
               " case when ACTIONTYPE = 4 then '������� email'" +
               " end " +
               " end " +
               " end " +
               " end), case when avtp.PLAIN is null then zagp.PLAIN collate Cyrillic_General_CI_AI + "+
               " ', '+ C.SORT else avtp.PLAIN collate Cyrillic_General_CI_AI + ', ' "+
               " + zagp.PLAIN collate Cyrillic_General_CI_AI + ', '+ C.SORT end,A.IDREADER,A.DATEACT" +
               " from Reservation_R..ABONEMENTACTIONS A  " +
               " left join BJVVV..DATAEXT B on B.SORT collate Cyrillic_General_CI_AI = A.BAR and B.MNFIELD = 899 and B.MSFIELD = '$w' " +
               " left join BJVVV..DATAEXT C on C.IDDATA = B.IDDATA and C.MNFIELD = 899 and C.MSFIELD = '$p' " +
               " left join BJVVV..DATAEXT zag on " +
                                                " zag.MNFIELD = 200 and " +
                                                " zag.MSFIELD = '$a' and " +
                                                " zag.IDMAIN = B.IDMAIN " +
               " left join BJVVV..DATAEXT avt on " +
                                                " avt.MNFIELD = 700 and " +
                                                " avt.MSFIELD = '$a' " +
                                                " and avt.IDMAIN = B.IDMAIN " +
               " left join BJVVV..DATAEXTPLAIN zagp on zagp.IDDATAEXT = zag.ID " +
               " left join BJVVV..DATAEXTPLAIN avtp on avtp.IDDATAEXT = avt.ID " +
               " where A.IDEMP = "+userID.ToString()+
               " and A.DATEACT between '"+start.ToString("dd.MM.yyyy")+"' and '"+end.AddDays(1).ToString("dd.MM.yyyy")+"'" ;

            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet R = new DataSet();
            int i = Conn.SQLDA.Fill(R);
            return R.Tables[0];
        }

        internal string GetLastDateEmail(string p)
        {
            Conn.SQLDA.SelectCommand.CommandText = "select max(DATEACT) from Reservation_R..ABONEMENTACTIONS where IDREADER = '" + p + "' and ACTIONTYPE = 4";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(Conn.SQLDA);
            DataSet B = new DataSet();
            int t = Conn.SQLDA.Fill(B,"t");
            string ret = (B.Tables[0].Rows[0][0] == DBNull.Value) ? "<���>" : B.Tables[0].Rows[0][0].ToString();
            return ret;
                  
        }

        internal DataTable getOperators()
        {
            Conn.SQLDA.SelectCommand.CommandText = "select ID,[NAME] from BJVVV..USERS where DEPT = 47";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet B = new DataSet();
            int t = Conn.SQLDA.Fill(B, "t");
            return B.Tables["t"];
        }

        internal void DeleteExceededOrders()//������� ������ � ����������� �� � �������, ������� ������ 3 ���� � ������� ������ � �����
        {
            Conn.SQLDA.SelectCommand.CommandText = "select * from Reservation_O..Orders where DATEDIFF(day,Start_Date,getdate()) >3";
            Conn.SQLDA.SelectCommand.Connection = Conn.ZakazCon;
            DataSet B = new DataSet();
            int t = Conn.SQLDA.Fill(B, "t");
            foreach (DataRow r in B.Tables["t"].Rows)
            {
                Conn.SQLDA.SelectCommand.CommandText = "select * from BJVVV..DATAEXT where MNFIELD = 899 and MSFIELD = '$a' and IDDATA = " + r["IDDATA"].ToString();
                t = Conn.SQLDA.Fill(B, "ab");
                if (t == 0)//�� ������ ����
                    continue;
                if (t > 1)//�� ������ ����
                    continue;
                if (B.Tables["ab"].Rows[0]["SORT"].ToString().Contains("���������"))
                {
                    Conn.SQLDA.InsertCommand = new SqlCommand();
                    Conn.SQLDA.InsertCommand.Connection = Conn.ZakazCon;
                    Conn.SQLDA.InsertCommand.Connection.Open();
                    Conn.SQLDA.InsertCommand.CommandText = "insert into Reservation_O..OrdHis " +
                                    " select ID_Reader,ID_Book_EC,ID_Book_CC, Status,Start_Date, " +
                                    " Change_Date,InvNumber,Form_Date,Duration,Who,ALGIDM,IDDATA,REFUSUAL " +
                                    " from Reservation_O..Orders where ID = " + r["ID"].ToString();
                    Conn.SQLDA.InsertCommand.ExecuteNonQuery();
                    Conn.SQLDA.InsertCommand.Connection.Close();


                    Conn.SQLDA.DeleteCommand = new SqlCommand();
                    Conn.SQLDA.DeleteCommand.Connection = Conn.ZakazCon;
                    if (Conn.SQLDA.DeleteCommand.Connection.State == ConnectionState.Closed)
                    {
                        Conn.SQLDA.DeleteCommand.Connection.Open();
                    }
                    Conn.SQLDA.DeleteCommand.CommandText = "delete from Reservation_O..Orders where ID = " + r["ID"].ToString();
                    Conn.SQLDA.DeleteCommand.ExecuteNonQuery();
                    if (Conn.SQLDA.DeleteCommand.Connection.State == ConnectionState.Open)
                    {
                        Conn.SQLDA.DeleteCommand.Connection.Close();
                    }
                }
            }
        }
    }
}
