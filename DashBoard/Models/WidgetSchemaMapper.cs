using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DashBoard.Models
{
    internal class WidgetSchemaMapper
    {
        private Widget item;

        public WidgetSchemaMapper(Widget item)
        {
            this.item = item;
        }
        public string GetSchema()
        {
            string barchartSchema = string.Empty;
            barchartSchema = item.WidgetType.WidgetSchema;
            if (item.DataSource.DataSourceID == 1)
            {
                if (item.WidgetType.WidgetTypeID == 4)
                {
                    string seriesArray = string.Empty;
                    DonutChart donutChart = JsonConvert.DeserializeObject<DonutChart>(item.WidgetTypeInputParamValues);
                    seriesArray = JsonConvert.SerializeObject(donutChart.series);
                    barchartSchema = barchartSchema.Replace("\"{SeriesArray}\"", seriesArray);
                    FillBaseProperties(ref barchartSchema, donutChart);
                }
                else
                {
                    ApiChart barChart = JsonConvert.DeserializeObject<ApiChart>(item.WidgetTypeInputParamValues);
                    FillBaseProperties(ref barchartSchema, barChart);
                    string seriesArray = string.Empty;
                    seriesArray = JsonConvert.SerializeObject(barChart.series);
                    barchartSchema = barchartSchema.Replace("\"{SeriesArray}\"", seriesArray);
                }
            }

            return barchartSchema;
        }



        private void FillBaseProperties(ref string barchartSchema, Chart barChart)
        {
            barchartSchema = barchartSchema.Replace("{Url}", barChart.dataSource.transport.read.url);
            barchartSchema = barchartSchema.Replace("{dataType}", barChart.dataSource.transport.read.dataType);
            if (barChart.dataSource.sort != null)
            {
                barchartSchema = barchartSchema.Replace("{SortField}", barChart.dataSource.sort.field);
                barchartSchema = barchartSchema.Replace("{SortDirection}", barChart.dataSource.sort.dir);
            }
            if (barChart.dataSource.group != null)
            {
                barchartSchema = barchartSchema.Replace("{GroupField}", barChart.dataSource.group.field);
            }
            barchartSchema = barchartSchema.Replace("{TitleText}", barChart.title.text);
            if (barChart.seriesDefaults != null)
            {
                barchartSchema = barchartSchema.Replace("{SeriesDefaultType}", barChart.seriesDefaults.type);
                if (barChart.seriesDefaults.labels != null)
                    barchartSchema = barchartSchema.Replace("{SeriesDefaultLabelTemplate}", barChart.seriesDefaults.labels.template);
            }
            if (barChart.valueAxis != null)
            {
                if (barChart.valueAxis.labels != null)
                    barchartSchema = barchartSchema.Replace("{ValueAxisLableFormat}", barChart.valueAxis.labels.format);
                //  barchartSchema = barchartSchema.Replace("\"{ValueAxisMajorUnit}\"", barChart.valueAxis.majorUnit.ToString());
            }
            if (barChart.legend != null)
            {
                barchartSchema = barchartSchema.Replace("{LegendPosition}", barChart.legend.position);
            }
            if (barChart.categoryAxis != null && barChart.categoryAxis.labels != null)
                barchartSchema = barchartSchema.Replace("\"{CagetoryAxisLabelRotation}\"", barChart.categoryAxis.labels.rotation.ToString());
            if (barChart.tooltip != null)
            {
                barchartSchema = barchartSchema.Replace("{TooltipTemplate}", barChart.tooltip.template);
                barchartSchema = barchartSchema.Replace("{TooltipFormat}", barChart.tooltip.format);
            }
        }
    }
}