using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DashBoard.Models;
using Newtonsoft.Json;

namespace DashBoard.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var model = new DashboardModel();
            using (var dbContext = new DashBoardDBEntities())
            {
                var widgets = dbContext.Widgets.ToList();

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
                result = dbContext.Widgets.Where(t => t.WidgetID == WidgetID).FirstOrDefault();
                if (result != null)
                {
                    if (result.WidgetTypeID != 4)
                    {
                        barChart = JsonConvert.DeserializeObject<ApiChart>(result.WidgetTypeInputParamValues);
                        var data = new WidgetModel()
                        {
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
            using (var dbContext = new DashBoardDBEntities())
            {
                var widget = dbContext.Widgets.Where(t => t.WidgetID == model.WidgetID).FirstOrDefault();
                var widgetType = dbContext.WidgetTypes.Where(t => t.WidgetTypeID == model.WidgetTypeID).FirstOrDefault();
                if (model.ApiChart != null)
                {
                    if (widget == null)
                    {
                        widget = CreateWidgetObject();
                        DashBoard.Models.DataSource dataSource = dbContext.DataSources.Where(t => t.DataSourceID == 1).FirstOrDefault();
                        dbContext.DataSources.Attach(dataSource);
                        widget.DataSource = dataSource;
                        dbContext.Entry(widget).State = EntityState.Added;

                    }
                    widget.WidgetType = widgetType;
                    widget.WidgetTypeID = widgetType.WidgetTypeID;
                    widget.WidgetTypeInputParamValues = JsonConvert.SerializeObject(model.ApiChart);
                    WidgetSchemaMapper widgetSchemaMapper = new WidgetSchemaMapper(widget);
                    string widgetSchema = widgetSchemaMapper.GetSchema();
                    widget.WidgetTypeInputParamValues = widgetSchema;
                    dbContext.SaveChanges();
                }
                if (model.DonutChart != null && widget != null && widgetType.WidgetTypeID == 4)
                {
                    if (widget == null)
                    {
                        widget = CreateWidgetObject();
                        DashBoard.Models.DataSource dataSource = dbContext.DataSources.Where(t => t.DataSourceID == 1).FirstOrDefault();
                        dbContext.DataSources.Attach(dataSource);
                        widget.DataSource = dataSource;
                        dbContext.Entry(widget).State = EntityState.Added;
                    }
                    widget.WidgetType = widgetType;
                    widget.WidgetTypeID = widgetType.WidgetTypeID;
                    widget.WidgetTypeInputParamValues = JsonConvert.SerializeObject(model.DonutChart);
                    WidgetSchemaMapper widgetSchemaMapper = new WidgetSchemaMapper(widget);
                    string widgetSchema = widgetSchemaMapper.GetSchema();
                    widget.WidgetTypeInputParamValues = widgetSchema;
                    dbContext.SaveChanges();
                }


            }
            return Json(new { Data = "Successfully saved" });
        }

        public ActionResult SaveWidgetShare(string[] sharedUsers, string widgetID)
        {
            return Json("");
        }
            private static Widget CreateWidgetObject()
        {
            Widget widget = new Widget();
            widget.WidgetName = "Widget " + new Random().Next();
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

    }
}