using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Jan25.Entities
{
#pragma warning disable
    /// <summary> 
    /// Represents a gender entity with essential details
    /// </summary>
    public class Gender
    {
        /// <summary>
        /// Primary key for the Gender 
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// TenantId of the Gender 
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Required field Name of the Gender 
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}