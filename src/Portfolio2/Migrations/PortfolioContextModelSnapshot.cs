using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Portfolio2.Data;

namespace Portfolio2.Migrations
{
    [DbContext(typeof(PortfolioContext))]
    partial class PortfolioContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Portfolio2.Data.Models.PriceHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Close");

                    b.Property<decimal>("High");

                    b.Property<decimal>("Low");

                    b.Property<decimal>("Open");

                    b.Property<DateTime>("PriceDate");

                    b.Property<int>("StockId");

                    b.Property<int>("Volume");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Portfolio2.Data.Models.Stock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Portfolio2.Data.Models.Txn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<int>("StockId");

                    b.Property<DateTime>("TxnDate");

                    b.Property<string>("TxnType");

                    b.Property<int>("Units");

                    b.Property<string>("Username");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Portfolio2.Data.Models.PriceHistory", b =>
                {
                    b.HasOne("Portfolio2.Data.Models.Stock")
                        .WithMany()
                        .HasForeignKey("StockId");
                });

            modelBuilder.Entity("Portfolio2.Data.Models.Txn", b =>
                {
                    b.HasOne("Portfolio2.Data.Models.Stock")
                        .WithMany()
                        .HasForeignKey("StockId");
                });
        }
    }
}
