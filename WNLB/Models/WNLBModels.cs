using NLBLib.Applications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WNLB.Misc;

namespace WNLB.Models
{
    public class WNLBContext : DbContext
    {
        public WNLBContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer<WNLBContext>(null);
        }

        public DbSet<Server> Servers { get; set; }
        public DbSet<Application> Applications { get; set; }
    }

    [Table("Server")]
    public class Server
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        public int ServerId { get; set; }

        [Required]
        [Display(Name = "Server Name")]
        [MaxLength(100, ErrorMessage = "Server name can't be longer than 100 chars.")]
        public string ServerName { get; set; }

        [Required]
        [Display(Name = "Hostname (FQDN)")]
        [MaxLength(1024, ErrorMessage = "Hostname can't be longer than 1024 chars.")]
        public string ServerHost { get; set; }

        [Required]
        [Range(1025, int.MaxValue, ErrorMessage="Port number must be greater than 1024")]
        public int Port { get; set; }

        [NotMapped]
        [ScaffoldColumn(false)]
        public bool IsAvailable { get; set; }

        public Server() { }
    }

    [Table("Application")]
    public class Application
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [ScaffoldColumn(false)]
        public int AppId { get; set; }

        [Required]
        [Display(Name = "Application Name")]
        [MaxLength(100, ErrorMessage="Applicatoin name can't be longer than 100 chars.")]
        public string AppName { get; set; }

        [Required]
        [Display(Name = "Path")]
        [MaxLength(1024, ErrorMessage = "Path can't be longer than 1024 chars.")]
        public string Path { get; set; }

        [Column]
        [Display(Name = "App Type")]
        public ApplicationType AppType { get; set; }

        [Column]
        [Display(Name = "Algorithm")]
        public int RoutingAlgorithm { get; set; }

        [ScaffoldColumn(false)]
        [NotMapped]
        public IEnumerable<SelectListItem> AppTypes { get; set; }

        [ScaffoldColumn(false)]
        [NotMapped]
        public IEnumerable<SelectListItem> RoutingAlgos { get; set; }

        public Application()
        {
            AppTypes = new List<SelectListItem>() {
                new SelectListItem() { Text = "Static", Value = "1", Selected = true },
                new SelectListItem() { Text = "Dynamic", Value = "2" }
            };

            RoutingAlgos = new List<SelectListItem>() {
                new SelectListItem() { Text = "Round Robin", Value = "1", Selected = true },
                new SelectListItem() { Text = "Weighted", Value = "2" },
                new SelectListItem() { Text = "IP-Hash", Value = "3" }
            };
        }
    }

    [TypeConverter(typeof(EnumToStringConverter))]
    public enum ApplicationType
    {
        RoundRobin,
        Wighted,
        IPHash
    }
}