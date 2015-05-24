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
        [Display(Name="Status")]
        [UIHint("Status")]
        public string Status { get; set; }

        public virtual ICollection<Application> Applications { get; set; }

        public Server() { }
    }

    [Table("Application")]
    public class Application : IValidatableObject
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
        public RoutingAlgo RoutingAlgorithm { get; set; }

        [Display(Name = "Distribute requests evenly amongst servers")]
        public bool DistributeEvenly { get; set; }

        [Display(Name = "Server Weights", Description="A comma seperate list of requests ratios per server")]
        [RegularExpression("^([1-9]+[0-9]*)$|^(([1-9]+[0-9]*,)+([1-9]+[0-9]*)+$)", ErrorMessage = "Please enter a list of comma seperated numbers")]
        public string Weights { get; set; }

        [Required]
        public virtual ICollection<Server> Servers { get; set; }

        public Application()
        {
            Servers = new List<Server>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RoutingAlgorithm == RoutingAlgo.Weighted && DistributeEvenly == false && (Weights == null || Weights.Length == 0))
            {
                yield return new ValidationResult("Specify weights for server or select even distribution", new [] { "Weights" });
            }
        }
    }

    public class AppServer
    {
        public int ServerId { get; set; }
        public string ServerName { get; set; }
        public bool IsSelected { get; set; }
    }

    [TypeConverter(typeof(EnumToStringConverter))]
    public enum ApplicationType
    {
        Static,
        Dynamic
    }

    [TypeConverter(typeof(EnumToStringConverter))]
    public enum RoutingAlgo
    {
        RoundRobin,
        Weighted,
        IPHash
    }
}