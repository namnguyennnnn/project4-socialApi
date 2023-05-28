﻿// <auto-generated />
using System;
using DoAn4.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DoAn4.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("DoAn4.Models.AccessToken", b =>
                {
                    b.Property<Guid>("AccessTokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("AccessTokenId");

                    b.HasIndex("UserId");

                    b.ToTable("AccessTokens");
                });

            modelBuilder.Entity("DoAn4.Models.Comment", b =>
                {
                    b.Property<Guid>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CommentTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("PostId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("UpdateCommentTime")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("CommentId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("DoAn4.Models.Conversations", b =>
                {
                    b.Property<Guid>("ConversationsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId1")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId2")
                        .HasColumnType("char(36)");

                    b.HasKey("ConversationsId");

                    b.HasIndex("UserId1");

                    b.HasIndex("UserId2");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("DoAn4.Models.Friendship", b =>
                {
                    b.Property<Guid>("FriendshipId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("FriendStatus")
                        .HasColumnType("int");

                    b.Property<Guid>("FriendUserId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("FriendshipId");

                    b.HasIndex("FriendUserId");

                    b.HasIndex("UserId");

                    b.ToTable("Friendships");
                });

            modelBuilder.Entity("DoAn4.Models.Images", b =>
                {
                    b.Property<Guid>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ImageLink")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("PostId")
                        .HasColumnType("char(36)");

                    b.HasKey("ImageId");

                    b.HasIndex("PostId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("DoAn4.Models.Like", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("PostId")
                        .HasColumnType("char(36)");

                    b.Property<int>("React")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("DoAn4.Models.Message", b =>
                {
                    b.Property<Guid>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("ConversationsId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("RecipientId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("char(36)");

                    b.HasKey("MessageId");

                    b.HasIndex("ConversationsId");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("DoAn4.Models.Notify", b =>
                {
                    b.Property<Guid>("NotifyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("FriendShipId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsRead")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("NotifyContent")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("NotifyTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NotifyType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("PostId")
                        .HasColumnType("char(36)");

                    b.HasKey("NotifyId");

                    b.HasIndex("FriendShipId");

                    b.ToTable("Notifies");
                });

            modelBuilder.Entity("DoAn4.Models.Post", b =>
                {
                    b.Property<Guid>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Content")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("PostTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("TotalComment")
                        .HasColumnType("int");

                    b.Property<int>("TotalReact")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("UserPostId")
                        .HasColumnType("char(36)");

                    b.HasKey("PostId");

                    b.HasIndex("UserPostId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("DoAn4.Models.RefreshToken", b =>
                {
                    b.Property<Guid>("RefreshTokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("RefreshTokenId");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("DoAn4.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Avatar")
                        .HasColumnType("longtext");

                    b.Property<string>("Bio")
                        .HasColumnType("longtext");

                    b.Property<string>("CoverPhoto")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("Gender")
                        .HasMaxLength(1)
                        .HasColumnType("int");

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnType("tinyint(1)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DoAn4.Models.UserOTP", b =>
                {
                    b.Property<Guid>("UserOtpID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("UserOtpID");

                    b.ToTable("UserOTPs");
                });

            modelBuilder.Entity("DoAn4.Models.Video", b =>
                {
                    b.Property<Guid>("VideoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("PostId")
                        .HasColumnType("char(36)");

                    b.Property<string>("VideoLink")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("VideoId");

                    b.HasIndex("PostId");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("DoAn4.Models.AccessToken", b =>
                {
                    b.HasOne("DoAn4.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DoAn4.Models.Comment", b =>
                {
                    b.HasOne("DoAn4.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DoAn4.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DoAn4.Models.Conversations", b =>
                {
                    b.HasOne("DoAn4.Models.User", "User1")
                        .WithMany()
                        .HasForeignKey("UserId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DoAn4.Models.User", "User2")
                        .WithMany()
                        .HasForeignKey("UserId2")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("DoAn4.Models.Friendship", b =>
                {
                    b.HasOne("DoAn4.Models.User", "FriendUser")
                        .WithMany("UserReply")
                        .HasForeignKey("FriendUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DoAn4.Models.User", "CurrenUser")
                        .WithMany("UserReqest")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CurrenUser");

                    b.Navigation("FriendUser");
                });

            modelBuilder.Entity("DoAn4.Models.Images", b =>
                {
                    b.HasOne("DoAn4.Models.Post", "Post")
                        .WithMany("Images")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("DoAn4.Models.Like", b =>
                {
                    b.HasOne("DoAn4.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DoAn4.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DoAn4.Models.Message", b =>
                {
                    b.HasOne("DoAn4.Models.Conversations", "Conversations")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DoAn4.Models.User", "UserRep")
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DoAn4.Models.User", "UserSend")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Conversations");

                    b.Navigation("UserRep");

                    b.Navigation("UserSend");
                });

            modelBuilder.Entity("DoAn4.Models.Notify", b =>
                {
                    b.HasOne("DoAn4.Models.Friendship", "FriendShip")
                        .WithMany("FriendShipNotify")
                        .HasForeignKey("FriendShipId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FriendShip");
                });

            modelBuilder.Entity("DoAn4.Models.Post", b =>
                {
                    b.HasOne("DoAn4.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DoAn4.Models.RefreshToken", b =>
                {
                    b.HasOne("DoAn4.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DoAn4.Models.Video", b =>
                {
                    b.HasOne("DoAn4.Models.Post", "Post")
                        .WithMany("Videos")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("DoAn4.Models.Conversations", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("DoAn4.Models.Friendship", b =>
                {
                    b.Navigation("FriendShipNotify");
                });

            modelBuilder.Entity("DoAn4.Models.Post", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Videos");
                });

            modelBuilder.Entity("DoAn4.Models.User", b =>
                {
                    b.Navigation("UserReply");

                    b.Navigation("UserReqest");
                });
#pragma warning restore 612, 618
        }
    }
}
