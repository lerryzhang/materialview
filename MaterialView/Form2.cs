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

namespace MaterialView
{
    public partial class Form2 : Form
    {
        private string item = "";
        private string type = "";
        private string supplier = "";
        private string mno = "";
        private string ykmno = "";
        private string suppliermno = "";
        private string content = "";


        public Form2()
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
                comboBox2.Items.Add(dr["itemname".ToString()]);
            }
            dr.Close();
            ListItem listItem = new ListItem();
            listItem.Text = "";
            listItem.Value = "";
            comboBox1.Items.Add(listItem);
            com = new SqlCommand("select *  from pc_mnoyk ", conn);
            dr = com.ExecuteReader();
            while (dr.Read())
            {
                listItem = new ListItem();
                listItem.Text = dr["cname"].ToString();
                listItem.Value = dr["mnotypeyk"].ToString();

                comboBox1.Items.Add(listItem);

            }
            dr.Close();


            listItem = new ListItem();
            listItem.Text = "";
            listItem.Value = "";
            comboBox3.Items.Add(listItem);
            com = new SqlCommand("select *  from pc_supplier ", conn);
            dr = com.ExecuteReader();
            while (dr.Read())
            {
                listItem = new ListItem();
                listItem.Text = dr["corp"].ToString();
                listItem.Value = dr["id"].ToString();

                comboBox3.Items.Add(listItem);

            }
            dr.Close();
            conn.Close();


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex > -1)
                item = comboBox2.SelectedItem.ToString();
            if (comboBox1.SelectedIndex > -1)
                type = (comboBox1.SelectedItem as ListItem).Value.ToString();
            if (comboBox3.SelectedIndex > -1)
                supplier = (comboBox3.SelectedItem as ListItem).Value.ToString();
            mno = textBox1.Text;
            ykmno = textBox2.Text;
            suppliermno = textBox3.Text;
            content = textBox4.Text;
            db testClass = new db();
            testClass.setcontent(content);
            testClass.setmitem(item);
            testClass.setmno(mno);
            testClass.setsupplier(supplier);
            testClass.setsuppliermno(suppliermno);
            testClass.setykmno(ykmno);
            testClass.settype(type);


            //在testclass对象的mainThread(委托)对象上搭载两个方法，在线程中调用mainThread对象时相当于调用了这两个方法。 
            testClass.inserMaterial = new db.InsertMaterial(saveMaterial);
            // testClass.setPos = new db.SetPos(SetTextMesssage);

            //创建一个无参数的线程,这个线程执行TestClass类中的TestFunction方法。 
            Thread testClassThread = new Thread(new ThreadStart(testClass.saveMaterial));
            //启动线程，启动之后线程才开始执行 
            testClassThread.Start();


        }
        private void saveMaterial()
        {
            MessageBox.Show("物料增加成功");
          
            
            
        }
    }
}
