using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Cerval_Library;
using System.Data.SqlClient;

namespace DCGS_Staff_Intranet2.content
{
    public partial class ValueAdded1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //going to get groups from new dawn structure...
            //whose great grand parent is dcgs root (ie level 3)
            // and whose parent is the year code....
            //this will give the structure group from the curriculum year and staff groups.

            GroupList gla = new GroupList();
            string yearcode = "2011";
            string sg = Cerval_Globals.DCGSroot.ToString();
            string SelectL1="( SELECT ChildId FROM tbl_Core_Groups_Groups WHERE (ParentId = '"+sg+"')) ";
            string SelectL2="( SELECT ChildId FROM tbl_Core_Groups_Groups ";
            SelectL2 +="INNER JOIN  tbl_Core_Groups ON tbl_Core_Groups_Groups.ChildId = tbl_Core_Groups.GroupId ";
            SelectL2 += "WHERE ( ParentId IN  " + SelectL1 + ") AND (tbl_Core_Groups.GroupCode ='"+yearcode+"')  ) ";
            string s= "SELECT ChildId, g.GroupCode FROM tbl_Core_Groups_Groups  ";
            s+=" INNER JOIN  tbl_Core_Groups AS g ON tbl_Core_Groups_Groups.ChildId = g.GroupId ";  
            s += " WHERE ( ParentId IN  "+SelectL2+ " ) ";

            Encode en = new Encode();
            string db_connection = en.GetDbConnection();
            int n = 0;
            using (SqlConnection cn = new SqlConnection(db_connection))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(s, cn))
                {
                    using (SqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            n++;
                            Guid g1 = dr.GetGuid(00);
                            string c = dr.GetString(1);
                            if (c.ToUpper().Trim() == "CC")
                            {
                                c = c;
                            }
                        }
                        dr.Close();
                    }
                }
                cn.Close();
            }

            ValueAddedMethodList ml1 = new ValueAddedMethodList();
            foreach (ValueAddedMethod vm in ml1._ValueAddedMethodList)
            {
                string s1 = vm.m_ValeAddedDescription;
            }
        }
    }
}
