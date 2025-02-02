using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Jan25.Entities
{
#pragma warning disable
    /// <summary> 
    /// Represents a qualification entity with essential details
    /// </summary>
    public class Qualification
    {
        /// <summary>
        /// Primary key for the Qualification 
        /// </summary>
        [Key]
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// TenantId of the Qualification 
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Required field Name of the Qualification 
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}