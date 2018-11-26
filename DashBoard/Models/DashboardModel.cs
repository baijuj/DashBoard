using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashBoard.Models
{
    public class DashboardModel
    {
        public DashboardModel()
        {
            Widgets = new List<WidgetModel>();
            AllUsers = new List<ApplicationUser>();


        }
        public List<WidgetModel> Widgets { get; set; }
        public List<ApplicationUser> AllUsers { get; set; }
    }
    public class WidgetModel
    {
        public int WidgetID { get; set; }
        public string WidgetName { get; set; }
        public string WidgetType { get; set; }
        public string WidgetSchema { get; set; }
        public string WidgetRenderType { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public Nullable<int> Column { get; set; }
        public Nullable<int> Row { get; set; }
        public int WidgetTypeID { get; set; }
        public ApiChart ApiChart { get; set; }
        public DonutChart DonutChart { get; set; }

    }
    //public class WidgetType
    //{
    //    public int WidgetTypeID { get; set; }
    //    public string TypeName { get; set; }
    //    public string WidgetSchema { get; set; }
    //}
    public class WidgetTypeParams
    {
        public string Url { get; set; }
    }
    public class CandleStickWidgetTypeParams : WidgetTypeParams
    {
        public string OpenField { get; set; }
        public string CloseField { get; set; }
        public string HighField { get; set; }
        public string LowField { get; set; }
        public string DateField { get; set; }
    }

    public class ChartModel
    {

        public int ChartID { get; set; }
        public int Title { get; set; }
        public ChartDataSourceModel DataSource { get; set; }
        public String ChartType { get; set; }
        public List<ChartSeriesModel> Series { get; set; }

    }
    public class ChartDataSourceModel
    {
        public string Url { get; set; }
        public string DataType { get; set; }
    }
    public class ChartSeriesModel
    {
        public string Name { get; set; }
        public String Data { get; set; }
    }

    public class ValueAxisModel
    {
        public int MaxValue { get; set; }
        public int AxisCrossingValue { get; set; }
        public String LabelFormat { get; set; }
        public string LabelRotation { get; set; }
    }
}