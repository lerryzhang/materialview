using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using Microsoft.Office.Interop.Excel;
using System.Net;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;





namespace MaterialView
{
    public partial class Form1 : Form
    {


        public string fileName = "物料信息_" + DateTime.Now.ToString("yyyy-MM-dd");
        public string item = "";
        public string mnoyktype = "";


        public Form1()
        {

            InitializeComponent();
            db db = new db();
            SqlConnection conn = db.getConn();
            comboBox1.Items.Add("");

            conn.Open();
            SqlCommand com = new SqlCommand("select distinct itemname from pc_item ", conn);
            SqlDataReader dr = com.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(dr["itemname".ToString()]);
            }
            dr.Close();
            ListItem listItem = new ListItem();
            listItem.Text = "";
            listItem.Value = "";
            comboBox2.Items.Add(listItem);
            com = new SqlCommand("select *  from pc_mnoyk ", conn);
            dr = com.ExecuteReader();
            while (dr.Read())
            {
                listItem = new ListItem();
                listItem.Text = dr["cname"].ToString();
                listItem.Value = dr["mnotypeyk"].ToString();

                comboBox2.Items.Add(listItem);

            }
            dr.Close();
            conn.Close();

        }

        public void button1_Click(object sender, EventArgs e)
        {
            /* 
            
             string server = "127.0.0.1";
             string userName = "sa";
             string passwd = "mis_123";
             string constr = "server=" + server + ";uid=" + userName + ";pwd=" + passwd + ";database=task";
             SqlConnection conn = new SqlConnection(constr);
             conn.Open();
             SqlCommand sqlComm = new SqlCommand();
             string strCmd = "select id from t_material";
             sqlComm.Connection = conn;
             sqlComm.CommandText = strCmd;




             SqlDataReader dataReader = sqlComm.ExecuteReader();
             db db = new db();
             while (dataReader.Read())
             {
                 int id = Convert.ToInt32(dataReader["id"]);
                 Console.WriteLine(id);
                 db.setId(id);
                 db.removeMaterial();
                

             }

             conn.Close();
             conn.Dispose();



             */








            dataGridView1.DataSource = null;
            if (comboBox1.SelectedIndex > -1)
                item = comboBox1.SelectedItem.ToString();
            if (comboBox2.SelectedIndex > -1)
                mnoyktype = (comboBox2.SelectedItem as ListItem).Value.ToString();

            button1.Enabled = false;
            db testClass = new db(item, mnoyktype);
            //在testclass对象的mainThread(委托)对象上搭载两个方法，在线程中调用mainThread对象时相当于调用了这两个方法。 
            testClass.mainThread = new db.TestDelegate(RefreshTable);
            // testClass.setPos = new db.SetPos(SetTextMesssage);

            //创建一个无参数的线程,这个线程执行TestClass类中的TestFunction方法。 
            Thread testClassThread = new Thread(new ThreadStart(testClass.showValue));
            //启动线程，启动之后线程才开始执行 
            testClassThread.Start();


        }


