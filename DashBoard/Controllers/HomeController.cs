using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using Dashboard.DataAccess.Models;
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
                    WidgetSchemaMapper widgetSchemaMapper = new WidgetSchemaMapper(item);
                    string widgetSchema = widgetSchemaMapper.GetSchema();
                    model.Widgets.Add(
                            new WidgetModel()
                            {
                                WidgetID = item.WidgetID,
                                WidgetName = item.WidgetName,
                                WidgetSchema = widgetSchema,
                                WidgetRenderType = "kendoChart",
                                Height = item.Height,
                                Width = item.Width
                            }
                            );
                }
            }

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
            Widget result;
            BarChart barChart = new BarChart();
            using (var dbContext = new DashBoardDBEntities())
            {
                result = dbContext.Widgets.Where(t => t.WidgetID == WidgetID).FirstOrDefault();
                if (result != null)
                {
                    barChart = JsonConvert.DeserializeObject<BarChart>(result.WidgetTypeInputParamValues);
                }
            }
            return Json(new { Data = barChart.series });
        }
        public JsonResult WidgetDetails(int WidgetID)
        {
            Widget result;
            BarChart barChart = new BarChart();
            DonutChart donutChart = new DonutChart();
            using (var dbContext = new DashBoardDBEntities())
            {
                result = dbContext.Widgets.Where(t => t.WidgetID == WidgetID).FirstOrDefault();
                if (result != null)
                {
                    if (result.WidgetTypeID != 4)
                    {
                        barChart = JsonConvert.DeserializeObject<BarChart>(result.WidgetTypeInputParamValues);
                        return Json(new { Data = barChart, WidgetType = result.WidgetTypeID });
                    }
                    else
                    {
                        donutChart = JsonConvert.DeserializeObject<DonutChart>(result.WidgetTypeInputParamValues);
                        return Json(new { Data = donutChart, WidgetType = result.WidgetTypeID });
                    }
                }
            }
            return Json(new { Data = "Data not found" });
        }
        public ActionResult Test()
        {
            ViewBag.Message = "Test";

            return View();
        }
    }
}