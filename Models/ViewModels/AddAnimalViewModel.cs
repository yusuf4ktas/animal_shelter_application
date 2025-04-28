using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace animal_shelter_app.Models.ViewModels
{
    public class AddAnimalViewModel
    {
        public int AnimalAge { get; set; }
        public bool AnimalGender { get; set; }
        public bool NeuteringStatus { get; set; }
        public string? Species { get; set; }
        public string? AnimalSpecies { get; set; }
        public int LifeExpectancy { get; set; }

        //Only for date attribute, no time requested
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
                   ApplyFormatInEditMode = true)]
        public DateTime ArrivalDate { get; set; }
        public string? CharacteristicFeatures { get; set; }
        public string? PastInformation { get; set; }
        public bool IsAdopted { get; set; }
        public IFormFile? ImageFile { get; set; }

        // Health condition properties

        //Only for date attribute, no time requested
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
           ApplyFormatInEditMode = true)]
        public DateTime HealthCheckUpDate { get; set; }
        public bool DisabilityStatus { get; set; }
        public string? DisabilityDetails { get; set; }
        public bool ChronicDiseaseStatus { get; set; }
        public string? ChronicDiseaseDetails { get; set; }

        // For the dropdown list
        public IEnumerable<SelectListItem>? SpeciesList { get; set; }
    }
}