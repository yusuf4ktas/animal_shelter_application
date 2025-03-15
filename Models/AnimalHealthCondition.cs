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

        [Column("chronic_diseasae")]
        public bool ChronicDisease { get; set; }

        // If you want a navigation property back to AnimalInformation
        public AnimalInformation? AnimalInformation { get; set; }
    }
}
