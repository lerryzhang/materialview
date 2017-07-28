using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace MaterialView
{
    class db
    {
        public string userName = "wwwsa";
        public string passwd = "0p@d08";
        public string server = "192.168.0.50";
        public string constr = "";
        public string sql = "";
        public SqlConnection conn = null;

        public delegate void TestDelegate(DataSet ds);
        public delegate void BomDelegate(DataSet ds);
        public delegate void InsertMaterial();
        public delegate void DeleteMaterial();


        public TestDelegate mainThread;
        public BomDelegate bomDelegate;
        public InsertMaterial inserMaterial;
        public DeleteMaterial deleteMaterial;


        private string item;
        private string mnotypeyk;

        private string startDate;
        private string endDate;



        private string mitem;
        private string type;
        private string supplier;
        private string mno;
        private string ykmno;
        private string suppliermno;
        private string content;
        private int id;
        private string materialokid;

        public string Materialokid
        {
            get { return materialokid; }
            set { materialokid = value; }
        }


        public void setId(int id)
        {

            this.id = id;

        }


        public void setmitem(string mitem)
        {

            this.mitem = mitem;

        }

        public void settype(string type)
        {

            this.type = type;

        }

        public void setsupplier(string supplier)
        {

            this.supplier = supplier;

        }

        public void setmno(string mno)
        {

            this.mno = mno;

        }

        public void setykmno(string ykmno)
        {

            this.ykmno = ykmno;

        }

        public void setsuppliermno(string suppliermno)
        {

            this.suppliermno = suppliermno;

        }

        public void setcontent(string content)
        {

            this.content = content;

        }





        public void setStartDate(string mstartdate)
        {

            startDate = mstartdate;

        }

        public void setEndDate(string menddate)
        {

            endDate = menddate;

        }


        //包含参数的构造函数
        public db(string citem, string cmnotypeyk)
        {
            item = citem;
            mnotypeyk = cmnotypeyk;
        }

        public db()
        {

        }



        public SqlConnection getConn()
        {

            constr = "server=" + server + ";uid=" + userName + ";pwd=" + passwd + ";database=prdtctrl";
            conn = new SqlConnection(constr);
            return conn;
        }



        public void showValue()
        {
            constr = "server=" + server + ";uid=" + userName + ";pwd=" + passwd + ";database=prdtctrl";
            sql = "select a.id,a.material,a.mno,b.corp,a.suppliermno,a.content,a.mnoyk,c.cname,a.itemname,a.materialokid from pc_material a,pc_supplier b,pc_mnoyk c where a.supplierid=b.id and c.mnotypeyk=a.mnotypeyk ";

            if (item != "")
            {
                sql += "and a.itemname='" + item + "'";
            }
            if (mnotypeyk != "")
            {
                sql += " and a.mnotypeyk='" + mnotypeyk + "'";
            }
            sql += " order by a.id desc";
            Console.WriteLine("{0}", sql);
            conn = new SqlConnection(constr);
            conn.Open();
            SqlCommand sc = new SqlCommand(sql, conn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            DataSet ds = new DataSet();
            sda.Fill(ds, "物料信息");
            mainThread(ds);
            conn.Close();
            conn.Dispose();
        }

        public void showBom()
        {
            constr = "server=" + server + ";uid=" + userName + ";pwd=" + passwd + ";database=task";
            conn = new SqlConnection(constr);
            conn.Open();
            sql = "select a.id,b.itemname,c.nodename,a.dccname,a.dccfile,a.filesize,a.uid,a.ptime,a.content  from ls_dcc a,ls_item b,ls_dcctree c where left(a.dcctype,3)='101' and  a.itemid=b.id and a.dcctype=c.nodeid";
            if (startDate != "")
            {
                sql += " and CONVERT(varchar(100),a.ptime, 111)>='" + startDate + "'";
            }
            if (endDate != "")
            {
                sql += " and CONVERT(varchar(100),a.ptime, 111)<='" + endDate + "'";
            }
            sql += " order by a.id desc";



            SqlCommand sc = new SqlCommand(sql, conn);
            SqlDataAdapter sda = new SqlDataAdapter(sc);
            DataSet ds = new DataSet();
            sda.Fill(ds, "BOM信息");
            bomDelegate(ds);
            conn.Close();
            conn.Dispose();
        }



        public void saveMaterial()
        {

            constr = "server=" + server + ";uid=" + userName + ";pwd=" + passwd + ";database=prdtctrl";
            conn = new SqlConnection(constr);
            conn.Open();
            sql = "insert into pc_material(material,mno,suppliermno,supplierid,content,mnoyk,mnotypeyk,itemname) values ('" + content + "','" + mno + "','" + suppliermno + "','" + supplier + "','" + content + "','" + ykmno + "','" + type + "','" + mitem + "')";
            SqlCommand com = new SqlCommand(sql, conn);
            int count = com.ExecuteNonQuery();
            if (count == 1) { inserMaterial(); }
            conn.Close();
            conn.Dispose();
        }


        public void removeMaterial()
        {

            constr = "server=" + server + ";uid=" + userName + ";pwd=" + passwd + ";database=prdtctrl";
            conn = new SqlConnection(constr);
            conn.Open();
            sql = "delete from pc_material where id=" + id;
            SqlCommand com = new SqlCommand(sql, conn);
            int count = com.ExecuteNonQuery();
            if (count == 1)
            { //deleteMaterial();
                Console.WriteLine(id);
                if (materialokid != "")
                {
                    int i_materialokid = int.Parse(materialokid);
                    sql = "delete from pc_materialok where id=" + id;
                    com = new SqlCommand(sql, conn);
                    count = com.ExecuteNonQuery();
                    if (count == 1)
                    {
                        Console.WriteLine(id);
                    }
                }
            }
            conn.Close();
            conn.Dispose();
        }

    }
}
