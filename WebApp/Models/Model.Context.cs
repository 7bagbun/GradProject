﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApp.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TestDbEntities : DbContext
    {
        public TestDbEntities()
            : base("name=TestDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Image> Image { get; set; }
        public virtual DbSet<Member> Member { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<PriceHistory> PriceHistory { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Selling> Selling { get; set; }
        public virtual DbSet<Source> Source { get; set; }
        public virtual DbSet<TrackProduct> TrackProduct { get; set; }
        public virtual DbSet<Article> Article { get; set; }
    }
}
