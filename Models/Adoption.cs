using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace animal_shelter_app.Models
{
    [Table("adoption")]
    public class Adoption
    {
        [Key]
        [Column("adoption_id")]
        public int AdoptionId { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("animal_id")]
        public int? AnimalId { get; set; }

        [Column("adoption_date")]
        public DateTime? AdoptionDate { get; set; }

        [Column("user_adress")]
        [StringLength(150)]
        public string? UserAddress { get; set; }

        //Navigation properties if you want to link to User or AnimalInformation
        public User? User { get; set; }
        public AnimalInformation? AnimalInformation { get; set; }
    }
}
