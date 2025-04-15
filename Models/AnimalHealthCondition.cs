using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace animal_shelter_app.Models
{
    [Table("animal_health_condition")]
    public class AnimalHealthCondition
    {
        [Key]
        [Column("animal_health_id")]
        public int AnimalHealthId { get; set; }

        [Column("animal_id")]
        public int? AnimalId { get; set; }

        [Column("health_check_up_date")]
        public DateTime? HealthCheckUpDate { get; set; }

        [Column("disability_status")]
        public bool DisabilityStatus { get; set; }

        // New property to store disability details text
        [Column("disability_details")]
        public string? DisabilityDetails { get; set; }

        [Column("chronic_diseasae_status")]
        public bool ChronicDiseaseStatus { get; set; }

        // New property to store disability details text
        [Column("chronic_disease_details")]
        public string? ChronicDiseaseDetails { get; set; }

        public AnimalInformation? AnimalInformation { get; set; }
    }
}