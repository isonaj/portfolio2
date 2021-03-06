using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using Portfolio2.Data;

namespace Portfolio2.Migrations
{
    [DbContext(typeof(PortfolioContext))]
    [Migration("20160221113610_3DecimalPrices")]
    partial class _3DecimalPrices
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Portfolio2.Data.Models.PriceHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Close")
                        .HasAnnotation("Relational:ColumnType", "decimal(18,3)");

                    b.Property<decimal>("High")
                        .HasAnnotation("Relational:ColumnType", "decimal(18,3)");

                    b.Property<decimal>("Low")
                        .HasAnnotation("Relational:ColumnType", "decimal(18,3)");

                    b.Property<decimal>("Open")
                        .HasAnnotation("Relational:ColumnType", "decimal(18,3)");

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
