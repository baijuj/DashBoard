﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DashBoard.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public partial class DashBoardDBEntities : DbContext
    {
        public DashBoardDBEntities()
            : base("name=DashBoardDBEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;


        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //throw new UnintentionalCodeFirstException();
            base.OnModelCreating(modelBuilder);

        }

        public virtual DbSet<DashBoard> DashBoards { get; set; }
        public virtual DbSet<DashBoardUserMap> DashBoardUserMaps { get; set; }
        public virtual DbSet<DataSource> DataSources { get; set; }
        public virtual DbSet<Widget> Widgets { get; set; }
        public virtual DbSet<WidgetType> WidgetTypes { get; set; }
        public virtual DbSet<WidgetUserMap> WidgetUserMaps { get; set; }
    }
}
