namespace WebAppEtronix20180824.Models.Entity
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }

        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<tblMainMenu> tblMainMenu { get; set; }
        public virtual DbSet<tblSubMenu> tblSubMenu { get; set; }
        public virtual DbSet<tlbRoles> tlbRoles { get; set; }
        public virtual DbSet<Procedures.MeasurementUnitTypes> MeasurementUnitType { get; set; }
        public virtual DbSet<Points> Points { get; set; }
        public virtual DbSet<AccessPoints> AccessP { get; set; }

        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblMainMenu>()
                .Property(e => e.MainMenu)
                .IsUnicode(false);

            modelBuilder.Entity<tblSubMenu>()
                .Property(e => e.SubMenu)
                .IsUnicode(false);

            modelBuilder.Entity<tblSubMenu>()
                .Property(e => e.Controller)
                .IsUnicode(false);

            modelBuilder.Entity<tblSubMenu>()
                .Property(e => e.Action)
                .IsUnicode(false);

            modelBuilder.Entity<tblSubMenu>()
                .HasMany(e => e.tlbRoles)
                .WithRequired(e => e.tblSubMenu)
                .HasForeignKey(e => e.tlbSubMenuId)
                .WillCascadeOnDelete(false);
        }
    }
}
