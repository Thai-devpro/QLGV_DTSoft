using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLGV_DTSoft.Data;

public partial class DtsoftContext : DbContext
{
    public DtsoftContext()
    {
    }

    public DtsoftContext(DbContextOptions<DtsoftContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BoPhan> BoPhans { get; set; }

    public virtual DbSet<ChiTieu> ChiTieus { get; set; }

    public virtual DbSet<CoQuyenTruyCap> CoQuyenTruyCaps { get; set; }

    public virtual DbSet<KeHoachCongViec> KeHoachCongViecs { get; set; }

    public virtual DbSet<KeHoachGiaoViec> KeHoachGiaoViecs { get; set; }

    public virtual DbSet<KhuVuc> KhuVucs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<Quyen> Quyens { get; set; }

    public virtual DbSet<ThamGium> ThamGia { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-J0DDGD9P\\MSSQLSERVER16;Database=DTSoft;Integrated Security=True;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BoPhan>(entity =>
        {
            entity.HasKey(e => e.IdBp);

            entity.ToTable("BO_PHAN");

            entity.HasIndex(e => e.IdKhuvuc, "CO_FK");

            entity.Property(e => e.IdBp).HasColumnName("ID_BP");
            entity.Property(e => e.Congviecchuyenmon)
                .HasMaxLength(255)
                .HasColumnName("CONGVIECCHUYENMON");
            entity.Property(e => e.IdKhuvuc).HasColumnName("ID_KHUVUC");
            entity.Property(e => e.Tenbophan)
                .HasMaxLength(255)
                .HasColumnName("TENBOPHAN");

            entity.HasOne(d => d.IdKhuvucNavigation).WithMany(p => p.BoPhans)
                .HasForeignKey(d => d.IdKhuvuc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BO_PHAN_CO_KHU_VUC");
        });

        modelBuilder.Entity<ChiTieu>(entity =>
        {
            entity.HasKey(e => e.IdCt);

            entity.ToTable("CHI_TIEU");

            entity.HasIndex(e => e.IdKh, "CO_CHI_TIEU_FK");

            entity.Property(e => e.IdCt).HasColumnName("ID_CT");
            entity.Property(e => e.Chitieu)
                .HasMaxLength(255)
                .HasColumnName("CHITIEU");
            entity.Property(e => e.Doanhso).HasColumnName("DOANHSO");
            entity.Property(e => e.Donvitinh)
                .HasMaxLength(255)
                .HasColumnName("DONVITINH");
            entity.Property(e => e.IdKh).HasColumnName("ID_KH");

            entity.HasOne(d => d.IdKhNavigation).WithMany(p => p.ChiTieus)
                .HasForeignKey(d => d.IdKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHI_TIEU_CO_CHI_TI_KE_HOACH");
        });

        modelBuilder.Entity<CoQuyenTruyCap>(entity =>
        {
            entity.HasKey(e => e.IdQuyentc);

            entity.ToTable("CO_QUYEN_TRUY_CAP");

            entity.HasIndex(e => e.IdQuyen, "CO_QUYEN_TRUY_CAP2_FK");

            entity.HasIndex(e => e.IdVt, "CO_QUYEN_TRUY_CAP_FK");

            entity.Property(e => e.IdQuyentc).HasColumnName("ID_QUYENTC");
            entity.Property(e => e.IdQuyen).HasColumnName("ID_QUYEN");
            entity.Property(e => e.IdVt).HasColumnName("ID_VT");

            entity.HasOne(d => d.IdQuyenNavigation).WithMany(p => p.CoQuyenTruyCaps)
                .HasForeignKey(d => d.IdQuyen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CO_QUYEN_CO_QUYEN__QUYEN");

            entity.HasOne(d => d.IdVtNavigation).WithMany(p => p.CoQuyenTruyCaps)
                .HasForeignKey(d => d.IdVt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CO_QUYEN_CO_QUYEN__VAI_TRO");
        });

        modelBuilder.Entity<KeHoachCongViec>(entity =>
        {
            entity.HasKey(e => e.IdKhcv);

            entity.ToTable("KE_HOACH_CONG_VIEC");

            entity.Property(e => e.IdKhcv).HasColumnName("ID_KHCV");
            entity.Property(e => e.Namthuchien)
                .HasColumnType("datetime")
                .HasColumnName("NAMTHUCHIEN");
            entity.Property(e => e.Noidungcongviec)
                .HasMaxLength(255)
                .HasColumnName("NOIDUNGCONGVIEC");
        });

        modelBuilder.Entity<KeHoachGiaoViec>(entity =>
        {
            entity.HasKey(e => e.IdKh);

            entity.ToTable("KE_HOACH_GIAO_VIEC");

            entity.HasIndex(e => e.IdKhcv, "GAN_VOI_FK");

            entity.HasIndex(e => e.IdBp, "PHU_TRACH_FK");

            entity.Property(e => e.IdKh).HasColumnName("ID_KH");
            entity.Property(e => e.IdBp).HasColumnName("ID_BP");
            entity.Property(e => e.IdKhcv).HasColumnName("ID_KHCV");
            entity.Property(e => e.Motakh)
                .HasMaxLength(255)
                .HasColumnName("MOTAKH");
            entity.Property(e => e.Ngaybatdau)
                .HasColumnType("date")
                .HasColumnName("NGAYBATDAU");
            entity.Property(e => e.Ngayketthuc)
                .HasColumnType("date")
                .HasColumnName("NGAYKETTHUC");
            entity.Property(e => e.Ngaytaokh)
                .HasColumnType("date")
                .HasColumnName("NGAYTAOKH");
            entity.Property(e => e.Tenkehoach)
                .HasMaxLength(255)
                .HasColumnName("TENKEHOACH");

            entity.HasOne(d => d.IdBpNavigation).WithMany(p => p.KeHoachGiaoViecs)
                .HasForeignKey(d => d.IdBp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KE_HOACH_PHU_TRACH_BO_PHAN");

            entity.HasOne(d => d.IdKhcvNavigation).WithMany(p => p.KeHoachGiaoViecs)
                .HasForeignKey(d => d.IdKhcv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KE_HOACH_GAN_VOI_KE_HOACH");
        });

        modelBuilder.Entity<KhuVuc>(entity =>
        {
            entity.HasKey(e => e.IdKhuvuc);

            entity.ToTable("KHU_VUC");

            entity.Property(e => e.IdKhuvuc).HasColumnName("ID_KHUVUC");
            entity.Property(e => e.Diachi)
                .HasMaxLength(255)
                .HasColumnName("DIACHI");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Sodienthoai)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SODIENTHOAI");
            entity.Property(e => e.Tenkhuvuc)
                .HasMaxLength(255)
                .HasColumnName("TENKHUVUC");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.IdNd);

            entity.ToTable("NGUOI_DUNG");

            entity.HasIndex(e => e.IdVt, "CO_VAI_TRO_FK");

            entity.HasIndex(e => e.IdBp, "THUOC_FK");

            entity.Property(e => e.IdNd).HasColumnName("ID_ND");
            entity.Property(e => e.Diachi)
                .HasMaxLength(255)
                .HasColumnName("DIACHI");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Gioitinh)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("GIOITINH");
            entity.Property(e => e.Hoten)
                .HasMaxLength(255)
                .HasColumnName("HOTEN");
            entity.Property(e => e.IdBp).HasColumnName("ID_BP");
            entity.Property(e => e.IdVt).HasColumnName("ID_VT");
            entity.Property(e => e.Matkhau)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("MATKHAU");
            entity.Property(e => e.Ngaybatdaulam)
                .HasColumnType("date")
                .HasColumnName("NGAYBATDAULAM");
            entity.Property(e => e.Ngaysinh)
                .HasColumnType("date")
                .HasColumnName("NGAYSINH");
            entity.Property(e => e.Quequan)
                .HasMaxLength(255)
                .HasColumnName("QUEQUAN");
            entity.Property(e => e.Sodienthoai)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("SODIENTHOAI");
            entity.Property(e => e.Tennguoidung)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("TENNGUOIDUNG");
            entity.Property(e => e.Thamnien).HasColumnName("THAMNIEN");

            entity.HasOne(d => d.IdBpNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.IdBp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NGUOI_DUNG_BO_PHAN");

            entity.HasOne(d => d.IdVtNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.IdVt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NGUOI_DUNG_VAI_TRO");
        });

        modelBuilder.Entity<Quyen>(entity =>
        {
            entity.HasKey(e => e.IdQuyen);

            entity.ToTable("QUYEN");

            entity.Property(e => e.IdQuyen).HasColumnName("ID_QUYEN");
            entity.Property(e => e.Tenquyen)
                .HasMaxLength(255)
                .HasColumnName("TENQUYEN");
        });

        modelBuilder.Entity<ThamGium>(entity =>
        {
            entity.HasKey(e => new { e.IdNd, e.IdKh, e.IdCt });

            entity.ToTable("THAM_GIA");

            entity.HasIndex(e => e.IdKh, "THAM_GIA2_FK");

            entity.HasIndex(e => e.IdNd, "THAM_GIA_FK");

            entity.Property(e => e.IdNd).HasColumnName("ID_ND");
            entity.Property(e => e.IdKh).HasColumnName("ID_KH");
            entity.Property(e => e.IdCt).HasColumnName("ID_CT");
            entity.Property(e => e.Danhgia)
                .HasMaxLength(50)
                .HasColumnName("DANHGIA");
            entity.Property(e => e.SlHoanthanh).HasColumnName("SL_HOANTHANH");

            entity.HasOne(d => d.IdCtNavigation).WithMany(p => p.ThamGia)
                .HasForeignKey(d => d.IdCt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CHI_TIEU_CUA_KE_HOACH");

            entity.HasOne(d => d.IdKhNavigation).WithMany(p => p.ThamGia)
                .HasForeignKey(d => d.IdKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_THAM_GIA_THAM_GIA2_KE_HOACH");

            entity.HasOne(d => d.IdNdNavigation).WithMany(p => p.ThamGia)
                .HasForeignKey(d => d.IdNd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_THAM_GIA_THAM_GIA_NGUOI_DU");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.IdVt);

            entity.ToTable("VAI_TRO");

            entity.Property(e => e.IdVt).HasColumnName("ID_VT");
            entity.Property(e => e.Mota)
                .HasMaxLength(255)
                .HasColumnName("MOTA");
            entity.Property(e => e.Tenvaitro)
                .HasMaxLength(100)
                .HasColumnName("TENVAITRO");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
