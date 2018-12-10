using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Microsoft.AspNet.SignalR;
using DashBoard.Models;

using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Runtime.Remoting.Contexts;

namespace DashBoard
{
    public class NotificationComponent
    {
        public void RegisterNotification()
        {
            string conStr = ConfigurationManager.ConnectionStrings["DashBoardDBEntities"].ConnectionString;

            string sqlCommand = @"SELECT[Id],[UserName],[WidgetID],[IsOwner],[IsRead] FROM[dbo].[WidgetUserMaps] WHERE ISNULL(IsRead,0)<>1";
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con);
                cmd.Parameters.AddWithValue("@AddedOn", DateTime.Now);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += sqlDep_OnChange;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    
                }
            }

        }

        void sqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {


            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;

               
                    //from here we will send notification message to client
                    var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                notificationHub.Clients.All.notify("added");

                //re-register notification
                RegisterNotification();

            }
        }

        public List<Widget> GetNotifications(string currentUser)
        {
            List<Widget> widgets = new List<Widget>();

            using (DashBoardDBEntities dbContext = new DashBoardDBEntities())
            {
                widgets = (from w in dbContext.Widgets
                               join u in dbContext.WidgetUserMaps
                               on w.WidgetID equals u.WidgetID
                               where u.UserName == currentUser && u.IsRead!=true
                               select w).ToList();
            }
            return widgets;
        }
    }
}