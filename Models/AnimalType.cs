﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace animal_shelter_app.Models
{
    [Table("animal_type")]
    public class AnimalType
    {
        [Key]
        [Column("animal_special_type_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] //To block auto generation to manually set the value
        public long AnimalSpecialTypeId { get; set; }

        [Column("animal_id")]
        public int? AnimalId { get; set; }

        [Column("animal_species")]
        [StringLength(50)]
        public string? AnimalSpecies { get; set; }

        [Column("life_expectancy")]
        public int LifeExpectancy { get; set; }

        //Navigation Property
        [ForeignKey("AnimalId")]
        public AnimalInformation? AnimalInformation { get; set; }
    }
}