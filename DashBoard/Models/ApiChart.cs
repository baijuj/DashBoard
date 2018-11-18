using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashBoard.Models
{
    public class Read
    {
        public string url { get; set; }
        public string dataType { get; set; }
    }

    public class Transport
    {
        public Read read { get; set; }
    }

    public class Sort
    {
        public string field { get; set; }
        public string dir { get; set; }
    }

    public class APIDataSource
    {
        public Transport transport { get; set; }
        public Sort sort { get; set; }
        public Group group { get; set; }
    }
    public class Group
    {
        public string field { get; set; }
    }
    public class Title
    {
        public string text { get; set; }
    }

    public class Legend
    {
        public string position { get; set; }
    }
    public class SeriesDefaults
    {
        public string type { get; set; }
        public Labels labels { get; set; }
    }
    public class DonutSeries
    {
        public string type { get; set; }
        public string field { get; set; }
        public string categoryField { get; set; }
        public string visibleInLegendField { get; set; }
    }
    public class Series
    {
        public string type { get; set; }
        public string field { get; set; }
        public string categoryField { get; set; }
        public string name { get; set; }
    }

    public class Labels
    {
        public int rotation { get; set; }
        public bool visible { get; set; }
        public string format { get; set; }
        public string background { get; set; }
        public string template { get; set; }
    }

    public class MajorGridLines
    {
        public bool visible { get; set; }
    }

    public class CategoryAxis
    {
        public Labels labels { get; set; }
        public MajorGridLines majorGridLines { get; set; }
    }

    public class Labels2
    {
        public string format { get; set; }
    }

    public class Line
    {
        public bool visible { get; set; }
    }

    public class ValueAxis
    {
        public Labels2 labels { get; set; }
        public Line line { get; set; }
    }

    public class Tooltip
    {
        public bool visible { get; set; }
        public string format { get; set; }
        public string template { get; set; }
    }
    public class Chart
    {

        public APIDataSource dataSource { get; set; }
        public Title title { get; set; }
        public Legend legend { get; set; }
        public SeriesDefaults seriesDefaults { get; set; }
        public CategoryAxis categoryAxis { get; set; }
        public ValueAxis valueAxis { get; set; }
        public Tooltip tooltip { get; set; }
        public int width { get; set; }
        public int hieght { get; set; }
    }
    public class ApiChart : Chart
    {
        public List<Series> series { get; set; }
    }
    public class DonutChart : Chart
    {
        public List<DonutSeries> series { get; set; }
    }

}