using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DashBoard.Models;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace DashBoard.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            string currentUser = User.Identity.Name;
            var model = new DashboardModel();
            using (var dbContext = new DashBoardDBEntities())
            {
                //var widgets = dbContext.Widgets.ToList();
                var widgets = (from w in dbContext.Widgets
                               join u in dbContext.WidgetUserMaps
                               on w.WidgetID equals u.WidgetID
                               where u.UserName == currentUser
                               select w).ToList();

                foreach (Widget item in widgets)
                {

                    model.Widgets.Add(
                            new WidgetModel()
                            {
                                WidgetID = item.WidgetID,
                                WidgetName = item.WidgetName,
                                WidgetSchema = item.WidgetTypeInputParamValues,
                                WidgetRenderType = "kendoChart",
                                Height = item.Height,
                                Width = item.Width,
                                Row = item.Row,
                                Column = item.Column
                            }
                            );
                }
            }

            var applicationContext = new ApplicationDbContext();
            model.AllUsers = applicationContext.Users.ToList();

            return View(model);
        }
        public JsonResult WidgetTypes()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            using (var dbContext = new DashBoardDBEntities())
            {
                result = (from d in dbContext.WidgetTypes
                          select new SelectListItem
                          {
                              Text = d.WidgetName,
                              Value = d.WidgetTypeID.ToString()
                          }).ToList();

            }
            return Json(new { Data = result });
        }
        public JsonResult ChartSeries(int WidgetID)
        {
            DashBoard.Models.Widget result;
            ApiChart barChart = new ApiChart();
            using (var dbContext = new DashBoardDBEntities())
            {
                result = dbContext.Widgets.Where(t => t.WidgetID == WidgetID).FirstOrDefault();
                if (result != null)
                {
                    barChart = JsonConvert.DeserializeObject<ApiChart>(result.WidgetTypeInputParamValues);
                }
            }
            return Json(new { Data = barChart.series });
        }
        public JsonResult WidgetDetails(int WidgetID)
        {
            DashBoard.Models.Widget result;
            ApiChart barChart = new ApiChart();
            DonutChart donutChart = new DonutChart();
            using (var dbContext = new DashBoardDBEntities())
            {
                result = dbContext.Widgets.Include(r => r.DataSource).Where(t => t.WidgetID == WidgetID).FirstOrDefault();
                if (result != null)
                {
                    if (result.WidgetTypeID != 4)
                    {
                        barChart = JsonConvert.DeserializeObject<ApiChart>(result.WidgetTypeInputParamValues);
                        var data = new WidgetModel()
                        {
                            DataSourceName = result.DataSource.DataSourceType,
                            ApiChart = barChart,
                            WidgetTypeID = result.WidgetTypeID,
                            WidgetID = WidgetID
                        };
                        return Json(new { Data = data });
                    }
                    else
                    {
                        donutChart = JsonConvert.DeserializeObject<DonutChart>(result.WidgetTypeInputParamValues);
                        var data = new WidgetModel()
                        {
                            DataSourceName = result.DataSource.DataSourceType,
                            DonutChart = donutChart,
                            WidgetTypeID = result.WidgetTypeID,
                            WidgetID = WidgetID
                        };
                        return Json(new { Data = data });
                    }
                }
            }
            return Json(new { Data = "Data not found" });
        }
        public ActionResult SaveApiWidget(WidgetModel model)
        {
            bool flgNewWidget=false;
            using (var dbContext = new DashBoardDBEntities())
            {
                var widget = dbContext.Widgets.Where(t => t.WidgetID == model.WidgetID).FirstOrDefault();
                var widgetType = dbContext.WidgetTypes.Where(t => t.WidgetTypeID == model.WidgetTypeID).FirstOrDefault();
                DataSource dataSource = dbContext.DataSources.Where(t => t.DataSourceType == model.DataSourceName).FirstOrDefault();
                if (model.ApiChart != null)
                {
                    if (widget == null)
                    {
                        widget = CreateWidgetObject(model.ApiChart.title.text);
                        dbContext.Entry(widget).State = EntityState.Added;
                        flgNewWidget = true;
                    }
                   
                    dbContext.DataSources.Attach(dataSource);
                    widget.DataSource = dataSource;

                    widget.WidgetType = widgetType;
                    widget.WidgetTypeID = widgetType.WidgetTypeID;
                    widget.WidgetTypeInputParamValues = JsonConvert.SerializeObject(model.ApiChart);
                    WidgetSchemaMapper widgetSchemaMapper = new WidgetSchemaMapper(widget);
                    string widgetSchema = widgetSchemaMapper.GetSchema();
                    widget.WidgetTypeInputParamValues = widgetSchema;
                    dbContext.SaveChanges();
                }
                if (model.DonutChart != null && widgetType.WidgetTypeID == 4)
                {
                    if (widget == null)
                    {
                        widget = CreateWidgetObject(model.DonutChart.title.text);
                        dbContext.Entry(widget).State = EntityState.Added;
                        flgNewWidget = true;
                    }
                    
                    dbContext.DataSources.Attach(dataSource);
                    widget.DataSource = dataSource;


                    widget.WidgetType = widgetType;
                    widget.WidgetTypeID = widgetType.WidgetTypeID;
                    widget.WidgetTypeInputParamValues = JsonConvert.SerializeObject(model.DonutChart);
                    WidgetSchemaMapper widgetSchemaMapper = new WidgetSchemaMapper(widget);
                    string widgetSchema = widgetSchemaMapper.GetSchema();
                    widget.WidgetTypeInputParamValues = widgetSchema;
                    dbContext.SaveChanges();
                }
                //user mapping for new widget
                if (flgNewWidget)
                {
                    WidgetUserMap userMap = new WidgetUserMap();
                    userMap.UserName = User.Identity.Name;
                    userMap.WidgetID = Convert.ToInt32(widget.WidgetID);
                    userMap.IsOwner = true;
                    userMap.IsRead = true;
                    dbContext.WidgetUserMaps.Add(userMap);
                    dbContext.SaveChanges();
                }
            }
            return Json(new { Data = "Successfully saved" });
        }

        public ActionResult SaveWidgetShare(string[] sharedUsers, string widgetID)
        {
            using (var dbContext = new DashBoardDBEntities())
            {
                foreach (string user in sharedUsers)
                {
                    //dbContext.WidgetUserMaps.Remove
                    var currentMap = dbContext.WidgetUserMaps.Where(c => c.UserName == user && c.WidgetID.ToString() == widgetID && c.IsOwner != true).FirstOrDefault();
                    if(currentMap!=null)
                        dbContext.WidgetUserMaps.Remove(currentMap);

                    WidgetUserMap userMap = new WidgetUserMap();
                    userMap.UserName = user;
                    userMap.WidgetID =Convert.ToInt32(widgetID);
                    dbContext.WidgetUserMaps.Add(userMap);
                }

                dbContext.SaveChanges();
            }
            return Json("");
        }
        private static Widget CreateWidgetObject(string title)
        {
            string widgetTitle = "";
            if (!string.IsNullOrEmpty(title))
                widgetTitle = title;
            else
                widgetTitle= "Widget " + new Random().Next();
            Widget widget = new Widget();
            widget.WidgetName = widgetTitle;
            widget.DataSourceID = 1;
            widget.DataSourceInputParamValues = "{}";
            widget.Width = 1;
            widget.Height = 1;

            //??
            widget.Row = 1;
            widget.Column = 3;

            return widget;
        }

        public JsonResult ResizeWidget(int WidgetID, int Width, int Height)
        {
            using (var dbContext = new DashBoardDBEntities())
            {
                var widget = dbContext.Widgets.Where(t => t.WidgetID == WidgetID).FirstOrDefault();
                if (widget != null)
                {
                    widget.Height = Height;
                    widget.Width = Width;
                }
                dbContext.SaveChanges();
            }
            return Json(new { Data = "Updated" });
        }
        public JsonResult DeleteWidget(int WidgetID)
        {
            using (var dbContext = new DashBoardDBEntities())
            {
                var widget = dbContext.Widgets.Where(t => t.WidgetID == WidgetID).FirstOrDefault();
                dbContext.Widgets.Remove(widget);

                //remove existing mapping
                var currentMaps = dbContext.WidgetUserMaps.Where(c =>  c.WidgetID == WidgetID );
                if (currentMaps != null)
                    dbContext.WidgetUserMaps.RemoveRange(currentMaps);

                dbContext.SaveChanges();
            }
            return Json(new { Data = "Updated" });
        }

        public JsonResult RepositionWidget(int WidgetID, int Column, int Row)
        {
            using (var dbContext = new DashBoardDBEntities())
            {
                var widget = dbContext.Widgets.Where(t => t.WidgetID == WidgetID).FirstOrDefault();
                if (widget != null)
                {
                    widget.Column = Column;
                    widget.Row = Row;
                }
                dbContext.SaveChanges();
            }
            return Json(new { Data = "Updated" });
        }
        [Authorize]
        public JsonResult GetNotificationWidget()
        {
            NotificationComponent NC = new NotificationComponent();
            var list = NC.GetNotifications(User.Identity.Name);
           return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult UpdateNotifications()
        {
            using (var dbContext = new DashBoardDBEntities())
            {
                var widgets = dbContext.WidgetUserMaps.Where(t => t.UserName == User.Identity.Name).ToList();
                foreach(WidgetUserMap widget in widgets)
                {
                    widget.IsRead = true;
                }
                dbContext.SaveChanges();
            }
            //return Json("");
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataSources()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            using (var dbContext = new DashBoardDBEntities())
            {
                result = (from d in dbContext.DataSources
                          select new SelectListItem
                          {
                              Text = d.DataSourceType,
                              Value = d.DataSourceID.ToString()
                          }).ToList();

            }
            return Json(new { Data = result });
        }


        [HttpPost]
        public void UploadCSV()
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                var fileName = Path.GetFileName(file.FileName);

                var path = Path.Combine(Server.MapPath("~/App_Data/"), fileName);
                file.SaveAs(path);
            }

        }
        public JsonResult GetCSVFileData(string fileName)
        {
            var path = System.IO.Path.Combine(Server.MapPath("~/App_Data/"), fileName);
            var csv = new List<string[]>();
            var lines = System.IO.File.ReadAllLines(path);
            foreach (string line in lines)
                csv.Add(line.Split(','));
            var properties = lines[0].Split(',');
            var listObjResult = new List<Dictionary<string, string>>();
            for (int i = 1; i < lines.Length; i++)
            {
                var objResult = new Dictionary<string, string>();
                for (int j = 0; j < properties.Length; j++)
                    objResult.Add(properties[j], csv[i][j]);
                listObjResult.Add(objResult);
            }
            var result = JsonConvert.SerializeObject(listObjResult);
            var dynamicObject = JsonConvert.DeserializeObject<dynamic>(result);
            return Json(listObjResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDatabaseTables()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            using (var dbContext = new DashBoardDBEntities())
            {
                var dataSource = dbContext.DataSources.Where(t => t.DataSourceType == "Database").FirstOrDefault();
                var databaseParam = JsonConvert.DeserializeObject<DatabaseParam>(dataSource.InputParams);
                using (var connection = new SqlConnection(databaseParam.ConnectionString))
                {
                    connection.Open();
                    IDbCommand cmd = new SqlCommand("SELECT Name FROM sys.tables Order by Name", connection);
                    IDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(
                                new SelectListItem
                                {
                                    Text = Convert.ToString(dr["Name"]),
                                    Value = Convert.ToString(dr["Name"])
                                }
                            );
                    }
                    dr.Dispose();
                    connection.Close();
                }
            }
            return Json(new { Data = result });
        }
        public JsonResult GetDatabaseData(string tableName)
        {
            DataTable dt = new DataTable();
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            using (var dbContext = new DashBoardDBEntities())
            {
                var dataSource = dbContext.DataSources.Where(t => t.DataSourceType == "Database").FirstOrDefault();
                var databaseParam = JsonConvert.DeserializeObject<DatabaseParam>(dataSource.InputParams);
                using (var connection = new SqlConnection(databaseParam.ConnectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(String.Format("SELECT * FROM {0}", tableName), connection);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    Dictionary<string, string> row;
                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, string>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, Convert.ToString(dr[col]));
                        }
                        rows.Add(row);
                    }
                    connection.Close();
                }
            }
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

    }
}