        public void RefreshTable(DataSet ds)
        {

            if (this.dataGridView1.InvokeRequired)
            {
                //再次创建一个TestClass类的对象 
                db testclass = new db();
                //为新对象的mainThread对象搭载方法 
                testclass.mainThread = new db.TestDelegate(RefreshTable);
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i) 
                this.Invoke(testclass.mainThread, new object[] { ds });
            }
            else
            {
                dataGridView1.DataSource = ds;
                dataGridView1.DataMember = "物料信息";
                dataGridView1.Columns[9].Visible = false; 
                button1.Enabled = true;

            }


        }


        public void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        public void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

            int id = int.Parse(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["id"].Value.ToString());
            dataGridView1.Rows.Remove(dataGridView1.Rows[dataGridView1.CurrentRow.Index]);
            db testClass = new db();
            testClass.setId(id);
            testClass.Materialokid = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["materialokid"].Value.ToString();
            //在testclass对象的mainThread(委托)对象上搭载两个方法，在线程中调用mainThread对象时相当于调用了这两个方法。 
            testClass.deleteMaterial = new db.DeleteMaterial(deleteMaterial);
            // testClass.setPos = new db.SetPos(SetTextMesssage);
            //创建一个无参数的线程,这个线程执行TestClass类中的TestFunction方法。 
            Thread testClassThread = new Thread(new ThreadStart(testClass.removeMaterial));
            //启动线程，启动之后线程才开始执行 
            testClassThread.Start();
        }

        public void deleteMaterial()
        {
            MessageBox.Show("删除成功");
        }

        public void 你好ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {

                string saveFileName = "";
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt = "xls";
                saveDialog.Filter = "Excel文件|*.xls";
                saveDialog.FileName = fileName;
                saveDialog.ShowDialog();
                saveFileName = saveDialog.FileName;

                if (saveFileName.IndexOf(":") < 0) return;

                HSSFWorkbook workbook = new HSSFWorkbook();
                MemoryStream ms = new MemoryStream();

                NPOI.SS.UserModel.ISheet sheet = workbook.CreateSheet("Sheet1");

                progressBar1.Maximum = dataGridView1.RowCount + 1;
                //ProgressBar1.Minimum = 0;
                progressBar1.Value = 1;
                progressBar1.Visible = true;

                int rowCount = dataGridView1.Rows.Count;
                int colCount = dataGridView1.Columns.Count;

                NPOI.SS.UserModel.IRow dataRow = sheet.CreateRow(0);
              
                for (int i = 0; i < colCount; i++)
                {
                    NPOI.SS.UserModel.ICell cell = dataRow.CreateCell(i);
                    cell.SetCellValue( dataGridView1.Columns[i].HeaderText);
                   
                }


                for (int i = 0; i < rowCount; i++)
                {

                    dataRow = sheet.CreateRow(i+1);
                    progressBar1.Value = progressBar1.Value + 1;
                    double d = progressBar1.Value / (double)progressBar1.Maximum;
                    System.Math.Round(d, 2);
                    label1.Text = d.ToString("##%");
                    for (int j = 0; j < colCount; j++)
                    {
                        if (dataGridView1.Columns[j].Visible && dataGridView1.Rows[i].Cells[j].Value != null)
                        {
                            NPOI.SS.UserModel.ICell cell = dataRow.CreateCell(j);
                            cell.SetCellValue(dataGridView1.Rows[i].Cells[j].Value.ToString());
                        }
                    }
                }

                workbook.Write(ms);
                FileStream file = new FileStream(saveFileName, FileMode.Create);
                workbook.Write(file);
                file.Close();
                workbook = null;
                ms.Close();
                ms.Dispose();

                MessageBox.Show("导出文件成功", "提示", MessageBoxButtons.OK);
                
                
                
                
                
                /*
                
                string saveFileName = "";
                //bool fileSaved = false;  
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.DefaultExt = "xls";
                saveDialog.Filter = "Excel文件|*.xls";
                saveDialog.FileName = fileName;
                saveDialog.ShowDialog();
                saveFileName = saveDialog.FileName;
                if (saveFileName.IndexOf(":") < 0) return; //被点了取消   

                Console.WriteLine("rerurn");
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                if (xlApp == null)
                {
                    MessageBox.Show("无法创建Excel对象，可能您的机子未安装Excel");
                    return;
                }

                Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
                Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
                Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1  

                progressBar1.Maximum = dataGridView1.RowCount + 1;
                //ProgressBar1.Minimum = 0;
                progressBar1.Value = 1;
                progressBar1.Visible = true;

                //写入标题  
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    worksheet.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText;
                }
                //写入数值  
                for (int r = 0; r < dataGridView1.Rows.Count; r++)
                {

                    progressBar1.Value = progressBar1.Value + 1;
                    double d = progressBar1.Value / (double)progressBar1.Maximum;
                    System.Math.Round(d, 2);
                    label1.Text = d.ToString("##%");
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        worksheet.Cells[r + 2, i + 1] = dataGridView1.Rows[r].Cells[i].Value;

                    }
                    System.Windows.Forms.Application.DoEvents();
                    Console.WriteLine("{0} ", r);


                }
                worksheet.Columns.EntireColumn.AutoFit();

                if (saveFileName != "")
                {
                    try
                    {
                        workbook.Saved = true;
                        workbook.SaveCopyAs(saveFileName);
                        //fileSaved = true;  
                    }
                    catch (Exception ex)
                    {
                        //fileSaved = false;  
                        MessageBox.Show("导出文件时出错,文件可能正被打开！\n" + ex.Message);
                    }

                }

                xlApp.Quit();
                GC.Collect();//强行销毁   
                MessageBox.Show("导出文件成功", "提示", MessageBoxButtons.OK);
               */
                progressBar1.Visible = false;
                label1.Visible = false;
            }
            else
            {
                MessageBox.Show("报表为空,无表格需要导出", "提示", MessageBoxButtons.OK);
            }




        }

        private void progressBar1_Click_1(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }







        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {



            button3.Enabled = false;
            dataGridView2.DataSource = null;
            string startdate = dateTimePicker1.Value.ToString("yyyy/MM/dd");
            string enddate = dateTimePicker2.Value.ToString("yyyy/MM/dd");
            db testClass = new db();
            testClass.setStartDate(startdate);
            testClass.setEndDate(enddate);
            //在testclass对象的mainThread(委托)对象上搭载两个方法，在线程中调用mainThread对象时相当于调用了这两个方法。 
            testClass.bomDelegate = new db.BomDelegate(RefreshBom);
            // testClass.setPos = new db.SetPos(SetTextMesssage);

            //创建一个无参数的线程,这个线程执行TestClass类中的TestFunction方法。 
            Thread testClassThread = new Thread(new ThreadStart(testClass.showBom));
            //启动线程，启动之后线程才开始执行 
            testClassThread.Start();
        }



        public void RefreshBom(DataSet ds)
        {

            if (this.dataGridView2.InvokeRequired)
            {
                //再次创建一个TestClass类的对象 
                db testclass = new db();
                //为新对象的mainThread对象搭载方法 
                testclass.bomDelegate = new db.BomDelegate(RefreshBom);
                //this指窗体，在这调用窗体的Invoke方法，也就是用窗体的创建线程来执行mainThread对象委托的方法，再加上需要的参数(i) 
                this.Invoke(testclass.bomDelegate, new object[] { ds });
            }
            else
            {
                dataGridView2.DataSource = ds;
                dataGridView2.DataMember = "BOM信息";
                button3.Enabled = true;

            }

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }






        private void dataGridView1_CellMouseDown_1(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    //若行已是选中状态就不再进行设置
                    if (dataGridView1.Rows[e.RowIndex].Selected == false)
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[e.RowIndex].Selected = true;
                    }
                    //只选中一行时设置活动单元格
                    if (dataGridView1.SelectedRows.Count == 1)
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    }
                    //弹出操作菜单
                    contextMenuStrip2.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }




        private void dataGridView2_CellMouseDown_1(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    //若行已是选中状态就不再进行设置
                    if (dataGridView2.Rows[e.RowIndex].Selected == false)
                    {
                        dataGridView2.ClearSelection();
                        dataGridView2.Rows[e.RowIndex].Selected = true;
                    }
                    //只选中一行时设置活动单元格
                    if (dataGridView2.SelectedRows.Count == 1)
                    {
                        dataGridView2.CurrentCell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    }
                    //弹出操作菜单
                    contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void 下载文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dccfile = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["dccfile"].Value.ToString();
            string dccname = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["dccname"].Value.ToString();
            SaveFileDialog ofd = new SaveFileDialog();
            //打开时指定默认路径
            ofd.InitialDirectory = @"E:";
            ofd.FileName = dccname;
            //如果用户点击确定
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileStream FStream = new FileStream(ofd.FileName, FileMode.Create);
                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create("http://140.207.75.118/down.asp?path=dcc&file=" + dccfile + "&filename=" + dccname);
                //设置Range值
                //向服务器请求,获得服务器的回应数据流
                Stream myStream = myRequest.GetResponse().GetResponseStream();
                //定义一个字节数据
                byte[] btContent = new byte[1024];
                int intSize = 0;
                intSize = myStream.Read(btContent, 0, 1024);
                while (intSize > 0)
                {
                    FStream.Write(btContent, 0, intSize);
                    intSize = myStream.Read(btContent, 0, 1024);
                }
                //关闭流
                FStream.Close();
                myStream.Close();
                MessageBox.Show("文件下载成功");
            }

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.Show();

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {

        }




    }
}
