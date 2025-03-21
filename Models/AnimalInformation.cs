using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace animal_shelter_app.Models
{
    [Table("animal_information")]
    public class AnimalInformation
    {
        [Key]
        [Column("animal_id")]
        public int AnimalId { get; set; }

        [Column("animal_age")]
        public int AnimalAge { get; set; }

        [Column("animal_gender")]
        public bool AnimalGender { get; set; }

        [Column("neutering_status")]
        public bool NeuteringStatus { get; set; }

        [Column("characteristic_features")]
        public string? CharacteristicFeatures { get; set; }

        [Column("past_information")]
        public string? PastInformation { get; set; }

        [Column("arrival_date")]
        public DateTime? ArrivalDate { get; set; }

        [Column("animal_image")]
        public string AnimalImage { get; set; }


        [Column("species")]
        public string Species { get; set; }

        [Column("isAdopted")]
        public bool IsAdopted { get; set; } = false;


        public AnimalHealthCondition? HealthCondition { get; set; }

    }
}
